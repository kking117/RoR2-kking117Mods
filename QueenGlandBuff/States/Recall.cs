using System;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace QueenGlandBuff.States
{
	public class Recall: BaseSkillState
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
					hasMadeSound = true;
					Util.PlayAttackSpeedSound(initialSoundString, gameObject, attackSpeedStat * (soundTime / duration) * soundOffset);
				}
				if (modelAnimator.GetFloat("DefenseUp.activate") > 0.5f && !hasTriedTele)
				{
					hasTriedTele = true;
					if (NetworkServer.active)
					{
						int teleresult = Utils.Helpers.TeleportToOwner(characterBody);
						if (teleresult == 0)
						{
							characterBody.AddTimedBuff(Changes.BeetleGuardAlly.BeetleFrenzy, buffDuration);
						}
						else if (teleresult == 1)
						{
							characterBody.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, buffDuration / 2f);
						}
						else
						{
							PlayCrossfade("Body", "Spawn1", "Spawn1.playbackRate", duration - fixedAge, 0.2f);
							Changes.QueensGland.UpdateAILeash(characterBody.master);
						}
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

		public static string initialSoundString = EntityStates.BeetleGuardMonster.SpawnState.spawnSoundString;
		private Animator modelAnimator;

		private float duration;
		private bool hasTriedTele;
		private bool hasMadeSound;
	}
}
