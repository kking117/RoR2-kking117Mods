using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace TestTest.States
{
	public class Infest : BaseState
	{
		public static GameObject enterEffectPrefab = EntityStates.VoidInfestor.Infest.enterEffectPrefab;
		public static GameObject successfulInfestEffectPrefab = EntityStates.VoidInfestor.Infest.successfulInfestEffectPrefab;
		public static GameObject infestVfxPrefab = EntityStates.VoidInfestor.Infest.infestVfxPrefab;

		public static string enterSoundString = EntityStates.VoidInfestor.Infest.enterSoundString;

		public static float searchMaxDistance = EntityStates.VoidInfestor.Infest.searchMaxAngle;
		public static float searchMaxAngle = EntityStates.VoidInfestor.Infest.searchMaxAngle;
		public static float velocityInitialSpeed = EntityStates.VoidInfestor.Infest.velocityInitialSpeed;
		public static float velocityTurnRate = EntityStates.VoidInfestor.Infest.velocityTurnRate;

		public static float infestDamageCoefficient = EntityStates.VoidInfestor.Infest.infestDamageCoefficient;

		private Transform targetTransform;
		private GameObject infestVfxInstance;
		private OverlapAttack attack;
		private List<HurtBox> victimsStruck = new List<HurtBox>();
		public override void OnEnter()
		{
			base.OnEnter();
			this.PlayAnimation("Base", "Infest");
			Util.PlaySound(Infest.enterSoundString, base.gameObject);
			if (Infest.enterEffectPrefab)
			{
				EffectManager.SimpleImpactEffect(Infest.enterEffectPrefab, base.characterBody.corePosition, Vector3.up, false);
			}
			if (Infest.infestVfxPrefab)
			{
				this.infestVfxInstance = UnityEngine.Object.Instantiate<GameObject>(Infest.infestVfxPrefab, base.transform.position, Quaternion.identity);
				this.infestVfxInstance.transform.parent = base.transform;
			}
			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Infest");
			}
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = characterBody.teamComponent.teamIndex;
			this.attack.damage = Infest.infestDamageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = null;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.damageType = DamageType.Stun1s;
			this.attack.damageColorIndex = DamageColorIndex.Void;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.viewer = base.characterBody;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(base.characterBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.minDistanceFilter = 0f;
			bullseyeSearch.maxDistanceFilter = Infest.searchMaxDistance;
			bullseyeSearch.searchOrigin = base.inputBank.aimOrigin;
			bullseyeSearch.searchDirection = base.inputBank.aimDirection;
			bullseyeSearch.maxAngleFilter = Infest.searchMaxAngle;
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.RefreshCandidates();
			HurtBox hurtBox = null;
			foreach (HurtBox target in bullseyeSearch.GetResults())
            {
				if (target && target.healthComponent)
				{
					if (target.healthComponent.body && target.healthComponent.body.master)
					{
						if (Utils.Helpers.CanInfest(characterBody, target.healthComponent.body.master))
						{
							hurtBox = target;
							break;
						}
					}
				}
            }
			if (hurtBox)
			{
				this.targetTransform = hurtBox.transform;
				if (base.characterMotor)
				{
					Vector3 position = this.targetTransform.position;
					float num = Infest.velocityInitialSpeed;
					Vector3 vector = position - base.transform.position;
					Vector2 vector2 = new Vector2(vector.x, vector.z);
					float magnitude = vector2.magnitude;
					float y = Trajectory.CalculateInitialYSpeed(magnitude / num, vector.y);
					Vector3 vector3 = new Vector3(vector2.x / magnitude * num, y, vector2.y / magnitude * num);
					base.characterMotor.velocity = vector3;
					base.characterMotor.disableAirControlUntilCollision = true;
					base.characterMotor.Motor.ForceUnground();
					base.characterDirection.forward = vector3;
				}
			}
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.targetTransform && base.characterMotor)
			{
				Vector3 target = this.targetTransform.position - base.transform.position;
				Vector3 vector = base.characterMotor.velocity;
				vector = Vector3.RotateTowards(vector, target, Infest.velocityTurnRate * Time.fixedDeltaTime * 0.017453292f, 0f);
				base.characterMotor.velocity = vector;
				if (NetworkServer.active && this.attack.Fire(this.victimsStruck))
				{
					int i = 0;
					while (i < this.victimsStruck.Count)
					{
						HealthComponent healthComponent = this.victimsStruck[i].healthComponent;
						CharacterBody body = healthComponent.body;
						CharacterMaster master = body.master;
						if (TryInfest(master))
						{
							break;
						}
						i++;
					}
				}
			}
			if (base.characterDirection)
			{
				base.characterDirection.moveVector = base.characterMotor.velocity;
			}
			if (base.isAuthority && base.characterMotor && base.characterMotor.isGrounded && base.fixedAge > 0.1f)
			{
				this.outer.SetNextStateToMain();
			}
		}
		private bool TryInfest(CharacterMaster target)
        {
			if(!target)
            {
				return false;
            }
			if (Utils.Helpers.CanInfest(characterBody, target))
			{
				target.teamIndex = teamComponent.teamIndex;
				target.GetBody().teamComponent.teamIndex = teamComponent.teamIndex;
				target.inventory.SetEquipmentIndex(DLC1Content.Elites.Void.eliteEquipmentDef.equipmentIndex);
				BaseAI component = target.GetComponent<BaseAI>();
				if (component)
				{
					component.currentEnemy.Reset();
					component.targetRefreshTimer = 0f;
				}
				base.healthComponent.Suicide(null, null, DamageType.Generic);
				if (Infest.successfulInfestEffectPrefab)
				{
					EffectManager.SimpleImpactEffect(Infest.successfulInfestEffectPrefab, base.transform.position, Vector3.up, false);
				}
				return true;
			}
			return false;
        }
		public override void OnExit()
		{
			if (this.infestVfxInstance)
			{
				EntityState.Destroy(this.infestVfxInstance);
			}
			base.OnExit();
		}
	}
}
