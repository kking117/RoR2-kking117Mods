using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace ZoeaRework.Changes
{
    public class ZoeaBehavior_Buff : CharacterBody.ItemBehavior
    {
        private float spawnTimer;
        private float spawntimerGoal;
        private DirectorPlacementRule placementRule;
        private List<CharacterSpawnCard> spawnCardList;
        private int cardIndex;
        private void Awake()
        {
            base.enabled = true;
            spawntimerGoal = MainPlugin.Config_Buff_SpawnTime.Value;
            spawnTimer = spawntimerGoal - 5f;
            cardIndex = 0;
            RefreshCardList();
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
                    if(cardIndex >= spawnCardList.Count)
                    {
                        cardIndex = 0;
                    }
                    spawnTimer += Time.fixedDeltaTime;
                    if (spawnTimer > spawntimerGoal)
                    {
                        spawnTimer -= 5f;
                        DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCardList[cardIndex], placementRule, RoR2Application.rng);
                        directorSpawnRequest.summonerBodyObject = base.gameObject;
                        directorSpawnRequest.teamIndexOverride = TeamIndex.Player;
                        directorSpawnRequest.ignoreTeamMemberLimit = true;
                        directorSpawnRequest.onSpawnedServer = new Action<SpawnCard.SpawnResult>(OnSummonSpawned);
                        DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                        cardIndex++;
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
                    CharacterBody spawnBody = spawnMaster.GetBody();
                    if (spawnBody)
                    {
                        spawnMaster.teamIndex = owner.teamIndex;
                        spawnBody.teamComponent.teamIndex = owner.teamIndex;
                    }
                    VoidMegaCrabItem_Shared.UpdateAILeash(spawnMaster);
                    spawnMaster.minionOwnership.SetOwner(owner);
                    owner.AddDeployable(deployable, DeployableSlot.VoidMegaCrabItem);
                    VoidMegaCrabItem_Buff.UpdateInventory(owner, spawnMaster);
                    VoidMegaCrabItem_Buff.CullSummons(owner);
                    spawnTimer -= spawntimerGoal;
                }
            }
        }
        private void RefreshCardList()
        {
            spawnCardList = new List<CharacterSpawnCard>();
            spawnCardList.Add(NullifierAlly.AllySpawnCard);
            spawnCardList.Add(VoidJailerAlly.AllySpawnCard);
            spawnCardList.Add(VoidMegaCrabAlly.AllySpawnCard);
        }
    }
}
