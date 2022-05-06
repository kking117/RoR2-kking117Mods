using System;
using RoR2;
using UnityEngine;

namespace ZoeaRework.Changes
{
    public class ZoeaBehavior_Rework : CharacterBody.ItemBehavior
    {
        private float spawnTimer;
        private float spawntimerGoal;
        private DirectorPlacementRule placementRule;
        private CharacterSpawnCard spawnCard;
        private void Awake()
        {
            base.enabled = true;
            spawntimerGoal = MainPlugin.Config_Rework_SpawnTime.Value;
            spawnTimer = spawntimerGoal - 5f;
            spawnCard = VoidMegaCrabAlly.AllySpawnCard;
            placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 10f,
                maxDistance = 30f,
                spawnOnTarget = base.transform
            };
        }
        private void FixedUpdate()
        {
            if (body.master)
            {
                if (!body.master.IsDeployableLimited(DeployableSlot.VoidMegaCrabItem))
                {
                    spawnTimer += Time.fixedDeltaTime;
                    if (spawnTimer > spawntimerGoal)
                    {
                        spawnTimer -= 5f;
                        DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng);
                        directorSpawnRequest.summonerBodyObject = base.gameObject;
                        directorSpawnRequest.onSpawnedServer = new Action<SpawnCard.SpawnResult>(OnSummonSpawned);
                        directorSpawnRequest.summonerBodyObject = base.gameObject;
                        DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                    }
                }
            }
        }
        private void OnSummonSpawned(SpawnCard.SpawnResult spawnResult)
        {
            GameObject spawnedInstance = spawnResult.spawnedInstance;
            if (!spawnedInstance)
            {
                return;
            }
            CharacterMaster spawnMaster = spawnedInstance.GetComponent<CharacterMaster>();
            if (spawnMaster)
            {
                Deployable deployable = spawnMaster.GetComponent<Deployable>();
                if (!deployable)
                {
                    deployable = spawnMaster.gameObject.AddComponent<Deployable>();
                }
                CharacterMaster owner = body.master;
                if(owner)
                {
                    VoidMegaCrabItem_Shared.UpdateAILeash(spawnMaster);
                    spawnMaster.minionOwnership.SetOwner(owner);
                    owner.AddDeployable(deployable, DeployableSlot.VoidMegaCrabItem);
                    VoidMegaCrabItem_Rework.UpdateInventory(owner, spawnMaster);
                    VoidMegaCrabItem_Rework.CullSummons(owner);
                    spawnTimer -= spawntimerGoal;
                }
            }
        }
    }
}
