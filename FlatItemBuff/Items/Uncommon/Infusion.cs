using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FlatItemBuff.Items
{
	public class Infusion
	{
		public static BuffDef InfusionTrackerBuff;
		private static Color BuffColor = new Color(0.918f, 0.408f, 0.42f, 1f);
		private const string LogName = "Infusion";
		internal static bool Enable = false;
		internal static float StackLevel = 2f;
		internal static bool Infinite = true;
		internal static bool Inherit = true;
		internal static int BossGainMult = 2;
		internal static int EliteGainMult = 3;
		internal static int ChampionGain = 5;
		internal static bool Comp_AssistManager = true;
		internal static bool BuffEnable = true;
		internal static bool BuffGrowth = true;
		public Infusion()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateItemDef();
			CreateBuffs();
			UpdateText();
			Hooks();
			if (MainPlugin.AssistManager_Loaded)
			{
				ApplyAssistManager();
			}
		}
		private void ApplyAssistManager()
		{
			AssistManager.VanillaTweaks.Infusion.Instance.SetEnabled(false);
			if (Comp_AssistManager)
			{
				AssistManager.AssistManager.HandleAssistInventoryActions += AssistManger_OnKill;
			}
		}
		private void ClampConfig()
		{
			StackLevel = Math.Max(0f, StackLevel);
			BossGainMult = Math.Max(0, BossGainMult);
			EliteGainMult = Math.Max(0, EliteGainMult);
			ChampionGain = Math.Max(0, ChampionGain);
		}
		private void CreateBuffs()
		{
			if (BuffEnable)
            {
				Sprite buffIcon = MainPlugin.assetBundle.LoadAsset<Sprite>("texInfusionTracker");
				InfusionTrackerBuff = Utils.ContentManager.AddBuff("InfusionTracker", buffIcon, BuffColor, true, false, false, false, !BuffGrowth);
			}
		}
		private void UpdateItemDef()
		{
			//"RoR2/Base/Infusion/Infusion.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("3d4d9c061a4d2b240bc519497506b247").WaitForCompletion();
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
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string level = string.Format("<style=cIsUtility>{0} <style=cStack>(+{0} per stack)</style></style>", StackLevel);

			string pickup = "Killing enemies give samples. Collected samples increases your level.";
			string desc = string.Format("Killing an enemy gives <style=cIsHealth>samples</style>, collecting enough will increase your level up to {0}.", level);
			LanguageAPI.Add("ITEM_INFUSION_PICKUP", pickup);
			LanguageAPI.Add("ITEM_INFUSION_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
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
		private void AssistManger_OnKill(AssistManager.AssistManager.Assist assist, Inventory assistInventory, CharacterBody killerBody, DamageInfo damageInfo)
		{
			CharacterBody assistBody = assist.attackerBody;
			if (assistBody == killerBody)
			{
				return;
			}

			int itemCount = assistInventory.GetItemCount(RoR2Content.Items.Infusion);
			if (itemCount > 0)
			{
				CharacterBody victimBody = assist.victimBody;
				float value = GetInfusionValue(victimBody) * itemCount;
				GiveInfusionOrb(assistBody, victimBody, (int)Math.Ceiling(value));
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

		private float GetInfusionLevel(CharacterBody body)
        {
			float levelBonus = 0f;
			Inventory inventory = body.inventory;
			if (inventory)
			{
				int itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
				if (itemCount > 0)
				{
					float levelCost = 100f / StackLevel;
					float maxCap = itemCount * 100;
					float samples = inventory.infusionBonus;
					if (Infinite && samples > maxCap)
					{
						levelBonus = maxCap / levelCost;
						float capReduction = samples / maxCap;
						levelBonus += ((samples - maxCap) / capReduction) / levelCost;
					}
					else
					{
						levelBonus = Math.Min(samples, maxCap) / levelCost;
					}
				}

				int levelFlat = (int)Math.Floor(levelBonus);
				Components.InfusionTracker comp = body.GetComponent<Components.InfusionTracker>();
				if (!comp)
				{
					comp = body.gameObject.AddComponent<Components.InfusionTracker>();
				}
				else
                {
					comp.negateLevelBonus = false;
				}
				if (levelFlat > comp.recordLevel)
				{
					comp.recordLevel = levelFlat;
				}
				else if (levelFlat != comp.lastLevel)
                {
					comp.negateLevelBonus = true;
				}
				comp.lastLevel = levelFlat;
			}
			if (BuffEnable)
			{
				int buffCount = (int)Math.Floor(levelBonus);
				if (body.GetBuffCount(InfusionTrackerBuff) != buffCount)
                {
					body.SetBuffCount(InfusionTrackerBuff.buffIndex, buffCount);
				}
			}
			return levelBonus;
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
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnCharacterDeath - Hook failed");
			}
		}
		private void IL_RecalculateStats(ILContext il)
		{
			//Kill old function
			//Note: Using R2API's addlevel stat hook, does not increase the actual level but does give level up stats.
			//For this reason we cannot use that and must use IL for this.
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
			))
            {
				ilcursor.Index -= 2;
				ilcursor.RemoveRange(5);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats A - Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdarg(0),
				x => x.MatchCallOrCallvirt<CharacterBody>("get_level"),
				x => x.MatchLdloc(3),
				x => x.MatchConvR4(),
				x => x.MatchAdd(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_level")
			))
			{
				ilcursor.Index += 5;
				ilcursor.Emit(OpCodes.Ldarg_0);


				ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					float levelBonus = GetInfusionLevel(self);
					return levelBonus;
				});
				ilcursor.Emit(OpCodes.Add);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats B - Hook failed");
			}

			//Prevents on level up abuse
			if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdloc(1),
				x => x.MatchLdarg(0),
				x => x.MatchCallOrCallvirt<CharacterBody>("get_level")
			))
			{
				ilcursor.Index += 4;
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					Components.InfusionTracker comp = self.GetComponent<Components.InfusionTracker>();
					if (comp)
					{
						if (comp.negateLevelBonus)
						{
							comp.negateLevelBonus = false;
							return 0f;
						}
					}
					return 1f;
				});
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats C - Hook failed");
			}
		}
	}
}
