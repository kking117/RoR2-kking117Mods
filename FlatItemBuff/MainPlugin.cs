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
		public const string MODVERSION = "1.11.0";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<bool> Steak_Change;
		public static ConfigEntry<float> Steak_BaseHP;
		public static ConfigEntry<float> Steak_LevelHP;
		public static ConfigEntry<float> Steak_BaseBuffDur;
		public static ConfigEntry<float> Steak_StackBuffDur;

		public static ConfigEntry<bool> Brooch_Change;
		public static ConfigEntry<float> Brooch_BaseFlatBarrier;
		public static ConfigEntry<float> Brooch_StackFlatBarrier;
		public static ConfigEntry<float> Brooch_BaseCentBarrier;
		public static ConfigEntry<float> Brooch_StackCentBarrier;

		public static ConfigEntry<bool> Harpoon_Change;
		public static ConfigEntry<float> Harpoon_BaseDuration;
		public static ConfigEntry<float> Harpoon_StackDuration;
		public static ConfigEntry<float> Harpoon_MoveSpeed;
		public static ConfigEntry<float> Harpoon_CooldownRate;

		public static ConfigEntry<bool> Infusion_Change;
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

		public static ConfigEntry<bool> LeechingSeed_Change;
		public static ConfigEntry<float> LeechingSeed_ProcHeal;
		public static ConfigEntry<float> LeechingSeed_NoProcHeal;

		public static ConfigEntry<bool> LeechingSeedRework_Enable;
		public static ConfigEntry<float> LeechingSeedRework_DoTFlatHeal;
		public static ConfigEntry<float> LeechingSeedRework_DoTChance;
		public static ConfigEntry<float> LeechingSeedRework_DoTLifeSteal;
		public static ConfigEntry<float> LeechingSeedRework_DoTBaseDamage;
		public static ConfigEntry<float> LeechingSeedRework_DoTBaseDuration;
		public static ConfigEntry<float> LeechingSeedRework_DoTStackDuration;

		public static ConfigEntry<bool> StealthKit_Change;
		public static ConfigEntry<bool> StealthKit_CancelCombat;
		public static ConfigEntry<bool> StealthKit_CancelDanger;

		public static ConfigEntry<bool> Squid_Change;
		public static ConfigEntry<bool> Squid_ClayHit;
		public static ConfigEntry<float> Squid_InactiveDecay;
		public static ConfigEntry<int> Squid_StackLife;
		public static ConfigEntry<float> Squid_Armor;

		public static ConfigEntry<bool> WarHorn_Change;
		public static ConfigEntry<float> WarHorn_BaseDuration;
		public static ConfigEntry<float> WarHorn_StackDuration;
		public static ConfigEntry<float> WarHorn_BaseSpeed;
		public static ConfigEntry<float> WarHorn_StackSpeed;

		public static ConfigEntry<bool> Aegis_Change;
		public static ConfigEntry<bool> Aegis_Regen;
		public static ConfigEntry<float> Aegis_Armor;

		public static ConfigEntry<bool> BenthicRework_Enable;
		public static ConfigEntry<int> BenthicRework_BaseCount;
		public static ConfigEntry<int> BenthicRework_StackCount;
		public static ConfigEntry<string> BenthicRework_BanList;
		public static ConfigEntry<float> BenthicRework_VoidManBonus;
		public static ConfigEntry<bool> BenthicRework_BuffDamage;
		public static ConfigEntry<bool> BenthicRework_BuffHealth;
		public static ConfigEntry<bool> BenthicRework_BuffSpeed;
		public static ConfigEntry<int> BenthicRework_SortMethod;
		public static ConfigEntry<string> BenthicRework_TierFavour;

		public static ConfigEntry<bool> Knurl_Change;
		public static ConfigEntry<float> Knurl_BaseHP;
		public static ConfigEntry<float> Knurl_LevelHP;
		public static ConfigEntry<float> Knurl_BaseRegen;
		public static ConfigEntry<float> Knurl_LevelRegen;

		public static ConfigEntry<bool> KnurlRework_Enable;
		public static ConfigEntry<float> KnurlRework_BaseDamage;
		public static ConfigEntry<float> KnurlRework_StackDamage;
		public static ConfigEntry<float> KnurlRework_BaseSpeed;
		public static ConfigEntry<float> KnurlRework_StackSpeed;

		public static ConfigEntry<bool> Nucleus_Change;
		public static ConfigEntry<bool> Nucleus_Infinite;
		public static ConfigEntry<int> Nucleus_BaseHealth;
		public static ConfigEntry<int> Nucleus_BaseAttack;
		public static ConfigEntry<int> Nucleus_StackHealth;
		public static ConfigEntry<int> Nucleus_StackAttack;

		public static ConfigEntry<bool> NucleusRework_Enable;
		public static ConfigEntry<int> NucleusRework_SummonCount;
		public static ConfigEntry<int> NucleusRework_BaseHealth;
		public static ConfigEntry<int> NucleusRework_BaseAttack;
		public static ConfigEntry<int> NucleusRework_StackHealth;
		public static ConfigEntry<int> NucleusRework_StackAttack;
		public static ConfigEntry<float> NucleusRework_ShieldBaseDuration;
		public static ConfigEntry<float> NucleusRework_ShieldStackDuration;

		public static ConfigEntry<bool> NucleusShared_TweakAI;
		public static ConfigEntry<float> NucleusShared_BlastRadius;
		public static ConfigEntry<float> NucleusShared_BlastDamage;
		public static ConfigEntry<bool> NucleusShared_Mechanical;
		public static ConfigEntry<bool> NucleusShared_ExtraDisplays;

		public static ConfigEntry<bool> RainCoat_Change;
		public static ConfigEntry<int> RainCoat_BaseBlock;
		public static ConfigEntry<int> RainCoat_StackBlock;
		public static ConfigEntry<float> RainCoat_Cooldown;
		public static ConfigEntry<bool> RainCoat_ImproveCooldown;
		private void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
			if (Steak_Change.Value)
			{
				ItemChanges.BisonSteak.EnableChanges();
			}
			if (Brooch_Change.Value)
			{
				ItemChanges.TopazBrooch.EnableChanges();
			}
			if (Infusion_Change.Value)
			{
				ItemChanges.Infusion.EnableChanges();
			}
			if (Squid_Change.Value)
			{
				ItemChanges.SquidPolyp.EnableChanges();
			}
			if (KnurlRework_Enable.Value)
			{
				ItemChanges.TitanicKnurl_Rework.EnableChanges();
			}
			else if (Knurl_Change.Value)
			{
				ItemChanges.TitanicKnurl.EnableChanges();
			}
			if (NucleusRework_Enable.Value)
			{
				ItemChanges.DefenseNucleus_Rework.EnableChanges();
			}
			else if (Nucleus_Change.Value)
			{
				ItemChanges.DefenseNucleus.EnableChanges();
			}
			if (LeechingSeedRework_Enable.Value)
			{
				ItemChanges.LeechingSeed_Rework.EnableChanges();
			}
			else if (LeechingSeed_Change.Value)
			{
				ItemChanges.LeechingSeed.EnableChanges();
			}
			if (StealthKit_Change.Value)
			{
				ItemChanges.Stealthkit.EnableChanges();
			}
			if(WarHorn_Change.Value)
            {
				ItemChanges.WarHorn.EnableChanges();
            }
			if(Aegis_Change.Value)
            {
				ItemChanges.Aegis.EnableChanges();
			}
			if(RainCoat_Change.Value)
            {
				ItemChanges.RainCoat.EnableChanges();
			}
			if(Harpoon_Change.Value)
            {
				ItemChanges.HuntersHarpoon.EnableChanges();
			}
			if (BenthicRework_Enable.Value)
			{
				ItemChanges.BenthicBloom_Rework.EnableChanges();
			}
			ModLogger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		private void PostLoad()
        {
			if (NucleusRework_Enable.Value || Nucleus_Change.Value)
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
			Steak_Change = Config.Bind<bool>(new ConfigDefinition("Bison Steak", "Enable Changes"), true, new ConfigDescription("Enables changes to Bison Steak.", null, Array.Empty<object>()));
			Steak_BaseHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base HP"), 25.0f, new ConfigDescription("The amount of HP each stack increases. (Set to 0 or less to disable health.)", null, Array.Empty<object>()));
			Steak_LevelHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Level HP"), 2.5f, new ConfigDescription("How much extra HP to give per level.", null, Array.Empty<object>()));
			Steak_BaseBuffDur = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base Buff Duration"), 3f, new ConfigDescription("Gives the Fresh Meat regen effect for X seconds on kill. (Set to 0 or less to disable regen.)", null, Array.Empty<object>()));
			Steak_StackBuffDur = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Stack Buff Duration"), 3f, new ConfigDescription("Gives the Fresh Meat regen effect for X seconds on kill per stack.", null, Array.Empty<object>()));

			Brooch_Change = Config.Bind<bool>(new ConfigDefinition("Topaz Brooch", "Enable Changes"), true, new ConfigDescription("Enables changes to Topaz Brooch.", null, Array.Empty<object>()));
			Brooch_BaseFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier each stack after the first gives.", null, Array.Empty<object>()));
			Brooch_BaseCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier each stack after the first gives.", null, Array.Empty<object>()));

			Harpoon_Change = Config.Bind<bool>(new ConfigDefinition("Hunters Harpoon", "Enable Changes"), true, new ConfigDescription("Enables changes to Hunters Harpoon.", null, Array.Empty<object>()));
			Harpoon_BaseDuration = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Base Duration"), 1.5f, new ConfigDescription("Buff duration at a single stack.", null, Array.Empty<object>()));
			Harpoon_StackDuration = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Stack Duration"), 0.75f, new ConfigDescription("Extra buff duration from additional stacks.", null, Array.Empty<object>()));
			Harpoon_MoveSpeed = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Move Speed Bonus"), 0.25f, new ConfigDescription("How much movement speed each stack of the buff gives. (Set to 0 to disable)", null, Array.Empty<object>()));
			Harpoon_CooldownRate = Config.Bind<float>(new ConfigDefinition("Hunters Harpoon", "Cooldown Rate Bonus"), 0.25f, new ConfigDescription("How much primary and secondary cooldown rate each stack of the buff gives. (Set to 0 to disable)", null, Array.Empty<object>()));

			Infusion_Change = Config.Bind<bool>(new ConfigDefinition("Infusion", "Enable Changes"), true, new ConfigDescription("Enables changes to Infusion.", null, Array.Empty<object>()));
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

			LeechingSeed_Change = Config.Bind<bool>(new ConfigDefinition("Leeching Seed", "Enable Changes"), true, new ConfigDescription("Enables changes for Leeching Seed.", null, Array.Empty<object>()));
			LeechingSeed_ProcHeal = Config.Bind<float>(new ConfigDefinition("Leeching Seed", "Normal Heal"), 1f, new ConfigDescription("How much healing to give on hits with a proc coefficient. (Set to 0 to disable this effect entirely.)", null, Array.Empty<object>()));
			LeechingSeed_NoProcHeal = Config.Bind<float>(new ConfigDefinition("Leeching Seed", "Fixed Heal"), 0.5f, new ConfigDescription("How much extra healing to give regardless of proc coefficient. (Set to 0 to disable this effect entirely.)", null, Array.Empty<object>()));

			LeechingSeedRework_Enable = Config.Bind<bool>(new ConfigDefinition("Leeching Seed Rework", "Enable Changes"), false, new ConfigDescription("Enables the rework for Leeching Seed. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			LeechingSeedRework_DoTFlatHeal = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "DoT Heal"), 1.5f, new ConfigDescription("How much healing DoTs give per hit per stack. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			LeechingSeedRework_DoTChance = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Chance"), 25f, new ConfigDescription("Proc chance of the Leeching debuff. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			LeechingSeedRework_DoTLifeSteal = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Life Steal"), 0.04f, new ConfigDescription("Life steal multiplier when damaging enemies with Leech.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTBaseDamage = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Damage"), 0.5f, new ConfigDescription("How much damage the Leeching debuff deals per second.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTBaseDuration = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Base Duration"), 5f, new ConfigDescription("How long the Leeching debuff lasts.", null, Array.Empty<object>()));
			LeechingSeedRework_DoTStackDuration = Config.Bind<float>(new ConfigDefinition("Leeching Seed Rework", "Leech Stack Duration"), 0f, new ConfigDescription("How much longer the Leeching debuff lasts per stack.", null, Array.Empty<object>()));

			StealthKit_Change = Config.Bind<bool>(new ConfigDefinition("Old War Stealthkit", "Enable Changes"), true, new ConfigDescription("Enables changes for Old War Stealthkit.", null, Array.Empty<object>()));
			StealthKit_CancelCombat = Config.Bind<bool>(new ConfigDefinition("Old War Stealthkit", "Cancel Combat"), true, new ConfigDescription("Puts you in 'Out of Combat' during the stealth buff.", null, Array.Empty<object>()));
			StealthKit_CancelDanger = Config.Bind<bool>(new ConfigDefinition("Old War Stealthkit", "Cancel Danger"), true, new ConfigDescription("Puts you in 'Out of Danger' during the stealth buff.", null, Array.Empty<object>()));

			Squid_Change = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Enable Changes"), true, new ConfigDescription("Enables changes to Squid Polyp.", null, Array.Empty<object>()));
			Squid_ClayHit = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Apply Tar"), true, new ConfigDescription("Makes Squid Polyps apply the Tar debuff with their attack.", null, Array.Empty<object>()));
			Squid_InactiveDecay = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Inactive Removal"), 20f, new ConfigDescription("Makes Squid Polyps unable to heal if they've been inactive for X seconds.", null, Array.Empty<object>()));
			Squid_StackLife = Config.Bind<int>(new ConfigDefinition("Squid Polyp", "Lifetime Per Stack"), 3, new ConfigDescription("Increases the lifespan of the Squid Polyp by this much in seconds per stack.", null, Array.Empty<object>()));
			Squid_Armor = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Armor"), 10f, new ConfigDescription("Increases Squid Polyp armor by this much per stack.", null, Array.Empty<object>()));

			WarHorn_Change = Config.Bind<bool>(new ConfigDefinition("War Horn", "Enable Changes"), true, new ConfigDescription("Enables changes to War Horn.", null, Array.Empty<object>()));
			WarHorn_BaseDuration = Config.Bind<float>(new ConfigDefinition("War Horn", "Base Duration"), 6f, new ConfigDescription("How long the War Horn's buff lasts at 1 stack.", null, Array.Empty<object>()));
			WarHorn_StackDuration = Config.Bind<float>(new ConfigDefinition("War Horn", "Stack Duration"), 2f, new ConfigDescription("How much longer the buff lasts from additional stacks.", null, Array.Empty<object>()));
			WarHorn_BaseSpeed = Config.Bind<float>(new ConfigDefinition("War Horn", "Base Attack Speed"), 0.6f, new ConfigDescription("How much attack speed the buff gives at 1 stack.", null, Array.Empty<object>()));
			WarHorn_StackSpeed = Config.Bind<float>(new ConfigDefinition("War Horn", "Stack Attack Speed"), 0.15f, new ConfigDescription("How much extra attack speed the buff gives from additional stacks.", null, Array.Empty<object>()));

			Aegis_Change = Config.Bind<bool>(new ConfigDefinition("Aegis", "Enable Changes"), true, new ConfigDescription("Enables changes to Aegis.", null, Array.Empty<object>()));
			Aegis_Regen = Config.Bind<bool>(new ConfigDefinition("Aegis", "Count Regen"), true, new ConfigDescription("Allows Aegis to convert excess regen into barrier.", null, Array.Empty<object>()));
			Aegis_Armor = Config.Bind<float>(new ConfigDefinition("Aegis", "Armor"), 20f, new ConfigDescription("How much Armor each Aegis gives.", null, Array.Empty<object>()));

			RainCoat_Change = Config.Bind<bool>(new ConfigDefinition("Bens Raincoat", "Enable Changes"), true, new ConfigDescription("Enables changes to Ben's Raincoat.", null, Array.Empty<object>()));
			RainCoat_ImproveCooldown = Config.Bind<bool>(new ConfigDefinition("Bens Raincoat", "Improve Cooldown"), true, new ConfigDescription("Starts the cooldown when losing a stack of block instead of when losing all stacks.", null, Array.Empty<object>()));
			RainCoat_BaseBlock = Config.Bind<int>(new ConfigDefinition("Bens Raincoat", "Base Block"), 2, new ConfigDescription("How many debuff blocks to give at 1 stack.", null, Array.Empty<object>()));
			RainCoat_StackBlock = Config.Bind<int>(new ConfigDefinition("Bens Raincoat", "Stack Block"), 1, new ConfigDescription("How many extra debuff blocks to give from additional stacks.", null, Array.Empty<object>()));
			RainCoat_Cooldown = Config.Bind<float>(new ConfigDefinition("Bens Raincoat", "Cooldown Time"), 7f, new ConfigDescription("How long in seconds it takes for the debuff blocks to restock. (Anything less than 0 will skip this change.)", null, Array.Empty<object>()));

			BenthicRework_Enable = Config.Bind<bool>(new ConfigDefinition("Benthic Bloom Rework", "Enable Changes"), false, new ConfigDescription("Enables the rework to Benthic Bloom. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			BenthicRework_BaseCount = Config.Bind<int>(new ConfigDefinition("Benthic Bloom Rework", "Base Corrupt Count"), 1, new ConfigDescription("How many items to corrupt at a single stack.", null, Array.Empty<object>()));
			BenthicRework_StackCount = Config.Bind<int>(new ConfigDefinition("Benthic Bloom Rework", "Stack Corrupt Count"), 1, new ConfigDescription("How many items to corrupt for each additional stack.", null, Array.Empty<object>()));
			BenthicRework_BanList = Config.Bind<string>(new ConfigDefinition("Benthic Bloom Rework", "Corrupt Blacklist"), "", new ConfigDescription("Prevents corrupting into these specific items. (Example = Clover, CloverVoid, ExtraLife, MoreMissile)", null, Array.Empty<object>()));
			BenthicRework_VoidManBonus = Config.Bind<float>(new ConfigDefinition("Benthic Bloom Rework", "Corruption Bonus"), 0.01f, new ConfigDescription("How much to increases stats by for each Void item.", null, Array.Empty<object>()));
			BenthicRework_BuffDamage = Config.Bind<bool>(new ConfigDefinition("Benthic Bloom Rework", "Damage Bonus"), true, new ConfigDescription("Allows the corruption bonus to increase Damage.", null, Array.Empty<object>()));
			BenthicRework_BuffHealth = Config.Bind<bool>(new ConfigDefinition("Benthic Bloom Rework", "Health Bonus"), true, new ConfigDescription("Allows the corruption bonus to increase Health.", null, Array.Empty<object>()));
			BenthicRework_BuffSpeed = Config.Bind<bool>(new ConfigDefinition("Benthic Bloom Rework", "Speed Bonus"), true, new ConfigDescription("Allows the corruption bonus to increase Movement Speed.", null, Array.Empty<object>()));
			BenthicRework_SortMethod = Config.Bind<int>(new ConfigDefinition("Benthic Bloom Rework", "Selection Method"), 2, new ConfigDescription("The method to use when selecting which item to corrupt. (0 = random, 1 = most common tier first, 2 = tier weighted)", null, Array.Empty<object>()));
			BenthicRework_TierFavour = Config.Bind<string>(new ConfigDefinition("Benthic Bloom Rework", "Tier Weights"), "12, 6, 1, 1", new ConfigDescription("Higher number means that tier is more likely to be selected for each corruption. (Tier1, Tier2, Tier3, BossTier)", null, Array.Empty<object>()));

			Knurl_Change = Config.Bind<bool>(new ConfigDefinition("Titanic Knurl", "Enable Changes"), true, new ConfigDescription("Enables changes to Titanic Knurl.", null, Array.Empty<object>()));
			Knurl_BaseHP = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Base HP"), 40f, new ConfigDescription("The amount of HP each stack gives. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			Knurl_LevelHP = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Level HP"), 4f, new ConfigDescription("How much extra HP to give per level.", null, Array.Empty<object>()));
			Knurl_BaseRegen = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Base Regen"), 1.6f, new ConfigDescription("The amount of health regen each stack gives. (Set to 0 to disable this effect entirely.)", null, Array.Empty<object>()));
			Knurl_LevelRegen = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Level Regen"), 0.32f, new ConfigDescription("How much extra health regen to give per level.", null, Array.Empty<object>()));

			KnurlRework_Enable = Config.Bind<bool>(new ConfigDefinition("Titanic Knurl Rework", "Enable Rework"), false, new ConfigDescription("Enables the rework to Titanic Knurl. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			KnurlRework_BaseDamage = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Base Damage"), 7f, new ConfigDescription("Base Damage of the stone fist. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			KnurlRework_StackDamage = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Stack Damage"), 3.5f, new ConfigDescription("Stacking Damage of the stone fist.", null, Array.Empty<object>()));
			KnurlRework_BaseSpeed = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Base Cooldown"), 6f, new ConfigDescription("Cooldown between each stone fist.", null, Array.Empty<object>()));
			KnurlRework_StackSpeed = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Stack Cooldown"), 0.15f, new ConfigDescription("Reduces the cooldown between each stone fist per stack. (Works as attack speed does.)", null, Array.Empty<object>()));

			Nucleus_Change = Config.Bind<bool>(new ConfigDefinition("Defense Nucleus", "Enable Changes"), true, new ConfigDescription("Enables changes to Defense Nucleus.", null, Array.Empty<object>()));
			Nucleus_Infinite = Config.Bind<bool>(new ConfigDefinition("Defense Nucleus", "Minion Can Proc"), true, new ConfigDescription("Allows constructs to proc their owner's defense nucleus when they kill.", null, Array.Empty<object>()));
			Nucleus_BaseHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Base Health"), 0, new ConfigDescription("How much extra health the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_StackHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Stack Health"), 5, new ConfigDescription("How much extra health the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_BaseAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Base Attack Speed"), 10, new ConfigDescription("How much extra attack speed the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			Nucleus_StackAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus", "Stack Attack Speed"), 10, new ConfigDescription("How much extra attack speed the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));

			NucleusRework_Enable = Config.Bind<bool>(new ConfigDefinition("Defense Nucleus Rework", "Enable Changes"), false, new ConfigDescription("Enables the rework to the Defense Nucleus. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			NucleusRework_SummonCount = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Summon Count"), 4, new ConfigDescription("How many constructs to summon on activation. (Cannot go above 6 because I said so.)", null, Array.Empty<object>()));
			NucleusRework_BaseHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Base Health"), 10, new ConfigDescription("How much extra health the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_StackHealth = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Stack Health"), 10, new ConfigDescription("How much extra health the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_BaseAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Base Attack Speed"), 10, new ConfigDescription("How much extra attack speed the constructs get. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_StackAttack = Config.Bind<int>(new ConfigDefinition("Defense Nucleus Rework", "Stack Attack Speed"), 10, new ConfigDescription("How much extra attack speed the constructs get per stack. (1 = +10%)", null, Array.Empty<object>()));
			NucleusRework_ShieldBaseDuration = Config.Bind<float>(new ConfigDefinition("Defense Nucleus Rework", "Shield Base Duration"), 3.0f, new ConfigDescription("How long in seconds that the shield lasts for.", null, Array.Empty<object>()));
			NucleusRework_ShieldStackDuration = Config.Bind<float>(new ConfigDefinition("Defense Nucleus Rework", "Shield Stack Duration"), 0.75f, new ConfigDescription("How many extra seconds the shield lasts for per stack.", null, Array.Empty<object>()));

			NucleusShared_TweakAI = Config.Bind<bool>(new ConfigDefinition("Alpha Construct Ally", "Better AI"), true, new ConfigDescription("Gives 360 Degree vision and prevents retaliation against team members.", null, Array.Empty<object>()));
			NucleusShared_Mechanical = Config.Bind<bool>(new ConfigDefinition("Alpha Construct Ally", "Is Mechanical"), true, new ConfigDescription("Gives it the Mechanical flag, allowing it to get Spare Drone Parts and Captain's Microbots.", null, Array.Empty<object>()));
			NucleusShared_ExtraDisplays = Config.Bind<bool>(new ConfigDefinition("Alpha Construct Ally", "Enable Modded Displays"), true, new ConfigDescription("Adds a few item displays to the Alpha Construct. (For Spare Drone Parts)", null, Array.Empty<object>()));
			NucleusShared_BlastRadius = Config.Bind<float>(new ConfigDefinition("Alpha Construct Ally", "Death Explosion Radius"), 12f, new ConfigDescription("Blast radius when they die. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			NucleusShared_BlastDamage = Config.Bind<float>(new ConfigDefinition("Alpha Construct Ally", "Death Explosion Damage"), 4.5f, new ConfigDescription("Blast damage when they die. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
		}
	}
}