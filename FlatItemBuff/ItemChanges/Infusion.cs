using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using FlatItemBuff.Utils;

namespace FlatItemBuff.ItemChanges
{
	public class Infusion
	{
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Infusion");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			LanguageAPI.Add("ITEM_INFUSION_PICKUP", string.Format("Killing an enemy permanently increases your stats, up to {0} levels worth of stats.", Math.Floor((double)(MainPlugin.Infusion_Stacks.Value / MainPlugin.Infusion_Level.Value))));
			LanguageAPI.Add("ITEM_INFUSION_DESC", string.Format("Killing an enemy increases your <style=cIsHealing>stats permanently</style>, up to <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style> levels worth.", Math.Floor((double)(MainPlugin.Infusion_Stacks.Value / MainPlugin.Infusion_Level.Value))));
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			MainPlugin.ModLogger.LogInfo("Changing stacking behaviour");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			MainPlugin.ModLogger.LogInfo("Changing proc behaviour");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			if (MainPlugin.Infusion_InheritOwner.Value)
			{
				On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
			}
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
				if (itemCount > 0)
				{
					float infusioncap = (MainPlugin.Infusion_Stacks.Value / MainPlugin.Infusion_Level.Value) * itemCount;
					float infusionlevels = (float)sender.inventory.infusionBonus / (float)MainPlugin.Infusion_Level.Value;
					if (infusionlevels > infusioncap)
					{
						infusionlevels = infusioncap;
					}
					if (infusionlevels > 0.000f)
					{
						args.baseHealthAdd += sender.levelMaxHealth * infusionlevels;
						args.baseDamageAdd += sender.levelDamage * infusionlevels;
						args.baseRegenAdd += sender.levelRegen * infusionlevels;
						args.baseMoveSpeedAdd += sender.levelMoveSpeed * infusionlevels;
						args.baseShieldAdd += sender.levelMaxShield * infusionlevels;
						args.baseAttackSpeedAdd += sender.levelAttackSpeed * infusionlevels;
						args.armorAdd += sender.levelArmor * infusionlevels;
						args.critAdd += sender.levelCrit * infusionlevels;
						args.jumpPowerMultAdd += sender.levelJumpPower * infusionlevels;
					}
				}
			}
		}
		private static void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
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
						self.inventory.AddInfusionBonus(truemaster.inventory.infusionBonus - self.inventory.infusionBonus);
					}
				}
			}
		}
		private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
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
		private static void GiveInfusionOrb(CharacterBody target, CharacterBody victim,  int amount)
        {
			RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
			infusionOrb.origin = victim.gameObject.transform.position;
			infusionOrb.target = Util.FindBodyMainHurtBox(target);
			infusionOrb.maxHpValue = amount;
			RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
		}
		private static void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 16),
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "Infusion"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount")
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 38)
			);
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			//I believe using IL to change this wouldn't be very fun
			//So instead I'll just make the code never run and apply my changes somewhere else
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "Infusion"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, 3)
			);
			ilcursor.Index -= 2;
			ilcursor.RemoveRange(5);
		}
	}
}
