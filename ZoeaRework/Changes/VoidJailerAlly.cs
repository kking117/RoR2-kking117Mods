using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates;

namespace ZoeaRework.Changes
{
    public class VoidJailerAlly
    {
        public static GameObject AllyBodyObject;
        public static GameObject AllyMasterObject;
        public static SkillDef UtilitySkill;
        public static CharacterSpawnCard AllySpawnCard;
        public static void Begin()
        {
            CreateUtilitySkill();
            FindMasterCreateSpawnCard();
            CheckAndUpdateMasterBody();
            UpdateAI();
        }
        private static void CheckAndUpdateMasterBody()
        {
            bool BodyExists = false;
            CharacterMaster master = AllyMasterObject.GetComponent<CharacterMaster>();
            if (master.bodyPrefab)
            {
                if(master.bodyPrefab.name != "VoidJailerBody")
                {
                    BodyExists = true;
                    MainPlugin.ModLogger.LogInfo("Master already has a replacement body, skipping creation step.");
                    AllyBodyObject = master.bodyPrefab;
                }
            }
            if(!BodyExists)
            {
                CreateNewBody();
            }
            if (master.bodyPrefab)
            {
                Modules.Skills.AddSkillToSlot(AllyBodyObject, UtilitySkill, SkillSlot.Utility);
                CharacterBody body = master.bodyPrefab.GetComponent<CharacterBody>();
                if (body)
                {
                    body.baseMoveSpeed = MainPlugin.Config_VoidJailer_BaseSpeed.Value;
                    body.baseMaxHealth *= 0.5f;
                    body.levelMaxHealth *= 0.5f;
                }
            }
        }
        private static void CreateNewBody()
        {
            GameObject bodyprefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerBody.prefab").WaitForCompletion();
            AllyBodyObject = bodyprefab.InstantiateClone("VoidJailerAllyBody", true);
            AllyBodyObject.GetComponent<DeathRewards>().logUnlockableDef = null;
            Modules.Prefabs.bodyPrefabs.Add(AllyBodyObject);
            AllyMasterObject.GetComponent<CharacterMaster>().bodyPrefab = AllyBodyObject;
        }
        private static void FindMasterCreateSpawnCard()
        {
            AllyMasterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerAllyMaster.prefab").WaitForCompletion();
            AllySpawnCard = UnityEngine.Object.Instantiate<CharacterSpawnCard>(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/VoidJailer/cscVoidJailer.asset").WaitForCompletion());
            AllySpawnCard.name = "cscZoeaReworkVoidJailerAlly";
            AllySpawnCard.prefab = AllyMasterObject;
        }
        private static void UpdateAI()
        {
            BaseAI BaseAI = AllyMasterObject.GetComponent<BaseAI>();
            if (BaseAI)
            {
                BaseAI.neverRetaliateFriendlies = true;
                BaseAI.fullVision = true;
            }
            foreach (AISkillDriver obj in AllyMasterObject.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }

            //Combat Drivers
            AISkillDriver aiskillDriver1 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver1.customName = "Capture";
            aiskillDriver1.activationRequiresAimConfirmation = false;
            aiskillDriver1.activationRequiresAimTargetLoS = false;
            aiskillDriver1.activationRequiresTargetLoS = true;
            aiskillDriver1.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver1.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver1.driverUpdateTimerOverride = -1f;
            aiskillDriver1.ignoreNodeGraph = false;
            aiskillDriver1.maxDistance = 80f;
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
            aiskillDriver1.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver1.selectionRequiresAimTarget = false;
            aiskillDriver1.selectionRequiresOnGround = false;
            aiskillDriver1.selectionRequiresTargetLoS = true;
            aiskillDriver1.shouldFireEquipment = false;
            aiskillDriver1.shouldSprint = false;
            aiskillDriver1.skillSlot = SkillSlot.Secondary;

            AISkillDriver aiskillDriver2 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver2.customName = "PrimaryCharge";
            aiskillDriver2.activationRequiresAimConfirmation = true;
            aiskillDriver2.activationRequiresAimTargetLoS = false;
            aiskillDriver2.activationRequiresTargetLoS = false;
            aiskillDriver2.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver2.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver2.driverUpdateTimerOverride = 1f;
            aiskillDriver2.ignoreNodeGraph = false;
            aiskillDriver2.maxDistance = 80f;
            aiskillDriver2.minDistance = 45f;
            aiskillDriver2.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver2.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver2.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver2.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver2.moveInputScale = 1f;
            aiskillDriver2.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver2.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver2.nextHighPriorityOverride =;
            aiskillDriver2.noRepeat = false;
            //aiskillDriver2.requiredSkill =;
            aiskillDriver2.requireEquipmentReady = false;
            aiskillDriver2.requireSkillReady = true;
            aiskillDriver2.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver2.selectionRequiresAimTarget = false;
            aiskillDriver2.selectionRequiresOnGround = false;
            aiskillDriver2.selectionRequiresTargetLoS = true;
            aiskillDriver2.shouldFireEquipment = false;
            aiskillDriver2.shouldSprint = false;
            aiskillDriver2.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver3 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver3.customName = "PrimaryStrafe";
            aiskillDriver3.activationRequiresAimConfirmation = true;
            aiskillDriver3.activationRequiresAimTargetLoS = false;
            aiskillDriver3.activationRequiresTargetLoS = false;
            aiskillDriver3.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver3.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver3.driverUpdateTimerOverride = 1f;
            aiskillDriver3.ignoreNodeGraph = false;
            aiskillDriver3.maxDistance = 45f;
            aiskillDriver3.minDistance = 1f;
            aiskillDriver3.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver3.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver3.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver3.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver3.moveInputScale = 1f;
            aiskillDriver3.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            aiskillDriver3.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver3.nextHighPriorityOverride =;
            aiskillDriver3.noRepeat = false;
            //aiskillDriver3.requiredSkill =;
            aiskillDriver3.requireEquipmentReady = false;
            aiskillDriver3.requireSkillReady = true;
            aiskillDriver3.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver3.selectionRequiresAimTarget = false;
            aiskillDriver3.selectionRequiresOnGround = false;
            aiskillDriver3.selectionRequiresTargetLoS = true;
            aiskillDriver3.shouldFireEquipment = false;
            aiskillDriver3.shouldSprint = false;
            aiskillDriver3.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver4 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver4.customName = "ReturnToOwnerLeash";
            aiskillDriver4.activationRequiresAimConfirmation = false;
            aiskillDriver4.activationRequiresAimTargetLoS = false;
            aiskillDriver4.activationRequiresTargetLoS = false;
            aiskillDriver4.aimType = AISkillDriver.AimType.AtCurrentLeader;
            aiskillDriver4.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver4.driverUpdateTimerOverride = 3f;
            aiskillDriver4.ignoreNodeGraph = false;
            aiskillDriver4.maxDistance = float.PositiveInfinity;
            aiskillDriver4.minDistance = MainPlugin.Config_AIShared_MinRecallDist.Value;
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
            aiskillDriver4.shouldSprint = false;
            aiskillDriver4.requireSkillReady = true;
            aiskillDriver4.skillSlot = SkillSlot.Utility;
            aiskillDriver4.resetCurrentEnemyOnNextDriverSelection = false;

            AISkillDriver aiskillDriver5 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver5.customName = "PathFromAfar";
            aiskillDriver5.activationRequiresAimConfirmation = false;
            aiskillDriver5.activationRequiresAimTargetLoS = false;
            aiskillDriver5.activationRequiresTargetLoS = false;
            aiskillDriver5.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver5.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver5.driverUpdateTimerOverride = -1f;
            aiskillDriver5.ignoreNodeGraph = false;
            aiskillDriver5.maxDistance = float.PositiveInfinity;
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

            //Follow Leader Drivers

            AISkillDriver aiskillDriver8 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver8.customName = "ReturnToLeaderDefault";
            aiskillDriver8.activationRequiresAimConfirmation = false;
            aiskillDriver8.activationRequiresAimTargetLoS = false;
            aiskillDriver8.activationRequiresTargetLoS = false;
            aiskillDriver8.aimType = AISkillDriver.AimType.MoveDirection;
            aiskillDriver8.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver8.driverUpdateTimerOverride = -1f;
            aiskillDriver8.ignoreNodeGraph = false;
            aiskillDriver8.maxDistance = float.PositiveInfinity;
            aiskillDriver8.minDistance = 20f;
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

            AISkillDriver aiskillDriver9 = AllyMasterObject.AddComponent<AISkillDriver>();
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
        private static void CreateUtilitySkill()
        {
            UtilitySkill = ScriptableObject.CreateInstance<SkillDef>();

            UtilitySkill.activationState = new SerializableEntityStateType(typeof(States.VoidJailer.Recall));
            Modules.States.RegisterState(typeof(States.VoidJailer.Recall));

            UtilitySkill.activationStateMachineName = "Body";
            UtilitySkill.dontAllowPastMaxStocks = false;

            UtilitySkill.resetCooldownTimerOnUse = false;
            UtilitySkill.keywordTokens = null;

            UtilitySkill.baseMaxStock = 1;
            UtilitySkill.rechargeStock = 1;
            UtilitySkill.requiredStock = 1;
            UtilitySkill.stockToConsume = 1;
            UtilitySkill.fullRestockOnAssign = true;

            UtilitySkill.baseRechargeInterval = MainPlugin.Config_VoidJailer_RecallCooldown.Value;
            UtilitySkill.beginSkillCooldownOnSkillEnd = false;

            UtilitySkill.canceledFromSprinting = false;
            UtilitySkill.cancelSprintingOnActivation = true;
            UtilitySkill.forceSprintDuringState = false;

            UtilitySkill.interruptPriority = InterruptPriority.Any;
            UtilitySkill.isCombatSkill = false;
            UtilitySkill.mustKeyPress = false;

            UtilitySkill.icon = null;
            UtilitySkill.skillDescriptionToken = MainPlugin.MODTOKEN + "UTILITY_TELEPORT_DESC";
            UtilitySkill.skillNameToken = MainPlugin.MODTOKEN + "UTILITY_TELEPORT_NAME";
            UtilitySkill.skillName = UtilitySkill.skillNameToken;

            Modules.Skills.RegisterSkill(UtilitySkill);
            
        }
    }
}
