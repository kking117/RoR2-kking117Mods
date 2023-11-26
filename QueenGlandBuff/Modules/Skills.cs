using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using RoR2.CharacterAI;

namespace QueenGlandBuff.Modules
{
    internal static class Skills
    {
		internal static List<SkillFamily> skillFamilies = new List<SkillFamily>();
		internal static List<SkillDef> skillDefs = new List<SkillDef>();
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
				//LoadoutAPI.AddSkillFamily(skillFamily);
				(skillFamily as ScriptableObject).name = prefab.name + "PrimaryFamily";
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.primary._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
			else if (slot == SkillSlot.Secondary)
			{
				skillLocator.secondary = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				//LoadoutAPI.AddSkillFamily(skillFamily);
				(skillFamily as ScriptableObject).name = prefab.name + "SecondaryFamily";
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.secondary._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
			else if (slot == SkillSlot.Utility)
			{
				skillLocator.utility = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				//LoadoutAPI.AddSkillFamily(skillFamily);
				(skillFamily as ScriptableObject).name = prefab.name + "UtilityFamily";
				skillFamily.variants = new SkillFamily.Variant[0];
				skillLocator.utility._skillFamily = skillFamily;
				skillFamilies.Add(skillFamily);
			}
			else if (slot == SkillSlot.Special)
			{
				skillLocator.special = prefab.AddComponent<GenericSkill>();
				SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
				//LoadoutAPI.AddSkillFamily(skillFamily);
				(skillFamily as ScriptableObject).name = prefab.name + "SpecialFamily";
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
			MainPlugin.print("activationRequiresAimConfirmation = " + copyme.activationRequiresAimConfirmation);
			MainPlugin.print("activationRequiresAimTargetLoS = " + copyme.activationRequiresAimTargetLoS);
			MainPlugin.print("activationRequiresTargetLoS = " + copyme.activationRequiresTargetLoS);
			MainPlugin.print("aimType = " + copyme.aimType);
			MainPlugin.print("buttonPressType = " + copyme.buttonPressType);
			MainPlugin.print("driverUpdateTimerOverride = " + copyme.driverUpdateTimerOverride);
			MainPlugin.print("ignoreNodeGraph = " + copyme.ignoreNodeGraph);
			MainPlugin.print("maxDistance = " + copyme.maxDistance);
			MainPlugin.print("maxTargetHealthFraction = " + copyme.maxTargetHealthFraction);
			MainPlugin.print("maxUserHealthFractio = " + copyme.maxUserHealthFraction);
			MainPlugin.print("minDistance = " + copyme.minDistance);
			MainPlugin.print("minTargetHealthFraction = " + copyme.minTargetHealthFraction);
			MainPlugin.print("minUserHealthFraction = " + copyme.minUserHealthFraction);
			MainPlugin.print("moveInputScale = " + copyme.moveInputScale);
			MainPlugin.print("movementType = " + copyme.movementType);
			MainPlugin.print("moveTargetType = " + copyme.moveTargetType);
			MainPlugin.print("nextHighPriorityOverride = " + copyme.nextHighPriorityOverride);
			MainPlugin.print("noRepeat = " + copyme.noRepeat);
			MainPlugin.print("requiredSkill = " + copyme.requiredSkill);
			MainPlugin.print("requireEquipmentReady = " + copyme.requireEquipmentReady);
			MainPlugin.print("requireSkillReady = " + copyme.requireSkillReady);
			MainPlugin.print("resetCurrentEnemyOnNextDriverSelection = " + copyme.resetCurrentEnemyOnNextDriverSelection);
			MainPlugin.print("selectionRequiresAimTarget = " + copyme.selectionRequiresAimTarget);
			MainPlugin.print("selectionRequiresOnGround = " + copyme.selectionRequiresOnGround);
			MainPlugin.print("selectionRequiresTargetLoS = " + copyme.selectionRequiresTargetLoS);
			MainPlugin.print("shouldFireEquipment = " + copyme.shouldFireEquipment);
			MainPlugin.print("shouldSprint = " + copyme.shouldSprint);
			MainPlugin.print("skillSlot = " + copyme.skillSlot);

			/*returnme.activationRequiresAimConfirmation = copyme.activationRequiresAimConfirmation;
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
			returnme.skillSlot = copyme.skillSlot;*/

			return returnme;
		}
	}
}
