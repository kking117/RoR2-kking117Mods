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

		public void ResetKill(CharacterMaster victim)
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
			int bestCount = 0;
			List<int> tieList = new List<int>();
			for (int i = 0; i < ChampionKills.Length; i++)
			{
				if (ChampionCard[i])
                {
					if (ChampionKills[i] > bestCount)
					{
						tieList = new List<int>();
						tieList.Add(i);
						result = ChampionCard[i];
						bestCount = ChampionKills[i];
					}
					else if (bestCount > 0 && ChampionKills[i] == bestCount)
					{
						tieList.Add(i);
					}
				}
			}
			if (tieList.Count > 0)
            {
				int maxRange = Math.Max(0, tieList.Count - 1);
				int roll = tieList[UnityEngine.Random.Range(0, maxRange)];
				if (roll > -1)
                {
					result = ChampionCard[roll];
					ChampionKills[roll] = 0;
				}
			}
			return result;
		}
	}
}
