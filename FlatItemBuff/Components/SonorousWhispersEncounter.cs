using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using FlatItemBuff.Items;

namespace FlatItemBuff.Components
{
	public class SonorousWhispersEncounter : MonoBehaviour
	{
		private int spawnState = 0;
		private float spawnTimer = 20f;
		private int itemCount = 0;
		internal static float SpawnDelayMin = 10;
		internal static float SpawnDelayMax = 17;
		private Vector3 LastDeathPos = new Vector3(0f, 0f, 0f);
		private PickupIndex rewardIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex);
		private void Awake()
        {
			spawnState = 0;
			spawnTimer = UnityEngine.Random.Range(SpawnDelayMin, SpawnDelayMax);
		}
		private void FixedUpdate()
		{
			if (spawnTimer > 0f)
			{
				spawnTimer -= Time.fixedDeltaTime;
			}
			else
            {
				if (spawnState == 0)
				{
					itemCount = Util.GetItemCountForTeam(TeamIndex.Player, DLC2Content.Items.ResetChests.itemIndex, true, true);
					if (itemCount > 0)
                    {
						if (itemCount > 50)
                        {
							itemCount = 50; //lmfao, no.
						}
						BroadcastSpawn();
						spawnTimer += 3f;
						spawnState = 1;
					}
					else
                    {
						Destroy(this);
					}
					
				}
				else if (spawnState == 1)
				{
					spawnState = 2;
					SpawnEchoBoss();
				}
			}
		}

		private void BroadcastSpawn()
        {
			Chat.SimpleChatMessage message = new Chat.SimpleChatMessage();
			message.baseToken = "<style=cWorldEvent>Echoes of the fallen grow louder.</style>";
			RoR2.Chat.SendBroadcastChat(message);
        }
		private Vector3 GetPositionFromScene(Stage stage)
        {
			if (stage)
            {
				SceneDef scene = stage.sceneDef;
				if (scene)
                {
					if(SonorousWhispers_Rework.SceneList != null)
                    {
						int index = -1;
						for(int i = 0; i< SonorousWhispers_Rework.SceneList.Count; i++)
                        {
							if (SonorousWhispers_Rework.SceneList[i] == scene)
                            {
								index = i;
								break;
                            }
                        }
						if (index > -1)
                        {
							return SonorousWhispers_Rework.SceneSpawn[index];
                        }
                    }
                }
            }
			return new Vector3(0f, 0f, 0f);
		}
		private void SpawnEchoBoss()
        {
			CombatSquad combatSquad = UnityEngine.Object.Instantiate<GameObject>(SonorousWhispers_Rework.CombatEncounterObject).GetComponent<CombatSquad>();
			if (combatSquad)
			{
				SpawnCard spawnCard = GetSpawnCard();
				LastDeathPos = GetPositionFromScene(Stage.instance);
				DirectorPlacementRule placementRule = new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
					position = LastDeathPos
				};
				DirectorSpawnRequest spawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, Run.instance.runRNG);
				spawnRequest.ignoreTeamMemberLimit = true;
				spawnRequest.teamIndexOverride = TeamIndex.Neutral;

				spawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(spawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
				{
					if (!result.success)
					{
						return;
					}
					CharacterMaster characterMaster = result.spawnedInstance.GetComponent<CharacterMaster>();
					if (characterMaster)
                    {
						Inventory inventory = characterMaster.inventory;
						if (inventory)
                        {
							CharacterBody characterBody = characterMaster.GetBody();
							inventory.ResetItem(RoR2Content.Items.InvadingDoppelganger);
							inventory.GiveItem(SonorousWhispers_Rework.EchoLevelItem, itemCount);
							inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
							inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
							if (SonorousWhispers_Rework.HasAdaptive)
                            {
								inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
							}
							if (SonorousWhispers_Rework.IsElite)
							{
								inventory.SetEquipmentIndex(DLC2Content.Equipment.EliteAurelioniteEquipment.equipmentIndex);
							}

							float itemMult = Math.Max(0, itemCount - 1);

							itemMult = SonorousWhispers_Rework.BasePower + (SonorousWhispers_Rework.StackPower * itemMult);

							float HPItem = 1f + (Run.instance.difficultyCoefficient / 2.5f);
							float DmgItem = 1f + (Run.instance.difficultyCoefficient / 30f);

							HPItem *= itemMult;
							DmgItem *= itemMult;

							int playerBonus = 1;
							if (SonorousWhispers_Rework.ScalePlayer)
                            {
								playerBonus = Mathf.Max(1, Run.instance.livingPlayerCount);
							}
							HPItem *= Mathf.Pow(playerBonus, 0.5f);

							inventory.GiveItem(RoR2Content.Items.BoostHp, Mathf.RoundToInt((HPItem - 1f) * 10f));
							inventory.GiveItem(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((DmgItem - 1f) * 10f));
						}
						combatSquad.AddMember(characterMaster);
					}
				}));
				DirectorCore.instance.TrySpawnObject(spawnRequest);
				combatSquad.onMemberDefeatedServer += OnEchoKilled;
				combatSquad.onDefeatedServer += OnEchoFinish;
			}
        }

		private SpawnCard GetSpawnCard()
        {
			SpawnCard returnCard = SonorousWhispers_Rework.BaseSpawnCard;
			SonorousWhispersTracker comp = Run.instance.GetComponent<SonorousWhispersTracker>();
			if (comp)
			{
				returnCard = comp.DoSelection();
			}
			return returnCard;
		}

		private void OnEchoKilled(CharacterMaster member, DamageReport damageReport)
        {
			CharacterBody victimBody = member.GetBody();
			if (victimBody)
            {
				LastDeathPos = victimBody.transform.position;
				DeathRewards deathRewards = victimBody.GetComponent<DeathRewards>();
				PickupIndex pickupIndex = PickupIndex.none;
				if (deathRewards && deathRewards.bossDropTable)
                {
					pickupIndex = deathRewards.bossDropTable.GenerateDrop(Run.instance.runRNG);
				}
				if (pickupIndex != PickupIndex.none)
                {
					rewardIndex = pickupIndex;
				}
				
			}
		}
		private void OnEchoFinish()
        {
			int rewardCount = itemCount;

			if (SonorousWhispers_Rework.ScalePlayer)
			{
				rewardCount *= Run.instance.participatingPlayerCount;
			}

			if (rewardCount > 200)
            {
				rewardCount = 200;
			}

			if (rewardCount > 0)
            {
				float horiAngle = 360f / rewardCount;
				Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 30f + Vector3.forward * 4f);
				Quaternion quaternion = Quaternion.AngleAxis(horiAngle, Vector3.up);
				int i = 0;
				while (i < rewardCount)
				{
					PickupDropletController.CreatePickupDroplet(rewardIndex, LastDeathPos, vector);
					i++;
					vector = quaternion * vector;
				}
			}
		}
	}
}
