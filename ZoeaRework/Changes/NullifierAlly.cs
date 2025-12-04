using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates;

namespace ZoeaRework.Changes
{
    public class NullifierAlly
    {
        public static GameObject AllyBodyObject;
        public static GameObject AllyMasterObject;
        public static SkillDef UtilitySkill;
        public static SkillDef SpecialSkill;
        public static CharacterSpawnCard AllySpawnCard;
        public static void Begin()
        {
            CreateUtilitySkill();
            CreateSpecialSkill();
            GatherEditAssets();
            UpdateAI();
        }
        private static void GatherEditAssets()
        {
            AllyMasterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierAllyMaster.prefab").WaitForCompletion();
            AllySpawnCard = UnityEngine.Object.Instantiate<CharacterSpawnCard>(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Nullifier/cscNullifierAlly.asset").WaitForCompletion());
            CharacterMaster master = AllyMasterObject.GetComponent<CharacterMaster>();
            if (master.bodyPrefab)
            {
                AllyBodyObject = master.bodyPrefab;
                Modules.Skills.AddSkillToSlot(AllyBodyObject, UtilitySkill, SkillSlot.Utility);
                Modules.Skills.AddSkillToSlot(AllyBodyObject, SpecialSkill, SkillSlot.Special);
                CharacterBody body = master.bodyPrefab.GetComponent<CharacterBody>();
                if (body)
                {
                    body.baseMoveSpeed = MainPlugin.Config_Nullifier_BaseSpeed;
                    body.baseMaxHealth *= 0.5f;
                    body.levelMaxHealth *= 0.5f;
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

            //Suicide Drivers
            AISkillDriver aiskillDriver1 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver1.customName = "SuicideDetonate";
            aiskillDriver1.activationRequiresAimConfirmation = false;
            aiskillDriver1.activationRequiresAimTargetLoS = false;
            aiskillDriver1.activationRequiresTargetLoS = false;
            aiskillDriver1.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver1.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            aiskillDriver1.driverUpdateTimerOverride = -1f;
            aiskillDriver1.ignoreNodeGraph = true;
            aiskillDriver1.maxDistance = 9f;
            aiskillDriver1.minDistance = 0f;
            aiskillDriver1.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver1.maxUserHealthFraction = 0.5f;
            aiskillDriver1.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver1.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver1.moveInputScale = 1f;
            aiskillDriver1.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver1.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver1.nextHighPriorityOverride =;
            aiskillDriver1.noRepeat = false;
            //aiskillDriver1.requiredSkill =;
            aiskillDriver1.requireEquipmentReady = false;
            aiskillDriver1.requireSkillReady = false;
            aiskillDriver1.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver1.selectionRequiresAimTarget = false;
            aiskillDriver1.selectionRequiresOnGround = false;
            aiskillDriver1.selectionRequiresTargetLoS = false;
            aiskillDriver1.shouldFireEquipment = false;
            aiskillDriver1.shouldSprint = true;
            aiskillDriver1.skillSlot = SkillSlot.Special;

            AISkillDriver aiskillDriver2 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver2.customName = "SuicideChaseClose";
            aiskillDriver2.activationRequiresAimConfirmation = false;
            aiskillDriver2.activationRequiresAimTargetLoS = false;
            aiskillDriver2.activationRequiresTargetLoS = false;
            aiskillDriver2.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver2.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            aiskillDriver2.driverUpdateTimerOverride = -1f;
            aiskillDriver2.ignoreNodeGraph = true;
            aiskillDriver2.maxDistance = 14f;
            aiskillDriver2.minDistance = 0f;
            aiskillDriver2.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver2.maxUserHealthFraction = 0.5f;
            aiskillDriver2.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver2.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver2.moveInputScale = 1f;
            aiskillDriver2.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver2.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver2.nextHighPriorityOverride =;
            aiskillDriver2.noRepeat = false;
            //aiskillDriver2.requiredSkill =;
            aiskillDriver2.requireEquipmentReady = false;
            aiskillDriver2.requireSkillReady = false;
            aiskillDriver2.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver2.selectionRequiresAimTarget = false;
            aiskillDriver2.selectionRequiresOnGround = false;
            aiskillDriver2.selectionRequiresTargetLoS = true;
            aiskillDriver2.shouldFireEquipment = false;
            aiskillDriver2.shouldSprint = true;
            aiskillDriver2.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver3 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver3.customName = "SuicideChaseNodeGraph";
            aiskillDriver3.activationRequiresAimConfirmation = false;
            aiskillDriver3.activationRequiresAimTargetLoS = false;
            aiskillDriver3.activationRequiresTargetLoS = false;
            aiskillDriver3.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver3.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            aiskillDriver3.driverUpdateTimerOverride = -1f;
            aiskillDriver3.ignoreNodeGraph = false;
            aiskillDriver3.maxDistance = 150f;
            aiskillDriver3.minDistance = 0f;
            aiskillDriver3.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver3.maxUserHealthFraction = 0.5f;
            aiskillDriver3.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver3.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver3.moveInputScale = 1f;
            aiskillDriver3.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver3.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver3.nextHighPriorityOverride =;
            aiskillDriver3.noRepeat = false;
            //aiskillDriver3.requiredSkill =;
            aiskillDriver3.requireEquipmentReady = false;
            aiskillDriver3.requireSkillReady = false;
            aiskillDriver3.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver3.selectionRequiresAimTarget = false;
            aiskillDriver3.selectionRequiresOnGround = false;
            aiskillDriver3.selectionRequiresTargetLoS = true;
            aiskillDriver3.shouldFireEquipment = false;
            aiskillDriver3.shouldSprint = true;
            aiskillDriver3.skillSlot = SkillSlot.Primary;

            //Combat Drivers
            AISkillDriver aiskillDriver4 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver4.customName = "PanicFireWhenClose";
            aiskillDriver4.activationRequiresAimConfirmation = false;
            aiskillDriver4.activationRequiresAimTargetLoS = false;
            aiskillDriver4.activationRequiresTargetLoS = false;
            aiskillDriver4.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver4.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            aiskillDriver4.driverUpdateTimerOverride = 3f;
            aiskillDriver4.ignoreNodeGraph = true;
            aiskillDriver4.maxDistance = 11f;
            aiskillDriver4.minDistance = 0f;
            aiskillDriver4.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver4.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver4.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver4.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver4.moveInputScale = 1f;
            aiskillDriver4.movementType = AISkillDriver.MovementType.FleeMoveTarget;
            aiskillDriver4.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver4.nextHighPriorityOverride =;
            aiskillDriver4.noRepeat = false;
            //aiskillDriver4.requiredSkill =;
            aiskillDriver4.requireEquipmentReady = false;
            aiskillDriver4.requireSkillReady = false;
            aiskillDriver4.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver4.selectionRequiresAimTarget = false;
            aiskillDriver4.selectionRequiresOnGround = false;
            aiskillDriver4.selectionRequiresTargetLoS = false;
            aiskillDriver4.shouldFireEquipment = false;
            aiskillDriver4.shouldSprint = false;
            aiskillDriver4.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver5 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver5.customName = "FireAndStrafe";
            aiskillDriver5.activationRequiresAimConfirmation = false;
            aiskillDriver5.activationRequiresAimTargetLoS = false;
            aiskillDriver5.activationRequiresTargetLoS = true;
            aiskillDriver5.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver5.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            aiskillDriver5.driverUpdateTimerOverride = -1f;
            aiskillDriver5.ignoreNodeGraph = false;
            aiskillDriver5.maxDistance = 75f;
            aiskillDriver5.minDistance = 11f;
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
            aiskillDriver5.selectionRequiresTargetLoS = false;
            aiskillDriver5.shouldFireEquipment = false;
            aiskillDriver5.shouldSprint = false;
            aiskillDriver5.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver7 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver7.customName = "ReturnToOwnerLeash";
            aiskillDriver7.activationRequiresAimConfirmation = false;
            aiskillDriver7.activationRequiresAimTargetLoS = false;
            aiskillDriver7.activationRequiresTargetLoS = false;
            aiskillDriver7.aimType = AISkillDriver.AimType.AtCurrentLeader;
            aiskillDriver7.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver7.driverUpdateTimerOverride = 3f;
            aiskillDriver7.ignoreNodeGraph = false;
            aiskillDriver7.maxDistance = float.PositiveInfinity;
            aiskillDriver7.minDistance = MainPlugin.Config_AIShared_MinRecallDist;
            aiskillDriver7.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver7.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver7.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver7.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver7.moveInputScale = 1f;
            aiskillDriver7.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver7.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            //aiskillDriver7.nextHighPriorityOverride =;
            aiskillDriver7.noRepeat = false;
            //aiskillDriver7.requiredSkill =;
            aiskillDriver7.requireEquipmentReady = false;
            aiskillDriver7.selectionRequiresAimTarget = false;
            aiskillDriver7.selectionRequiresOnGround = false;
            aiskillDriver7.selectionRequiresTargetLoS = false;
            aiskillDriver7.shouldFireEquipment = false;
            aiskillDriver7.shouldSprint = false;
            aiskillDriver7.requireSkillReady = false;
            aiskillDriver7.skillSlot = SkillSlot.Utility;
            aiskillDriver7.resetCurrentEnemyOnNextDriverSelection = false;

            AISkillDriver aiskillDriver9 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver9.customName = "FireAndChase";
            aiskillDriver9.activationRequiresAimConfirmation = false;
            aiskillDriver9.activationRequiresAimTargetLoS = false;
            aiskillDriver9.activationRequiresTargetLoS = true;
            aiskillDriver9.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver9.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            aiskillDriver9.driverUpdateTimerOverride = -1f;
            aiskillDriver9.ignoreNodeGraph = false;
            aiskillDriver9.maxDistance = 200f;
            aiskillDriver9.minDistance = 75f;
            aiskillDriver9.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver9.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver9.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver9.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver9.moveInputScale = 1f;
            aiskillDriver9.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            aiskillDriver9.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver9.nextHighPriorityOverride =;
            aiskillDriver9.noRepeat = false;
            //aiskillDriver9.requiredSkill =;
            aiskillDriver9.requireEquipmentReady = false;
            aiskillDriver9.requireSkillReady = true;
            aiskillDriver9.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver9.selectionRequiresAimTarget = false;
            aiskillDriver9.selectionRequiresOnGround = false;
            aiskillDriver9.selectionRequiresTargetLoS = true;
            aiskillDriver9.shouldFireEquipment = false;
            aiskillDriver9.shouldSprint = false;
            aiskillDriver9.skillSlot = SkillSlot.Primary;

            AISkillDriver aiskillDriver10 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver10.customName = "FollowNodeGraphToTarget";
            aiskillDriver10.activationRequiresAimConfirmation = false;
            aiskillDriver10.activationRequiresAimTargetLoS = false;
            aiskillDriver10.activationRequiresTargetLoS = false;
            aiskillDriver10.aimType = AISkillDriver.AimType.AtMoveTarget;
            aiskillDriver10.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver10.driverUpdateTimerOverride = -1f;
            aiskillDriver10.ignoreNodeGraph = false;
            aiskillDriver10.maxDistance = float.PositiveInfinity;
            aiskillDriver10.minDistance = 0f;
            aiskillDriver10.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver10.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver10.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver10.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver10.moveInputScale = 1f;
            aiskillDriver10.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver10.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //aiskillDriver10.nextHighPriorityOverride =;
            aiskillDriver10.noRepeat = false;
            //aiskillDriver10.requiredSkill =;
            aiskillDriver10.requireEquipmentReady = false;
            aiskillDriver10.requireSkillReady = false;
            aiskillDriver10.resetCurrentEnemyOnNextDriverSelection = false;
            aiskillDriver10.selectionRequiresAimTarget = false;
            aiskillDriver10.selectionRequiresOnGround = false;
            aiskillDriver10.selectionRequiresTargetLoS = true;
            aiskillDriver10.shouldFireEquipment = false;
            aiskillDriver10.shouldSprint = false;
            aiskillDriver10.skillSlot = SkillSlot.None;

            //Follow Leader Drivers

            AISkillDriver aiskillDriver11 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver11.customName = "ReturnToLeaderDefault";
            aiskillDriver11.activationRequiresAimConfirmation = false;
            aiskillDriver11.activationRequiresAimTargetLoS = false;
            aiskillDriver11.activationRequiresTargetLoS = false;
            aiskillDriver11.aimType = AISkillDriver.AimType.MoveDirection;
            aiskillDriver11.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver11.driverUpdateTimerOverride = -1f;
            aiskillDriver11.ignoreNodeGraph = false;
            aiskillDriver11.maxDistance = float.PositiveInfinity;
            aiskillDriver11.minDistance = 20f;
            aiskillDriver11.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver11.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver11.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver11.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver11.moveInputScale = 1f;
            aiskillDriver11.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            aiskillDriver11.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            //aiskillDriver11.nextHighPriorityOverride =;
            aiskillDriver11.noRepeat = false;
            //aiskillDriver11.requiredSkill =;
            aiskillDriver11.requireEquipmentReady = false;
            aiskillDriver11.requireSkillReady = false;
            aiskillDriver11.resetCurrentEnemyOnNextDriverSelection = true;
            aiskillDriver11.selectionRequiresAimTarget = false;
            aiskillDriver11.selectionRequiresOnGround = false;
            aiskillDriver11.selectionRequiresTargetLoS = false;
            aiskillDriver11.shouldFireEquipment = false;
            aiskillDriver11.shouldSprint = false;
            aiskillDriver11.skillSlot = SkillSlot.None;

            AISkillDriver aiskillDriver12 = AllyMasterObject.AddComponent<AISkillDriver>();
            aiskillDriver12.customName = "WaitNearLeaderDefault";
            aiskillDriver12.activationRequiresAimConfirmation = false;
            aiskillDriver12.activationRequiresAimTargetLoS = false;
            aiskillDriver12.activationRequiresTargetLoS = false;
            aiskillDriver12.aimType = AISkillDriver.AimType.MoveDirection;
            aiskillDriver12.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            aiskillDriver12.driverUpdateTimerOverride = -1f;
            aiskillDriver12.ignoreNodeGraph = false;
            aiskillDriver12.maxDistance = float.PositiveInfinity;
            aiskillDriver12.minDistance = 0f;
            aiskillDriver12.maxTargetHealthFraction = float.PositiveInfinity;
            aiskillDriver12.maxUserHealthFraction = float.PositiveInfinity;
            aiskillDriver12.minTargetHealthFraction = float.NegativeInfinity;
            aiskillDriver12.minUserHealthFraction = float.NegativeInfinity;
            aiskillDriver12.moveInputScale = 1f;
            aiskillDriver12.movementType = AISkillDriver.MovementType.Stop;
            aiskillDriver12.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            //aiskillDriver12.nextHighPriorityOverride =;
            aiskillDriver12.noRepeat = false;
            //aiskillDriver12.requiredSkill =;
            aiskillDriver12.requireEquipmentReady = false;
            aiskillDriver12.requireSkillReady = false;
            aiskillDriver12.resetCurrentEnemyOnNextDriverSelection = true;
            aiskillDriver12.selectionRequiresAimTarget = false;
            aiskillDriver12.selectionRequiresOnGround = false;
            aiskillDriver12.selectionRequiresTargetLoS = false;
            aiskillDriver12.shouldFireEquipment = false;
            aiskillDriver12.shouldSprint = false;
            aiskillDriver12.skillSlot = SkillSlot.None;
        }
        private static void CreateUtilitySkill()
        {
            UtilitySkill = ScriptableObject.CreateInstance<SkillDef>();

            UtilitySkill.activationState = new SerializableEntityStateType(typeof(States.Nullifier.Recall));
            Modules.States.RegisterState(typeof(States.Nullifier.Recall));

            UtilitySkill.activationStateMachineName = "Body";
            UtilitySkill.dontAllowPastMaxStocks = false;

            UtilitySkill.resetCooldownTimerOnUse = false;
            UtilitySkill.keywordTokens = null;

            UtilitySkill.baseMaxStock = 1;
            UtilitySkill.rechargeStock = 1;
            UtilitySkill.requiredStock = 1;
            UtilitySkill.stockToConsume = 1;
            UtilitySkill.fullRestockOnAssign = true;

            UtilitySkill.baseRechargeInterval = MainPlugin.Config_Nullifier_RecallCooldown;
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

        private static void CreateSpecialSkill()
        {
            LanguageAPI.Add(MainPlugin.MODTOKEN + "SPECIAL_DETONATE_NAME", "Detonate");
            LanguageAPI.Add(MainPlugin.MODTOKEN + "SPECIAL_DETONATE_DESC", "Kill yourself.");

            SpecialSkill = ScriptableObject.CreateInstance<SkillDef>();

            SpecialSkill.activationState = new SerializableEntityStateType(typeof(States.Nullifier.SelfDetonate));
            Modules.States.RegisterState(typeof(States.Nullifier.SelfDetonate));

            SpecialSkill.activationStateMachineName = "Body";
            SpecialSkill.dontAllowPastMaxStocks = false;

            SpecialSkill.resetCooldownTimerOnUse = false;
            SpecialSkill.keywordTokens = null;

            SpecialSkill.baseMaxStock = 1;
            SpecialSkill.rechargeStock = 1;
            SpecialSkill.requiredStock = 1;
            SpecialSkill.stockToConsume = 1;
            SpecialSkill.fullRestockOnAssign = true;

            SpecialSkill.baseRechargeInterval = 3f;
            SpecialSkill.beginSkillCooldownOnSkillEnd = false;

            SpecialSkill.canceledFromSprinting = false;
            SpecialSkill.cancelSprintingOnActivation = true;
            SpecialSkill.forceSprintDuringState = false;

            SpecialSkill.interruptPriority = InterruptPriority.Any;
            SpecialSkill.isCombatSkill = true;
            SpecialSkill.mustKeyPress = false;

            SpecialSkill.icon = null;
            SpecialSkill.skillDescriptionToken = MainPlugin.MODTOKEN + "SPECIAL_DETONATE_DESC";
            SpecialSkill.skillNameToken = MainPlugin.MODTOKEN + "SPECIAL_DETONATE_NAME";
            SpecialSkill.skillName = SpecialSkill.skillNameToken;

            Modules.Skills.RegisterSkill(SpecialSkill);
        }
    }
}
