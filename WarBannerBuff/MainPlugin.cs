using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace WarBannerBuff
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[BepInDependency("com.bepis.r2api")]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI"
	})]

	public class MainPlugin : BaseUnityPlugin
    {
		public const string MODUID = "com.kking117.WarBannerBuff";
		public const string MODNAME = "WarBannerBuff";
		public const string MODTOKEN = "KKING117_WARBANNERBUFF_";
		public const string MODVERSION = "5.0.2";

		public static ConfigEntry<float> RecoveryTick;

		public static ConfigEntry<float> HealBase;
		public static ConfigEntry<float> HealLevel;
		public static ConfigEntry<float> HealMin;

		public static ConfigEntry<float> RechargeBase;
		public static ConfigEntry<float> RechargeLevel;
		public static ConfigEntry<float> RechargeMin;

		public static ConfigEntry<float> RegenBonus;
		public static ConfigEntry<float> DamageBonus;
		public static ConfigEntry<float> CritBonus;
		public static ConfigEntry<float> ArmorBonus;
		public static ConfigEntry<float> AttackBonus;
		public static ConfigEntry<bool> UseBaseAttackSpeed;
		public static ConfigEntry<float> MoveBonus;

		public static ConfigEntry<float> VoidBanner;
		public static ConfigEntry<float> PillarBanner;
		public static ConfigEntry<float> DeepVoidBanner;
		public static ConfigEntry<float> BossBanner;

		public static ConfigEntry<float> FocusBanner;
		public void Awake()
		{
			ReadConfig();
			ItemChanges.WarBanner.Begin();
			new Modules.ContentPacks().Initialize();
		}
		public void ReadConfig()
		{
			RecoveryTick = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Tick Interval"), 0.0f, new ConfigDescription("Delay in seconds between each heal/recharge tick. (0 or less disables this feature)", null, Array.Empty<object>()));

			HealBase = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Heal Max Health"), 0.0025f, new ConfigDescription("Heal this % amount of health based on their total health. (0.01 = 1%)", null, Array.Empty<object>()));
			HealLevel = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Heal Level Health"), 0.25f, new ConfigDescription("Heal this flat amount of health per level.", null, Array.Empty<object>()));
			HealMin = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Minimum Heal"), 1.0f, new ConfigDescription("The minimum amount of healing to gain.", null, Array.Empty<object>()));

			RechargeBase = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Recharge Max Shield"), 0.0f, new ConfigDescription("Recharge this % amount of shield based on their total shield. (0.01 = 1%)", null, Array.Empty<object>()));
			RechargeLevel = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Recharge Level Shield"), 0.0f, new ConfigDescription("Recharge this flat amount of shield per level.", null, Array.Empty<object>()));
			RechargeMin = Config.Bind<float>(new ConfigDefinition("Healing and Recharge", "Minimum Recharge"), 0.0f, new ConfigDescription("The minimum amount of recharge to gain.", null, Array.Empty<object>()));

			UseBaseAttackSpeed = Config.Bind<bool>(new ConfigDefinition("Stat Bonuses", "Increase Base Attack Speed"), false, new ConfigDescription("Should the attack speed increase the base attack speed instead? (Vanilla behaviour = false)", null, Array.Empty<object>()));
			AttackBonus = Config.Bind<float>(new ConfigDefinition("Stat Bonuses", "Attack Speed Bonus"), 0.3f, new ConfigDescription("Attack Speed bonus from Warbanners.", null, Array.Empty<object>()));
			MoveBonus = Config.Bind<float>(new ConfigDefinition("Stat Bonuses", "Move Speed Bonus"), 0.3f, new ConfigDescription("Movement Speed bonus from Warbanners.", null, Array.Empty<object>()));
			DamageBonus = Config.Bind<float>(new ConfigDefinition("Stat Bonuses", "Damage Bonus"), 2.0f, new ConfigDescription("Damage bonus from Warbanners. (Scales with level)", null, Array.Empty<object>()));
			CritBonus = Config.Bind<float>(new ConfigDefinition("Stat Bonuses", "Crit Bonus"), 10.0f, new ConfigDescription("Crit bonus from Warbanners.", null, Array.Empty<object>()));
			ArmorBonus = Config.Bind<float>(new ConfigDefinition("Stat Bonuses", "Armor Bonus"), 0.0f, new ConfigDescription("Armor bonus from Warbanners.", null, Array.Empty<object>()));
			RegenBonus = Config.Bind<float>(new ConfigDefinition("Stat Bonuses", "Regen Bonus"), 3.0f, new ConfigDescription("Regen bonus from Warbanners. (Scales with level)", null, Array.Empty<object>()));

			BossBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Mithrix Phase Banners"), 1.0f, new ConfigDescription("Players equipped with Warbanners will place one down at the start of Mithrix's phases. (Except the item steal phase.) (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			PillarBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Moon Pillar Banners"), 0.75f, new ConfigDescription("Players equipped with Warbanners will place one down at the start of a Moon Pillar event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			DeepVoidBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Deep Void Signal Banners"), 0.75f, new ConfigDescription("Players equipped with Warbanners will place one down at the start of a Deep Void Signal event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			VoidBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Void Cell Banners"), 0.5f, new ConfigDescription("Players equipped with Warbanners will place one down at the start of a Void Cell event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			FocusBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Focus Banners"), 1f, new ConfigDescription("Players equipped with Warbanners will place one down when activating the Focus in Simulacrum. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
		}
	}
}
