﻿using System.Collections.Generic;
using RoR2;
using UnityEngine;
using FlatItemBuff.Components;

namespace FlatItemBuff.Items.Behaviors
{
    public class Planula_Rework : CharacterBody.ItemBehavior
    {
		SphereSearch AOESphere = new SphereSearch();
		List<HurtBox> SphereResult = new List<HurtBox>();

		private float nextAttack;
		private float attackRadius = Items.Planula_Rework.Radius;
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
			if (!body.outOfCombat)
			{
				nextAttack -= Time.fixedDeltaTime;
				if (nextAttack <= 0f)
				{
					nextAttack += 0.5f;
					BurnAOE();
				}
			}
        }
        private void BurnAOE()
        {
			AOESphere.origin = body.corePosition;
			AOESphere.mask = LayerIndex.entityPrecise.mask;
			AOESphere.radius = attackRadius;
			AOESphere.RefreshCandidates();
			AOESphere.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(body.teamComponent.teamIndex));
			AOESphere.FilterCandidatesByDistinctHurtBoxEntities();

			AOESphere.GetHurtBoxes(SphereResult);

			float baseDamage = body.damage * Items.Planula_Rework.GetDuration();
			float damageMult = 1f + Items.Planula_Rework.GetTotalDamage(stack);

			bool didHit = false;

			foreach (HurtBox target in SphereResult)
			{
				if (IsValidTarget(target))
				{
					didHit = true;
					InflictDotInfo inflictDotInfo = new InflictDotInfo
					{
						victimObject = target.healthComponent.gameObject,
						attackerObject = body.gameObject,
						totalDamage = baseDamage * damageMult,
						dotIndex = DotController.DotIndex.Burn,
						damageMultiplier = damageMult,
					};
					if (body.inventory)
					{
						StrengthenBurnUtils.CheckDotForUpgrade(body.inventory, ref inflictDotInfo);
					}
					DotController.InflictDot(ref inflictDotInfo);
				}
			}
			SphereResult.Clear();
			if (didHit)
            {
				EffectManager.SpawnEffect(Items.Planula_Rework.BurnEffect, new EffectData
				{
					origin = body.corePosition,
					scale = attackRadius,
					rotation = body.transform.rotation
				}, true);
			}
		}
		private bool IsValidTarget(HurtBox target)
        {
			HealthComponent hpcomp = target.healthComponent;
			if (hpcomp && hpcomp.body)
			{
				return true;
			}
			return false;
		}
	}
}
