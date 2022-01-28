using System;
using EntityStates;
using RoR2;
using UnityEngine;

namespace TotallyFairSkills.States
{
	public class ReloadPistolsFancy : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			PlayAnimation("Gesture, Override", "ReloadPistols", "ReloadPistols.playbackRate", duration);
			PlayAnimation("Gesture, Additive", "ReloadPistols", "ReloadPistols.playbackRate", duration);
			Transform transform = base.FindModelChild("GunMeshL");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			Transform transform2 = base.FindModelChild("GunMeshR");
			if (transform2 == null)
			{
				return;
			}
			transform2.gameObject.SetActive(false);
			Util.PlaySound(enterSoundString, gameObject);
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (isAuthority)
			{
				if (fixedAge >= duration * 0.8f && !HasReloaded)
				{
					HasReloaded = true;
					Util.PlaySound(reloadSoundString, gameObject);
				}
				if (fixedAge >= duration)
				{
					outer.SetNextStateToMain();
				}
			}
		}
		public override void OnExit()
		{
			Transform transform = base.FindModelChild("ReloadFXL");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			Transform transform2 = base.FindModelChild("ReloadFXR");
			if (transform2 != null)
			{
				transform2.gameObject.SetActive(false);
			}
			Transform transform3 = base.FindModelChild("GunMeshL");
			if (transform3 != null)
			{
				transform3.gameObject.SetActive(true);
			}
			Transform transform4 = base.FindModelChild("GunMeshR");
			if (transform4 != null)
			{
				transform4.gameObject.SetActive(true);
			}
			PlayAnimation("Gesture, Override", "ReloadPistolsExit");
			PlayAnimation("Gesture, Additive", "ReloadPistolsExit");
			base.OnExit();
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		private bool HasReloaded;
		private float duration;

		public static float baseDuration = 1.0f;
		public static string reloadSoundString = "Play_bandit_M2_load";
		public static string enterSoundString = "Play_bandit2_R_load";
	}
}
