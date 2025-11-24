using System;
using System.IO;
using RoR2;
using R2API;
using FlatItemBuff.Items;
using UnityEngine;
using BepInEx.Configuration;

namespace FlatItemBuff
{
	//99% of this is practically ripped off from RiskyMod, ain't gonna lie.
	public static class Configs
	{
		public static ConfigFile GeneralConfig;

		public static ConfigFile Item_Common_Config;
		public static ConfigFile Item_Uncommon_Config;
		public static ConfigFile Item_Legendary_Config;
		public static ConfigFile Item_Yellow_Config;
		public static ConfigFile Item_Void_Config;
		public static ConfigFile Item_Food_Config;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_ElusiveAntlers_Rework = "Elusive Antlers Rework";

		private const string Section_BisonSteak_Buff = "Bison Steak";
		private const string Section_BisonSteak_Rework = "Bison Steak Rework";

		private const string Section_TopazBrooch_Buff = "Topaz Brooch";

		private const string Section_RollOfPennies_Rework = "Roll of Pennies Rework";

		private const string Section_WarpedEcho_Buff = "Warped Echo";

		private const string Section_BreachingFin_Rework = "Breaching Fin Rework";

		private const string Section_Chronobauble_Buff = "Chronobauble";

		private const string Section_DeathMark_Buff = "Death Mark";

		private const string Section_LeptonDaisy_Buff = "Lepton Daisy";

		private const string Section_LeechingSeed_Buff = "Leeching Seed";
		private const string Section_LeechingSeed_Rework = "Leeching Seed Rework";

		private const string Section_IgnitionTank_Rework = "Ignition Tank Rework";

		private const string Section_Infusion_Buff = "Infusion";

		private const string Section_WarHorn_Buff = "War Horn";

		private const string Section_HuntersHarpoon_Buff = "Hunters Harpoon";

		private const string Section_SquidPolyp_Buff = "Squid Polyp";

		private const string Section_Stealthkit_Buff = "Old War Stealthkit";

		private const string Section_UnstableTransmitter_Rework = "Unstable Transmitter Rework";

		private const string Section_RedWhip_Buff = "Red Whip";

		private const string Section_RoseBuckler_Buff = "Rose Buckler";

		private const string Section_WaxQuail_Buff = "Wax Quail";

		private const string Section_Aegis_Buff = "Aegis";

		private const string Section_BensRaincoat_Buff = "Bens Raincoat";

		private const string Section_GrowthNectar_Buff = "Growth Nectar";

		private const string Section_HappiestMask_Rework = "Happiest Mask Rework";

		private const string Section_LaserScope_Buff = "Laser Scope";

		private const string Section_PocketICBM_Buff = "Pocket ICBM";

		private const string Section_SonorousWhispers_Rework = "Sonorous Whispers Rework";

		private const string Section_SymbioticScorpion_Rework = "Symbiotic Scorpion Rework";

		private const string Section_Planula_Buff = "Planula";
		private const string Section_Planula_Rework = "Planula Rework";

		private const string Section_TitanicKnurl_Buff = "Titanic Knurl";
		private const string Section_TitanicKnurl_Rework = "Titanic Knurl Rework";
		private const string Section_TitanicKnurl_Rework_B = "Titanic Knurl Rework B";

		private const string Section_DefenseNucleus_Buff = "Defense Nucleus";
		private const string Section_DefenseNucleus_Rework = "Defense Nucleus Rework";
		private const string Section_DefenseNucleus_Shared = "Alpha Construct Ally";

		private const string Section_LigmaLenses_Buff = "Lost Seers Lenses";

		private const string Section_VoidsentFlame_Buff = "Voidsent Flame";

		private const string Section_NewlyHatchedZoea_Rework = "Newly Hatched Zoea Rework";

		private const string Section_SearedSteak_Buff = "Seared Steak";
		private const string Section_SearedSteak_Rework = "Seared Steak Rework";

		private const string Label_EnableBuff = "Enable Changes";
		private const string Label_EnableRework = "Enable Rework";

		private const string Desc_EnableBuff = "Enables changes for this item.";
		private const string Desc_EnableRework = "Enables the rework for this item. Has priority over the normal changes.";
		private const string Desc_EnableRework_B = "Enables the rework for this item. Has priority over the normal changes and other rework.";

		private const string Section_General_Mechanics = "Mechanics";

		private const string Label_AssistManager = "Enable Kill Assists";
		private const string Desc_AssistManager = "Allows on kill effects from this item to work with AssistManager.";

		public static void Setup()
        {
			GeneralConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_General.cfg"), true);

			Item_Common_Config = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_Items_Common.cfg"), true);
			Item_Uncommon_Config = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_Items_Uncommon.cfg"), true);
			Item_Legendary_Config = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_Items_Legendary.cfg"), true);
			Item_Yellow_Config = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_Items_Yellow.cfg"), true);
			Item_Void_Config = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_Items_Void.cfg"), true);
			Item_Food_Config = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"FlatItemBuff_Items_Food.cfg"), true);
			//Common
			Read_BisonSteak();
			Read_ElusiveAntlers();
			Read_TopazBrooch();
			Read_RollOfPennies();
			Read_WarpedEcho();
			//Uncommon
			Read_BreachingFin();
			Read_Chronobauble();
			Read_DeathMark();
			Read_HuntersHarpoon();
			Read_IgnitionTank();
			Read_Infusion();
			Read_LeechingSeed();
			Read_LeptonDaisy();
			Read_SquidPolyp();
			Read_Stealthkit();
			Read_UnstableTransmitter();
			Read_RedWhip();
			Read_RoseBuckler();
			Read_WarHorn();
			Read_WaxQuail();
			//Legendary
			Read_Aegis();
			Read_BensRaincoat();
			Read_GrowthNectar();
			Read_HappiestMask_Rework();
			Read_LaserScope();
			Read_PocketICBM();
			Read_SonorousWhispers();
			Read_SymbioticScorpion();
			//Boss
			Read_Planula();
			Read_TitanicKnurl();
			Read_DefenseNucleus();
			//Void
			Read_LigmaLenses();
			Read_VoidsentFlame();
			Read_NewlyHatchedZoea();
			//Food
			Read_SearedSteak();
			//General
			Read_General();
		}
		private static void Read_General()
        {
			//Section_Bugs
			GeneralChanges.TweakBarrierDecay = GeneralConfig.Bind(Section_General_Mechanics, "Tweak Barrier Decay", false, "Changes barrier decay to scale from max health + shields instead of max barrier, recommended and specifically catered for Aegis changes.").Value;
		}
		private static void Read_BisonSteak()
        {
			BisonSteak.Enable = Item_Common_Config.Bind(Section_BisonSteak_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			BisonSteak.BaseHP = Item_Common_Config.Bind(Section_BisonSteak_Buff, "Base HP", 10f, "Health each stack gives.").Value;
			BisonSteak.LevelHP = Item_Common_Config.Bind(Section_BisonSteak_Buff, "Level HP", 3f, "Health each stack gives per level.").Value;

			BisonSteak_Rework.Enable = Item_Common_Config.Bind(Section_BisonSteak_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			BisonSteak_Rework.BaseRegen = Item_Common_Config.Bind(Section_BisonSteak_Rework, "Base Regen", 1f, "Health regen at a single stack. (Scales with level)").Value;
			BisonSteak_Rework.StackRegen = Item_Common_Config.Bind(Section_BisonSteak_Rework, "Stack Regen", 0f, "Health regen for each additional stack. (Scales with level)").Value;
			BisonSteak_Rework.BaseDuration = Item_Common_Config.Bind(Section_BisonSteak_Rework, "Base Regen Duration", 3f, "Duration of the regen buff at a single stack.").Value;
			BisonSteak_Rework.StackDuration = Item_Common_Config.Bind(Section_BisonSteak_Rework, "Stack Regen Duration", 3f, "Duration of the regen buff for each additional stack.").Value;
			BisonSteak_Rework.ExtendDuration = Item_Common_Config.Bind(Section_BisonSteak_Rework, "Extend Duration", 1f, "How much to extend the effect duration on kill.").Value;
			BisonSteak_Rework.NerfFakeKill = Item_Common_Config.Bind(Section_BisonSteak_Rework, "Nerf Fake Kills", false, "Prevents fake kills from extending the duration.").Value;
			BisonSteak_Rework.Comp_AssistManager = Item_Common_Config.Bind(Section_BisonSteak_Rework, Label_AssistManager, true, Desc_AssistManager).Value;
		}
		private static void Read_ElusiveAntlers()
		{
			ElusiveAntlers_Rework.Enable = Item_Common_Config.Bind(Section_ElusiveAntlers_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			ElusiveAntlers_Rework.StackArmor = Item_Common_Config.Bind(Section_ElusiveAntlers_Rework, "Stack Armor", 5f, "Armor each stack gives.").Value;
			ElusiveAntlers_Rework.StackSpeed = Item_Common_Config.Bind(Section_ElusiveAntlers_Rework, "Stack Movement Speed", 0.07f, "Movement speed each stack gives.").Value;
		}
		private static void Read_TopazBrooch()
        {
			TopazBrooch.Enable = Item_Common_Config.Bind(Section_TopazBrooch_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			TopazBrooch.BaseFlatBarrier = Item_Common_Config.Bind(Section_TopazBrooch_Buff, "Base Flat Barrier", 8.0f, "Flat amount of barrier given at a single stack.").Value;
			TopazBrooch.StackFlatBarrier = Item_Common_Config.Bind(Section_TopazBrooch_Buff, "Stack Flat Barrier", 0.0f, "Flat amount of barrier given for each additional stack.").Value;
			TopazBrooch.BaseCentBarrier = Item_Common_Config.Bind(Section_TopazBrooch_Buff, "Base Percent Barrier", 0.02f, "Percent amount of barrier given at a single stack.").Value;
			TopazBrooch.StackCentBarrier = Item_Common_Config.Bind(Section_TopazBrooch_Buff, "Stack Percent Barrier", 0.02f, "Percent amount of barrier given for each additional stack.").Value;
			TopazBrooch.Comp_AssistManager = Item_Common_Config.Bind(Section_TopazBrooch_Buff, Label_AssistManager, true, Desc_AssistManager).Value;
		}
		private static void Read_RollOfPennies()
        {
			RollOfPennies_Rework.Enable = Item_Common_Config.Bind(Section_RollOfPennies_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			RollOfPennies_Rework.BaseGold = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Base Gold", 3f, "Gold amount given at a single stack.").Value;
			RollOfPennies_Rework.StackGold = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Stack Gold", 0f, "Gold amount given for each additional stack.").Value;
			RollOfPennies_Rework.BaseArmor = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Base Armor", 5f, "Armor given at a single stack.").Value;
			RollOfPennies_Rework.StackArmor = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Stack Armor", 0f, "Armor given for each additional stack.").Value;
			RollOfPennies_Rework.BaseDuration = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Base Armor Duration", 2f, "Duration given to the armor at a single stack.").Value;
			RollOfPennies_Rework.StackDuration = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Stack Armor Duration", 2f, "Duration given to the armor for each additional stack.").Value;
			RollOfPennies_Rework.GoldDuration = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "Gold Armor Duration", 0.5f, "Multiplier for the gold's value when calculating the extra duration.").Value;
			RollOfPennies_Rework.VFXAmount = Item_Common_Config.Bind(Section_RollOfPennies_Rework, "VFX Amount", 10, "How many stacks of the armor buff are needed for the gold shield VFX active. (Set to 0 to disable this)").Value;
		}
		private static void Read_WarpedEcho()
        {
			WarpedEcho.Enable = Item_Common_Config.Bind(Section_WarpedEcho_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			WarpedEcho.BaseArmor = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Base Armor", 6f, "Armor given at a single stack.").Value;
			WarpedEcho.StackArmor = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Stack Armor", 6f, "Armor given for each additional stack.").Value;
			WarpedEcho.InCountArmor = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Is Armor", true, "Counts as Armor when spliting damage.").Value;
			WarpedEcho.InCountBlock = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Is Block", false, "Counts as Block when spliting damage.").Value;
			WarpedEcho.OutIgnoreArmor = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Delay Ignore Armor", false, "Should delayed damage ignore armor?").Value;
			WarpedEcho.OutIgnoreBlock = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Delay Ignore Block", false, "Should delayed damage ignore block?").Value;
			WarpedEcho.UseOldVisual = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Use Old VFX", false, "Uses the Warped Echo VFX from before v1.3.6.").Value;
			WarpedEcho.HealthDisplay = Item_Common_Config.Bind(Section_WarpedEcho_Buff, "Health Display", true, "Use the health display for echo damage?").Value;
		}
		private static void Read_BreachingFin()
		{
			BreachingFin_Rework.Enable = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			BreachingFin_Rework.BaseForce = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Base Force", 24.0f, "Vertical force at a single stack.").Value;
			BreachingFin_Rework.StackForce = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Stack Force", 3.0f, "Vertical force for each additional stack.").Value;
			BreachingFin_Rework.BackForce = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Push Force", 0.5f, "How much to push away, is multiplied by the vertical force.").Value;
			BreachingFin_Rework.ChampionMult = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Champion Force Mult", 0.5f, "Force multiplier for champion targets.").Value;
			BreachingFin_Rework.BossMult = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Boss Force Mult", 1f, "Force multiplier for boss targets.").Value;
			BreachingFin_Rework.FlyingMult = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Flying Force Mult", 1f, "Force multiplier for flying targets.").Value;
			BreachingFin_Rework.MaxForce = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Max Force", 200f, "The limit on how much force can be gained from stacking the item.").Value;
			BreachingFin_Rework.BaseRadius = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Base Radius", 12f, "Radius in metres for the impact. (Set to 0 to completely disable the Impact and its damage.)").Value;
			BreachingFin_Rework.BaseDamage = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Base Damage", 1.5f, "Impact damage at a single stack.").Value;
			BreachingFin_Rework.StackDamage = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Stack Damage", 0.3f, "Impact damage for each additional stack.").Value;
			BreachingFin_Rework.MaxDistDamage = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Velocity Damage", 10f, "Maximum damage multiplier that can be achieved through velocity.").Value;
			BreachingFin_Rework.ProcRate = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Proc Coefficient", 0f, "Impact proc coefficient. (It can proc itself)").Value;
			BreachingFin_Rework.DoStun = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Stun", true, "Stuns launched targets when they impact the ground.").Value;
			BreachingFin_Rework.CreditFall = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Credit Fall Damage", false, "Credits any fall damage the target takes to the inflictor of the knockback.").Value;
			BreachingFin_Rework.Cooldown = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "Cooldown", 10, "The cooldown between knockbacks.").Value;
			BreachingFin_Rework.OnSkill = Item_Uncommon_Config.Bind(Section_BreachingFin_Rework, "On Skills", true, "Only activate on Skill damage?").Value;
		}
		private static void Read_Chronobauble()
        {
			Chronobauble.Enable = Item_Uncommon_Config.Bind(Section_Chronobauble_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Chronobauble.SlowDown = Item_Uncommon_Config.Bind(Section_Chronobauble_Buff, "Move Speed Reduction", 0.6f, "Move Speed debuff amount.").Value;
			Chronobauble.AttackDown = Item_Uncommon_Config.Bind(Section_Chronobauble_Buff, "Attack Speed Reduction", 0.3f, "Attack Speed debuff amount.").Value;
			Chronobauble.BaseDuration = Item_Uncommon_Config.Bind(Section_Chronobauble_Buff, "Base Duration", 2f, "Debuff duration at a single stack.").Value;
			Chronobauble.StackDuration = Item_Uncommon_Config.Bind(Section_Chronobauble_Buff, "Stack Duration", 2f, "Debuff duration for each additional.").Value;
		}
		private static void Read_DeathMark()
		{
			DeathMark.Enable = Item_Uncommon_Config.Bind(Section_DeathMark_Buff, Label_EnableRework, false, Desc_EnableRework).Value;
			DeathMark.BaseDuration = Item_Uncommon_Config.Bind(Section_DeathMark_Buff, "Base Duration", 6f, "Duration of the Death Mark at a single stack.").Value;
			DeathMark.StackDuration = Item_Uncommon_Config.Bind(Section_DeathMark_Buff, "Stack Duration", 4f, "Duration of the Death Mark for each additional stack.").Value;
			DeathMark.DamagePerDebuff = Item_Uncommon_Config.Bind(Section_DeathMark_Buff, "Damage Per Debuff", 0.1f, "Damage take per debuff.").Value;
			DeathMark.MaxDebuffs = Item_Uncommon_Config.Bind(Section_DeathMark_Buff, "Max Debuffs", 5, "The max amount of debuff that can increase damage.").Value;
		}
		private static void Read_LeptonDaisy()
        {
			LeptonDaisy.Enable = Item_Uncommon_Config.Bind(Section_LeptonDaisy_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LeptonDaisy.BaseHeal = Item_Uncommon_Config.Bind(Section_LeptonDaisy_Buff, "Base Healing", 0.1f, "Healing at a single stack.").Value;
			LeptonDaisy.StackHeal = Item_Uncommon_Config.Bind(Section_LeptonDaisy_Buff, "Stack Healing", 0.05f, "Healing for each additional stack.").Value;
			LeptonDaisy.Cooldown = Item_Uncommon_Config.Bind(Section_LeptonDaisy_Buff, "Cooldown", 10f, "Cooldown of the healing nova.").Value;
			LeptonDaisy.UseBaseRadius = Item_Uncommon_Config.Bind(Section_LeptonDaisy_Buff, "Use Base Radius", false, "Healing nova uses the base radius of the holdout instead of the current radius.").Value;
		}
		private static void Read_IgnitionTank()
		{
			IgnitionTank_Rework.Enable = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			IgnitionTank_Rework.BurnChance = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Burn Chance", 10f, "Chance to Burn on hit.").Value;
			IgnitionTank_Rework.BurnBaseDamage = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Burn Damage", 0.8f, "How much damage Burn deals per second.").Value;
			IgnitionTank_Rework.BurnDuration = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Burn Duration", 3f, "How long in seconds Burn lasts for.").Value;

			IgnitionTank_Rework.BlastTicks = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explode Ticks", 10, "Explodes after X instances of damage over time.").Value;
			IgnitionTank_Rework.BlastChance = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explode Chance", 0.0f, "Chance of damage over time to cause explosions. (Overrides Explode Ticks)").Value;
			IgnitionTank_Rework.BlastBaseDamage = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explosion Base Damage", 3f, "Damage the explosion deals at a single stack.").Value;
			IgnitionTank_Rework.BlastStackDamage = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explosion Stack Damage", 2f, "Extra damage the explosion deals for each additional stack.").Value;
			IgnitionTank_Rework.BlastBaseRadius = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explosion Base Radius", 12f, "Radius of the the explosion at a single stack.").Value;
			IgnitionTank_Rework.BlastStackRadius = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explosion Stack Radius", 2.4f, "Extra explosion radius for each additional stack.").Value;
			IgnitionTank_Rework.BlastInheritDamageType = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explosion Inherit Damage Type", false, "Makes the explosion inherit the damage types used from the hit that procced. (For example it would make the explosion non-lethal if it was from Acrid's poison)").Value;
			IgnitionTank_Rework.BlastRollCrit = Item_Uncommon_Config.Bind(Section_IgnitionTank_Rework, "Explosion Roll Crits", false, "Allows the explosion to roll for crits, it will still inherit crits from the hit that procced regardless of this setting.").Value;
		}
		private static void Read_Infusion()
        {
			Infusion.Enable = Item_Uncommon_Config.Bind(Section_Infusion_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Infusion.StackLevel = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Stack Level", 2, "Number of levels gained per stack.").Value;
			Infusion.Infinite = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Soft Cap", true, "Allows samples to be collected beyond the cap with diminishing returns.").Value;
			Infusion.Inherit = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Inherit", true, "Should your minions with infusions inherit your count.").Value;
			Infusion.ChampionGain = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Champion Value", 5, "Sample value of champion enemies. (Wandering Vagrant, Magma Worm, etc)").Value;
			Infusion.EliteGainMult = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Elite Multiplier", 3, "Sample value multiplier from elite enemies.").Value;
			Infusion.BossGainMult = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Boss Multiplier", 2, "Sample value multiplier from boss enemies.").Value;
			Infusion.Comp_AssistManager = Item_Uncommon_Config.Bind(Section_Infusion_Buff, Label_AssistManager, true, Desc_AssistManager).Value;
			Infusion.BuffEnable = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Buff Icon", true, "Enables a buff icon to show how many levels are gained from this item.").Value;
			Infusion.BuffGrowth = Item_Uncommon_Config.Bind(Section_Infusion_Buff, "Buff Count Nectar", true, "Buff given counts towards Growth Nectar's effect.").Value;
		}
		private static void Read_LeechingSeed()
        {
			LeechingSeed.Enable = Item_Uncommon_Config.Bind(Section_LeechingSeed_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LeechingSeed.ProcHeal = Item_Uncommon_Config.Bind(Section_LeechingSeed_Buff, "Proc Healing", 1f, "Healing amount that's affected by proc coefficient.").Value;
			LeechingSeed.BaseHeal = Item_Uncommon_Config.Bind(Section_LeechingSeed_Buff, "Base Healing", 1f, "Extra healing amount regardless of proc coefficient.").Value;

			LeechingSeed_Rework.Enable = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			LeechingSeed_Rework.BaseDoTHeal = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Base DoT Healing", 1f, "Healing amount given from damage over time ticks at a single stack.").Value;
			LeechingSeed_Rework.StackDoTHeal = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Stack DoT Healing", 1f, "Healing amount given from damage over time for each additional stack.").Value;
			LeechingSeed_Rework.ScaleToTickRate = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Scale To Tick Rate", true, "Healing amount scales with how often the damage over time ticks, slower tick rates give more healing.").Value;
			LeechingSeed_Rework.LeechChance = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Leech Chance", 20f, "Chance of applying the Leech debuff.").Value;
			LeechingSeed_Rework.LeechLifeSteal = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Leech Life Steal", 1f, "Percent of damage received as healing when damaging a target with Leech. (Gets scaled by the attacker's damage stat.)").Value;
			LeechingSeed_Rework.LeechBaseDamage = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Leech Base Damage", 2.5f, "Damage of the Leech at a single stack.").Value;
			LeechingSeed_Rework.LeechStackDamage = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Leech Stack Damage", 0f, "Damage of the Leech for each additional stack.").Value;
			LeechingSeed_Rework.LeechBaseDuration = Item_Uncommon_Config.Bind(Section_LeechingSeed_Rework, "Leech Base Duration", 5f, "Duration of the Leech debuff.").Value;
		}
		private static void Read_WaxQuail()
        {
			WaxQuail.Enable = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			WaxQuail.BaseHori = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Base Horizontal Boost", 12f, "Horizontal force at a single stack.").Value;
			WaxQuail.StackHori = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Stack Horizontal Boost", 8f, "Horizontal force for each additional stack.").Value;
			WaxQuail.CapHori = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Capped Horizontal Boost", 120f, "Hyperbolic cap to horizontal force. (Set to 0 or less to disable.)").Value;
			WaxQuail.BaseVert = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Base Vertical Boost", 0.2f, "Vertical force at a single stack.").Value;
			WaxQuail.StackVert = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Stack Vertical", 0f, "Vertical force for each additional stack.").Value;
			WaxQuail.CapVert = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Capped Vertical Boost", 0f, "Hyperbolic cap to vertical force. (Set to 0 or less to disable.)").Value;
			WaxQuail.BaseAirSpeed = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Base Air Speed", 0.12f, "Airborne movement speed at a single stack.").Value;
			WaxQuail.StackAirSpeed = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Stack Air Speed", 0.08f, "Airborne movement speed for each additional stack.").Value;
			WaxQuail.CapAirSpeed = Item_Uncommon_Config.Bind(Section_WaxQuail_Buff, "Capped Air Speed", 1.2f, "Hyperbolic cap to airborne movement speed. (Set to 0 or less to disable.)").Value;
		}
		private static void Read_Stealthkit()
        {
			Stealthkit.Enable = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Stealthkit.BaseRecharge = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Base Cooldown", 30f, "Cooldown between uses.").Value;
			Stealthkit.StackRecharge = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Stack Cooldown", 0.5f, "Cooldown rate for each additional stack.").Value;
			Stealthkit.BuffDuration = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Buff Duration", 5f, "Duration of the Stealth buff.").Value;
			Stealthkit.Stealth_MoveSpeed = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Stealth Movement Speed", 0.4f, "How much Movement Speed is given from the Stealth buff.").Value;
			Stealthkit.Stealth_ArmorPerBuff = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Stealth Armor", 20f, "How much Armor to give per stack of the Stealth buff.").Value;
			Stealthkit.CancelDanger = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Cancel Danger", true, "Puts you in 'Out of Danger' upon activation.").Value;
			Stealthkit.CleanseDoT = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Clean DoTs", true, "Removes damage over time effects upon activation.").Value;
			Stealthkit.SmokeBomb = Item_Uncommon_Config.Bind(Section_Stealthkit_Buff, "Smoke Bomb", true, "Causes a stunning smoke bomb effect upon activation.").Value;
		}
		private static void Read_UnstableTransmitter()
        {
			UnstableTransmitter_Rework.Enable = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			UnstableTransmitter_Rework.BaseCooldown = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Base Cooldown", 30f, "Cooldown between activations.").Value;
			UnstableTransmitter_Rework.AllyStackCooldown = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Ally Cooldown", 0.5f, "Cooldown reduction per ally owned.").Value;
			UnstableTransmitter_Rework.CapCooldown = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Cap Cooldown", 1f, "The lowest the cooldown can go.").Value;
			UnstableTransmitter_Rework.BaseDamage = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Base Damage", 3.5f, "Damage at a single stack.").Value;
			UnstableTransmitter_Rework.StackDamage = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Stack Damage", 2.8f, "Damage for each additional stack.").Value;
			UnstableTransmitter_Rework.BaseRadius = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Base Radius", 16f, "Blast radius at a single stack.").Value;
			UnstableTransmitter_Rework.StackRadius = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Stack Radius", 0f, "Blast radius for each additional stack.").Value;
			UnstableTransmitter_Rework.ProcRate = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Proc Coefficient", 1f, "The Proc Coefficient of the blast.").Value;
			UnstableTransmitter_Rework.ProcBands = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Proc Bands", true, "Should the blast proc bands?").Value;
			UnstableTransmitter_Rework.AllyOwnsDamage = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Ally Owns Damage", false, "Should the ally own the explosion instead of the user?").Value;
			UnstableTransmitter_Rework.Respawns = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Respawns Minion", false, "Should the Strike Drone that comes with the item respawn if killed?").Value;
			UnstableTransmitter_Rework.TeleportRadius = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Teleport Radius", 40f, "The maximum radius away from the user that allies can be teleported to.").Value;
			UnstableTransmitter_Rework.TeleFragRadius = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Telefrag Radius", 60f, "Enemies within this radius of the user can be purposely telefragged by allies. (Set to 0 or less to disable this behavior.)").Value;
			UnstableTransmitter_Rework.TeleImmobile = Item_Uncommon_Config.Bind(Section_UnstableTransmitter_Rework, "Teleport Immobile", true, "Allows immobile allies to be targeted for teleportation.").Value;
		}
		private static void Read_RedWhip()
		{
			RedWhip.Enable = Item_Uncommon_Config.Bind(Section_RedWhip_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			RedWhip.BaseSpeed = Item_Uncommon_Config.Bind(Section_RedWhip_Buff, "Base Speed", 0.1f, "Passive speed increase at a single stack.").Value;
			RedWhip.StackSpeed = Item_Uncommon_Config.Bind(Section_RedWhip_Buff, "Stack Speed", 0.1f, "Passive speed increase for each additional stack.").Value;
			RedWhip.BaseActiveSpeed = Item_Uncommon_Config.Bind(Section_RedWhip_Buff, "Base Active Speed", 0.2f, "Active speed increase at a single stack.").Value;
			RedWhip.StackActiveSpeed = Item_Uncommon_Config.Bind(Section_RedWhip_Buff, "Stack Active Speed", 0.2f, "Active speed increase for each additional stack.").Value;
		}

		private static void Read_RoseBuckler()
		{
			RoseBuckler.Enable = Item_Uncommon_Config.Bind(Section_RoseBuckler_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			RoseBuckler.BaseArmor = Item_Uncommon_Config.Bind(Section_RoseBuckler_Buff, "Base Armor", 10f, "Passive armor increase at a single stack.").Value;
			RoseBuckler.StackArmor = Item_Uncommon_Config.Bind(Section_RoseBuckler_Buff, "Stack Armor", 10f, "Passive armor increase for each additional stack.").Value;
			RoseBuckler.BaseActiveArmor = Item_Uncommon_Config.Bind(Section_RoseBuckler_Buff, "Base Active Armor", 20f, "Active armor increase at a single stack.").Value;
			RoseBuckler.StackActiveArmor = Item_Uncommon_Config.Bind(Section_RoseBuckler_Buff, "Stack Active Armor", 20f, "Active armor increase for each additional stack.").Value;
		}
		private static void Read_HuntersHarpoon()
        {
			HuntersHarpoon.Enable = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			HuntersHarpoon.BaseDuration = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Base Duration", 1f, "Buff duration at a single stack.").Value;
			HuntersHarpoon.StackDuration = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Stack Duration", 1f, "Extra buff duration for each additional stack.").Value;
			HuntersHarpoon.MovementSpeed = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Movement Speed Bonus", 1.25f, "Movement speed given from the buff.").Value;
			HuntersHarpoon.ExtendDuration = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Extend Duration", true, "Adds duration to the buff with each kill instead of refreshing it.").Value;
			HuntersHarpoon.BaseCooldownReduction = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Cooldown Reduction", 1f, "Cooldown reduction on kill.").Value;
			HuntersHarpoon.CoolPrimary = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Cooldown Primary", false, "Cooldown reduction affects Primary skills?").Value;
			HuntersHarpoon.CoolSecondary = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Cooldown Secondary", true, "Cooldown reduction affects Secondary skills?").Value;
			HuntersHarpoon.CoolUtility = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Cooldown Utility", false, "Cooldown reduction affects Utility skills?").Value;
			HuntersHarpoon.CoolSpecial = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, "Cooldown Special", false, "Cooldown reduction affects Special skills?").Value;
			HuntersHarpoon.Comp_AssistManager = Item_Uncommon_Config.Bind(Section_HuntersHarpoon_Buff, Label_AssistManager, true, Desc_AssistManager).Value;
		}
		private static void Read_SquidPolyp()
        {
			SquidPolyp.Enable = Item_Uncommon_Config.Bind(Section_SquidPolyp_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			SquidPolyp.ApplyTar = Item_Uncommon_Config.Bind(Section_SquidPolyp_Buff, "Apply Tar", true, "Makes Squid Turrets apply the Tar debuff with their attack.").Value;
			SquidPolyp.BaseDuration = Item_Uncommon_Config.Bind(Section_SquidPolyp_Buff, "Base Duration", 25, "Squid Turret duration at a single stack.").Value;
			SquidPolyp.StackDuration = Item_Uncommon_Config.Bind(Section_SquidPolyp_Buff, "Stack Duration", 5, "Squid Turret duration for each additional stack.").Value;
			SquidPolyp.StackHealth = Item_Uncommon_Config.Bind(Section_SquidPolyp_Buff, "Stack Health", 2, "Extra Squid Turret health for each additional stack. (1 = +10%)").Value;
			SquidPolyp.MaxTurrets = Item_Uncommon_Config.Bind(Section_SquidPolyp_Buff, "Max Turrets", 8, "How many Squid Turrets each player can have, newer turrets will replace old ones. (Set to 0 for vanilla behavior.)").Value;
		}
		private static void Read_WarHorn()
        {
			WarHorn.Enable = Item_Uncommon_Config.Bind(Section_WarHorn_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			WarHorn.BaseDuration = Item_Uncommon_Config.Bind(Section_WarHorn_Buff, "Base Duration", 6f, "Duration at a single stack.").Value;
			WarHorn.StackDuration = Item_Uncommon_Config.Bind(Section_WarHorn_Buff, "Stack Duration", 3f, "Duration for each additional stack.").Value;
			WarHorn.BaseAttack = Item_Uncommon_Config.Bind(Section_WarHorn_Buff, "Base Attack", 0.6f, "Attack Speed at a single stack.").Value;
			WarHorn.StackAttack = Item_Uncommon_Config.Bind(Section_WarHorn_Buff, "Stack Attack", 0.15f, "Attack Speed for each additional stack.").Value;
		}
		private static void Read_Aegis()
		{
			Aegis.Enable = Item_Legendary_Config.Bind(Section_Aegis_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Aegis.AllowRegen = Item_Legendary_Config.Bind(Section_Aegis_Buff, "Count Regen", true, "Allows excess regen to be converted into barrier.").Value;
			Aegis.BaseOverheal = Item_Legendary_Config.Bind(Section_Aegis_Buff, "Base Overheal", 1f, "Conversion rate of overheal to barrier at a single stack.").Value;
			Aegis.StackOverheal = Item_Legendary_Config.Bind(Section_Aegis_Buff, "Stack Overheal", 0.2f, "Conversion rate of overheal to barrier for each additional stack.").Value;
			Aegis.BaseMaxBarrier = Item_Legendary_Config.Bind(Section_Aegis_Buff, "Base Max Barrier", 0.5f, "Increases maximum barrier by this much at a single stack.").Value;
			Aegis.StackMaxBarrier = Item_Legendary_Config.Bind(Section_Aegis_Buff, "Stack Max Barrier", 0.5f, "Increases maximum barrier by this much for each additional stack.").Value;
		}
		private static void Read_BensRaincoat()
        {
			BensRaincoat.Enable = Item_Legendary_Config.Bind(Section_BensRaincoat_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			BensRaincoat.BaseBlock = Item_Legendary_Config.Bind(Section_BensRaincoat_Buff, "Base Block", 2, "Debuff blocks to give at a single stack.").Value;
			BensRaincoat.StackBlock = Item_Legendary_Config.Bind(Section_BensRaincoat_Buff, "Stack Block", 1, "Debuff blocks to give for each additional stack.").Value;
			BensRaincoat.Cooldown = Item_Legendary_Config.Bind(Section_BensRaincoat_Buff, "Cooldown", 7f, "Time in seconds it takes to restock debuff blocks. (Anything less than 0 will skip this change.)").Value;
			BensRaincoat.GraceTime = Item_Legendary_Config.Bind(Section_BensRaincoat_Buff, "Debuff Grace Time", 0.25f, "Time in seconds after consuming a block that further debuffs are negated for free.").Value;
		}
		private static void Read_GrowthNectar()
		{
			GrowthNectar.Enable = Item_Legendary_Config.Bind(Section_GrowthNectar_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			GrowthNectar.BaseBoost = Item_Legendary_Config.Bind(Section_GrowthNectar_Buff, "Base Boost", 0.04f, "Stat increase per unique buff at a single stack.").Value;
			GrowthNectar.StackBoost = Item_Legendary_Config.Bind(Section_GrowthNectar_Buff, "Stack Boost", 0.04f, "Stat increase per unique buff for each additional stack.").Value;
			GrowthNectar.BaseCap = Item_Legendary_Config.Bind(Section_GrowthNectar_Buff, "Base Cap", 5, "The cap on how many buffs can increase your stats.").Value;
		}
		private static void Read_HappiestMask_Rework()
        {
			HappiestMask_Rework.Enable = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			HappiestMask_Rework.BaseDamage = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "Base Damage", 2f, "Damage increase for ghosts at a single stack.").Value;
			HappiestMask_Rework.StackDamage = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "Stack Damage", 3f, "Damage increase for ghosts for each additional stack.").Value;
			HappiestMask_Rework.BaseMoveSpeed = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "Movement Speed", 0.45f, "Movement speed increase for ghosts.").Value;
			HappiestMask_Rework.BaseDuration = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "Duration", 30, "How long in seconds the ghosts lasts before dying.").Value;
			HappiestMask_Rework.BaseCooldown = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "Cooldown", 3, "How long in seconds until a new ghost is summoned.").Value;
			HappiestMask_Rework.OnKillOnDeath = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "On Kill On Death", true, "Credits the ghost's death as your kill.").Value;
			HappiestMask_Rework.PassKillCredit = Item_Legendary_Config.Bind(Section_HappiestMask_Rework, "Kill Credit Owner", true, "Credits all kills the ghost scores as yours.").Value;
		}
		private static void Read_LaserScope()
        {
			LaserScope.Enable = Item_Legendary_Config.Bind(Section_LaserScope_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LaserScope.BaseCrit = Item_Legendary_Config.Bind(Section_LaserScope_Buff, "Crit Chance", 5f, "Crit chance at a single stack.").Value;
		}
		private static void Read_PocketICBM()
		{
			PocketICBM.Enable = Item_Legendary_Config.Bind(Section_PocketICBM_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			PocketICBM.BaseChance = Item_Legendary_Config.Bind(Section_PocketICBM_Buff, "Base Chance", 7f, "Chance of firing a missile at a single stack. (100 = 100%)").Value;
			PocketICBM.StackChance = Item_Legendary_Config.Bind(Section_PocketICBM_Buff, "Stack Chance", 0f, "Chance of firing a missile for each additional stack.").Value;
			PocketICBM.BaseDamage = Item_Legendary_Config.Bind(Section_PocketICBM_Buff, "Base Damage", 2f, "Damage of the missile at a single stack.").Value;
			PocketICBM.StackDamage = Item_Legendary_Config.Bind(Section_PocketICBM_Buff, "Stack Damage", 0f, "Damage of the missile for each additional stack.").Value;
			PocketICBM.MissileProc = Item_Legendary_Config.Bind(Section_PocketICBM_Buff, "Proc Coefficient", 1f, "Missile proc coefficient.").Value;
		}
		private static void Read_SonorousWhispers()
        {
			SonorousWhispers_Rework.Enable = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			SonorousWhispers_Rework.BasePower = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Base Power", 1f, "Health and Damage scaling of the monster at a single stack.").Value;
			SonorousWhispers_Rework.StackPower = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Stack Power", 0f, "Health and Damage scaling of the monster for each additional stack.").Value;
			SonorousWhispers_Rework.BaseDamage = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Base Damage", 60f, "Damage of the explosion the monster causes when defeated at a single stack. (Set to 0 to disable this.)").Value;
			SonorousWhispers_Rework.StackDamage = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Stack Damage", 48f, "Damage of the explosion the monster causes when defeated for each additional stack.").Value;
			SonorousWhispers_Rework.BaseRadius = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Base Radius", 100f, "Radius of the explosion the monster causes when defeated at a single stack. (Set to 0 to disable this.)").Value;
			SonorousWhispers_Rework.StackRadius = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Stack Radius", 20f, "Radius of the explosion the monster causes when defeated for each additional stack.").Value;
			SonorousWhispers_Rework.BaseReward = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Base Rewards", 1, "Rewards dropped upon defeat at a single stack.").Value;
			SonorousWhispers_Rework.StackReward = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Stack Rewards", 1, "Rewards dropped upon defeat for each additional stack.").Value;
			SonorousWhispers_Rework.RewardLimit = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Reward Limit", 200, "The maximum amount of rewards the monster can drop, also acts as a cap for other stacking effects.").Value;
			SonorousWhispers_Rework.BaseGold = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Base Gold", 200, "Gold dropped upon defeat at a single stack.").Value;
			SonorousWhispers_Rework.StackGold = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Stack Gold", 100, "Gold dropped upon defeat for each additional stack.").Value;
			SonorousWhispers_Rework.ScalePlayer = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Player Scaling", true, "Scales the monster's stats and rewards with player count.").Value;
			SonorousWhispers_Rework.IsElite = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Elite", true, "Makes the monster always spawn as Gilded, has no affect on stats.").Value;
			SonorousWhispers_Rework.HasAdaptive = Item_Legendary_Config.Bind(Section_SonorousWhispers_Rework, "Adaptive Armor", false, "Gives the monster adaptive armor.").Value;
		}
		private static void Read_SymbioticScorpion()
        {
			SymbioticScorpion_Rework.Enable = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			SymbioticScorpion_Rework.Slayer_BaseDamage = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Slayer DoT Base Damage", 2f, "Slayer DoT damage increase at a single stack.").Value;
			SymbioticScorpion_Rework.Slayer_StackDamage = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Slayer DoT Stack Damage", 0f, "Slayer DoT damage increase for each additional stack.").Value;
			SymbioticScorpion_Rework.SlayerDot_AffectTotalDamage = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Slayer DoT Affects Total Damage", false, "Makes the Slayer DoT affect the total damage for the purpose of proc items. (False = Vanilla)").Value;
			SymbioticScorpion_Rework.Radius = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Venom Radius", 13f, "The targetting radius for the Venom attack. (Set to 0 to disable the effect entirely.)").Value;
			SymbioticScorpion_Rework.Cooldown = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Venom Cooldown", 5f, "Cooldown between Venom attacks.").Value;
			SymbioticScorpion_Rework.VenomBaseDamage = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Venom Base Damage", 6f, "Damage of the Venom at a single stack.").Value;
			SymbioticScorpion_Rework.VenomStackDamage = Item_Legendary_Config.Bind(Section_SymbioticScorpion_Rework, "Venom Stack Damage", 6f, "Damage of the Venom for each additional stack.").Value;
		}
		private static void Read_Planula()
        {
			Planula.Enable = Item_Yellow_Config.Bind(Section_Planula_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Planula.BaseFlatHeal = Item_Yellow_Config.Bind(Section_Planula_Buff, "Base Flat Healing", 10f, "Flat healing at a single stack.").Value;
			Planula.StackFlatHeal = Item_Yellow_Config.Bind(Section_Planula_Buff, "Stack Flat Healing", 10f, "Flat healing for each additional stack.").Value;
			Planula.BaseMaxHeal = Item_Yellow_Config.Bind(Section_Planula_Buff, "Base Max Healing", 0.02f, "Percent healing at a single stack.").Value;
			Planula.StackMaxHeal = Item_Yellow_Config.Bind(Section_Planula_Buff, "Stack Max Healing", 0.02f, "Percent healing for each additional stack.").Value;

			Planula_Rework.Enable = Item_Yellow_Config.Bind(Section_Planula_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			Planula_Rework.BaseDamage = Item_Yellow_Config.Bind(Section_Planula_Rework, "Base Damage", 0.8f, "Damage per second the burn deals at a single stack.").Value;
			Planula_Rework.StackDamage = Item_Yellow_Config.Bind(Section_Planula_Rework, "Stack Damage", 0.6f, "Damage per second the burn deals for each additional stack.").Value;
			Planula_Rework.Duration = Item_Yellow_Config.Bind(Section_Planula_Rework, "Burn Duration", 5f, "Duration of the burn.").Value;
			Planula_Rework.Radius = Item_Yellow_Config.Bind(Section_Planula_Rework, "Burn Radius", 15f, "Radius for enemies to be within to start burning.").Value;
		}
		private static void Read_TitanicKnurl()
        {
			TitanicKnurl.Enable = Item_Yellow_Config.Bind(Section_TitanicKnurl_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			TitanicKnurl.BaseHP = Item_Yellow_Config.Bind(Section_TitanicKnurl_Buff, "Base Health", 30f, "Health each stack gives.").Value;
			TitanicKnurl.LevelHP = Item_Yellow_Config.Bind(Section_TitanicKnurl_Buff, "Level Health", 9f, "Health each stack gives per level.").Value;
			TitanicKnurl.BaseRegen = Item_Yellow_Config.Bind(Section_TitanicKnurl_Buff, "Base Regen", 1.6f, "Health Regen each stack gives.").Value;
			TitanicKnurl.LevelRegen = Item_Yellow_Config.Bind(Section_TitanicKnurl_Buff, "Level Regen", 0.32f, "Health Regen each stack gives per level.").Value;
			
			TitanicKnurl_Rework.Enable = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			TitanicKnurl_Rework.BaseDamage = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Base Damage", 8f, "Base damage at a single stack.").Value;
			TitanicKnurl_Rework.StackDamage = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Stack Damage", 6f, "Base damage for each additional stack.").Value;
			TitanicKnurl_Rework.BaseCooldown = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Base Cooldown", 8f, "Cooldown at a single stack.").Value;
			TitanicKnurl_Rework.StackCooldown = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Stack Cooldown", 0.15f, "Cooldown rate for each additional stack.").Value;
			TitanicKnurl_Rework.ProcRate = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Proc Coefficient", 1.0f, "Proc coefficient of the stone fist.").Value;
			TitanicKnurl_Rework.ProcBands = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Proc Bands", true, "Should the stone fist proc bands?").Value;
			TitanicKnurl_Rework.TargetRadius = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Target Radius", 60f, "Targeting radius in metres.").Value;
			TitanicKnurl_Rework.TargetMode = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework, "Target Mode", 0, "Decides how the target is selected. (0 = Weak, 1 = Closest)").Value;

			TitanicKnurl_Rework_B.Enable = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, Label_EnableRework, false, Desc_EnableRework_B).Value;
			TitanicKnurl_Rework_B.LaserBaseDamage = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Laser Base Damage", 0.625f, "Base damage per tick of the laser at a single stack.").Value;
			TitanicKnurl_Rework_B.LaserStackDamage = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Laser Stack Damage", 0.0f, "Base damage per tick of the laser for each additional stack.").Value;
			TitanicKnurl_Rework_B.ShotBaseDamage = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Shot Base Damage", 1.5f, "Base damage the laser shots at a single stack.").Value;
			TitanicKnurl_Rework_B.ShotStackDamage = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Shot Stack Damage", 0.0f, "Base damage the laser shots for each additional stack.").Value;
			TitanicKnurl_Rework_B.BaseDuration = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Base Duration", 4.0f, "Laser duration at a single stack.").Value;
			TitanicKnurl_Rework_B.StackDuration = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Stack Duration", 2.0f, "Laser duration for each additional stack.").Value;
			TitanicKnurl_Rework_B.LaserProcRate = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Laser Proc Coefficient", 0.15f, "Proc coefficient of the laser per tick.").Value;
			TitanicKnurl_Rework_B.ShotProcRate = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Shot Proc Coefficient", 0.15f, "Proc coefficient of the laser shots.").Value;
			TitanicKnurl_Rework_B.ExtraShots = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Fire Extra Shots", 3, "Fire a laser shot if you build this much charge during the laser, set to 0 to disable this.").Value;
			TitanicKnurl_Rework_B.HitProcMult = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Hit Proc Mult", 1f, "How effective hitting is for building charge.").Value;
			TitanicKnurl_Rework_B.HurtProcMult = Item_Yellow_Config.Bind(Section_TitanicKnurl_Rework_B, "Hurt Proc Mult", 5f, "How effective being hit is for building charge.").Value;
		}
		private static void Read_DefenseNucleus()
        {
			DefenseNucleus.Enable = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			DefenseNucleus.BaseArmor = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Base Armor", 50, "How much armor allied minions get at a single stack.").Value;
			DefenseNucleus.StackArmor = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Stack Armor", 35, "How much armor allied minions get for each additional stack.").Value;
			DefenseNucleus.BaseHealth = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Base Health", 10, "Extra health the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackHealth = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Stack Health", 0, "Extra health the construct gets for each additional stack.").Value;
			DefenseNucleus.BaseAttack = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Base Attack Speed", 3, "Extra attack speed the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackAttack = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Stack Attack Speed", 0, "Extra attack speed the construct gets for each additional stack.").Value;
			DefenseNucleus.BaseDamage = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Base Damage", 0, "Extra damage the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackDamage = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, "Stack Damage", 5, "Extra damage the construct gets for each additional stack.").Value;
			DefenseNucleus.Comp_AssistManager = Item_Yellow_Config.Bind(Section_DefenseNucleus_Buff, Label_AssistManager, false, Desc_AssistManager).Value;

			DefenseNucleus_Rework.Enable = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			DefenseNucleus_Rework.SummonCount = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Summon Count", 4, "How many constructs to summon on activation. (Cannot go above 8 because I said so.)").Value;
			DefenseNucleus_Rework.BaseHealth = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Base Health", 10, "Extra health the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackHealth = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Stack Health", 10, "Extra health the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.BaseAttack = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Base Attack Speed", 3, "Extra attack speed the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackAttack = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Stack Attack Speed", 0, "Extra attack speed the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.BaseDamage = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Base Damage", 0, "Extra damage the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackDamage = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Stack Damage", 5, "Extra damage the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.ShieldBaseDuration = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Shield Base Duration", 5f, "Duration of the projectile shield at a single stack.").Value;
			DefenseNucleus_Rework.ShieldStackDuration = Item_Yellow_Config.Bind(Section_DefenseNucleus_Rework, "Shield Stack Duration", 2f, "Duration of the projectile shield for each additional stack.").Value;

			DefenseNucleus_Shared.TweakAI = Item_Yellow_Config.Bind(Section_DefenseNucleus_Shared, "Better AI", false, "Gives 360 Degree vision and prevents retaliation against allies.").Value;
			DefenseNucleus_Shared.ForceMechanical = Item_Yellow_Config.Bind(Section_DefenseNucleus_Shared, "Is Mechanical", false, "Gives it the Mechanical flag, allowing it to get Spare Drone Parts and Captain's Microbots.").Value;
			DefenseNucleus_Shared.ExtraDisplays = Item_Yellow_Config.Bind(Section_DefenseNucleus_Shared, "Modded Displays", false, "Adds Spare Drone Parts item displays to the Alpha Construct.").Value;
		}
		private static void Read_LigmaLenses()
        {
			LigmaLenses.Enable = Item_Void_Config.Bind(Section_LigmaLenses_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LigmaLenses.BaseChance = Item_Void_Config.Bind(Section_LigmaLenses_Buff, "Base Chance", 0.5f, "Detain chance at a single stack.").Value;
			LigmaLenses.StackChance = Item_Void_Config.Bind(Section_LigmaLenses_Buff, "Stack Chance", 0.5f, "Detain chance for each additional stack.").Value;
			LigmaLenses.BaseDamage = Item_Void_Config.Bind(Section_LigmaLenses_Buff, "Base Damage", 50.0f, "Base damage at a single stack.").Value;
			LigmaLenses.StackDamage = Item_Void_Config.Bind(Section_LigmaLenses_Buff, "Stack Damage", 0.0f, "Base damage for each additional stack.").Value;
			LigmaLenses.UseTotalDamage = Item_Void_Config.Bind(Section_LigmaLenses_Buff, "Deal Total", false, "Deal Total Damage of the attack instead of the attacker's damage stat?").Value;
		}
		private static void Read_VoidsentFlame()
        {
			VoidsentFlame.Enable = Item_Void_Config.Bind(Section_VoidsentFlame_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			VoidsentFlame.BaseRadius = Item_Void_Config.Bind(Section_VoidsentFlame_Buff, "Base Radius", 12f, "Blast radius at a single stack.").Value;
			VoidsentFlame.StackRadius = Item_Void_Config.Bind(Section_VoidsentFlame_Buff, "Stack Radius", 2.4f, "Extra blast radius for each additional stack.").Value;
			VoidsentFlame.BaseDamage = Item_Void_Config.Bind(Section_VoidsentFlame_Buff, "Base Damage", 2.6f, "Blast damage at a single stack.").Value;
			VoidsentFlame.StackDamage = Item_Void_Config.Bind(Section_VoidsentFlame_Buff, "Stack Damage", 1.56f, "Blast damage for each additional stack.").Value;
			VoidsentFlame.ProcRate = Item_Void_Config.Bind(Section_VoidsentFlame_Buff, "Proc Coefficient", 1f, "Blast proc coefficient.").Value;
		}
		private static void Read_NewlyHatchedZoea()
		{
			NewlyHatchedZoea_Rework.Enable = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			NewlyHatchedZoea_Rework.BaseStock = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Base Stock", 12, "How many missiles to store at a single stack.").Value;
			NewlyHatchedZoea_Rework.StackStock = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Stack Stock", 4, "Extra missiles for each additional stack.").Value;
			NewlyHatchedZoea_Rework.BaseDamage = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Base Damage", 3f, "Missile damage at a single stack.").Value;
			NewlyHatchedZoea_Rework.StackDamage = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Stack Damage", 0.75f, "Missile damage for each additional stack.").Value;
			NewlyHatchedZoea_Rework.FireDelay = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Fire Delay", 0.2f, "Time between each missile fired, scales with attack speed.").Value;
			NewlyHatchedZoea_Rework.ProcRate = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Proc Coefficient", 0.2f, "Missile proc coefficient.").Value;
			NewlyHatchedZoea_Rework.RestockTime = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Restock Time", 30, "How long it takes in seconds to fully restock.").Value;
			NewlyHatchedZoea_Rework.CanCorrupt = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Allow Corruption", true, "Set to false to disable the item corruption effect.").Value;
			NewlyHatchedZoea_Rework.CorruptList = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Corruption List", "", "List of items that this item will corrupt. (Leave blank for vanilla values.)").Value;
			NewlyHatchedZoea_Rework.CorruptText = Item_Void_Config.Bind(Section_NewlyHatchedZoea_Rework, "Corruption Text", "<style=cIsTierBoss>yellow items</style>", "The item(s) for the \"Corrupts all X\" text.").Value;
		}

		private static void Read_SearedSteak()
		{
			SearedSteak.Enable = Item_Food_Config.Bind(Section_SearedSteak_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			SearedSteak.BaseHP = Item_Food_Config.Bind(Section_SearedSteak_Buff, "Base HP", 20f, "Health each stack gives.").Value;
			SearedSteak.LevelHP = Item_Food_Config.Bind(Section_SearedSteak_Buff, "Level HP", 6f, "Health each stack gives per level.").Value;
			SearedSteak.BasePercentHP = Item_Food_Config.Bind(Section_SearedSteak_Buff, "Percent HP", 0.05f, "Percent Health each stack gives.").Value;

			SearedSteak_Rework.Enable = Item_Food_Config.Bind(Section_SearedSteak_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			SearedSteak_Rework.BasePercentHP = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Percent HP", 0.05f, "Percent Health each stack gives.").Value;
			SearedSteak_Rework.BaseRegen = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Base Regen", 2f, "Health regen at a single stack. (Scales with level)").Value;
			SearedSteak_Rework.StackRegen = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Stack Regen", 0f, "Health regen for each additional stack. (Scales with level)").Value;
			SearedSteak_Rework.BaseDuration = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Base Regen Duration", 3f, "Duration of the regen buff at a single stack.").Value;
			SearedSteak_Rework.StackDuration = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Stack Regen Duration", 3f, "Duration of the regen buff for each additional stack.").Value;
			SearedSteak_Rework.ExtendDuration = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Extend Duration", 1f, "How much to extend the effect duration on kill.").Value;
			SearedSteak_Rework.NerfFakeKill = Item_Food_Config.Bind(Section_SearedSteak_Rework, "Nerf Fake Kills", false, "Prevents fake kills from extending the duration.").Value;
			SearedSteak_Rework.Comp_AssistManager = Item_Food_Config.Bind(Section_SearedSteak_Rework, Label_AssistManager, true, Desc_AssistManager).Value;
		}
	}
}
