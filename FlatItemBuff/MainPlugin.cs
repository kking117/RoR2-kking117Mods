using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace FlatItemBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI",
		"DotAPI",
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.FlatItemBuff";
		public const string MODNAME = "FlatItemBuff";
		public const string MODTOKEN = "KKING117_FLATITEMBUFF_";
		public const string MODVERSION = "1.14.0";

		//ToDo:
		//Make mentions of "multiple and single stacks" consistent in config descriptions.

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<float> General_OutOfCombatTime;
		public static ConfigEntry<float> General_OutOfDangerTime;

		public static ConfigEntry<bool> Steak_Enable;
		public static ConfigEntry<float> Steak_BaseHP;
		public static ConfigEntry<float> Steak_LevelHP;
		public static ConfigEntry<float> Steak_BaseBuffDur;
		public static ConfigEntry<float> Steak_StackBuffDur;

		public static ConfigEntry<bool> Brooch_Enable;
		public static ConfigEntry<float> Brooch_BaseFlatBarrier;
		public static ConfigEntry<float> Brooch_StackFlatBarrier;
		public static ConfigEntry<float> Brooch_BaseCentBarrier;
		public static ConfigEntry<float> Brooch_StackCentBarrier;

		public static ConfigEntry<bool> Harpoon_Enable;
		public static ConfigEntry<float> Harpoon_BaseDuration;
		public static ConfigEntry<float> Harpoon_StackDuration;
		public static ConfigEntry<float> Harpoon_MoveSpeed;
		public static ConfigEntry<float> Harpoon_CooldownRate;

		public static ConfigEntry<bool> Infusion_Enable;
		public static ConfigEntry<int> Infusion_Stacks;
		public static ConfigEntry<int> Infusion_Level;
		public static ConfigEntry<bool> Infusion_OwnerGains;
		public static ConfigEntry<bool> Infusion_InheritOwner;
		public static ConfigEntry<bool> Infusion_Tracker;

		public static ConfigEntry<int> Infusion_Fake_Bonus;
		public static ConfigEntry<int> Infusion_Kill_Bonus;
		public static ConfigEntry<int> Infusion_Champ_Bonus;
		public static ConfigEntry<int> Infusion_Elite_Bonus;
		public static ConfigEntry<int> Infusion_Boss_Bonus;

		public static ConfigEntry<bool> LeechingSeed_Enable;
		public static ConfigEntry<float> LeechingSeed_ProcHeal;
		public static ConfigEntry<float> LeechingSeed_NoProcHeal;

		public static ConfigEntry<bool> LeechingSeedRework_Enable;
		public static ConfigEntry<float> LeechingSeedRework_DoTFlatHeal;
		public static ConfigEntry<float> LeechingSeedRework_DoTChance;
		public static ConfigEntry<float> LeechingSeedRework_DoTLifeSteal;
		public static ConfigEntry<float> LeechingSeedRework_DoTMinLifeSteal;
		public static ConfigEntry<float> LeechingSeedRework_DoTBaseDamage;
		public static ConfigEntry<float> LeechingSeedRework_DoTBaseDuration;
		public static ConfigEntry<float> LeechingSeedRework_DoTStackDuration;

		public static ConfigEntry<bool> LeptonDaisy_Enable;
		public static ConfigEntry<float> LeptonDaisy_BaseHeal;
		public static ConfigEntry<float> LeptonDaisy_StackHeal;
		public static ConfigEntry<float> LeptonDaisy_HealTime;
		public static ConfigEntry<float> LeptonDaisy_CapHeal;

		public static ConfigEntry<bool> LigmaLenses_Enable;
		public static ConfigEntry<float> LigmaLenses_BaseDamage;
		public static ConfigEntry<float> LigmaLenses_StackDamage;
		public static ConfigEntry<float> LigmaLenses_BaseRadius;
		public static ConfigEntry<float> LigmaLenses_StackRadius;
		public static ConfigEntry<int> LigmaLenses_Cooldown;
		public static ConfigEntry<float> LigmaLenses_ProcRate;

		public static ConfigEntry<bool> StealthKit_Enable;
		public static ConfigEntry<float> StealthKit_BaseRecharge;
		public static ConfigEntry<float> StealthKit_StackRecharge;
		public static ConfigEntry<float> StealthKit_BuffDuration;
		public static ConfigEntry<bool> StealthKit_CancelCombat;
		public static ConfigEntry<bool> StealthKit_CancelDanger;
		public static ConfigEntry<float> StealthKit_CancelDuration;

		public static ConfigEntry<bool> Squid_Enable;
		public static ConfigEntry<bool> Squid_ClayHit;
		public static ConfigEntry<float> Squid_InactiveDecay;
		public static ConfigEntry<int> Squid_StackLife;
		public static ConfigEntry<float> Squid_Armor;

		public static ConfigEntry<bool> WarHorn_Enable;
		public static ConfigEntry<float> WarHorn_BaseDuration;
		public static ConfigEntry<float> WarHorn_StackDuration;
		public static ConfigEntry<float> WarHorn_BaseSpeed;
		public static ConfigEntry<float> WarHorn_StackSpeed;

		public static ConfigEntry<bool> WaxQuail_Enable;
		public static ConfigEntry<float> WaxQuail_BaseHori;
		public static ConfigEntry<float> WaxQuail_StackHori;
		public static ConfigEntry<float> WaxQuail_CapHori;
		public static ConfigEntry<float> WaxQuail_BaseVert;
		public static ConfigEntry<float> WaxQuail_StackVert;
		public static ConfigEntry<float> WaxQuail_CapVert;
		public static ConfigEntry<float> WaxQuail_BaseAirSpeed;
		public static ConfigEntry<float> WaxQuail_StackAirSpeed;
		public static ConfigEntry<float> WaxQuail_CapAirSpeed;

		public static ConfigEntry<bool> Aegis_Enable;
		public static ConfigEntry<bool> Aegis_Regen;
		public static ConfigEntry<float> Aegis_Armor;

		public static ConfigEntry<bool> LaserScope_Enable;
		public static ConfigEntry<float> LaserScope_Crit;

		public static ConfigEntry<bool> BensRaincoat_Enable;
		public static ConfigEntry<int> BensRaincoat_BaseBlock;
		public static ConfigEntry<int> BensRaincoat_StackBlock;
		public static ConfigEntry<float> BensRaincoat_Cooldown;
		public static ConfigEntry<bool> BensRaincoat_FixCooldown;

		public static ConfigEntry<bool> VoidsentFlame_Enable;
		public static ConfigEntry<float> VoidsentFlame_BaseRadius;
		public static ConfigEntry<float> VoidsentFlame_StackRadius;
		public static ConfigEntry<float> VoidsentFlame_BaseDamage;
		public static ConfigEntry<float> VoidsentFlame_StackDamage;
		public static ConfigEntry<float> VoidsentFlame_ProcRate;

		public static ConfigEntry<bool> Knurl_Enable;
		public static ConfigEntry<float> Knurl_BaseHP;
		public static ConfigEntry<float> Knurl_LevelHP;
		public static ConfigEntry<float> Knurl_BaseRegen;
		public static ConfigEntry<float> Knurl_LevelRegen;

		public static ConfigEntry<bool> KnurlRework_Enable;
		public static ConfigEntry<float> KnurlRework_BaseDamage;
		public static ConfigEntry<float> KnurlRework_StackDamage;
		public static ConfigEntry<float> KnurlRework_BaseSpeed;
		public static ConfigEntry<float> KnurlRework_StackSpeed;
		public static ConfigEntry<float> KnurlRework_ProcRate;
		public static ConfigEntry<float> KnurlRework_AttackRange;
		public static ConfigEntry<int> KnurlRework_TargetType;

		public static ConfigEntry<bool> Nucleus_Enable;
		public static ConfigEntry<bool> Nucleus_Infinite;
		public static ConfigEntry<int> Nucleus_BaseHealth;
		public static ConfigEntry<int> Nucleus_StackHealth;
		public static ConfigEntry<int> Nucleus_BaseAttack;
		public static ConfigEntry<int> Nucleus_StackAttack;
		public static ConfigEntry<int> Nucleus_BaseDamage;
		public static ConfigEntry<int> Nucleus_StackDamage;
		public static ConfigEntry<float> Nucleus_Cooldown;

		public static ConfigEntry<bool> NucleusRework_Enable;
		public static ConfigEntry<int> NucleusRework_SummonCount;
		public static ConfigEntry<int> NucleusRework_BaseHealth;
		public static ConfigEntry<int> NucleusRework_StackHealth;
		public static ConfigEntry<int> NucleusRework_BaseDamage;
		public static ConfigEntry<int> NucleusRework_StackDamage;
		public static ConfigEntry<int> NucleusRework_BaseAttack;
		public static ConfigEntry<int> NucleusRework_StackAttack;
		public static ConfigEntry<float> NucleusRework_ShieldBaseDuration;
		public static ConfigEntry<float> NucleusRework_ShieldStackDuration;

		public static ConfigEntry<bool> NucleusShared_TweakAI;
		public static ConfigEntry<float> NucleusShared_BlastRadius;
		public static ConfigEntry<float> NucleusShared_BlastDamage;
		public static ConfigEntry<bool> NucleusShared_Mechanical;
		public static ConfigEntry<bool> NucleusShared_ExtraDisplays;
		private void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
			//Common/White
			if (Steak_Enable.Value)
			{
				ItemChanges.BisonSteak.EnableChanges();
			}
			if (Brooch_Enable.Value)
			{
				ItemChanges.TopazBrooch.EnableChanges();
			}
			//Uncommon/Green
			if (Infusion_Enable.Value)
			{
				ItemChanges.Infusion.EnableChanges();
			}
			if (LeechingSeedRework_Enable.Value)
			{
				ItemChanges.LeechingSeed_Rework.EnableChanges();
			}
			else if (LeechingSeed_Enable.Value)
			{
				ItemChanges.LeechingSeed.EnableChanges();
			}
			if (LeptonDaisy_Enable.Value)
			{
				ItemChanges.LeptonDaisy.EnableChanges();
			}
			if (Harpoon_Enable.Value)
			{
				ItemChanges.HuntersHarpoon.EnableChanges();
			}
			if (StealthKit_Enable.Value)
			{
				ItemChanges.Stealthkit.EnableChanges();
			}
			if (Squid_Enable.Value)
			{
				ItemChanges.SquidPolyp.EnableChanges();
			}
			if (WarHorn_Enable.Value)
			{
				ItemChanges.WarHorn.EnableChanges();
			}
			if (WaxQuail_Enable.Value)
            {
				ItemChanges.WaxQuail.EnableChanges();
			}
			//Legendary/Red
			if (Aegis_Enable.Value)
			{
				ItemChanges.Aegis.EnableChanges();
			}
			if (LaserScope_Enable.Value)
			{
				ItemChanges.LaserScope.EnableChanges();
			}
			if (BensRaincoat_Enable.Value)
			{
				ItemChanges.BensRaincoat.EnableChanges();
			}
			//Boss/Yellow
			if (KnurlRework_Enable.Value)
			{
				ItemChanges.TitanicKnurl_Rework.EnableChanges();
			}
			else if (Knurl_Enable.Value)
			{
				ItemChanges.TitanicKnurl.EnableChanges();
			}
			if (NucleusRework_Enable.Value)
			{
				ItemChanges.DefenseNucleus_Rework.EnableChanges();
			}
			else if (Nucleus_Enable.Value)
			{
				ItemChanges.DefenseNucleus.EnableChanges();
			}
			//Void
			if (LigmaLenses_Enable.Value)
			{
				ItemChanges.LigmaLenses.EnableChanges();
			}
			if (VoidsentFlame_Enable.Value)
			{
				ItemChanges.VoidsentFlame.EnableChanges();
			}
			ModLogger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		private void PostLoad()
        {
			if (NucleusRework_Enable.Value || Nucleus_Enable.Value)
			{
				ItemChanges.DefenseNucleus_Shared.ExtraChanges();
			}
        }
		//Shamelessly taken from FW_Artifacts
		internal static Sprite LoadAsSprite(byte[] resourceBytes, int size)
		{
			if (resourceBytes == null)
			{
				throw new ArgumentNullException("resourceBytes");
			}
			Texture2D texture2D = new Texture2D(size, size, TextureFormat.RGBA32, false);
			texture2D.LoadImage(resourceBytes, false);
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)size, (float)size), new Vector2(0f, 0f));
		}
		public void ReadConfig()
		{
			General_OutOfCombatTime = Config.Bind<float>(new ConfigDefinition("!General!", "Out of Combat Time"), 5f, new ConfigDescription("How long it takes to be considered Out of Combat. (5 = Vanilla) (Is used for internal reference, does not affect the game.)", null, Array.Empty<object>()));
			General_OutOfDangerTime = Config.Bind<float>(new ConfigDefinition("!General!", "Out of Danger Time"), 7f, new ConfigDescription("How long it takes to be considered Out of Danger. (7 = Vanilla) (Is used for internal reference, does not affect the game.)", null, Array.Empty<object>()));

			Steak_Enable = Config.Bind<bool>(new ConfigDefinition("Bison Steak", "Enable Changes"), true, new ConfigDescription("Enables changes to Bison Steak.", null, Array.Empty<object>()));
			Steak_BaseHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base HP"), 20.0f, new ConfigDescription("The amount of HP each stack increases. (Set to 0 or less to disable health.)", null, Array.Empty<object>()));
			Steak_LevelHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Level HP"), 2.0f, new ConfigDescription("How much extra HP to give per level.", null, Array.Empty<object>()));
			Steak_BaseBuffDur = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base Buff Duration"), 3f, new ConfigDescription("Gives the Fresh Meat regen effect for X seconds on kill. (Set to 0 or less to disable regen.)", null, Array.Empty<object>()));
			Steak_StackBuffDur = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Stack Buff Duration"), 3f, new ConfigDescription("Gives the Fresh Meat regen effect for X seconds on kill per stack.", null, Array.Empty<object>()));

			Brooch_Enable = Config.Bind<bool>(new ConfigDefinition("Topaz Brooch", "Enable Changes"), true, new ConfigDescription("Enables changes to Topaz Brooch.", null, Array.Empty<object>()));
			Brooch_BaseFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier each stack after the first gives.", null, Array.Empty<object>()));
			Brooch_BaseCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier each stack after the first gives.", null, Array.Empty<object>()));

			Harpoon_Enable = Config.Bind<bool>(new ConfigDefinition("Hunters Harpoon", "Enable Changes"), true, new ConfigDescription("Enables changes to Hunters Harpoon.", null, Array.Empty<object>()));
			Harpoon_BaseDuration = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Base Duration"), 1.5f, new ConfigDescription("Buff duration at a single stack.", null, Array.Empty<object>()));
			Harpoon_StackDuration = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Stack Duration"), 0.75f, new ConfigDescription("Extra buff duration from additional stacks.", null, Array.Empty<object>()));
			Harpoon_MoveSpeed = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Move Speed Bonus"), 0.25f, new ConfigDescription("How much movement speed each stack of the buff gives. (Set to 0 to disable)", null, Array.Empty<object>()));
			Harpoon_CooldownRate = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Cooldown Rate Bonus"), 0.25f, new ConfigDescription("How much primary and secondary cooldown rate each stack of the buff gives. (Set to 0 to disable)", null, Array.Empty<object>()));

			Infusion_Enable = Config.Bind<bool>(new ConfigDefinition("Infusion", "Enable Changes"), true, new ConfigDescription("Enables changes to Infusion.", null, Array.Empty<object>()));
			Infusion_Stacks = Config.Bind<int>(new ConfigDefinition("Infusion", "Max Stacks"), 100, new ConfigDescription("How many stacks an infusion has (100 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Level = Config.Bind<int>(new ConfigDefinition("Infusion", "Level Per Stacks"), 100, new ConfigDescription("How many stacks are needed to gain a level up.", null, Array.Empty<object>()));
			Infusion_OwnerGains = Config.Bind<bool>(new ConfigDefinition("Infusion", "Give To Owner"), true, new ConfigDescription("Should minions with infusions send their uncollected samples to their owner instead?", null, Array.Empty<object>()));
			Infusion_InheritOwner = Config.Bind<bool>(new ConfigDefinition("Infusion", "Inherit From Owner"), true, new ConfigDescription("Should minions with infusions inherit their owner's collected samples.", null, Array.Empty<object>()));
			Infusion_Tracker = Config.Bind<bool>(new ConfigDefinition("Infusion", "Tracker"), true, new ConfigDescription("Enables a cosmetic buff icon to help keep track of your infusion stacks.", null, Array.Empty<object>()));

			Infusion_Fake_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Fake Stack"), 1, new ConfigDescription("How many samples certain non-ai and non-player enemies give.", null, Array.Empty<object>()));
			Infusion_Kill_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Kill Stack"), 1, new ConfigDescription("How many samples normal enemies give.", null, Array.Empty<object>()));
			Infusion_Champ_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Champion Stack"), 5, new ConfigDescription("How many samples champion enemies give.", null, Array.Empty<object>()));
			Infusion_Elite_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Elite Bonus"), 2, new ConfigDescription("Sample multiplier for elites.", null, Array.Empty<object>()));
			Infusion_Boss_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Boss Bonus"), 2, new ConfigDescription("Sample multiplier for bosses.", null, Array.Empty<object>()));

			LeechingSeed_Enable = Config.Bind<bool>(new ConfigDefinition("Leeching Seed", "Enable Changes"), true, new ConfigDescription("Enables changes for Leeching Seed.", null, Array.Empty<object>()));
			LeechingSeed_ProcHeal = Config.Bind<float>(new ConfigDefinition("Leeching Seed", "Normal Heal"), 0.5f, new ConfigDescription("How much healing to give on hits with a proc coefficient. (Set to 0 to disable this effect entirely.)", null, Array.Empty<object>()));
			LeechingSeed_NoProcHeal = Config.Bind<float>(new ConfigDefinition("Leeching Seed", "Fixed Heal"), 0.5f, new ConfigDescription("How much extra healing to give regardless of proc coefficient. (Set to 0 to disable this effect entirely.)", null, Array.Empty<object>()));

			LeechingSeedRework_Enable = Config.Bind<bool>(new ConfigDefinition("Leeching Seed Rework", "Enable Changes"), false, new ConfigDescription("Enables the rework for Leeching Seed. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			LeechingSeedRework_DoTFlatHeal = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "DoT Heal"), 2f, new ConfigDescription("How much healing DoTs give per hit per stack. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			LeechingSeedRework_DoTChance = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Chance"), 25f, new ConfigDescription("Proc chance of the Leeching debuff. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			LeechingSeedRework_DoTLifeSteal = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Life Steal"), 0.02f, new ConfigDescription("Life steal multiplier when damaging enemies with Leech.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTMinLifeSteal = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Minimum Life Steal"), 0.1f, new ConfigDescription("Minimum amount of life steal with leech, gets multiplied by the attackers level.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTBaseDamage = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Damage"), 0.5f, new ConfigDescription("How much damage the Leeching debuff deals per second.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTBaseDuration = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Base Duration"), 5f, new ConfigDescription("How long the Leeching debuff lasts.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTStackDuration = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Stack Duration"), 0f, new ConfigDescription("How much longer the Leeching debuff lasts per stack.", null, Array.Empty<object>()));

			LeptonDaisy_Enable = Config.Bind<bool>(new ConfigDefinition("Lepton Daisy", "Enable Changes"), true, new ConfigDescription("Enables changes for Lepton Daisy.", null, Array.Empty<object>()));
			LeptonDaisy_BaseHeal = Config.Bind<float>(new ConfigDefinition("Lepton Daisy", "Base Healing"), 0.1f, new ConfigDescription("How much healing to give at a single stack.", null, Array.Empty<object>()));
			LeptonDaisy_StackHeal = Config.Bind<float>(new ConfigDefinition("Lepton Daisy", "Stack Healing"), 0.1f, new ConfigDescription("How much extra healing to give for each additional stack.", null, Array.Empty<object>()));
			LeptonDaisy_CapHeal = Config.Bind<float>(new ConfigDefinition("Lepton Daisy", "Capped Healing"), 2f, new ConfigDescription("Healing limit, makes the stacking work hyperbolically. (Set to 0 or less to disable this)", null, Array.Empty<object>()));
			LeptonDaisy_HealTime = Config.Bind<float>(new ConfigDefinition("Lepton Daisy", "Nova Interval"), 10f, new ConfigDescription("The duration between each healing nova.", null, Array.Empty<object>()));

			LigmaLenses_Enable = Config.Bind<bool>(new ConfigDefinition("Lost Seers Lenses", "Enable Changes"), true, new ConfigDescription("Enables changes for Lost Seers Lenses.", null, Array.Empty<object>()));
			LigmaLenses_BaseDamage = Config.Bind<float>(new ConfigDefinition("Lost Seers Lenses", "Base Damage"), 0.15f, new ConfigDescription("Total damage at the first stack.", null, Array.Empty<object>()));
			LigmaLenses_StackDamage = Config.Bind<float>(new ConfigDefinition("Lost Seers Lenses", "Stack Damage"), 0.15f, new ConfigDescription("Total damage for each additional stack.", null, Array.Empty<object>()));
			LigmaLenses_BaseRadius = Config.Bind<float>(new ConfigDefinition("Lost Seers Lenses", "Base Radius"), 20f, new ConfigDescription("Radius at the first stack", null, Array.Empty<object>()));
			LigmaLenses_StackRadius = Config.Bind<float>(new ConfigDefinition("Lost Seers Lenses", "Stack Radius"), 0f, new ConfigDescription("Radius for each additional stack.", null, Array.Empty<object>()));
			LigmaLenses_Cooldown = Config.Bind<int>(new ConfigDefinition("Lost Seers Lenses", "Cooldown"), 10, new ConfigDescription("Cooldown between each use.", null, Array.Empty<object>()));
			LigmaLenses_ProcRate = Config.Bind<float>(new ConfigDefinition("Lost Seers Lenses", "Proc Coefficient"), 0f, new ConfigDescription("Proc Coefficient for the void seekers.", null, Array.Empty<object>()));

			StealthKit_Enable = Config.Bind<bool>(new ConfigDefinition("Old War Stealthkit", "Enable Changes"), true, new ConfigDescription("Enables changes for Old War Stealthkit.", null, Array.Empty<object>()));
			StealthKit_BaseRecharge = Config.Bind<float>(new ConfigDefinition("Old War Stealthkit", "Base Cooldown"), 30.0f, new ConfigDescription("How long it takes for this item to recharge. (Vanilla = 30)", null, Array.Empty<object>()));
			StealthKit_StackRecharge = Config.Bind<float>(new ConfigDefinition("Old War Stealthkit", "Stack Cooldown"), 0.5f, new ConfigDescription("How much faster it recharges for each additional stack. (Vanilla = 0.5)", null, Array.Empty<object>()));
			StealthKit_BuffDuration = Config.Bind<float>(new ConfigDefinition("Old War Stealthkit", "Buff Duration"), 5.0f, new ConfigDescription("Duration of the Stealth buff upon activation. (Vanilla = 5)", null, Array.Empty<object>()));
			StealthKit_CancelCombat = Config.Bind<bool>(new ConfigDefinition("Old War Stealthkit", "Cancel Combat"), true, new ConfigDescription("Puts you in 'Out of Combat' upon activation.", null, Array.Empty<object>()));
			StealthKit_CancelDanger = Config.Bind<bool>(new ConfigDefinition("Old War Stealthkit", "Cancel Danger"), true, new ConfigDescription("Puts you in 'Out of Danger' upon activation.", null, Array.Empty<object>()));
			StealthKit_CancelDuration = Config.Bind<float>(new ConfigDefinition("Old War Stealthkit", "Cancel Duration"), 1.0f, new ConfigDescription("Duration of the Combat and Danger cancel.", null, Array.Empty<object>()));

			Squid_Enable = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Enable Changes"), true, new ConfigDescription("Enables changes to Squid Polyp.", null, Array.Empty<object>()));
			Squid_ClayHit = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Apply Tar"), true, new ConfigDescription("Makes Squid Polyps apply the Tar debuff with their attack.", null, Array.Empty<object>()));
			Squid_InactiveDecay = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Inactive Removal"), 20f, new ConfigDescription("Makes Squid Polyps unable to heal if they've been inactive for X seconds.", null, Array.Empty<object>()));
			Squid_StackLife = Config.Bind<int>(new ConfigDefinition("Squid Polyp", "Lifetime Per Stack"), 3, new ConfigDescription("Increases the lifespan of the Squid Polyp by this much in seconds per stack.", null, Array.Empty<object>()));
			Squid_Armor = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Armor"), 10f, new ConfigDescription("Increases Squid Polyp armor by this much per stack.", null, Array.Empty<object>()));

			WarHorn_Enable = Config.Bind<bool>(new ConfigDefinition("War Horn", "Enable Changes"), true, new ConfigDescription("Enables changes to War Horn.", null, Array.Empty<object>()));
			WarHorn_BaseDuration = Config.Bind<float>(new ConfigDefinition("War Horn", "Base Duration"), 6f, new ConfigDescription("How long the War Horn's buff lasts at 1 stack.", null, Array.Empty<object>()));
			WarHorn_StackDuration = Config.Bind<float>(new ConfigDefinition("War Horn", "Stack Duration"), 2f, new ConfigDescription("How much longer the buff lasts from additional stacks.", null, Array.Empty<object>()));
			WarHorn_BaseSpeed = Config.Bind<float>(new ConfigDefinition("War Horn", "Base Attack Speed"), 0.6f, new ConfigDescription("How much attack speed the buff gives at 1 stack.", null, Array.Empty<object>()));
			WarHorn_StackSpeed = Config.Bind<float>(new ConfigDefinition("War Horn", "Stack Attack Speed"), 0.15f, new ConfigDescription("How much extra attack speed the buff gives from additional stacks.", null, Array.Empty<object>()));

			WaxQuail_Enable = Config.Bind<bool>(new ConfigDefinition("Wax Quail", "Enable Changes"), true, new ConfigDescription("Enables changes to Wax Quail.", null, Array.Empty<object>()));
			WaxQuail_BaseHori = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Base Horizontal Boost"), 12f, new ConfigDescription("How far horizontally to boost the user at 1 stack. (10 = Vanilla)", null, Array.Empty<object>()));
			WaxQuail_StackHori = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Stack Horizontal Boost"), 6f, new ConfigDescription("How far horizontally to boost the user from additional stacks. (10 = Vanilla)", null, Array.Empty<object>()));
			WaxQuail_CapHori = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Capped Horizontal Boost"), 150f, new ConfigDescription("Horizontal boost limit, makes the stacking work hyperbolically. (Set to 0 or less to disable this)", null, Array.Empty<object>()));
			WaxQuail_BaseVert = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Base Vertical Boost"), 0.2f, new ConfigDescription("How far vertically to boost the user at 1 stack. (0.5 = Hopoo Feather)", null, Array.Empty<object>()));
			WaxQuail_StackVert = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Stack Vertical Boost"), 0f, new ConfigDescription("How far vertically to boost the user from additional stacks.", null, Array.Empty<object>()));
			WaxQuail_CapVert = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Capped Vertical Boost"), 1f, new ConfigDescription("Vertical boost limit, makes the stacking work hyperbolically. (Set to 0 or less to disable this)", null, Array.Empty<object>()));
			WaxQuail_BaseAirSpeed = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Base Air Speed Bonus"), 0.14f, new ConfigDescription("Aerial speed boost at 1 stack.", null, Array.Empty<object>()));
			WaxQuail_StackAirSpeed = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Stack Air Speed Bonus"), 0.07f, new ConfigDescription("Aerial speed boost from additional stacks.", null, Array.Empty<object>()));
			WaxQuail_CapAirSpeed = Config.Bind<float>(new ConfigDefinition("Wax Quail", "Capped Air Speed Bonus"), 1.5f, new ConfigDescription("Aerial speed boost limit, makes the stacking work hyperbolically. (Set to 0 or less to disable this)", null, Array.Empty<object>()));

			Aegis_Enable = Config.Bind<bool>(new ConfigDefinition("Aegis", "Enable Changes"), true, new ConfigDescription("Enables changes to Aegis.", null, Array.Empty<object>()));
			Aegis_Regen = Config.Bind<bool>(new ConfigDefinition("Aegis", "Count Regen"), true, new ConfigDescription("Allows Aegis to convert excess regen into barrier.", null, Array.Empty<object>()));
			Aegis_Armor = Config.Bind<float>(new ConfigDefinition("Aegis", "Armor"), 20f, new ConfigDescription("How much Armor each Aegis gives.", null, Array.Empty<object>()));

			LaserScope_Enable = Config.Bind<bool>(new ConfigDefinition("Laser Scope", "Enable Changes"), true, new ConfigDescription("Enables changes to Laser Scope.", null, Array.Empty<object>()));
			LaserScope_Crit = Config.Bind<float>(new ConfigDefinition("Laser Scope", "Crit Chance"), 5f, new ConfigDescription("How much extra crit chance to give at a single stack.", null, Array.Empty<object>()));

			BensRaincoat_Enable = Config.Bind<bool>(new ConfigDefinition("Bens Raincoat", "Enable Changes"), true, new ConfigDescription("Enables changes to Ben's Raincoat.", null, Array.Empty<object>()));
			BensRaincoat_FixCooldown = Config.Bind<bool>(new ConfigDefinition("Bens Raincoat", "Improve Cooldown"), true, new ConfigDescription("Starts the cooldown when losing a stack of block instead of when losing all stacks.", null, Array.Empty<object>()));
			BensRaincoat_BaseBlock = Config.Bind<int>(new ConfigDefinition("Bens Raincoat", "Base Block"), 2, new ConfigDescription("How many debuff blocks to give at a single stack.", null, Array.Empty<object>()));
			BensRaincoat_StackBlock = Config.Bind<int>(new ConfigDefinition("Bens Raincoat", "Stack Block"), 1, new ConfigDescription("How many extra debuff blocks to give from additional stacks.", null, Array.Empty<object>()));
			BensRaincoat_Cooldown = Config.Bind<float>(new ConfigDefinition("Bens Raincoat", "Cooldown Time"), 7f, new ConfigDescription("How long in seconds it takes for the debuff blocks to restock. (Anything less than 0 will skip this change.)", null, Array.Empty<object>()));

			VoidsentFlame_Enable = Config.Bind<bool>(new ConfigDefinition("Voidsent Flame", "Enable Changes"), true, new ConfigDescription("Enables changes to Voidsent Flame.", null, Array.Empty<object>()));
			VoidsentFlame_BaseRadius = Config.Bind<float>(new ConfigDefinition("Voidsent Flame", "Base Radius"), 10f, new ConfigDescription("How large the blast radius is at a single stack. (12 = Vanilla)", null, Array.Empty<object>()));
			VoidsentFlame_StackRadius = Config.Bind<float>(new ConfigDefinition("Voidsent Flame", "Stack Radius"), 2f, new ConfigDescription("How much larger the blast radius becomes from additional stacks. (2.4 = Vanilla)", null, Array.Empty<object>()));
			VoidsentFlame_BaseDamage = Config.Bind<float>(new ConfigDefinition("Voidsent Flame", "Base Damage"), 2.6f, new ConfigDescription("Blast damage at a single stack. (2.6 = Vanilla)", null, Array.Empty<object>()));
			VoidsentFlame_StackDamage = Config.Bind<float>(new ConfigDefinition("Voidsent Flame", "Stack Damage"), 1.56f, new ConfigDescription("Blast damage from additional stacks. (1.56 = Vanilla)", null, Array.Empty<object>()));
			VoidsentFlame_ProcRate = Config.Bind<float>(new ConfigDefinition("Voidsent Flame", "Proc Coefficient"), 1f, new ConfigDescription("Blast proc coefficient. (1 = Vanilla)", null, Array.Empty<object>()));

			Knurl_Enable = Config.Bind<bool>(new ConfigDefinition("Titanic Knurl", "Enable Changes"), true, new ConfigDescription("Enables changes to Titanic Knurl.", null, Array.Empty<object>()));
			Knurl_BaseHP = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Base HP"), 40f, new ConfigDescription("The amount of HP each stack gives. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			Knurl_LevelHP = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Level HP"), 4f, new ConfigDescription("How much extra HP to give per level.", null, Array.Empty<object>()));
			Knurl_BaseRegen = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Base Regen"), 1.6f, new ConfigDescription("The amount of health regen each stack gives. (Set to 0 to disable this effect entirely.)", null, Array.Empty<object>()));
			Knurl_LevelRegen = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Level Regen"), 0.32f, new ConfigDescription("How much extra health regen to give per level.", null, Array.Empty<object>()));

			KnurlRework_Enable = Config.Bind<bool>(new ConfigDefinition("Titanic Knurl Rework", "Enable Rework"), false, new ConfigDescription("Enables the rework to Titanic Knurl. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			KnurlRework_BaseDamage = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Base Damage"), 7f, new ConfigDescription("Base Damage of the stone fist.", null, Array.Empty<object>()));
			KnurlRework_StackDamage = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Stack Damage"), 4f, new ConfigDescription("Stacking Damage of the stone fist.", null, Array.Empty<object>()));
			KnurlRework_BaseSpeed = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Base Cooldown"), 6f, new ConfigDescription("Cooldown between each stone fist.", null, Array.Empty<object>()));
			KnurlRework_StackSpeed = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Stack Cooldown"), 0.15f, new ConfigDescription("Reduces the cooldown between each stone fist per stack. (Works as attack speed does.)", null, Array.Empty<object>()));
			KnurlRework_ProcRate = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Proc Coefficient"), 1.0f, new ConfigDescription("The proc coefficient of the stone fist.", null, Array.Empty<object>()));
			KnurlRework_AttackRange = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Attack Distance"), 50.0f, new ConfigDescription("The maximum targeting radius in metres around you for the stone fist.", null, Array.Empty<object>()));
			KnurlRework_TargetType = Config.Bind<int>(new ConfigDefinition("Titanic Knurl Rework", "Target Mode"), 0, new ConfigDescription("Decides how the target is selected. (0 = Weak, 1 = Closest)", null, Array.Empty<object>()));

			Nucleus_Enable = Config.Bind<bool>(new ConfigDefinition("Defense Nucleus", "Enable Changes"), true, new ConfigDescription("Enables changes to Defense Nucleus.", null, Array.Empty<object>()));
			Nucleus_Infinite = Config.Bind<bool>(new ConfigDefinition("Defense Nucleus", "Minion Can Proc"), true, new ConfigDescription("Allows constructs to proc their owner's defense nucleus when they kill.", null, Array.Empty<object>()));
			Nucleus_BaseHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Base Health"), 10, new ConfigDescription("How much extra health the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_StackHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Stack Health"), 10, new ConfigDescription("How much extra health the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_BaseAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Base Attack Speed"), 6, new ConfigDescription("How much extra attack speed the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_StackAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Stack Attack Speed"), 0, new ConfigDescription("How much extra attack speed the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_BaseDamage = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Base Damage"), 6, new ConfigDescription("How much extra damage the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_StackDamage = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Stack Damage"), 8, new ConfigDescription("How much extra damage the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_Cooldown = Config.Bind<float>(new ConfigDefinition("Defense Nucleus", "Cooldown"), 1f, new ConfigDescription("Cooldown before this item can be procced again. (Cannot go below 0.25 because I said so.)", null, Array.Empty<object>()));

			NucleusRework_Enable = Config.Bind<bool>(new ConfigDefinition("Defense Nucleus Rework", "Enable Changes"), false, new ConfigDescription("Enables the rework to the Defense Nucleus. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			NucleusRework_SummonCount = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Summon Count"), 3, new ConfigDescription("How many constructs to summon on activation. (Cannot go above 6 because I said so.)", null, Array.Empty<object>()));
			NucleusRework_BaseHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Base Health"), 10, new ConfigDescription("How much extra health the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_StackHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Stack Health"), 10, new ConfigDescription("How much extra health the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_BaseAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Base Attack Speed"), 6, new ConfigDescription("How much extra attack speed the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_StackAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Stack Attack Speed"), 0, new ConfigDescription("How much extra attack speed the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_BaseDamage = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Base Damage"), 6, new ConfigDescription("How much extra damage the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_StackDamage = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Stack Damage"), 8, new ConfigDescription("How much extra damage the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_ShieldBaseDuration = Config.Bind<float>(new ConfigDefinition("Defense Nucleus Rework", "Shield Base Duration"), 3.5f, new ConfigDescription("How long in seconds that the shield lasts for.", null, Array.Empty<object>()));
			NucleusRework_ShieldStackDuration = Config.Bind<float>(new ConfigDefinition("Defense Nucleus Rework", "Shield Stack Duration"), 1f, new ConfigDescription("How many extra seconds the shield lasts for per stack.", null, Array.Empty<object>()));

			NucleusShared_TweakAI = Config.Bind<bool>(new ConfigDefinition("Alpha Construct Ally", "Better AI"), true, new ConfigDescription("Gives 360 Degree vision and prevents retaliation against team members.", null, Array.Empty<object>()));
			NucleusShared_Mechanical = Config.Bind<bool>(new ConfigDefinition("Alpha Construct Ally", "Is Mechanical"), true, new ConfigDescription("Gives it the Mechanical flag, allowing it to get Spare Drone Parts and Captain's Microbots.", null, Array.Empty<object>()));
			NucleusShared_ExtraDisplays = Config.Bind<bool>(new ConfigDefinition("Alpha Construct Ally", "Enable Modded Displays"), true, new ConfigDescription("Adds a few item displays to the Alpha Construct. (For Spare Drone Parts)", null, Array.Empty<object>()));
			NucleusShared_BlastRadius = Config.Bind<float>(new ConfigDefinition("Alpha Construct Ally", "Death Explosion Radius"), 12f, new ConfigDescription("Blast radius when they die. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			NucleusShared_BlastDamage = Config.Bind<float>(new ConfigDefinition("Alpha Construct Ally", "Death Explosion Damage"), 4.5f, new ConfigDescription("Blast damage when they die. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
		}
	}
}