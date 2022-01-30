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
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI",
		"PrefabAPI",
		"LoadoutAPI"
	})]
	[BepInPlugin("com.kking117.QueenGlandBuff", "QueenGlandBuff", "1.0.0")]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class Main : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.QueenGlandBuff";
		public const string ModLangToken = "QUEENGLANDBUFF_";
		public bool MoonstormSharedUtils;

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
		public static ConfigEntry<float> Gland_StackDamage;
		public static ConfigEntry<float> Gland_StackHealth;
		//Staunch Buff
		public static ConfigEntry<float> Gland_Staunch_AggroRange;
		public static ConfigEntry<float> Gland_Staunch_AggroChance;
		public static ConfigEntry<float> Gland_Staunch_AggroBossChance;
		public static ConfigEntry<float> Gland_Staunch_BuffRange;
		//Leash
		public static ConfigEntry<float> Gland_AI_LeashLength;
		//Ally Skill
		public static ConfigEntry<bool> Gland_PrimaryBuff;
		public static ConfigEntry<bool> Gland_SecondaryBuff;
		public static ConfigEntry<bool> Gland_AddUtility;
		public static ConfigEntry<bool> Gland_AddSpecial;
		//Guard Skill
		public static ConfigEntry<bool> Gland_UpgradeEnemy;

		private static AISkillDriver[] BeetleGuardAlly_AISkillDriver;
		private static BaseAI BeetleGuardAlly_BaseAI;
		private string BeetleAIName;

		public static GameObject Default_Proj = Resources.Load<GameObject>("prefabs/projectiles/hermitcrabbombprojectile");
		public static GameObject Perfect_Sunder_MainProj = Resources.Load<GameObject>("prefabs/projectiles/brothersunderwave");
		public static GameObject Perfect_Sunder_SecProj = Resources.Load<GameObject>("prefabs/projectiles/lunarshardprojectile");
		public static GameObject Perfect_Slam_Proj = Resources.Load<GameObject>("prefabs/projectiles/brotherfirepillar");

		public static GameObject BeetleGuardBody = Resources.Load<GameObject>("prefabs/characterbodies/BeetleGuardBody");
		public static GameObject BeetleGuardMaster = Resources.Load<GameObject>("prefabs/charactermasters/BeetleGuardMaster");
		public static GameObject BeetleGuardAllyBody = Resources.Load<GameObject>("prefabs/characterbodies/BeetleGuardAllyBody");
		public static GameObject BeetleGuardAllyMaster = Resources.Load<GameObject>("prefabs/charactermasters/BeetleGuardAllyMaster");

		public static SkillDef OldSlamSkill = Resources.Load<SkillDef>("skilldefs/beetleguardbody/beetleguardbodygroundslam");
		public static SkillDef OldSunderSkill = Resources.Load<SkillDef>("skilldefs/beetleguardbody/beetleguardbodysunder");

		private float timer;
		public void Awake()
		{
			MoonstormSharedUtils = Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.MoonstormSharedUtils");
			ReadConfig();
			On.RoR2.Stage.BeginServer += Stage_BeginServer;
			On.RoR2.CharacterBody.UpdateBeetleGuardAllies += CharacterBody_UpdateBeetleGuardAllies;
			On.RoR2.CharacterAI.BaseAI.OnBodyStart += BaseAI_OnBodyStart;
			On.RoR2.CharacterAI.BaseAI.FixedUpdate += BaseAI_FixedUpdate;
			On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
			On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
			On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
			On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
			RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;

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

			Setup_BodyAttributes(BeetleGuardAllyBody);
			ChangeAllyAI(BeetleGuardAllyMaster);
			ChangeBeetleLoadouts();
			Update_ItemDescs();

			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Initializing ContentPack.");
			}
			new Modules.ContentPacks().Initialize();
		}
		public static void PrintMe(string text)
        {
			print(text);
        }
		private void Update_ItemDescs()
        {
			if (Gland_Debug.Value)
            {
				Logger.LogInfo("Changing descriptions.");
			}
			string pickup = "Recruit ";
			string desc = "<style=cIsUtility>Summon ";
			if (Gland_SpawnAffix.Value || Gland_DefaultAffix_Var != EquipmentIndex.None)
            {
				pickup += "an Elite Beetle Guard.";
				desc += "an Elite Beetle Guard</style>";
			}
			else
            {
				pickup += "a Beetle Guard.";
				desc += "a Beetle Guard</style>";
			}
			desc += " with <style=cIsHealing>" + (10 + Gland_BaseHealth.Value) * 10 + "% base health</style>";
			desc += " and <style=cIsDamage>" + (10 + Gland_BaseDamage.Value) * 10 + "% base damage</style>.";
			desc += " Can have up to <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> total Guards, up to <style=cIsUtility>" + Gland_MaxSummons.Value + "</style>. Further stacks give";
			desc += " <style=cStack>+" + Gland_StackHealth.Value * 100 + "%</style> <style=cIsHealing>health</style>";
			desc += " and <style=cStack>+" + Gland_StackDamage.Value * 100 + "%</style> <style=cIsDamage>damage</style>.";
			LanguageAPI.Add("ITEM_BEETLEGLAND_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BEETLEGLAND_DESC", desc);
		}
		private void Stage_BeginServer(On.RoR2.Stage.orig_BeginServer orig, Stage self)
		{
			orig(self);
			UpdateEliteList();
		}
		private void BaseAI_OnBodyStart(On.RoR2.CharacterAI.BaseAI.orig_OnBodyStart orig, BaseAI self, CharacterBody body)
		{
			orig(self, body);
			if (body.master.name == BeetleAIName)
            {
				self.skillDrivers = BeetleGuardAlly_AISkillDriver;
			}
		}
		private void BaseAI_FixedUpdate(On.RoR2.CharacterAI.BaseAI.orig_FixedUpdate orig, BaseAI self)
		{
			orig(self);
			if (self.gameObject.name == BeetleAIName)
			{
				if (self.body)
				{
					if (self.currentEnemy.gameObject)
					{
						CharacterBody targetbody = self.currentEnemy.gameObject.GetComponent<CharacterBody>();
						if (targetbody)
						{
							if (targetbody.isFlying && !targetbody.isBoss)
							{
								if (self.enemyAttention <= 0f)
								{
									if (Gland_Debug.Value)
									{
										Logger.LogInfo("Redirecting focus from flying target.");
									}
									BullseyeSearch search = new BullseyeSearch();
									search.viewer = self.body;
									search.teamMaskFilter = TeamMask.allButNeutral;
									search.teamMaskFilter.RemoveTeam(self.master.teamIndex);
									search.sortMode = BullseyeSearch.SortMode.Distance;
									search.maxDistanceFilter = Gland_AI_LeashLength.Value;
									search.searchOrigin = self.bodyInputBank.aimOrigin;
									search.searchDirection = self.bodyInputBank.aimDirection;
									search.maxAngleFilter = (self.fullVision ? 180f : 90f);
									search.filterByLoS = true;
									search.RefreshCandidates();
									HurtBox result = null;
									float flydistance = 0f;
									foreach (HurtBox target in search.GetResults())
									{
										if (target && target.healthComponent)
										{
											if (target.healthComponent.body.isFlying && !target.healthComponent.body.isBoss)
											{
												if (result == null)
												{
													result = target;
													flydistance = Vector3.Distance(self.body.transform.position, target.healthComponent.body.transform.position);
												}
											}
											else
											{
												if (result)
												{
													if (Vector3.Distance(self.body.transform.position, target.healthComponent.body.transform.position) < flydistance * 2)
													{
														result = target;
														break;
													}
												}
												else
												{
													result = target;
													break;
												}
											}
										}
									}
									if (result)
									{
										self.currentEnemy.gameObject = result.healthComponent.gameObject;
										self.currentEnemy.bestHurtBox = result;
									}
									self.enemyAttention = self.enemyAttentionDuration;
								}
							}
						}
					}
				}
			}
		}

		private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
		{
			var result = orig(self, slot);
			if (slot != DeployableSlot.BeetleGuardAlly)
            {
				return result;
            }
			int mult = 1;
			if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef))
			{
				mult = 2;
			}
			return Math.Min(Gland_MaxSummons.Value, self.inventory.GetItemCount(RoR2Content.Items.BeetleGland)) * mult;
		}
		private void CharacterBody_UpdateBeetleGuardAllies(On.RoR2.CharacterBody.orig_UpdateBeetleGuardAllies orig, CharacterBody self)
        {
			if (NetworkServer.active)
			{
				if(self.inventory && self.master)
                {
					int extraglands = Math.Max(0, self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - Gland_MaxSummons.Value);
					int deployableCount = self.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly);
					int maxdeployable = self.master.GetDeployableSameSlotLimit(DeployableSlot.BeetleGuardAlly);
					if (deployableCount < maxdeployable)
					{
						self.guardResummonCooldown -= Time.fixedDeltaTime;
						if (self.guardResummonCooldown <= 0f)
						{
							self.guardResummonCooldown = 2f;
							DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Approximate,
								minDistance = 3f,
								maxDistance = 40f,
								spawnOnTarget = self.transform,
							}, RoR2Application.rng);
							directorSpawnRequest.summonerBodyObject = self.gameObject;
							directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
							{
								SetupSummonedBeetleGuard(spawnResult, self.master, extraglands);
							}));
							DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
						}
					}
					else if (deployableCount > maxdeployable)
                    {
						Helpers.RemoveDeployableBeetles(self.master, deployableCount - maxdeployable);
                    }
				}
			}
		}
		private void SetupSummonedBeetleGuard(SpawnCard.SpawnResult spawnResult, CharacterMaster summoner, int itemcount)
        {
			GameObject spawnedInstance = spawnResult.spawnedInstance;
			if (!spawnedInstance)
			{
				return;
			}
			CharacterMaster beeble = spawnedInstance.GetComponent<CharacterMaster>();
			if (beeble)
			{
				beeble.inventory.GiveItem(RoR2Content.Items.BoostDamage, Gland_BaseDamage.Value);
				beeble.inventory.GiveItem(RoR2Content.Items.BoostHp, Gland_BaseHealth.Value);
				Helpers.GiveRandomEliteAffix(beeble);
				Deployable deployable = beeble.GetComponent<Deployable>();
				if (deployable)
				{
					deployable.onUndeploy.AddListener(new UnityEngine.Events.UnityAction(beeble.TrueKill));
					summoner.AddDeployable(deployable, DeployableSlot.BeetleGuardAlly);
					if (summoner.GetBody())
                    {
						summoner.GetBody().guardResummonCooldown = 30f;
					}
				}
			}
		}
		private void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, RoR2.Run self)
		{
			orig(self);
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
			orig(self);
			if (self)
			{
				int buffCount = self.GetBuffCount(Modules.Buffs.BeetleFrenzy);
				if (self.bodyIndex == BodyCatalog.FindBodyIndex("BeetleGuardAllyBody"))
				{
					if (self.skillLocator.primary)
					{
						if (self.skillLocator.primary.isCombatSkill)
						{
							self.skillLocator.primary.cooldownScale /= self.attackSpeed;
						}
						self.skillLocator.primary.cooldownScale /= 1f + (0.3f * buffCount);
					}
					if (self.skillLocator.secondary)
					{
						if (self.skillLocator.secondary.isCombatSkill)
						{
							self.skillLocator.secondary.cooldownScale /= self.attackSpeed;
						}
						self.skillLocator.secondary.cooldownScale /= 1f + (0.3f * buffCount);
					}
					if (self.skillLocator.utility)
					{
						if (self.skillLocator.utility.isCombatSkill)
						{
							self.skillLocator.utility.cooldownScale /= self.attackSpeed;
						}
						self.skillLocator.utility.cooldownScale /= 1f + (0.3f * buffCount);
					}
					if (self.skillLocator.special)
					{
						if (self.skillLocator.special.isCombatSkill)
						{
							self.skillLocator.special.cooldownScale /= self.attackSpeed;
						}
						self.skillLocator.special.cooldownScale /= 1f + (0.3f * buffCount);
					}
				}
				else if (buffCount > 0)
				{
					if (self.skillLocator.primary)
					{
						self.skillLocator.primary.cooldownScale /= 1f + (0.3f * buffCount);
					}
					if (self.skillLocator.secondary)
					{
						self.skillLocator.secondary.cooldownScale /= 1f + (0.3f * buffCount);
					}
					if (self.skillLocator.utility)
					{
						self.skillLocator.utility.cooldownScale /= 1f + (0.3f * buffCount);
					}
					if (self.skillLocator.special)
					{
						self.skillLocator.special.cooldownScale /= 1f + (0.3f * buffCount);
					}
				}
			}
		}
		private void CalculateStatsHook(RoR2.CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.HasBuff(Modules.Buffs.Staunching))
			{
				args.armorAdd += 100f;
			}
			int buffCount = sender.GetBuffCount(Modules.Buffs.BeetleFrenzy);
			if (buffCount > 0)
            {
				float speedbonus = 1 - 1 / (1.3f * buffCount);
				args.baseAttackSpeedAdd += 0.3f * buffCount;
				args.moveSpeedMultAdd += speedbonus;
				args.baseDamageAdd += sender.levelDamage * 0.25f * sender.level * buffCount;
				args.baseRegenAdd += 1f * sender.level * buffCount;
				args.armorAdd -= 10f * buffCount;
			}
			if (sender.master)
			{
				MinionOwnership ownership = sender.master.minionOwnership;
				if (ownership)
				{
					CharacterMaster owner = ownership.ownerMaster;
					if (owner)
					{
						if (Helpers.DoesMasterHaveDeployable(owner, DeployableSlot.BeetleGuardAlly, sender.master))
                        {
							int stackbonus = Math.Max(0, owner.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - Gland_MaxSummons.Value);
							if (stackbonus > 0)
							{
								args.baseDamageAdd += (sender.baseDamage + (sender.levelDamage * (sender.level - 1))) * (stackbonus * Gland_StackDamage.Value);
								args.baseHealthAdd += (sender.baseMaxHealth + (sender.levelMaxHealth * (sender.level - 1))) * (stackbonus * Gland_StackHealth.Value);
							}
						}
					}
				}
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
		private void ChangeBeetleLoadouts()
        {
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Adding new Skills to BeetleGuardAlly.");
			}
			Modules.Skills.WipeLoadout(BeetleGuardAllyBody);
			if (Gland_PrimaryBuff.Value)
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.SlamSkill, SkillSlot.Primary);
			}
			else
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, OldSlamSkill, SkillSlot.Primary);
			}
			if (Gland_SecondaryBuff.Value)
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.SunderSkill, SkillSlot.Secondary);
			}
			else
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, OldSunderSkill, SkillSlot.Secondary);
			}
			if (Gland_AddUtility.Value)
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.RecallSkill, SkillSlot.Utility);
			}
			if (Gland_AddSpecial.Value)
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.StaunchSkill, SkillSlot.Special);
			}

			if (Gland_UpgradeEnemy.Value)
			{
				if (Gland_Debug.Value)
				{
					Logger.LogInfo("Adding new Skills to BeetleGuard.");
				}
				Modules.Skills.WipeLoadout(BeetleGuardBody);
				Modules.Skills.AddSkillToSlot(BeetleGuardBody, Modules.Skills.SlamSkill, SkillSlot.Primary);
				Modules.Skills.AddSkillToSlot(BeetleGuardBody, Modules.Skills.SunderSkill, SkillSlot.Secondary);
			}
		}
		private void Setup_BodyAttributes (GameObject prefab)
        {
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Changing BeetleGuardAlly attributes.");
			}
			CharacterBody charBody = prefab.GetComponent<CharacterBody>();
			charBody.baseAcceleration *= 1.5f;
			charBody.baseJumpPower *= 1.3f;
			charBody.baseRegen = 0f;
			charBody.levelRegen = 0f;
			CharacterDirection charDir = prefab.GetComponent<CharacterDirection>();
			charDir.turnSpeed *= 2f;
		}
		
		private void ChangeAllyAI(GameObject prefab)
		{
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("Changing BeetleGuardAlly AI and SkillDrivers.");
			}
			BeetleGuardAlly_BaseAI = prefab.GetComponent<BaseAI>();

			BeetleAIName = BeetleGuardAlly_BaseAI.gameObject.name + "(Clone)";
			BeetleGuardAlly_BaseAI.neverRetaliateFriendlies = true;
			BeetleGuardAlly_BaseAI.fullVision = true;

			AISkillDriver[] oldai = BeetleGuardAllyMaster.GetComponents<AISkillDriver>();
			AISkillDriver[] newai = new AISkillDriver[10];

			foreach (AISkillDriver aiskillDriver in oldai)
			{
				if (aiskillDriver.customName == "FireSunder")
				{
					aiskillDriver.minDistance = 15f;
					aiskillDriver.resetCurrentEnemyOnNextDriverSelection = true;
					aiskillDriver.driverUpdateTimerOverride = 1f;
					aiskillDriver.movementType = AISkillDriver.MovementType.StrafeMovetarget;
					newai[2] = aiskillDriver;
				}
				if (aiskillDriver.customName == "Slam")
				{
					aiskillDriver.maxDistance = 10f;
					aiskillDriver.resetCurrentEnemyOnNextDriverSelection = true;
					newai[3] = aiskillDriver;
				}
				if (aiskillDriver.customName == "ReturnToOwnerLeash")
				{
					aiskillDriver.shouldSprint = true;
					aiskillDriver.minDistance = Gland_AI_LeashLength.Value;
					if (Gland_AddUtility.Value)
					{
						aiskillDriver.requireSkillReady = true;
						aiskillDriver.skillSlot = SkillSlot.Utility;
						aiskillDriver.resetCurrentEnemyOnNextDriverSelection = true;
					}
					newai[4] = aiskillDriver;
				}
				if (aiskillDriver.customName == "StrafeBecausePrimaryIsntReady")
				{
					newai[5] = aiskillDriver;
				}
				if (aiskillDriver.customName == "ChaseOffNodegraph")
				{
					newai[6] = aiskillDriver;
				}
				if (aiskillDriver.customName == "Chase")
				{
					newai[7] = aiskillDriver;
				}
				if (aiskillDriver.customName == "ReturnToLeaderDefault")
				{
					aiskillDriver.minDistance *= 1.25f;
					aiskillDriver.shouldSprint = true;
					newai[8] = aiskillDriver;
				}
				if (aiskillDriver.customName == "WaitNearLeaderDefault")
				{
					newai[9] = aiskillDriver;
				}
			}
			
			
			if (Gland_AddSpecial.Value)
			{
				AISkillDriver BuffAI = BeetleGuardAllyMaster.AddComponent<AISkillDriver>();
				newai[0] = BuffAI;

				BuffAI.customName = "EmpowerAllies";

				BuffAI.activationRequiresAimConfirmation = true;
				BuffAI.activationRequiresAimTargetLoS = false;
				BuffAI.activationRequiresTargetLoS = true;
				BuffAI.aimType = AISkillDriver.AimType.AtMoveTarget;
				BuffAI.buttonPressType = AISkillDriver.ButtonPressType.Hold;
				BuffAI.driverUpdateTimerOverride = 1f;
				BuffAI.ignoreNodeGraph = false;
				BuffAI.maxDistance = 40f;
				BuffAI.maxTargetHealthFraction = float.PositiveInfinity;
				BuffAI.maxUserHealthFraction = float.PositiveInfinity;
				BuffAI.minDistance = 0f;
				BuffAI.minTargetHealthFraction = float.NegativeInfinity;
				BuffAI.minUserHealthFraction = float.NegativeInfinity;
				BuffAI.moveInputScale = 1f;
				BuffAI.movementType = AISkillDriver.MovementType.StrafeMovetarget;
				BuffAI.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
				//BuffAI.nextHighPriorityOverride =;
				BuffAI.noRepeat = false;
				//BuffAI.requiredSkill =;
				BuffAI.requireEquipmentReady = false;
				BuffAI.requireSkillReady = true;
				BuffAI.resetCurrentEnemyOnNextDriverSelection = true;
				BuffAI.selectionRequiresAimTarget = false;
				BuffAI.selectionRequiresOnGround = false;
				BuffAI.selectionRequiresTargetLoS = true;
				BuffAI.shouldFireEquipment = false;
				BuffAI.shouldSprint = false;
				BuffAI.skillSlot = SkillSlot.Special;
			}

			AISkillDriver SunderClose = BeetleGuardAllyMaster.AddComponent<AISkillDriver>();
			newai[1] = SunderClose;

			SunderClose.customName = "CloseSunder";

			SunderClose.activationRequiresAimConfirmation = true;
			SunderClose.activationRequiresAimTargetLoS = false;
			SunderClose.activationRequiresTargetLoS = true;
			SunderClose.aimType = AISkillDriver.AimType.AtMoveTarget;
			SunderClose.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			SunderClose.driverUpdateTimerOverride = 2f;
			SunderClose.ignoreNodeGraph = false;
			SunderClose.maxDistance = 15f;
			SunderClose.maxTargetHealthFraction = float.PositiveInfinity;
			SunderClose.maxUserHealthFraction = float.PositiveInfinity;
			SunderClose.minDistance = 5f;
			SunderClose.minTargetHealthFraction = float.NegativeInfinity;
			SunderClose.minUserHealthFraction = float.NegativeInfinity;
			SunderClose.moveInputScale = 1f;
			SunderClose.movementType = AISkillDriver.MovementType.StrafeMovetarget;
			SunderClose.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			//SunderClose.nextHighPriorityOverride =;
			SunderClose.noRepeat = false;
			//SunderClose.requiredSkill =;
			SunderClose.requireEquipmentReady = false;
			SunderClose.requireSkillReady = true;
			SunderClose.resetCurrentEnemyOnNextDriverSelection = true;
			SunderClose.selectionRequiresAimTarget = false;
			SunderClose.selectionRequiresOnGround = false;
			SunderClose.selectionRequiresTargetLoS = true;
			SunderClose.shouldFireEquipment = false;
			SunderClose.shouldSprint = false;
			SunderClose.skillSlot = SkillSlot.Secondary;

			if (Gland_Debug.Value)
			{
				Logger.LogInfo("New AI has the following SkillDrivers:");
				Logger.LogInfo("-------------------------------------");
			}
			BeetleGuardAlly_AISkillDriver = new AISkillDriver[0];
			foreach (AISkillDriver aiskillDriver in newai)
			{
				if (aiskillDriver)
				{
					if (aiskillDriver.customName.Length > 0)
					{
						
						if (Gland_Debug.Value)
						{
							Logger.LogInfo(aiskillDriver.customName);
						}
						Array.Resize(ref BeetleGuardAlly_AISkillDriver, BeetleGuardAlly_AISkillDriver.Length + 1);
						BeetleGuardAlly_AISkillDriver[BeetleGuardAlly_AISkillDriver.Length - 1] = aiskillDriver;
					}
				}
			}
			if (Gland_Debug.Value)
			{
				Logger.LogInfo("-------------------------------------");
			}
		}
		
		public void ReadConfig()
		{
			Gland_Debug = Config.Bind<bool>(new ConfigDefinition("Misc", "Debug"), false, new ConfigDescription("Enables debug messages.", null, Array.Empty<object>()));

			Gland_SpawnAffix = Config.Bind<bool>(new ConfigDefinition("Elite Buff", "Become Elite"), true, new ConfigDescription("Makes Beetle Guard Ally spawn with an Elite Affix.", null, Array.Empty<object>()));
			Gland_DefaultAffix = Config.Bind<string>(new ConfigDefinition("Elite Buff", "Default Elite"), RoR2.RoR2Content.Equipment.AffixRed.name, new ConfigDescription("The Fallback equipment to give if an Elite Affix wasn't selected. (Set to None to disable)", null, Array.Empty<object>()));

			Gland_MaxSummons = Config.Bind<int>(new ConfigDefinition("Gland Stats", "Max Summons"), 3, new ConfigDescription("The Max amount of Beetle Guards each player can have.)", null, Array.Empty<object>()));
			Gland_BaseHealth = Config.Bind<int>(new ConfigDefinition("Gland Stats", "BaseHealth"), 0, new ConfigDescription("How many BoostHP items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Gland_BaseDamage = Config.Bind<int>(new ConfigDefinition("Gland Stats", "BaseDamage"), 20, new ConfigDescription("How many DamageBonus items the Beetle Guards spawn with. (1 = +10%)", null, Array.Empty<object>()));
			Gland_StackHealth = Config.Bind<float>(new ConfigDefinition("Gland Stats", "StackHealth"), 0.25f, new ConfigDescription("How much extra base health to give when stacking above Max Summons.", null, Array.Empty<object>()));
			Gland_StackDamage = Config.Bind<float>(new ConfigDefinition("Gland Stats", "StackDamage"), 0.5f, new ConfigDescription("How much extra base damage to give when stacking above Max Summons.", null, Array.Empty<object>()));

			Gland_PrimaryBuff = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Slam Buff"), true, new ConfigDescription("Enables the modified slam attack.", null, Array.Empty<object>()));
			Gland_SecondaryBuff = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Sunder Buff"), true, new ConfigDescription("Enables the modified sunder attack.", null, Array.Empty<object>()));
			Gland_AddUtility = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Enable Utility Skill"), true, new ConfigDescription("Enables the Recall skill, makes the user teleport back to their owner. (AI will use this if they've gone beyond the Leash Length)", null, Array.Empty<object>()));
			Gland_AddSpecial = Config.Bind<bool>(new ConfigDefinition("Skill Changes", "Enable Special Skill"), true, new ConfigDescription("Enables the Staunch skill, buffs nearby friendly beetles and makes enemies target the user.", null, Array.Empty<object>()));

			Gland_UpgradeEnemy = Config.Bind<bool>(new ConfigDefinition("Cursed", "Upgrade Beetle Guard"), false, new ConfigDescription("Gives the normal Beetle Guards the upgraded skills.", null, Array.Empty<object>()));

			Gland_Staunch_AggroRange = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Range"), 100f, new ConfigDescription("Enemies within this range will aggro to affected characters.", null, Array.Empty<object>()));
			Gland_Staunch_AggroChance = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Normal Chance"), 1f, new ConfigDescription("Percent chance every second that an enemy will aggro onto affected characters.", null, Array.Empty<object>()));
			Gland_Staunch_AggroBossChance = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Aggro Boss Chance"), 0.1f, new ConfigDescription("Percent chance every second that a boss will aggro onto affected characters.", null, Array.Empty<object>()));
			Gland_Staunch_BuffRange = Config.Bind<float>(new ConfigDefinition("Staunch Buff", "Buff Range"), 100f, new ConfigDescription("Allies with beetle in their name will be given a buff from affected characters.", null, Array.Empty<object>()));

			Gland_AI_LeashLength = Config.Bind<float>(new ConfigDefinition("AI Changes", "Leash Length"), 125f, new ConfigDescription("How far away Beetle Guards can be before following their owner is a priority.", null, Array.Empty<object>()));
		}
	}
}
