using System;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace QueenGlandBuff.Changes
{
    public class ValorBuff
	{
		public static BuffDef ThisBuffDef;
		private static Color BuffColor = new Color(0.827f, 0.196f, 0.098f, 1f);
		internal static float Aggro_Range = 60f;
		internal static float Buff_Armor = 100f;
		internal static void Begin()
		{
			MainPlugin.ModLogger.LogInfo("Creating Valor buff.");
			CreateBuff();
			if (Aggro_Range > 0f)
			{
				On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
			}
			RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
		}
		private static void CreateBuff()
        {
			//"RoR2/Base/BeetleGroup/bdBeetleJuice.asset"
			ThisBuffDef = Modules.Buffs.AddNewBuff("Valor", Addressables.LoadAssetAsync<BuffDef>("60b10a34c45065e4f8ee877e3c306f56").WaitForCompletion().iconSprite, BuffColor, false, false, false);
		}
		private static void CalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender)
			{
				if (sender.HasBuff(ThisBuffDef))
				{
					args.armorAdd += Buff_Armor;
				}
			}
		}
		private static void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
		{
			orig(self, buff);
			if (buff == ThisBuffDef)
			{
				StealAggro(self);
			}
		}
		private static void StealAggro(CharacterBody self)
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
			search.maxDistanceFilter = Aggro_Range;
			search.searchOrigin = self.inputBank.aimOrigin;
			search.searchDirection = self.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = true;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				if (target && target.healthComponent)
				{
					CharacterBody targetbody = target.healthComponent.body;
					if (targetbody)
					{
						if (!targetbody.isBoss && targetbody.master)
						{
							CharacterMaster targetmaster = target.healthComponent.body.master;
							BaseAI targetai = targetmaster.GetComponent<BaseAI>();
							if (targetai)
							{
								if (!targetai.isHealer)
								{
									targetai.currentEnemy.gameObject = self.healthComponent.gameObject;
									targetai.currentEnemy.bestHurtBox = self.mainHurtBox;
									targetai.enemyAttention = Math.Min(9f, targetai.enemyAttentionDuration);
									targetai.targetRefreshTimer = targetai.enemyAttention;
								}
							}
						}
					}
				}
			}
		}
	}
}
