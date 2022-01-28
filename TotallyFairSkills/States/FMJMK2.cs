using System;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace TotallyFairSkills.States
{
	public class FMJMK2 : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			Ray aimRay = GetAimRay();
			StartAimMode(aimRay, 2f, false);

			PlayAnimation("Gesture, Additive", "FireFMJ", "FireFMJ.playbackRate", duration);
			PlayAnimation("Gesture, Override", "FireFMJ", "FireFMJ.playbackRate", duration);
			string muzzleName = "MuzzleCenter";
			Util.PlaySound(fireSoundString, gameObject);
			AddRecoil(-0.8f * recoilAmplitude, -1f * recoilAmplitude, -0.1f * recoilAmplitude, 0.15f * recoilAmplitude);
			if (effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, muzzleName, false);
			}
			ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, characterBody.master), DamageColorIndex.Default, null, projSpeed);
			characterBody.AddSpreadBloom(spreadBloomValue);
		}
		public override void OnExit()
		{
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (isAuthority)
			{
				if (fixedAge >= duration)
				{
					if (Main.FMJMK2_ActionArmy.Value)
					{
						outer.SetNextState(new ReloadPistolsFancy());
					}
					else
                    {
						outer.SetNextStateToMain();
					}
					return;
				}
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (fixedAge > duration * nextShot)
            {
				return InterruptPriority.Any;
			}
			else
            {
				return InterruptPriority.PrioritySkill;
			}
			
		}

		public static GameObject projectilePrefab = Modules.Projectiles.FMJMK2Prefab;
		public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashfmj");
		public static string fireSoundString = "Play_commando_M2";

		public static float damageCoefficient = Main.FMJMK2_Damage.Value;
		public static float projSpeed = 400f;
		public static float force = Main.FMJMK2_Force.Value;
		public static float recoilAmplitude = 3f;
		public static float spreadBloomValue = 0.8f;

		public static float baseDuration = 0.6f;
		public static float nextShot = 0.8f;
		private float duration;
	}
}
