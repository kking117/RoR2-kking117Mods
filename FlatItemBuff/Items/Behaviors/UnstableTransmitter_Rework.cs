using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items.Behaviors
{
	public class UnstableTransmitter_Rework : CharacterBody.ItemBehavior
	{
		Xoroshiro128Plus rng;
		CharacterSpawnCard droneSpawnCard;
		DirectorPlacementRule placementRule;
		GameObject helperPrefab;

		private DeployableSlot deploySlot = Items.UnstableTransmitter_Rework.Drone_DeployableSlot;

		private const float spawnRetryDelay = 1f;
		private float nextSpawn = spawnRetryDelay;
		private Vector3 immobileOffset = new Vector3(0f, 2f, 0f);
		private int oldTeleIndex = 0;

		private float nextTeleport = 5f;
		private void OnEnable()
		{
			ulong num = Run.instance.seed ^ (ulong)Run.instance.stageClearCount;
			rng = new Xoroshiro128Plus(num);
			droneSpawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Drones/cscBackupDrone.asset").WaitForCompletion();
			helperPrefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
			placementRule = new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				minDistance = 3f,
				maxDistance = 40f,
				spawnOnTarget = base.transform
			};
		}
		private void FixedUpdate()
		{
			if (!body)
			{
				return;
			}
			CharacterMaster owner = body.master;
			int maxDrones = owner.GetDeployableSameSlotLimit(deploySlot);
			if (owner.GetDeployableCount(deploySlot) < maxDrones)
			{
				if (nextSpawn <= 0f)
				{
					SpawnDrone();
				}
				else
				{
					nextSpawn -= Time.fixedDeltaTime;
				}
			}
			nextTeleport -= Time.fixedDeltaTime;
			if (nextTeleport < 0f)
            {
				CalcTeleDelay();
				SetTargetAndPos();
			}
		}
		private void CalcTeleDelay()
        {
			float delay = Items.UnstableTransmitter_Rework.BaseCooldown;
			CharacterMaster master = body.master;
			if (master)
            {
				int allyCount = Math.Max(TeamComponent.GetTeamMembers(body.teamComponent.teamIndex).Count -1, 0);
				delay /= 1f + (Items.UnstableTransmitter_Rework.AllyStackCooldown * allyCount);
			}
            nextTeleport += Math.Max(Items.UnstableTransmitter_Rework.CapCooldown, delay);
		}

		private void SetTargetAndPos()
        {
			List<CharacterBody> TeleBodies = new List<CharacterBody>();
			foreach(TeamComponent comp in TeamComponent.GetTeamMembers(body.teamComponent.teamIndex))
            {
				CharacterBody ibody = comp.body;
				if (ibody && ibody.master)
                {
					if (!ibody.isPlayerControlled)
                    {
						MinionOwnership minion = ibody.master.minionOwnership;
						if (minion && minion.ownerMaster == body.master)
                        {
							if (Items.UnstableTransmitter_Rework.TeleImmobile || IsMobile(ibody))
                            {
								TeleBodies.Add(ibody);
							}
						}
                    }
                }
			}
			if (TeleBodies.Count > 0)
            {
				if (oldTeleIndex > TeleBodies.Count-1)
                {
					oldTeleIndex = 0;
				}
				CharacterBody targetBody = TeleBodies[oldTeleIndex];
				oldTeleIndex += 1;
				if (targetBody)
				{
					bool foundPos = false;
					Vector3 newPos = targetBody.corePosition;
					bool mobile = IsMobile(targetBody);
					float maxHeight = 2f + GetBlastRadius() / 2f;
					if (Items.UnstableTransmitter_Rework.TeleFragRadius > 0f)
                    {
						BullseyeSearch search = new BullseyeSearch();
						search.viewer = body;
						search.teamMaskFilter = TeamMask.allButNeutral;
						search.teamMaskFilter.RemoveTeam(body.master.teamIndex);
						search.sortMode = BullseyeSearch.SortMode.Distance;
						search.minDistanceFilter = body.radius + 2f;
						search.maxDistanceFilter = Items.UnstableTransmitter_Rework.TeleFragRadius;
						search.searchOrigin = body.inputBank.aimOrigin;
						search.searchDirection = body.inputBank.aimDirection;
						search.maxAngleFilter = 180f;
						search.filterByLoS = false;
						search.RefreshCandidates();
						foreach (HurtBox target in search.GetResults())
						{
							HealthComponent hpcomp = target.healthComponent;
							if (hpcomp.alive)
							{
								if (hpcomp.body)
								{
									if (!mobile)
									{
										RaycastHit raycastHit;
										Physics.Raycast(hpcomp.body.corePosition + immobileOffset, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
										{
										"World"
										}));
										float distance = Vector3.Distance(raycastHit.point, hpcomp.body.corePosition);
										if (distance < maxHeight)
										{
											newPos = raycastHit.point;
											foundPos = true;
											break;
										}
									}
									else
									{
										newPos = hpcomp.body.corePosition;
										foundPos = true;
										break;
									}
								}
							}
						}
					}
					if (!foundPos)
                    {
						SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
						spawnCard.hullSize = targetBody.hullClassification;
						spawnCard.nodeGraphType = (targetBody.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
						spawnCard.prefab = helperPrefab;
						GameObject resultObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.Approximate,
							position = body.corePosition,
							minDistance = body.radius + 10f,
							maxDistance = Items.UnstableTransmitter_Rework.TeleportRadius
						}, RoR2Application.rng));
						UnityEngine.Object.Destroy(spawnCard);
						if (resultObject)
						{
							UnityEngine.Object.Destroy(resultObject);
							if (!mobile)
							{
								RaycastHit raycastHit;
								Physics.Raycast(resultObject.transform.position+immobileOffset, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
								{
								"World"
								}));
								float distance = Vector3.Distance(raycastHit.point, resultObject.transform.position);
								if (distance < maxHeight)
								{
									newPos = raycastHit.point;
									foundPos = true;
								}
							}
							else
                            {
								newPos = resultObject.transform.position;
								foundPos = true;
							}
						}
					}
					if (foundPos)
                    {
						if(!mobile)
                        {
							newPos.y += 0.4f;
						}
						TeleTarget(targetBody, newPos);
					}
					else
                    {
                        nextSpawn = Items.UnstableTransmitter_Rework.CapCooldown;
                    }
				}
			}
		}
		private void TeleTarget(CharacterBody targetBody, Vector3 newPos)
        {
			targetBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 1f);
			Vector3 corePosition = targetBody.corePosition;
			EffectManager.SpawnEffect(CharacterBody.CommonAssets.teleportOnLowHealthVFX, new EffectData
			{
				origin = corePosition,
				rotation = Quaternion.identity
			}, true);
			TeleportHelper.TeleportBody(targetBody, newPos);

			CharacterBody attackerBody = body;
			if (Items.UnstableTransmitter_Rework.AllyOwnsDamage)
            {
				attackerBody = targetBody;
			}
			ProcChainMask procChainMask = new ProcChainMask();
			if (!Items.UnstableTransmitter_Rework.ProcBands)
			{
				procChainMask.AddProc(ProcType.Rings);
			}

			float blastRadius = GetBlastRadius();
			float damageCoef = Items.UnstableTransmitter_Rework.BaseDamage + (Items.UnstableTransmitter_Rework.StackDamage * Math.Max(0, base.stack - 1));
			new BlastAttack
			{
				attacker = attackerBody.gameObject,
				baseDamage = attackerBody.damage * damageCoef,
				baseForce = 2000f,
				bonusForce = Vector3.zero,
				attackerFiltering = AttackerFiltering.NeverHitSelf,
				crit = attackerBody.RollCrit(),
				damageColorIndex = DamageColorIndex.Item,
				damageType = DamageType.Generic,
				falloffModel = BlastAttack.FalloffModel.SweetSpot,
				inflictor = attackerBody.gameObject,
				position = newPos,
				procChainMask = procChainMask,
				procCoefficient = Items.UnstableTransmitter_Rework.ProcRate,
				radius = blastRadius,
				losType = BlastAttack.LoSType.None,
				teamIndex = body.teamComponent.teamIndex
			}.Fire();
			EffectManager.SpawnEffect(CharacterBody.CommonAssets.teleportOnLowHealthExplosion, new EffectData
			{
				origin = newPos,
				scale = Math.Max(5f, blastRadius * 1.25f),
				rotation = Quaternion.identity
			}, true);
			Util.PlaySound("Play_item_proc_teleportOnLowHealth", targetBody.gameObject);
		}

		private float GetBlastRadius()
        {
			return Items.UnstableTransmitter_Rework.BaseRadius + (Items.UnstableTransmitter_Rework.StackRadius * Math.Max(0, base.stack - 1));
		}
		private bool IsMobile(CharacterBody targetBody)
		{
			if (targetBody.GetComponent<CharacterMotor>())
            {
				return true;
            }
			Rigidbody rigidBody = targetBody.GetComponent<Rigidbody>();
			if (rigidBody)
            {
				if (rigidBody.GetComponent<RigidbodyMotor>())
				{
					return true;
				}
			}
			return false;
		}
		private void SpawnDrone()
        {
			nextSpawn = spawnRetryDelay;
			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(droneSpawnCard, placementRule, rng);
			directorSpawnRequest.summonerBodyObject = base.gameObject;
			directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
			{
				if (!result.success)
				{
					return;
				}
				CharacterMaster summonMaster = result.spawnedInstance.GetComponent<CharacterMaster>();
				Deployable deployable = result.spawnedInstance.AddComponent<Deployable>();
				body.master.AddDeployable(deployable, deploySlot);
				deployable.onUndeploy = deployable.onUndeploy ?? new UnityEvent();
				deployable.onUndeploy.AddListener(new UnityAction(summonMaster.TrueKill));
				GameObject bodyObject = summonMaster.GetBodyObject();
				if (bodyObject)
				{
					CharacterBody summonBody = summonMaster.GetBody();
					if (summonBody)
					{
						Util.PlaySound("Play_item_proc_teleportOnLowHealth", summonBody.gameObject);
						EffectManager.SpawnEffect(CharacterBody.CommonAssets.teleportOnLowHealthVFX, new EffectData
						{
							origin = summonBody.transform.position,
							rotation = Quaternion.identity
						}, true);
					}
				}
			}));
			DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
		}
	}
}
