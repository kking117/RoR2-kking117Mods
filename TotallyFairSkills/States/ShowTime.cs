using System;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TotallyFairSkills.States
{
	public class ShowTime : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration;
			maxboostDuration = duration * jetDuration;
			if (characterMotor.velocity.magnitude > 1f)
			{
				characterBody.isSprinting = true;
				FireJets();
			}
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
			bool IsGrounded = characterMotor.isGrounded;
			if (fixedAge >= duration)
			{
				if (isAuthority)
				{
					outer.SetNextStateToMain();
				}
				return;
			}
			else if (fixedAge >= duration * 0.8f && !HasReloaded)
			{
				HasReloaded = true;
				if (NetworkServer.active)
				{
					characterBody.AddTimedBuff(Modules.Buffs.ShowOff, buffDuration + (fixedAge - duration));
				}
				Util.PlaySound(reloadSoundString, gameObject);
				ReloadSkills();
			}
			if (fixedAge <= duration * 0.6f)
			{
				if (!HasBoosted)
				{
					if (characterMotor.velocity.y != 0f || inputBank.moveVector.magnitude > 0.2f)
					{
						FireJets();
					}
				}
				if (!IsGrounded)
				{
					float force = characterMotor.velocity.y;
					force *= glideResistance;
					force = glideForce * force;
					if (force < minForce)
					{
						force = minForce;
					}
					else if (force > maxForce)
					{
						force = maxForce;
					}
					characterMotor.velocity += airGlide * force;
					if (boostDuration == maxboostDuration)
					{
						float force2 = characterMotor.velocity.y + jetInitVert;
						if (force2 > jetMaxUp)
						{
							force2 = Math.Max(jetMaxUp, characterMotor.velocity.y);
						}
						characterMotor.velocity = airGlide * force2;
					}
				}
			}
			if (boostDuration > 0f)
			{
				float baseforce = boostDuration/maxboostDuration;
				boostDuration -= Time.fixedDeltaTime;
				if (IsGrounded)
				{
					baseforce *= jetSlideHori;
				}
				else
				{
					baseforce *= jetSlideVert;
				}
				characterMotor.rootMotion += boostDirection * (baseforce * moveSpeedStat);
			}
			Vector3 velocity = characterMotor.velocity;
			velocity.y = 0f;
			if (velocity.magnitude > 1f)
			{
				characterBody.isSprinting = true;
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
			return InterruptPriority.Pain;
		}

		private void FireJets()
		{
			Transform ljet = FindModelChild("LeftJet");
			Transform rjet = FindModelChild("RightJet");
			if (ljet)
			{
				UnityEngine.Object.Instantiate<GameObject>(jetEffectPrefab, ljet);
			}
			if (rjet)
			{
				UnityEngine.Object.Instantiate<GameObject>(jetEffectPrefab, rjet);
			}
			if (inputBank.moveVector.magnitude > 0.1f)
            {
				boostDirection = inputBank.moveVector;
			}
			else
            {
				boostDirection = characterDirection.forward;
			}
			Util.PlaySound(boostSoundString, gameObject);
			HasBoosted = true;
			boostDuration = maxboostDuration;
		}

		private void ReloadSkills()
        {
			GenericSkill skill = skillLocator.GetSkill(SkillSlot.Primary);
			if (skill.skillDef != Modules.Skills.ShowTimeSkill)
			{
				skill.ApplyAmmoPack();
			}
			skill = skillLocator.GetSkill(SkillSlot.Secondary);
			if (skill.skillDef != Modules.Skills.ShowTimeSkill)
			{
				skill.ApplyAmmoPack();
			}
			skill = skillLocator.GetSkill(SkillSlot.Utility);
			if (skill.skillDef != Modules.Skills.ShowTimeSkill)
			{
				skill.ApplyAmmoPack();
			}
			skill = skillLocator.GetSkill(SkillSlot.Special);
			if (skill.skillDef != Modules.Skills.ShowTimeSkill)
			{
				skill.ApplyAmmoPack();
			}
		}

		private bool HasReloaded;
		private float duration;
		private float buffDuration = Main.ShowTime_ReadyDuration.Value;
		private float boostDuration;
		private float maxboostDuration;
		private bool HasBoosted;
		private Vector3 boostDirection;

		public static float baseDuration = 1.0f;
		public static string reloadSoundString = "Play_bandit_M2_load";
		public static string enterSoundString = "Play_bandit2_R_load";
		public static string boostSoundString = "Play_commando_shift";

		public static Vector3 airGlide = Vector3.up;
		public static float glideForce = 1f;
		public static float minForce = 0f;
		public static float maxForce = 3f;
		public static float glideResistance = -0.1f;

		public static GameObject jetEffectPrefab = EntityStates.Commando.SlideState.jetEffectPrefab;
		public static float jetMaxUp = 12f;

		public static float jetInitVert = 40f;

		public static float jetSlideHori = 0.05f;
		public static float jetSlideVert = 0.03f;

		public static float jetDuration = 0.4f;
	}
}
