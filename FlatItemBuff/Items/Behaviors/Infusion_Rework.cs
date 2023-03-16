using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using EntityStates.GummyClone;

namespace FlatItemBuff.Items.Behaviors
{
    public class Infusion_Rework : CharacterBody.ItemBehavior
    {
		private float updateTimer = 3f;
		private bool customLeash = Items.Infusion_Rework.CustomLeash > 0f;
		private DeployableSlot deploySlot = Items.Infusion_Rework.InfusionDeployable;

		private const float mincreateDistance = 8f;
		private const float maxcreateDistance = 24f;
		private const float updateDelayTime = 3f;
		private void FixedUpdate()
		{
			updateTimer -= Time.fixedDeltaTime;
			if (updateTimer <= 0f)
			{
				updateTimer = updateDelayTime;
				UpdateClone();
			}
		}
		private void UpdateClone()
        {
			CharacterMaster master = body.master;
			if (master)
			{
				Inventory inventory = master.inventory;
				if (inventory)
				{
					if (inventory.infusionBonus > Items.Infusion_Rework.CloneCost)
					{
						if (master.GetDeployableCount(Items.Infusion_Rework.InfusionDeployable) < 1)
						{
							CreateClone(master, body);
						}
						else
						{
							FeedClone(master, inventory);
						}
					}
				}
			}
		}
		private void FeedClone(CharacterMaster owner, Inventory inventory)
		{
			uint totalBlood = inventory.infusionBonus;
			CharacterMaster clone = null;
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
								if (master.teamIndex == owner.teamIndex)
								{
									if (clone)
									{
										totalBlood += master.inventory.infusionBonus;
										master.TrueKill();
									}
									else
									{
										clone = master;
									}
								}
							}
						}
					}
				}
			}
			if (totalBlood > 0 && clone)
            {
				clone.inventory.AddInfusionBonus(totalBlood);
				inventory.infusionBonus = 0;
			}
		}
		private void CreateClone(CharacterMaster ownerMaster, CharacterBody ownerBody)
		{
			if (ownerBody && ownerMaster)
			{
				MasterCopySpawnCard spawnCard = MasterCopySpawnCard.FromMaster(ownerMaster, false, false, null);
				if (!spawnCard)
				{
					return;
				}
				if (customLeash)
				{
					spawnCard.GiveItem(RoR2Content.Items.MinionLeash, 1);
				}
				spawnCard.GiveItem(Items.Infusion_Rework.BloodCloneItem, 1);
				spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;

				DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Approximate,
					minDistance = mincreateDistance,
					maxDistance = maxcreateDistance,
					position = ownerBody.corePosition
				}, RoR2Application.rng);
				directorSpawnRequest.summonerBodyObject = ownerBody.gameObject;
				directorSpawnRequest.teamIndexOverride = ownerMaster.teamIndex;
				directorSpawnRequest.ignoreTeamMemberLimit = true;

				directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
				{
					CharacterMaster summonMaster = result.spawnedInstance.GetComponent<CharacterMaster>();

					Deployable deployable = result.spawnedInstance.AddComponent<Deployable>();
					ownerMaster.AddDeployable(deployable, deploySlot);
					deployable.onUndeploy = (deployable.onUndeploy ?? new UnityEvent());
					deployable.onUndeploy.AddListener(new UnityAction(summonMaster.TrueKill));

					GameObject bodyObject = summonMaster.GetBodyObject();
					if (bodyObject)
					{
						foreach (EntityStateMachine entityStateMachine in bodyObject.GetComponents<EntityStateMachine>())
						{
							if (entityStateMachine.customName == "Body")
							{
								entityStateMachine.SetState(new GummyCloneSpawnState());
								return;
							}
						}
					}
					FeedClone(summonMaster, ownerMaster.inventory);
				}));
				DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
				UnityEngine.Object.Destroy(spawnCard);
			}
		}
	}
}
