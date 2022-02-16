using System;
using System.Collections.Generic;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using QueenGlandBuff.Utils;

namespace QueenGlandBuff.ItemChanges
{
    public class QueensGland
    {
		private static GameObject BeetleGuardBody = Resources.Load<GameObject>("prefabs/characterbodies/BeetleGuardBody");
		private static GameObject BeetleGuardMaster = Resources.Load<GameObject>("prefabs/charactermasters/BeetleGuardMaster");
		private static GameObject BeetleGuardAllyBody = Resources.Load<GameObject>("prefabs/characterbodies/BeetleGuardAllyBody");
		private static GameObject BeetleGuardAllyMaster = Resources.Load<GameObject>("prefabs/charactermasters/BeetleGuardAllyMaster");

		private static BaseAI BeetleGuardAlly_BaseAI;

		public static void Begin()
        {
			UpdateBody(BeetleGuardAllyBody);
			UpdateLoadouts();
			UpdateAI(BeetleGuardAllyMaster);
			UpdateItemDescription();
			Hooks();
		}
		private static void UpdateItemDescription()
		{
			if (MainPlugin.Gland_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing descriptions.");
			}
			string pickup = "Recruit ";
			string desc = "<style=cIsUtility>Summon ";
			if (MainPlugin.Gland_SpawnAffix.Value || MainPlugin.Gland_DefaultAffix_Var != EquipmentIndex.None)
			{
				pickup += "an Elite Beetle Guard.";
				desc += "an Elite Beetle Guard</style>";
			}
			else
			{
				pickup += "a Beetle Guard.";
				desc += "a Beetle Guard</style>";
			}
			desc += " with <style=cIsHealing>" + (10 + MainPlugin.Gland_BaseHealth.Value) * 10 + "% base health</style>";
			desc += " and <style=cIsDamage>" + (10 + MainPlugin.Gland_BaseDamage.Value) * 10 + "% base damage</style>.";
			desc += " Can have up to <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> total Guards, up to <style=cIsUtility>" + MainPlugin.Gland_MaxSummons.Value + "</style>. Further stacks give";
			desc += " <style=cStack>+" + MainPlugin.Gland_StackHealth.Value * 10 + "%</style> <style=cIsHealing>base health</style>";
			desc += " and <style=cStack>+" + MainPlugin.Gland_StackDamage.Value * 10 + "%</style> <style=cIsDamage>base damage</style>.";
			LanguageAPI.Add("ITEM_BEETLEGLAND_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BEETLEGLAND_DESC", desc);
		}
		public static void UpdateBody(GameObject prefab)
		{
			if (MainPlugin.Gland_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing " + prefab.name + " attributes.");
			}
			CharacterBody charBody = prefab.GetComponent<CharacterBody>();
			charBody.baseAcceleration *= 1.5f;
			charBody.baseJumpPower *= 1.3f;
			charBody.baseRegen = 0f;
			charBody.levelRegen = 0f;
			CharacterDirection charDir = prefab.GetComponent<CharacterDirection>();
			charDir.turnSpeed *= 2f;
		}
		public static void UpdateLoadouts()
		{
			if (MainPlugin.Gland_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Adding new Skills to BeetleGuardAllyBody.");
			}
			Modules.Skills.WipeLoadout(BeetleGuardAllyBody);
			Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.SlamSkill, SkillSlot.Primary);
			Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.SunderSkill, SkillSlot.Secondary);
			if (MainPlugin.Gland_AddUtility.Value)
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.RecallSkill, SkillSlot.Utility);
			}
			if (MainPlugin.Gland_AddSpecial.Value)
			{
				Modules.Skills.AddSkillToSlot(BeetleGuardAllyBody, Modules.Skills.StaunchSkill, SkillSlot.Special);
			}
		}

		public static void UpdateAI(GameObject prefab)
		{
			if (MainPlugin.Gland_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing BeetleGuardAlly AI and SkillDrivers.");
			}
			BeetleGuardAlly_BaseAI = prefab.GetComponent<BaseAI>();

			BeetleGuardAlly_BaseAI.neverRetaliateFriendlies = true;
			BeetleGuardAlly_BaseAI.fullVision = true;

			foreach (AISkillDriver obj in prefab.GetComponentsInChildren<AISkillDriver>())
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}

			if (MainPlugin.Gland_AddSpecial.Value)
			{
				AISkillDriver aiskillDriver1 = prefab.AddComponent<AISkillDriver>();
				aiskillDriver1.customName = "BuffAlly";

				aiskillDriver1.activationRequiresAimConfirmation = true;
				aiskillDriver1.activationRequiresAimTargetLoS = false;
				aiskillDriver1.activationRequiresTargetLoS = true;
				aiskillDriver1.aimType = AISkillDriver.AimType.AtMoveTarget;
				aiskillDriver1.buttonPressType = AISkillDriver.ButtonPressType.Hold;
				aiskillDriver1.driverUpdateTimerOverride = 1f;
				aiskillDriver1.ignoreNodeGraph = false;
				aiskillDriver1.maxDistance = 40f;
				aiskillDriver1.minDistance = 0f;
				aiskillDriver1.maxTargetHealthFraction = float.PositiveInfinity;
				aiskillDriver1.maxUserHealthFraction = float.PositiveInfinity;
				aiskillDriver1.minTargetHealthFraction = float.NegativeInfinity;
				aiskillDriver1.minUserHealthFraction = float.NegativeInfinity;
				aiskillDriver1.moveInputScale = 1f;
				aiskillDriver1.movementType = AISkillDriver.MovementType.StrafeMovetarget;
				aiskillDriver1.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
				//aiskillDriver1.nextHighPriorityOverride =;
				aiskillDriver1.noRepeat = false;
				//aiskillDriver1.requiredSkill =;
				aiskillDriver1.requireEquipmentReady = false;
				aiskillDriver1.requireSkillReady = true;
				aiskillDriver1.resetCurrentEnemyOnNextDriverSelection = true;
				aiskillDriver1.selectionRequiresAimTarget = false;
				aiskillDriver1.selectionRequiresOnGround = false;
				aiskillDriver1.selectionRequiresTargetLoS = true;
				aiskillDriver1.shouldFireEquipment = false;
				aiskillDriver1.shouldSprint = false;
				aiskillDriver1.skillSlot = SkillSlot.Special;
			}

			AISkillDriver aiskillDriver2 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver2.customName = "FireSunder";
			aiskillDriver2.activationRequiresAimConfirmation = true;
			aiskillDriver2.activationRequiresAimTargetLoS = false;
			aiskillDriver2.activationRequiresTargetLoS = true;
			aiskillDriver2.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver2.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver2.driverUpdateTimerOverride = 1f;
			aiskillDriver2.ignoreNodeGraph = false;
			aiskillDriver2.maxDistance = 40f;
			aiskillDriver2.minDistance = 0f;
			aiskillDriver2.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver2.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver2.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver2.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver2.moveInputScale = 1f;
			aiskillDriver2.movementType = AISkillDriver.MovementType.StrafeMovetarget;
			aiskillDriver2.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			//aiskillDriver2.nextHighPriorityOverride =;
			aiskillDriver2.noRepeat = false;
			//aiskillDriver2.requiredSkill =;
			aiskillDriver2.requireEquipmentReady = false;
			aiskillDriver2.requireSkillReady = true;
			aiskillDriver2.resetCurrentEnemyOnNextDriverSelection = true;
			aiskillDriver2.selectionRequiresAimTarget = true;
			aiskillDriver2.selectionRequiresOnGround = false;
			aiskillDriver2.selectionRequiresTargetLoS = true;
			aiskillDriver2.shouldFireEquipment = false;
			aiskillDriver2.shouldSprint = false;
			aiskillDriver2.skillSlot = SkillSlot.Secondary;

			AISkillDriver aiskillDriver3 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver3.customName = "Slam";
			aiskillDriver3.activationRequiresAimConfirmation = false;
			aiskillDriver3.activationRequiresAimTargetLoS = false;
			aiskillDriver3.activationRequiresTargetLoS = false;
			aiskillDriver3.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver3.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver3.driverUpdateTimerOverride = -1f;
			aiskillDriver3.ignoreNodeGraph = true;
			aiskillDriver3.maxDistance = 10f;
			aiskillDriver3.minDistance = 0f;
			aiskillDriver3.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver3.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver3.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver3.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver3.moveInputScale = 1f;
			aiskillDriver3.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver3.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			//aiskillDriver3.nextHighPriorityOverride =;
			aiskillDriver3.noRepeat = false;
			//aiskillDriver3.requiredSkill =;
			aiskillDriver3.requireEquipmentReady = false;
			aiskillDriver3.requireSkillReady = true;
			aiskillDriver3.resetCurrentEnemyOnNextDriverSelection = true;
			aiskillDriver3.selectionRequiresAimTarget = false;
			aiskillDriver3.selectionRequiresOnGround = false;
			aiskillDriver3.selectionRequiresTargetLoS = false;
			aiskillDriver3.shouldFireEquipment = false;
			aiskillDriver3.shouldSprint = false;
			aiskillDriver3.skillSlot = SkillSlot.Primary;

			AISkillDriver aiskillDriver4 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver4.customName = "ReturnToOwnerLeash";
			aiskillDriver4.activationRequiresAimConfirmation = false;
			aiskillDriver4.activationRequiresAimTargetLoS = false;
			aiskillDriver4.activationRequiresTargetLoS = false;
			aiskillDriver4.aimType = AISkillDriver.AimType.AtCurrentLeader;
			aiskillDriver4.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver4.driverUpdateTimerOverride = 3f;
			aiskillDriver4.ignoreNodeGraph = false;
			aiskillDriver4.maxDistance = float.PositiveInfinity;
			aiskillDriver4.minDistance = MainPlugin.Gland_AI_LeashLength.Value;
			aiskillDriver4.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver4.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver4.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver4.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver4.moveInputScale = 1f;
			aiskillDriver4.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver4.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
			//aiskillDriver4.nextHighPriorityOverride =;
			aiskillDriver4.noRepeat = false;
			//aiskillDriver4.requiredSkill =;
			aiskillDriver4.requireEquipmentReady = false;
			aiskillDriver4.selectionRequiresAimTarget = false;
			aiskillDriver4.selectionRequiresOnGround = false;
			aiskillDriver4.selectionRequiresTargetLoS = false;
			aiskillDriver4.shouldFireEquipment = false;
			aiskillDriver4.shouldSprint = true;
			aiskillDriver4.requireSkillReady = false;
			if (MainPlugin.Gland_AddUtility.Value)
            {
				aiskillDriver4.skillSlot = SkillSlot.Utility;
				aiskillDriver4.resetCurrentEnemyOnNextDriverSelection = true;
			}
			else
            {
				aiskillDriver4.skillSlot = SkillSlot.None;
				aiskillDriver4.resetCurrentEnemyOnNextDriverSelection = false;
			}

			AISkillDriver aiskillDriver5 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver5.customName = "StrafeBecauseCooldowns";
			aiskillDriver5.activationRequiresAimConfirmation = false;
			aiskillDriver5.activationRequiresAimTargetLoS = false;
			aiskillDriver5.activationRequiresTargetLoS = false;
			aiskillDriver5.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver5.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver5.driverUpdateTimerOverride = -1f;
			aiskillDriver5.ignoreNodeGraph = true;
			aiskillDriver5.maxDistance = 10f;
			aiskillDriver5.minDistance = 0f;
			aiskillDriver5.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver5.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver5.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver5.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver5.moveInputScale = 1f;
			aiskillDriver5.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver5.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			//aiskillDriver5.nextHighPriorityOverride =;
			aiskillDriver5.noRepeat = false;
			//aiskillDriver5.requiredSkill =;
			aiskillDriver5.requireEquipmentReady = false;
			aiskillDriver5.requireSkillReady = false;
			aiskillDriver5.resetCurrentEnemyOnNextDriverSelection = false;
			aiskillDriver5.selectionRequiresAimTarget = false;
			aiskillDriver5.selectionRequiresOnGround = false;
			aiskillDriver5.selectionRequiresTargetLoS = true;
			aiskillDriver5.shouldFireEquipment = false;
			aiskillDriver5.shouldSprint = false;
			aiskillDriver5.skillSlot = SkillSlot.None;

			AISkillDriver aiskillDriver6 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver6.customName = "ChaseOffNodegraph";
			aiskillDriver6.activationRequiresAimConfirmation = false;
			aiskillDriver6.activationRequiresAimTargetLoS = false;
			aiskillDriver6.activationRequiresTargetLoS = false;
			aiskillDriver6.aimType = AISkillDriver.AimType.AtMoveTarget;
			aiskillDriver6.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver6.driverUpdateTimerOverride = -1f;
			aiskillDriver6.ignoreNodeGraph = true;
			aiskillDriver6.maxDistance = 25f;
			aiskillDriver6.minDistance = 10f;
			aiskillDriver6.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver6.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver6.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver6.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver6.moveInputScale = 1f;
			aiskillDriver6.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver6.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			//aiskillDriver6.nextHighPriorityOverride =;
			aiskillDriver6.noRepeat = false;
			//aiskillDriver6.requiredSkill =;
			aiskillDriver6.requireEquipmentReady = false;
			aiskillDriver6.requireSkillReady = false;
			aiskillDriver6.resetCurrentEnemyOnNextDriverSelection = true;
			aiskillDriver6.selectionRequiresAimTarget = false;
			aiskillDriver6.selectionRequiresOnGround = false;
			aiskillDriver6.selectionRequiresTargetLoS = true;
			aiskillDriver6.shouldFireEquipment = false;
			aiskillDriver6.shouldSprint = false;
			aiskillDriver6.skillSlot = SkillSlot.None;

			AISkillDriver aiskillDriver7 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver7.customName = "Chase";
			aiskillDriver7.activationRequiresAimConfirmation = false;
			aiskillDriver7.activationRequiresAimTargetLoS = false;
			aiskillDriver7.activationRequiresTargetLoS = false;
			aiskillDriver7.aimType = AISkillDriver.AimType.MoveDirection;
			aiskillDriver7.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver7.driverUpdateTimerOverride = -1f;
			aiskillDriver7.ignoreNodeGraph = false;
			aiskillDriver7.maxDistance = float.PositiveInfinity;
			aiskillDriver7.minDistance = 0f;
			aiskillDriver7.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver7.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver7.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver7.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver7.moveInputScale = 1f;
			aiskillDriver7.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver7.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			//aiskillDriver7.nextHighPriorityOverride =;
			aiskillDriver7.noRepeat = false;
			//aiskillDriver7.requiredSkill =;
			aiskillDriver7.requireEquipmentReady = false;
			aiskillDriver7.requireSkillReady = false;
			aiskillDriver7.resetCurrentEnemyOnNextDriverSelection = true;
			aiskillDriver7.selectionRequiresAimTarget = false;
			aiskillDriver7.selectionRequiresOnGround = false;
			aiskillDriver7.selectionRequiresTargetLoS = true;
			aiskillDriver7.shouldFireEquipment = false;
			aiskillDriver7.shouldSprint = false;
			aiskillDriver7.skillSlot = SkillSlot.None;

			AISkillDriver aiskillDriver8 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver8.customName = "ReturnToLeaderDefault";
			aiskillDriver8.activationRequiresAimConfirmation = false;
			aiskillDriver8.activationRequiresAimTargetLoS = false;
			aiskillDriver8.activationRequiresTargetLoS = false;
			aiskillDriver8.aimType = AISkillDriver.AimType.MoveDirection;
			aiskillDriver8.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver8.driverUpdateTimerOverride = -1f;
			aiskillDriver8.ignoreNodeGraph = false;
			aiskillDriver8.maxDistance = float.PositiveInfinity;
			aiskillDriver8.minDistance = 18f;
			aiskillDriver8.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver8.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver8.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver8.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver8.moveInputScale = 1f;
			aiskillDriver8.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aiskillDriver8.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
			//aiskillDriver8.nextHighPriorityOverride =;
			aiskillDriver8.noRepeat = false;
			//aiskillDriver8.requiredSkill =;
			aiskillDriver8.requireEquipmentReady = false;
			aiskillDriver8.requireSkillReady = false;
			aiskillDriver8.resetCurrentEnemyOnNextDriverSelection = true;
			aiskillDriver8.selectionRequiresAimTarget = false;
			aiskillDriver8.selectionRequiresOnGround = false;
			aiskillDriver8.selectionRequiresTargetLoS = false;
			aiskillDriver8.shouldFireEquipment = false;
			aiskillDriver8.shouldSprint = true;
			aiskillDriver8.skillSlot = SkillSlot.None;

			AISkillDriver aiskillDriver9 = prefab.AddComponent<AISkillDriver>();
			aiskillDriver9.customName = "WaitNearLeaderDefault";
			aiskillDriver9.activationRequiresAimConfirmation = false;
			aiskillDriver9.activationRequiresAimTargetLoS = false;
			aiskillDriver9.activationRequiresTargetLoS = false;
			aiskillDriver9.aimType = AISkillDriver.AimType.MoveDirection;
			aiskillDriver9.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver9.driverUpdateTimerOverride = -1f;
			aiskillDriver9.ignoreNodeGraph = false;
			aiskillDriver9.maxDistance = float.PositiveInfinity;
			aiskillDriver9.minDistance = 0f;
			aiskillDriver9.maxTargetHealthFraction = float.PositiveInfinity;
			aiskillDriver9.maxUserHealthFraction = float.PositiveInfinity;
			aiskillDriver9.minTargetHealthFraction = float.NegativeInfinity;
			aiskillDriver9.minUserHealthFraction = float.NegativeInfinity;
			aiskillDriver9.moveInputScale = 1f;
			aiskillDriver9.movementType = AISkillDriver.MovementType.Stop;
			aiskillDriver9.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
			//aiskillDriver9.nextHighPriorityOverride =;
			aiskillDriver9.noRepeat = false;
			//aiskillDriver9.requiredSkill =;
			aiskillDriver9.requireEquipmentReady = false;
			aiskillDriver9.requireSkillReady = false;
			aiskillDriver9.resetCurrentEnemyOnNextDriverSelection = true;
			aiskillDriver9.selectionRequiresAimTarget = false;
			aiskillDriver9.selectionRequiresOnGround = false;
			aiskillDriver9.selectionRequiresTargetLoS = false;
			aiskillDriver9.shouldFireEquipment = false;
			aiskillDriver9.shouldSprint = false;
			aiskillDriver9.skillSlot = SkillSlot.None;
		}
		public static void Hooks()
		{
			On.RoR2.CharacterBody.UpdateBeetleGuardAllies += CharacterBody_UpdateBeetleGuardAllies;
			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
			if (MainPlugin.Gland_AI_Target.Value)
			{
				On.RoR2.CharacterAI.BaseAI.FixedUpdate += BaseAI_FixedUpdate;
			}
		}
		private static int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
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
			return Math.Min(MainPlugin.Gland_MaxSummons.Value, self.inventory.GetItemCount(RoR2Content.Items.BeetleGland)) * mult;
		}
		private static void CharacterBody_UpdateBeetleGuardAllies(On.RoR2.CharacterBody.orig_UpdateBeetleGuardAllies orig, CharacterBody self)
		{
			if (NetworkServer.active)
			{
				if (self.inventory && self.master)
				{
					int extraglands = Math.Max(0, self.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - MainPlugin.Gland_MaxSummons.Value);
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
		private static void SetupSummonedBeetleGuard(SpawnCard.SpawnResult spawnResult, CharacterMaster summoner, int itemcount)
		{
			GameObject spawnedInstance = spawnResult.spawnedInstance;
			if (!spawnedInstance)
			{
				return;
			}
			CharacterMaster beeble = spawnedInstance.GetComponent<CharacterMaster>();
			if (beeble)
			{
				Helpers.GiveRandomEliteAffix(beeble);
				Deployable deployable = beeble.GetComponent<Deployable>();
				if (deployable)
				{
					deployable.onUndeploy.AddListener(new UnityEngine.Events.UnityAction(beeble.TrueKill));
					summoner.AddDeployable(deployable, DeployableSlot.BeetleGuardAlly);
					if (summoner.GetBody())
					{
						summoner.GetBody().guardResummonCooldown = MainPlugin.Gland_RespawnTime.Value;
					}
				}
			}
		}
		private static void BaseAI_FixedUpdate(On.RoR2.CharacterAI.BaseAI.orig_FixedUpdate orig, BaseAI self)
		{
			float lastAttention = self.enemyAttention;
			orig(self);
			if (self.name != BeetleGuardAlly_BaseAI.name + "(Clone)")
			{
				return;
			}
			if(!self.body)
            {
				return;
            }
			if (!self.currentEnemy.gameObject)
			{
				return;
			}
			CharacterBody targetbody = self.currentEnemy.gameObject.GetComponent<CharacterBody>();
			if (targetbody)
			{
				if (targetbody.isFlying && !targetbody.isBoss)
				{
					if (lastAttention < self.enemyAttention)
					{
						if (MainPlugin.Gland_Debug.Value)
						{
							MainPlugin.ModLogger.LogInfo("Attempt to redirect focus from flying target.");
						}
						HurtBox target = FindEnemy(self);
						if (target)
						{
							self.currentEnemy.gameObject = target.healthComponent.gameObject;
							self.currentEnemy.bestHurtBox = target;
							self.enemyAttention = self.enemyAttentionDuration;
							if(!target.healthComponent.body.isFlying)
                            {
								self.targetRefreshTimer = self.enemyAttentionDuration;
                            }
						}
					}
				}
			}
		}
		private static HurtBox FindEnemy(BaseAI self)
        {
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = self.body;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(self.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = MainPlugin.Gland_AI_LeashLength.Value;
			search.searchOrigin = self.bodyInputBank.aimOrigin;
			search.searchDirection = self.bodyInputBank.aimDirection;
			search.maxAngleFilter = (self.fullVision ? 180f : 90f);
			search.filterByLoS = true;
			search.RefreshCandidates();
			HurtBox result = null;
			foreach (HurtBox target in search.GetResults())
			{
				if (target && target.healthComponent)
				{
					if (target.healthComponent.body.isFlying && !target.healthComponent.body.isBoss)
					{
						if (result == null)
						{
							result = target;
						}
					}
					else
					{
						if (result)
						{
							if (Vector3.Distance(self.body.transform.position, target.healthComponent.body.transform.position) < 25f)
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
			return result;
		}
	}
}