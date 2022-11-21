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
				HurtBox victim;
				if (MainPlugin.KnurlRework_TargetType.Value == 0)
                {
					victim = GetFistTarget_Weak();
				}
				else
                {
					victim = GetFistTarget_Close();
				}
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
		private HurtBox GetFistTarget_Weak()
		{
			float damage = GetFistDamage();
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = body;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(body.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.minDistanceFilter = 0f;
			search.maxDistanceFilter = MainPlugin.KnurlRework_AttackRange.Value;
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
				if (IsValidTarget(target))
				{
					HealthComponent hpcomp = target.healthComponent;
					if (hpcomp.alive)
					{
						float actualHealth = GetEffectiveHealth(hpcomp);
						if (actualHealth < damage)
						{
							if(actualHealth > highesthealth)
                            {
								highesthealth = actualHealth;
								resultA = target;
							}
						}
						else
						{
							if (lowesthealth == -1f || actualHealth < lowesthealth)
							{
								resultB = target;
								lowesthealth = actualHealth;
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

		private HurtBox GetFistTarget_Close()
		{
			float damage = GetFistDamage();
			BullseyeSearch search = new BullseyeSearch();
			search.viewer = body;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(body.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.minDistanceFilter = 0f;
			search.maxDistanceFilter = MainPlugin.KnurlRework_AttackRange.Value;
			search.searchOrigin = body.inputBank.aimOrigin;
			search.searchDirection = body.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = false;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				if (IsValidTarget(target))
				{
					HealthComponent hpcomp = target.healthComponent;
					if (hpcomp.alive)
					{
						return target;
					}
				}
			}
			return null;
		}
		private float GetFistDamage()
        {
			float dmgcof = MainPlugin.KnurlRework_BaseDamage.Value + (MainPlugin.KnurlRework_StackDamage.Value * (stack - 1));
			return dmgcof * body.damage;
        }

		private float GetEffectiveHealth(HealthComponent hpcomp)
        {
			CharacterBody body = hpcomp.body;
			float effhp = hpcomp.combinedHealth;
			if (body)
            {
				if (body.armor > 0)
                {
					effhp = effhp / (100 / (100 + body.armor));
				}
				else
                {
					effhp = effhp * (100 / (100 - body.armor));
				}
			}
			return effhp;
        }

		private bool IsValidTarget(HurtBox target)
        {
			HealthComponent hpcomp = target.healthComponent;
			if (hpcomp && hpcomp.body)
			{
				if (hpcomp.body.characterMotor)
				{
					if (hpcomp.body.characterMotor.isGrounded)
					{
						return true;
					}
				}
				RaycastHit raycastHit;
				Physics.Raycast(target.transform.position, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
				{
					"World"
				}));
				float distance = Vector3.Distance(raycastHit.point, hpcomp.body.footPosition);
				if (distance <= 6f)
				{
					return true;
				}
			}
			return false;
		}
	}
}
