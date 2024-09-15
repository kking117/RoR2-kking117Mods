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
		public static ConfigFile GeneralConfig;
		public static ConfigFile ItemConfig;
		public static ConfigFile ArtifactConfig;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_AntlerShield_Rework = "Antler Shield Rework";

		private const string Section_BisonSteak_Buff = "Bison Steak";
		private const string Section_BisonSteak_Rework = "Bison Steak Rework";

		private const string Section_KnockbackFin_Buff = "Knockback Fin";

		private const string Section_TopazBrooch_Buff = "Topaz Brooch";

		private const string Section_RollOfPennies_Rework = "Roll of Pennies Rework";

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

		private const string Section_UnstableTransmitter_Rework = "Unstable Transmitter";

		private const string Section_WaxQuail_Buff = "Wax Quail";

		private const string Section_Aegis_Buff = "Aegis";

		private const string Section_BensRaincoat_Buff = "Bens Raincoat";

		private const string Section_HappiestMask_Rework = "Happiest Mask Rework";

		private const string Section_LaserScope_Buff = "Laser Scope";

		private const string Section_SymbioticScorpion_Rework = "Symbiotic Scorpion Rework";

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

		private const string Section_General_Bugs = "Bug Fixes";
		private const string Section_General_Mechanics = "Mechanics";

		private const string Label_AssistManager = "Enable Kill Assists";
		private const string Desc_AssistManager = "Allows on kill effects from this item to work with AssistManager.";

		public static void Setup()
        {
			GeneralConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"General.cfg"), true);
			ItemConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Items.cfg"), true);
			ArtifactConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Artifacts.cfg"), true);
			//Common
			Read_AntlerShield();
			Read_BisonSteak();
			Read_KnockbackFin();
			Read_TopazBrooch();
			Read_RollOfPennies();
			//Uncommon
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
			Read_WarHorn();
			Read_WaxQuail();
			//Legendary
			Read_Aegis();
			Read_BensRaincoat();
			Read_HappiestMask_Rework();
			Read_LaserScope();
			Read_SymbioticScorpion();
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
			//General
			Read_General();
		}
		private static void Read_General()
        {
			//Section_Bugs
			GeneralChanges.TweakBarrierDecay = GeneralConfig.Bind(Section_General_Mechanics, "Tweak Barrier Decay", false, "Changes barrier decay to scale from max health + shields instead of max barrier, recommended and specifically catered for Aegis changes.").Value;
		}

		private static void Read_AntlerShield()
        {
			AntlerShield_Rework.Enable = ItemConfig.Bind(Section_AntlerShield_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			AntlerShield_Rework.StackArmor = ItemConfig.Bind(Section_AntlerShield_Rework, "Stack Armor", 5f, "Armor each stack gives.").Value;
			AntlerShield_Rework.StackSpeed = ItemConfig.Bind(Section_AntlerShield_Rework, "Stack Movement Speed", 0.07f, "Movement speed each stack gives.").Value;
		}
		private static void Read_BisonSteak()
        {
			BisonSteak.Enable = ItemConfig.Bind(Section_BisonSteak_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			BisonSteak.BaseHP = ItemConfig.Bind(Section_BisonSteak_Buff, "Base HP", 10f, "Health each stack gives.").Value;
			BisonSteak.LevelHP = ItemConfig.Bind(Section_BisonSteak_Buff, "Level HP", 3f, "Health each stack gives per level.").Value;

			BisonSteak_Rework.Enable = ItemConfig.Bind(Section_BisonSteak_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			BisonSteak_Rework.BaseRegen = ItemConfig.Bind(Section_BisonSteak_Rework, "Base Regen", 1f, "Health regen at a single stack. (Scales with level)").Value;
			BisonSteak_Rework.StackRegen = ItemConfig.Bind(Section_BisonSteak_Rework, "Stack Regen", 0f, "Health regen for each additional stack. (Scales with level)").Value;
			BisonSteak_Rework.BaseDuration = ItemConfig.Bind(Section_BisonSteak_Rework, "Base Regen Duration", 3f, "Duration of the regen buff at a single stack.").Value;
			BisonSteak_Rework.StackDuration = ItemConfig.Bind(Section_BisonSteak_Rework, "Stack Regen Duration", 3f, "Duration of the regen buff for each additional stack.").Value;
			BisonSteak_Rework.ExtendDuration = ItemConfig.Bind(Section_BisonSteak_Rework, "Extend Duration", 1f, "How much to extend the effect duration on kill.").Value;
			BisonSteak_Rework.NerfFakeKill = ItemConfig.Bind(Section_BisonSteak_Rework, "Nerf Fake Kills", false, "Prevents fake kills from extending the duration.").Value;
			BisonSteak_Rework.Comp_AssistManager = ItemConfig.Bind(Section_BisonSteak_Rework, Label_AssistManager, true, Desc_AssistManager).Value;
		}

		private static void Read_KnockbackFin()
		{
			KnockbackFin.Enable = ItemConfig.Bind(Section_KnockbackFin_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			KnockbackFin.BaseForce = ItemConfig.Bind(Section_KnockbackFin_Buff, "Base Force", 20.0f, "Vertical force at a single stack.").Value;
			KnockbackFin.StackForce = ItemConfig.Bind(Section_KnockbackFin_Buff, "Stack Force", 2.0f, "Vertical force for each additional stack.").Value;
			KnockbackFin.BackForce = ItemConfig.Bind(Section_KnockbackFin_Buff, "Push Force", 0.5f, "How much to push away, is multiplied by the vertical force.").Value;
			KnockbackFin.ChampionMult = ItemConfig.Bind(Section_KnockbackFin_Buff, "Champion Force Mult", 0.5f, "Force multiplier for champion targets.").Value;
			KnockbackFin.BossMult = ItemConfig.Bind(Section_KnockbackFin_Buff, "Boss Force Mult", 1f, "Force multiplier for boss targets.").Value;
			KnockbackFin.FlyingMult = ItemConfig.Bind(Section_KnockbackFin_Buff, "Flying Force Mult", 1f, "Force multiplier for flying targets.").Value;
			KnockbackFin.MaxForce = ItemConfig.Bind(Section_KnockbackFin_Buff, "Max Force", 200f, "The limit on how much force can be gained from stacking the item.").Value;
			KnockbackFin.BaseRadius = ItemConfig.Bind(Section_KnockbackFin_Buff, "Base Radius", 12f, "Radius in metres for the impact. (Set to 0 to completely disable the Impact and its damage.)").Value;
			KnockbackFin.BaseDamage = ItemConfig.Bind(Section_KnockbackFin_Buff, "Base Damage", 1f, "Impact damage at a single stack.").Value;
			KnockbackFin.StackDamage = ItemConfig.Bind(Section_KnockbackFin_Buff, "Stack Damage", 0.1f, "Impact damage for each additional stack.").Value;
			KnockbackFin.MaxDistDamage = ItemConfig.Bind(Section_KnockbackFin_Buff, "Velocity Damage", 10f, "Maximum damage multiplier that can be achieved through velocity.").Value;
			KnockbackFin.ProcRate = ItemConfig.Bind(Section_KnockbackFin_Buff, "Proc Coefficient", 0f, "Impact proc coefficient. (It can proc itself)").Value;
			KnockbackFin.DoStun = ItemConfig.Bind(Section_KnockbackFin_Buff, "Stun", true, "Stuns launched targets when they impact the ground.").Value;
			KnockbackFin.CreditFall = ItemConfig.Bind(Section_KnockbackFin_Buff, "Credit Fall Damage", false, "Credits any fall damage the target takes to the inflictor of the knockback.").Value;
			KnockbackFin.Cooldown = ItemConfig.Bind(Section_KnockbackFin_Buff, "Cooldown", 5, "The cooldown between knockbacks.").Value;
		}
		private static void Read_TopazBrooch()
        {
			TopazBrooch.Enable = ItemConfig.Bind(Section_TopazBrooch_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			TopazBrooch.BaseFlatBarrier = ItemConfig.Bind(Section_TopazBrooch_Buff, "Base Flat Barrier", 8.0f, "Flat amount of barrier given at a single stack.").Value;
			TopazBrooch.StackFlatBarrier = ItemConfig.Bind(Section_TopazBrooch_Buff, "Stack Flat Barrier", 0.0f, "Flat amount of barrier given for each additional stack.").Value;
			TopazBrooch.BaseCentBarrier = ItemConfig.Bind(Section_TopazBrooch_Buff, "Base Percent Barrier", 0.02f, "Percent amount of barrier given at a single stack.").Value;
			TopazBrooch.StackCentBarrier = ItemConfig.Bind(Section_TopazBrooch_Buff, "Stack Percent Barrier", 0.02f, "Percent amount of barrier given for each additional stack.").Value;
			TopazBrooch.Comp_AssistManager = ItemConfig.Bind(Section_TopazBrooch_Buff, Label_AssistManager, true, Desc_AssistManager).Value;
		}
		private static void Read_RollOfPennies()
        {
			RollOfPennies_Rework.Enable = ItemConfig.Bind(Section_RollOfPennies_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			RollOfPennies_Rework.BaseGold = ItemConfig.Bind(Section_RollOfPennies_Rework, "Base Gold", 3f, "Gold amount given at a single stack.").Value;
			RollOfPennies_Rework.StackGold = ItemConfig.Bind(Section_RollOfPennies_Rework, "Stack Gold", 0f, "Gold amount given for each additional stack.").Value;
			RollOfPennies_Rework.BaseArmor = ItemConfig.Bind(Section_RollOfPennies_Rework, "Base Armor", 5f, "Armor given at a single stack.").Value;
			RollOfPennies_Rework.StackArmor = ItemConfig.Bind(Section_RollOfPennies_Rework, "Stack Armor", 0f, "Armor given for each additional stack.").Value;
			RollOfPennies_Rework.BaseDuration = ItemConfig.Bind(Section_RollOfPennies_Rework, "Base Armor Duration", 2f, "Duration given to the armor at a single stack.").Value;
			RollOfPennies_Rework.StackDuration = ItemConfig.Bind(Section_RollOfPennies_Rework, "Stack Armor Duration", 2f, "Duration given to the armor for each additional stack.").Value;
			RollOfPennies_Rework.GoldDuration = ItemConfig.Bind(Section_RollOfPennies_Rework, "Gold Armor Duration", 0.5f, "Multiplier for the gold's value when calculating the extra duration.").Value;
		}
		private static void Read_Chronobauble()
        {
			Chronobauble.Enable = ItemConfig.Bind(Section_Chronobauble_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Chronobauble.SlowDown = ItemConfig.Bind(Section_Chronobauble_Buff, "Move Speed Reduction", 0.6f, "Move Speed debuff amount.").Value;
			Chronobauble.AttackDown = ItemConfig.Bind(Section_Chronobauble_Buff, "Attack Speed Reduction", 0.3f, "Attack Speed debuff amount.").Value;
			Chronobauble.BaseDuration = ItemConfig.Bind(Section_Chronobauble_Buff, "Base Duration", 2f, "Debuff duration at a single stack.").Value;
			Chronobauble.StackDuration = ItemConfig.Bind(Section_Chronobauble_Buff, "Stack Duration", 2f, "Debuff duration for each additional.").Value;
		}
		private static void Read_DeathMark()
		{
			DeathMark.Enable = ItemConfig.Bind(Section_DeathMark_Buff, Label_EnableRework, false, Desc_EnableRework).Value;

			DeathMark.BaseDuration = ItemConfig.Bind(Section_DeathMark_Buff, "Base Duration", 6f, "Duration of the Death Mark at a single stack.").Value;
			DeathMark.StackDuration = ItemConfig.Bind(Section_DeathMark_Buff, "Stack Duration", 4f, "Duration of the Death Mark for each additional stack.").Value;
			DeathMark.DamagePerDebuff = ItemConfig.Bind(Section_DeathMark_Buff, "Damage Per Debuff", 0.1f, "Damage take per debuff.").Value;
			DeathMark.MaxDebuffs = ItemConfig.Bind(Section_DeathMark_Buff, "Max Debuffs", 5, "The max amount of debuff that can increase damage.").Value;
		}
		private static void Read_LeptonDaisy()
        {
			LeptonDaisy.Enable = ItemConfig.Bind(Section_LeptonDaisy_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LeptonDaisy.BaseHeal = ItemConfig.Bind(Section_LeptonDaisy_Buff, "Base Healing", 0.1f, "Healing at a single stack.").Value;
			LeptonDaisy.StackHeal = ItemConfig.Bind(Section_LeptonDaisy_Buff, "Stack Healing", 0.1f, "Healing for each additional stack.").Value;
			LeptonDaisy.CapHeal = ItemConfig.Bind(Section_LeptonDaisy_Buff, "Capped Healing", 2f, "Hyperbolic healing limit. (Set to 0 or less to disable.)").Value;
			LeptonDaisy.Cooldown = ItemConfig.Bind(Section_LeptonDaisy_Buff, "Cooldown", 10f, "Cooldown of the healing nova.").Value;
		}
		private static void Read_IgnitionTank()
		{
			IgnitionTank_Rework.Enable = ItemConfig.Bind(Section_IgnitionTank_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			IgnitionTank_Rework.BurnChance = ItemConfig.Bind(Section_IgnitionTank_Rework, "Burn Chance", 10f, "Chance to Burn on hit.").Value;
			IgnitionTank_Rework.BurnBaseDamage = ItemConfig.Bind(Section_IgnitionTank_Rework, "Burn Damage", 0.8f, "How much damage Burn deals per second.").Value;
			IgnitionTank_Rework.BurnDuration = ItemConfig.Bind(Section_IgnitionTank_Rework, "Burn Duration", 3f, "How long in seconds Burn lasts for.").Value;

			IgnitionTank_Rework.BlastTicks = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explode Ticks", 10, "Explodes after X instances of damage over time.").Value;
			IgnitionTank_Rework.BlastChance = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explode Chance", 0.0f, "Chance of damage over time to cause explosions. (Overrides Explode Ticks)").Value;
			IgnitionTank_Rework.BlastBaseDamage = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explosion Base Damage", 3f, "Damage the explosion deals at a single stack.").Value;
			IgnitionTank_Rework.BlastStackDamage = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explosion Stack Damage", 2f, "Extra damage the explosion deals for each additional stack.").Value;
			IgnitionTank_Rework.BlastBaseRadius = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explosion Base Radius", 12f, "Radius of the the explosion at a single stack.").Value;
			IgnitionTank_Rework.BlastStackRadius = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explosion Stack Radius", 2.4f, "Extra explosion radius for each additional stack.").Value;
			IgnitionTank_Rework.BlastInheritDamageType = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explosion Inherit Damage Type", false, "Makes the explosion inherit the damage types used from the hit that procced. (For example it would make the explosion non-lethal if it was from Acrid's poison)").Value;
			IgnitionTank_Rework.BlastRollCrit = ItemConfig.Bind(Section_IgnitionTank_Rework, "Explosion Roll Crits", false, "Allows the explosion to roll for crits, it will still inherit crits from the hit that procced regardless of this setting.").Value;
		}
		private static void Read_Infusion()
        {
			Infusion.Enable = ItemConfig.Bind(Section_Infusion_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Infusion.StackLevel = ItemConfig.Bind(Section_Infusion_Buff, "Stack Level", 2, "Number of levels gained per stack.").Value;
			Infusion.Infinite = ItemConfig.Bind(Section_Infusion_Buff, "Soft Cap", true, "Allows samples to be collected beyond the cap with diminishing returns.").Value;
			Infusion.Inherit = ItemConfig.Bind(Section_Infusion_Buff, "Inherit", true, "Should your minions with infusions inherit your count.").Value;
			Infusion.ChampionGain = ItemConfig.Bind(Section_Infusion_Buff, "Champion Value", 5, "Sample value of champion enemies. (Wandering Vagrant, Magma Worm, etc)").Value;
			Infusion.EliteGainMult = ItemConfig.Bind(Section_Infusion_Buff, "Elite Multiplier", 3, "Sample value multiplier from elite enemies.").Value;
			Infusion.BossGainMult = ItemConfig.Bind(Section_Infusion_Buff, "Boss Multiplier", 2, "Sample value multiplier from boss enemies.").Value;
			Infusion.Comp_AssistManager = ItemConfig.Bind(Section_Infusion_Buff, Label_AssistManager, true, Desc_AssistManager).Value;
		}
		private static void Read_LeechingSeed()
        {
			LeechingSeed.Enable = ItemConfig.Bind(Section_LeechingSeed_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LeechingSeed.ProcHeal = ItemConfig.Bind(Section_LeechingSeed_Buff, "Proc Healing", 1f, "Healing amount that's affected by proc coefficient.").Value;
			LeechingSeed.BaseHeal = ItemConfig.Bind(Section_LeechingSeed_Buff, "Base Healing", 1f, "Extra healing amount regardless of proc coefficient.").Value;

			LeechingSeed_Rework.Enable = ItemConfig.Bind(Section_LeechingSeed_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			LeechingSeed_Rework.BaseDoTHeal = ItemConfig.Bind(Section_LeechingSeed_Rework, "Base DoT Healing", 1f, "Healing amount given from damage over time ticks at a single stack.").Value;
			LeechingSeed_Rework.StackDoTHeal = ItemConfig.Bind(Section_LeechingSeed_Rework, "Stack DoT Healing", 1f, "Healing amount given from damage over time for each additional stack.").Value;
			LeechingSeed_Rework.ScaleToTickRate = ItemConfig.Bind(Section_LeechingSeed_Rework, "Scale To Tick Rate", true, "Healing amount scales with how often the damager over time ticks, slower tick rates give more healing.").Value;
			LeechingSeed_Rework.LeechChance = ItemConfig.Bind(Section_LeechingSeed_Rework, "Leech Chance", 20f, "Chance of applying the Leech debuff.").Value;
			LeechingSeed_Rework.LeechLifeSteal = ItemConfig.Bind(Section_LeechingSeed_Rework, "Leech Life Steal", 1f, "Percent of damage received as healing when damaging a target with Leech. (Gets scaled by the attacker's damage stat.)").Value;
			LeechingSeed_Rework.LeechBaseDamage = ItemConfig.Bind(Section_LeechingSeed_Rework, "Leech Base Damage", 2.5f, "Damage of the Leech at a single stack.").Value;
			LeechingSeed_Rework.LeechStackDamage = ItemConfig.Bind(Section_LeechingSeed_Rework, "Leech Stack Damage", 0f, "Damage of the Leech for each additional stack.").Value;
			LeechingSeed_Rework.LeechBaseDuration = ItemConfig.Bind(Section_LeechingSeed_Rework, "Leech Base Duration", 5f, "Duration of the Leech debuff.").Value;
			
		}
		private static void Read_WaxQuail()
        {
			WaxQuail.Enable = ItemConfig.Bind(Section_WaxQuail_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			WaxQuail.BaseHori = ItemConfig.Bind(Section_WaxQuail_Buff, "Base Horizontal Boost", 12f, "Horizontal force at a single stack.").Value;
			WaxQuail.StackHori = ItemConfig.Bind(Section_WaxQuail_Buff, "Stack Horizontal Boost", 8f, "Horizontal force for each additional stack.").Value;
			WaxQuail.CapHori = ItemConfig.Bind(Section_WaxQuail_Buff, "Capped Horizontal Boost", 120f, "Hyperbolic cap to horizontal force. (Set to 0 or less to disable.)").Value;
			WaxQuail.BaseVert = ItemConfig.Bind(Section_WaxQuail_Buff, "Base Vertical Boost", 0.2f, "Vertical force at a single stack.").Value;
			WaxQuail.StackVert = ItemConfig.Bind(Section_WaxQuail_Buff, "Stack Vertical", 0f, "Vertical force for each additional stack.").Value;
			WaxQuail.CapVert = ItemConfig.Bind(Section_WaxQuail_Buff, "Capped Vertical Boost", 0f, "Hyperbolic cap to vertical force. (Set to 0 or less to disable.)").Value;
			WaxQuail.BaseAirSpeed = ItemConfig.Bind(Section_WaxQuail_Buff, "Base Air Speed", 0.12f, "Airborne movement speed at a single stack.").Value;
			WaxQuail.StackAirSpeed = ItemConfig.Bind(Section_WaxQuail_Buff, "Stack Air Speed", 0.08f, "Airborne movement speed for each additional stack.").Value;
			WaxQuail.CapAirSpeed = ItemConfig.Bind(Section_WaxQuail_Buff, "Capped Air Speed", 1.2f, "Hyperbolic cap to airborne movement speed. (Set to 0 or less to disable.)").Value;
		}
		private static void Read_Stealthkit()
        {
			Stealthkit.Enable = ItemConfig.Bind(Section_Stealthkit_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Stealthkit.BaseRecharge = ItemConfig.Bind(Section_Stealthkit_Buff, "Base Cooldown", 30f, "Cooldown between uses.").Value;
			Stealthkit.StackRecharge = ItemConfig.Bind(Section_Stealthkit_Buff, "Stack Cooldown", 0.5f, "Cooldown rate for each additional stack.").Value;
			Stealthkit.BuffDuration = ItemConfig.Bind(Section_Stealthkit_Buff, "Buff Duration", 5f, "Duration of the Stealth buff.").Value;
			Stealthkit.Stealth_MoveSpeed = ItemConfig.Bind(Section_Stealthkit_Buff, "Stealth Movement Speed", 0.4f, "How much Movement Speed is given from the Stealth buff.").Value;
			Stealthkit.Stealth_ArmorPerBuff = ItemConfig.Bind(Section_Stealthkit_Buff, "Stealth Armor", 20f, "How much Armor to give per stack of the Stealth buff.").Value;
			Stealthkit.CancelDanger = ItemConfig.Bind(Section_Stealthkit_Buff, "Cancel Danger", true, "Puts you in 'Out of Danger' upon activation.").Value;
			Stealthkit.CleanseDoT = ItemConfig.Bind(Section_Stealthkit_Buff, "Clean DoTs", true, "Removes damage over time effects upon activation.").Value;
			Stealthkit.SmokeBomb = ItemConfig.Bind(Section_Stealthkit_Buff, "Smoke Bomb", true, "Causes a stunning smoke bomb effect upon activation.").Value;
		}
		private static void Read_UnstableTransmitter()
        {
			UnstableTransmitter_Rework.Enable = ItemConfig.Bind(Section_UnstableTransmitter_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			UnstableTransmitter_Rework.BaseCooldown = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Base Cooldown", 30f, "Cooldown between activations.").Value;
			UnstableTransmitter_Rework.AllyStackCooldown = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Ally Cooldown", 0.5f, "Cooldown reduction per ally owned.").Value;
			UnstableTransmitter_Rework.CapCooldown = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Cap Cooldown", 1f, "The lowest the cooldown can go.").Value;
			UnstableTransmitter_Rework.BaseDamage = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Base Damage", 3.5f, "Damage at a single stack.").Value;
			UnstableTransmitter_Rework.StackDamage = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Stack Damage", 2.8f, "Damage for each additional stack.").Value;
			UnstableTransmitter_Rework.BaseRadius = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Base Radius", 16f, "Blast radius at a single stack.").Value;
			UnstableTransmitter_Rework.StackRadius = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Stack Radius", 0f, "Blast radius for each additional stack.").Value;
			UnstableTransmitter_Rework.ProcRate = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Proc Coefficient", 1f, "The Proc Coefficient of the blast.").Value;
			UnstableTransmitter_Rework.ProcBands = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Proc Bands", true, "Should the blast proc bands?").Value;
			UnstableTransmitter_Rework.AllyOwnsDamage = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Ally Owns Damage", false, "Should the ally own the explosion instead of the user?").Value;
			UnstableTransmitter_Rework.TeleportRadius = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Teleport Radius", 40f, "The maximum radius away from the user that allies can be teleported to.").Value;
			UnstableTransmitter_Rework.TeleFragRadius = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Telefrag Radius", 60f, "Enemies within this radius of the user can be purposely telefragged by allies. (Set to 0 or less to disable this behavior.)").Value;
			UnstableTransmitter_Rework.TeleImmobile = ItemConfig.Bind(Section_UnstableTransmitter_Rework, "Teleport Immobile", true, "Allows immobile allies to be targeted for teleportation.").Value;
		}
		private static void Read_HuntersHarpoon()
        {
			HuntersHarpoon.Enable = ItemConfig.Bind(Section_HuntersHarpoon_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			HuntersHarpoon.BaseDuration = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Base Duration", 1f, "Buff duration at a single stack.").Value;
			HuntersHarpoon.StackDuration = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Stack Duration", 1f, "Extra buff duration for each additional stack.").Value;
			HuntersHarpoon.MovementSpeed = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Movement Speed Bonus", 1.25f, "Movement speed given from the buff.").Value;
			HuntersHarpoon.ExtendDuration = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Extend Duration", true, "Adds duration to the buff with each kill instead of refreshing it.").Value;
			HuntersHarpoon.BaseCooldownReduction = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Reduction", 1f, "Cooldown reduction on kill.").Value;
			HuntersHarpoon.CoolPrimary = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Primary", false, "Cooldown reduction affects Primary skills?").Value;
			HuntersHarpoon.CoolSecondary = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Secondary", true, "Cooldown reduction affects Secondary skills?").Value;
			HuntersHarpoon.CoolUtility = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Utility", false, "Cooldown reduction affects Utility skills?").Value;
			HuntersHarpoon.CoolSpecial = ItemConfig.Bind(Section_HuntersHarpoon_Buff, "Cooldown Special", false, "Cooldown reduction affects Special skills?").Value;
			HuntersHarpoon.Comp_AssistManager = ItemConfig.Bind(Section_HuntersHarpoon_Buff, Label_AssistManager, true, Desc_AssistManager).Value;
		}
		private static void Read_SquidPolyp()
        {
			SquidPolyp.Enable = ItemConfig.Bind(Section_SquidPolyp_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			SquidPolyp.ApplyTar = ItemConfig.Bind(Section_SquidPolyp_Buff, "Apply Tar", true, "Makes Squid Turrets apply the Tar debuff with their attack.").Value;
			SquidPolyp.BaseDuration = ItemConfig.Bind(Section_SquidPolyp_Buff, "Base Duration", 25, "Squid Turret duration at a single stack.").Value;
			SquidPolyp.StackDuration = ItemConfig.Bind(Section_SquidPolyp_Buff, "Stack Duration", 5, "Squid Turret duration for each additional stack.").Value;
			SquidPolyp.StackHealth = ItemConfig.Bind(Section_SquidPolyp_Buff, "Stack Health", 2, "Extra Squid Turret health for each additional stack. (1 = +10%)").Value;
			SquidPolyp.MaxTurrets = ItemConfig.Bind(Section_SquidPolyp_Buff, "Max Turrets", 8, "How many Squid Turrets each player can have, newer turrets will replace old ones. (Set to 0 for vanilla behavior.)").Value;
		}
		private static void Read_WarHorn()
        {
			WarHorn.Enable = ItemConfig.Bind(Section_WarHorn_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			WarHorn.BaseDuration = ItemConfig.Bind(Section_WarHorn_Buff, "Base Duration", 6f, "Duration at a single stack.").Value;
			WarHorn.StackDuration = ItemConfig.Bind(Section_WarHorn_Buff, "Stack Duration", 3f, "Duration for each additional stack.").Value;
			WarHorn.BaseAttack = ItemConfig.Bind(Section_WarHorn_Buff, "Base Attack", 0.6f, "Attack Speed at a single stack.").Value;
			WarHorn.StackAttack = ItemConfig.Bind(Section_WarHorn_Buff, "Stack Attack", 0.15f, "Attack Speed for each additional stack.").Value;
		}
		private static void Read_Aegis()
		{
			Aegis.Enable = ItemConfig.Bind(Section_Aegis_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Aegis.AllowRegen = ItemConfig.Bind(Section_Aegis_Buff, "Count Regen", true, "Allows excess regen to be converted into barrier.").Value;
			Aegis.BaseOverheal = ItemConfig.Bind(Section_Aegis_Buff, "Base Overheal", 1f, "Conversion rate of overheal to barrier at single stack.").Value;
			Aegis.StackOverheal = ItemConfig.Bind(Section_Aegis_Buff, "Stack Overheal", 0f, "Conversion rate of overheal to barrier for each additional stack.").Value;
			Aegis.BaseMaxBarrier = ItemConfig.Bind(Section_Aegis_Buff, "Base Max Barrier", 1f, "Increases maximum barrier by this much at single stack.").Value;
			Aegis.StackMaxBarrier = ItemConfig.Bind(Section_Aegis_Buff, "Stack Max Barrier", 1f, "Increases maximum barrier by this much for each additional stack.").Value;
		}
		private static void Read_BensRaincoat()
        {
			BensRaincoat.Enable = ItemConfig.Bind(Section_BensRaincoat_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			BensRaincoat.BaseBlock = ItemConfig.Bind(Section_BensRaincoat_Buff, "Base Block", 2, "Debuff blocks to give at a single stack.").Value;
			BensRaincoat.StackBlock = ItemConfig.Bind(Section_BensRaincoat_Buff, "Stack Block", 1, "Debuff blocks to give for each additional stack.").Value;
			BensRaincoat.Cooldown = ItemConfig.Bind(Section_BensRaincoat_Buff, "Cooldown", 7f, "Time in seconds it takes to restock debuff blocks. (Anything less than 0 will skip this change.)").Value;
			BensRaincoat.GraceTime = ItemConfig.Bind(Section_BensRaincoat_Buff, "Debuff Grace Time", 0.25f, "Time in seconds after consuming a block that further debuffs are negated for free.").Value;
		}
		private static void Read_HappiestMask_Rework()
        {
			HappiestMask_Rework.Enable = ItemConfig.Bind(Section_HappiestMask_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			HappiestMask_Rework.BaseDamage = ItemConfig.Bind(Section_HappiestMask_Rework, "Base Damage", 2f, "Damage increase for ghosts at a single stack.").Value;
			HappiestMask_Rework.StackDamage = ItemConfig.Bind(Section_HappiestMask_Rework, "Stack Damage", 2f, "Damage increase for ghosts for each additional stack.").Value;
			HappiestMask_Rework.BaseMoveSpeed = ItemConfig.Bind(Section_HappiestMask_Rework, "Movement Speed", 0.45f, "Movement speed increase for ghosts.").Value;
			HappiestMask_Rework.BaseDuration = ItemConfig.Bind(Section_HappiestMask_Rework, "Duration", 30, "How long in seconds the ghosts lasts before dying.").Value;
			HappiestMask_Rework.BaseCooldown = ItemConfig.Bind(Section_HappiestMask_Rework, "Cooldown", 3, "How long in seconds until a new ghost is summoned.").Value;
			HappiestMask_Rework.OnKillOnDeath = ItemConfig.Bind(Section_HappiestMask_Rework, "On Kill On Death", true, "Credits the ghost's death as your kill.").Value;
			HappiestMask_Rework.PassKillCredit = ItemConfig.Bind(Section_HappiestMask_Rework, "Kill Credit Owner", true, "Credits all kills the ghost scores as yours.").Value;
		}
		private static void Read_LaserScope()
        {
			LaserScope.Enable = ItemConfig.Bind(Section_LaserScope_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LaserScope.BaseCrit = ItemConfig.Bind(Section_LaserScope_Buff, "Crit Chance", 5f, "Crit chance at a single stack.").Value;
		}
		private static void Read_SymbioticScorpion()
        {
			SymbioticScorpion_Rework.Enable = ItemConfig.Bind(Section_SymbioticScorpion_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			SymbioticScorpion_Rework.Slayer_BaseDamage = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Slayer DoT Base Damage", 2f, "Slayer DoT damage increase at a single stack.").Value;
			SymbioticScorpion_Rework.Slayer_StackDamage = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Slayer DoT Stack Damage", 0f, "Slayer DoT damage increase for each additional stack.").Value;
			SymbioticScorpion_Rework.SlayerDot_AffectTotalDamage = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Slayer DoT Affects Total Damage", false, "Makes the Slayer DoT affect the total damage for the purpose of proc items. (False = Vanilla)").Value;
			SymbioticScorpion_Rework.Radius = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Venom Radius", 13f, "The targetting radius for the Venom attack. (Set to 0 to disable the effect entirely.)").Value;
			SymbioticScorpion_Rework.Cooldown = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Venom Cooldown", 5f, "Cooldown between Venom attacks.").Value;
			SymbioticScorpion_Rework.VenomBaseDamage = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Venom Base Damage", 6f, "Damage of the Venom at a single stack.").Value;
			SymbioticScorpion_Rework.VenomStackDamage = ItemConfig.Bind(Section_SymbioticScorpion_Rework, "Venom Stack Damage", 6f, "Damage of the Venom for each additional stack.").Value;
		}
		private static void Read_Planula()
        {
			Planula.Enable = ItemConfig.Bind(Section_Planula_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			Planula.BaseFlatHeal = ItemConfig.Bind(Section_Planula_Buff, "Base Flat Healing", 10f, "Flat healing at a single stack.").Value;
			Planula.StackFlatHeal = ItemConfig.Bind(Section_Planula_Buff, "Stack Flat Healing", 10f, "Flat healing for each additional stack.").Value;
			Planula.BaseMaxHeal = ItemConfig.Bind(Section_Planula_Buff, "Base Max Healing", 0.02f, "Percent healing at a single stack.").Value;
			Planula.StackMaxHeal = ItemConfig.Bind(Section_Planula_Buff, "Stack Max Healing", 0.02f, "Percent healing for each additional stack.").Value;

			Planula_Rework.Enable = ItemConfig.Bind(Section_Planula_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			Planula_Rework.BaseDamage = ItemConfig.Bind(Section_Planula_Rework, "Base Damage", 0.8f, "Damage per second the burn deals at a single stack.").Value;
			Planula_Rework.StackDamage = ItemConfig.Bind(Section_Planula_Rework, "Stack Damage", 0.6f, "Damage per second the burn deals for each additional stack.").Value;
			Planula_Rework.Duration = ItemConfig.Bind(Section_Planula_Rework, "Burn Duration", 5f, "Duration of the burn.").Value;
			Planula_Rework.Radius = ItemConfig.Bind(Section_Planula_Rework, "Burn Radius", 15f, "Radius for enemies to be within to start burning.").Value;
		}
		private static void Read_TitanicKnurl()
        {
			TitanicKnurl.Enable = ItemConfig.Bind(Section_TitanicKnurl_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			TitanicKnurl.BaseHP = ItemConfig.Bind(Section_TitanicKnurl_Buff, "Base Health", 30f, "Health each stack gives.").Value;
			TitanicKnurl.LevelHP = ItemConfig.Bind(Section_TitanicKnurl_Buff, "Level Health", 9f, "Health each stack gives per level.").Value;
			TitanicKnurl.BaseRegen = ItemConfig.Bind(Section_TitanicKnurl_Buff, "Base Regen", 1.6f, "Health Regen each stack gives.").Value;
			TitanicKnurl.LevelRegen = ItemConfig.Bind(Section_TitanicKnurl_Buff, "Level Regen", 0.32f, "Health Regen each stack gives per level.").Value;
			
			TitanicKnurl_Rework.Enable = ItemConfig.Bind(Section_TitanicKnurl_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			TitanicKnurl_Rework.BaseDamage = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Base Damage", 8f, "Base damage at a single stack.").Value;
			TitanicKnurl_Rework.StackDamage = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Stack Damage", 6f, "Base damage for each additional stack.").Value;
			TitanicKnurl_Rework.BaseCooldown = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Base Cooldown", 6f, "Cooldown at a single stack.").Value;
			TitanicKnurl_Rework.StackCooldown = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Stack Cooldown", 0.25f, "Cooldown rate for each additional stack.").Value;
			TitanicKnurl_Rework.ProcRate = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Proc Coefficient", 1.0f, "Proc coefficient of the stone fist.").Value;
			TitanicKnurl_Rework.ProcBands = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Proc Bands", true, "Should the stone fist proc bands?").Value;
			TitanicKnurl_Rework.TargetRadius = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Target Radius", 60f, "Targeting radius in metres.").Value;
			TitanicKnurl_Rework.TargetMode = ItemConfig.Bind(Section_TitanicKnurl_Rework, "Target Mode", 0, "Decides how the target is selected. (0 = Weak, 1 = Closest)").Value;
		}
		private static void Read_DefenseNucleus()
        {
			DefenseNucleus.Enable = ItemConfig.Bind(Section_DefenseNucleus_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			DefenseNucleus.BaseHealth = ItemConfig.Bind(Section_DefenseNucleus_Buff, "Base Health", 10, "Extra health the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackHealth = ItemConfig.Bind(Section_DefenseNucleus_Buff, "Stack Health", 10, "Extra health the construct gets for each additional stack.").Value;
			DefenseNucleus.BaseAttack = ItemConfig.Bind(Section_DefenseNucleus_Buff, "Base Attack Speed", 3, "Extra attack speed the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackAttack = ItemConfig.Bind(Section_DefenseNucleus_Buff, "Stack Attack Speed", 0, "Extra attack speed the construct gets for each additional stack.").Value;
			DefenseNucleus.BaseDamage = ItemConfig.Bind(Section_DefenseNucleus_Buff, "Base Damage", 0, "Extra damage the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus.StackDamage = ItemConfig.Bind(Section_DefenseNucleus_Buff, "Stack Damage", 5, "Extra damage the construct gets for each additional stack.").Value;
			DefenseNucleus.Comp_AssistManager = ItemConfig.Bind(Section_DefenseNucleus_Buff, Label_AssistManager, false, Desc_AssistManager).Value;

			DefenseNucleus_Rework.Enable = ItemConfig.Bind(Section_DefenseNucleus_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			DefenseNucleus_Rework.SummonCount = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Summon Count", 3, "How many constructs to summon on activation. (Cannot go above 8 because I said so.)").Value;
			DefenseNucleus_Rework.BaseHealth = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Base Health", 10, "Extra health the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackHealth = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Stack Health", 10, "Extra health the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.BaseAttack = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Base Attack Speed", 3, "Extra attack speed the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackAttack = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Stack Attack Speed", 0, "Extra attack speed the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.BaseDamage = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Base Damage", 0, "Extra damage the construct gets at a single stack. (1 = +10%)").Value;
			DefenseNucleus_Rework.StackDamage = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Stack Damage", 5, "Extra damage the construct gets for each additional stack.").Value;
			DefenseNucleus_Rework.ShieldBaseDuration = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Shield Base Duration", 5f, "Duration of the projectile shield at a single stack.").Value;
			DefenseNucleus_Rework.ShieldStackDuration = ItemConfig.Bind(Section_DefenseNucleus_Rework, "Shield Stack Duration", 2f, "Duration of the projectile shield for each additional stack.").Value;

			DefenseNucleus_Shared.TweakAI = ItemConfig.Bind(Section_DefenseNucleus_Shared, "Better AI", false, "Gives 360 Degree vision and prevents retaliation against allies.").Value;
			DefenseNucleus_Shared.ForceMechanical = ItemConfig.Bind(Section_DefenseNucleus_Shared, "Is Mechanical", false, "Gives it the Mechanical flag, allowing it to get Spare Drone Parts and Captain's Microbots.").Value;
			DefenseNucleus_Shared.ExtraDisplays = ItemConfig.Bind(Section_DefenseNucleus_Shared, "Modded Displays", false, "Adds Spare Drone Parts item displays to the Alpha Construct.").Value;
		}
		private static void Read_LigmaLenses()
        {
			LigmaLenses.Enable = ItemConfig.Bind(Section_LigmaLenses_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			LigmaLenses.BaseChance = ItemConfig.Bind(Section_LigmaLenses_Buff, "Base Chance", 0.5f, "Detain chance at a single stack.").Value;
			LigmaLenses.StackChance = ItemConfig.Bind(Section_LigmaLenses_Buff, "Stack Chance", 0.5f, "Detain chance for each additional stack.").Value;
			LigmaLenses.BaseDamage = ItemConfig.Bind(Section_LigmaLenses_Buff, "Base Damage", 50.0f, "Base damage at a single stack.").Value;
			LigmaLenses.StackDamage = ItemConfig.Bind(Section_LigmaLenses_Buff, "Stack Damage", 0.0f, "Base damage for each additional stack.").Value;
			LigmaLenses.UseTotalDamage = ItemConfig.Bind(Section_LigmaLenses_Buff, "Deal Total", false, "Deal Total Damage of the attack instead of the attacker's damage stat?").Value;
		}
		private static void Read_VoidsentFlame()
        {
			VoidsentFlame.Enable = ItemConfig.Bind(Section_VoidsentFlame_Buff, Label_EnableBuff, false, Desc_EnableBuff).Value;
			VoidsentFlame.BaseRadius = ItemConfig.Bind(Section_VoidsentFlame_Buff, "Base Radius", 12f, "Blast radius at a single stack.").Value;
			VoidsentFlame.StackRadius = ItemConfig.Bind(Section_VoidsentFlame_Buff, "Stack Radius", 2.4f, "Extra blast radius for each additional stack.").Value;
			VoidsentFlame.BaseDamage = ItemConfig.Bind(Section_VoidsentFlame_Buff, "Base Damage", 2.6f, "Blast damage at a single stack.").Value;
			VoidsentFlame.StackDamage = ItemConfig.Bind(Section_VoidsentFlame_Buff, "Stack Damage", 1.56f, "Blast damage for each additional stack.").Value;
			VoidsentFlame.ProcRate = ItemConfig.Bind(Section_VoidsentFlame_Buff, "Proc Coefficient", 1f, "Blast proc coefficient.").Value;
		}
		private static void Read_NewlyHatchedZoea()
		{
			NewlyHatchedZoea_Rework.Enable = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, Label_EnableRework, false, Desc_EnableRework).Value;
			NewlyHatchedZoea_Rework.BaseStock = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Base Stock", 12, "How many missiles to store at a single stack.").Value;
			NewlyHatchedZoea_Rework.StackStock = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Stack Stock", 4, "Extra missiles for each additional stack.").Value;
			NewlyHatchedZoea_Rework.BaseDamage = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Base Damage", 3f, "Missile damage at a single stack.").Value;
			NewlyHatchedZoea_Rework.StackDamage = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Stack Damage", 0.75f, "Missile damage for each additional stack.").Value;
			NewlyHatchedZoea_Rework.ProcRate = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Proc Coefficient", 0.2f, "Missile proc coefficient.").Value;
			NewlyHatchedZoea_Rework.RestockTime = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Restock Time", 30, "How long it takes in seconds to fully restock.").Value;
			NewlyHatchedZoea_Rework.CanCorrupt = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Allow Corruption", true, "Set to false to disable the item corruption effect.").Value;
			NewlyHatchedZoea_Rework.CorruptList = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Corruption List", "", "List of items that this item will corrupt. (Leave blank for vanilla values.)").Value;
			NewlyHatchedZoea_Rework.CorruptText = ItemConfig.Bind(Section_NewlyHatchedZoea_Rework, "Corruption Text", "<style=cIsTierBoss>yellow items</style>", "The item(s) for the \"Corrupts all X\" text.").Value;
		}
		private static void Read_ArtifactSpite()
        {
			Spite.Enable = ArtifactConfig.Bind(Section_Artifact_Spite, Label_EnableBuff, false, "Enables changes to this Artifact").Value;
			Spite.BaseDamage = ArtifactConfig.Bind(Section_Artifact_Spite, "Base Damage", 12f, "Base damage of Spite bombs.").Value;
			Spite.LevelDamage = ArtifactConfig.Bind(Section_Artifact_Spite, "Stack Damage", 2.4f, "Extra damage Spite bombs gain per victim's level.").Value;
		}
	}
}
