using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AurelioniteStuck
{
	[BepInPlugin("com.kking117.AurelioniteStuck", "AurelioniteStuck", "1.0.0")]
	[BepInDependency("com.bepis.r2api", "2.5.14")]
	public class mainstuff : BaseUnityPlugin
	{
		private CharacterMaster[] TitanList = new CharacterMaster[10];

		private float[] NextTitanSpawn = new float[3]
		{
			-1f,
			-1f,
			-1f
		};

		private float[] NextTitanKill = new float[3]
		{
			-1f,
			-1f,
			-1f
		};

		private int BossSeedCount;

		Vector3 spawnposition = new Vector3(-88.2f, 491.95f, -2.54f);
		Quaternion spawnrotation = new Quaternion(0.0f, 0.5f, 0.0f, 0.0f);
		public void Awake()
		{
			On.RoR2.Run.Start += (orig, self) =>
			{
				orig(self);
				for (int i = 0; i < 3; i++)
				{
					NextTitanSpawn[i] = -1f;
					NextTitanKill[i] = -1f;
				}
			};
			On.RoR2.Run.FixedUpdate += (orig, self) =>
			{
				orig(self);
				if (SceneManager.GetActiveScene().name.Equals("moon"))
				{
					for (int i = 0; i < 3; i++)
					{
						if (NextTitanSpawn[i] >= 0f)
						{
							if (NextTitanSpawn[i] <= Time.time)
							{
								if (i == 2)
								{
									SpawnAurelionite((TeamIndex)i, BossSeedCount);
								}
								else
								{
									SpawnAurelionite((TeamIndex)i, GetTeamSeedCount((TeamIndex)i));
								}
								NextTitanSpawn[i] = -1f;
							}
						}
						if (NextTitanKill[i] >= 0f)
						{
							if (NextTitanKill[i] <= Time.time)
							{
								KillTitans((TeamIndex)i);
								NextTitanKill[i] = -1f;
							}
						}
					}
				}
			};

			On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += (orig, self) =>
			{
				orig(self);
				if (spawnonphase == 1)
				{
					NextTitanSpawn[1] = Time.time + 5.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += (orig, self) =>
			{
				orig(self);
				if (spawnonphase == 2)
				{
					NextTitanSpawn[1] = Time.time + 3.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += (orig, self) =>
			{
				orig(self);
				if (spawnonphase == 3)
				{
					NextTitanSpawn[1] = Time.time + 5.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += (orig, self) =>
			{
				orig(self);
				if (spawnonsteal)
				{
					NextTitanKill[1] = Time.time + 7.5f;
					BossSeedCount = GetTeamSeedCount(TeamIndex.Player);
					NextTitanSpawn[2] = Time.time + 12.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += (orig, self) =>
			{
				orig(self);
				if (spawnonsteal)
				{
					NextTitanSpawn[2] = -1f;
					NextTitanKill[2] = Time.time + 2.5f;
				}
			};

			int GetTeamSeedCount(TeamIndex team)
			{
				int SeedCount = 0;
				for (int i = 0; i < RoR2.PlayerCharacterMasterController.instances.Count; i++)
				{
					CharacterBody charbody = RoR2.PlayerCharacterMasterController.instances[i].master.GetBody();
					if (charbody)
					{
						TeamComponent teamComponent = charbody.teamComponent;
						if (teamComponent.teamIndex == team)
						{
							HealthComponent healthComponent = charbody.healthComponent;
							if (healthComponent && healthComponent.health > 0)
							{
								CharacterMaster master = charbody.master;
								if (master)
								{
									SeedCount += master.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
								}
							}
						}
					}
				}
				return SeedCount;
			};
			void SpawnAurelionite(TeamIndex team, int SeedCount)
			{
				if (SeedCount > 0)
				{
					float deadrangeA = 5f;
					float maxrangeA = 15f;
					if (UnityEngine.Random.Range(0f, 1f)>0.5f)
                    {
						deadrangeA *= 1f;
						maxrangeA *= 1f;
					}
					float deadrangeB = 5f;
					float maxrangeB = 15f;
					if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
					{
						deadrangeB *= 1f;
						maxrangeB *= 1f;
					}
					CharacterMaster titan = new MasterSummon
					{
						masterPrefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindMasterIndex("TitanGoldMaster")),
						position = spawnposition - new Vector3(UnityEngine.Random.Range(deadrangeA, maxrangeA), 0f, UnityEngine.Random.Range(deadrangeB, maxrangeB)),
						rotation = spawnrotation,
						summonerBodyObject = null,
						ignoreTeamMemberLimit = true,
						teamIndexOverride = team,
					}.Perform();

					if (titan)
					{
						float num = 1f;
						float num2 = 1f;
						num2 += Run.instance.difficultyCoefficient / 8f;
						num += Run.instance.difficultyCoefficient / 2f;
						num *= Mathf.Pow((float)SeedCount, 1f);
						num2 *= Mathf.Pow((float)SeedCount, 0.5f);
						if (team == TeamIndex.Monster)
						{
							num2 *= 0.75f; //nerf its damage into the ground for the player's sake
						}
						titan.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num - 1f) * 10f));
						titan.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num2 - 1f) * 10f));
						RegisterTitan(titan);
					}
				}
			};
			void RegisterTitan(CharacterMaster titan)
			{
				for (int i = 0; i < TitanList.GetLength(0); i++)
				{
					if (TitanList[i] == null)
					{
						TitanList[i] = titan;
						MonoBehaviour.print("aure id = " + i);
						break;
					}
				}
			}
			void KillTitans(TeamIndex team)
			{
				for (int i = 0; i < TitanList.GetLength(0); i++)
				{
					if (TitanList[i])
					{
						if (TitanList[i].teamIndex == team)
						{
							TitanList[i].TrueKill();
							TitanList[i] = null;
						}
					}
				}
			}
		}
		public static ConfigFile CustomConfigFile = new ConfigFile(Paths.ConfigPath + "\\kking117.AurelioniteStuck.cfg", true);

		public static ConfigEntry<int> spawnonphase_config = CustomConfigFile.Bind<int>("AurelioniteStuck Config", "Spawn On Boss Phase", 2, "Makes Aurelionite spawn on this phase of the Final Boss if players have the Halcyon Seed. (0 - 3) (0 = don't spawn)");
		private int spawnonphase = spawnonphase_config.Value;

		public static ConfigEntry<bool> spawnonsteal_config = CustomConfigFile.Bind<bool>("AurelioniteStuck Config", "Spawn Phase Four", true, "Allows the Final Boss to spawn his own Aurelionite when he steals all items. (Will get automatically killed when the boss is defeated.)");
		private bool spawnonsteal = spawnonsteal_config.Value;
	}
}
