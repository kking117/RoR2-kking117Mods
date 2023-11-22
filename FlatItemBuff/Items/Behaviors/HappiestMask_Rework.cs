using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;

namespace FlatItemBuff.Items.Behaviors
{
	public class HappiestMask_Rework : CharacterBody.ItemBehavior
	{
		private DeployableSlot deploySlot = Items.HappiestMask_Rework.Ghost_DeployableSlot;
		private float updateTimer = 1f;
		private float summonTimer = 1f;
		private const float updateDelayTime = 1f;
		private const float MinSummonDistance = 6f;
		private const float MaxSummonDistance = 18f;
		private void FixedUpdate()
		{
			if (!body)
			{
				return;
			}
			updateTimer -= Time.fixedDeltaTime;
			if (summonTimer > 0f)
            {
				summonTimer -= Time.fixedDeltaTime;
			}
			if (updateTimer <= 0f)
			{
				updateTimer += updateDelayTime;
				UpdateGhosts();
			}
		}
		private void UpdateGhosts()
		{
			CharacterMaster owner = body.master;
			int maxGhosts = owner.GetDeployableCount(deploySlot);
			if (owner.deployablesList != null)
			{
				for (int i = 0; i < owner.deployablesList.Count; i++)
				{
					if (owner.deployablesList[i].slot == deploySlot)
					{
						Deployable deploy = owner.deployablesList[i].deployable;
						if (deploy)
						{
							CharacterMaster master = deploy.GetComponent<CharacterMaster>();
							if (master)
							{
								maxGhosts--;
								if (maxGhosts < 0)
								{
									master.TrueKill();
								}
							}
						}
					}
				}
			}
			if (maxGhosts > 0)
			{
				if (summonTimer <= 0f)
				{
					CreateGhost(owner);
				}
			}
		}
		private void CreateGhost(CharacterMaster ownerMaster)
		{
			CharacterBody ownerBody = ownerMaster.GetBody();
			if (!ownerBody)
            {
				return;
            }

			MasterCopySpawnCard spawnCard = MasterCopySpawnCard.FromMaster(ownerMaster, false, false, null);
			if (!spawnCard)
			{
				return;
			}
			spawnCard.GiveItem(RoR2Content.Items.Ghost);
			spawnCard.GiveItem(Items.HappiestMask_Rework.GhostCloneIdentifier, stack);
			spawnCard.GiveItem(RoR2Content.Items.HealthDecay, Items.HappiestMask_Rework.BaseDuration);
			spawnCard.GiveItem(RoR2Content.Items.MinionLeash);
			spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;

			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				minDistance = ownerBody.radius + MinSummonDistance,
				maxDistance = ownerBody.radius + MaxSummonDistance,
				position = ownerBody.corePosition
			}, RoR2Application.rng);
			directorSpawnRequest.summonerBodyObject = ownerBody.gameObject;
			directorSpawnRequest.teamIndexOverride = ownerMaster.teamIndex;
			directorSpawnRequest.ignoreTeamMemberLimit = true;

			directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
			{
				summonTimer += Items.HappiestMask_Rework.BaseCooldown;
				CharacterMaster summonMaster = result.spawnedInstance.GetComponent<CharacterMaster>();

				Deployable deployable = result.spawnedInstance.AddComponent<Deployable>();
				ownerMaster.AddDeployable(deployable, deploySlot);
				deployable.onUndeploy = (deployable.onUndeploy ?? new UnityEvent());
				deployable.onUndeploy.AddListener(new UnityAction(summonMaster.TrueKill));
				GameObject bodyObject = summonMaster.GetBodyObject();
				if (bodyObject)
				{
					CharacterBody summonBody = summonMaster.GetBody();
					if (summonBody)
					{
						EffectData effectData = new EffectData();
						effectData.origin = summonBody.corePosition;
						effectData.SetNetworkedObjectReference(summonBody.gameObject);
						effectData.scale = summonBody.radius;
						EffectManager.SpawnEffect(Items.HappiestMask_Rework.GhostSpawnEffect, effectData, true);
					}
					foreach (EntityStateMachine entityStateMachine in bodyObject.GetComponents<EntityStateMachine>())
					{
						entityStateMachine.initialStateType = entityStateMachine.mainStateType;
					}
				}
			}));
			DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
			UnityEngine.Object.Destroy(spawnCard);
		}
	}
}
