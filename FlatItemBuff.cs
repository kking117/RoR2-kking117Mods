using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using IL.RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.RoR2;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.OkIGotIt.Fresh_Bison_Steak", BepInDependency.DependencyFlags.SoftDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI"
	})]
	[BepInPlugin("com.kking117.FlatItemBuff", "FlatItemBuff", "1.1.0")]
	public class Base : BaseUnityPlugin
	{
		public static ConfigEntry<bool> Steak_Change;
		public static ConfigEntry<float> Steak_BaseHP;
		public static ConfigEntry<float> Steak_LevelMult;

		public static ConfigEntry<bool> Infusion_Change;
		public static ConfigEntry<int> Infusion_Stacks;
		public static ConfigEntry<int> Infusion_Level;
		public static ConfigEntry<bool> Infusion_OldStack;

		public static ConfigEntry<int> Infusion_Fake_Bonus;
		public static ConfigEntry<int> Infusion_Kill_Bonus;
		public static ConfigEntry<int> Infusion_Champ_Bonus;
		public static ConfigEntry<int> Infusion_Elite_Bonus;
		public static ConfigEntry<int> Infusion_Boss_Bonus;

		//IL Search Stuff
		private string Steak_CatalogName = "FlatHealth";
		private int Steak_LocationOffset = 2;
		private int Steak_Location = 35;

		private string Infusion_CatalogName = "Infusion";
		private int Infusion_Stack_Location = 40;
		private int Infusion_Location = 3;
		public void Awake()
        {
			ReadConfig();
			if (Steak_Change.Value)
			{
				Logger.LogInfo("Changing Bison Steak.");
				ChangeBisonSteak();
			}
			if (Infusion_Change.Value)
			{
				Logger.LogInfo("Changing Infusion.");
				ChangeInfusion();
			}
		}
		private void ChangeBisonSteak()
        {
			Logger.LogInfo("Applying IL modifications.");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(BisonSteakILChange);
			Logger.LogInfo("Changing descriptions.");
			if (Chainloader.PluginInfos.ContainsKey("com.OkIGotIt.Fresh_Bison_Steak"))
			{
				LanguageAPI.Add("ITEM_FLATHEALTH_NAME", string.Format("Fresh Bison Steak"));
				LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", string.Format("Gain <style=cIsHealing>{0}</style> max health. <style=cIsHealing>Regenerate health</style> after killing an enemy.", Steak_BaseHP.Value));
				LanguageAPI.Add("ITEM_FLATHEALTH_DESC", string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>. Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+2 hp/s</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> after killing an enemy.", Steak_BaseHP.Value));
			}
			else
            {
				LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", string.Format("Gain <style=cIsHealing>{0}</style> max health.", Steak_BaseHP.Value));
				LanguageAPI.Add("ITEM_FLATHEALTH_DESC", string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", Steak_BaseHP.Value));
			}
		}
		private void BisonSteakILChange(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", Steak_CatalogName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<RoR2.Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, Steak_Location)
			);
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, Steak_Location)
			});
			ilcursor.Index += Steak_LocationOffset;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<RoR2.CharacterBody, float>>((bs) =>
			{
				return Steak_BaseHP.Value * (1f + (bs.level - 1f) * Steak_LevelMult.Value);
			});
		}
		private void ChangeInfusion()
		{
			Logger.LogInfo("Applying IL modifications.");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(InfusionRecalcILChange);
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(InfusionDeathILChange);
			RoR2.GlobalEventManager.onCharacterDeathGlobal += OnInfusionProc;
			On.RoR2.CharacterMaster.OnBodyStart += OnCharacterMasterBodyStart_Infusion;
			RecalculateStatsAPI.GetStatCoefficients += CalcInfusionStats;
			Logger.LogInfo("Changing descriptions.");
			if (Infusion_OldStack.Value)
			{
				LanguageAPI.Add("ITEM_INFUSION_PICKUP", string.Format("Kill enemies to collect samples. Collecting samples grants a level worth of stat ups.", Steak_BaseHP.Value));
				LanguageAPI.Add("ITEM_INFUSION_DESC", string.Format("Kill enemies to collect samples. Samples grant a <style=cIsUtility>level worth</style> of <style=cIsHealing>stat increases</style> up to <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style> levels.", Math.Floor((double)(Infusion_Stacks.Value / Infusion_Level.Value))));
			}
			else
            {
				LanguageAPI.Add("ITEM_INFUSION_PICKUP", string.Format("Kill enemies to collect samples. Collecting enough grants a level worth of stat ups.", Steak_BaseHP.Value));
				LanguageAPI.Add("ITEM_INFUSION_DESC", string.Format("Kill enemies to collect samples. Enough samples grant a <style=cIsUtility>level worth</style> of <style=cIsHealing>stat increases</style> up to <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style> levels.", Math.Floor((double)(Infusion_Stacks.Value / Infusion_Level.Value))));
			}
		}
		private void OnInfusionProc(RoR2.DamageReport report)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			RoR2.CharacterBody attacker = report.attackerBody;
			if (attacker)
			{
				RoR2.Inventory inventory = attacker.inventory;
				if (inventory)
				{
					int infusions = inventory.GetItemCount(RoR2.RoR2Content.Items.Infusion);
					if (infusions > 0)
					{
						int healthgain = Infusion_Fake_Bonus.Value;
						RoR2.CharacterBody victimbody = report.victimBody;
						if (victimbody.master)
						{
							healthgain = Infusion_Kill_Bonus.Value;
							if (victimbody.isChampion)
							{
								healthgain = Infusion_Champ_Bonus.Value;
							}
							if (victimbody.isBoss)
							{
								healthgain *= Infusion_Boss_Bonus.Value;
							}
							if (victimbody.isElite)
							{
								healthgain *= Infusion_Elite_Bonus.Value;
							}
						}
						if (healthgain > 0)
						{
							RoR2.CharacterMaster target = attacker.master;
							RoR2.MinionOwnership minionowner = attacker.master.minionOwnership;
							if (target)
							{
								if (minionowner)
								{
									target = GetTrueOwner(minionowner);
									if (!target || !target.GetBody())
                                    {
										target = attacker.master;
                                    }
								}
							}
							if (target.inventory.infusionBonus < target.inventory.GetItemCount(RoR2.RoR2Content.Items.Infusion) * Infusion_Stacks.Value)
                            {
								healthgain *= infusions;
								RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
								infusionOrb.origin = victimbody.gameObject.transform.position;
								infusionOrb.target = RoR2.Util.FindBodyMainHurtBox(target.GetBody());
								infusionOrb.maxHpValue = healthgain;
								RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
							}
						}
					}
				}
			}
		}
		private void InfusionRecalcILChange(ILContext il)
		{
			Logger.LogInfo("Overriding Stat Behaviour.");
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", Infusion_CatalogName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<RoR2.Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, Infusion_Location)
			);
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, Infusion_Location)
			});
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, Infusion_Stack_Location)
			});
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldc_I4, 0);
			Logger.LogInfo("Overriding Collection Behaviour.");
		}
		private void InfusionDeathILChange(ILContext il)
        {
			Logger.LogInfo("Overriding stack cap.");
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 36),
				x => ILPatternMatchingExt.MatchLdcI4(x, 100),
				x => ILPatternMatchingExt.MatchMul(x),
				x => ILPatternMatchingExt.MatchStloc(x, 50)
			);
			ilcursor.Index += 1;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldc_I4, 0);
			//I think using IL for this would be very difficult to do the way I want it to behave.
			//So instead I'll just make the code non-functional and add my code in an event.
		}
		private void CalcInfusionStats(RoR2.CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int infusions = sender.inventory.GetItemCount(RoR2.RoR2Content.Items.Infusion);
				if (infusions > 0)
                {
					float infusioncap = (Infusion_Stacks.Value / Infusion_Level.Value) * infusions;
					float infusionlevels;
					if (Infusion_OldStack.Value)
                    {
						infusionlevels = (float)sender.inventory.infusionBonus / (float)Infusion_Level.Value;
					}
					else
                    {
						infusionlevels = sender.inventory.infusionBonus / Infusion_Level.Value;
					}
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

		private void OnCharacterMasterBodyStart_Infusion(On.RoR2.CharacterMaster.orig_OnBodyStart orig, RoR2.CharacterMaster self, RoR2.CharacterBody body)
        {
			orig(self, body);
			if (self)
			{
				RoR2.MinionOwnership minionowner = self.minionOwnership;
				if (minionowner)
				{
					RoR2.CharacterMaster truemaster = GetTrueOwner(minionowner);
					if (truemaster)
					{
						if (truemaster.inventory)
						{
							self.inventory.AddInfusionBonus(truemaster.inventory.infusionBonus - self.inventory.infusionBonus);
						}
					}
				}
			}
		}
		private RoR2.CharacterMaster GetTrueOwner(RoR2.MinionOwnership minionowner)
        {
			RoR2.CharacterMaster returnmaster = minionowner.ownerMaster;
			if (minionowner.ownerMaster)
			{
				do
				{
					minionowner = returnmaster.minionOwnership;
					if (minionowner.ownerMaster)
					{
						returnmaster = minionowner.ownerMaster;
					}
				} while (minionowner.ownerMaster);
			}
			return returnmaster;
		}
		public void ReadConfig()
		{
			Steak_Change = Config.Bind<bool>(new ConfigDefinition("Bison Steak", "Enable Changes"), true, new ConfigDescription("Enables changes to Bison Steak.", null, Array.Empty<object>()));
			Steak_BaseHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base HP"), 25.0f, new ConfigDescription("The amount of HP each stack increases.", null, Array.Empty<object>()));
			Steak_LevelMult = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Level HP"), 0.2f, new ConfigDescription("Inceases the total HP gained per level. (0.2 is what Cautious Slug uses)", null, Array.Empty<object>()));

			Infusion_Change = Config.Bind<bool>(new ConfigDefinition("Infusion", "Enable Changes"), true, new ConfigDescription("Enables changes to Infusion.", null, Array.Empty<object>()));
			Infusion_Stacks = Config.Bind<int>(new ConfigDefinition("Infusion", "Max Stacks"), 200, new ConfigDescription("How many stacks an infusion has (100 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Level = Config.Bind<int>(new ConfigDefinition("Infusion", "Level Per Stacks"), 100, new ConfigDescription("How many stacks are needed to gain a level up.", null, Array.Empty<object>()));
			Infusion_OldStack = Config.Bind<bool>(new ConfigDefinition("Infusion", "Normal Stat Gain"), true, new ConfigDescription("When set to True, each stack will increase stats as you gain them instead of only when you reach Level Per Stacks.", null, Array.Empty<object>()));

			Infusion_Fake_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Fake Stack"), 1, new ConfigDescription("How many stacks killing certain non-ai and non-player enemies give. (1 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Kill_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Kill Stack"), 2, new ConfigDescription("How many stacks killing a normal enemy gives (1 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Champ_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Champion Stack"), 6, new ConfigDescription("How many stacks killing a champion enemy gives (1 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Elite_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Elite Bonus"), 3, new ConfigDescription("Stack multiplier for killing elites.", null, Array.Empty<object>()));
			Infusion_Boss_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Boss Bonus"), 2, new ConfigDescription("Stack multiplier for killing bosses.", null, Array.Empty<object>()));
		}
	}
}
