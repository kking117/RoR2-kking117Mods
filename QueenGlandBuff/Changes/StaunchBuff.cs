using System;
using System.Collections.Generic;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace QueenGlandBuff.Changes
{
    public class StaunchBuff
	{
		public static BuffDef Staunching;
		private static float AggroChance = 1f;
		private static float BossAggroChance = 0.025f;
		private static float AggroRange = 100f;
		internal static void Begin()
		{
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Creating Staunch buff.");
			}
			AggroChance = MainPlugin.Config_Staunch_AggroChance.Value;
			BossAggroChance = MainPlugin.Config_Staunch_AggroBossChance.Value;
			AggroRange = MainPlugin.Config_Staunch_AggroRange.Value;

			BuffDef beetlebuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Beetle/bdBeetleJuice.asset").WaitForCompletion();
			BuffDef warcrybuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/TeamWarCry/bdTeamWarCry.asset").WaitForCompletion();
			Staunching = Modules.Buffs.AddNewBuff("Staunch", beetlebuff.iconSprite, warcrybuff.buffColor, false, false, false);

			On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
			On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
			RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
		}
		private static void CalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender)
			{
				if (sender.HasBuff(Staunching))
				{
					args.armorAdd += 100f;
				}
			}
		}
		private static void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
		{
			orig(self, buff);
			if (buff == Staunching)
			{
				TickStaunchBuff(self);
			}
		}
		private static void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
		{
			orig(self);
			if (MainPlugin.FixedTimer < 0)
			{
				if (self.HasBuff(Staunching))
				{
					TickStaunchBuff(self);
				}
			}
		}
		private static void TickStaunchBuff(CharacterBody self)
		{
			DrawAggro(self);
		}
		private static void DrawAggro(CharacterBody self)
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
			search.maxDistanceFilter = AggroRange;
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
		private static bool RollAggroChance(CharacterBody target)
		{
			float result = UnityEngine.Random.Range(0f, 1f);
			if (target.isBoss)
			{
				return BossAggroChance > result;
			}
			return AggroChance > result;
		}
	}
}
