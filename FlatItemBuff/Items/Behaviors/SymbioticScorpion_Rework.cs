using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using RoR2.Orbs;
using UnityEngine;
using RoR2.Projectile;

namespace FlatItemBuff.Items.Behaviors
{
    public class SymbioticScorpion_Rework : CharacterBody.ItemBehavior
    {
		private float nextAttack;
		private void Awake()
        {
			nextAttack = Items.SymbioticScorpion_Rework.Cooldown;
		}
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
			if (nextAttack <= 0f)
			{
				if (TryEnvenom())
				{
					nextAttack += GetCooldown();
				}
				else
				{
					nextAttack += 0.5f;
				}
			}
		}
		private float GetCooldown()
        {
			return Items.SymbioticScorpion_Rework.Cooldown;
		}
		private bool TryEnvenom()
		{
			if (body.teamComponent.teamIndex == TeamIndex.Player)
            {
				return Player_EnvenomAttack();
			}
			return Monster_EnvenomAttack();
		}
		private bool Player_EnvenomAttack()
        {
			bool didHit = false;
			bool isCrit = body.RollCrit();
			float damage = body.damage;
			TeamIndex teamIndex = body.teamComponent.teamIndex;
			HurtBox[] hurtBoxes = new SphereSearch
			{
				origin = body.aimOrigin,
				radius = Items.SymbioticScorpion_Rework.Radius,
				mask = LayerIndex.entityPrecise.mask,
				queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
			}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
			for (int i = 0; i < hurtBoxes.Length; i++)
			{
				CharacterBody targetBody = hurtBoxes[i].healthComponent.body;
				if (targetBody)
				{
					if (targetBody.master)
					{
						Orbs.ScorpionOrb orbAttack = new Orbs.ScorpionOrb();
						orbAttack.origin = body.aimOrigin;
						orbAttack.attacker = body.gameObject;
						orbAttack.damageValue = damage;
						orbAttack.damageColorIndex = DamageColorIndex.Item;
						orbAttack.isCrit = isCrit;
						orbAttack.procCoefficient = 1f;
						orbAttack.teamIndex = teamIndex;
						orbAttack.target = hurtBoxes[i];
						OrbManager.instance.AddOrb(orbAttack);
						didHit = true;
						break;
					}
				}
			}
			return didHit;
		}
		private bool Monster_EnvenomAttack()
		{
			bool didHit = false;
			bool isCrit = body.RollCrit();
			float damage = body.damage * 0.25f;
			if (HasAValidTargetInView())
            {
				FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
				{
					projectilePrefab = Items.SymbioticScorpion_Rework.MonsterVenomProjectile,
					position = body.aimOrigin,
					rotation = Util.QuaternionSafeLookRotation(body.inputBank.aimDirection),
					owner = body.gameObject,
					damage = damage,
					crit = isCrit,
					force = 0f,
					speedOverride = 30f,
					useSpeedOverride = true
				};
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
				didHit = true;
			}
			return didHit;
		}
		private bool HasAValidTargetInView()
		{
			TeamIndex teamIndex = body.teamComponent.teamIndex;
			float viewRange = body.radius + Items.SymbioticScorpion_Rework.Radius;

			BullseyeSearch search = new BullseyeSearch();
			search.viewer = body;
			search.teamMaskFilter = TeamMask.GetEnemyTeams(teamIndex);
			search.teamMaskFilter.RemoveTeam(body.master.teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.minDistanceFilter = 0f;
			search.maxDistanceFilter = viewRange;
			search.searchOrigin = body.inputBank.aimOrigin;
			search.searchDirection = body.inputBank.aimDirection;
			search.maxAngleFilter = 90f;
			search.filterByLoS = true;
			search.filterByDistinctEntity = true;
			search.RefreshCandidates();
			HurtBox[] hurtBoxes = search.GetResults().ToArray();

			for (int i = 0; i < hurtBoxes.Length; i++)
			{
				CharacterBody targetBody = hurtBoxes[i].healthComponent.body;
				if (targetBody)
				{
					if (targetBody.master)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
