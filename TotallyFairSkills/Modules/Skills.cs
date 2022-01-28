using System;
using System.Collections.Generic;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace TotallyFairSkills.Modules
{
	internal static class Skills
	{
		internal static List<SkillFamily> skillFamilies = new List<SkillFamily>();
		internal static List<SkillDef> skillDefs = new List<SkillDef>();

		internal static SkillDef ShowTimeSkill;
		internal static SkillDef FMJMK2Skill;
		internal static void RegisterSkills()
		{
			if (Main.FMJMK2_Enable.Value)
			{
				CreateFMJMK2Skill();
			}
			if (Main.ShowTime_Enable.Value)
			{
				CreateShowTimeSkill();
			}
		}
		private static void CreateFMJMK2Skill()
		{
			FMJMK2Skill = ScriptableObject.CreateInstance<SkillDef>();
			LanguageAPI.Add(Main.MODTOKEN + "SECONDARY_FMJ_NAME", "FMJ Mk.II");
			LanguageAPI.Add(Main.MODTOKEN + "SECONDARY_FMJ_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Shoot <style=cIsDamage>through enemies</style> for <style=cIsDamage>" + Main.FMJMK2_Damage.Value * 100f + "%</style> damage, knocking them back.");

			FMJMK2Skill.skillNameToken = Main.MODTOKEN + "SECONDARY_FMJ_NAME";
			FMJMK2Skill.skillName = FMJMK2Skill.skillNameToken;
			FMJMK2Skill.skillDescriptionToken = Main.MODTOKEN + "SECONDARY_FMJ_DESCRIPTION";
			FMJMK2Skill.keywordTokens = new string[]
			{
				"KEYWORD_STUNNING"
			};
			FMJMK2Skill.icon = Resources.Load<Sprite>("textures/achievementicons/texmercxskillsinysecondsicon");

			FMJMK2Skill.activationState = new SerializableEntityStateType(typeof(TotallyFairSkills.States.FMJMK2));
			FMJMK2Skill.activationStateMachineName = "Weapon";
			FMJMK2Skill.interruptPriority = InterruptPriority.Skill;

			FMJMK2Skill.baseMaxStock = 1;
			FMJMK2Skill.rechargeStock = 1;
			FMJMK2Skill.requiredStock = 1;
			FMJMK2Skill.stockToConsume = 1;
			FMJMK2Skill.baseRechargeInterval = Main.FMJMK2_Cooldown.Value;

			FMJMK2Skill.isCombatSkill = true;
			FMJMK2Skill.forceSprintDuringState = false;
			FMJMK2Skill.canceledFromSprinting = false;
			FMJMK2Skill.cancelSprintingOnActivation = true;

			RegisterSkill(FMJMK2Skill);
		}
		private static void CreateShowTimeSkill()
        {
			Sprite icon = Resources.Load<Sprite>("textures/achievementicons/texmercxskillsinysecondsicon");
			SkillLocator skills = Resources.Load<GameObject>("prefabs/characterbodies/toolbotbody").GetComponent<SkillLocator>();
			RoR2.Skills.SkillFamily primary_skills = skills.special.skillFamily;
			if (primary_skills)
			{
				RoR2.Skills.SkillDef skill = primary_skills.variants[0].skillDef;
				if (skill)
				{
					icon = skill.icon;
				}
			}
			LanguageAPI.Add(Main.MODTOKEN + "KEYWORD_SHOWOFF", "<style=cKeywordName>Show-Off</style><style=cSub><style=cIsDamage>+" + Main.ShowOff_Crit.Value + "%</style> crit chance and <style=cIsHealing>+" + Main.ShowOff_Luck.Value + "</style> luck.\nExcess crit chance up to " + Main.ShowOff_ExcessCritCap.Value + "% increases <style=cIsDamage>damage</style>.\n<style=cIsHealth>Taking a heavy hit</style> will cause <style=cIsHealth>critical damage</style> and <style=cIsHealth>cancel this effect</style></style>.");

			ShowTimeSkill = ScriptableObject.CreateInstance<SkillDef>();
			LanguageAPI.Add(Main.MODTOKEN + "UTILITY_SHOWTIME_NAME", "Showtime");
			LanguageAPI.Add(Main.MODTOKEN + "UTILITY_SHOWTIME_DESCRIPTION", "<style=cIsUtility>Show-Off</style>. <style=cIsUtility>Dash</style> a short distance and <style=cIsDamage>reload all other skills</style>.");

			ShowTimeSkill.skillNameToken = Main.MODTOKEN + "UTILITY_SHOWTIME_NAME";
			ShowTimeSkill.skillName = ShowTimeSkill.skillNameToken;
			ShowTimeSkill.skillDescriptionToken = Main.MODTOKEN + "UTILITY_SHOWTIME_DESCRIPTION";
			ShowTimeSkill.keywordTokens = new string[]
			{
				Main.MODTOKEN + "KEYWORD_SHOWOFF"
			};
			ShowTimeSkill.icon = icon;

			ShowTimeSkill.activationState = new SerializableEntityStateType(typeof(TotallyFairSkills.States.ShowTime));
			ShowTimeSkill.activationStateMachineName = "Weapon";
			ShowTimeSkill.interruptPriority = InterruptPriority.Skill;

			ShowTimeSkill.baseMaxStock = 1;
			ShowTimeSkill.rechargeStock = 1;
			ShowTimeSkill.requiredStock = 1;
			ShowTimeSkill.stockToConsume = 1;
			ShowTimeSkill.baseRechargeInterval = Main.ShowTime_Cooldown.Value;

			ShowTimeSkill.isCombatSkill = false;
			ShowTimeSkill.forceSprintDuringState = false;
			ShowTimeSkill.canceledFromSprinting = false;
			ShowTimeSkill.cancelSprintingOnActivation = false;

			RegisterSkill(ShowTimeSkill);
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
	}
}