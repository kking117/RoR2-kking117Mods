using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FlatItemBuff.Items
{
	public class Infusion
	{
		internal static bool Enable = true;
		internal static float StackLevel = 2f;
		internal static bool Infinite = true;
		internal static bool Inherit = true;
		internal static int BossGainMult = 2;
		internal static int EliteGainMult = 3;
		internal static int ChampionGain = 5;
		public Infusion()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Infusion");
			UpdateItemDef();
			UpdateText();
			Hooks();
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/Infusion/Infusion.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.Utility);
				itemTags.Remove(ItemTag.Healing);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string level = string.Format("<style=cIsUtility>{0} <style=cStack>(+{0} per stack)</style></style>", StackLevel);

			string pickup = "Killing enemies give samples. Collected samples increases your level.";
			string desc = string.Format("Killing an enemy gives <style=cIsHealth>samples</style>, collecting enough will increase your level up to {0}.", level);
			LanguageAPI.Add("ITEM_INFUSION_PICKUP", pickup);
			LanguageAPI.Add("ITEM_INFUSION_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			MainPlugin.ModLogger.LogInfo("Changing stacking behaviour");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			MainPlugin.ModLogger.LogInfo("Changing proc behaviour");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			if (Inherit)
			{
				CharacterMaster.onStartGlobal += CharacterMaster_Start;
			}
		}
		private void CharacterMaster_Start(CharacterMaster self)
		{
			if (NetworkServer.active)
			{
				CharacterMaster owner = self.minionOwnership.ownerMaster;
				if (owner && owner != self)
				{
					if (owner.inventory.GetItemCount(RoR2Content.Items.Infusion) > 0)
					{
						self.inventory.infusionBonus = owner.inventory.infusionBonus;
					}
				}
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			Inventory inventory = attackerBody.inventory;
			if (inventory)
			{
				int itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
				if (itemCount > 0)
				{
					CharacterBody victimBody = damageReport.victimBody;
					float value = GetInfusionValue(victimBody) * itemCount;
					GiveInfusionOrb(attackerBody, victimBody, (int)Math.Ceiling(value));
				}
			}
		}
		private float GetInfusionValue(CharacterBody body)
		{
			float infusionValue = 1f;
			if (body.master)
			{
				if (body.isChampion)
                {
					infusionValue = ChampionGain;
				}
				if (body.isElite)
                {
					infusionValue *= EliteGainMult;
				}
				if (body.isBoss)
				{
					infusionValue *= BossGainMult;
				}
			}
			return infusionValue;
		}
		private void GiveInfusionOrb(CharacterBody target, CharacterBody victim,  int amount)
        {
			if (amount > 0)
			{
				RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
				infusionOrb.origin = victim.gameObject.transform.position;
				infusionOrb.target = Util.FindBodyMainHurtBox(target);
				infusionOrb.maxHpValue = amount;
				RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
			}
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
			);
			ilcursor.Index += 2;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
		}
		private void IL_RecalculateStats(ILContext il)
		{
			//Kill old function
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
			);
			ilcursor.Index -= 2;
			ilcursor.RemoveRange(5);
			//Add new
			ilcursor.GotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdarg(0),
				x => x.MatchCallOrCallvirt<CharacterBody>("get_level"),
				x => x.MatchLdloc(2),
				x => x.MatchConvR4(),
				x => x.MatchAdd(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_level")
			);
			ilcursor.Index += 5;
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
			{
				float levelBonus = 0f;
				Inventory inventory = self.inventory;
				if (inventory)
				{
					CharacterMaster master = self.master;
					int itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
					if (itemCount > 0)
					{
						float levelCost = 100f / StackLevel;
						float maxCap = itemCount * 100;
						float samples = inventory.infusionBonus;
						if (Infinite && samples > maxCap)
                        {
							levelBonus = maxCap / levelCost;
							float bonus = (float)Math.Pow(samples - maxCap, 0.95f);
							levelBonus += bonus / levelCost;
						}
						else
                        {
							levelBonus = Math.Min(samples, maxCap) / levelCost;
						}
					}
				}
				return levelBonus;
			});
			ilcursor.Emit(OpCodes.Add);
		}
	}
}
