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
		private int playerCount = 1;
		private int itemCount = 0;
		private int rewardCount = 0;
		private uint goldReward = 0;
		internal static float SpawnDelayMin = 10;
		internal static float SpawnDelayMax = 17;
		private CharacterBody lastKiller;
		private CharacterBody lastBody;
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
						if (SonorousWhispers_Rework.ScalePlayer)
						{
							playerCount = Math.Max(1, Run.instance.participatingPlayerCount);
						}
						if (SonorousWhispers_Rework.BaseReward > 0)
                        {
							rewardCount = CalcRewardCount(itemCount);
						}
						if (SonorousWhispers_Rework.BaseGold > 0)
                        {
							goldReward = CalcGoldReward(itemCount);
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
		private uint CalcGoldReward(int itemAmount)
		{
			int result = SonorousWhispers_Rework.BaseGold + (SonorousWhispers_Rework.StackGold * Math.Max(0, itemAmount-1));
			if (result > 0)
            {
				if (SonorousWhispers_Rework.ScalePlayer)
				{
					result = Mathf.CeilToInt(playerCount * Run.instance.GetDifficultyScaledCost(result));
				}
				else
				{
					result = Run.instance.GetDifficultyScaledCost(result);
				}
				return (uint)result;
			}
			return 0;
		}
		private int CalcRewardCount(int itemAmount)
        {
			int baseAmount = SonorousWhispers_Rework.BaseReward;
			int stackAmount = SonorousWhispers_Rework.StackReward;
			if (SonorousWhispers_Rework.ScalePlayer)
			{
				baseAmount *= playerCount;
				stackAmount *= playerCount;
			}
			int result = baseAmount;
			itemCount = 1;
			for (int i = 1; i < itemAmount; i++)
            {
				if (result + stackAmount <= SonorousWhispers_Rework.RewardLimit)
                {
					result += stackAmount;
					itemCount += 1;
				}
				else
                {
					break;
                }
			}
			return result;
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
				spawnRequest.teamIndexOverride = TeamIndex.Monster;

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

							HPItem *= Mathf.Pow(playerCount, 0.5f);

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
			lastBody = member.GetBody();
			if (damageReport.attackerBody)
            {
				lastKiller = damageReport.attackerBody;
			}
			if (lastBody)
            {
				LastDeathPos = lastBody.transform.position;
				DeathRewards deathRewards = lastBody.GetComponent<DeathRewards>();
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
			if (goldReward > 0)
            {
				TeamManager.instance.GiveTeamMoney(TeamIndex.Player, goldReward);
			}
			if (SonorousWhispers_Rework.BaseDamage > 0f && SonorousWhispers_Rework.BaseRadius > 0f)
            {
				
				float blastDamage = SonorousWhispers_Rework.BaseDamage + (Math.Max(0, itemCount - 1) * SonorousWhispers_Rework.StackDamage);
				float blastRadius = SonorousWhispers_Rework.BaseRadius + (Math.Max(0, itemCount - 1) * SonorousWhispers_Rework.StackRadius);

				float baseDamage = 12f + (3.6f * Math.Max(0, Run.instance.ambientLevel -1f));
				new BlastAttack
				{
					attacker = (lastKiller && lastKiller.gameObject) ? lastKiller.gameObject : null,
					baseDamage = baseDamage * blastDamage * playerCount,
					baseForce = blastDamage * 100f,
					bonusForce = Vector3.zero,
					attackerFiltering = AttackerFiltering.Default,
					crit = lastKiller ? lastKiller.RollCrit() : false,
					damageColorIndex = DamageColorIndex.Item,
					damageType = DamageType.Stun1s|DamageType.BonusToLowHealth,
					falloffModel = BlastAttack.FalloffModel.None,
					inflictor = (lastBody && lastBody.gameObject) ? lastBody.gameObject : null,
					position = LastDeathPos,
					procChainMask = default(ProcChainMask),
					procCoefficient = 3f,
					radius = blastRadius,
					losType = BlastAttack.LoSType.None,
					teamIndex = TeamIndex.Player
				}.Fire();
				EffectManager.SpawnEffect(EntityStates.VagrantMonster.FireMegaNova.novaEffectPrefab, new EffectData
				{
					origin = LastDeathPos,
					scale = 1f,
					rotation = Quaternion.identity
				}, true);
				if (lastBody)
                {
					Util.PlaySound(EntityStates.VagrantNovaItem.DetonateState.explosionSound, lastBody.gameObject);
				}
			}
		}
	}
}
