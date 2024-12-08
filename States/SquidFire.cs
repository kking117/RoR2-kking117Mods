using System.Linq;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.States
{
	public class SquidFire : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			GetAimRay();
			PlayAnimation("Gesture", "FireGoo");
			if (isAuthority)
			{
				FireOrbArrow();
			}
		}
		private void FireOrbArrow()
		{
			if (this.hasFiredArrow || !NetworkServer.active)
			{
				return;
			}
			Ray aimRay = GetAimRay();
			enemyFinder = new BullseyeSearch();
			enemyFinder.viewer = characterBody;
			enemyFinder.maxDistanceFilter = float.PositiveInfinity;
			enemyFinder.searchOrigin = aimRay.origin;
			enemyFinder.searchDirection = aimRay.direction;
			enemyFinder.sortMode = BullseyeSearch.SortMode.Distance;
			enemyFinder.teamMaskFilter = TeamMask.allButNeutral;
			enemyFinder.minDistanceFilter = 0f;
			enemyFinder.maxAngleFilter = (fullVision ? 180f : 90f);
			enemyFinder.filterByLoS = true;
			if (teamComponent)
			{
				enemyFinder.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
			}
			enemyFinder.RefreshCandidates();
			HurtBox hurtBox = Enumerable.FirstOrDefault<HurtBox>(enemyFinder.GetResults());
			if (hurtBox)
			{
				Vector3 vector = hurtBox.transform.position - GetAimRay().origin;
				aimRay.origin = GetAimRay().origin;
				aimRay.direction = vector;
				inputBank.aimDirection = vector;
				StartAimMode(aimRay, 2f, false);
				hasFiredArrow = true;
				RoR2.Orbs.SquidOrb squidOrb = new RoR2.Orbs.SquidOrb();
				squidOrb.damageValue = characterBody.damage * damageCoefficient;
				squidOrb.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
				squidOrb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
				squidOrb.attacker = gameObject;
				squidOrb.procCoefficient = procCoefficient;
				squidOrb.damageType = DamageType.ClayGoo;
				HurtBox hurtBox2 = hurtBox;
				if (hurtBox2)
				{
					Transform transform = characterBody.modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Muzzle");
					EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "Muzzle", true);
					squidOrb.origin = transform.position;
					squidOrb.target = hurtBox2;
					RoR2.Orbs.OrbManager.instance.AddOrb(squidOrb);
				}
			}
		}
		public override void OnExit()
		{
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		public static GameObject hitEffectPrefab = EntityStates.Squid.SquidWeapon.FireSpine.hitEffectPrefab;
		public static GameObject muzzleflashEffectPrefab = EntityStates.Squid.SquidWeapon.FireSpine.muzzleflashEffectPrefab;

		public static float damageCoefficient = 5f;
		public static float procCoefficient = 0.1f;
		public static float baseDuration = 0.75f; //was 2f originally, but that seems way too slow, also tried 1f slow again, tried 0.75f as that's what the wiki states

		private const float maxVisionDistance = float.PositiveInfinity;
		public bool fullVision = true;

		private bool hasFiredArrow;
		//private ChildLocator childLocator;
		private BullseyeSearch enemyFinder;
		private float duration;
	}
}
