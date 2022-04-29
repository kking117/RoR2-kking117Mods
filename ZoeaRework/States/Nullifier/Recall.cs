using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ZoeaRework.States.Nullifier
{
    public class Recall : BaseSkillState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration;
			GetTelportLocation();
			if(TeleLoc != null && ownerMaster != null)
            {
				EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, spawnMuzzleName, false);
				Teleport();
			}
			else
            {
				outer.SetNextStateToMain();
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!TeleportAnim && fixedAge >= 0.1f)
			{
				TeleportAnim = true;
				Util.PlaySound(soundString, gameObject);
				EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, spawnMuzzleName, false);
				if (isAuthority)
				{
					if (characterMotor != null)
					{
						characterMotor.velocity *= 0f;
					}
				}
			}
			if (fixedAge >= duration)
			{
				if (isAuthority)
				{
					outer.SetNextStateToMain();
					return;
				}
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		private void Teleport()
        {
			if (NetworkServer.active)
			{
				TeleportHelper.TeleportBody(characterBody, TeleLoc);
			}
			GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(characterBody.gameObject);
			if (teleportEffectPrefab)
			{
				EffectManager.SimpleEffect(teleportEffectPrefab, TeleLoc, Quaternion.identity, true);
			}
			PlayAnimation(animationLayerName, animationStateName, animationPlaybackRateParam, duration - fixedAge);
		}
		private void GetTelportLocation()
		{
			if (characterBody.master && characterBody.master.minionOwnership && characterBody.master.minionOwnership.ownerMaster)
			{
				CharacterMaster owner = (characterBody.master.minionOwnership.ownerMaster);
				if (characterBody.teamComponent.teamIndex == owner.teamIndex)
				{
					if (owner.GetBody())
					{
						if (owner.GetBody().healthComponent.alive)
						{
							ownerMaster = owner;
							Vector3 ownerposition = owner.GetBody().corePosition;
							SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
							spawnCard.hullSize = characterBody.hullClassification;
							spawnCard.nodeGraphType = (characterBody.isFlying ? RoR2.Navigation.MapNodeGroup.GraphType.Air : RoR2.Navigation.MapNodeGroup.GraphType.Ground);
							spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
							GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Approximate,
								minDistance = 20f,
								maxDistance = 45f,
								position = ownerposition
							}, RoR2Application.rng));
							if (gameObject)
							{
								TeleLoc = gameObject.transform.position;
								UnityEngine.Object.Destroy(gameObject);
							}
							UnityEngine.Object.Destroy(spawnCard);
						}
					}
				}
			}
		}


		public static float baseDuration = 4.05f;

		private float duration;
		private bool TeleportAnim = false;
		private Vector3 TeleLoc;
		private CharacterMaster ownerMaster;

		private string soundString = EntityStates.NullifierMonster.SpawnState.spawnSoundString;
		private string animationLayerName = "Body";
		private string animationStateName = "Spawn";
		private string animationPlaybackRateParam = "Spawn.playbackRate";
		private string spawnMuzzleName = "PortalEffect";
		private GameObject effectPrefab = EntityStates.NullifierMonster.SpawnState.spawnEffectPrefab;
	}
}
