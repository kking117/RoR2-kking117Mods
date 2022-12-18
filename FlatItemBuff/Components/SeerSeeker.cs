using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using RoR2.Orbs;

namespace FlatItemBuff.Components
{
	public class SeerSeeker : Orb
	{
		private GameObject effectPrefab;
		private Vector3 endPosSafe;

		public float damageValue;
		public GameObject attacker;
		public GameObject inflictor;
		public float strikeTime;
		public TeamIndex teamIndex;
		public bool isCrit;
		public ProcChainMask procChainMask;
		public float procCoefficient = 1f;
		public DamageColorIndex damageColorIndex;
		public DamageType damageType;
		public float SearchDistance = 25f;
		public List<HealthComponent> hitList;
		public bool isFirst = false;

		public override void Begin()
		{
			base.duration = strikeTime;
			if (hitList == null)
			{
				hitList = new List<HealthComponent>();
				HealthComponent hpcomp = target.healthComponent;
				if (hpcomp)
				{
					hitList.Add(hpcomp);
				}
			}
			endPosSafe = target.transform.position;
			effectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/VoidLightningOrbEffect");
			if (isFirst)
			{ 
				Redirect();
			}
		}
		public override void OnArrival()
		{
			Strike();
		}
		public void FixedUpdate()
		{
		}
		private void Strike()
		{
			if (target)
			{
				HealthComponent healthComponent = target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = damageValue;
					damageInfo.attacker = attacker;
					damageInfo.inflictor = inflictor;
					damageInfo.force = Vector3.zero;
					damageInfo.crit = isCrit;
					damageInfo.procChainMask = procChainMask;
					damageInfo.procCoefficient = procCoefficient;
					damageInfo.position = target.transform.position;
					damageInfo.damageColorIndex = damageColorIndex;
					damageInfo.damageType = damageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
					if (target.hurtBoxGroup)
					{
						target = target.hurtBoxGroup.hurtBoxes[UnityEngine.Random.Range(0, target.hurtBoxGroup.hurtBoxes.Length)];
					}
					EffectData effectData = new EffectData
					{
						origin = origin,
						genericFloat = 0.1f
					};
					endPosSafe = target.transform.position;
					effectData.SetHurtBoxReference(target);
					EffectManager.SpawnEffect(effectPrefab, effectData, true);
				}
			}
			if (SearchDistance > 0f && !isFirst)
			{
				Redirect();
			}
		}
		private void Redirect()
		{
			BullseyeSearch search = new BullseyeSearch();
			search.searchOrigin = endPosSafe;
			search.searchDirection = Vector3.zero;
			search.filterByLoS = false;
			search.teamMaskFilter = TeamMask.GetEnemyTeams(teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.maxDistanceFilter = SearchDistance;
			search.minThetaDot = -3.2f;
			search.maxThetaDot = 3.2f;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				HealthComponent hpcomp = target.healthComponent;
				if (hpcomp)
				{
					if (!hitList.Contains(hpcomp))
					{
						CharacterBody targetBody = hpcomp.body;
						if (targetBody)
						{
							hitList.Add(hpcomp);
							SeerSeeker seerker = new SeerSeeker();
							seerker.origin = endPosSafe;
							seerker.damageValue = damageValue;
							seerker.isCrit = isCrit;
							seerker.teamIndex = teamIndex;
							seerker.attacker = attacker;
							seerker.procChainMask = procChainMask;
							seerker.procCoefficient = procCoefficient;
							seerker.damageColorIndex = DamageColorIndex.Void;
							seerker.strikeTime = strikeTime;
							seerker.hitList = hitList;
							seerker.SearchDistance = SearchDistance;
							HurtBox mainHurtBox = targetBody.mainHurtBox;
							if (mainHurtBox)
							{
								seerker.target = mainHurtBox;
								OrbManager.instance.AddOrb(seerker);
							}
							break;
						}
					}
				}
			}
		}
	}
}
