using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using RoR2.CharacterAI;

namespace FlatItemBuff.Items.Behaviors
{
	public class GhostCloneIdentifier : CharacterBody.ItemBehavior
	{
		//forces a new target onto the ai every 3 seconds if a target is within close range
		private float updateTimer = 1f;
		private const float updateDelayTime = 3f;
		private const float AggroRadius = 12f;
		private void FixedUpdate()
		{
			if (!body)
			{
				return;
			}
			if(!body.master)
            {
				return;
            }
			updateTimer -= Time.fixedDeltaTime;
			if (updateTimer <= 0f)
			{
				updateTimer += updateDelayTime;
				ForceTarget();
			}
		}
		private void ForceTarget()
		{
			BaseAI baseAI = body.master.gameObject.GetComponent<BaseAI>();
			if (baseAI)
            {
				HurtBox hurtBox = baseAI.FindEnemyHurtBox(body.radius + AggroRadius, true, true);
				if (hurtBox && hurtBox.healthComponent)
				{
					baseAI.currentEnemy.gameObject = hurtBox.healthComponent.gameObject;
					baseAI.currentEnemy.bestHurtBox = hurtBox;
				}
			}
		}
	}
}
