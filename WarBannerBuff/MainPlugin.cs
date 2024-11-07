using System;
using BepInEx;
using BepInEx.Bootstrap;
using RoR2;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace WarBannerBuff
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
	public class MainPlugin : BaseUnityPlugin
    {
		public const string MODUID = "com.kking117.WarBannerBuff";
		public const string MODNAME = "WarBannerBuff";
		public const string MODTOKEN = "KKING117_WARBANNERBUFF_";
		public const string MODVERSION = "5.3.1";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		public static PluginInfo pluginInfo;

		internal static bool Starstorm_Loaded = false;

		public static float RecoveryTick;

		public static float HealBase;
		public static float HealLevel;
		public static float HealMin;

		public static float RechargeBase;
		public static float RechargeLevel;
		public static float RechargeMin;

		public static float RegenBonus;
		public static float DamageBonus;
		public static float CritBonus;
		public static float ArmorBonus;
		public static float AttackBonus;
		public static float MoveBonus;

		public static float VoidBanner;
		public static float PillarBanner;
		public static float DeepVoidBanner;
		public static float BossBanner;
		public static float FocusBanner;
		public static float MeridianBanner;
		public static float HalcyonBanner;

		public static bool Merge_Enable;
		public static float Merge_MinOverlap;
		public static float Merge_FuseMult;

		public static bool SS2_ShareWithGreaterBanner;
		public void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;

			ReadConfig();
			new ItemChanges.WarBanner();
			new Modules.ContentPacks().Initialize();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
		}
		private void PostLoad()
		{
			LoadCompat();
		}
		private void LoadCompat()
		{
			if (SS2_ShareWithGreaterBanner)
            {
				Starstorm_Loaded = Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm");
				if (Starstorm_Loaded)
				{
					BuffIndex greaterBanner = BuffCatalog.FindBuffIndex("BuffGreaterBanner");
					if (greaterBanner > BuffIndex.None)
					{
						ItemChanges.WarBanner.GreaterBannerBuff = BuffCatalog.GetBuffDef(greaterBanner);
					}
					if (ItemChanges.WarBanner.GreaterBannerBuff != RoR2Content.Buffs.Warbanner)
                    {
						ModLogger.LogInfo("Found SS2 Greater Banner Buff");
					}
					else
                    {
						ModLogger.LogInfo("Couldn't find SS2 Greater Banner Buff");
					}
				}
			}
		}
		public void ReadConfig()
		{
			RecoveryTick = Config.Bind("Healing and Recharge", "Tick Interval", 0.0f, "Delay in seconds between each heal/recharge tick. (0 or less disables this feature)").Value;

			HealBase = Config.Bind("Healing and Recharge", "Heal Max Health", 0.0025f, "Heal this % amount of health based on their total health. (0.01 = 1%)").Value;
			HealLevel = Config.Bind("Healing and Recharge", "Heal Level Health", 0.25f, "Heal this flat amount of health per level.").Value;
			HealMin = Config.Bind("Healing and Recharge", "Minimum Heal", 1.0f, "The minimum amount of healing to gain.").Value;

			RechargeBase = Config.Bind("Healing and Recharge", "Recharge Max Shield", 0.0f, "Recharge this % amount of shield based on their total shield. (0.01 = 1%)").Value;
			RechargeLevel = Config.Bind("Healing and Recharge", "Recharge Level Shield", 0.0f, "Recharge this flat amount of shield per level.").Value;
			RechargeMin = Config.Bind("Healing and Recharge", "Minimum Recharge", 0.0f, "The minimum amount of recharge to gain.").Value;

			AttackBonus = Config.Bind("Stat Bonuses", "Attack Speed Bonus", 0.3f, "Attack Speed bonus from Warbanners.").Value;
			MoveBonus = Config.Bind("Stat Bonuses", "Move Speed Bonus", 0.3f, "Movement Speed bonus from Warbanners.").Value;
			DamageBonus = Config.Bind("Stat Bonuses", "Damage Bonus", 0.2f, "Damage bonus from Warbanners.").Value;
			CritBonus = Config.Bind("Stat Bonuses", "Crit Bonus", 0.0f, "Crit bonus from Warbanners.").Value;
			ArmorBonus = Config.Bind("Stat Bonuses", "Armor Bonus", 0.0f, "Armor bonus from Warbanners.").Value;
			RegenBonus = Config.Bind("Stat Bonuses", "Regen Bonus", 3.0f, "Regen bonus from Warbanners. (Scales with level)").Value;

			BossBanner = Config.Bind("Placement Events", "Mithrix Phase Banners", 1.0f, "Players equipped with Warbanners will place one down at the start of Mithrix's phases. (Except the item steal phase.) (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;
			PillarBanner = Config.Bind("Placement Events", "Moon Pillar Banners", 0.75f, "Players equipped with Warbanners will place one down at the start of a Moon Pillar event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;
			DeepVoidBanner = Config.Bind("Placement Events", "Deep Void Signal Banners", 0.75f, "Players equipped with Warbanners will place one down at the start of a Deep Void Signal event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;
			VoidBanner = Config.Bind("Placement Events", "Void Cell Banners", 0.5f, "Players equipped with Warbanners will place one down at the start of a Void Cell event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;
			FocusBanner = Config.Bind("Placement Events", "Focus Banners", 1f, "Players equipped with Warbanners will place one down when activating the Focus in Simulacrum. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;

			MeridianBanner = Config.Bind("Placement Events", "Prime Meridian Banners", 1f, "Players equipped with Warbanners will place one down at the start of False Son's phases. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;
			HalcyonBanner = Config.Bind("Placement Events", "Halcyon Shrine Banners", 0f, "Players equipped with Warbanners will place one down when activating the Halcyon Shrine. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)").Value;

			Merge_Enable = Config.Bind("Banner Merging", "Enable", true, "Allow banner merging? (May cause stutters when banners are placed.)").Value;
			Merge_MinOverlap = Config.Bind("Banner Merging", "Minimum Overlap", 0.25f, "Minimum amount of overlap that banners need to merge. Values closer to 0 makes the requirement less strict. (Accepts 0-1)").Value;
			Merge_FuseMult = Config.Bind("Banner Merging", "Merge Multiplier", 0.5f, "How much radius to take from the smaller banners when merging.").Value;

			SS2_ShareWithGreaterBanner = Config.Bind("Starstorm 2", "Count Greater Warbanner", false, "Makes the buff granted by the Greater Warbanner also count as the Warbanner buff.").Value;
		}
	}
}
