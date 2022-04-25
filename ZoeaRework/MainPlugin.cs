using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ZoeaRework
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.Wolfo.WolfoQualityOfLife", BepInDependency.DependencyFlags.SoftDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI",
		"PrefabAPI"
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.ZoeaRework";
		public const string MODNAME = "ZoeaRework";
		public const string MODVERSION = "1.0.0";

		public const string MODTOKEN = "KKING117_ZOEAREWORK_";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<int> Config_Zoea_DamageBonus;
		public static ConfigEntry<int> Config_Zoea_DamageBase;
		public static ConfigEntry<int> Config_Zoea_HealthBonus;
		public static ConfigEntry<int> Config_Zoea_HealthBase;
		public static ConfigEntry<float> Config_Zoea_SpawnTime;
		public static ConfigEntry<bool> Config_Zoea_OnlyGlands;
		public static ConfigEntry<bool> Config_Zoea_Inherit;

		public static ConfigEntry<float> Config_VoidMegaCrab_BaseSpeed;
		public static ConfigEntry<float> Config_VoidMegaCrab_RecallCooldown;
		public static ConfigEntry<bool> Config_VoidMegaCrab_EnableDisplays;
		public void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();

			Changes.VoidMegaCrabItem.Begin();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
			Logger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		public void PostLoad()
		{
			Changes.VoidMegaCrabAlly.PostLoad();
		}

		public void ReadConfig()
		{
			Config_Zoea_OnlyGlands = Config.Bind<bool>(new ConfigDefinition("VoidCrabMegaItem Changes", "Corrupt Queens Glands Only"), true, new ConfigDescription("Should this item only corrupt Queens Glands?", null, Array.Empty<object>()));
			Config_Zoea_Inherit = Config.Bind<bool>(new ConfigDefinition("VoidCrabMegaItem Changes", "Inherit Items"), true, new ConfigDescription("Should minions from this item inherit the user's items?", null, Array.Empty<object>()));

			Config_Zoea_SpawnTime = Config.Bind<float>(new ConfigDefinition("Void Minion Changes", "Respawn Time"), 30f, new ConfigDescription("Cooldown time between summons in seconds.", null, Array.Empty<object>()));

			Config_Zoea_DamageBase = Config.Bind<int>(new ConfigDefinition("Void Minion Changes", "Base Damage Bonus"), 0, new ConfigDescription("How many extra BoostDamage items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Zoea_DamageBonus = Config.Bind<int>(new ConfigDefinition("Void Minion Changes", "Stack Damage Bonus"), 10, new ConfigDescription("How many extra BoostDamage items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));

			Config_Zoea_HealthBase = Config.Bind<int>(new ConfigDefinition("Void Minion Changes", "Base Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Zoea_HealthBonus = Config.Bind<int>(new ConfigDefinition("Void Minion Changes", "Stack Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));

			Config_VoidMegaCrab_BaseSpeed = Config.Bind<float>(new ConfigDefinition("VoidMegaCrabAlly Changes", "Base Movement Speed"), 7f, new ConfigDescription("Base movement speed of the Void Devastator. (8 = Vanilla)", null, Array.Empty<object>()));
			Config_VoidMegaCrab_RecallCooldown = Config.Bind<float>(new ConfigDefinition("VoidMegaCrabAlly Changes", "Recall Cooldown"), 25f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));
			Config_VoidMegaCrab_EnableDisplays = Config.Bind<bool>(new ConfigDefinition("VoidMegaCrabAlly Changes", "Enable Item Displays"), true, new ConfigDescription("Should the Void Devastator have custom item displays?", null, Array.Empty<object>()));
		}
	}
}
