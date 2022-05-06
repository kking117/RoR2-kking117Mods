using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace QueenGlandBuff.Changes
{
    public class QueensGland
    {
		internal static void Begin()
        {
			BeetleGuardAlly.Begin();
			UpdateItemDescription();
			QueensGlandHooks.Begin();
		}
		private static void UpdateItemDescription()
		{
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing Queen's Gland descriptions.");
			}
			string pickup = "Recruit ";
			string desc = "<style=cIsUtility>Summon ";
			if (MainPlugin.Config_SpawnAffix.Value == 1)
			{
				pickup += "an Elite Beetle Guard.";
				desc += "an Elite Beetle Guard</style>";
			}
			else
			{
				pickup += "a Beetle Guard.";
				desc += "a Beetle Guard</style>";
			}
			desc += " with <style=cIsHealing>" + (10 + MainPlugin.Config_BaseHealth.Value) * 10 + "% health</style>";
			desc += " and <style=cIsDamage>" + (10 + MainPlugin.Config_BaseDamage.Value) * 10 + "% damage</style>.";
			desc += " Can have <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> total Guards, up to <style=cIsUtility>" + MainPlugin.Config_MaxSummons.Value + "</style>.";
			if(MainPlugin.Config_StackHealth.Value != 0 || MainPlugin.Config_StackDamage.Value != 0)
            {
				desc += " Further stacks give";
				if(MainPlugin.Config_StackHealth.Value != 0)
                {
					desc += " <style=cStack>+" + MainPlugin.Config_StackHealth.Value * 10 + "%</style> <style=cIsHealing>health</style>";
				}
				if (MainPlugin.Config_StackDamage.Value != 0)
				{
					desc += " and <style=cStack>+" + MainPlugin.Config_StackDamage.Value * 10 + "%</style> <style=cIsDamage>damage</style>.";
				}
			}
			LanguageAPI.Add("ITEM_BEETLEGLAND_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BEETLEGLAND_DESC", desc);
		}
		internal static void UpdateAILeash(CharacterMaster master)
		{
			if (master)
			{
				foreach (AISkillDriver driver in master.GetComponentsInChildren<AISkillDriver>())
				{
					if (driver.customName == "ReturnToOwnerLeash")
					{
						driver.minDistance = GetLeashDistance();
						break;
					}
				}
			}
		}
		internal static float GetLeashDistance()
		{
			Run run = Run.instance;
			float distance = MainPlugin.Config_AI_MinRecallDist.Value;
			if (run)
			{
				float diff = (run.difficultyCoefficient - 1f) * MainPlugin.Config_AI_RecallDistDiff.Value;
				if (diff > 0f)
				{
					distance += diff;
				}
				distance = Mathf.Min(MainPlugin.Config_AI_MaxRecallDist.Value, distance);
				distance = Mathf.Max(MainPlugin.Config_AI_MinRecallDist.Value, distance);
			}
			//MainPlugin.ModLogger.LogInfo("Recall distance: " + distance + "m.");
			return distance;
		}
		internal static void TickStaunchBuff(CharacterBody self)
		{
			DrawAggro(self);
			EmpowerBeetles(self);
		}
		internal static void DrawAggro(CharacterBody self)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = self;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(self.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = MainPlugin.Config_Staunch_AggroRange.Value;
			search.searchOrigin = self.inputBank.aimOrigin;
			search.searchDirection = self.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = true;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				if (target && target.healthComponent)
				{
					if (target.healthComponent.body)
					{
						CharacterBody targetbody = target.healthComponent.body;
						if (targetbody.master)
						{
							CharacterMaster targetmaster = target.healthComponent.body.master;
							BaseAI targetai = targetmaster.GetComponent<BaseAI>();
							if (targetai)
							{
								if (!targetai.isHealer)
								{
									if (!targetai.currentEnemy.gameObject || targetai.enemyAttention <= 0f)
									{
										if (RollAggroChance(targetbody))
										{
											targetai.currentEnemy.gameObject = self.gameObject;
											targetai.currentEnemy.bestHurtBox = self.mainHurtBox;
											targetai.enemyAttention = targetai.enemyAttentionDuration;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		internal static void EmpowerBeetles(CharacterBody self)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = self;
			search.teamMaskFilter = TeamMask.none;
			search.teamMaskFilter.AddTeam(self.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = MainPlugin.Config_Staunch_AggroRange.Value;
			search.searchOrigin = self.inputBank.aimOrigin;
			search.searchDirection = self.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = false;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				if (target && target.healthComponent)
				{
					CharacterBody targetBody = target.healthComponent.body;
					if (targetBody)
					{
						if (targetBody != self)
						{
							if (QueensGlandHooks.BeetleFrenzyWhiteList.Contains(targetBody.bodyIndex))
							{
								targetBody.AddTimedBuff(BeetleGuardAlly.BeetleFrenzy, 1.5f);
							}
						}
					}
				}
			}
		}
		private static bool RollAggroChance(CharacterBody target)
		{
			float result = UnityEngine.Random.Range(0f, 1f);
			if (target.isBoss)
			{
				return MainPlugin.Config_Staunch_AggroBossChance.Value > result;
			}
			return MainPlugin.Config_Staunch_AggroChance.Value > result;
		}
	}
}