using System;
using System.Linq;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;

namespace QueenGlandBuff.States
{
	public class Sunder : BaseSkillState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			modelAnimator = GetModelAnimator();
			modelTransform = GetModelTransform();
			duration = baseDuration / attackSpeedStat;
			Util.PlayAttackSpeedSound(initialAttackSoundString, gameObject, attackSpeedStat * (soundtime / duration));
			//PlayCrossfade("Body", "FireSunder", "FireSunder.playbackRate", duration, 0.2f);
			PlayAnimation("Body", "FireSunder", "FireSunder.playbackRate", duration);
			if (characterBody)
			{
				characterBody.SetAimTimer(duration + 2f);
			}
			if (modelTransform)
			{
				AimAnimator component = modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = true;
				}
				modelChildLocator = modelTransform.GetComponent<ChildLocator>();
				if (modelChildLocator)
				{
					GameObject original = chargeEffectPrefab;
					handRTransform = modelChildLocator.FindChild("HandR");
					if (handRTransform)
					{
						rightHandChargeEffect = UnityEngine.Object.Instantiate<GameObject>(original, handRTransform);
					}
				}
			}
		}
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(rightHandChargeEffect);
			if (modelTransform)
			{
				AimAnimator component = modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = true;
				}
			}
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (modelAnimator && modelAnimator.GetFloat("FireSunder.activate") > 0.5f && !hasAttacked)
			{
				if (isAuthority && modelTransform)
				{
					crit = characterBody.RollCrit();
					Ray aimRay = GetAimRay();
					if (characterBody.HasBuff(RoR2Content.Buffs.AffixLunar))
					{
						ProjectileManager.instance.FireProjectile(projectile3Prefab, handRTransform.position, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, -1f);
						ShootShards();
					}
					else
					{
						ProjectileManager.instance.FireProjectile(projectile1Prefab, handRTransform.position, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, -1f);
						ShootRocks();
					}
				}
				hasAttacked = true;
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
		private void ShootShards()
		{
			Ray aimRay = GetAimRay();
			Vector3 ShotAngle = aimRay.direction;
			Vector3 ShotPos = handRTransform.position;
			ShotPos.y += 1f;
			ShotAngle.Normalize();

			float damage = projCount * projdmgCoefficient / 12f / 10f;
			float speed = (projSpeed + projSpeedRng) * 1.25f;
			float force = projectile1Prefab.GetComponent<ProjectileDamage>().force;

			GameObject finaltarget = null;
			HurtBox hurtbox;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.maxDistanceFilter = 150f;
			bullseyeSearch.maxAngleFilter = 10f;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(characterBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			hurtbox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (hurtbox)
			{
				finaltarget = HurtBox.FindEntityObject(hurtbox);
				ShotAngle = finaltarget.transform.position - ShotPos;
				ShotAngle.Normalize();
			}
			Vector3 axis = Vector3.Cross(Vector3.up, ShotAngle);
			for (int i = 0; i < 12; i++)
			{
				Vector3 ShotAngleTemp = ShotAngle;
				if (i != 0)
				{
					float x = UnityEngine.Random.Range(0, spreadangle);
					if (finaltarget)
					{
						x = UnityEngine.Random.Range(0, spreadangle * 2.5f);
					}
					float z = UnityEngine.Random.Range(0f, 360f);
					Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
					float y = vector.y;
					vector.y = 0f;
					float angle = (Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f) * 1f;
					float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f * 1f;
					ShotAngleTemp = Quaternion.AngleAxis(angle, Vector3.up) * (Quaternion.AngleAxis(angle2, axis) * ShotAngleTemp);
				}
				ShotAngleTemp.Normalize();
				if (ShotAngleTemp.y < -0.125f)
				{
					ShotAngleTemp.y = -0.125f;
				}
				ProjectileManager.instance.FireProjectile(projectile4Prefab, ShotPos + ShotAngleTemp.normalized, Util.QuaternionSafeLookRotation(ShotAngleTemp), gameObject, damageStat * damage, 1f, crit, DamageColorIndex.Default, finaltarget, speed);
			}
		}
		private void ShootRocks()
		{
			Ray aimRay = GetAimRay();
			Vector3 ShotAngle = aimRay.direction;
			Vector3 ShotPos = handRTransform.position;
			ShotPos.y += 1f;
			ShotAngle.Normalize();

			if (!characterBody.isPlayerControlled)
			{
				GameObject finaltarget = null;
				HurtBox hurtbox;
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.maxDistanceFilter = 100f;
				bullseyeSearch.maxAngleFilter = 10f;
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				bullseyeSearch.teamMaskFilter.RemoveTeam(characterBody.teamComponent.teamIndex);
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
				bullseyeSearch.RefreshCandidates();
				hurtbox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
				if (hurtbox)
				{
					finaltarget = HurtBox.FindEntityObject(hurtbox);
					ShotAngle = finaltarget.transform.position - ShotPos;
					ShotAngle.Normalize();
				}
			}
			else
			{
				ShotAngle = (aimRay.origin + aimRay.direction * 100f) - ShotPos;
				ShotAngle.Normalize();
			}
			ShotAngle.y += 0.2f;
			ShotAngle.Normalize();
			int spreadcount = projCount - 1;
			Vector3 axis = Vector3.Cross(Vector3.up, ShotAngle);
			float baseangle = UnityEngine.Random.Range(0f, 360f);
			for (int i = 0; i < projCount; i++)
			{
				Vector3 ShotAngleTemp = ShotAngle;
				if (i != 0)
				{
					float x = spreadangle;
					float z = (i - 1) * (360f / spreadcount);
					z += baseangle;
					Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
					float y = vector.y;
					vector.y = 0f;
					float angle = (Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f) * 1f;
					float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f * 1f;
					ShotAngleTemp = Quaternion.AngleAxis(angle, Vector3.up) * (Quaternion.AngleAxis(angle2, axis) * ShotAngleTemp);
				}
				ShotAngleTemp.Normalize();
				if (ShotAngleTemp.y < -0.125f)
				{
					ShotAngleTemp.y = -0.125f;
				}
				ProjectileManager.instance.FireProjectile(projectile2Prefab, ShotPos + ShotAngleTemp.normalized, Util.QuaternionSafeLookRotation(ShotAngleTemp), gameObject, damageStat * projdmgCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, projSpeed + UnityEngine.Random.Range(0.0f, projSpeedRng));
			}
		}

		public static float baseDuration = 1.5f;
		public static float soundtime = 3f;

		public static float damageCoefficient = 4f;
		public static float forceMagnitude = 16f;

		public static float projdmgCoefficient = 0.75f;
		public static int projCount = 5;
		public static float projSpeed = 50.0f;
		public static float projSpeedRng = 5.0f;
		public static float spreadangle = 3f;

		public static string initialAttackSoundString = EntityStates.BeetleGuardMonster.FireSunder.initialAttackSoundString;

		public static GameObject chargeEffectPrefab = EntityStates.BeetleGuardMonster.FireSunder.chargeEffectPrefab;
		public static GameObject hitEffectPrefab = EntityStates.BeetleGuardMonster.FireSunder.hitEffectPrefab;

		public static GameObject projectile1Prefab = EntityStates.BeetleGuardMonster.FireSunder.projectilePrefab;
		public static GameObject projectile2Prefab = ItemChanges.QueensGland.SlamRockProjectile;
		public static GameObject projectile3Prefab = MainPlugin.Perfect_Sunder_MainProj;
		public static GameObject projectile4Prefab = MainPlugin.Perfect_Sunder_SecProj;

		private UnityEngine.Animator modelAnimator;

		private Transform modelTransform;

		private bool hasAttacked;
		private float duration;
		private bool crit;

		private GameObject rightHandChargeEffect;
		private ChildLocator modelChildLocator;
		private Transform handRTransform;
	}
}
