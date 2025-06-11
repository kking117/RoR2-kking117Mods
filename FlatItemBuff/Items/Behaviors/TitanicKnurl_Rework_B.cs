using System;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using EntityStates.TitanMonster;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items.Behaviors
{
    public class TitanicKnurl_Rework_B : CharacterBody.ItemBehavior
    {
		private SkillLocator skillLocator;
		private InputBankTest inputBank;
		private int ExtraShots = Items.TitanicKnurl_Rework_B.ExtraShots;

		private static string LaserParentA = "MuzzleLaser";
		private static string LaserParentB = "Head";
		private static bool ParentAimRay = false;
		private static Transform laserMuzzle = null;

		private static float laserVFXScale = 0.25f;
		private static float laserMaxDistance = 180f;
		private static float laserLockDistance = 120f;
		private static float laserAimAngle = 3f;
		private static float shotMaxDistance = 180f;
		private static float shotAimAngle = 9f;
		private static float shotMinAngle = 3f;
		private static float shotMaxAngle = 9f;

		private bool fullyCharged = false;
		private int laserPhase = 0;
		private float phaseTime = -1f;
		private float laserDamageRate = 0.5f;
		private float nextLaserBullet = -1f;
		private float shotDamageRate = 1.5f;
		private bool laserIsCrit = false;
		private int storedShots = 0;

		BullseyeSearch targetSearch = null;
		HurtBox currentTarget = null;

		private BulletAttack bulletAttack;
		public GameObject effectPrefab;
		public GameObject hitEffectPrefab;
		//"RoR2/Base/Titan/LaserTitan.prefab"
		public GameObject laserPrefab = Addressables.LoadAssetAsync<GameObject>("a9d8d0c6a96e5964d87c197d2e87e24e").WaitForCompletion();
		private GameObject laserEffect = null;
		private Transform laserEffectEnd = null;
		private ChildLocator laserChildLocator = null;
		private Vector3 laserEndPoint = new Vector3(0, 0, 0);

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
				body.SetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff.buffIndex, 0);
				EndLaser();
			}
			targetSearch = null;
			currentTarget = null;
			inputBank = null;
			skillLocator = null;
		}
		private void OnSkillActivated(GenericSkill skill)
		{
			if (laserPhase < 1)
            {
				SkillLocator skillLocator = this.skillLocator;
				if (skillLocator != null && skillLocator.special != null)
				{
					if (skillLocator.special == skill)
					{
						if (body.GetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff) >= Items.TitanicKnurl_Rework_B.ChargeCap)
						{
							body.SetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff.buffIndex, 0);
							ChargeLaser();
						}
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
			if (!body.healthComponent || !body.healthComponent.alive)
			{
				if (laserPhase > 0)
                {
					EndLaser();
				}
			}
			if (laserPhase == 2)
            {
				phaseTime -= Time.fixedDeltaTime;
				if (phaseTime < 0f)
				{
					EndLaser();
				}
				else
				{
					body.SetAimTimer(2f);
					UpdateLaserEffect();
					nextLaserBullet -= Time.fixedDeltaTime;
					if (nextLaserBullet < 0f)
					{
						FireLaser();
						nextLaserBullet += 1f / 8f;
					}
					if (ExtraShots > 0)
					{
						TryFireShot();
					}
					else
					{
						body.SetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff.buffIndex, 0);
					}
				}
			}
			else if (laserPhase == 1)
            {
				phaseTime -= Time.fixedDeltaTime;
				if (phaseTime < 0f)
				{
					StartLaser();
				}
				else
				{
					body.SetAimTimer(2f);
					body.SetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff.buffIndex, 0);
				}
			}
			else
            {
				int buffCount = body.GetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff.buffIndex);
				if (buffCount >= Items.TitanicKnurl_Rework_B.ChargeCap)
                {
					if (!fullyCharged)
                    {
						fullyCharged = true;
						Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, base.gameObject);
						EffectData vfxData = new EffectData();
						vfxData.origin = body.corePosition;
						EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, vfxData, true);
					}
				}
				else
                {
					fullyCharged = false;
				}
            }
		}
		private void ChargeLaser()
        {
			laserPhase = 1;
			phaseTime = 0.4f;
		}
		private void StartLaser()
        {
			DestroyLaser();
			laserPhase = 2;
			phaseTime = Items.TitanicKnurl_Rework_B.BaseDuration + (Math.Max(0, base.stack - 1) * Items.TitanicKnurl_Rework_B.StackDuration);
			laserDamageRate = Items.TitanicKnurl_Rework_B.LaserBaseDamage + (Math.Max(0, base.stack - 1) * Items.TitanicKnurl_Rework_B.LaserStackDamage);
			shotDamageRate = Items.TitanicKnurl_Rework_B.ShotBaseDamage + (Math.Max(0, base.stack - 1) * Items.TitanicKnurl_Rework_B.ShotStackDamage);
			laserIsCrit = Util.CheckRoll(base.body.crit, base.body.master);
			Util.PlaySound(FireMegaLaser.playAttackSoundString, base.gameObject);
			Util.PlaySound(FireMegaLaser.playLoopSoundString, base.gameObject);
			nextLaserBullet = 1f / 8f;
			Ray aimRay = GetAimRay();

			laserMuzzle = null;
			ParentAimRay = true;
			ModelLocator parentModel = body.GetComponent<ModelLocator>();
			if (parentModel && parentModel.modelTransform)
			{
				ChildLocator parentLocator = parentModel.modelTransform.GetComponent<ChildLocator>();
				if (parentLocator)
				{
					Transform parentTransform = GetParentTransform(parentLocator);
					if (parentTransform)
					{
						ParentAimRay = false;
						laserMuzzle = parentTransform;
					}
				}
			}
			if (ParentAimRay)
			{
				laserEffect = UnityEngine.Object.Instantiate<GameObject>(laserPrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction));
			}
			else
			{
				laserEffect = UnityEngine.Object.Instantiate<GameObject>(laserPrefab, laserMuzzle.position, laserMuzzle.rotation);
				//laserEffect.transform.parent = laserMuzzle;
			}
			laserChildLocator = laserEffect.GetComponent<ChildLocator>();
			laserEffectEnd = laserChildLocator.FindChild("LaserEnd");
			//scale down the vfx
			Transform startVFX = laserEffect.transform.GetChild(1);
			if (startVFX)
			{
				Transform vfx = startVFX.GetChild(0);
				if (vfx)
				{
					vfx.localScale *= laserVFXScale;
				}
				vfx = startVFX.GetChild(1);
				if (vfx)
				{
					vfx.localScale *= laserVFXScale;
				}
			}
			Transform bezierVFX = laserEffect.transform.GetChild(3);
			if (bezierVFX)
			{
				bezierVFX.localScale *= laserVFXScale;
			}

			ResetTargetSearch();
		}
		private Transform GetParentTransform(ChildLocator parentLocator)
        {
			int childIndex = parentLocator.FindChildIndex(LaserParentA);
			Transform childTransform = parentLocator.FindChild(childIndex);
			if (childTransform)
			{
				return childTransform;
			}
			childIndex = parentLocator.FindChildIndex(LaserParentB);
			childTransform = parentLocator.FindChild(childIndex);
			if (childTransform)
			{
				return childTransform;
			}
			return null;
		}
		private void EndLaser()
		{
			phaseTime = -1f;
			laserPhase = 0;
			DestroyLaser();
		}
		private void DestroyLaser()
        {
			Util.PlaySound(FireMegaLaser.stopLoopSoundString, base.gameObject);
			if (laserEffect)
			{
				UnityEngine.Object.Destroy(laserEffect);
				laserEffect = null;
			}
		}
		private void ResetTargetSearch()
        {
			targetSearch = new BullseyeSearch();
			targetSearch.viewer = body;
			targetSearch.maxDistanceFilter = laserLockDistance;
			targetSearch.maxAngleFilter = laserAimAngle;
			targetSearch.filterByLoS = true;
			targetSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			targetSearch.teamMaskFilter = TeamMask.allButNeutral;
			if (body.teamComponent)
			{
				targetSearch.teamMaskFilter.RemoveTeam(body.teamComponent.teamIndex);
			}
		}
		private void UpdateTargetSearch()
        {
			Ray aimRay = GetAimRay();
			if (targetSearch == null)
			{
				targetSearch = new BullseyeSearch();
				targetSearch.viewer = body;
				targetSearch.maxDistanceFilter = laserLockDistance;
				targetSearch.maxAngleFilter = laserAimAngle;
				targetSearch.searchOrigin = laserEffect.transform.position;
				targetSearch.searchDirection = aimRay.direction;
				targetSearch.filterByLoS = true;
				targetSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
				targetSearch.teamMaskFilter = TeamMask.allButNeutral;
				if (body.teamComponent)
				{
					targetSearch.teamMaskFilter.RemoveTeam(body.teamComponent.teamIndex);
				}
			}
			targetSearch.searchOrigin = laserEffect.transform.position;
			targetSearch.searchDirection = aimRay.direction;
			targetSearch.RefreshCandidates();
			currentTarget = null;
			foreach (HurtBox hbox in targetSearch.GetResults())
            {
				if (hbox.healthComponent && hbox.healthComponent.alive)
                {
					currentTarget = hbox;
					break;
				}
			}
			laserEndPoint = aimRay.GetPoint(laserMaxDistance);
			if (currentTarget)
            {
				laserEndPoint = currentTarget.collider.transform.position;
			}
			RaycastHit raycastHit;
			if (Util.CharacterRaycast(body.gameObject, new Ray(laserEffect.transform.position, laserEndPoint - laserEffect.transform.position), out raycastHit, laserMaxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
            {
				laserEndPoint = raycastHit.point;
			}
		}
		private void UpdateLaserEffect()
        {
			UpdateTargetSearch();
			if (laserEffect)
            {
				Ray aimRay = GetAimRay();
				if (ParentAimRay)
                {
					laserEffect.transform.position = aimRay.origin;
				}
				else
                {
					laserEffect.transform.position = laserMuzzle.transform.position;
				}
				laserEffect.transform.rotation = Util.QuaternionSafeLookRotation(laserEndPoint - laserEffect.transform.position);
				laserEffectEnd.transform.position = laserEndPoint;
			}
        }
		private void FireLaser()
        {
			Ray aimRay = GetAimRay();
			if (bulletAttack == null)
			{
				bulletAttack = new BulletAttack();
			}
			bulletAttack.owner = base.gameObject;
			bulletAttack.weapon = base.gameObject;
			bulletAttack.origin = laserEffect.transform.position;
			bulletAttack.aimVector = laserEndPoint - laserEffect.transform.position;
			bulletAttack.minSpread = 0f;
			bulletAttack.maxSpread = 0f;
			bulletAttack.bulletCount = 1;
			bulletAttack.damage = laserDamageRate * base.body.damage;
			bulletAttack.force = 0f;
			//bulletAttack.muzzleName = targetMuzzle;
			//bulletAttack.hitEffectPrefab = EntityStates.TitanMonster.FireMegaLaser.hitEffectPrefab;
			bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
			bulletAttack.isCrit = laserIsCrit;
			bulletAttack.procCoefficient = Items.TitanicKnurl_Rework_B.LaserProcRate;
			bulletAttack.HitEffectNormal = false;
			bulletAttack.damageColorIndex = DamageColorIndex.Item;
			if (currentTarget)
            {
				bulletAttack.radius = 0f;
			}
			else
            {
				bulletAttack.radius = 1f;
			}
			bulletAttack.maxDistance = laserMaxDistance;
			bulletAttack.Fire();
		}
		private Ray GetAimRay()
		{
			if (this.inputBank)
			{
				return new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
			}
			return new Ray(base.transform.position, base.transform.forward);
		}
		private void TryFireShot()
		{
			storedShots += Math.Max(0, body.GetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff));
			body.SetBuffCount(Items.TitanicKnurl_Rework_B.LaserChargeBuff.buffIndex, 0);
			if (storedShots >= ExtraShots)
			{
				storedShots -= ExtraShots;
				Ray aimRay = GetAimRay();
				GameObject target = null;
				if (currentTarget && currentTarget.healthComponent && currentTarget.healthComponent.alive)
                {
					target = currentTarget.gameObject;
				}
				else
                {
					target = GetLaserShotTarget();
				}
				Vector3 aimDirection = Util.ApplySpread(aimRay.direction, shotMinAngle, shotMaxAngle, 1f, 1f);
				ProjectileManager.instance.FireProjectileWithoutDamageType(Items.TitanicKnurl_Rework_B.LaserShotProjectile, laserEffect.transform.position, Util.QuaternionSafeLookRotation(aimDirection), base.gameObject, shotDamageRate * base.body.damage, 0f, laserIsCrit, DamageColorIndex.Item, target);
			}
		}

		private GameObject GetLaserShotTarget()
		{
			GameObject finalTarget = null;
			Ray aimRay = GetAimRay();
			BullseyeSearch tempSearch = new BullseyeSearch();

			tempSearch.viewer = body;
			tempSearch.maxDistanceFilter = shotMaxDistance;
			tempSearch.maxAngleFilter = shotAimAngle;
			tempSearch.searchOrigin = laserEffect.transform.position;
			tempSearch.searchDirection = aimRay.direction;
			tempSearch.filterByLoS = true;
			tempSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			tempSearch.teamMaskFilter = TeamMask.allButNeutral;
			if (body.teamComponent)
			{
				tempSearch.teamMaskFilter.RemoveTeam(body.teamComponent.teamIndex);
			}
			tempSearch.searchOrigin = laserEffect.transform.position;
			tempSearch.searchDirection = aimRay.direction;
			tempSearch.RefreshCandidates();
			HurtBox tempTarget = null;
			foreach (HurtBox hbox in tempSearch.GetResults())
			{
				if (hbox.healthComponent && hbox.healthComponent.alive)
				{
					tempTarget = hbox;
					break;
				}
			}
			if (tempTarget)
			{
				finalTarget = tempTarget.gameObject;
			}
			return finalTarget;
		}
	}
}
