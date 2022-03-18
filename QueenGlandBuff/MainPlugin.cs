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
//using Moonstorm;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace QueenGlandBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	//[BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.SoftDependency)]
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
		public const string MODVERSION = "1.3.0";

		public const string MODTOKEN = "KKING117_QUEENGLANDBUFF_";

		public static bool MoonstormSharedUtils;

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static List<EquipmentDef> StageEliteEquipmentDefs = new List<EquipmentDef>();

		public static EquipmentIndex Gland_DefaultAffix_Var;

		//Stuff
		public static ConfigEntry<bool> Gland_Debug;
		public static ConfigEntry<int> Gland_SpawnAffix;
		public static ConfigEntry<string> Gland_DefaultAffix;
		//Gland Stats
		public static ConfigEntry<int> Gland_MaxSummons;
		public static ConfigEntry<int> Gland_BaseDamage;
		public static ConfigEntry<int> Gland_BaseHealth;
		public static ConfigEntry<int> Gland_StackDamage;
		public static ConfigEntry<int> Gland_StackHealth;
		public static ConfigEntry<float> Gland_RespawnTime;
		public static ConfigEntry<float> Gland_Regen;
		//Staunch Buff
		public static ConfigEntry<float> Gland_Staunch_AggroRange;
		public static ConfigEntry<float> Gland_Staunch_AggroChance;
		public static ConfigEntry<float> Gland_Staunch_AggroBossChance;
		public static ConfigEntry<float> Gland_Staunch_BuffRange;
		//Leash
		public static ConfigEntry<float> Gland_AI_LeashLength;
		public static ConfigEntry<bool> Gland_AI_Target;
		//Ally Skill
		public static ConfigEntry<bool> Gland_PrimaryBuff;
		public static ConfigEntry<bool> Gland_SecondaryBuff;
		public static ConfigEntry<bool> Gland_AddUtility;
		public static ConfigEntry<bool> Gland_AddSpecial;

		public static GameObject Default_Proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/hermitcrabbombprojectile");
		public static GameObject Perfect_Sunder_MainProj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/brothersunderwave");
		public static GameObject Perfect_Sunder_SecProj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/lunarshardprojectile");
		public static GameObject Perfect_Slam_Proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/brotherfirepillar");

		public static float FixedTimer;
		public void Awake()
		{
			ModLogger = this.Logger;

			Logger.LogInfo("Reading Config.");
			ReadConfig();
			//GetModCompat();

			if (Gland_SpawnAffix.Value != 0)
			{
				On.RoR2.Stage.BeginServer += Stage_BeginServer;
			}
			ItemChanges.QueensGland.Begin();
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Initializing ContentPack.");
			}
			new Modules.ContentPacks().Initialize();
		}
		private void GetModCompat()
        {
			//MoonstormSharedUtils = Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.MoonstormSharedUtils");
		}
		private void Stage_BeginServer(On.RoR2.Stage.orig_BeginServer orig, Stage self)
		{
			orig(self);
			UpdateEliteList();
		}
		private void FixedUpdate()
        {
			if (FixedTimer < 0.0f)
			{
				FixedTimer += 1f;
			}
			FixedTimer -= Time.fixedDeltaTime;
		}
		private void UpdateEliteList()
		{
			if(!NetworkServer.active)
            {
				return;
            }
			Gland_DefaultAffix_Var = EquipmentCatalog.FindEquipmentIndex(Gland_DefaultAffix.Value);
			if(Gland_DefaultAffix_Var != EquipmentIndex.None)
            {
				if(Run.instance.IsEquipmentExpansionLocked(Gland_DefaultAffix_Var))
                {
					Gland_DefaultAffix_Var = EquipmentIndex.None;
				}
            }
			StageEliteEquipmentDefs.Clear();
			CombatDirector.EliteTierDef[] DirectorElite = EliteAPI.GetCombatDirectorEliteTiers();
			bool IsMoon = Stage.instance.sceneDef.cachedName.Contains("moon");
			bool IsHonor = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly);
			bool IsLoop = Run.instance.loopClearCount > 0;

			SpawnCard.EliteRules eliterules = SpawnCard.EliteRules.Default;
			if(IsHonor)
            {
				eliterules = SpawnCard.EliteRules.ArtifactOnly;
			}
			if(IsMoon)
            {
				eliterules = SpawnCard.EliteRules.Lunar;
			}
			
			for (int i = 0; i < DirectorElite.Length; i++)
            {
				if(DirectorElite[i].isAvailable.Invoke(eliterules))
                {
					for (int z = 0; z < DirectorElite[i].eliteTypes.GetLength(0); z++)
					{
						if (DirectorElite[i].eliteTypes[z])
						{
							StageEliteEquipmentDefs.Add(DirectorElite[i].eliteTypes[z].eliteEquipmentDef);
						}
					}
				}
            }
			/*if (MoonstormSharedUtils)
			{
				AddMoonstormElites(IsLoop);
			}*/
		}
		/*private void AddMoonstormElites(bool IsLoop)
        {
			for (int i = 0; i < EliteModuleBase.MoonstormElites.Count; i++)
			{
				EliteTiers tier = EliteModuleBase.MoonstormElites[i].eliteTier;
				if (IsLoop && tier == EliteTiers.PostLoop)
				{
					StageEliteEquipmentDefs.Add(EliteModuleBase.MoonstormElites[i].eliteEquipmentDef);
				}
				else if (tier == EliteTiers.Basic)
				{
					StageEliteEquipmentDefs.Add(EliteModuleBase.MoonstormElites[i].eliteEquipmentDef);
				}
			}
		}*/
		public void ReadConfig()
		{
			Gland_Debug = Config.Bind<bool>(new ConfigDefinition("Misc", "Debug"), false, new ConfigDescription("Enables debug messages.", null, Array.Empty<object>()));

			Gland_SpawnAffix = Config.Bind<int>(new ConfigDefinition("Elite Buff", "Become Elite"), 1, new ConfigDescription("Makes Beetle Guard Ally spawn with an Elite Affix. (0 = never, 1 = always, 2 = only during honor)", null, Array.Empty<object>()));
			Gland_DefaultAffix = Config.Bind<string>(new ConfigDefinition("Elite Buff", "Default Elite"), "EliteFireEquipment", new ConfigDescription("The Fallback equipment to give if an Elite Affix wasn't selected. (Set to None to disable)", null, Array.Empty<object>()));

			Gland_MaxSummons = Config.Bind<int>(new ConfigDefinition("Gland Stats", "Max Summons"), 3, new ConfigDescription("The Max amount of Beetle Guards each player can have.)", null, Array.Empty<object>()));
			Gland_RespawnTime = Config.Bind<float>(new ConfigDefinition("Gland Stats", "Respawn Time"), 20, new ConfigDescription("How long it takes for Beetle Guards to respawn.)", null, Array.Empty<object>()));
			Gland_BaseHealth = Config.Bind<int>(new ConfigDefinition("Gland Stats", "BaseHealth"), 10, new ConfigDescription("How many BoostHP items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Gland_BaseDamage = Config.Bind<int>(new ConfigDefinition("Gland Stats", "BaseDamage"), 20, new ConfigDescription("How many DamageBonus items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Gland_StackHealth = Config.Bind<int>(new ConfigDefinition("Gland Stats", "StackHealth"), 10, new ConfigDescription("How many extra BoostHP to give when stacking above Max Summons.", null, Array.Empty<object>()));
			Gland_StackDamage = Config.Bind<int>(new ConfigDefinition("Gland Stats", "StackDamage"), 15, new ConfigDescription("How many extra DamageBonus to give when stacking above Max Summons.", null, Array.Empty<object>()));
			Gland_Regen = Config.Bind<float>(new ConfigDefinition("Gland Stats", "Regen"), 2, new ConfigDescription("Out of combat regen for Beetle Guards. (scales with max health)", null, Array.Empty<object>()));

			Gland_PrimaryBuff = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Slam Buff"), true, new ConfigDescription("Enables the modified slam attack.", null, Array.Empty<object>()));
			Gland_SecondaryBuff = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Sunder Buff"), true, new ConfigDescription("Enables the modified sunder attack.", null, Array.Empty<object>()));
			Gland_AddUtility = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Enable Utility Skill"), true, new ConfigDescription("Enables the Recall skill, makes the user teleport back to their owner. (AI will use this if they've gone beyond the Leash Length)", null, Array.Empty<object>()));
			Gland_AddSpecial = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Enable Special Skill"), true, new ConfigDescription("Enables the Staunch skill, buffs nearby friendly beetles and makes enemies target the user.", null, Array.Empty<object>()));

			Gland_Staunch_AggroRange = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Range"), 100f, new ConfigDescription("Enemies within this range will aggro to affected characters.", null, Array.Empty<object>()));
			Gland_Staunch_AggroChance = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Normal Chance"), 1f, new ConfigDescription("Percent chance every second that an enemy will aggro onto affected characters.", null, Array.Empty<object>()));
			Gland_Staunch_AggroBossChance = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Boss Chance"), 0.025f, new ConfigDescription("Percent chance every second that a boss will aggro onto affected characters.", null, Array.Empty<object>()));
			Gland_Staunch_BuffRange = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Buff Range"), 100f, new ConfigDescription("Allies with beetle in their name will be given a buff from affected characters.", null, Array.Empty<object>()));

			Gland_AI_LeashLength = Config.Bind<float>(new ConfigDefinition("AI Changes", "Leash Length"), 125f, new ConfigDescription("How far away Beetle Guards can be before following their owner is a priority.", null, Array.Empty<object>()));
			Gland_AI_Target = Config.Bind<bool>(new ConfigDefinition("AI Changes", "Targeting Changes"), true, new ConfigDescription("Enables increased priority to grounded enemies. (Disable if you feel this is unneeded or suspect it causes performance issues.)", null, Array.Empty<object>()));
		}
	}
}
