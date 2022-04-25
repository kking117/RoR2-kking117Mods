using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZoeaRework.Changes
{
    public class ZoeaBehavior : CharacterBody.ItemBehavior
    {
        private float spawnTimer;
        private float spawntimerGoal;
        private DirectorPlacementRule placementRule;
        private CharacterSpawnCard spawnCard;
        private void Awake()
        {
            base.enabled = true;
            spawntimerGoal = MainPlugin.Config_Zoea_SpawnTime.Value;
            spawnTimer = spawntimerGoal - 5f;
            spawnCard = VoidMegaCrabAlly.AllySpawnCard;
            placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 5f,
                maxDistance = 40f,
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
                        spawnTimer = 0f;
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
                    spawnMaster.minionOwnership.SetOwner(owner);
                    owner.AddDeployable(deployable, DeployableSlot.VoidMegaCrabItem);
                    VoidMegaCrabAlly.UpdateInventory(owner, spawnMaster);
                }
            }
        }
    }
}
