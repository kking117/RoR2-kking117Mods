using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace FlatItemBuff.Items.Behaviors
{
    public class NewlyHatchedZoea_Rework : CharacterBody.ItemBehavior
    {
		private SkillLocator skillLocator;
		private InputBankTest inputBank;

		private int loadedMissiles = 0;
		private float nextMissile;
		private float nextStock;

		internal static float FireDelay = 0.25f;

		private void Awake()
		{
			base.enabled = false; //this is important
		}
		private void OnEnable()
		{
			if (body)
			{
				body.onSkillActivatedServer += OnSkillActivated;
				skillLocator = body.GetComponent<SkillLocator>();
				inputBank = body.GetComponent<InputBankTest>();
			}
		}
		private void OnDisable()
		{
			if (body)
			{
				body.onSkillActivatedServer -= OnSkillActivated;
				body.SetBuffCount(Items.NewlyHatchedZoea_Rework.VoidMissileStockBuff.buffIndex, 0);
			}
			inputBank = null;
			skillLocator = null;
		}
		private void OnSkillActivated(GenericSkill skill)
		{
			SkillLocator skillLocator = this.skillLocator;
			if (skillLocator != null && skillLocator.special != null)
			{
				if (skillLocator.special == skill)
				{
					if (body.GetBuffCount(Items.NewlyHatchedZoea_Rework.VoidMissileStockBuff) > 0)
					{
						loadedMissiles += body.GetBuffCount(Items.NewlyHatchedZoea_Rework.VoidMissileStockBuff);
						body.SetBuffCount(Items.NewlyHatchedZoea_Rework.VoidMissileStockBuff.buffIndex, 0);
					}
				}
            }
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
			if (loadedMissiles > 0)
			{
				nextMissile -= Time.fixedDeltaTime;
				if (nextMissile <= 0f)
				{
					loadedMissiles -= 1;
					FireMissile();
					if (loadedMissiles < 1)
					{
						nextMissile = 0f;
					}
					else
                    {
						nextMissile += FireDelay / body.attackSpeed;
					}
				}
			}
			if (body.GetBuffCount(Items.NewlyHatchedZoea_Rework.VoidMissileStockBuff) < Items.NewlyHatchedZoea_Rework.GetMaxStock(stack))
			{
				nextStock -= Time.fixedDeltaTime;
				if (nextStock <= 0f)
				{
					nextStock += Items.NewlyHatchedZoea_Rework.RestockTime / Items.NewlyHatchedZoea_Rework.GetMaxStock(stack);
					body.AddBuff(Items.NewlyHatchedZoea_Rework.VoidMissileStockBuff);
				}
			}
		}

		private void FireMissile()
        {
			float damage = body.damage * Items.NewlyHatchedZoea_Rework.GetMissileDamage(stack);
			MissileUtils.FireMissile(body.corePosition, body, new ProcChainMask(), null, damage, Util.CheckRoll(body.crit, body.master), Items.NewlyHatchedZoea_Rework.VoidMissileProjectile, DamageColorIndex.Void, true);
		}
	}
}
