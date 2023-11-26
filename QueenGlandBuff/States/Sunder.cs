using System;
using System.Linq;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using QueenGlandBuff.Changes;

namespace QueenGlandBuff.States
{
	public class Sunder : BaseSkillState
	{
		public static float baseDuration = 1.5f;
		public static float soundtime = 3f;

		public static float damageCoefficient = 4f;
		public static float forceMagnitude = 16f;

		public static float RockDamageCoefficient = 1.25f;
		public static int RockCount = 3;
		public static float RockSpeed = 55.0f;
		public static float RockFanSpreadAngle = 2f;
		public static float RockYLocOffset = 1f;
		public static float RockYawAdd = 2f;
		public static float RockYawOffset = -9f;

		public static string initialAttackSoundString = EntityStates.BeetleGuardMonster.FireSunder.initialAttackSoundString;
		//RoR2/Base/Mage/MageLightningboltBasic.prefab
		public static GameObject chargeEffectPrefab = EntityStates.BeetleGuardMonster.FireSunder.chargeEffectPrefab;
		public static GameObject hitEffectPrefab = EntityStates.BeetleGuardMonster.FireSunder.hitEffectPrefab;

		public static GameObject SunderProjectile = EntityStates.BeetleGuardMonster.FireSunder.projectilePrefab;

		private UnityEngine.Animator modelAnimator;

		private Transform modelTransform;

		private bool hasAttacked;
		private float duration;
		private bool crit;

		private GameObject rightHandChargeEffect;
		private ChildLocator modelChildLocator;
		private Transform handRTransform;
		public override void OnEnter()
		{
			base.OnEnter();
			modelAnimator = GetModelAnimator();
			modelTransform = GetModelTransform();
			duration = baseDuration / attackSpeedStat;
			Util.PlayAttackSpeedSound(initialAttackSoundString, gameObject, attackSpeedStat * (soundtime / duration));
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
			crit = characterBody.RollCrit();
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
					FireProjectiles();
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
		private void FireProjectiles()
        {
			if (isAuthority)
			{
				Ray aimRay = GetAimRay();
				aimRay.origin = handRTransform.position;
				if (BeetleGuardAlly.Elite_Skills)
				{
					if (characterBody.HasBuff(RoR2Content.Buffs.AffixLunar))
					{
						Perfected_FanRocks();
						ProjectileManager.instance.FireProjectile(BeetleGuardAlly.Perfected_Sunder_MainProjectile, handRTransform.position, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, -1f);
						return;
					}
				}
				ProjectileManager.instance.FireProjectile(SunderProjectile, handRTransform.position, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, -1f);
				FanRocks();
			}
		}
		private void Perfected_FanRocks()
		{
			Ray baseaimRay = GetAimRay();
			Ray finalaimRay = GetAimRay();
			finalaimRay.origin = handRTransform.position;
			finalaimRay.origin += new Vector3(0f, RockYLocOffset, 0f);
			int ShardCount = 5;
			float damage = RockCount * RockDamageCoefficient / (float)ShardCount / 10f;
			float speed = RockSpeed * 1.25f;

			GameObject homeTarget = null;
			HurtBox hurtbox;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = baseaimRay.origin;
			bullseyeSearch.searchDirection = baseaimRay.direction;
			bullseyeSearch.maxDistanceFilter = 150f;
			bullseyeSearch.maxAngleFilter = 10f;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(characterBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			hurtbox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (hurtbox)
			{
				homeTarget = HurtBox.FindEntityObject(hurtbox);
				if (homeTarget)
				{
					finalaimRay.direction = hurtbox.transform.position - finalaimRay.origin;
				}
			}
			float spreadAngle = (ShardCount / 2f) + 0.5f;
			for (int i = 0; i < ShardCount; i++)
			{
				spreadAngle -= 1f;
				Vector3 direction = Util.ApplySpread(finalaimRay.direction, 0f, 0f, 1f, 1f, spreadAngle * RockFanSpreadAngle, 0f);
				ProjectileManager.instance.FireProjectile(BeetleGuardAlly.Perfected_Sunder_RockProjectile, finalaimRay.origin, Util.QuaternionSafeLookRotation(direction), gameObject, damageStat * damage, forceMagnitude, crit, DamageColorIndex.Default, homeTarget, speed);
			}
		}
		private void FanRocks()
        {
			Ray baseaimRay = GetAimRay();
			Ray finalaimRay = GetAimRay();

			finalaimRay.origin = handRTransform.position;
			finalaimRay.origin += new Vector3(0f, RockYLocOffset, 0f);

			GameObject homeTarget = null;
			HurtBox hurtbox;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = baseaimRay.origin;
			bullseyeSearch.searchDirection = baseaimRay.direction;
			bullseyeSearch.maxDistanceFilter = 90f;
			bullseyeSearch.maxAngleFilter = 10f;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(characterBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.RefreshCandidates();
			hurtbox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (hurtbox)
			{
				homeTarget = HurtBox.FindEntityObject(hurtbox);
				if (homeTarget)
                {
					finalaimRay.direction = hurtbox.transform.position - finalaimRay.origin;
					finalaimRay.direction.Normalize();
				}
			}
			float yawBonus = 0f;
			if (finalaimRay.direction.y < 0.93f || finalaimRay.direction.y > 0.0f)
			{
				yawBonus = RockYawOffset;
			}
			float spreadAngle = (RockCount / 2f) + 0.5f;
			for (int i = 0; i < RockCount; i++)
			{
				spreadAngle -= 1f;
				float yawAngle = spreadAngle;
				if (yawAngle < 0f)
				{
					yawAngle *= -1f;
				}
				yawAngle *= RockYawAdd;
				Vector3 direction = Util.ApplySpread(finalaimRay.direction, 0f, 0f, 1f, 1f, spreadAngle * RockFanSpreadAngle, yawBonus + yawAngle);
				ProjectileManager.instance.FireProjectile(BeetleGuardAlly.Default_RockProjectile, finalaimRay.origin, Util.QuaternionSafeLookRotation(direction), gameObject, damageStat * RockDamageCoefficient, forceMagnitude, crit, DamageColorIndex.Default, null, RockSpeed);
			}
		}
	}
}
