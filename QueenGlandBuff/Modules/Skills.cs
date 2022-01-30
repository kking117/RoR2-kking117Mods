using System;
using System.Collections.Generic;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using RoR2.CharacterAI;
using EntityStates;

namespace QueenGlandBuff.Modules
{
    internal static class Skills
    {
		internal static List<SkillFamily> skillFamilies = new List<SkillFamily>();
		internal static List<SkillDef> skillDefs = new List<SkillDef>();

		public static SkillDef SlamSkill;
		public static SkillDef SunderSkill;
		public static SkillDef RecallSkill;
		public static SkillDef StaunchSkill;

		internal static void RegisterSkills()
        {
			CreateSlamSkill();
			CreateSunderSkill();
			CreateRecallSkill();
			CreateStaunchSkill();
		}
		internal static void CreateSlamSkill()
		{
			SlamSkill = ScriptableObject.CreateInstance<SkillDef>();

			LanguageAPI.Add(Main.ModLangToken + "PRIMARY_SLAM_NAME", "Slam");
			if (Main.Gland_PrimaryBuff.Value)
			{
				SlamSkill.activationState = new SerializableEntityStateType(typeof(QueenGlandBuff.States.Slam));
				LanguageAPI.Add(Main.ModLangToken + "PRIMARY_SLAM_DESC", "Strike the ground for <style=cIsDamage>400%</style> and launch debris for <style=cIsDamage>5x75%</style> damage.");
			}
			else
			{
				SlamSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.BeetleGuardMonster.GroundSlam));
				LanguageAPI.Add(Main.ModLangToken + "PRIMARY_SLAM_DESC", "Strike the ground for <style=cIsDamage>400%</style> damage.");
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
			SlamSkill.skillDescriptionToken = Main.ModLangToken + "PRIMARY_SLAM_DESC";
			SlamSkill.skillNameToken = Main.ModLangToken + "PRIMARY_SLAM_NAME";
			SlamSkill.skillName = SlamSkill.skillNameToken;

			RegisterSkill(SlamSkill);
		}
		internal static void CreateSunderSkill()
		{
			SunderSkill = ScriptableObject.CreateInstance<SkillDef>();

			LanguageAPI.Add(Main.ModLangToken + "SECONDARY_SUNDER_NAME", "Sunder");
			if (Main.Gland_SecondaryBuff.Value)
			{
				SunderSkill.activationState = new SerializableEntityStateType(typeof(QueenGlandBuff.States.Sunder));
				LanguageAPI.Add(Main.ModLangToken + "SECONDARY_SUNDER_DESC", "Tear the ground infront of you for <style=cIsDamage>400%</style>, launching a cluster of rocks for <style=cIsDamage>5x75%</style> damage.");
			}
			else
			{
				SunderSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.BeetleGuardMonster.FireSunder));
				LanguageAPI.Add(Main.ModLangToken + "SECONDARY_SUNDER_DESC", "Tear the ground infront of you for <style=cIsDamage>400%</style> damage.");
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
			SunderSkill.skillDescriptionToken = Main.ModLangToken + "SECONDARY_SUNDER_DESC";
			SunderSkill.skillNameToken = Main.ModLangToken + "SECONDARY_SUNDER_NAME";
			SunderSkill.skillName = SunderSkill.skillNameToken;

			RegisterSkill(SunderSkill);
		}
		internal static void CreateRecallSkill()
		{
			RecallSkill = ScriptableObject.CreateInstance<SkillDef>();

			LanguageAPI.Add(Main.ModLangToken + "UTILITY_TELEPORT_NAME", "Recall");
			LanguageAPI.Add(Main.ModLangToken + "UTILITY_TELEPORT_DESC", "<style=cIsUtility>Burrow</style> to your owner's side. Enter a <style=cIsDamage>frenzy</style> for <style=cIsUtility>10</style> seconds if you have no owner.");

			RecallSkill.activationState = new SerializableEntityStateType(typeof(QueenGlandBuff.States.Recall));
			RecallSkill.activationStateMachineName = "Body";

			RecallSkill.baseMaxStock = 1;
			RecallSkill.rechargeStock = 1;
			RecallSkill.requiredStock = 1;
			RecallSkill.stockToConsume = 1;
			RecallSkill.fullRestockOnAssign = true;

			RecallSkill.baseRechargeInterval = 10f;
			RecallSkill.beginSkillCooldownOnSkillEnd = false;

			RecallSkill.canceledFromSprinting = false;
			RecallSkill.cancelSprintingOnActivation = true;
			RecallSkill.forceSprintDuringState = false;

			RecallSkill.interruptPriority = InterruptPriority.Skill;
			RecallSkill.isCombatSkill = false;
			RecallSkill.mustKeyPress = false;

			RecallSkill.icon = null;
			RecallSkill.skillDescriptionToken = Main.ModLangToken + "UTILITY_TELEPORT_DESC";
			RecallSkill.skillNameToken = Main.ModLangToken + "UTILITY_TELEPORT_NAME";
			RecallSkill.skillName = RecallSkill.skillNameToken;

			Modules.Skills.RegisterSkill(RecallSkill);
		}
		internal static void CreateStaunchSkill()
		{
			StaunchSkill = ScriptableObject.CreateInstance<SkillDef>();
			LanguageAPI.Add(Main.ModLangToken + "SPECIAL_TAUNT_NAME", "Staunch");
			LanguageAPI.Add(Main.ModLangToken + "SPECIAL_TAUNT_DESC", "Draw the <style=cIsHealth>attention</style> of nearby enemies for <style=cIsUtility>10</style> seconds. While active gain <style=cIsUtility>100</style> armor and send nearby <style=cIsHealing>friendly beetles</style> into a <style=cIsDamage>frenzy</style>.");

			StaunchSkill.activationState = new SerializableEntityStateType(typeof(QueenGlandBuff.States.Staunch));
			StaunchSkill.activationStateMachineName = "Body";

			StaunchSkill.baseMaxStock = 1;
			StaunchSkill.rechargeStock = 1;
			StaunchSkill.requiredStock = 1;
			StaunchSkill.stockToConsume = 1;
			StaunchSkill.fullRestockOnAssign = true;

			StaunchSkill.baseRechargeInterval = 40f;
			StaunchSkill.beginSkillCooldownOnSkillEnd = false;

			StaunchSkill.canceledFromSprinting = false;
			StaunchSkill.cancelSprintingOnActivation = true;
			StaunchSkill.forceSprintDuringState = false;

			StaunchSkill.interruptPriority = InterruptPriority.Skill;
			StaunchSkill.isCombatSkill = false;
			StaunchSkill.mustKeyPress = false;

			StaunchSkill.icon = null;
			StaunchSkill.skillDescriptionToken = Main.ModLangToken + "SPECIAL_TAUNT_DESC";
			StaunchSkill.skillNameToken = Main.ModLangToken + "SPECIAL_TAUNT_NAME";
			StaunchSkill.skillName = StaunchSkill.skillNameToken;

			RegisterSkill(StaunchSkill);
		}
		internal static void RegisterSkill(SkillDef skill)
        {
			skillDefs.Add(skill);
        }
		internal static void AddSkillToSlot(GameObject prefab, SkillDef skill, SkillSlot slot)
		{
			SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();
			SkillFamily skillFamily = null;
			if (slot == SkillSlot.Primary)
			{
				if (!skillLocator.primary)
				{
					AddSkillFamilyToSlot(prefab, skillLocator, slot);
				}
				skillFamily = skillLocator.primary.skillFamily;
			}
			else if (slot == SkillSlot.Secondary)
			{
				if (!skillLocator.secondary)
				{
					AddSkillFamilyToSlot(prefab, skillLocator, slot);
				}
				skillFamily = skillLocator.secondary.skillFamily;
			}
			else if (slot == SkillSlot.Utility)
			{
				if (!skillLocator.utility)
				{
					AddSkillFamilyToSlot(prefab, skillLocator, slot);
				}
				skillFamily = skillLocator.utility.skillFamily;
			}
			else if (slot == SkillSlot.Special)
			{
				if (!skillLocator.special)
				{
					AddSkillFamilyToSlot(prefab, skillLocator, slot);
				}
				skillFamily = skillLocator.special.skillFamily;
			}
			if (skillFamily)
			{
				Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
				skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
				{
					skillDef = skill,
					viewableNode = new ViewablesCatalog.Node(skill.skillNameToken, false, null)
				};
			}
		}
		internal static void AddSkillFamilyToSlot(GameObject prefab, SkillLocator skillLocator, SkillSlot slot)
		{
			if (slot == SkillSlot.Primary)
			{
				skillLocator.primary = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				LoadoutAPI.AddSkillFamily(skillFamily);
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.primary._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
			else if (slot == SkillSlot.Secondary)
			{
				skillLocator.secondary = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				LoadoutAPI.AddSkillFamily(skillFamily);
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.secondary._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
			else if (slot == SkillSlot.Utility)
			{
				skillLocator.utility = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				LoadoutAPI.AddSkillFamily(skillFamily);
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.utility._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
			else if (slot == SkillSlot.Special)
			{
				skillLocator.special = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				LoadoutAPI.AddSkillFamily(skillFamily);
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.special._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
		}

		internal static void WipeLoadout(GameObject prefab)
		{
			foreach (GenericSkill obj in prefab.GetComponentsInChildren<GenericSkill>())
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();
		}
		internal static AISkillDriver CopySkillDriver(AISkillDriver returnme, AISkillDriver copyme)
		{
			Main.PrintMe("activationRequiresAimConfirmation = " + copyme.activationRequiresAimConfirmation);
			Main.PrintMe("activationRequiresAimTargetLoS = " + copyme.activationRequiresAimTargetLoS);
			Main.PrintMe("activationRequiresTargetLoS = " + copyme.activationRequiresTargetLoS);
			Main.PrintMe("aimType = " + copyme.aimType);
			Main.PrintMe("buttonPressType = " + copyme.buttonPressType);
			Main.PrintMe("driverUpdateTimerOverride = " + copyme.driverUpdateTimerOverride);
			Main.PrintMe("ignoreNodeGraph = " + copyme.ignoreNodeGraph);
			Main.PrintMe("maxDistance = " + copyme.maxDistance);
			Main.PrintMe("maxTargetHealthFraction = " + copyme.maxTargetHealthFraction);
			Main.PrintMe("maxUserHealthFractio = " + copyme.maxUserHealthFraction);
			Main.PrintMe("minDistance = " + copyme.minDistance);
			Main.PrintMe("minTargetHealthFraction = " + copyme.minTargetHealthFraction);
			Main.PrintMe("minUserHealthFraction = " + copyme.minUserHealthFraction);
			Main.PrintMe("moveInputScale = " + copyme.moveInputScale);
			Main.PrintMe("movementType = " + copyme.movementType);
			Main.PrintMe("moveTargetType = " + copyme.moveTargetType);
			Main.PrintMe("nextHighPriorityOverride = " + copyme.nextHighPriorityOverride);
			Main.PrintMe("noRepeat = " + copyme.noRepeat);
			Main.PrintMe("requiredSkill = " + copyme.requiredSkill);
			Main.PrintMe("requireEquipmentReady = " + copyme.requireEquipmentReady);
			Main.PrintMe("requireSkillReady = " + copyme.requireSkillReady);
			Main.PrintMe("resetCurrentEnemyOnNextDriverSelection = " + copyme.resetCurrentEnemyOnNextDriverSelection);
			Main.PrintMe("selectionRequiresAimTarget = " + copyme.selectionRequiresAimTarget);
			Main.PrintMe("selectionRequiresOnGround = " + copyme.selectionRequiresOnGround);
			Main.PrintMe("selectionRequiresTargetLoS = " + copyme.selectionRequiresTargetLoS);
			Main.PrintMe("shouldFireEquipment = " + copyme.shouldFireEquipment);
			Main.PrintMe("shouldSprint = " + copyme.shouldSprint);
			Main.PrintMe("skillSlot = " + copyme.skillSlot);

			returnme.activationRequiresAimConfirmation = copyme.activationRequiresAimConfirmation;
			returnme.activationRequiresAimTargetLoS = copyme.activationRequiresAimTargetLoS;
			returnme.activationRequiresTargetLoS = copyme.activationRequiresTargetLoS;
			returnme.aimType = copyme.aimType;
			returnme.buttonPressType = copyme.buttonPressType;
			returnme.customName = copyme.customName;
			returnme.driverUpdateTimerOverride = copyme.driverUpdateTimerOverride;
			returnme.ignoreNodeGraph = copyme.ignoreNodeGraph;
			returnme.maxDistance = copyme.maxDistance;
			returnme.maxTargetHealthFraction = copyme.maxTargetHealthFraction;
			returnme.maxUserHealthFraction = copyme.maxUserHealthFraction;
			returnme.minDistance = copyme.minDistance;
			returnme.minTargetHealthFraction = copyme.minTargetHealthFraction;
			returnme.minUserHealthFraction = copyme.minUserHealthFraction;
			returnme.moveInputScale = copyme.moveInputScale;
			returnme.movementType = copyme.movementType;
			returnme.moveTargetType = copyme.moveTargetType;
			returnme.nextHighPriorityOverride = copyme.nextHighPriorityOverride;
			returnme.noRepeat = copyme.noRepeat;
			returnme.requiredSkill = copyme.requiredSkill;
			returnme.requireEquipmentReady = copyme.requireEquipmentReady;
			returnme.requireSkillReady = copyme.requireSkillReady;
			returnme.resetCurrentEnemyOnNextDriverSelection = copyme.resetCurrentEnemyOnNextDriverSelection;
			returnme.selectionRequiresAimTarget = copyme.selectionRequiresAimTarget;
			returnme.selectionRequiresOnGround = copyme.selectionRequiresOnGround;
			returnme.selectionRequiresTargetLoS = copyme.selectionRequiresTargetLoS;
			returnme.shouldFireEquipment = copyme.shouldFireEquipment;
			returnme.shouldSprint = copyme.shouldSprint;
			returnme.skillSlot = copyme.skillSlot;

			return returnme;
		}
	}
}
