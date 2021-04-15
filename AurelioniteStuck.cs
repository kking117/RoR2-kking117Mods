using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AurelioniteStuck
{
	[BepInPlugin("com.kking117.AurelioniteStuck", "AurelioniteStuck", "2.0.0")]
	[BepInDependency("com.bepis.r2api")]
	public class MainPlugin : BaseUnityPlugin
	{
		private static GameObject TitanHostile = Resources.Load<GameObject>("prefabs/charactermasters/TitanGoldMaster");
		private static GameObject TitanFriendly = Resources.Load<GameObject>("prefabs/charactermasters/TitanGoldAllyMaster");

		private CombatSquad[] TitanTeam = new CombatSquad[4];

		public static ConfigEntry<int> Config_SpawnOnPhase;
		public static ConfigEntry<bool> Config_KillOnSteal;
		public static ConfigEntry<bool> Config_SpawnOnSteal;
		public static ConfigEntry<bool> Config_Debug;

		private float[] NextTitanSpawn = new float[4]
		{
			-1f,
			-1f,
			-1f,
			-1f
		};

		private float[] NextTitanKill = new float[4]
		{
			-1f,
			-1f,
			-1f,
			-1f
		};

		private int BossSeedCount;

		Vector3 spawnposition = new Vector3(-87.4f, 491.95f, -2.54f);
		Quaternion spawnrotation = new Quaternion(0.0f, 0.5f, 0.0f, 0.0f);
		public void Awake()
		{
			ReadConfig();
			On.RoR2.Run.Start += (orig, self) =>
			{
				orig(self);
				for (int i = 0; i < NextTitanSpawn.Length; i++)
				{
					NextTitanSpawn[i] = -1f;
					NextTitanKill[i] = -1f;
				}
			};
			On.RoR2.Run.FixedUpdate += (orig, self) =>
			{
				orig(self);
				string scenename = SceneManager.GetActiveScene().name;
				if (scenename == "moon" || scenename == "moon2")
				{
					for (int i = 0; i < NextTitanSpawn.Length; i++)
					{
						if (NextTitanSpawn[i] >= 0f)
						{
							if (NextTitanSpawn[i] <= Time.time)
							{
								if (i == 2)
								{
									CreateTitanGoldCombatSquad((TeamIndex)i, BossSeedCount);
								}
								else
								{
									CreateTitanGoldCombatSquad((TeamIndex)i, GetTeamSeedCount((TeamIndex)i));
								}
								NextTitanSpawn[i] = -1f;
							}
						}
						if (NextTitanKill[i] >= 0f)
						{
							if (NextTitanKill[i] <= Time.time)
							{
								KillTitanSquad(TitanTeam[i]);
								NextTitanKill[i] = -1f;
							}
						}
					}
				}
			};

			On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += (orig, self) =>
			{
				orig(self);
				if (Config_SpawnOnPhase.Value == 1)
				{
					NextTitanSpawn[1] = Time.time + 5.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += (orig, self) =>
			{
				orig(self);
				if (Config_SpawnOnPhase.Value == 2)
				{
					NextTitanSpawn[1] = Time.time + 3.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += (orig, self) =>
			{
				orig(self);
				if (Config_SpawnOnPhase.Value == 3)
				{
					NextTitanSpawn[1] = Time.time + 5.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += (orig, self) =>
			{
				orig(self);
				if(Config_KillOnSteal.Value)
                {
					NextTitanKill[1] = Time.time + 7.5f;
				}
				if (Config_SpawnOnSteal.Value)
				{
					BossSeedCount = GetTeamSeedCount(TeamIndex.Player);
					NextTitanSpawn[2] = Time.time + 12.0f;
				}
			};
			On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += (orig, self) =>
			{
				orig(self);
				for(int i = 0; i<NextTitanSpawn.Length; i++)
                {
					NextTitanSpawn[i] = -1f;
					NextTitanKill[i] = Time.time + 2.5f;
				}
				
			};
			On.RoR2.GoldTitanManager.OnBossGroupStartServer += (orig, bossGroup) =>
			{
				bool IsMithrix = false;
				CombatSquad combatSquad = bossGroup.combatSquad;
				using (IEnumerator<CharacterMaster> enumerator = combatSquad.readOnlyMembersList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.masterIndex == MasterCatalog.FindMasterIndex("BrotherHurtMaster"))
						{
							IsMithrix = true;
							break;
						}
					}
				}
				if(IsMithrix)
                {
                }
				else
                {
					orig(bossGroup);
				}
			};
			int GetTeamSeedCount(TeamIndex team)
			{
				int SeedCount = 0;
				SeedCount = Util.GetItemCountForTeam(team, RoR2Content.Items.TitanGoldDuringTP.itemIndex, true, true);
				return SeedCount;
			};
			void CreateTitanGoldCombatSquad(TeamIndex team, int SeedCount)
			{
				if (SeedCount > 0)
				{
					CombatSquad combatSquad;
					if(team == TeamIndex.Player)
                    {
						combatSquad = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/MonstersOnShrineUseEncounter")).GetComponent<CombatSquad>();
					}
					else
                    {
						combatSquad = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/ShadowCloneEncounter")).GetComponent<CombatSquad>();
					}
					int SpawnCount = 1;
					//This is just here really.
					//Like, swarms doesn't double aurelionite so this shouldn't be needed
					/*if (RunArtifactManager.instance.IsArtifactEnabled(ArtifactCatalog.FindArtifactDef("Swarms")))
                    {
						if (Config_Debug.Value)
						{
							print("Doubled Aurelionite for Swarms");
						}
					}*/
					for (int i = 0; i < SpawnCount; i++)
					{
						Vector3 offsetpos = GetRandomSpawnPos();
						CharacterMaster component = new MasterSummon
						{
							masterPrefab = team == TeamIndex.Player ? TitanFriendly : TitanHostile,
							position = spawnposition + offsetpos,
							summonerBodyObject = null,
							ignoreTeamMemberLimit = true,
							teamIndexOverride = team,
						}.Perform();
						AddTitanInventory(component, SeedCount);
						combatSquad.AddMember(component);
					}
					if (combatSquad)
					{
						NetworkServer.Spawn(combatSquad.gameObject);
						RegisterTitanSquad(combatSquad, team);
					}
				}
			}

			Vector3 GetRandomSpawnPos()
			{
				Vector3 angle = new Vector3(1f, 1f, 1f);
				//Rotate the angle by a random degree
				float rotation = UnityEngine.Random.Range(0.0f, 360.0f);
				float x = angle[0];
				float y = angle[2];
				angle[1] = 0f;
				angle[0] = x * (float)Math.Cos(rotation) - y * (float)Math.Sin(rotation);
				angle[2] = x * (float)Math.Sin(rotation) + y * (float)Math.Cos(rotation);
				//Then normalize the angles
				angle = Vector3.Normalize(angle);
				angle *= UnityEngine.Random.Range(20f, 40f);
				print(angle);
				return angle;
			}
			void AddTitanInventory(CharacterMaster titan, int seeds)
            {
				float num = 1f;
				float num2 = 1f;
				num += Run.instance.difficultyCoefficient / 2f;
				num2 += Run.instance.difficultyCoefficient / 2f;
				num *= Mathf.Pow((float)seeds, 1f);
				num2 *= Mathf.Pow((float)seeds, 0.5f);
				if (titan.teamIndex != TeamIndex.Player)
				{
					num = Mathf.Pow(num2, 0.5f);
					num2 = Mathf.Pow(num2, 0.5f);
				}
				titan.inventory.GiveItem(RoR2Content.Items.BoostHp.itemIndex, Mathf.RoundToInt((num - 1f) * 10f));
				titan.inventory.GiveItem(RoR2Content.Items.BoostDamage.itemIndex, Mathf.RoundToInt((num2 - 1f) * 10f));
				if (Config_Debug.Value)
				{
					print("Aurelionite spawned with " + (10 + Mathf.RoundToInt((num - 1f) * 10f)) * 10.0f + "% health.");
					print("Aurelionite spawned with " + (10 + Mathf.RoundToInt((num2 - 1f) * 10f)) * 10.0f + "% damage.");
				}
			}
			void RegisterTitanSquad(CombatSquad combatSquad, TeamIndex team)
			{
				int slot = (int)team;
				if(slot>= 0 && slot<=TitanTeam.GetLength(0))
                {
					KillTitanSquad(TitanTeam[slot]);
					TitanTeam[slot] = combatSquad;
				}
			}
			void KillTitanSquad(CombatSquad combatSquad)
            {
				CharacterMaster[] HitList = new CharacterMaster[0];
				if (combatSquad)
				{
					using (IEnumerator<CharacterMaster> enumerator = combatSquad.readOnlyMembersList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current)
							{
								if (enumerator.Current.GetBody())
								{
									if (enumerator.Current.GetBody().healthComponent.alive)
									{
										Array.Resize<CharacterMaster>(ref HitList, HitList.Length + 1);
										HitList[HitList.Length - 1] = enumerator.Current;
									}
								}
							}
						}
					}
				}
				for(int i = 0; i< HitList.Length; i++)
                {
					if(HitList[i])
                    {
						HitList[i].TrueKill();
                    }
                }
			}
		}

		public void ReadConfig()
        {
			Config_SpawnOnPhase = Config.Bind<int>(new ConfigDefinition("AurelioniteStuck", "Spawn Phase"), 2, new ConfigDescription("Spawns Aurelionite on this phase during the Final Boss if players have the Halcyon Seed. (0-3) (0 = Don't spawn)", null, Array.Empty<object>()));
			Config_SpawnOnSteal = Config.Bind<bool>(new ConfigDefinition("AurelioniteStuck", "Spawn On Last Phase"), true, new ConfigDescription("Spawns an Aurelionite for Mithrix on the item steal phase. (Gets automatically killed if Mithrix dies)", null, Array.Empty<object>()));
			Config_KillOnSteal = Config.Bind<bool>(new ConfigDefinition("AurelioniteStuck", "Kill On Last Phase"), true, new ConfigDescription("Kill the player's Aurelionite on Mithrix's item steal phase.", null, Array.Empty<object>()));
			Config_Debug = Config.Bind<bool>(new ConfigDefinition("AurelioniteStuck", "Spam Console"), false, new ConfigDescription("Prints console messages for debug purposes.", null, Array.Empty<object>()));
		}
	}
}
