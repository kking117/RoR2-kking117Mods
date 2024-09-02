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
		private HealthComponent hpComp;
		public CharacterBody attackerBody;
		private float Ypeak = 0f;

		public int itemCount = 0;
		private void Awake()
		{
			victimBody = GetComponent<CharacterBody>();
			if (victimBody)
            {
				hpComp = victimBody.GetComponent<HealthComponent>();
				Ypeak = victimBody.transform.position.y;
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
			float yPos = victimBody.transform.position.y;
			if (yPos > Ypeak)
            {
				Ypeak = yPos;
			}
			CharacterMotor victimMotor = victimBody.GetComponent<CharacterMotor>();
			if (victimMotor.isGrounded)
            {
				Detonate();
			}
		}
		private void Detonate()
        {
			victimBody.RemoveBuff(DLC2Content.Buffs.KnockUpHitEnemies);
			victimBody.AddTimedBuff(DLC2Content.Buffs.KnockUpHitEnemies, KnockbackFin.Cooldown);
			if (attackerBody)
            {
				float blastDamage = attackerBody.damage * KnockbackFin.GetImpactDamage(itemCount);
				float blastRadius = KnockbackFin.GetImpactRadius(itemCount);
				if (blastDamage > 0f)
				{
					float distDmg = Mathf.Max(0f, Ypeak - victimBody.footPosition.y);
					distDmg = Mathf.InverseLerp(0f, 120f, distDmg);
					//blastRadius *= Mathf.Lerp(0.5f, 5f, distDmg);
					blastDamage *= Mathf.Lerp(1f, KnockbackFin.MaxDistDamage, distDmg);

					bool isCrit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
					Vector3 blastPosition = victimBody.footPosition;
					GameObject blastVFX = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), blastPosition, Quaternion.identity);
					blastVFX.transform.localScale = new Vector3(blastRadius, blastRadius, blastRadius);
					DelayBlast delayBlast = blastVFX.GetComponent<DelayBlast>();
					if (delayBlast)
					{
						delayBlast.position = blastPosition;
						delayBlast.radius = blastRadius;
						delayBlast.baseDamage = blastDamage;
						delayBlast.baseForce = 0f;
						delayBlast.procCoefficient = KnockbackFin.ProcRate;
						delayBlast.crit = isCrit;
						delayBlast.falloffModel = BlastAttack.FalloffModel.SweetSpot;
						delayBlast.maxTimer = 0f;
						delayBlast.attacker = attackerBody.gameObject;
						delayBlast.inflictor = null;
						delayBlast.teamFilter.teamIndex = attackerBody.teamComponent.teamIndex;
						delayBlast.explosionEffect = KnockbackFin.ImpactEffect;
						delayBlast.damageType = DamageType.Generic;
						delayBlast.damageColorIndex = DamageColorIndex.Item;
						TeamFilter teamFilter = blastVFX.GetComponent<TeamFilter>();
						if (teamFilter)
						{
							teamFilter.teamIndex = attackerBody.teamComponent.teamIndex;
						}
						if (KnockbackFin.DoStun)
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
						EffectManager.SimpleSoundEffect(EntityStates.Croco.BaseLeap.landingSound.index, victimBody.footPosition, true);
						NetworkServer.Spawn(blastVFX);
					}
				}
			}
			Destroy(this);
		}
	}
}
