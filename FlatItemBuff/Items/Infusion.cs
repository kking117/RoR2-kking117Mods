using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using FlatItemBuff.Utils;
using UnityEngine;

namespace FlatItemBuff.Items
{
	public class Infusion
	{
		static BuffDef infusionTrackerBuff;
		public Infusion()
		{
			MainPlugin.ModLogger.LogInfo("Changing Infusion");
			UpdateText();
			Hooks();
			if(MainPlugin.Infusion_Tracker.Value)
            {
				CreateBuff();
            }
		}
		private void CreateBuff()
        {
			infusionTrackerBuff = Modules.Buffs.AddNewBuff("InfusionTracker", MainPlugin.LoadAsSprite(Properties.Resources.texInfusionTracker, 64), new Color(0.588f, 0.003f, 0.192f, 1f), true, false, true);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			LanguageAPI.Add("ITEM_INFUSION_PICKUP", string.Format("Kill enemies to collect samples, gaining enough will increase your level."));
			LanguageAPI.Add("ITEM_INFUSION_DESC", string.Format("Killing enough enemies increases your <style=cIsHealing>level</style>, for up to <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", Math.Floor((double)(MainPlugin.Infusion_Stacks.Value / MainPlugin.Infusion_Level.Value))));
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			MainPlugin.ModLogger.LogInfo("Changing stacking behaviour");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			MainPlugin.ModLogger.LogInfo("Changing proc behaviour");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			if (MainPlugin.Infusion_InheritOwner.Value)
			{
				On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
			}
			CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_OnInventoryChanged;
		}
		private void CharacterBody_OnInventoryChanged(CharacterBody self)
		{
			UpdateTracker(self);
		}
		private void UpdateTracker(CharacterBody body)
		{
			Inventory inv = body.inventory;
			if (inv)
			{
				int itemCount = inv.GetItemCount(RoR2Content.Items.Infusion);
				if (infusionTrackerBuff != null)
				{
					int infusioncap = MainPlugin.Infusion_Stacks.Value * itemCount;
					int percent = 0;
					if (inv.infusionBonus < infusioncap)
					{
						percent = (100 * (int)inv.infusionBonus) / infusioncap;
						percent = Math.Min(percent, 100);
						percent = Math.Max(percent, 0);
					}

					int buffCount = body.GetBuffCount(infusionTrackerBuff.buffIndex);
					while (buffCount != percent)
					{
						if (buffCount > percent)
						{
							body.RemoveBuff(infusionTrackerBuff.buffIndex);
							buffCount--;
						}
						else if (buffCount < percent)
						{
							body.AddBuff(infusionTrackerBuff.buffIndex);
							buffCount++;
						}
					}
				}
			}
		}
		private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
		{
			orig(self, body);
			if (!NetworkServer.active)
			{
				return;
			}
			MinionOwnership minionowner = self.minionOwnership;
			if (minionowner)
			{
				CharacterMaster truemaster = Helpers.GetTrueOwner(minionowner);
				if (truemaster)
				{
					if (truemaster.inventory)
					{
						self.inventory.infusionBonus += truemaster.inventory.infusionBonus - self.inventory.infusionBonus;
					}
				}
			}
		}
		private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			CharacterBody attacker = damageReport.attackerBody;
			if (attacker)
			{
				Inventory inventory = attacker.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
					if (itemCount > 0)
					{
						int healthgain = MainPlugin.Infusion_Fake_Bonus.Value;
						CharacterBody victimbody = damageReport.victimBody;
						if (victimbody.master)
						{
							healthgain = MainPlugin.Infusion_Kill_Bonus.Value;
							if (victimbody.isChampion)
							{
								healthgain = MainPlugin.Infusion_Champ_Bonus.Value;
							}
							if (victimbody.isBoss)
							{
								healthgain *= MainPlugin.Infusion_Boss_Bonus.Value;
							}
							if (victimbody.isElite)
							{
								healthgain *= MainPlugin.Infusion_Elite_Bonus.Value;
							}
						}
						if (healthgain > 0)
						{
							healthgain *= itemCount;
							CharacterMaster target = attacker.master;
							if (MainPlugin.Infusion_OwnerGains.Value)
							{
								MinionOwnership minionowner = attacker.master.minionOwnership;
								if (target)
								{
									if (minionowner)
									{
										target = Helpers.GetTrueOwner(minionowner);
										if (!target || !target.GetBody() || target.inventory.infusionBonus >= target.inventory.GetItemCount(RoR2Content.Items.Infusion) * MainPlugin.Infusion_Stacks.Value)
										{
											target = attacker.master;
										}
									}
								}
							}
							if (target.inventory.infusionBonus < target.inventory.GetItemCount(RoR2Content.Items.Infusion) * MainPlugin.Infusion_Stacks.Value)
							{
								GiveInfusionOrb(target.GetBody(), victimbody, healthgain);
							}
						}
					}
				}
			}
		}
		private void GiveInfusionOrb(CharacterBody target, CharacterBody victim,  int amount)
        {
			RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
			infusionOrb.origin = victim.gameObject.transform.position;
			infusionOrb.target = Util.FindBodyMainHurtBox(target);
			infusionOrb.maxHpValue = amount;
			RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 17),
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "Infusion"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount")
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 43)
			);
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			//I believe using IL to change this wouldn't be very fun
			//So instead I'll just make the code never run and apply my changes somewhere else
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "Infusion"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, 3)
			);
			ilcursor.Index -= 2;
			ilcursor.RemoveRange(5);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdarg(x, 0),
				x => ILPatternMatchingExt.MatchLdarg(x, 0),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "get_level"),
				x => ILPatternMatchingExt.MatchLdloc(x, 2),
				x => ILPatternMatchingExt.MatchConvR4(x),
				x => ILPatternMatchingExt.MatchAdd(x),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "set_level")
			);
			ilcursor.Index += 5;
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
			{
				if (self.inventory)
				{
					int itemCount = self.inventory.GetItemCount(RoR2Content.Items.Infusion);
					if (itemCount > 0)
					{
						float infusioncap = (MainPlugin.Infusion_Stacks.Value / MainPlugin.Infusion_Level.Value) * itemCount;
						float infusionlevels = (float)self.inventory.infusionBonus / (float)MainPlugin.Infusion_Level.Value;
						if (infusionlevels > infusioncap)
						{
							infusionlevels = infusioncap;
						}
						if (infusionlevels > 0.0f)
						{
							return infusionlevels;
						}
					}
				}
				return 0f;
			});
			ilcursor.Emit(OpCodes.Add);
		}
	}
}
