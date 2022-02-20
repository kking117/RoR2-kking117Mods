using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;

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
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.FlatItemBuff";
		public const string MODNAME = "FlatItemBuff";
		public const string MODTOKEN = "KKING117_FLATITEMBUFF_";
		public const string MODVERSION = "1.4.0";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<bool> Steak_Change;
		public static ConfigEntry<float> Steak_BaseHP;
		public static ConfigEntry<float> Steak_LevelHP;
		public static ConfigEntry<float> Steak_OnKillDur;

		public static ConfigEntry<bool> Brooch_Change;
		public static ConfigEntry<float> Brooch_BaseFlatBarrier;
		public static ConfigEntry<float> Brooch_StackFlatBarrier;
		public static ConfigEntry<float> Brooch_BaseCentBarrier;
		public static ConfigEntry<float> Brooch_StackCentBarrier;

		public static ConfigEntry<bool> Squid_Change;
		public static ConfigEntry<bool> Squid_ClayHit;
		public static ConfigEntry<float> Squid_InactiveDecay;
		public static ConfigEntry<int> Squid_StackLife;
		public static ConfigEntry<float> Squid_Armor;

		public static ConfigEntry<bool> Infusion_Change;
		public static ConfigEntry<int> Infusion_Stacks;
		public static ConfigEntry<int> Infusion_Level;
		public static ConfigEntry<bool> Infusion_OwnerGains;
		public static ConfigEntry<bool> Infusion_InheritOwner;

		public static ConfigEntry<int> Infusion_Fake_Bonus;
		public static ConfigEntry<int> Infusion_Kill_Bonus;
		public static ConfigEntry<int> Infusion_Champ_Bonus;
		public static ConfigEntry<int> Infusion_Elite_Bonus;
		public static ConfigEntry<int> Infusion_Boss_Bonus;

		public static ConfigEntry<bool> Knurl_Change;
		public static ConfigEntry<float> Knurl_BaseHP;
		public static ConfigEntry<float> Knurl_LevelHP;
		public static ConfigEntry<float> Knurl_BaseRegen;
		public static ConfigEntry<float> Knurl_LevelRegen;

		public static ConfigEntry<bool> Knurl_Rework;
		public static ConfigEntry<float> Knurl_BaseDamage;
		public static ConfigEntry<float> Knurl_StackDamage;
		public static ConfigEntry<float> Knurl_BaseSpeed;
		public static ConfigEntry<float> Knurl_StackSpeed;
		public void Awake()
        {
			ModLogger = this.Logger;
			ReadConfig();
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
			if (Knurl_Rework.Value)
            {
				ItemChanges.TitanicKnurl_Rework.EnableChanges();
			}
			else if(Knurl_Change.Value)
            {
				ItemChanges.TitanicKnurl.EnableChanges();
			}
			ModLogger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		public void ReadConfig()
		{
			Steak_Change = Config.Bind<bool>(new ConfigDefinition("Bison Steak", "Enable Changes"), true, new ConfigDescription("Enables changes to Bison Steak.", null, Array.Empty<object>()));
			Steak_BaseHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base HP"), 25.0f, new ConfigDescription("The amount of HP each stack increases.", null, Array.Empty<object>()));
			Steak_LevelHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Level HP"), 2.5f, new ConfigDescription("How much extra HP to give per level.", null, Array.Empty<object>()));
			Steak_OnKillDur = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Buff Duration"), 3f, new ConfigDescription("Gives the Fresh Steak regen effect for X seconds on kill. (0 or less disables this)", null, Array.Empty<object>()));

			Brooch_Change = Config.Bind<bool>(new ConfigDefinition("Topaz Brooch", "Enable Changes"), true, new ConfigDescription("Enables changes to Topaz Brooch.", null, Array.Empty<object>()));
			Brooch_BaseFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackFlatBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Flat Barrier"), 14.0f, new ConfigDescription("The amount of flat Barrier each stack after the first gives.", null, Array.Empty<object>()));
			Brooch_BaseCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Base Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier a single stack gives.", null, Array.Empty<object>()));
			Brooch_StackCentBarrier = Config.Bind<float>(new ConfigDefinition("Topaz Brooch", "Stack Percent Barrier"), 0.005f, new ConfigDescription("The amount of percent Barrier each stack after the first gives.", null, Array.Empty<object>()));

			Squid_Change = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Enable Changes"), true, new ConfigDescription("Enables changes to Squid Polyp.", null, Array.Empty<object>()));
			Squid_ClayHit = Config.Bind<bool>(new ConfigDefinition("Squid Polyp", "Apply Tar"), true, new ConfigDescription("Makes Squid Polyps apply the Tar debuff with their attack.", null, Array.Empty<object>()));
			Squid_InactiveDecay = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Inactive Removal"), 20f, new ConfigDescription("Makes Squid Polyps unable to heal if they've been inactive for X seconds.", null, Array.Empty<object>()));
			Squid_StackLife = Config.Bind<int>(new ConfigDefinition("Squid Polyp", "Lifetime Per Stack"), 3, new ConfigDescription("Increases the lifespan of the Squid Polyp by this much in seconds per stack.", null, Array.Empty<object>()));
			Squid_Armor = Config.Bind<float>(new ConfigDefinition("Squid Polyp", "Armor"), 10f, new ConfigDescription("Increases Squid Polyp armor by this much per stack.", null, Array.Empty<object>()));

			Infusion_Change = Config.Bind<bool>(new ConfigDefinition("Infusion", "Enable Changes"), true, new ConfigDescription("Enables changes to Infusion.", null, Array.Empty<object>()));
			Infusion_Stacks = Config.Bind<int>(new ConfigDefinition("Infusion", "Max Stacks"), 200, new ConfigDescription("How many stacks an infusion has (100 is the vanilla value).", null, Array.Empty<object>()));
			Infusion_Level = Config.Bind<int>(new ConfigDefinition("Infusion", "Level Per Stacks"), 100, new ConfigDescription("How many stacks are needed to gain a level up.", null, Array.Empty<object>()));
			Infusion_OwnerGains = Config.Bind<bool>(new ConfigDefinition("Infusion", "Gives To Owner"), true, new ConfigDescription("Should minions with infusions send their uncollected samples to their owner instead.", null, Array.Empty<object>()));
			Infusion_InheritOwner = Config.Bind<bool>(new ConfigDefinition("Infusion", "Inherit From Owner"), true, new ConfigDescription("Should minions with infusions inherit their owner's collected samples.", null, Array.Empty<object>()));

			Infusion_Fake_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Fake Stack"), 1, new ConfigDescription("How many samples certain non-ai and non-player enemies give.", null, Array.Empty<object>()));
			Infusion_Kill_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Kill Stack"), 2, new ConfigDescription("How many samples normal enemies give.", null, Array.Empty<object>()));
			Infusion_Champ_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Champion Stack"), 8, new ConfigDescription("How many samples champion enemies give.", null, Array.Empty<object>()));
			Infusion_Elite_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Elite Bonus"), 3, new ConfigDescription("Sample multiplier for elites.", null, Array.Empty<object>()));
			Infusion_Boss_Bonus = Config.Bind<int>(new ConfigDefinition("Infusion", "Boss Bonus"), 2, new ConfigDescription("Sample multiplier for bosses.", null, Array.Empty<object>()));

			Knurl_Change = Config.Bind<bool>(new ConfigDefinition("Titanic Knurl", "Enable Changes"), true, new ConfigDescription("Enables changes to Titanic Knurl.", null, Array.Empty<object>()));
			Knurl_BaseHP = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Base HP"), 40f, new ConfigDescription("The amount of HP each stack gives. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			Knurl_LevelHP = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Level HP"), 4f, new ConfigDescription("How much extra HP to give per level.", null, Array.Empty<object>()));
			Knurl_BaseRegen = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Base Regen"), 1.6f, new ConfigDescription("The amount of health regen each stack gives. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			Knurl_LevelRegen = Config.Bind<float>(new ConfigDefinition("Titanic Knurl", "Level Regen"), 0.32f, new ConfigDescription("How much extra health regen to give per level.", null, Array.Empty<object>()));

			Knurl_Rework = Config.Bind<bool>(new ConfigDefinition("Titanic Knurl Rework", "Enable Rework"), false, new ConfigDescription("Enables the rework to Titanic Knurl. (Has priority over the normal changes.)", null, Array.Empty<object>()));
			Knurl_BaseDamage = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Base Damage"), 5f, new ConfigDescription("Base Damage of the stone fist. (Set to 0 to disable this effect entirely)", null, Array.Empty<object>()));
			Knurl_StackDamage = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Stack Damage"), 3f, new ConfigDescription("Stacking Damage of the stone fist.", null, Array.Empty<object>()));
			Knurl_BaseSpeed = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Base Cooldown"), 6f, new ConfigDescription("Cooldown between each stone fist.", null, Array.Empty<object>()));
			Knurl_StackSpeed = Config.Bind<float>(new ConfigDefinition("Titanic Knurl Rework", "Stack Cooldown"), 0.2f, new ConfigDescription("Reduces the cooldown between each stone fist per stack. (Works as attack speed does.)", null, Array.Empty<object>()));
		}
	}
}
