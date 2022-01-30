using System;
using RoR2;
using UnityEngine;
using RoR2.CharacterAI;

namespace QueenGlandBuff.Utils
{
    internal class Helpers
    {
		private static readonly System.Random rng = new System.Random();
		public static bool DoesBodyContainName(CharacterBody body, string name)
	    {
            if (body.name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return false;
	    }

        public static bool DoesMasterHaveDeployable(CharacterMaster self, DeployableSlot deployslot, CharacterMaster target)
        {
			Deployable deployable = target.GetComponent<Deployable>();
			if (deployable)
			{
				if(self.deployablesList != null)
				{
					for (int i = 0; i < self.deployablesList.Count; i++)
					{
						if (self.deployablesList[i].slot == deployslot)
						{
							if (self.deployablesList[i].deployable == deployable)
							{
								return true;
							}
						}
					}
				}
			}
            return false;
        }
		public static void RemoveDeployableBeetles(CharacterMaster self, int deletecount)
		{
			if (self)
			{
				MinionOwnership ownership = self.minionOwnership;
				if (ownership)
				{
					for (int i = 0; i < self.deployablesList.Count && deletecount > 0; i++)
					{
						if (self.deployablesList[i].slot == DeployableSlot.BeetleGuardAlly)
						{
							Deployable deployable = self.deployablesList[i].deployable;
							self.deployablesList.RemoveAt(i);
							deployable.ownerMaster = null;
							deployable.onUndeploy.Invoke();
							deletecount--;
						}
					}
				}
			}
		}
		public static void GiveRandomEliteAffix(CharacterMaster self)
		{
			if (Main.Gland_SpawnAffix.Value && Main.StageEliteEquipmentDefs.Count > 0)
			{
				int result = rng.Next(Main.StageEliteEquipmentDefs.Count);
				if (Main.StageEliteEquipmentDefs[result])
				{
					self.inventory.SetEquipmentIndex(Main.StageEliteEquipmentDefs[result].equipmentIndex);
					return;
				}
			}
			self.inventory.SetEquipmentIndex(Main.Gland_DefaultAffix_Var);
		}
		public static int TeleportToOwner(CharacterBody self)
		{
			//0 = no owner/owner is dead
			//1 = simply couldn't find a spot to teleport to
			//2 = success
			if (self.master && self.master.minionOwnership && self.master.minionOwnership.ownerMaster)
			{
				CharacterMaster owner = (self.master.minionOwnership.ownerMaster);
				if (owner.GetBody())
				{
					if (owner.GetBody().healthComponent.alive && self.inventory.GetItemCount(RoR2Content.Items.MinionLeash) > 0)
					{
						Vector3 ownerposition = owner.GetBody().corePosition;
						SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
						spawnCard.hullSize = self.hullClassification;
						spawnCard.nodeGraphType = (self.isFlying ? RoR2.Navigation.MapNodeGroup.GraphType.Air : RoR2.Navigation.MapNodeGroup.GraphType.Ground);
						spawnCard.prefab = Resources.Load<GameObject>("SpawnCards/HelperPrefab");
						GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.Approximate,
							minDistance = 15f,
							maxDistance = 40f,
							position = ownerposition
						}, RoR2Application.rng));
						if (gameObject)
						{
							Vector3 position = gameObject.transform.position;
							TeleportHelper.TeleportBody(self, position);
							GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(self.gameObject);
							if (teleportEffectPrefab)
							{
								EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, true);
							}
							UnityEngine.Object.Destroy(spawnCard);
							UnityEngine.Object.Destroy(gameObject);
							return 2;
						}
						else
						{
							UnityEngine.Object.Destroy(spawnCard);
							return 1;
						}
					}
					else
					{
						return 0;
					}
				}
			}
			return 0;
		}
		public static void DrawAggro(CharacterBody self)
        {
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = self;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(self.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = Main.Gland_Staunch_AggroRange.Value;
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
		public static void EmpowerBeetles(CharacterBody self)
		{
			BullseyeSearch search = new BullseyeSearch();
			search = new BullseyeSearch();
			search.viewer = self;
			search.teamMaskFilter = TeamMask.none;
			search.teamMaskFilter.AddTeam(self.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = Main.Gland_Staunch_AggroRange.Value;
			search.searchOrigin = self.inputBank.aimOrigin;
			search.searchDirection = self.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = false;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				if (target && target.healthComponent)
				{
					if (target.healthComponent.body)
					{
						CharacterBody targetbody = target.healthComponent.body;
						if (targetbody != self)
						{
							if (DoesBodyContainName(targetbody, "beetle"))
							{
								targetbody.AddTimedBuff(Modules.Buffs.BeetleFrenzy, 1f);
							}
						}
					}
				}
			}
		}
		private static bool RollAggroChance(CharacterBody target)
		{
			float result = UnityEngine.Random.Range(0f, 1f);
			if (target.isBoss && Main.Gland_Staunch_AggroBossChance.Value > result)
			{
				return true;
			}
			else if (Main.Gland_Staunch_AggroChance.Value > result)
			{
				return true;
			}
			return false;
		}
	}
}
