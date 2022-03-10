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
					float firerate = 1f + (MainPlugin.Knurl_StackSpeed.Value * (stack -1));
					nextAttack += Math.Max(0.5f, MainPlugin.Knurl_BaseSpeed.Value / firerate);
				}
				else
                {
					nextAttack += 0.5f;
                }
			}
        }
        private void FireFist(GameObject target)
        {
			float dmgcof = MainPlugin.Knurl_BaseDamage.Value + (MainPlugin.Knurl_StackDamage.Value * (stack - 1));
			float damage = body.damage * dmgcof;
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
				if (distance <= 8f)
				{
					return true;
				}
			}
			return false;
		}
		private HurtBox GetFistTarget()
		{
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
			HurtBox result = null;
			float Health = float.PositiveInfinity;
			foreach (HurtBox target in search.GetResults())
			{
				if (IsGrounded(target) || CanPunchAirborne(target))
				{
					if (target.healthComponent.alive)
					{
						if (target.healthComponent.combinedHealth < Health)
						{
							result = target;
							Health = target.healthComponent.combinedHealth;
						}
					}
				}
			}
			return result;
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
