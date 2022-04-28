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
		public const string MODVERSION = "1.1.0";

		public const string MODTOKEN = "KKING117_ZOEAREWORK_";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<bool> Config_Shared_OnlyGlands;

		public static ConfigEntry<bool> Config_Rework_Enable;
		public static ConfigEntry<int> Config_Rework_DamageStack;
		public static ConfigEntry<int> Config_Rework_DamageBase;
		public static ConfigEntry<int> Config_Rework_HealthStack;
		public static ConfigEntry<int> Config_Rework_HealthBase;
		public static ConfigEntry<float> Config_Rework_SpawnTime;
		public static ConfigEntry<bool> Config_Rework_Inherit;

		public static ConfigEntry<int> Config_Buff_DamageStack;
		public static ConfigEntry<int> Config_Buff_DamageBase;
		public static ConfigEntry<int> Config_Buff_HealthStack;
		public static ConfigEntry<int> Config_Buff_HealthBase;
		public static ConfigEntry<float> Config_Buff_SpawnTime;

		public static ConfigEntry<float> Config_VoidMegaCrab_BaseSpeed;
		public static ConfigEntry<float> Config_VoidMegaCrab_RecallCooldown;
		public static ConfigEntry<bool> Config_VoidMegaCrab_EnableDisplays;

		public static ConfigEntry<float> Config_VoidJailer_BaseSpeed;
		public static ConfigEntry<float> Config_VoidJailer_RecallCooldown;

		public static ConfigEntry<float> Config_Nullifier_BaseSpeed;
		public static ConfigEntry<float> Config_Nullifier_RecallCooldown;
		public void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();

			LanguageAPI.Add(MODTOKEN + "UTILITY_TELEPORT_NAME", "Recall");
			LanguageAPI.Add(MODTOKEN + "UTILITY_TELEPORT_DESC", "Return to your owner.");

			if (MainPlugin.Config_Rework_Enable.Value)
			{
				Changes.VoidMegaCrabItem_Rework.Begin();
			}
			else
            {
				Changes.VoidMegaCrabItem_Buff.Begin();
			}
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));

			Logger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		public void PostLoad()
		{
			if (!MainPlugin.Config_Rework_Enable.Value)
			{
				Changes.VoidMegaCrabAlly.PostLoad();
			}
		}

		public void ReadConfig()
		{
			Config_Shared_OnlyGlands = Config.Bind<bool>(new ConfigDefinition("Zoea Shared", "Corrupt Queens Glands Only"), true, new ConfigDescription("Should this item only corrupt Queens Glands?", null, Array.Empty<object>()));

			Config_Rework_Enable = Config.Bind<bool>(new ConfigDefinition("Zoea Rework", "Enable"), true, new ConfigDescription("Enables this rework instead of buffing the item.", null, Array.Empty<object>()));
			Config_Rework_Inherit = Config.Bind<bool>(new ConfigDefinition("Zoea Rework", "Inherit Items"), true, new ConfigDescription("Should the Void Devastator from this item inherit the user's items?", null, Array.Empty<object>()));
			Config_Rework_SpawnTime = Config.Bind<float>(new ConfigDefinition("Zoea Rework", "Respawn Time"), 30f, new ConfigDescription("Cooldown time between summons in seconds.", null, Array.Empty<object>()));
			Config_Rework_DamageBase = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Base Damage Bonus"), 0, new ConfigDescription("How many extra BoostDamage items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Rework_DamageStack = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Stack Damage Bonus"), 4, new ConfigDescription("How many extra BoostDamage items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));
			Config_Rework_HealthBase = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Base Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Rework_HealthStack = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Stack Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));

			Config_Buff_SpawnTime = Config.Bind<float>(new ConfigDefinition("Zoea Buff", "Respawn Time"), 30f, new ConfigDescription("Cooldown time between summons in seconds.", null, Array.Empty<object>()));
			Config_Buff_DamageBase = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Base Damage Bonus"), 0, new ConfigDescription("How many extra BoostDamage items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Buff_DamageStack = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Stack Damage Bonus"), 5, new ConfigDescription("How many extra BoostDamage items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));
			Config_Buff_HealthBase = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Base Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Buff_HealthStack = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Stack Health Bonus"), 2, new ConfigDescription("How many extra BoostHP items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));

			Config_VoidMegaCrab_BaseSpeed = Config.Bind<float>(new ConfigDefinition("VoidMegaCrabAlly", "Base Movement Speed"), 8f, new ConfigDescription("Base movement speed of the Void Devastator. (8 = Vanilla)", null, Array.Empty<object>()));
			Config_VoidMegaCrab_RecallCooldown = Config.Bind<float>(new ConfigDefinition("VoidMegaCrabAlly", "Recall Cooldown"), 25f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));
			Config_VoidMegaCrab_EnableDisplays = Config.Bind<bool>(new ConfigDefinition("VoidMegaCrabAlly", "Enable Item Displays"), true, new ConfigDescription("Should the Void Devastator have custom item displays?", null, Array.Empty<object>()));

			Config_VoidJailer_BaseSpeed = Config.Bind<float>(new ConfigDefinition("VoidJailerAlly", "Base Movement Speed"), 7f, new ConfigDescription("Base movement speed of the Void Jailer. (7 = Vanilla)", null, Array.Empty<object>()));
			Config_VoidJailer_RecallCooldown = Config.Bind<float>(new ConfigDefinition("VoidJailerAlly", "Recall Cooldown"), 25f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));

			Config_Nullifier_BaseSpeed = Config.Bind<float>(new ConfigDefinition("NullifierAlly", "Base Movement Speed"), 6f, new ConfigDescription("Base movement speed of the Void Jailer. (6 = Vanilla)", null, Array.Empty<object>()));
			Config_Nullifier_RecallCooldown = Config.Bind<float>(new ConfigDefinition("NullifierAlly", "Recall Cooldown"), 25f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));
		}
	}
}
