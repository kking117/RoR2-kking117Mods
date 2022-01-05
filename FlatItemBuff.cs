using System;
using System.Linq;
using System.Collections.Generic;
using EntityStates;
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
	[BepInPlugin("com.kking117.FlatItemBuff", "FlatItemBuff", "1.2.0")]
	public class Base : BaseUnityPlugin
	{
		public static ConfigEntry<bool> Steak_Change;
		public static ConfigEntry<float> Steak_BaseHP;
		public static ConfigEntry<float> Steak_LevelMult;

		public static ConfigEntry<bool> Brooch_Change;
		public static ConfigEntry<float> Brooch_BaseFlatBarrier;
		public static ConfigEntry<float> Brooch_StackFlatBarrier;
		public static ConfigEntry<float> Brooch_BaseCentBarrier;
		public static ConfigEntry<float> Brooch_StackCentBarrier;

		public static ConfigEntry<bool> Squid_Change;
		public static ConfigEntry<bool> Squid_ClayHit;
		public static ConfigEntry<bool> Squid_AtkToDmg;
		public static ConfigEntry<bool> Squid_InactiveDecay;
		public static ConfigEntry<int> Squid_StackLife;
		public static ConfigEntry<float> Squid_Armor;

		public static ConfigEntry<bool> Infusion_Change;
		public static ConfigEntry<int> Infusion_Stacks;
		public static ConfigEntry<int> Infusion_Level;
		public static ConfigEntry<bool> Infusion_OldStack;
		public static ConfigEntry<bool> Infusion_OwnerGains;
		public static ConfigEntry<bool> Infusion_InheritOwner;

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

		private string Brooch_CatalogName = "BarrierOnKill";
		public void Awake()
        {
			ReadConfig();
			if (Steak_Change.Value)
			{
				Logger.LogInfo("Changing Bison Steak.");
				ChangeBisonSteak();
			}
			if (Brooch_Change.Value)
			{
				Logger.LogInfo("Changing Topaz Brooch.");
				ChangeBrooch();
			}
			if (Infusion_Change.Value)
			{
				Logger.LogInfo("Changing Infusion.");
				ChangeInfusion();
			}
			if (Squid_Change.Value)
			{
				Logger.LogInfo("Changing Squid Polyp.");
				ChangeSquidPolyp();
			}
		}
		private void ChangeSquidPolyp()
        {
			Logger.LogInfo("Altering Skill.");
			if (Squid_ClayHit.Value)
			{
				ModifySquidSkill();
			}
			On.RoR2.CharacterMaster.OnBodyStart += Squid_OnBodyStart;
			Logger.LogInfo("Changing descriptions.");
			string desc = "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies ";
			if (Squid_AtkToDmg.Value)
            {
				desc += "for <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> damage</style>";
			}
			else
            {
				desc += "at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>";
			}
			if (Squid_ClayHit.Value)
			{
				desc += " applying <style=cIsDamage>tar</style>.";
			}
			else
            {
				desc += ".";
            }
			if (Squid_StackLife.Value > 0)
			{
				desc = desc + " Lasts <style=cIsUtility>30</style> <style=cStack>(+" + Squid_StackLife.Value + " per stack)</style> seconds.";
			}
			else
			{
				desc = desc + " Lasts <style=cIsUtility>30</style> seconds.";
			}
			LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", desc);
		}
		private void ModifySquidSkill()
        {
			GameObject SquidPolyp = Resources.Load<GameObject>("prefabs/characterbodies/SquidTurretBody");
			RoR2.SkillLocator skills = SquidPolyp.GetComponent<RoR2.SkillLocator>();
			RoR2.Skills.SkillFamily primary_skills = skills.primary.skillFamily;
			if (primary_skills)
			{
				RoR2.Skills.SkillDef skill = primary_skills.variants[0].skillDef;
				if (skill)
				{
					skill.activationState = new SerializableEntityStateType(typeof(FlatItemBuff.FireSpine));
				}
			}
		}
		public void Squid_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, RoR2.CharacterMaster self, RoR2.CharacterBody body)
		{
			orig(self, body);
			//So as you can see this is a fairly unreliable and generally a bad way to do this
			//But the IL for the interaction part is a complete mess and I can't even get the stack count from there
			if (body.bodyIndex == RoR2.BodyCatalog.FindBodyIndex("SquidTurretBody"))
            {
				RoR2.Inventory inventory = body.inventory;
				if (inventory)
                {
					if (inventory.GetItemCount(RoR2.RoR2Content.Items.Ghost) == 0)
					{
						if (inventory.GetItemCount(RoR2.RoR2Content.Items.HealthDecay) == 30)
						{
							RoR2.MinionOwnership minionowner = self.minionOwnership;
							if (minionowner)
							{
								RoR2.CharacterMaster owner = GetOwner(minionowner);
								if (owner && owner != self)
								{
									int stacks = inventory.GetItemCount(RoR2.RoR2Content.Items.BoostAttackSpeed);
									int decay = (int)Math.Floor(30 + (stacks * Squid_StackLife.Value * 0.1f));
									if (Squid_AtkToDmg.Value)
									{
										inventory.GiveItem(RoR2.RoR2Content.Items.BoostDamage, stacks);
										inventory.RemoveItem(RoR2.RoR2Content.Items.BoostAttackSpeed, stacks);
									}
									inventory.RemoveItem(RoR2.RoR2Content.Items.HealthDecay, 30);
									inventory.GiveItem(RoR2.RoR2Content.Items.HealthDecay, decay);
									body.baseArmor = Squid_Armor.Value;
									if (Squid_InactiveDecay.Value)
                                    {
										body.gameObject.AddComponent<Squid_Manager>();
									}
								}
							}
						}
					}
				}
            }
		}
		private void ChangeBisonSteak()
        {
			Logger.LogInfo("Applying IL modifications.");
			Logger.LogInfo("Changing Stack Behaviour.");
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
		private void ChangeBrooch()
		{
			Logger.LogInfo("Applying IL modifications.");
			Logger.LogInfo("Overriding behaviour.");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(BroochDeathILChange);
			RoR2.GlobalEventManager.onCharacterDeathGlobal += OnBroochProc;
			Logger.LogInfo("Changing descriptions.");
			string desc = "Gain a <style=cIsHealing>temporary barrier</style> on kill for ";
			if (Brooch_BaseFlatBarrier.Value > 0f || Brooch_StackFlatBarrier.Value > 0f)
            {
				desc += "<style=cIsHealing>" + Brooch_BaseFlatBarrier.Value + " health <style=cStack>(+" + Brooch_StackFlatBarrier.Value + " per stack)</style></style>";
            }

			if (Brooch_BaseCentBarrier.Value > 0f || Brooch_StackCentBarrier.Value > 0f)
			{
				if (Brooch_BaseFlatBarrier.Value > 0f || Brooch_StackFlatBarrier.Value > 0f)
                {
					desc += " plus an additional ";

				}
				desc += "<style=cIsHealing>" + Brooch_BaseCentBarrier.Value * 100f + "% <style=cStack>(+" + Brooch_StackCentBarrier.Value * 100f + "% per stack)</style></style> of <style=cIsHealing>maximum health</style>";
			}
			desc += ".";
			LanguageAPI.Add("ITEM_BARRIERONKILL_DESC", desc);
		}
		private void BroochDeathILChange(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 15),
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", Brooch_CatalogName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<RoR2.Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, 40),
				x => ILPatternMatchingExt.MatchLdloc(x, 40)
			);
			ilcursor.Index += 4;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldc_I4, 0);
		}
		private void OnBroochProc(RoR2.DamageReport report)
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
					int brooch = inventory.GetItemCount(RoR2.RoR2Content.Items.BarrierOnKill);
					if (brooch > 0)
					{
						brooch--;
						float barrier = attacker.healthComponent.fullCombinedHealth * (Brooch_BaseCentBarrier.Value + (Brooch_StackCentBarrier.Value * brooch));
						barrier += Brooch_BaseFlatBarrier.Value + (Brooch_StackFlatBarrier.Value * brooch);
						attacker.healthComponent.AddBarrier(barrier);
					}
				}
			}
		}
		private void ChangeInfusion()
		{
			Logger.LogInfo("Applying IL modifications.");
			Logger.LogInfo("Overriding Stat Behaviour.");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(InfusionRecalcILChange);
			Logger.LogInfo("Overriding stack cap.");
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
							if (Infusion_OwnerGains.Value)
							{
								RoR2.MinionOwnership minionowner = attacker.master.minionOwnership;
								if (target)
								{
									if (minionowner)
									{
										target = GetTrueOwner(minionowner);
										if (!target || !target.GetBody() || target.inventory.infusionBonus >= target.inventory.GetItemCount(RoR2.RoR2Content.Items.Infusion) * Infusion_Stacks.Value)
										{
											target = attacker.master;
										}
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
		}
		private void InfusionDeathILChange(ILContext il)
        {
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
			if (Infusion_InheritOwner.Value)
			{
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
		private RoR2.CharacterMaster GetOwner(RoR2.MinionOwnership minionowner)
		{
			RoR2.CharacterMaster returnmaster = minionowner.ownerMaster;
			if (minionowner.ownerMaster)
			{
				minionowner = returnmaster.minionOwnership;
				if (minionowner.ownerMaster)
				{
					returnmaster = minionowner.ownerMaster;
				}
			}
			return returnmaster;
		}
		public void ReadConfig()
		{
			Steak_Change = Config.Bind<bool>(new ConfigDefinition("Bison Steak", "Enable Changes"), true, new ConfigDescription("Enables changes to Bison Steak.", null, Array.Empty<object>()));
			Steak_BaseHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base HP"), 25.0f, new ConfigDescription("The amount of HP each stack increases.", null, Array.Empty<object>()));
			Steak_LevelMult = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Level HP"), 0.1f, new ConfigDescription("Inceases the total HP gained per level. (0.2 is what Cautious Slug uses for its regen.)", null, Array.Empty<object>()));

			Brooch_Change = Config.Bind<bool>(new ConfigDefinition("Topaz Brooch", "Enable Changes"), true, new ConfigDescription("Enables changes to Topaz Brooch.", null, Array.Empty<object>()));
			Brooch_BaseFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier each stack after the first gives.", null, Array.Empty<object>()));
			Brooch_BaseCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier each stack after the first gives.", null, Array.Empty<object>()));

			Squid_Change = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Enable Changes"), true, new ConfigDescription("Enables changes to Squid Polyp.", null, Array.Empty<object>()));
			Squid_ClayHit = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Apply Tar"), true, new ConfigDescription("Makes Squid Polyp's apply the Tar debuff with their main attack.", null, Array.Empty<object>()));
			Squid_AtkToDmg = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Attack Speed to Damage"), true, new ConfigDescription("Converts the stacking attack speed bonus into a damage bonus instead.", null, Array.Empty<object>()));
			Squid_InactiveDecay = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Inactive Removal"), true, new ConfigDescription("Makes Squid Polyp's unable to heal if they haven't attacked or been hit in a while. (Useful to remove useless Squids on the otherside of the map.)", null, Array.Empty<object>()));
			Squid_StackLife = Config.Bind<int>(new ConfigDefinition("Squid Polyp", "Lifetime Per Stack"), 3, new ConfigDescription("Increases the lifespan of the Squid Polyp by this much in seconds per stack.", null, Array.Empty<object>()));
			Squid_Armor = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Armor"), 50, new ConfigDescription("How much Armor summoned Squid Polyps have.", null, Array.Empty<object>()));

			Infusion_Change = Config.Bind<bool>(new ConfigDefinition("Infusion", "Enable Changes"), true, new ConfigDescription("Enables changes to Infusion.", null, Array.Empty<object>()));
			Infusion_Stacks = Config.Bind<int>(new ConfigDefinition("Infusion", "Max Stacks"), 200, new ConfigDescription("How many stacks an infusion has (100 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Level = Config.Bind<int>(new ConfigDefinition("Infusion", "Level Per Stacks"), 100, new ConfigDescription("How many stacks are needed to gain a level up.", null, Array.Empty<object>()));
			Infusion_OldStack = Config.Bind<bool>(new ConfigDefinition("Infusion", "Normal Stat Gain"), true, new ConfigDescription("When set to True, each stack will increase stats as you gain them instead of only when you reach Level Per Stacks.", null, Array.Empty<object>()));
			Infusion_OwnerGains = Config.Bind<bool>(new ConfigDefinition("Infusion", "Gives To Owner"), true, new ConfigDescription("When set to True, if a minion with infusions kills an enemy it gives the stack to their owner if they can still collect stacks.", null, Array.Empty<object>()));
			Infusion_InheritOwner = Config.Bind<bool>(new ConfigDefinition("Infusion", "Inherit From Owner"), true, new ConfigDescription("When set to True, minions with infusions will inherit their owner's collected stacks", null, Array.Empty<object>()));

			Infusion_Fake_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Fake Stack"), 1, new ConfigDescription("How many stacks killing certain non-ai and non-player enemies give. (1 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Kill_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Kill Stack"), 2, new ConfigDescription("How many stacks killing a normal enemy gives (1 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Champ_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Champion Stack"), 6, new ConfigDescription("How many stacks killing a champion enemy gives (1 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Elite_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Elite Bonus"), 3, new ConfigDescription("Stack multiplier for killing elites.", null, Array.Empty<object>()));
			Infusion_Boss_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Boss Bonus"), 2, new ConfigDescription("Stack multiplier for killing bosses.", null, Array.Empty<object>()));
		}
	}
	public class Squid_Manager : MonoBehaviour
	{
		private RoR2.CharacterBody body;
		private float gracetime;

		private void Awake()
		{
			body = GetComponent<RoR2.CharacterBody>();
			gracetime = 7f;
		}
		private void FixedUpdate()
		{
			if (!body)
			{
				Destroy(this);
			}
			if(body.outOfCombat && body.outOfDanger)
            {
				if (gracetime > 0f)
				{
					gracetime -= Time.fixedDeltaTime;
				}
				else
				{
					body.AddTimedBuff(RoR2.RoR2Content.Buffs.HealingDisabled, 0.5f);
				}
            }
			else
            {
				gracetime = 7f;
			}
		}
	}
	public class FireSpine : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			GetAimRay();
			PlayAnimation("Gesture", "FireGoo");
			if (isAuthority)
			{
				FireOrbArrow();
			}
		}
		private void FireOrbArrow()
		{
			if (this.hasFiredArrow || !NetworkServer.active)
			{
				return;
			}
			Ray aimRay = GetAimRay();
			enemyFinder = new RoR2.BullseyeSearch();
			enemyFinder.viewer = characterBody;
			enemyFinder.maxDistanceFilter = float.PositiveInfinity;
			enemyFinder.searchOrigin = aimRay.origin;
			enemyFinder.searchDirection = aimRay.direction;
			enemyFinder.sortMode = RoR2.BullseyeSearch.SortMode.Distance;
			enemyFinder.teamMaskFilter = RoR2.TeamMask.allButNeutral;
			enemyFinder.minDistanceFilter = 0f;
			enemyFinder.maxAngleFilter = (fullVision ? 180f : 90f);
			enemyFinder.filterByLoS = true;
			if (teamComponent)
			{
				enemyFinder.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
			}
			enemyFinder.RefreshCandidates();
			RoR2.HurtBox hurtBox = Enumerable.FirstOrDefault<RoR2.HurtBox>(enemyFinder.GetResults());
			if (hurtBox)
			{
				Vector3 vector = hurtBox.transform.position - GetAimRay().origin;
				aimRay.origin = GetAimRay().origin;
				aimRay.direction = vector;
				inputBank.aimDirection = vector;
				StartAimMode(aimRay, 2f, false);
				hasFiredArrow = true;
				RoR2.Orbs.SquidOrb squidOrb = new RoR2.Orbs.SquidOrb();
				squidOrb.damageValue = characterBody.damage * damageCoefficient;
				squidOrb.isCrit = RoR2.Util.CheckRoll(characterBody.crit, characterBody.master);
				squidOrb.teamIndex = RoR2.TeamComponent.GetObjectTeam(gameObject);
				squidOrb.attacker = gameObject;
				squidOrb.procCoefficient = procCoefficient;
				squidOrb.damageType = DamageType.ClayGoo;
				RoR2.HurtBox hurtBox2 = hurtBox;
				if (hurtBox2)
				{
					Transform transform = characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Muzzle");
					RoR2.EffectManager.SimpleMuzzleFlash(FireSpine.muzzleflashEffectPrefab, gameObject, "Muzzle", true);
					squidOrb.origin = transform.position;
					squidOrb.target = hurtBox2;
					RoR2.Orbs.OrbManager.instance.AddOrb(squidOrb);
				}
			}
		}
		public override void OnExit()
		{
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject hitEffectPrefab = EntityStates.Squid.SquidWeapon.FireSpine.hitEffectPrefab;
		public static GameObject muzzleflashEffectPrefab = EntityStates.Squid.SquidWeapon.FireSpine.muzzleflashEffectPrefab;

		public static float damageCoefficient = 1f;
		public static float procCoefficient = 1f;
		public static float baseDuration = 0.75f; //was 2f originally, but that seems way too slow, also tried 1f slow again, tried 0.75f as that's what the wiki states

		private const float maxVisionDistance = float.PositiveInfinity;
		public bool fullVision = true;

		private bool hasFiredArrow;
		//private ChildLocator childLocator;
		private RoR2.BullseyeSearch enemyFinder;
		private float duration;
	}
}
