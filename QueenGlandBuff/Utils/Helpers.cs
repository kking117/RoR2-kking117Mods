using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.CharacterAI;

namespace QueenGlandBuff.Utils
{
    internal class Helpers
    {
		private static readonly System.Random rng = new System.Random();
		internal static void GiveRandomEliteAffix(CharacterMaster self)
		{
			if(MainPlugin.Config_QueensGland_SpawnAffix.Value == 0)
            {
				return;
            }
			if (MainPlugin.Config_QueensGland_SpawnAffix.Value == 2 && !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
				return;
            }
			if (Changes.QueensGland.StageEliteEquipmentDefs.Count > 0)
            {
				int result = rng.Next(Changes.QueensGland.StageEliteEquipmentDefs.Count);
				if (Changes.QueensGland.StageEliteEquipmentDefs[result])
				{
					self.inventory.SetEquipmentIndex(Changes.QueensGland.StageEliteEquipmentDefs[result].equipmentIndex);
					return;
				}
			}
			self.inventory.SetEquipmentIndex(Changes.QueensGland.Gland_DefaultAffix_Var);
		}
		internal static int TeleportToOwner(CharacterBody self)
		{
			//0 = no owner/owner is dead
			//1 = simply couldn't find a spot to teleport to
			//2 = success
			if (self.master && self.master.minionOwnership && self.master.minionOwnership.ownerMaster)
			{
				CharacterMaster owner = (self.master.minionOwnership.ownerMaster);
				if (owner && owner.GetBody())
				{
					CharacterBody ownerBody = owner.GetBody();
					if (owner.teamIndex == self.teamComponent.teamIndex)
					{
						if (ownerBody.healthComponent.alive)
						{
							Vector3 ownerposition = ownerBody.corePosition;
							SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
							spawnCard.hullSize = self.hullClassification;
							spawnCard.nodeGraphType = (self.isFlying ? RoR2.Navigation.MapNodeGroup.GraphType.Air : RoR2.Navigation.MapNodeGroup.GraphType.Ground);
							spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
							GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Approximate,
								minDistance = 15f,
								maxDistance = 40f,
								position = ownerposition
							}, RoR2Application.rng));
							if (gameObject)
							{
								Vector3 position = gameObject.transform.position;
								TeleportHelper.TeleportBody(self, position);
								GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(self.gameObject);
								if (teleportEffectPrefab)
								{
									EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, true);
								}
								UnityEngine.Object.Destroy(spawnCard);
								UnityEngine.Object.Destroy(gameObject);
								return 2;
							}
							else
							{
								UnityEngine.Object.Destroy(spawnCard);
								return 1;
							}
						}
						else
						{
							return 0;
						}
					}
				}
			}
			return 0;
		}
	}
}
