using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine.AddressableAssets;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ZoeaRework
{
	[BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.ZoeaRework";
		public const string MODNAME = "ZoeaRework";
		public const string MODVERSION = "1.2.0";

		public const string MODTOKEN = "KKING117_ZOEAREWORK_";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static bool Config_Rework_Enable;
		public static string Config_Rework_CorruptList;
		public static string Config_Rework_CorruptText;
		public static int Config_Rework_DamageStack;
		public static int Config_Rework_DamageBase;
		public static int Config_Rework_HealthStack;
		public static int Config_Rework_HealthBase;
		public static float Config_Rework_SpawnTime;

		public static bool Config_ReworkInherit_Enable;
		public static string Config_ReworkInherit_ItemBlackList;
		public static string Config_ReworkInherit_TierBlackList;

		public static string Config_Buff_CorruptList;
		public static string Config_Buff_CorruptText;
		public static int Config_Buff_DamageStack;
		public static int Config_Buff_DamageBase;
		public static int Config_Buff_HealthStack;
		public static int Config_Buff_HealthBase;
		public static float Config_Buff_SpawnTime;

		public static float Config_VoidMegaCrab_Rework_BaseHealth;
		public static float Config_VoidMegaCrab_Rework_LevelHealth;
		public static float Config_VoidMegaCrab_Rework_BaseDamage;
		public static float Config_VoidMegaCrab_Rework_LevelDamage;

		public static float Config_VoidMegaCrab_BaseSpeed;
		public static float Config_VoidMegaCrab_RecallCooldown;
		public static bool Config_VoidMegaCrab_EnableDisplays;

		public static float Config_VoidJailer_BaseSpeed;
		public static float Config_VoidJailer_RecallCooldown;

		public static float Config_Nullifier_BaseSpeed;
		public static float Config_Nullifier_RecallCooldown;

		public static float Config_AIShared_MinRecallDist;
		public static float Config_AIShared_MaxRecallDist;
		public static float Config_AIShared_RecallDistDiff;
		internal static List<SceneDef> NoSpawnScene = new List<SceneDef>();

		internal static string Section_ZoeaBuff = "Zoea Buff";
		internal static string Section_ZoeaRework = "Zoea Rework";
		internal static string Section_ReworkInherit = "Rework Inheritance";

		internal static string Section_VoidMegaCrabRework = "Rework Void Devastator";
		internal static string Section_VoidMegaCrab = "Void Devastator";
		internal static string Section_VoidJailer = "Void Jailer";
		internal static string Section_VoidNullifier = "Void Reaver";
		internal static string Section_AIShared = "AI Shared";
		private void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();

			if (Config_Rework_Enable)
			{
				Changes.VoidMegaCrabItem_Rework.Begin();
			}
			else
            {
				Changes.VoidMegaCrabItem_Buff.Begin();
			}
			ItemCatalog.availability.CallWhenAvailable(new Action(PostLoad_Items));

			//"RoR2/Base/bazaar/bazaar.asset"
			SceneDef newScene = Addressables.LoadAssetAsync<SceneDef>("4116724a9bd3d05499bac80e6297a949").WaitForCompletion();
			if (newScene != null)
			{
				NoSpawnScene.Add(newScene);
			}
			//"RoR2/DLC3/computationalexchange/computationalexchange.asset"
			newScene = Addressables.LoadAssetAsync<SceneDef>("55dcf3dbd137f99458af33ad467bb574").WaitForCompletion();
			if (newScene != null)
			{
				NoSpawnScene.Add(newScene);
			}

			Logger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		private void PostLoad_Items()
		{
			Changes.VoidMegaCrabAlly.PostLoad();
		}
		private void ReadConfig()
		{
			
			Config_Rework_Enable = Config.Bind(Section_ZoeaRework, "Enable", false, "Enables this rework instead of buffing the item.").Value;
			Config_Rework_CorruptList = Config.Bind(Section_ZoeaRework, "Corrupt List", "BeetleGland NovaOnLowHealth Knurl SiphonOnLowHealth BleedOnHitAndExplode SprintWisp RoboBallBuddy FireballsOnHit LightningStrikeOnHit ParentEgg TitanGoldDuringTP MinorConstructOnKill ExtraEquipment ShockDamageAura", "List of items that will be corrupted by this item.").Value;
			Config_Rework_CorruptText = Config.Bind(Section_ZoeaRework, "Corrupt Text", "<style=cIsVoid>Corrupts all </style><style=cIsTierBoss>boss items</style>.", "Controls the \"Corrupts all Xs\" text").Value;

			Config_Rework_SpawnTime = Config.Bind(Section_ZoeaRework, "Respawn Time", 30f, "Cooldown time between summons in seconds.").Value;
			Config_Rework_DamageBase = Config.Bind(Section_ZoeaRework, "Base Damage Bonus", 0, "How many extra BoostDamage items the summons get at a single stack. (1 = +10%)").Value;
			Config_Rework_DamageStack = Config.Bind(Section_ZoeaRework, "Stack Damage Bonus", 2, "How many extra BoostDamage items the summons get for each additional stack. (1 = +10%)").Value;
			Config_Rework_HealthBase = Config.Bind(Section_ZoeaRework, "Base Health Bonus", 0, "How many extra BoostHP items the summons get at a single stack. (1 = +10%)").Value;
			Config_Rework_HealthStack = Config.Bind(Section_ZoeaRework, "Stack Health Bonus", 1, "How many extra BoostHP items the summons get for each additional stack. (1 = +10%)").Value;
			
			Config_ReworkInherit_Enable = Config.Bind(Section_ReworkInherit, "Allow Item Inhertiance", true, "Should the Rework's Void Devastator inherit the user's items?").Value;
			Config_ReworkInherit_ItemBlackList = Config.Bind(Section_ReworkInherit, "Item Blacklist", "LunarPrimaryReplacement LunarSecondaryReplacement LunarUtilityReplacement LunarSpecialReplacement", "Blacklist these item from the Void Devastator. (Note: It cannot use Heresy Skills.)").Value;
			Config_ReworkInherit_TierBlackList = Config.Bind(Section_ReworkInherit, "Tier Blacklist", "", "Blacklist all items in this tier from the Void Devastator. (Tier1Def, Tier2Def, Tier3Def, BossTierDef, LunarTierDef, VoidTier1Def, VoidTier2Def, VoidTier3Def, VoidBossDef, FoodTier)").Value;

			Config_Buff_CorruptList = Config.Bind(Section_ZoeaBuff, "Corrupt List", "BeetleGland", "List of items that will be corrupted by this item.").Value;
			Config_Buff_CorruptText = Config.Bind(Section_ZoeaBuff, "Corrupt Text", "<style=cIsVoid>Corrupts all </style><style=cIsTierBoss>Queen's Glands</style>.", "Controls the \"Corrupts all Xs\" text").Value;

			Config_Buff_SpawnTime = Config.Bind(Section_ZoeaBuff, "Respawn Time", 30f, "Cooldown time between summons in seconds.").Value;
			Config_Buff_DamageBase = Config.Bind(Section_ZoeaBuff, "Base Damage Bonus", 0, "How many extra BoostDamage items the summons get at a single stack. (1 = +10%)").Value;
			Config_Buff_DamageStack = Config.Bind(Section_ZoeaBuff, "Stack Damage Bonus", 5, "How many extra BoostDamage items the summons get for each additional stack. (1 = +10%)").Value;
			Config_Buff_HealthBase = Config.Bind(Section_ZoeaBuff, "Base Health Bonus", 0, "How many extra BoostHP items the summons get at a single stack. (1 = +10%)").Value;
			Config_Buff_HealthStack = Config.Bind(Section_ZoeaBuff, "Stack Health Bonus", 2, "How many extra BoostHP items the summons get for each additional stack. (1 = +10%)").Value;

			Config_VoidMegaCrab_Rework_BaseDamage = Config.Bind(Section_VoidMegaCrabRework, "Base Damage", 12f, "Base Damage of the Void Devastator.").Value;
			Config_VoidMegaCrab_Rework_LevelDamage = Config.Bind(Section_VoidMegaCrabRework, "Stack Damage", 2.4f, "Level Damage of the Void Devastator.").Value;
			Config_VoidMegaCrab_Rework_BaseHealth = Config.Bind(Section_VoidMegaCrabRework, "Base Health", 1400f, "Base Health of the Void Devastator.").Value;
			Config_VoidMegaCrab_Rework_LevelHealth = Config.Bind(Section_VoidMegaCrabRework, "Stack Damage", 420f, "Level Health of the Void Devastator.").Value;

			Config_VoidMegaCrab_BaseSpeed = Config.Bind(Section_VoidMegaCrab, "Base Movement Speed", 8f, "Base movement speed of the Void Devastator. (8 = Vanilla)").Value;
			Config_VoidMegaCrab_RecallCooldown = Config.Bind(Section_VoidMegaCrab, "Recall Cooldown", 25f, "Cooldown time of its recall ability.").Value;
			Config_VoidMegaCrab_EnableDisplays = Config.Bind(Section_VoidMegaCrab, "Enable Item Displays", true, "Should the Void Devastator have extra custom item displays?").Value;

			Config_VoidJailer_BaseSpeed = Config.Bind(Section_VoidJailer, "Base Movement Speed", 8f, "Base movement speed of the Void Jailer. (7 = Vanilla)").Value;
			Config_VoidJailer_RecallCooldown = Config.Bind(Section_VoidJailer, "Recall Cooldown", 25f, "Cooldown time of its recall ability.").Value;

			Config_Nullifier_BaseSpeed = Config.Bind(Section_VoidNullifier, "Base Movement Speed", 8f, "Base movement speed of the Void Reaver. (6 = Vanilla)").Value;
			Config_Nullifier_RecallCooldown = Config.Bind(Section_VoidNullifier, "Recall Cooldown", 25f, "Cooldown time of its recall ability.").Value;

			Config_AIShared_MinRecallDist = Config.Bind(Section_AIShared, "Base Recall Distance", 130f, "Minimum distance in metres before the AI will recall itself.").Value;
			Config_AIShared_MaxRecallDist = Config.Bind(Section_AIShared, "Max Recall Distance", 300f, "Max distance cap for recalling.").Value;
			Config_AIShared_RecallDistDiff = Config.Bind(Section_AIShared, "Recall Distance Scaler", 4f, "Scales the minimum recall distance with the current run difficulty by this much.").Value;
		}
	}
}
