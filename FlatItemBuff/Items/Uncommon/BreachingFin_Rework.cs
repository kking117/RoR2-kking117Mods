using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using KinematicCharacterController;

namespace FlatItemBuff.Items
{
	public class BreachingFin_Rework
	{
		internal static string LogName = "Breaching Fin Rework";
		internal static bool Enable = false;
		internal static float BaseForce = 20f;
		internal static float StackForce = 2f;
		internal static float BackForce = 0.5f;
		internal static float ChampionMult = 0.5f;
		internal static float BossMult = 1f;
		internal static float FlyingMult = 1f;
		internal static float MaxForce = 200f;
		internal static float BaseRadius = 12f;
		internal static float BaseDamage = 1.5f;
		internal static float ProcRate = 0f;
		internal static float StackDamage = 0.3f;
		internal static float MaxDistDamage = 10f;
		internal static bool OnSkill = true;
		internal static bool DoStun = true;
		internal static bool CreditFall = false;
		internal static int Cooldown = 10;
		
		internal static BuffDef KBDebuff;
		internal static BuffDef KBCooldown;
		private static Color buffColor = new Color(0.95f, 0.35f, 0.9f);

		internal static GameObject ImpactEffect;
		public BreachingFin_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			if (BaseRadius > 0f)
            {
				UpdateItemDef();
			}
			UpdateVFX();
			UpdateBuff();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseForce = Math.Max(0f, BaseForce);
			StackForce = Math.Max(0f, StackForce);
			BackForce = Math.Max(0f, BackForce);
			MaxDistDamage = Math.Max(1f, MaxDistDamage);
			ChampionMult = Math.Max(0f, ChampionMult);
			BossMult = Math.Max(0f, BossMult);
			MaxForce = Math.Max(1f, MaxForce);
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseRadius = Math.Max(0f, BaseRadius);
			Cooldown = Math.Max(0, Cooldown);
			ProcRate = Math.Max(0f, ProcRate);
		}
		private void UpdateText()
		{
			string pickup = "";
			string desc = "";
			if (BaseForce > 0f)
			{
				if (OnSkill)
                {
					pickup += "Enemies hit by skills are launched.";
					desc = "Enemies hit by <style=cIsUtility>skills</style> are <style=cIsUtility>launched</style> with";
				}
				else
                {
					pickup += "Enemies hit are launched.";
					desc = "Enemies hit are <style=cIsUtility>launched</style>";
				}
				
				if (StackForce > 0f)
                {
					desc += string.Format(" <style=cIsUtility>{0}%</style> <style=cStack>(+{1}% per stack)</style> <style=cIsUtility>force</style>.", 100f, StackForce / BaseForce * 100f);
				}
				else
                {
					desc += string.Format("  <style=cIsUtility>{0}%</style> <style=cIsUtility>force</style>.", 100f);
				}
				if (BaseRadius > 0f)
				{
					pickup += " ";
					desc += " ";
				}
			}
			if (BaseRadius > 0f)
			{
				pickup += "Launched enemies deal damage on impact.";
				if (StackDamage > 0f)
                {
					desc += string.Format("Enemies launched cause a <style=cIsDamage>{0}m</style> radius <style=cIsDamage>impact</style>, dealing <style=cIsDamage>{1}%</style> <style=cStack>(+{2}% per stack)</style> of your base damage", BaseRadius, BaseDamage * 100f, StackDamage * 100f);
				}
				else
                {
					desc += string.Format("Enemies launched cause a <style=cIsDamage>{0}m</style> radius <style=cIsDamage>impact</style>, dealing <style=cIsDamage>{1}%</style> of your base damage", BaseRadius, BaseDamage * 100f);
				}
				if (MaxDistDamage > 1f)
                {
					desc += string.Format(" that scales with <style=cIsDamage>velocity</style>.");
				}
				else
                {
					desc += string.Format(".");
				}
			}
			LanguageAPI.Add("ITEM_KNOCKBACKHITENEMIES_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNOCKBACKHITENEMIES_DESC", desc);
		}
		private void UpdateItemDef()
		{
			//"RoR2/DLC2/Items/KnockBackHitEnemies/Items.KnockBackHitEnemies.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("8d0e62a56e442ce41ab072c258ec22b1").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.Damage);
			}
		}
		private void UpdateVFX()
		{
			//"RoR2/Base/BeetleGuard/BeetleGuardGroundSlam.prefab"
			ImpactEffect = Addressables.LoadAssetAsync<GameObject>("d9cbb9db8a4992e49b933ab13eea4f9c").WaitForCompletion();
		}
		private void UpdateBuff()
		{
			//"RoR2/DLC2/Items/KnockBackHitEnemies/bdKnockUpHitEnemies.asset"
			KBDebuff = Addressables.LoadAssetAsync<BuffDef>("1877d8952d395174cabd9234e907c007").WaitForCompletion();
			//"RoR2/Base/Treebot/bdWeak.asset"
			BuffDef weakDef = Addressables.LoadAssetAsync<BuffDef>("41f235937593e0a4e954d6de0ec4c636").WaitForCompletion();
			KBCooldown = Utils.ContentManager.AddBuff("Knockback Cooldown", weakDef.iconSprite, buffColor, true, false, false);
			KBDebuff = Utils.ContentManager.AddBuff("Knockback Debuff", weakDef.iconSprite, buffColor, false, true, false);
			KBDebuff.flags |= BuffDef.Flags.ExcludeFromNoxiousThorns;
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_TakeDamageProcess);
			IL.RoR2.CharacterBody.OnInventoryChanged += new ILContext.Manipulator(IL_OnInventoryChanged);
			On.RoR2.RigidbodyMotor.OnCollisionEnter += RigidBody_OnImpact;
			On.RoR2.CharacterMotor.OnGroundHit += CharacterMotor_OnImpact;
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			if (CreditFall)
			{
				On.RoR2.HealthComponent.TakeDamageProcess += OnTakeDamage;
				IL.RoR2.RigidbodyMotor.OnCollisionEnter += new ILContext.Manipulator(IL_OnCollisionEnter);
			}
			On.RoR2.CharacterBody.UpdateKnockBackHitVisualsAndGracePeriod += UpdateKBVisuals;
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			float procRate = damageReport.damageInfo.procCoefficient;
			if (procRate > 0f)
			{
				CharacterBody victimBody = damageReport.victimBody;
				CharacterBody attackerBody = damageReport.attackerBody;
				if (attackerBody && victimBody && attackerBody.inventory)
				{
					if (attackerBody != victimBody)
                    {
						int itemCount = attackerBody.inventory.GetItemCountEffective(DLC2Content.Items.KnockBackHitEnemies);
						if (itemCount > 0)
						{
							if (!OnSkill || damageReport.damageInfo.damageType.IsDamageSourceSkillBased)
                            {
								itemCount = Math.Max(0, itemCount);
								if (!victimBody.HasBuff(KBCooldown) && !victimBody.HasBuff(KBDebuff))
								{
									CharacterMotor victimMotor = victimBody.GetComponent<CharacterMotor>();
									Rigidbody victimRigid = victimBody.GetComponent<Rigidbody>();
									if (victimMotor)
									{
										float vfxScale = 1f;
										switch (victimBody.hullClassification)
										{
											case HullClassification.Golem:
												vfxScale = 2f;
												break;
											case HullClassification.BeetleQueen:
												vfxScale = 3f;
												break;
										}

										float itemForce = Math.Min(BaseForce + (itemCount * StackForce), MaxForce);
										float vertForce = 1f;
										float pushForce = BackForce;

										if (victimBody.isChampion)
										{
											vertForce *= ChampionMult;
										}

										if (victimBody.isBoss)
										{
											vertForce *= BossMult;
										}

										vertForce *= itemForce;

										if (vertForce != 0f)
										{
											Ray aimRay = attackerBody.inputBank.GetAimRay();
											Vector3 vertVector = new Vector3(0f, 1f, 0f);

											vertForce *= victimMotor.mass;
											pushForce *= vertForce;

											if (!victimMotor.isGrounded)
											{
												if (victimBody.isFlying)
												{
													vertForce *= FlyingMult;
												}
												vertForce *= -1f;
											}

											Util.PlaySound("Play_item_proc_knockBackHitEnemies", attackerBody.gameObject);

											victimBody.AddBuff(KBDebuff);

											victimMotor.ApplyForce(vertForce * vertVector, false, false);
											victimMotor.ApplyForce(pushForce * aimRay.direction, false, false);

											Components.FinImpact comp = victimBody.GetComponent<Components.FinImpact>();
											if (!comp)
											{
												comp = victimBody.gameObject.AddComponent<Components.FinImpact>();
											}
											else
											{
												comp.Reset();
											}
											comp.itemCount = itemCount;
											comp.attackerBody = attackerBody;
											comp.enforceDuration = 0.5f + (itemForce * 0.01f);
										}
									}
									else if (victimRigid)
									{
										RigidbodyMotor victimRigidMotor = victimRigid.GetComponent<RigidbodyMotor>();
										if (victimRigidMotor)
										{
											float vfxScale = 1f;
											switch (victimBody.hullClassification)
											{
												case HullClassification.Golem:
													vfxScale = 2f;
													break;
												case HullClassification.BeetleQueen:
													vfxScale = 3f;
													break;
											}

											float itemForce = Math.Min(BaseForce + (itemCount * StackForce), MaxForce);
											float vertForce = 1f;
											float pushForce = BackForce;

											if (victimBody.isChampion)
											{
												vertForce *= ChampionMult;
											}

											if (victimBody.isBoss)
											{
												vertForce *= BossMult;
											}

											vertForce *= itemForce * FlyingMult;

											if (vertForce != 0f)
											{
												Ray aimRay = attackerBody.inputBank.GetAimRay();
												Vector3 vertVector = new Vector3(0f, 1f, 0f);

												vertForce *= victimRigid.mass;
												pushForce *= vertForce;
												vertForce *= -1f;

												Util.PlaySound("Play_item_proc_knockBackHitEnemies", attackerBody.gameObject);

												victimBody.AddBuff(KBDebuff);

												victimRigid.AddForce(vertForce * vertVector, ForceMode.Impulse);
												victimRigid.AddForce(pushForce * aimRay.direction, ForceMode.Impulse);

												Components.FinImpactRigid comp = victimBody.GetComponent<Components.FinImpactRigid>();
												if (!comp)
												{
													comp = victimBody.gameObject.AddComponent<Components.FinImpactRigid>();
												}
												else
												{
													comp.Reset();
												}
												comp.itemCount = itemCount;
												comp.attackerBody = attackerBody;
												comp.enforceDuration = 0.5f + (itemForce * 0.01f);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		private void UpdateKBVisuals(On.RoR2.CharacterBody.orig_UpdateKnockBackHitVisualsAndGracePeriod orig, CharacterBody self)
        {
			bool runVFX = self.HasBuff(KBDebuff);
			if (runVFX)
            {
				if (self.GetComponent<Components.FinImpact>() == null && self.GetComponent<Components.FinImpactRigid>() == null)
                {
					runVFX = false;
					self.RemoveBuff(KBDebuff);
                }
			}
			self.UpdateSingleTemporaryVisualEffect(ref self.knockbackHitEnemiesVulnerableInstance, CharacterBody.AssetReferences.KnockbackFinDebuffVFXPrefab, self.radius, runVFX, "");
			return;
		}
		private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
		{
			if ((damageInfo.damageType & DamageType.FallDamage) != DamageType.Generic)
            {
				CharacterBody launchBody = GetLaunchAttacker(self.body);
				if (launchBody)
                {
					damageInfo.attacker = launchBody.gameObject;
					if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                    {
						damageInfo.damageColorIndex = DamageColorIndex.Item;
					}
				}
			}
			orig(self, damageInfo);
        }
		private CharacterBody GetLaunchAttacker(CharacterBody targetBody)
        {
			Components.FinImpact compA = targetBody.GetComponent<Components.FinImpact>();
			if (compA)
			{
				return compA.attackerBody;
			}
			Components.FinImpactRigid compB = targetBody.GetComponent<Components.FinImpactRigid>();
			if (compB)
			{
				return compB.attackerBody;
			}
			return null;
		}
		private void CharacterMotor_OnImpact(On.RoR2.CharacterMotor.orig_OnGroundHit orig, CharacterMotor self, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
			orig(self, hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
			Components.FinImpact comp = self.GetComponent<Components.FinImpact>();
			if (comp)
			{
				comp.OnImpact();
			}
		}
		private void RigidBody_OnImpact(On.RoR2.RigidbodyMotor.orig_OnCollisionEnter orig, RigidbodyMotor self, Collision collision)
        {
			if(validCollision(collision))
            {
				float resistance = Mathf.Max(self.characterBody.moveSpeed, self.characterBody.baseMoveSpeed) * 2f;
				float magnitude = collision.relativeVelocity.magnitude;
				Components.FinImpactRigid comp = self.characterBody.GetComponent<Components.FinImpactRigid>();
				if (comp)
				{
					if (magnitude >= resistance)
					{
						comp.impactPos = collision.contacts[0].point;
						comp.OnImpact();
					}
				}
			}
			orig(self, collision);
        }
		private bool validCollision(Collision collision)
        {
			if (collision.gameObject.layer == LayerIndex.world.intVal)
            {
				return true;
            }
			if (collision.gameObject.layer == LayerIndex.playerBody.intVal)
			{
				return true;
			}
			return false;
        }
		internal static float GetImpactDamage(int itemCount)
        {
			return BaseDamage + (StackDamage * itemCount);
		}

		internal static float GetImpactRadius(int itemCount)
		{
			return BaseRadius;
		}
		private void IL_TakeDamageProcess(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "KnockBackHitEnemies"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_TakeDamageProcess - Hook failed");
			}
		}
		private void IL_OnCollisionEnter(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(4),
				x => x.MatchLdcR4(0f),
				x => x.MatchStfld(typeof(DamageInfo), "procCoefficient")
			))
			{
				ilcursor.Index += 1;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldloc, 4);
				ilcursor.EmitDelegate<Func<DamageInfo, float>>((damageInfo) =>
				{
					damageInfo.damageType = DamageType.FallDamage;
					return 0f;
				});
			}
			else
			{
				MainPlugin.ModLogger.LogError(MainPlugin.MODNAME + ": " + LogName  + " - IL_OnCollisionEnter - Hook failed");
			}
		}

		private void IL_OnInventoryChanged(ILContext il)
		{
			//doing this since it shits the bed otherwise
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "KnockBackHitEnemies"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnInventoryChanged - Hook failed.");
			}
		}
	}
}
