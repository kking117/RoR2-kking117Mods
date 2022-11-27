using System;
using BepInEx;
using BepInEx.Configuration;
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
		public const string MODVERSION = "1.1.7";

		public const string MODTOKEN = "KKING117_ZOEAREWORK_";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<bool> Config_Rework_Enable;
		public static ConfigEntry<string> Config_Rework_CorruptList;
		public static ConfigEntry<string> Config_Rework_CorruptText;
		public static ConfigEntry<int> Config_Rework_DamageStack;
		public static ConfigEntry<int> Config_Rework_DamageBase;
		public static ConfigEntry<int> Config_Rework_HealthStack;
		public static ConfigEntry<int> Config_Rework_HealthBase;
		public static ConfigEntry<float> Config_Rework_SpawnTime;

		public static ConfigEntry<bool> Config_ReworkInherit_Enable;
		public static ConfigEntry<string> Config_ReworkInherit_ItemBlackList;
		public static ConfigEntry<string> Config_ReworkInherit_TierBlackList;

		public static ConfigEntry<string> Config_Buff_CorruptList;
		public static ConfigEntry<string> Config_Buff_CorruptText;
		public static ConfigEntry<int> Config_Buff_DamageStack;
		public static ConfigEntry<int> Config_Buff_DamageBase;
		public static ConfigEntry<int> Config_Buff_HealthStack;
		public static ConfigEntry<int> Config_Buff_HealthBase;
		public static ConfigEntry<float> Config_Buff_SpawnTime;

		public static ConfigEntry<float> Config_VoidMegaCrab_Rework_BaseHealth;
		public static ConfigEntry<float> Config_VoidMegaCrab_Rework_BaseDamage;

		public static ConfigEntry<float> Config_VoidMegaCrab_BaseSpeed;
		public static ConfigEntry<float> Config_VoidMegaCrab_RecallCooldown;
		public static ConfigEntry<bool> Config_VoidMegaCrab_EnableDisplays;

		public static ConfigEntry<float> Config_VoidJailer_BaseSpeed;
		public static ConfigEntry<float> Config_VoidJailer_RecallCooldown;

		public static ConfigEntry<float> Config_Nullifier_BaseSpeed;
		public static ConfigEntry<float> Config_Nullifier_RecallCooldown;

		public static ConfigEntry<float> Config_AIShared_MinRecallDist;
		public static ConfigEntry<float> Config_AIShared_MaxRecallDist;
		public static ConfigEntry<float> Config_AIShared_RecallDistDiff;
		internal static SceneDef BazaarSceneDef;
		private void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();

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
		private void PostLoad()
		{
			Changes.VoidMegaCrabAlly.PostLoad();
			BazaarSceneDef = SceneCatalog.FindSceneDef("bazaar");
		}
		private void ReadConfig()
		{
			Config_Rework_Enable = Config.Bind<bool>(new ConfigDefinition("Zoea Rework", "Enable"), true, new ConfigDescription("Enables this rework instead of buffing the item.", null, Array.Empty<object>()));
			Config_Rework_CorruptList = Config.Bind<string>(new ConfigDefinition("Zoea Rework", "Corrupt List"), "BeetleGland NovaOnLowHealth Knurl SiphonOnLowHealth BleedOnHitAndExplode SprintWisp RoboBallBuddy FireballsOnHit LightningStrikeOnHit ParentEgg TitanGoldDuringTP MinorConstructOnKill LaserEye GoldenKnurl BigSword", new ConfigDescription("List of items that will be corrupted by this item.", null, Array.Empty<object>()));
			Config_Rework_CorruptText = Config.Bind<string>(new ConfigDefinition("Zoea Rework", "Corrupt Text"), "<style=cIsVoid>Corrupts all </style><style=cIsTierBoss>boss items</style>.", new ConfigDescription("Controls the \"Corrupts all Xs\" text", null, Array.Empty<object>()));

			Config_Rework_SpawnTime = Config.Bind<float>(new ConfigDefinition("Zoea Rework", "Respawn Time"), 30f, new ConfigDescription("Cooldown time between summons in seconds.", null, Array.Empty<object>()));
			Config_Rework_DamageBase = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Base Damage Bonus"), 0, new ConfigDescription("How many extra BoostDamage items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Rework_DamageStack = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Stack Damage Bonus"), 2, new ConfigDescription("How many extra BoostDamage items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));
			Config_Rework_HealthBase = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Base Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Rework_HealthStack = Config.Bind<int>(new ConfigDefinition("Zoea Rework", "Stack Health Bonus"), 1, new ConfigDescription("How many extra BoostHP items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));
			
			Config_ReworkInherit_Enable = Config.Bind<bool>(new ConfigDefinition("Rework Inheritance", "Allow Item Inhertiance"), true, new ConfigDescription("Should the Rework's Void Devastator inherit the user's items?", null, Array.Empty<object>()));
			Config_ReworkInherit_ItemBlackList = Config.Bind<string>(new ConfigDefinition("Rework Inheritance", "Item Blacklist"), "LunarPrimaryReplacement LunarSecondaryReplacement LunarUtilityReplacement LunarSpecialReplacement", new ConfigDescription("Blacklist these item from the Void Devastator. (Note: It cannot use Heresy Skills.)", null, Array.Empty<object>()));
			Config_ReworkInherit_TierBlackList = Config.Bind<string>(new ConfigDefinition("Rework Inheritance", "Tier Blacklist"), "", new ConfigDescription("Blacklist all items in this tier from the Void Devastator. (Tier1Def, Tier2Def, Tier3Def, BossTierDef, LunarTierDef, VoidTier1Def, VoidTier2Def, VoidTier3Def, VoidBossDef)", null, Array.Empty<object>()));

			Config_Buff_CorruptList = Config.Bind<string>(new ConfigDefinition("Zoea Buff", "Corrupt List"), "BeetleGland", new ConfigDescription("List of items that will be corrupted by this item.", null, Array.Empty<object>()));
			Config_Buff_CorruptText = Config.Bind<string>(new ConfigDefinition("Zoea Buff", "Corrupt Text"), "<style=cIsVoid>Corrupts all </style><style=cIsTierBoss>Queen's Glands</style>.", new ConfigDescription("Controls the \"Corrupts all Xs\" text", null, Array.Empty<object>()));
			Config_Buff_SpawnTime = Config.Bind<float>(new ConfigDefinition("Zoea Buff", "Respawn Time"), 30f, new ConfigDescription("Cooldown time between summons in seconds.", null, Array.Empty<object>()));
			Config_Buff_DamageBase = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Base Damage Bonus"), 0, new ConfigDescription("How many extra BoostDamage items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Buff_DamageStack = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Stack Damage Bonus"), 5, new ConfigDescription("How many extra BoostDamage items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));
			Config_Buff_HealthBase = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Base Health Bonus"), 0, new ConfigDescription("How many extra BoostHP items the summons get at 1 stack. (1 = +10%)", null, Array.Empty<object>()));
			Config_Buff_HealthStack = Config.Bind<int>(new ConfigDefinition("Zoea Buff", "Stack Health Bonus"), 2, new ConfigDescription("How many extra BoostHP items the summons get from additional stacks. (1 = +10%)", null, Array.Empty<object>()));

			Config_VoidMegaCrab_Rework_BaseDamage = Config.Bind<float>(new ConfigDefinition("Rework VoidMegaCrabAlly", "Base Damage"), 0.5f, new ConfigDescription("Base Damage multiplier for the Void Devastator.", null, Array.Empty<object>()));
			Config_VoidMegaCrab_Rework_BaseHealth = Config.Bind<float>(new ConfigDefinition("Rework VoidMegaCrabAlly", "Base Health"), 0.5f, new ConfigDescription("Base Health multiplier for the Void Devastator.", null, Array.Empty<object>()));

			Config_VoidMegaCrab_BaseSpeed = Config.Bind<float>(new ConfigDefinition("VoidMegaCrabAlly", "Base Movement Speed"), 8f, new ConfigDescription("Base movement speed of the Void Devastator. (8 = Vanilla)", null, Array.Empty<object>()));
			Config_VoidMegaCrab_RecallCooldown = Config.Bind<float>(new ConfigDefinition("VoidMegaCrabAlly", "Recall Cooldown"), 25f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));
			Config_VoidMegaCrab_EnableDisplays = Config.Bind<bool>(new ConfigDefinition("VoidMegaCrabAlly", "Enable Item Displays"), true, new ConfigDescription("Should the Void Devastator have custom item displays?", null, Array.Empty<object>()));

			Config_VoidJailer_BaseSpeed = Config.Bind<float>(new ConfigDefinition("VoidJailerAlly", "Base Movement Speed"), 7f, new ConfigDescription("Base movement speed of the Void Jailer. (7 = Vanilla)", null, Array.Empty<object>()));
			Config_VoidJailer_RecallCooldown = Config.Bind<float>(new ConfigDefinition("VoidJailerAlly", "Recall Cooldown"), 25f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));

			Config_Nullifier_BaseSpeed = Config.Bind<float>(new ConfigDefinition("NullifierAlly", "Base Movement Speed"), 7f, new ConfigDescription("Base movement speed of the Void Reaver. (6 = Vanilla)", null, Array.Empty<object>()));
			Config_Nullifier_RecallCooldown = Config.Bind<float>(new ConfigDefinition("NullifierAlly", "Recall Cooldown"), 20f, new ConfigDescription("Cooldown time of its recall ability.", null, Array.Empty<object>()));

			Config_AIShared_MinRecallDist = Config.Bind<float>(new ConfigDefinition("AI Shared", "Base Recall Distance"), 130f, new ConfigDescription("Minimum distance in metres before the AI will recall itself.", null, Array.Empty<object>()));
			Config_AIShared_MaxRecallDist = Config.Bind<float>(new ConfigDefinition("AI Shared", "Max Recall Distance"), 300f, new ConfigDescription("Max distance cap for recalling.", null, Array.Empty<object>()));
			Config_AIShared_RecallDistDiff = Config.Bind<float>(new ConfigDefinition("AI Shared", "Recall Distance Scaler"), 4f, new ConfigDescription("Scales the minimum recall distance with the current run difficulty by this much.", null, Array.Empty<object>()));
		}
	}
}
