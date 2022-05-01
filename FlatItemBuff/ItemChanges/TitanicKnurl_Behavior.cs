using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using RoR2.Projectile;

namespace FlatItemBuff.ItemChanges
{
    public class TitanicKnurl_Behavior : CharacterBody.ItemBehavior
    {
		private float nextAttack;
		private void FixedUpdate()
        {
			if (!body)
			{
				return;
			}
			if (!body.master)
			{
				return;
			}
			nextAttack -= Time.fixedDeltaTime;
			if (body.healthComponent)
            {
				if (body.healthComponent.isHealthLow)
				{
					nextAttack -= Time.fixedDeltaTime * 0.5f;
				}
			}
			if (nextAttack <= 0f)
            {
				HurtBox victim = GetFistTarget();
				if(victim && victim.healthComponent && victim.healthComponent.body)
                {
					FireFist(victim.healthComponent.body.gameObject);
					float firerate = 1f + (MainPlugin.KnurlRework_StackSpeed.Value * (stack -1));
					nextAttack += Math.Max(0.5f, MainPlugin.KnurlRework_BaseSpeed.Value / firerate);
				}
				else
                {
					nextAttack += 0.5f;
                }
			}
        }
        private void FireFist(GameObject target)
        {
			float damage = GetFistDamage();
			RaycastHit raycastHit;
			Physics.Raycast(target.transform.position, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
			{
				"World"
			}));
			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = TitanicKnurl_Rework.StoneFistProjectile,
				position = raycastHit.point,
				rotation = Quaternion.identity,
				target = target,
				owner = body.gameObject,
				damage = damage,
				crit = body.RollCrit(),
				force = 0f,
				damageColorIndex = DamageColorIndex.Item,
				fuseOverride = body.teamComponent.teamIndex == TeamIndex.Player ? 0.5f : -1f
			};
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}
		private bool CanPunchAirborne(HurtBox target)
        {
			if (target.healthComponent && target.healthComponent.body)
			{
				RaycastHit raycastHit;
				Physics.Raycast(target.transform.position, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
				{
				"World"
				}));
				float distance = Vector3.Distance(raycastHit.point, target.healthComponent.body.footPosition);
				if (distance <= 7f)
				{
					return true;
				}
			}
			return false;
		}
		private HurtBox GetFistTarget()
		{
			float damage = GetFistDamage();
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = body;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(body.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.minDistanceFilter = 0f;
			search.maxDistanceFilter = 50f;
			search.searchOrigin = body.inputBank.aimOrigin;
			search.searchDirection = body.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = false;
			search.RefreshCandidates();
			HurtBox resultA = null;
			HurtBox resultB = null;
			float highesthealth = -1f;
			float lowesthealth = -1f;
			foreach (HurtBox target in search.GetResults())
			{
				if (IsGrounded(target) || CanPunchAirborne(target))
				{
					HealthComponent hpcomp = target.healthComponent;
					if (hpcomp.alive)
					{
						if (hpcomp.combinedHealth * 0.8f <= damage)
						{
							if(hpcomp.combinedHealth > highesthealth)
                            {
								highesthealth = hpcomp.combinedHealth;
								resultA = target;
							}
						}
						else
						{
							if (lowesthealth == -1f || hpcomp.combinedHealth < lowesthealth)
							{
								resultB = target;
								lowesthealth = hpcomp.combinedHealth;
							}
						}
					}
				}
			}
			if(resultA == null)
            {
				return resultB;
            }
			return resultA;
		}
		private float GetFistDamage()
        {
			float dmgcof = MainPlugin.KnurlRework_BaseDamage.Value + (MainPlugin.KnurlRework_StackDamage.Value * (stack - 1));
			return dmgcof * body.damage;
        }
		private bool IsGrounded(HurtBox target)
        {
			if (target.healthComponent && target.healthComponent.body)
            {
				if (target.healthComponent.body.characterMotor)
                {
					if (target.healthComponent.body.characterMotor.isGrounded)
                    {
						return true;
                    }
				}
			}
			return false;
		}
	}
}
