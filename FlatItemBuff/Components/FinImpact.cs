using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using FlatItemBuff.Items;

namespace FlatItemBuff.Components
{
	public class FinImpact: MonoBehaviour
	{
		private CharacterBody victimBody;
		private CharacterMotor victimMotor;
		private HealthComponent hpComp;
		public CharacterBody attackerBody;
		public int itemCount = 0;

		public float enforceDuration = 1f;
		private float lastSpeed = 0f;
		private float lastSpeedA = 0f;
		private float lastSpeedB = 0f;
		private float fallCreditGrace = 1f;
		private bool HasImpact = false;
		private void Awake()
		{
			Reset();
		}
		internal void Reset()
		{
			lastSpeed = 0f;
			lastSpeedA = 0f;
			lastSpeedB = 0f;
			fallCreditGrace = 1f;
			HasImpact = false;

			enforceDuration = 1f;

			victimBody = GetComponent<CharacterBody>();
			if (victimBody)
			{
				hpComp = victimBody.GetComponent<HealthComponent>();
				victimMotor = victimBody.GetComponent<CharacterMotor>();
				if (victimMotor)
                {
					lastSpeed = victimMotor.velocity.magnitude;
				}
			}
		}
		private void OnDestroy()
		{
			if (victimBody)
			{
				StartCooldown();
			}
		}
		private void FixedUpdate()
		{
			if (!victimBody || !hpComp)
			{
				Destroy(this);
				return;
			}
			if (!hpComp.alive)
            {
				Destroy(this);
				return;
			}
			if (victimBody.HasBuff(BreachingFin_Rework.KBDebuff))
			{
				fallCreditGrace = 1f;
				if (!victimMotor.isGrounded)
				{
					if (enforceDuration < 0f)
					{
						if (victimMotor.isFlying)
						{
							float resistance = Mathf.Max(victimBody.moveSpeed, victimBody.baseMoveSpeed);
							float magnitude = victimMotor.velocity.magnitude;
							if (magnitude < resistance)
							{
								StartCooldown();
							}
						}
					}
					else
					{
						enforceDuration -= Time.fixedDeltaTime;
					}
					lastSpeedB = lastSpeedA;
					lastSpeedA = lastSpeed;
					lastSpeed = victimMotor.velocity.y;
					if (lastSpeed < 0f)
                    {
						lastSpeed *= -1f;
					}
				}
			}
			else
            {
				fallCreditGrace -= Time.fixedDeltaTime;
				if (fallCreditGrace <= 0f)
                {
					Destroy(this);
				}
			}
		}

		internal void OnImpact()
        {
			if (!HasImpact)
            {
				StartCooldown();
				Detonate();
			}
		}
		private void StartCooldown()
		{
			victimBody.RemoveBuff(BreachingFin_Rework.KBDebuff);
			if (!victimBody.HasBuff(BreachingFin_Rework.KBCooldown))
			{
				for (int i = BreachingFin_Rework.Cooldown; i > 0; i--)
				{
					victimBody.AddTimedBuff(BreachingFin_Rework.KBCooldown, i);
				}
			}
		}
		private float GetBestSpeed()
        {
			float returnVal = Math.Max(lastSpeed, lastSpeedA);
			return Math.Max(returnVal, lastSpeedB);
		}
		private void Detonate()
        {
			float bestSpeed = GetBestSpeed();
			HasImpact = true;
			if (BreachingFin_Rework.BaseRadius > 0f && attackerBody)
            {
				float blastDamage = attackerBody.damage * BreachingFin_Rework.GetImpactDamage(itemCount);
				float blastRadius = BreachingFin_Rework.GetImpactRadius(itemCount);
				if (blastDamage > 0f)
				{
					float resistance = (victimBody.jumpPower + 20f) / 4f;
					bool isCrit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
					float velDmg = Mathf.Max(0f, bestSpeed);
					velDmg = Mathf.InverseLerp(resistance, 80f, velDmg);
					blastDamage *= Mathf.Lerp(1f, BreachingFin_Rework.MaxDistDamage, velDmg);
					

					Vector3 blastPosition = victimBody.footPosition;
					BlastAttack blastAttack = new BlastAttack();
					if (BreachingFin_Rework.DoStun)
					{
						SetStateOnHurt comp = victimBody.GetComponent<SetStateOnHurt>();
						if (comp)
						{
							if (comp.canBeStunned)
							{
								comp.SetStun(1f);
							}
						}
					}
					blastAttack.position = blastPosition;
					blastAttack.radius = blastRadius;
					blastAttack.baseDamage = blastDamage;
					blastAttack.baseForce = 0f;
					blastAttack.procCoefficient = BreachingFin_Rework.ProcRate;
					blastAttack.crit = isCrit;
					blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
					blastAttack.attacker = attackerBody.gameObject;
					blastAttack.inflictor = null;
					blastAttack.teamIndex = TeamComponent.GetObjectTeam(attackerBody.gameObject);
					blastAttack.damageType = DamageType.Generic;
					blastAttack.damageColorIndex = DamageColorIndex.Item;
					blastAttack.Fire();
					EffectManager.SpawnEffect(BreachingFin_Rework.ImpactEffect, new EffectData
					{
						origin = blastPosition,
						rotation = Util.QuaternionSafeLookRotation(victimBody.transform.forward),
						scale = blastRadius
					}, true);
					EffectManager.SimpleSoundEffect(EntityStates.Croco.BaseLeap.landingSound.index, victimBody.footPosition, true);
				}
			}
		}
	}
}
