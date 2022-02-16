using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using Moonstorm;
using QueenGlandBuff.Utils;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace QueenGlandBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.Chen.ChensGradiusMod", BepInDependency.DependencyFlags.SoftDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI",
		"PrefabAPI",
		"LoadoutAPI"
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.QueenGlandBuff";
		public const string MODNAME = "QueenGlandBuff";
		public const string MODVERSION = "1.1.0";

		public const string MODTOKEN = "KKING117_QUEENGLANDBUFF_";

		public static bool MoonstormSharedUtils;

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static List<EquipmentDef> StageEliteEquipmentDefs = new List<EquipmentDef>();

		public static EquipmentIndex Gland_DefaultAffix_Var;

		private int T1_EliteList = 1;
		private int AH_EliteList = 2;
		private int T2_EliteList = 3;
		private int TM_EliteList = 4;

		//Stuff
		public static ConfigEntry<bool> Gland_Debug;
		public static ConfigEntry<bool> Gland_SpawnAffix;
		public static ConfigEntry<string> Gland_DefaultAffix;
		//Gland Stats
		public static ConfigEntry<int> Gland_MaxSummons;
		public static ConfigEntry<int> Gland_BaseDamage;
		public static ConfigEntry<int> Gland_BaseHealth;
		public static ConfigEntry<int> Gland_StackDamage;
		public static ConfigEntry<int> Gland_StackHealth;
		public static ConfigEntry<float> Gland_RespawnTime;
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

		public static GameObject Default_Proj = Resources.Load<GameObject>("prefabs/projectiles/hermitcrabbombprojectile");
		public static GameObject Perfect_Sunder_MainProj = Resources.Load<GameObject>("prefabs/projectiles/brothersunderwave");
		public static GameObject Perfect_Sunder_SecProj = Resources.Load<GameObject>("prefabs/projectiles/lunarshardprojectile");
		public static GameObject Perfect_Slam_Proj = Resources.Load<GameObject>("prefabs/projectiles/brotherfirepillar");

		public static SkillDef OldSlamSkill = Resources.Load<SkillDef>("skilldefs/beetleguardbody/beetleguardbodygroundslam");
		public static SkillDef OldSunderSkill = Resources.Load<SkillDef>("skilldefs/beetleguardbody/beetleguardbodysunder");

		private float timer;
		public void Awake()
		{
			ModLogger = this.Logger;
			
			ReadConfig();
			GetModCompat();

			On.RoR2.Stage.BeginServer += Stage_BeginServer;
			if (Gland_AddSpecial.Value)
			{
				On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
				On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
				RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
			}
			On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Registering States.");
			}
			Modules.States.RegisterStates();
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Registering Buffs.");
			}
			Modules.Buffs.RegisterBuffs();
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Registering Projectiles.");
			}
			Modules.Projectiles.RegisterProjectiles();
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Registering Skills.");
			}
			Modules.Skills.RegisterSkills();

			ItemChanges.QueensGland.Begin();

			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Initializing ContentPack.");
			}
			new Modules.ContentPacks().Initialize();
		}
		private void GetModCompat()
        {
			MoonstormSharedUtils = Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.MoonstormSharedUtils");
		}
		private void Stage_BeginServer(On.RoR2.Stage.orig_BeginServer orig, Stage self)
		{
			orig(self);
			UpdateEliteList();
		}
		private void FixedUpdate()
        {
			if (timer < 0.0f)
			{
				timer += 1f;
			}
			timer -= Time.fixedDeltaTime;
		}
		private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
		{
			orig(self);
			if (timer < 0)
			{
				if (self.HasBuff(Modules.Buffs.Staunching))
				{
					Helpers.DrawAggro(self);
					Helpers.EmpowerBeetles(self);
				}
			}
		}
		private void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
		{
			orig(self, buff);
			if (buff == Modules.Buffs.Staunching)
			{
				if (NetworkServer.active)
				{
					Helpers.DrawAggro(self);
					Helpers.EmpowerBeetles(self);
				}
			}
		}
		private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			if (NetworkServer.active)
			{
				if (self.master)
				{
					MinionOwnership ownership = self.master.minionOwnership;
					if (ownership)
					{
						CharacterMaster owner = ownership.ownerMaster;
						if (owner)
						{
							if (Helpers.DoesMasterHaveDeployable(owner, DeployableSlot.BeetleGuardAlly, self.master))
							{
								int stackbonus = Math.Max(0, owner.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - Gland_MaxSummons.Value);
								int dmgitem = Gland_BaseDamage.Value + (Gland_StackDamage.Value * stackbonus);
								int hpitem = Gland_BaseHealth.Value + (Gland_StackHealth.Value * stackbonus);
								self.inventory.GiveItem(RoR2Content.Items.BoostDamage, dmgitem - self.inventory.GetItemCount(RoR2Content.Items.BoostDamage));
								self.inventory.GiveItem(RoR2Content.Items.BoostHp, hpitem - self.inventory.GetItemCount(RoR2Content.Items.BoostHp));
							}
						}
					}
				}
			}
			orig(self);
			if (self)
			{
				if (self.bodyIndex == BodyCatalog.FindBodyIndex("BeetleGuardAllyBody"))
				{
					if (self.skillLocator.primary)
					{
						if (self.skillLocator.primary.isCombatSkill)
						{
							self.skillLocator.primary.cooldownScale /= self.attackSpeed;
						}
					}
					if (self.skillLocator.secondary)
					{
						if (self.skillLocator.secondary.isCombatSkill)
						{
							self.skillLocator.secondary.cooldownScale /= self.attackSpeed;
						}
					}
					if (self.skillLocator.utility)
					{
						if (self.skillLocator.utility.isCombatSkill)
						{
							self.skillLocator.utility.cooldownScale /= self.attackSpeed;
						}
					}
					if (self.skillLocator.special)
					{
						if (self.skillLocator.special.isCombatSkill)
						{
							self.skillLocator.special.cooldownScale /= self.attackSpeed;
						}
					}
				}
			}
		}
		private void CalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			float levelbonus = sender.level - 1f;
			if (sender.HasBuff(Modules.Buffs.Staunching))
			{
				args.armorAdd += 100f;
			}
			if (sender.HasBuff(Modules.Buffs.BeetleFrenzy))
			{
				args.baseAttackSpeedAdd += 0.5f;
				args.moveSpeedMultAdd += 0.5f;
				args.baseRegenAdd += 3f + (levelbonus * 0.6f);
				args.baseDamageAdd += (sender.baseDamage + (levelbonus * sender.levelDamage)) * 0.25f;
			}
		}
		private void UpdateEliteList()
		{
			Gland_DefaultAffix_Var = EquipmentCatalog.FindEquipmentIndex(Gland_DefaultAffix.Value);
			StageEliteEquipmentDefs.Clear();
			CombatDirector.EliteTierDef[] DirectorElite = EliteAPI.GetCombatDirectorEliteTiers();
			bool IsMoon = Stage.instance.sceneDef.cachedName.Contains("moon");
			bool IsHonor = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly);
			bool IsLoop = Run.instance.loopClearCount > 0;
			if (IsHonor || !IsMoon)
			{
				int T1List = T1_EliteList;
				if (IsHonor)
                {
					T1List = AH_EliteList;
				}
				for (int i = 0; i < DirectorElite[T1List].eliteTypes.GetLength(0); i++)
				{
					if (DirectorElite[T1List].eliteTypes[i])
					{
						StageEliteEquipmentDefs.Add(DirectorElite[T1List].eliteTypes[i].eliteEquipmentDef);
					}
				}
				if (IsLoop)
				{
					for (int i = 0; i < DirectorElite[T2_EliteList].eliteTypes.GetLength(0); i++)
					{
						if (DirectorElite[T2_EliteList].eliteTypes[i])
						{
							StageEliteEquipmentDefs.Add(DirectorElite[T2_EliteList].eliteTypes[i].eliteEquipmentDef);
						}
					}
				}
				if (MoonstormSharedUtils)
				{
					AddMoonstormElites(true);
				}
			}
			if (IsMoon)
			{
				for (int i = 0; i < DirectorElite[TM_EliteList].eliteTypes.GetLength(0); i++)
				{
					if (DirectorElite[TM_EliteList].eliteTypes[i])
					{
						StageEliteEquipmentDefs.Add(DirectorElite[TM_EliteList].eliteTypes[i].eliteEquipmentDef);
					}
				}
			}
		}
		private void AddMoonstormElites(bool IsLoop)
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
		}
		public void ReadConfig()
		{
			Gland_Debug = Config.Bind<bool>(new ConfigDefinition("Misc", "Debug"), false, new ConfigDescription("Enables debug messages.", null, Array.Empty<object>()));

			Gland_SpawnAffix = Config.Bind<bool>(new ConfigDefinition("Elite Buff", "Become Elite"), true, new ConfigDescription("Makes Beetle Guard Ally spawn with an Elite Affix.", null, Array.Empty<object>()));
			Gland_DefaultAffix = Config.Bind<string>(new ConfigDefinition("Elite Buff", "Default Elite"), RoR2.RoR2Content.Equipment.AffixRed.name, new ConfigDescription("The Fallback equipment to give if an Elite Affix wasn't selected. (Set to None to disable)", null, Array.Empty<object>()));

			Gland_MaxSummons = Config.Bind<int>(new ConfigDefinition("Gland Stats", "Max Summons"), 3, new ConfigDescription("The Max amount of Beetle Guards each player can have.)", null, Array.Empty<object>()));
			Gland_RespawnTime = Config.Bind<float>(new ConfigDefinition("Gland Stats", "Respawn Time"), 20, new ConfigDescription("How long it takes for Beetle Guards to respawn.)", null, Array.Empty<object>()));
			Gland_BaseHealth = Config.Bind<int>(new ConfigDefinition("Gland Stats", "BaseHealth"), 0, new ConfigDescription("How many BoostHP items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Gland_BaseDamage = Config.Bind<int>(new ConfigDefinition("Gland Stats", "BaseDamage"), 20, new ConfigDescription("How many DamageBonus items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Gland_StackHealth = Config.Bind<int>(new ConfigDefinition("Gland Stats", "StackHealth"), 5, new ConfigDescription("How many extra BoostHP to give when stacking above Max Summons.", null, Array.Empty<object>()));
			Gland_StackDamage = Config.Bind<int>(new ConfigDefinition("Gland Stats", "StackDamage"), 15, new ConfigDescription("How many extra DamageBonus to give when stacking above Max Summons.", null, Array.Empty<object>()));

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
