using System;
using RoR2;
using RoR2.Orbs;
using R2API;
using UnityEngine;

namespace FlatItemBuff.Orbs
{
	public class ScorpionOrb : GenericDamageOrb
	{
        public override void Begin()
        {
			this.speed = 30f;
			base.Begin();
		}
		public override GameObject GetOrbEffect()
        {
			return Items.SymbioticScorpion_Rework.OrbVFX;
		}
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = null;
					damageInfo.force = Vector3.zero;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					damageInfo.damageType = this.damageType;
					damageInfo.AddModdedDamageType(Items.SymbioticScorpion_Rework.ScorpionVenomOnHit);
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
			}
		}
	}
}
