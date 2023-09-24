using System;
using System.IO;
using RoR2;
using R2API;
using FlatItemBuff.Items;
using FlatItemBuff.Artifacts;
using UnityEngine;
using BepInEx.Configuration;

namespace FlatItemBuff
{
	//99% of this is practically ripped off from RiskyMod, ain't gonna lie.
	public static class Configs
	{
		public static ConfigFile MainConfig;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_Steak_Buff = "Bison Steak";

		private const string Section_Brooch_Buff = "Topaz Brooch";

		private const string Section_LeptonDaisy_Buff = "Lepton Daisy";

		private const string Section_LeechingSeed_Buff = "Leeching Seed";
		private const string Section_LeechingSeed_Rework = "Leeching Seed Rework";

		private const string Section_Infusion_Buff = "Infusion";
		private const string Section_Infusion_Rework = "Infusion Rework";

		private const string Section_WarHorn_Buff = "War Horn";

		private const string Section_HuntersHarpoon_Buff = "Hunters Harpoon";

		private const string Section_Stealthkit_Buff = "Old War Stealthkit";

		private const string Section_SquidPolyp_Buff = "Squid Polyp";

		private const string Section_WaxQuail_Buff = "Wax Quail";

		private const string Section_BensRaincoat_Buff = "Bens Raincoat";

		private const string Section_LaserScope_Buff = "Laser Scope";

		private const string Section_Aegis_Buff = "Aegis";

		private const string Section_Planula_Buff = "Planula";
		private const string Section_Planula_Rework = "Planula Rework";

		private const string Section_TitanicKnurl_Buff = "Titanic Knurl";
		private const string Section_TitanicKnurl_Rework = "Titanic Knurl Rework";

		private const string Section_DefenseNucleus_Buff = "Defense Nucleus";
		private const string Section_DefenseNucleus_Rework = "Defense Nucleus Rework";
		private const string Section_DefenseNucleus_Shared = "Alpha Construct Ally";

		private const string Section_LigmaLenses_Buff = "Lost Seers Lenses";

		private const string Section_VoidsentFlame_Buff = "Voidsent Flame";

		private const string Section_NewlyHatchedZoea_Rework = "Newly Hatched Zoea Rework";

		private const string Section_Artifact_Spite = "Artifact of Spite";

		private const string Label_EnableBuff = "Enable Changes";
		private const string Label_EnableRework = "Enable Rework";

		private const string Desc_EnableBuff = "Enables changes for this item.";
		private const string Desc_EnableRework = "Enables the rework for this item. Has priority over the the normal changes.";
		
		public static void Setup()
        {
			MainConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"MainConfig.cfg"), true);
			//Common
			Read_Steak();
			Read_Brooch();
			//Uncommon
			Read_Infusion();
			Read_LeechingSeed();
			Read_Stealthkit();
			Read_HuntersHarpoon();
			Read_SquidPolyp();
			Read_WarHorn();
			Read_LeptonDaisy();
			Read_WaxQuail();
			//Legendary
			Read_BensRaincoat();
			Read_LaserScope();
			Read_Aegis();
			//Boss
			Read_Planula();
			Read_TitanicKnurl();
			Read_DefenseNucleus();
			//Void
			Read_LigmaLenses();
			Read_VoidsentFlame();
			Read_NewlyHatchedZoea();
			//Artifacts
			Read_ArtifactSpite();
		}
		private static void Read_Steak()
        {
			BisonSteak.Enable = MainConfig.Bind(Section_Steak_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			BisonSteak.BaseHP = MainConfig.Bind(Section_Steak_Buff, "Base HP", 20f, "Health each stack gives.").Value;
			BisonSteak.LevelHP = MainConfig.Bind(Section_Steak_Buff, "Level HP", 2f, "Health each stack gives per level.").Value;
			BisonSteak.BaseDuration = MainConfig.Bind(Section_Steak_Buff, "Base Regen Duration", 3f, "Duration of the regen buff at a single stack.").Value;
			BisonSteak.StackDuration = MainConfig.Bind(Section_Steak_Buff, "Stack Regen Duration", 3f, "Duration of the regen buff for each additional stack.").Value;
		}
		private static void Read_Brooch()
        {
			TopazBrooch.Enable = MainConfig.Bind(Section_Brooch_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			TopazBrooch.BaseFlatBarrier = MainConfig.Bind(Section_Brooch_Buff, "Base Flat Barrier", 15.0f, "Flat amount of barrier given at a single stack.").Value;
			TopazBrooch.StackFlatBarrier = MainConfig.Bind(Section_Brooch_Buff, "Stack Flat Barrier", 15.0f, "Flat amount of barrier given at for each additional stack.").Value;
			TopazBrooch.BaseCentBarrier = MainConfig.Bind(Section_Brooch_Buff, "Base Percent Barrier", 0.005f, "Percent amount of barrier given at a single stack.").Value;
			TopazBrooch.StackCentBarrier = MainConfig.Bind(Section_Brooch_Buff, "Stack Percent Barrier", 0.005f, "Percent amount of barrier given at for each additional stack.").Value;
		}
		private static void Read_LeptonDaisy()
        {
			LeptonDaisy.Enable = MainConfig.Bind(Section_LeptonDaisy_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			LeptonDaisy.BaseHeal = MainConfig.Bind(Section_LeptonDaisy_Buff, "Base Healing", 0.15f, "Healing at a single stack.").Value;
			LeptonDaisy.StackHeal = MainConfig.Bind(Section_LeptonDaisy_Buff, "Stack Healing", 0.1f, "Healing for each additional stack.").Value;
			LeptonDaisy.CapHeal = MainConfig.Bind(Section_LeptonDaisy_Buff, "Capped Healing", 2f, "Hyperbolic healing limit. (Set ot 0 or less to disable.)").Value;
			LeptonDaisy.Cooldown = MainConfig.Bind(Section_LeptonDaisy_Buff, "Cooldown", 10f, "Cooldown of the healing nova.").Value;
		}
		private static void Read_Infusion()
        {
			Infusion.Enable = MainConfig.Bind(Section_Infusion_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			Infusion.StackLevel = MainConfig.Bind(Section_Infusion_Buff, "Stack Level", 2, "Number of levels gained per stack.").Value;
			Infusion.Infinite = MainConfig.Bind(Section_Infusion_Buff, "Soft Cap", true, "Allows samples to be collected beyond the cap with diminishing returns.").Value;
			Infusion.Inherit = MainConfig.Bind(Section_Infusion_Buff, "Inherit", true, "Should your minions with infusions inherit your count.").Value;
			Infusion.ChampionGain = MainConfig.Bind(Section_Infusion_Buff, "Champion Value", 5, "Sample value of champion enemies. (Wandering Vagrant, Magma Worm, etc)").Value;
			Infusion.EliteGainMult = MainConfig.Bind(Section_Infusion_Buff, "Elite Multiplier", 3, "Sample value multiplier from elite enemies.").Value;
			Infusion.BossGainMult = MainConfig.Bind(Section_Infusion_Buff, "Boss Multiplier", 2, "Sample value multiplier from boss enemies.").Value;

			Infusion_Rework.Enable = MainConfig.Bind(Section_Infusion_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			Infusion_Rework.StackGain = MainConfig.Bind(Section_Infusion_Rework, "Stack Gain", 0.5f, "Blood gain for each additional stack.").Value;
			Infusion_Rework.FakeBaseGain = MainConfig.Bind(Section_Infusion_Rework, "Fake Kill Gain", 10, "Base blood gain from fake kills.").Value;
			Infusion_Rework.CloneCost = MainConfig.Bind(Section_Infusion_Rework, "Clone Cost", 400, "The amount of blood needed to create a clone.").Value;
			Infusion_Rework.LevelCost = MainConfig.Bind(Section_Infusion_Rework, "Level Cost", 4000, "The amount of blood needed to double the clone's level.").Value;
			Infusion_Rework.CustomLeash = MainConfig.Bind(Section_Infusion_Rework, "Leash Distance", 90f, "Sets a custom leash distance for the clone. (Set to 0 or less to use the normal minion leash.)").Value;
		}
		private static void Read_LeechingSeed()
        {
			LeechingSeed.Enable = MainConfig.Bind(Section_LeechingSeed_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			LeechingSeed.ProcHeal = MainConfig.Bind(Section_LeechingSeed_Buff, "Proc Healing", 0.75f, "Healing amount that's affected by proc coefficient.").Value;
			LeechingSeed.BaseHeal = MainConfig.Bind(Section_LeechingSeed_Buff, "Base Healing", 0.75f, "Extra healing amount regardless of proc coefficient.").Value;

			LeechingSeed_Rework.Enable = MainConfig.Bind(Section_LeechingSeed_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			LeechingSeed_Rework.HealFromDoT = MainConfig.Bind(Section_LeechingSeed_Rework, "DoT Healing", 2f, "Healing amount given from damage over time ticks.").Value;
			LeechingSeed_Rework.LeechChance = MainConfig.Bind(Section_LeechingSeed_Rework, "Leech Chance", 25f, "Chance of applying the Leech debuff.").Value;
			LeechingSeed_Rework.LeechLifeSteal = MainConfig.Bind(Section_LeechingSeed_Rework, "Leech Life Steal", 0.02f, "Percent of damage received as healing when damaging a target with Leech.").Value;
			LeechingSeed_Rework.LeechMinLifeSteal = MainConfig.Bind(Section_LeechingSeed_Rework, "Leech Minimum Life Steal", 0.5f, "Minimum amount of healing received from Leech.").Value;
			LeechingSeed_Rework.LeechBaseDamage = MainConfig.Bind(Section_LeechingSeed_Rework, "Leech Base Damage", 0.5f, "Damage dealt per second from Leech.").Value;
			LeechingSeed_Rework.LeechBaseDuration = MainConfig.Bind(Section_LeechingSeed_Rework, "Leech Base Duration", 5f, "How long Leech is applied for.").Value;
			LeechingSeed_Rework.LeechStackDuration = MainConfig.Bind(Section_LeechingSeed_Rework, "Leech Stack Duration", 0f, "How much longer Leech is applied for each additional stack.").Value;
		}
		private static void Read_WaxQuail()
        {
			WaxQuail.Enable = MainConfig.Bind(Section_WaxQuail_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			WaxQuail.BaseHori = MainConfig.Bind(Section_WaxQuail_Buff, "Base Horizontal Boost", 12f, "Horizontal force at a single stack.").Value;
			WaxQuail.StackHori = MainConfig.Bind(Section_WaxQuail_Buff, "Stack Horizontal Boost", 6f, "Horizontal force for each additional stack.").Value;
			WaxQuail.CapHori = MainConfig.Bind(Section_WaxQuail_Buff, "Capped Horizontal Boost", 240f, "Hyperbolic cap to horizontal force. (Set to 0 or less to disable.)").Value;
			WaxQuail.BaseVert = MainConfig.Bind(Section_WaxQuail_Buff, "Base Vertical Boost", 0.2f, "Vertical force at a single stack.").Value;
			WaxQuail.StackVert = MainConfig.Bind(Section_WaxQuail_Buff, "Stack Vertical", 0f, "Vertical force for each additional stack.").Value;
			WaxQuail.CapVert = MainConfig.Bind(Section_WaxQuail_Buff, "Capped Vertical Boost", 0f, "Hyperbolic cap to vertical force. (Set to 0 or less to disable.)").Value;
			WaxQuail.BaseAirSpeed = MainConfig.Bind(Section_WaxQuail_Buff, "Base Air Speed", 0.14f, "Airborne movement speed at a single stack.").Value;
			WaxQuail.StackAirSpeed = MainConfig.Bind(Section_WaxQuail_Buff, "Stack Air Speed", 0.07f, "Airborne movement speed for each additional stack.").Value;
			WaxQuail.CapAirSpeed = MainConfig.Bind(Section_WaxQuail_Buff, "Capped Air Speed", 2.8f, "Hyperbolic cap to airborne movement speed. (Set to 0 or less to disable.)").Value;
		}

		private static void Read_Stealthkit()
        {
			Stealthkit.Enable = MainConfig.Bind(Section_Stealthkit_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			Stealthkit.BaseRecharge = MainConfig.Bind(Section_Stealthkit_Buff, "Base Cooldown", 30f, "Cooldown between uses.").Value;
			Stealthkit.StackRecharge = MainConfig.Bind(Section_Stealthkit_Buff, "Stack Cooldown", 0.5f, "Cooldown rate for each additional stack.").Value;
			Stealthkit.BuffDuration = MainConfig.Bind(Section_Stealthkit_Buff, "Buff Duration", 5f, "Duration of the Stealth buff.").Value;
			Stealthkit.GraceDuration = MainConfig.Bind(Section_Stealthkit_Buff, "Cancel Duration", 0.5f, "How long to force Combat and Danger cancels upon activation.").Value;
			Stealthkit.CancelCombat = MainConfig.Bind(Section_Stealthkit_Buff, "Cancel Combat", true, "Puts you in 'Out of Combat' upon activation.").Value;
			Stealthkit.CancelDanger = MainConfig.Bind(Section_Stealthkit_Buff, "Cancel Danger", true, "Puts you in 'Out of Danger' upon activation.").Value;
			Stealthkit.CleanseDoT = MainConfig.Bind(Section_Stealthkit_Buff, "Clean DoTs", true, "Removes damage over time effects upon activation.").Value;
		}
		private static void Read_HuntersHarpoon()
        {
			HuntersHarpoon.Enable = MainConfig.Bind(Section_HuntersHarpoon_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			HuntersHarpoon.BaseDuration = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Base Duration", 1.5f, "Buff duration at a single stack.").Value;
			HuntersHarpoon.StackDuration = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Stack Duration", 0.75f, "Extra buff duration for each additional stack.").Value;
			HuntersHarpoon.MovementSpeed = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Movement Speed Bonus", 0.25f, "Movement speed each stack of the buff gives.").Value;
			HuntersHarpoon.CooldownRate = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Rate Bonus", 0.25f, "Cooldown rate each stack of the buff gives.").Value;
			HuntersHarpoon.CoolPrimary = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Primary", true, "Cooldown rate affects Primary skills?").Value;
			HuntersHarpoon.CoolSecondary = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Secondary", true, "Cooldown rate affects Secondary skills?").Value;
			HuntersHarpoon.CoolUtility = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Utility", false, "Cooldown rate affects Utility skills?").Value;
			HuntersHarpoon.CoolSpecial = MainConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Special", false, "Cooldown rate affects Special skills?").Value;
		}
		private static void Read_SquidPolyp()
        {
			SquidPolyp.Enable = MainConfig.Bind(Section_SquidPolyp_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			SquidPolyp.ApplyTar = MainConfig.Bind(Section_SquidPolyp_Buff, "Apply Tar", true, "Makes Squid Turrets apply the Tar debuff with their attack.").Value;
			SquidPolyp.RemoveDelay = MainConfig.Bind(Section_SquidPolyp_Buff, "Inactive Removal", 30f, "Kills Squid Turrets if they've been inactive for this long in seconds.").Value;
			SquidPolyp.StackDuration = MainConfig.Bind(Section_SquidPolyp_Buff, "Stack Duration", 3, "Squid Turret duration per stack.").Value;
			SquidPolyp.StackArmor = MainConfig.Bind(Section_SquidPolyp_Buff, "Stack Armor", 10f, "Squid Turret armor per stack.").Value;
		}
		private static void Read_WarHorn()
        {
			WarHorn.Enable = MainConfig.Bind(Section_WarHorn_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			WarHorn.BaseDuration = MainConfig.Bind(Section_WarHorn_Buff, "Base Duration", 6f, "Duration at a single stack.").Value;
			WarHorn.StackDuration = MainConfig.Bind(Section_WarHorn_Buff, "Stack Duration", 2f, "Duration for each additional stack.").Value;
			WarHorn.BaseAttack = MainConfig.Bind(Section_WarHorn_Buff, "Base Attack", 0.6f, "Attack Speed at a single stack.").Value;
			WarHorn.StackAttack = MainConfig.Bind(Section_WarHorn_Buff, "Stack Attack", 0.15f, "Attack Speed for each additional stack.").Value;
		}
		private static void Read_BensRaincoat()
        {
			BensRaincoat.Enable = MainConfig.Bind(Section_BensRaincoat_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			BensRaincoat.BaseBlock = MainConfig.Bind(Section_BensRaincoat_Buff, "Base Block", 2, "Debuff blocks to give at a single stack.").Value;
			BensRaincoat.StackBlock = MainConfig.Bind(Section_BensRaincoat_Buff, "Stack Block", 2, "Debuff blocks to give for each additional stack.").Value;
			BensRaincoat.Cooldown = MainConfig.Bind(Section_BensRaincoat_Buff, "Cooldown", 7f, "Time in seconds it takes to restock debuff blocks. (Anything less than 0 will skip this change.)").Value;
		}
		private static void Read_LaserScope()
        {
			LaserScope.Enable = MainConfig.Bind(Section_LaserScope_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			LaserScope.BaseCrit = MainConfig.Bind(Section_LaserScope_Buff, "Crit Chance", 5f, "Crit chance at a single stack.").Value;
		}
		private static void Read_Aegis()
        {
			Aegis.Enable = MainConfig.Bind(Section_Aegis_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			Aegis.AllowRegen = MainConfig.Bind(Section_Aegis_Buff, "Count Regen", true, "Allows excess regen to be converted into barrier.").Value;
			Aegis.Armor = MainConfig.Bind(Section_Aegis_Buff, "Armor", 20f, "Armor each stack gives.").Value;
		}
		private static void Read_Planula()
        {
			Planula.Enable = MainConfig.Bind(Section_Planula_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			Planula.BaseFlatHeal = MainConfig.Bind(Section_Planula_Buff, "Base Flat Healing", 10f, "Flat healing at a single stack.").Value;
			Planula.StackFlatHeal = MainConfig.Bind(Section_Planula_Buff, "Stack Flat Healing", 10f, "Flat healing for each additional stack.").Value;
			Planula.BaseMaxHeal = MainConfig.Bind(Section_Planula_Buff, "Base Max Healing", 0.02f, "Percent healing at a single stack.").Value;
			Planula.StackMaxHeal = MainConfig.Bind(Section_Planula_Buff, "Stack Max Healing", 0.02f, "Percent healing for each additional stack.").Value;

			Planula_Rework.Enable = MainConfig.Bind(Section_Planula_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			Planula_Rework.BaseDamage = MainConfig.Bind(Section_Planula_Rework, "Base Damage", 1f, "Damage per second the burn deals at a single stack.").Value;
			Planula_Rework.StackDamage = MainConfig.Bind(Section_Planula_Rework, "Base Damage", 1f, "Damage per second the burn deals for each additional stack.").Value;
			Planula_Rework.Duration = MainConfig.Bind(Section_Planula_Rework, "Burn Duration", 5f, "Duration of the burn.").Value;
			Planula_Rework.Radius = MainConfig.Bind(Section_Planula_Rework, "Burn Radius", 15f, "Radius for enemies to be within to start burning.").Value;
		}
		private static void Read_TitanicKnurl()
        {
			TitanicKnurl.Enable = MainConfig.Bind(Section_TitanicKnurl_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			TitanicKnurl.BaseHP = MainConfig.Bind(Section_TitanicKnurl_Buff, "Base Health", 40f, "Health each stack gives.").Value;
			TitanicKnurl.LevelHP = MainConfig.Bind(Section_TitanicKnurl_Buff, "Level Health", 4f, "Health each stack gives per level.").Value;
			TitanicKnurl.BaseRegen = MainConfig.Bind(Section_TitanicKnurl_Buff, "Base Regen", 1.6f, "Health Regen each stack gives.").Value;
			TitanicKnurl.LevelRegen = MainConfig.Bind(Section_TitanicKnurl_Buff, "Level Regen", 0.32f, "Health Regen each stack gives per level.").Value;
			
			TitanicKnurl_Rework.Enable = MainConfig.Bind(Section_TitanicKnurl_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			TitanicKnurl_Rework.BaseDamage = MainConfig.Bind(Section_TitanicKnurl_Rework, "Base Damage", 8f, "Base damage at a single stack.").Value;
			TitanicKnurl_Rework.StackDamage = MainConfig.Bind(Section_TitanicKnurl_Rework, "Stack Damage", 6f, "Base damage for each additional stack.").Value;
			TitanicKnurl_Rework.BaseCooldown = MainConfig.Bind(Section_TitanicKnurl_Rework, "Base Cooldown", 6f, "Cooldown at a single stack.").Value;
			TitanicKnurl_Rework.StackCooldown = MainConfig.Bind(Section_TitanicKnurl_Rework, "Stack Cooldown", 0.15f, "Cooldown rate for each additional stack.").Value;
			TitanicKnurl_Rework.ProcRate = MainConfig.Bind(Section_TitanicKnurl_Rework, "Proc Coefficient", 1.0f, "Proc coefficient of the stone fist.").Value;
			TitanicKnurl_Rework.ProcBands = MainConfig.Bind(Section_TitanicKnurl_Rework, "Proc Bands", true, "Should the stone fist proc bands?").Value;
			TitanicKnurl_Rework.TargetRadius = MainConfig.Bind(Section_TitanicKnurl_Rework, "Target Radius", 60f, "Targeting radius in metres.").Value;
			TitanicKnurl_Rework.TargetMode = MainConfig.Bind(Section_TitanicKnurl_Rework, "Target Mode", 0, "Decides how the target is selected. (0 = Weak, 1 = Closest)").Value;
		}
		private static void Read_DefenseNucleus()
        {
			DefenseNucleus.Enable = MainConfig.Bind(Section_DefenseNucleus_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			DefenseNucleus.BaseHealth = MainConfig.Bind(Section_DefenseNucleus_Buff, "Base Health", 10, "Extra health the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackHealth = MainConfig.Bind(Section_DefenseNucleus_Buff, "Stack Health", 10, "Extra health the construct gets for each additional stack.").Value;
			DefenseNucleus.BaseAttack = MainConfig.Bind(Section_DefenseNucleus_Buff, "Base Attack Speed", 6, "Extra attack speed the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackAttack = MainConfig.Bind(Section_DefenseNucleus_Buff, "Stack Attack Speed", 0, "Extra attack speed the construct gets for each additional stack.").Value;
			DefenseNucleus.BaseDamage = MainConfig.Bind(Section_DefenseNucleus_Buff, "Base Damage", 6, "Extra damage the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackDamage = MainConfig.Bind(Section_DefenseNucleus_Buff, "Stack Damage", 0, "Extra damage the construct gets for each additional stack.").Value;
			DefenseNucleus.Cooldown = MainConfig.Bind(Section_DefenseNucleus_Buff, "Cooldown", 1f, "Cooldown for summoning constructs.").Value;

			DefenseNucleus_Rework.Enable = MainConfig.Bind(Section_DefenseNucleus_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			DefenseNucleus_Rework.SummonCount = MainConfig.Bind(Section_DefenseNucleus_Rework, "Summon Count", 3, "How many constructs to summon on activation. (Cannot go above 6 because I said so.)").Value;
			DefenseNucleus_Rework.BaseHealth = MainConfig.Bind(Section_DefenseNucleus_Rework, "Base Health", 10, "Extra health the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackHealth = MainConfig.Bind(Section_DefenseNucleus_Rework, "Stack Health", 10, "Extra health the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.BaseAttack = MainConfig.Bind(Section_DefenseNucleus_Rework, "Base Attack Speed", 6, "Extra attack speed the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackAttack = MainConfig.Bind(Section_DefenseNucleus_Rework, "Stack Attack Speed", 0, "Extra attack speed the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.BaseDamage = MainConfig.Bind(Section_DefenseNucleus_Rework, "Base Damage", 6, "Extra damage the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackDamage = MainConfig.Bind(Section_DefenseNucleus_Rework, "Stack Damage", 0, "Extra damage the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.ShieldBaseDuration = MainConfig.Bind(Section_DefenseNucleus_Rework, "Shield Base Duration", 3.5f, "Duration of the projectile shield at a single stack.").Value;
			DefenseNucleus_Rework.ShieldStackDuration = MainConfig.Bind(Section_DefenseNucleus_Rework, "Shield Stack Duration", 1f, "Duration of the projectile shield for each additional stack.").Value;

			DefenseNucleus_Shared.TweakAI = MainConfig.Bind(Section_DefenseNucleus_Shared, "Better AI", true, "Gives 360 Degree vision and prevents retaliation against allies.").Value;
			DefenseNucleus_Shared.ForceMechanical = MainConfig.Bind(Section_DefenseNucleus_Shared, "Is Mechanical", true, "Gives it the Mechanical flag, allowing it to get Spare Drone Parts and Captain's Microbots.").Value;
			DefenseNucleus_Shared.ExtraDisplays = MainConfig.Bind(Section_DefenseNucleus_Shared, "Modded Displays", true, "Adds Spare Drone Parts item displays to the Alpha Construct.").Value;
		}
		private static void Read_LigmaLenses()
        {
			LigmaLenses.Enable = MainConfig.Bind(Section_LigmaLenses_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			LigmaLenses.BaseChance = MainConfig.Bind(Section_LigmaLenses_Buff, "Base Chance", 0.5f, "Detain chance at a single stack.").Value;
			LigmaLenses.StackChance = MainConfig.Bind(Section_LigmaLenses_Buff, "Stack Chance", 0.5f, "Detain chance for each additional stack.").Value;
			LigmaLenses.BaseDamage = MainConfig.Bind(Section_LigmaLenses_Buff, "Base Damage", 50.0f, "Base damage at a single stack.").Value;
			LigmaLenses.StackDamage = MainConfig.Bind(Section_LigmaLenses_Buff, "Stack Damage", 0.0f, "Base damage for each additional stack.").Value;
			LigmaLenses.UseTotalDamage = MainConfig.Bind(Section_LigmaLenses_Buff, "Deal Total", false, "Deal Total Damage of the attack instead of the attacker's damage stat?").Value;
		}

		private static void Read_VoidsentFlame()
        {
			VoidsentFlame.Enable = MainConfig.Bind(Section_VoidsentFlame_Buff, Label_EnableBuff, true, Desc_EnableBuff).Value;
			VoidsentFlame.BaseRadius = MainConfig.Bind(Section_VoidsentFlame_Buff, "Base Radius", 10f, "Blast radius at a single stack.").Value;
			VoidsentFlame.StackRadius = MainConfig.Bind(Section_VoidsentFlame_Buff, "Stack Radius", 2f, "Extra blast radius for each additional stack.").Value;
			VoidsentFlame.BaseDamage = MainConfig.Bind(Section_VoidsentFlame_Buff, "Base Damage", 2.6f, "Blast damage at a single stack.").Value;
			VoidsentFlame.StackDamage = MainConfig.Bind(Section_VoidsentFlame_Buff, "Stack Damage", 1.56f, "Blast damage for each additional stack.").Value;
			VoidsentFlame.ProcRate = MainConfig.Bind(Section_VoidsentFlame_Buff, "Proc Coefficient", 1f, "Blast proc coefficient.").Value;
		}

		private static void Read_NewlyHatchedZoea()
		{
			NewlyHatchedZoea_Rework.Enable = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			NewlyHatchedZoea_Rework.BaseStock = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, "Base Stock", 12, "How many missiles to store at a single stack.").Value;
			NewlyHatchedZoea_Rework.StackStock = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, "Stack Stock", 4, "Extra missiles for each additional stack.").Value;
			NewlyHatchedZoea_Rework.BaseDamage = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, "Base Damage", 3f, "Missile damage at a single stack.").Value;
			NewlyHatchedZoea_Rework.StackDamage = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, "Stack Damage", 0.75f, "Missile damage for each additional stack.").Value;
			NewlyHatchedZoea_Rework.ProcRate = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, "Proc Coefficient", 0.2f, "Missile proc coefficient.").Value;
			NewlyHatchedZoea_Rework.RestockTime = MainConfig.Bind(Section_NewlyHatchedZoea_Rework, "Restock Time", 30, "How long it takes in seconds to fully restock.").Value;
		}
		private static void Read_ArtifactSpite()
        {
			Spite.Enable = MainConfig.Bind(Section_Artifact_Spite, Label_EnableBuff, false, "Enables changes to this Artifact").Value;
			Spite.BaseDamage = MainConfig.Bind(Section_Artifact_Spite, "Base Damage", 12f, "Base damage of Spite bombs.").Value;
			Spite.LevelDamage = MainConfig.Bind(Section_Artifact_Spite, "Stack Damage", 2.4f, "Extra damage Spite bombs gain per victim's level.").Value;
		}
	}
}
