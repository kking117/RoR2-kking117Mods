using System;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace FlatItemBuff.Items.Behaviors
{
    public class BloodClone : CharacterBody.ItemBehavior
    {
		private float leashDistance = Items.Infusion_Rework.CustomLeash;
		private GameObject helperPrefab;
		private RigidbodyMotor rigidbodyMotor;

		private float teleportAttemptTimer = 10f;

		private const float minDistance = 10f;
		private const float maxDistance = 40f;
		private const float teleportDelayTime = 10f;

		public void Start()
		{
			if (body.hasEffectiveAuthority)
			{
				helperPrefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
				rigidbodyMotor = GetComponent<RigidbodyMotor>();
			}
		}
		private void FixedUpdate()
		{
			if (!body.hasEffectiveAuthority)
			{
				return;
			}
			teleportAttemptTimer -= Time.fixedDeltaTime;
			if (teleportAttemptTimer <= 0f)
			{
				teleportAttemptTimer = teleportDelayTime;
				CharacterMaster master = body.master;
				if (master)
				{
					CharacterMaster owner = master.minionOwnership.ownerMaster;
					if (owner)
					{
						CharacterBody characterBody = owner.GetBody();
						if (!characterBody)
						{
							return;
						}
						Vector3 corePosition = characterBody.corePosition;
						Vector3 corePosition2 = body.corePosition;
						if (CanMove() && (corePosition2 - corePosition).sqrMagnitude > leashDistance)
						{
							SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
							spawnCard.hullSize = body.hullClassification;
							spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
							spawnCard.prefab = helperPrefab;
							GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Approximate,
								position = corePosition,
								minDistance = minDistance,
								maxDistance = maxDistance
							}, RoR2Application.rng));
							if (gameObject)
							{
								Vector3 position = gameObject.transform.position;
								if ((position - corePosition).sqrMagnitude < leashDistance)
								{
									TeleportHelper.TeleportBody(body, position);
									GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(body.gameObject);
									if (teleportEffectPrefab)
									{
										EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, true);
									}
									UnityEngine.Object.Destroy(gameObject);
								}
							}
							UnityEngine.Object.Destroy(spawnCard);
						}
					}
				}
			}
		}
		private bool CanMove()
        {
			if (body.characterMotor && body.characterMotor.walkSpeed > 0f)
            {
				return true;
            }
			if (rigidbodyMotor && body.moveSpeed > 0f)
            {
				return true;
			}
			return false;
        }
	}
}
