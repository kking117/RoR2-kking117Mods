using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace QueenGlandBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
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
		public const string MODUID = "com.kking117.QueenGlandBuff";
		public const string MODNAME = "QueenGlandBuff";
		public const string MODVERSION = "1.3.6";

		public const string MODTOKEN = "KKING117_QUEENGLANDBUFF_";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		//Stuff
		public static ConfigEntry<bool> Config_Debug;
		
		//Beetle Guard Ally Stats
		public static ConfigEntry<float> Config_BeetleGuardAlly_HealthMult;
		public static ConfigEntry<float> Config_BeetleGuardAlly_DamageMult;
		public static ConfigEntry<float> Config_BeetleGuardAlly_RegenMult;
		public static ConfigEntry<int> Config_QueensGland_SpawnAffix;
		public static ConfigEntry<string> Config_QueensGland_DefaultAffix;
		//Gland Stats
		public static ConfigEntry<bool> Config_QueensGland_Enable;
		public static ConfigEntry<int> Config_QueensGland_MaxSummons;
		public static ConfigEntry<float> Config_QueensGland_RespawnTime;
		public static ConfigEntry<int> Config_BaseDamage;
		public static ConfigEntry<int> Config_BaseHealth;
		public static ConfigEntry<int> Config_StackDamage;
		public static ConfigEntry<int> Config_StackHealth;
		//Staunch Buff
		public static ConfigEntry<float> Config_Staunch_AggroRange;
		public static ConfigEntry<float> Config_Staunch_AggroChance;
		public static ConfigEntry<float> Config_Staunch_AggroBossChance;
		public static ConfigEntry<float> Config_Staunch_BuffRange;
		//Leash
		public static ConfigEntry<bool> Config_AI_Target;
		public static ConfigEntry<float> Config_AI_MinRecallDist;
		public static ConfigEntry<float> Config_AI_MaxRecallDist;
		public static ConfigEntry<float> Config_AI_RecallDistDiff;
		//Ally Skill
		public static ConfigEntry<bool> Config_PrimaryBuff;
		public static ConfigEntry<bool> Config_SecondaryBuff;
		public static ConfigEntry<bool> Config_AddUtility;
		public static ConfigEntry<bool> Config_AddSpecial;

		internal static SceneDef BazaarSceneDef;
		internal static float FixedTimer;
		private void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();
			Changes.BeetleGuardAlly.Begin();
			if (Config_QueensGland_Enable.Value)
			{
				Changes.QueensGland.Begin();
			}
			if (Config_Debug.Value)
			{
				Logger.LogInfo("Initializing ContentPack.");
			}
			new Modules.ContentPacks().Initialize();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
		}
		private void PostLoad()
		{
			BazaarSceneDef = SceneCatalog.FindSceneDef("bazaar");
		}
		private void FixedUpdate()
        {
			if (FixedTimer < 0.0f)
			{
				FixedTimer += 1f;
            }
			FixedTimer -= Time.fixedDeltaTime;
		}
		private void ReadConfig()
		{
			Config_Debug = Config.Bind<bool>(new ConfigDefinition("Misc", "Debug"), false, new ConfigDescription("Enables debug messages.", null, Array.Empty<object>()));

			Config_QueensGland_Enable = Config.Bind<bool>(new ConfigDefinition("Queens Gland", "Enable Changes"), true, new ConfigDescription("Enable changes to the Queens Gland item. (Suggest setting to False if using RiskyMod's changes.)", null, Array.Empty<object>()));
			Config_QueensGland_MaxSummons = Config.Bind<int>(new ConfigDefinition("Queens Gland", "Max Summons"), 3, new ConfigDescription("The Max amount of Beetle Guards each player can have.", null, Array.Empty<object>()));
			Config_QueensGland_RespawnTime = Config.Bind<float>(new ConfigDefinition("Queens Gland", "Respawn Time"), 20, new ConfigDescription("How long it takes for Beetle Guards to respawn.", null, Array.Empty<object>()));
			Config_BaseHealth = Config.Bind<int>(new ConfigDefinition("Queens Gland", "BaseHealth"), 10, new ConfigDescription("How many BoostHP items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Config_StackHealth = Config.Bind<int>(new ConfigDefinition("Queens Gland", "StackHealth"), 10, new ConfigDescription("How many extra BoostHP to give when stacking above Max Summons.", null, Array.Empty<object>()));
			Config_BaseDamage = Config.Bind<int>(new ConfigDefinition("Queens Gland", "BaseDamage"), 20, new ConfigDescription("How many DamageBonus items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Config_StackDamage = Config.Bind<int>(new ConfigDefinition("Queens Gland", "StackDamage"), 20, new ConfigDescription("How many extra DamageBonus to give when stacking above Max Summons.", null, Array.Empty<object>()));
			Config_QueensGland_SpawnAffix = Config.Bind<int>(new ConfigDefinition("Queens Gland", "Become Elite"), 1, new ConfigDescription("Makes Beetle Guard Ally spawn with an Elite Affix. (0 = never, 1 = always, 2 = only during honor)", null, Array.Empty<object>()));
			Config_QueensGland_DefaultAffix = Config.Bind<string>(new ConfigDefinition("Queens Gland", "Default Elite"), "EliteFireEquipment", new ConfigDescription("The Fallback equipment to give if an Elite Affix wasn't selected. (Set to None to disable)", null, Array.Empty<object>()));
			//Haven't renamed to Config_QueensGland_BaseDamage as RiskyMod references these values and it was updated like a day before this.

			Config_PrimaryBuff = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Slam Buff"), true, new ConfigDescription("Enables the modified slam attack.", null, Array.Empty<object>()));
			Config_SecondaryBuff = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Sunder Buff"), true, new ConfigDescription("Enables the modified sunder attack.", null, Array.Empty<object>()));
			Config_AddUtility = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Enable Utility Skill"), true, new ConfigDescription("Enables the Recall skill, makes the user teleport back to their owner. (AI will use this if they've gone beyond the Leash Length)", null, Array.Empty<object>()));
			Config_AddSpecial = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Enable Special Skill"), true, new ConfigDescription("Enables the Staunch skill, buffs nearby friendly beetles and makes enemies target the user.", null, Array.Empty<object>()));

			Config_Staunch_AggroRange = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Range"), 100f, new ConfigDescription("Enemies within this range will aggro to affected characters.", null, Array.Empty<object>()));
			Config_Staunch_AggroChance = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Normal Chance"), 1f, new ConfigDescription("Percent chance every second that an enemy will aggro onto affected characters.", null, Array.Empty<object>()));
			Config_Staunch_AggroBossChance = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Boss Chance"), 0.025f, new ConfigDescription("Percent chance every second that a boss will aggro onto affected characters.", null, Array.Empty<object>()));

			Config_AI_MinRecallDist = Config.Bind<float>(new ConfigDefinition("Recall Skill", "Base Recall Distance"), 125f, new ConfigDescription("Minimum distance in metres before the Beetle Guard will start following their owner.", null, Array.Empty<object>()));
			Config_AI_MaxRecallDist = Config.Bind<float>(new ConfigDefinition("Recall Skill", "Max Recall Distance"), 300f, new ConfigDescription("Max distance cap for following.", null, Array.Empty<object>()));
			Config_AI_RecallDistDiff = Config.Bind<float>(new ConfigDefinition("Recall Skill", "Recall Distance Scaler"), 4f, new ConfigDescription("Scales the minimum follow distance with the current run difficulty by this much.", null, Array.Empty<object>()));

			Config_BeetleGuardAlly_HealthMult = Config.Bind<float>(new ConfigDefinition("Beetle Guard Ally Base Stats", "Health"), 1f, new ConfigDescription("Health multiplier.", null, Array.Empty<object>()));
			Config_BeetleGuardAlly_DamageMult = Config.Bind<float>(new ConfigDefinition("Beetle Guard Ally Base Stats", "Damage"), 1f, new ConfigDescription("Damage multiplier.", null, Array.Empty<object>()));
			Config_BeetleGuardAlly_RegenMult = Config.Bind<float>(new ConfigDefinition("Beetle Guard Ally Base Stats", "Regen"), 3f, new ConfigDescription("Regen multiplier.", null, Array.Empty<object>()));
		}
	}
}
