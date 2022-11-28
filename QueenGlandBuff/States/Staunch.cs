using System;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace QueenGlandBuff.States
{
	public class Staunch : BaseSkillState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			modelAnimator = GetModelAnimator();
			if (modelAnimator)
			{
				PlayAnimation("Body", "DefenseUp", "DefenseUp.playbackRate", duration);
			}
			if (NetworkServer.active)
			{
				characterBody.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, duration);
			}
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (modelAnimator)
			{
				if (modelAnimator.GetFloat("DefenseUp.activate") > 0.0f && !hasMadeSound)
				{
					Util.PlayAttackSpeedSound(initialSoundString, gameObject, attackSpeedStat * (soundTime / duration) * soundOffset);
					hasMadeSound = true;
				}
				if (modelAnimator.GetFloat("DefenseUp.activate") > 0.5f && !hasCastBuff)
				{
					ScaleParticleSystemDuration component = UnityEngine.Object.Instantiate<GameObject>(defenseUpPrefab, transform.position, Quaternion.identity, transform).GetComponent<ScaleParticleSystemDuration>();
					if (component)
					{
						component.newDuration = buffDuration;
					}
					hasCastBuff = true;
					if (NetworkServer.active)
					{
						characterBody.AddTimedBuff(Changes.StaunchBuff.Staunching, buffDuration);
					}
				}
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

		public static float baseDuration = 3.0f;
		public static float soundTime = 3.5f;
		public static float soundOffset = 1.25f;
		public static float buffDuration = 10f;

		public static GameObject defenseUpPrefab = EntityStates.BeetleGuardMonster.DefenseUp.defenseUpPrefab;
		public static string initialSoundString = EntityStates.BeetleGuardMonster.SpawnState.spawnSoundString;
		private Animator modelAnimator;

		private float duration;
		private bool hasCastBuff;
		private bool hasMadeSound;
	}
}
