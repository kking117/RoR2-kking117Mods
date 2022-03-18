using System;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using QueenGlandBuff.Utils;

namespace QueenGlandBuff.ItemChanges
{
    public class QueensGlandHooks
    {
		public static void Begin()
		{
			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
			if (MainPlugin.Gland_AI_Target.Value)
			{
				On.RoR2.CharacterAI.BaseAI.FixedUpdate += BaseAI_FixedUpdate;
			}
			if (MainPlugin.Gland_AddSpecial.Value)
			{
				On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
				On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
				RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
			}
			On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

			BeetleGland_Override();
		}
		private static void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
		{
			orig(self);
			if (MainPlugin.FixedTimer < 0)
			{
				if (self.HasBuff(QueensGland.Staunching))
				{
					Helpers.DrawAggro(self);
					Helpers.EmpowerBeetles(self);
				}
			}
		}
		private static void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buff)
		{
			orig(self, buff);
			if (buff == QueensGland.Staunching)
			{
				if (NetworkServer.active)
				{
					Helpers.DrawAggro(self);
					Helpers.EmpowerBeetles(self);
				}
			}
		}
		private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			if (NetworkServer.active)
			{
				if (self.master)
				{
					MinionOwnership ownership = self.master.minionOwnership;
					if (ownership)
					{
						CharacterMaster owner = ownership.ownerMaster;
						if (owner)
						{
							if (Helpers.DoesMasterHaveDeployable(owner, DeployableSlot.BeetleGuardAlly, self.master))
							{
								int stackbonus = Math.Max(0, owner.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - MainPlugin.Gland_MaxSummons.Value);
								int dmgitem = MainPlugin.Gland_BaseDamage.Value + (MainPlugin.Gland_StackDamage.Value * stackbonus);
								int hpitem = MainPlugin.Gland_BaseHealth.Value + (MainPlugin.Gland_StackHealth.Value * stackbonus);
								self.inventory.GiveItem(RoR2Content.Items.BoostDamage, dmgitem - self.inventory.GetItemCount(RoR2Content.Items.BoostDamage));
								self.inventory.GiveItem(RoR2Content.Items.BoostHp, hpitem - self.inventory.GetItemCount(RoR2Content.Items.BoostHp));
							}
						}
					}
				}
			}
			orig(self);
			if (self)
			{
				if (self.bodyIndex == BodyCatalog.FindBodyIndex("BeetleGuardAllyBody"))
				{
					if (self.skillLocator.primary)
					{
						if (self.skillLocator.primary.isCombatSkill)
						{
							self.skillLocator.primary.cooldownScale /= self.attackSpeed;
						}
					}
					if (self.skillLocator.secondary)
					{
						if (self.skillLocator.secondary.isCombatSkill)
						{
							self.skillLocator.secondary.cooldownScale /= self.attackSpeed;
						}
					}
					if (self.skillLocator.utility)
					{
						if (self.skillLocator.utility.isCombatSkill)
						{
							self.skillLocator.utility.cooldownScale /= self.attackSpeed;
						}
					}
					if (self.skillLocator.special)
					{
						if (self.skillLocator.special.isCombatSkill)
						{
							self.skillLocator.special.cooldownScale /= self.attackSpeed;
						}
					}
				}
			}
		}
		private static void CalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender && sender.inventory)
			{
				float levelbonus = sender.level - 1f;
				if (sender.bodyIndex == BodyCatalog.FindBodyIndex("BeetleGuardAllyBody"))
				{
					float regenmult = 1f + (sender.inventory.GetItemCount(RoR2Content.Items.BoostHp) * 0.1f);
					if (sender.outOfDanger)
					{
						args.baseRegenAdd += regenmult * MainPlugin.Gland_BaseHealth.Value * (MainPlugin.Gland_Regen.Value + (levelbonus * MainPlugin.Gland_Regen.Value * 0.2f));
					}
				}
				if (sender.HasBuff(QueensGland.Staunching))
				{
					args.armorAdd += 100f;
				}
				if (sender.HasBuff(QueensGland.BeetleFrenzy))
				{
					args.baseAttackSpeedAdd += 0.5f;
					args.moveSpeedMultAdd += 0.5f;
					args.baseRegenAdd += 3f + (levelbonus * 0.6f);
					args.baseDamageAdd += (sender.baseDamage + (levelbonus * sender.levelDamage)) * 0.25f;
				}
			}
		}
		private static int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
		{
			var result = orig(self, slot);
			if (slot != DeployableSlot.BeetleGuardAlly)
			{
				return result;
			}
			int mult = 1;
			if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef))
			{
				mult = 2;
			}
			return Math.Min(MainPlugin.Gland_MaxSummons.Value, self.inventory.GetItemCount(RoR2Content.Items.BeetleGland)) * mult;
		}
		private static void BeetleGland_Override()
		{
			On.RoR2.Items.BeetleGlandBodyBehavior.FixedUpdate += (orig, self) =>
			{
				if (self.body && self.body.inventory)
				{
					int extraglands = Math.Max(0, self.body.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - MainPlugin.Gland_MaxSummons.Value);
					int deployableCount = self.body.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly);
					int maxdeployable = self.body.master.GetDeployableSameSlotLimit(DeployableSlot.BeetleGuardAlly);
					if (deployableCount < maxdeployable)
					{
						self.guardResummonCooldown -= Time.fixedDeltaTime;
						if (self.guardResummonCooldown <= 0f)
						{
							self.guardResummonCooldown = 2f;
							DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Approximate,
								minDistance = 3f,
								maxDistance = 40f,
								spawnOnTarget = self.transform,
							}, RoR2Application.rng);
							directorSpawnRequest.summonerBodyObject = self.gameObject;
							directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
							{
								if (SetupSummonedBeetleGuard(spawnResult, self.body.master, extraglands))
								{
									self.guardResummonCooldown = MainPlugin.Gland_RespawnTime.Value;
								}
							}));
							DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
						}
					}
					else if (deployableCount > maxdeployable)
					{
						Helpers.RemoveDeployableBeetles(self.body.master, deployableCount - maxdeployable);
					}
				}
			};

		}
		private static bool SetupSummonedBeetleGuard(SpawnCard.SpawnResult spawnResult, CharacterMaster summoner, int itemcount)
		{
			GameObject spawnedInstance = spawnResult.spawnedInstance;
			if (!spawnedInstance)
			{
				return false;
			}
			CharacterMaster beeble = spawnedInstance.GetComponent<CharacterMaster>();
			if (beeble)
			{
				Helpers.GiveRandomEliteAffix(beeble);
				Deployable deployable = beeble.GetComponent<Deployable>();
				if (deployable)
				{
					deployable.onUndeploy.AddListener(new UnityEngine.Events.UnityAction(beeble.TrueKill));
					summoner.AddDeployable(deployable, DeployableSlot.BeetleGuardAlly);
					if (summoner.GetBody())
					{
						return true;
					}
				}
			}
			return false;
		}
		private static void BaseAI_FixedUpdate(On.RoR2.CharacterAI.BaseAI.orig_FixedUpdate orig, BaseAI self)
		{
			float lastAttention = self.enemyAttention;
			orig(self);
			if (self.name != QueensGland.BeetleGuardAlly_BaseAI.name + "(Clone)")
			{
				return;
			}
			if (!self.body)
			{
				return;
			}
			if (!self.currentEnemy.gameObject)
			{
				return;
			}
			CharacterBody targetbody = self.currentEnemy.gameObject.GetComponent<CharacterBody>();
			if (targetbody)
			{
				if (targetbody.isFlying && !targetbody.isBoss)
				{
					if (lastAttention < self.enemyAttention)
					{
						if (MainPlugin.Gland_Debug.Value)
						{
							MainPlugin.ModLogger.LogInfo("Attempt to redirect focus from flying target.");
						}
						HurtBox target = FindEnemy(self);
						if (target)
						{
							self.currentEnemy.gameObject = target.healthComponent.gameObject;
							self.currentEnemy.bestHurtBox = target;
							self.enemyAttention = self.enemyAttentionDuration;
							if (!target.healthComponent.body.isFlying)
							{
								self.targetRefreshTimer = self.enemyAttentionDuration;
							}
						}
					}
				}
			}
		}
		private static HurtBox FindEnemy(BaseAI self)
		{
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = self.body;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(self.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = MainPlugin.Gland_AI_LeashLength.Value;
			search.searchOrigin = self.bodyInputBank.aimOrigin;
			search.searchDirection = self.bodyInputBank.aimDirection;
			search.maxAngleFilter = (self.fullVision ? 180f : 90f);
			search.filterByLoS = true;
			search.RefreshCandidates();
			HurtBox result = null;
			foreach (HurtBox target in search.GetResults())
			{
				if (target && target.healthComponent)
				{
					if (target.healthComponent.body.isFlying && !target.healthComponent.body.isBoss)
					{
						if (result == null)
						{
							result = target;
						}
					}
					else
					{
						if (result)
						{
							if (Vector3.Distance(self.body.transform.position, target.healthComponent.body.transform.position) < 25f)
							{
								result = target;
								break;
							}
						}
						else
						{
							result = target;
							break;
						}
					}
				}
			}
			return result;
		}
	}
}
