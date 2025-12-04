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
    public class VoidMegaCrabAlly
    {
        public static GameObject AllyBodyObject;
        public static GameObject AllyMasterObject;
        public static SkillDef UtilitySkill;
        public static CharacterSpawnCard AllySpawnCard;
        internal static GameObject PortalEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabSpawnEffect.prefab").WaitForCompletion();
        public static void Begin()
        {
            CreateUtilitySkill();
            GatherEditAssets();
            UpdateAI();
        }
        internal static void PostLoad()
        {
            if (MainPlugin.Config_VoidMegaCrab_EnableDisplays)
            {
                CharacterMaster master = AllyMasterObject.GetComponent<CharacterMaster>();
                if (master.bodyPrefab)
                {
                    VoidMegaCrabAllyDisplays.Create(master.bodyPrefab);
                }
            }
        }
        private static void GatherEditAssets()
        {
            AllyMasterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabAllyMaster.prefab").WaitForCompletion();
            AllySpawnCard = UnityEngine.Object.Instantiate<CharacterSpawnCard>(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/VoidMegaCrab/cscVoidMegaCrabAlly.asset").WaitForCompletion());
            CharacterMaster master = AllyMasterObject.GetComponent<CharacterMaster>();
            if (master.bodyPrefab)
            {
                AllyBodyObject = master.bodyPrefab;
                Modules.Skills.AddSkillToSlot(AllyBodyObject, UtilitySkill, SkillSlot.Utility);
                CharacterBody body = master.bodyPrefab.GetComponent<CharacterBody>();
                if (body)
                {
                    body.baseMoveSpeed = MainPlugin.Config_VoidMegaCrab_BaseSpeed;
                    if (MainPlugin.Config_Rework_Enable)
                    {
                        body.baseMaxHealth = MainPlugin.Config_VoidMegaCrab_Rework_BaseHealth;
                        body.levelMaxHealth = MainPlugin.Config_VoidMegaCrab_Rework_LevelHealth;

                        body.baseDamage = MainPlugin.Config_VoidMegaCrab_Rework_BaseDamage;
                        body.levelDamage = MainPlugin.Config_VoidMegaCrab_Rework_LevelDamage;
                    }
                    else
                    {
                        body.baseMaxHealth *= 0.5f;
                        body.levelMaxHealth *= 0.5f;
                    }
                }
            }
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
            aiskillDriver1.customName = "FireBackMissiles";
            aiskillDriver1.activationRequiresAimConfirmation = false;
            aiskillDriver1.activationRequiresAimTargetLoS = false;
            aiskillDriver1.activationRequiresTargetLoS = false;
            aiskillDriver1.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver1.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver1.driverUpdateTimerOverride = -1f;
            aiskillDriver1.ignoreNodeGraph = false;
            aiskillDriver1.maxDistance = 300f;
            aiskillDriver1.minDistance = 0f;
            aiskillDriver1.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver1.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver1.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver1.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver1.moveInputScale = 1f;
            aiskillDriver1.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver1.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver1.nextHighPriorityOverride =;
            aiskillDriver1.noRepeat = false;
            //aiskillDriver1.requiredSkill =;
            aiskillDriver1.requireEquipmentReady = false;
            aiskillDriver1.requireSkillReady = true;
            aiskillDriver1.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver1.selectionRequiresAimTarget = true;
            aiskillDriver1.selectionRequiresOnGround = false;
            aiskillDriver1.selectionRequiresTargetLoS = false;
            aiskillDriver1.shouldFireEquipment = false;
            aiskillDriver1.shouldSprint = false;
            aiskillDriver1.skillSlot = SkillSlot.Special;

            AISkillDriver aiskillDriver2 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver2.customName = "FireSecondaryStrafe";
            aiskillDriver2.activationRequiresAimConfirmation = false;
            aiskillDriver2.activationRequiresAimTargetLoS = false;
            aiskillDriver2.activationRequiresTargetLoS = false;
            aiskillDriver2.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver2.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver2.driverUpdateTimerOverride = 0.5f;
            aiskillDriver2.ignoreNodeGraph = false;
            aiskillDriver2.maxDistance = 50f;
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
            aiskillDriver2.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver2.selectionRequiresAimTarget = false;
            aiskillDriver2.selectionRequiresOnGround = false;
            aiskillDriver2.selectionRequiresTargetLoS = false;
            aiskillDriver2.shouldFireEquipment = false;
            aiskillDriver2.shouldSprint = false;
            aiskillDriver2.skillSlot = SkillSlot.Secondary;

            AISkillDriver aiskillDriver3 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver3.customName = "FirePrimaryStrafe";
            aiskillDriver3.activationRequiresAimConfirmation = false;
            aiskillDriver3.activationRequiresAimTargetLoS = false;
            aiskillDriver3.activationRequiresTargetLoS = true;
            aiskillDriver3.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver3.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver3.driverUpdateTimerOverride = 1f;
            aiskillDriver3.ignoreNodeGraph = false;
            aiskillDriver3.maxDistance = 50f;
            aiskillDriver3.minDistance = 0f;
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
            aiskillDriver4.minDistance = MainPlugin.Config_AIShared_MinRecallDist;
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
            aiskillDriver5.customName = "FireSecondaryChase";
            aiskillDriver5.activationRequiresAimConfirmation = false;
            aiskillDriver5.activationRequiresAimTargetLoS = false;
            aiskillDriver5.activationRequiresTargetLoS = false;
            aiskillDriver5.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver5.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver5.driverUpdateTimerOverride = 0.5f;
            aiskillDriver5.ignoreNodeGraph = false;
            aiskillDriver5.maxDistance = 100f;
            aiskillDriver5.minDistance = 50f;
            aiskillDriver5.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver5.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver5.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver5.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver5.moveInputScale = 1f;
            aiskillDriver5.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            aiskillDriver5.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver5.nextHighPriorityOverride =;
            aiskillDriver5.noRepeat = false;
            //aiskillDriver5.requiredSkill =;
            aiskillDriver5.requireEquipmentReady = false;
            aiskillDriver5.requireSkillReady = true;
            aiskillDriver5.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver5.selectionRequiresAimTarget = false;
            aiskillDriver5.selectionRequiresOnGround = false;
            aiskillDriver5.selectionRequiresTargetLoS = true;
            aiskillDriver5.shouldFireEquipment = false;
            aiskillDriver5.shouldSprint = false;
            aiskillDriver5.skillSlot = SkillSlot.Secondary;

            AISkillDriver aiskillDriver6 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver6.customName = "FirePrimaryChase";
            aiskillDriver6.activationRequiresAimConfirmation = false;
            aiskillDriver6.activationRequiresAimTargetLoS = false;
            aiskillDriver6.activationRequiresTargetLoS = true;
            aiskillDriver6.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver6.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver6.driverUpdateTimerOverride = 1f;
            aiskillDriver6.ignoreNodeGraph = false;
            aiskillDriver6.maxDistance = 100f;
            aiskillDriver6.minDistance = 50f;
            aiskillDriver6.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver6.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver6.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver6.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver6.moveInputScale = 1f;
            aiskillDriver6.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            aiskillDriver6.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver6.nextHighPriorityOverride =;
            aiskillDriver6.noRepeat = false;
            //aiskillDriver6.requiredSkill =;
            aiskillDriver6.requireEquipmentReady = false;
            aiskillDriver6.requireSkillReady = true;
            aiskillDriver6.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver6.selectionRequiresAimTarget = false;
            aiskillDriver6.selectionRequiresOnGround = false;
            aiskillDriver6.selectionRequiresTargetLoS = true;
            aiskillDriver6.shouldFireEquipment = false;
            aiskillDriver6.shouldSprint = false;
            aiskillDriver6.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver7 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver7.customName = "Chase";
            aiskillDriver7.activationRequiresAimConfirmation = false;
            aiskillDriver7.activationRequiresAimTargetLoS = false;
            aiskillDriver7.activationRequiresTargetLoS = false;
            aiskillDriver7.aimType = AISkillDriver.AimType.AtMoveTarget;
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
            aiskillDriver7.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver7.selectionRequiresAimTarget = false;
            aiskillDriver7.selectionRequiresOnGround = false;
            aiskillDriver7.selectionRequiresTargetLoS = false;
            aiskillDriver7.shouldFireEquipment = false;
            aiskillDriver7.shouldSprint = false;
            aiskillDriver7.skillSlot = SkillSlot.None;

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
            aiskillDriver8.minDistance = 25f;
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
            aiskillDriver8.shouldSprint = false;
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

            if(MainPlugin.Config_Rework_Enable)
            {
                UtilitySkill.activationState = new SerializableEntityStateType(typeof(States.VoidMegaCrab.Recall_Rework));
                Modules.States.RegisterState(typeof(States.VoidMegaCrab.Recall_Rework));
            }
            else
            {
                UtilitySkill.activationState = new SerializableEntityStateType(typeof(States.VoidMegaCrab.Recall));
                Modules.States.RegisterState(typeof(States.VoidMegaCrab.Recall));
            }

            

            UtilitySkill.activationStateMachineName = "Body";
            UtilitySkill.dontAllowPastMaxStocks = false;

            UtilitySkill.resetCooldownTimerOnUse = false;
            UtilitySkill.keywordTokens = null;

            UtilitySkill.baseMaxStock = 1;
            UtilitySkill.rechargeStock = 1;
            UtilitySkill.requiredStock = 1;
            UtilitySkill.stockToConsume = 1;
            UtilitySkill.fullRestockOnAssign = true;

            UtilitySkill.baseRechargeInterval = MainPlugin.Config_VoidMegaCrab_RecallCooldown;
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
