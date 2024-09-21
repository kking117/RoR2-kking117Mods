using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using FlatItemBuff.Items;

namespace FlatItemBuff.Components
{
	public class SonorousWhispersTracker : MonoBehaviour
	{
		private int[] ChampionKills = new int[MasterCatalog.masterPrefabs.Length];
		public SpawnCard[] ChampionCard = new SpawnCard[MasterCatalog.masterPrefabs.Length];

		public void IncrementKill(CharacterMaster victim, int amount = 1)
        {
			if (victim && victim.GetBody())
            {
				int index = (int)victim.masterIndex;
				if (index > -1 && index < ChampionKills.Length)
				{
					ChampionKills[index] += amount;
					if (ChampionCard[index] == null)
					{
						ChampionCard[index] = GenerateSpawnCard(victim);
					}
				}
			}
        }

		private SpawnCard GenerateSpawnCard(CharacterMaster master)
        {
			CharacterBody body = master.GetBody();
			SpawnCard returnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
			returnCard.sendOverNetwork = true;
			returnCard.prefab = MasterCatalog.GetMasterPrefab(master.masterIndex);
			returnCard.hullSize = master.GetBody().hullClassification;
			returnCard.nodeGraphType = body.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground;
			returnCard.directorCreditCost = 0;
			returnCard.eliteRules = SonorousWhispers_Rework.BossCardTemplate.eliteRules;
			returnCard.forbiddenFlags = SonorousWhispers_Rework.BossCardTemplate.forbiddenFlags;
			returnCard.requiredFlags = SonorousWhispers_Rework.BossCardTemplate.requiredFlags;
			returnCard.name = MainPlugin.MODNAME + master.name;
			return returnCard;
		}

		public void ClearKills(CharacterMaster victim)
		{
			int index = (int)victim.masterIndex;
			if (index > -1 && index < ChampionKills.Length)
			{
				ChampionKills[index] = 0;
			}
		}

		public SpawnCard DoSelection()
		{
			SpawnCard result = SonorousWhispers_Rework.BaseSpawnCard;
			List<int> tieListWeight = new List<int>();
			List<int> tieListIndex = new List<int>();
			int maxRoll = 0;
			for (int i = 0; i < ChampionKills.Length; i++)
			{
				if (ChampionCard[i] && ChampionKills[i] > 0)
                {
					maxRoll += ChampionKills[i];
					tieListWeight.Add(maxRoll);
					tieListIndex.Add(i);
				}
			}
			if (tieListIndex.Count > 0 && maxRoll > 0)
            {
				//Does RangeInt cap at maxRange - 1, seems like it?
				int roll = Run.instance.runRNG.RangeInt(1, maxRoll + 1);
				if (roll > maxRoll)
                {
					roll = maxRoll;
				}
				//MainPlugin.ModLogger.LogInfo("rng roll = " + roll + ", max is: " + maxRoll);
				if (roll > -1)
                {
					for(int i = 0; i<tieListWeight.Count; i++)
                    {
						if (roll <= tieListWeight[i])
                        {
							//MainPlugin.ModLogger.LogInfo("Chose index " + tieListIndex[i] + " with weight " + tieListWeight[i]);
							int index = tieListIndex[i];
							result = ChampionCard[index];
							ChampionKills[index] = 1;
							break;
                        }
                    }
				}
			}
			return result;
		}
	}
}
