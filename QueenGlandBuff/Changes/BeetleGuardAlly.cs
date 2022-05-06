using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates;
using RoR2.Projectile;

namespace QueenGlandBuff.Changes
{
    public class BeetleGuardAlly
    {
		public static GameObject Default_Proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/hermitcrabbombprojectile");
		public static GameObject Perfect_Sunder_MainProj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/brothersunderwave");
		public static GameObject Perfect_Sunder_SecProj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/lunarshardprojectile");
		public static GameObject Perfect_Slam_Proj = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/brotherfirepillar");
		public static GameObject SlamRockProjectile;

		public static GameObject BodyObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BeetleGuardAllyBody");
		public static GameObject MasterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BeetleGuardAllyMaster");

		public static BaseAI BaseAI;

		public static BuffDef Staunching;
		public static BuffDef BeetleFrenzy;

		public static SkillDef SlamSkill;
		public static SkillDef SunderSkill;
		public static SkillDef RecallSkill;
		public static SkillDef StaunchSkill;

		public static SkillDef OldSlamSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Beetle/BeetleGuardBodyGroundSlam.asset").WaitForCompletion();
		public static SkillDef OldSunderSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Beetle/BeetleGuardBodySunder.asset").WaitForCompletion();

		public static void Begin()
        {
			CreateBuffs();
			CreateProjectiles();
			UpdateBody();
			CreateSkills();
			UpdateLoadouts();
			UpdateAI();
		}
		private static void CreateBuffs()
        {
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Creating BeetleGuardAlly buffs.");
			}
			BuffDef beetlebuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Beetle/bdBeetleJuice.asset").WaitForCompletion();
			BuffDef warcrybuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/TeamWarCry/bdTeamWarCry.asset").WaitForCompletion();
			if (MainPlugin.Config_AddUtility.Value || MainPlugin.Config_AddSpecial.Value)
			{
				BeetleFrenzy = Modules.Buffs.AddNewBuff("BeetleFrenzy", warcrybuff.iconSprite, beetlebuff.buffColor, false, false, false);
			}
			if (MainPlugin.Config_AddSpecial.Value)
			{
				Staunching = Modules.Buffs.AddNewBuff("Staunch", beetlebuff.iconSprite, warcrybuff.buffColor, false, false, false);
			}
		}
		private static void CreateProjectiles()
        {
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Creating BeetleGuardAlly projectiles.");
			}
			if (MainPlugin.Config_PrimaryBuff.Value || MainPlugin.Config_SecondaryBuff.Value)
			{
				SlamRockProjectile = PrefabAPI.InstantiateClone(Default_Proj, MainPlugin.MODTOKEN + "RockProjectile", true);
				SlamRockProjectile.GetComponent<ProjectileExplosion>().falloffModel = BlastAttack.FalloffModel.Linear;
				SlamRockProjectile.GetComponent<ProjectileExplosion>().bonusBlastForce = Vector3.down * 4f;
				SlamRockProjectile.GetComponent<ProjectileExplosion>().blastRadius *= 1.5f;
				Modules.Projectiles.AddProjectile(SlamRockProjectile);
			}
		}
		private static void UpdateBody()
		{
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing BeetleGuardAlly attributes.");
			}
			CharacterBody charBody = BodyObject.GetComponent<CharacterBody>();
			charBody.baseAcceleration *= 1.5f;
			charBody.baseJumpPower *= 1.3f;
			charBody.baseRegen = 1f;
			charBody.levelRegen = 0.2f;
			CharacterDirection charDir = BodyObject.GetComponent<CharacterDirection>();
			charDir.turnSpeed *= 2f;
		}
		private static void CreateSkills()
        {
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Creating BeetleGuardAlly skills.");
			}
			SlamSkill = ScriptableObject.CreateInstance<SkillDef>();

			LanguageAPI.Add(MainPlugin.MODTOKEN + "PRIMARY_SLAM_NAME", "Slam");
			if (MainPlugin.Config_PrimaryBuff.Value)
			{
				SlamSkill.activationState = new SerializableEntityStateType(typeof(States.Slam));
				LanguageAPI.Add(MainPlugin.MODTOKEN + "PRIMARY_SLAM_DESC", "Strike the ground for <style=cIsDamage>400%</style> and launch debris for <style=cIsDamage>5x75%</style> damage.");
				Modules.States.RegisterState(typeof(States.Slam));
			}
			else
			{
				SlamSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.BeetleGuardMonster.GroundSlam));
				LanguageAPI.Add(MainPlugin.MODTOKEN + "PRIMARY_SLAM_DESC", "Strike the ground for <style=cIsDamage>400%</style> damage.");
			}
			SlamSkill.activationStateMachineName = "Body";
			SlamSkill.dontAllowPastMaxStocks = false;

			SlamSkill.resetCooldownTimerOnUse = false;
			SlamSkill.keywordTokens = null;

			SlamSkill.baseMaxStock = 1;
			SlamSkill.rechargeStock = 1;
			SlamSkill.requiredStock = 1;
			SlamSkill.stockToConsume = 1;
			SlamSkill.fullRestockOnAssign = true;

			SlamSkill.baseRechargeInterval = 3;
			SlamSkill.beginSkillCooldownOnSkillEnd = false;

			SlamSkill.canceledFromSprinting = false;
			SlamSkill.cancelSprintingOnActivation = true;
			SlamSkill.forceSprintDuringState = false;

			SlamSkill.interruptPriority = InterruptPriority.Skill;
			SlamSkill.isCombatSkill = true;
			SlamSkill.mustKeyPress = false;

			SlamSkill.icon = null;
			SlamSkill.skillDescriptionToken = MainPlugin.MODTOKEN + "PRIMARY_SLAM_DESC";
			SlamSkill.skillNameToken = MainPlugin.MODTOKEN + "PRIMARY_SLAM_NAME";
			SlamSkill.skillName = SlamSkill.skillNameToken;

			Modules.Skills.RegisterSkill(SlamSkill);

			SunderSkill = ScriptableObject.CreateInstance<SkillDef>();

			LanguageAPI.Add(MainPlugin.MODTOKEN + "SECONDARY_SUNDER_NAME", "Sunder");
			if (MainPlugin.Config_SecondaryBuff.Value)
			{
				SunderSkill.activationState = new SerializableEntityStateType(typeof(States.Sunder));
				LanguageAPI.Add(MainPlugin.MODTOKEN + "SECONDARY_SUNDER_DESC", "Tear the ground infront of you for <style=cIsDamage>400%</style>, launching a cluster of rocks for <style=cIsDamage>5x75%</style> damage.");
				Modules.States.RegisterState(typeof(States.Sunder));
			}
			else
			{
				SunderSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.BeetleGuardMonster.FireSunder));
				LanguageAPI.Add(MainPlugin.MODTOKEN + "SECONDARY_SUNDER_DESC", "Tear the ground infront of you for <style=cIsDamage>400%</style> damage.");
			}
			SunderSkill.activationStateMachineName = "Body";

			SunderSkill.baseMaxStock = 2;
			SunderSkill.rechargeStock = 1;
			SunderSkill.requiredStock = 1;
			SunderSkill.stockToConsume = 1;
			SunderSkill.fullRestockOnAssign = true;

			SunderSkill.baseRechargeInterval = 8;
			SunderSkill.beginSkillCooldownOnSkillEnd = false;

			SunderSkill.canceledFromSprinting = false;
			SunderSkill.cancelSprintingOnActivation = true;
			SunderSkill.forceSprintDuringState = false;

			SunderSkill.interruptPriority = InterruptPriority.Skill;
			SunderSkill.isCombatSkill = true;
			SunderSkill.mustKeyPress = false;

			SunderSkill.icon = null;
			SunderSkill.skillDescriptionToken = MainPlugin.MODTOKEN + "SECONDARY_SUNDER_DESC";
			SunderSkill.skillNameToken = MainPlugin.MODTOKEN + "SECONDARY_SUNDER_NAME";
			SunderSkill.skillName = SunderSkill.skillNameToken;

			Modules.Skills.RegisterSkill(SunderSkill);

			if (MainPlugin.Config_AddUtility.Value)
			{
				RecallSkill = ScriptableObject.CreateInstance<SkillDef>();

				LanguageAPI.Add(MainPlugin.MODTOKEN + "UTILITY_TELEPORT_NAME", "Recall");
				LanguageAPI.Add(MainPlugin.MODTOKEN + "UTILITY_TELEPORT_DESC", "<style=cIsUtility>Burrow</style> to your owner's side. Enter a <style=cIsDamage>frenzy</style> for <style=cIsUtility>10</style> seconds if you have no owner.");

				RecallSkill.activationState = new SerializableEntityStateType(typeof(States.Recall));
				RecallSkill.activationStateMachineName = "Body";
				Modules.States.RegisterState(typeof(States.Recall));

				RecallSkill.baseMaxStock = 1;
				RecallSkill.rechargeStock = 1;
				RecallSkill.requiredStock = 1;
				RecallSkill.stockToConsume = 1;
				RecallSkill.fullRestockOnAssign = true;

				RecallSkill.baseRechargeInterval = 12f;
				RecallSkill.beginSkillCooldownOnSkillEnd = false;

				RecallSkill.canceledFromSprinting = false;
				RecallSkill.cancelSprintingOnActivation = true;
				RecallSkill.forceSprintDuringState = false;

				RecallSkill.interruptPriority = InterruptPriority.Skill;
				RecallSkill.isCombatSkill = false;
				RecallSkill.mustKeyPress = false;

				RecallSkill.icon = null;
				RecallSkill.skillDescriptionToken = MainPlugin.MODTOKEN + "UTILITY_TELEPORT_DESC";
				RecallSkill.skillNameToken = MainPlugin.MODTOKEN + "UTILITY_TELEPORT_NAME";
				RecallSkill.skillName = RecallSkill.skillNameToken;

				Modules.Skills.RegisterSkill(RecallSkill);
			}

			if (MainPlugin.Config_AddSpecial.Value)
			{
				StaunchSkill = ScriptableObject.CreateInstance<SkillDef>();
				LanguageAPI.Add(MainPlugin.MODTOKEN + "SPECIAL_TAUNT_NAME", "Staunch");
				LanguageAPI.Add(MainPlugin.MODTOKEN + "SPECIAL_TAUNT_DESC", "Draw the <style=cIsHealth>attention</style> of nearby enemies for <style=cIsUtility>10</style> seconds. While active gain <style=cIsUtility>100</style> armor and send nearby <style=cIsHealing>friendly beetles</style> into a <style=cIsDamage>frenzy</style>.");

				StaunchSkill.activationState = new SerializableEntityStateType(typeof(States.Staunch));
				StaunchSkill.activationStateMachineName = "Body";
				Modules.States.RegisterState(typeof(States.Staunch));

				StaunchSkill.baseMaxStock = 1;
				StaunchSkill.rechargeStock = 1;
				StaunchSkill.requiredStock = 1;
				StaunchSkill.stockToConsume = 1;
				StaunchSkill.fullRestockOnAssign = true;

				StaunchSkill.baseRechargeInterval = 30f;
				StaunchSkill.beginSkillCooldownOnSkillEnd = false;

				StaunchSkill.canceledFromSprinting = false;
				StaunchSkill.cancelSprintingOnActivation = true;
				StaunchSkill.forceSprintDuringState = false;

				StaunchSkill.interruptPriority = InterruptPriority.Skill;
				StaunchSkill.isCombatSkill = false;
				StaunchSkill.mustKeyPress = false;

				StaunchSkill.icon = null;
				StaunchSkill.skillDescriptionToken = MainPlugin.MODTOKEN + "SPECIAL_TAUNT_DESC";
				StaunchSkill.skillNameToken = MainPlugin.MODTOKEN + "SPECIAL_TAUNT_NAME";
				StaunchSkill.skillName = StaunchSkill.skillNameToken;

				Modules.Skills.RegisterSkill(StaunchSkill);
			}
		}
		private static void UpdateLoadouts()
		{
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Adding new Skills to BeetleGuardAllyBody.");
			}
			Modules.Skills.WipeLoadout(BodyObject);
			Modules.Skills.AddSkillToSlot(BodyObject, SlamSkill, SkillSlot.Primary);
			Modules.Skills.AddSkillToSlot(BodyObject, SunderSkill, SkillSlot.Secondary);
			if (MainPlugin.Config_AddUtility.Value)
			{
				Modules.Skills.AddSkillToSlot(BodyObject, RecallSkill, SkillSlot.Utility);
			}
			if (MainPlugin.Config_AddSpecial.Value)
			{
				Modules.Skills.AddSkillToSlot(BodyObject, StaunchSkill, SkillSlot.Special);
			}
		}

		private static void UpdateAI()
		{
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing BaseAI and SkillDrivers.");
			}
			BaseAI = MasterObject.GetComponent<BaseAI>();

			BaseAI.neverRetaliateFriendlies = true;
			BaseAI.fullVision = true;

			foreach (AISkillDriver obj in MasterObject.GetComponentsInChildren<AISkillDriver>())
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}

			if (MainPlugin.Config_AddSpecial.Value)
			{
				AISkillDriver aiskillDriver1 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver2 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver3 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver4 = MasterObject.AddComponent<AISkillDriver>();
			aiskillDriver4.customName = "ReturnToOwnerLeash";
			aiskillDriver4.activationRequiresAimConfirmation = false;
			aiskillDriver4.activationRequiresAimTargetLoS = false;
			aiskillDriver4.activationRequiresTargetLoS = false;
			aiskillDriver4.aimType = AISkillDriver.AimType.AtCurrentLeader;
			aiskillDriver4.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aiskillDriver4.driverUpdateTimerOverride = 3f;
			aiskillDriver4.ignoreNodeGraph = false;
			aiskillDriver4.maxDistance = float.PositiveInfinity;
			aiskillDriver4.minDistance = MainPlugin.Config_AI_MinRecallDist.Value;
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
			if (MainPlugin.Config_AddUtility.Value)
            {
				aiskillDriver4.skillSlot = SkillSlot.Utility;
				aiskillDriver4.resetCurrentEnemyOnNextDriverSelection = true;
			}
			else
            {
				aiskillDriver4.skillSlot = SkillSlot.None;
				aiskillDriver4.resetCurrentEnemyOnNextDriverSelection = false;
			}

			AISkillDriver aiskillDriver5 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver6 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver7 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver8 = MasterObject.AddComponent<AISkillDriver>();
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

			AISkillDriver aiskillDriver9 = MasterObject.AddComponent<AISkillDriver>();
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
	}
}