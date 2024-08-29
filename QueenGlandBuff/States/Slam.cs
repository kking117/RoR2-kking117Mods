using System;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using QueenGlandBuff.Changes;

namespace QueenGlandBuff.States
{
	public class Slam : BaseSkillState
	{
		public static float baseDuration = 1.75f;
		public static float soundtime = 3.5f;

		public static float damageCoefficient = 4f;
		public static float forceMagnitude = 16f;

		public static float RockDamageCoefficient = 1.25f;
		public static int RockCount = 3;
		public static float RockSpeed = 25.0f;
		private static float RockRotateOffset = 9f;

		private OverlapAttack attack;

		public static string initialAttackSoundString = EntityStates.BeetleGuardMonster.GroundSlam.initialAttackSoundString;
		public static GameObject chargeEffectPrefab = EntityStates.BeetleGuardMonster.GroundSlam.chargeEffectPrefab;
		public static GameObject slamEffectPrefab = EntityStates.BeetleGuardMonster.GroundSlam.slamEffectPrefab;
		public static GameObject hitEffectPrefab = EntityStates.BeetleGuardMonster.GroundSlam.hitEffectPrefab;

		private Animator modelAnimator;
		private Transform modelTransform;

		private bool hasAttacked;
		private float duration;
		private bool crit;

		private GameObject leftHandChargeEffect;
		private GameObject rightHandChargeEffect;

		private ChildLocator modelChildLocator;
		private Transform groundSlamIndicatorInstance;
		private void EnableIndicator(Transform indicator)
		{
			if (indicator)
			{
				indicator.gameObject.SetActive(true);
				ObjectScaleCurve component = indicator.gameObject.GetComponent<ObjectScaleCurve>();
				if (component)
				{
					component.time = 0f;
				}
			}
		}
		private void DisableIndicator(Transform indicator)
		{
			if (indicator)
			{
				indicator.gameObject.SetActive(false);
			}
		}
		public override void OnEnter()
		{
			base.OnEnter();
			modelAnimator = GetModelAnimator();
			modelTransform = GetModelTransform();
			attack = new OverlapAttack();
			attack.attacker = gameObject;
			attack.inflictor = gameObject;
			attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
			attack.damage = damageCoefficient * damageStat;
			attack.hitEffectPrefab = hitEffectPrefab;
			attack.forceVector = Vector3.up * forceMagnitude;
			if (modelTransform)
			{
				attack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "GroundSlam");
			}
			duration = baseDuration / attackSpeedStat;
			Util.PlayAttackSpeedSound(initialAttackSoundString, gameObject, attackSpeedStat * (soundtime / duration));
			//PlayCrossfade("Body", "GroundSlam", "GroundSlam.playbackRate", duration, 0.2f);
			PlayAnimation("Body", "GroundSlam", "GroundSlam.playbackRate", duration);
			if (modelTransform)
			{
				modelChildLocator = modelTransform.GetComponent<ChildLocator>();
				if (modelChildLocator)
				{
					GameObject original = chargeEffectPrefab;
					Transform transform = modelChildLocator.FindChild("HandL");
					Transform transform2 = modelChildLocator.FindChild("HandR");
					if (transform)
					{
						leftHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform);
					}
					if (transform2)
					{
						rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, transform2);
					}
					groundSlamIndicatorInstance = modelChildLocator.FindChild("GroundSlamIndicator");
					EnableIndicator(groundSlamIndicatorInstance);
				}
			}
		}
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(leftHandChargeEffect);
			EntityState.Destroy(rightHandChargeEffect);
			DisableIndicator(groundSlamIndicatorInstance);
		}

		//TODO: Figure out why this is necessary
        public override void PlayAnimation(string layerName, int animationStateHash)
        {
            //base.PlayAnimation(layerName, animationStateHash);
        }
        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (modelAnimator && modelAnimator.GetFloat("GroundSlam.hitBoxActive") > 0.5f && !hasAttacked)
			{
				crit = characterBody.RollCrit();
				if (NetworkServer.active)
				{
					attack.isCrit = crit;
					attack.Fire(null);
				}
				if (isAuthority && modelTransform)
				{
					FireProjectiles();
				}
				hasAttacked = true;
				EntityState.Destroy(leftHandChargeEffect);
				EntityState.Destroy(rightHandChargeEffect);
			}
			if (fixedAge >= duration && isAuthority)
			{
				outer.SetNextStateToMain();
				return;
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
		private void FireProjectiles()
		{
			DisableIndicator(groundSlamIndicatorInstance);
			EffectManager.SimpleMuzzleFlash(slamEffectPrefab, gameObject, "SlamZone", true);
			if (isAuthority)
			{
				if (BeetleGuardAlly.Elite_Skills)
				{
					if (characterBody.HasBuff(RoR2Content.Buffs.AffixLunar))
					{
						Perfected_SpewRocks();
						return;
					}
				}
				SpewRocks();
			}
		}
		//Overloading_Slam_RockProjectile
		private void Perfected_SpewRocks()
        {
			float damage = RockCount * RockDamageCoefficient / 8f;
			ProjectileManager.instance.FireProjectile(BeetleGuardAlly.Perfected_Slam_RockProjectile, modelChildLocator.FindChild("GroundSlamIndicator").position, Quaternion.identity, gameObject, damageStat * damage, 1f, crit, DamageColorIndex.Default, null, -1f);
		}
		private void SpewRocks()
        {
			Vector3 ShotPos = modelChildLocator.FindChild("GroundSlamIndicator").position;
			Vector3 ShotAngle = characterDirection.forward;
			float baseangle = UnityEngine.Random.Range(0f, 360f);
			for (int i = 0; i < RockCount; i++)
			{
				Vector3 ShotAngleTemp = ShotAngle;
				ShotAngleTemp = Quaternion.AngleAxis(baseangle + (360f / RockCount * (i - RockCount / 2f)), Vector3.up) * ShotAngle;
				ShotAngleTemp.y = RockRotateOffset;
				ProjectileManager.instance.FireProjectile(BeetleGuardAlly.Default_RockProjectile, ShotPos + ShotAngleTemp.normalized, Util.QuaternionSafeLookRotation(ShotAngleTemp), gameObject, damageStat * RockDamageCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, RockSpeed);
			}
		}
	}
}
