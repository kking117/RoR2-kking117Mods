﻿using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class KnockbackFin
	{
		internal static bool Enable = true;
		internal static float BaseForce = 20f;
		internal static float StackForce = 2f;
		internal static float BackForce = 0.5f;
		internal static float T1Mult = 1f;
		internal static float T2Mult = 5f;
		internal static float T3Mult = 10f;
		internal static float ChampionMult = 0.5f;
		internal static float BossMult = 1f;
		internal static float MaxForce = 200f;
		internal static float BaseDamage = 1f;
		internal static float StackDamage = 0.1f;
		internal static float MaxDistDamage = 10f;
		internal static float BaseRadius = 12f;
		internal static float Cooldown = 5f;
		internal static float ProcRate = 0f;
		internal static bool DoStun = true;
		private static BuffDef knockBackBuff;

		internal static GameObject ImpactEffect;
		public KnockbackFin()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Knockback Fin");
			ClampConfig();
			UpdateText();
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
			T1Mult = Math.Max(0f, T1Mult);
			T2Mult = Math.Max(0f, T2Mult);
			T3Mult = Math.Max(0f, T3Mult);
			ChampionMult = Math.Max(0f, ChampionMult);
			BossMult = Math.Max(0f, BossMult);
			MaxForce = Math.Max(1f, MaxForce);
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseRadius = Math.Max(0f, BaseRadius);
			Cooldown = Math.Max(0f, Cooldown);
			ProcRate = Math.Max(0f, ProcRate);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			if (BaseForce > 0f)
			{
				pickup += "Knock enemies upwards on hit.";
				
				if (StackForce > 0f)
                {
					desc += string.Format("Knock enemies upwards <style=cStack>(downward if airborne)</style> on hit with <style=cIsUtility>{0}%</style> <style=cStack>(+{1}% per stack)</style> <style=cIsUtility>force</style>.", 100f, StackForce / BaseForce * 100f);
				}
				else
                {
					desc += string.Format("Knock enemies upwards <style=cStack>(downward if airborne)</style> on hit with <style=cIsUtility>{0}%</style> <style=cIsUtility>force</style>.", 100f);
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
					desc += string.Format(" that scales up with <style=cIsDamage>fall distance</style>.");
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
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC2/Items/KnockBackHitEnemies/KnockBackHitEnemies.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.Damage);
			}
		}
		private void UpdateVFX()
		{
			ImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardGroundSlam.prefab").WaitForCompletion();
		}
		private void UpdateBuff()
		{
			knockBackBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC2/Items/KnockBackHitEnemies/bdKnockUpHitEnemies.asset").WaitForCompletion();
			BuffDef weakDef = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Treebot/bdWeak.asset").WaitForCompletion();
			knockBackBuff.isHidden = false;
			knockBackBuff.iconSprite = weakDef.iconSprite;
			knockBackBuff.buffColor = new Color(0.95f, 0.35f, 0.9f);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			On.RoR2.RigidbodyMotor.OnCollisionEnter += RigidBody_OnImpact;
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
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
					int itemCount = attackerBody.inventory.GetItemCount(DLC2Content.Items.KnockBackHitEnemies);
					if (itemCount > 0)
					{
						itemCount = Math.Max(0, itemCount);
						if (!victimBody.HasBuff(DLC2Content.Buffs.KnockUpHitEnemies))
						{
							CharacterMotor victimMotor = victimBody.GetComponent<CharacterMotor>();
							Rigidbody victimRigid = victimBody.GetComponent<Rigidbody>();
							if (victimMotor)
							{
								float itemForce = Math.Min(BaseForce + (itemCount * StackForce), MaxForce);

								float vertForce = T1Mult;
								float pushForce = BackForce;
								switch (victimBody.hullClassification)
								{
									case HullClassification.Golem:
										vertForce = T2Mult;
										break;
									case HullClassification.BeetleQueen:
										vertForce = T3Mult;
										break;
								}

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
									EffectManager.SpawnEffect(GlobalEventManager.CommonAssets.knockbackFinEffect, new EffectData
									{
										origin = victimBody.gameObject.transform.position,
										scale = itemForce / 20f
									}, true);

									Ray aimRay = attackerBody.inputBank.GetAimRay();
									Vector3 vertVector = new Vector3(0f, 1f, 0f);

									vertForce *= victimMotor.mass;
									pushForce *= vertForce;

									if (!victimMotor.isGrounded)
									{
										vertForce *= -1f;
									}

									Util.PlaySound("Play_item_proc_knockBackHitEnemies", attackerBody.gameObject);

									victimBody.AddBuff(DLC2Content.Buffs.KnockUpHitEnemies);

									victimMotor.ApplyForce(vertForce * vertVector, false, false);
									victimMotor.ApplyForce(pushForce * aimRay.direction, false, false);

									if (BaseRadius > 0f)
									{
										Components.FinImpact comp = victimBody.GetComponent<Components.FinImpact>();
										if (!comp)
										{
											comp = victimBody.gameObject.AddComponent<Components.FinImpact>();
										}
										comp.itemCount = itemCount;
										comp.attackerBody = attackerBody;
									}
								}
							}
							else if (victimRigid)
                            {
								float itemForce = Math.Min(BaseForce + (itemCount * StackForce), MaxForce);

								float vertForce = T1Mult;
								float pushForce = BackForce;
								switch (victimBody.hullClassification)
								{
									case HullClassification.Golem:
										vertForce = T2Mult;
										break;
									case HullClassification.BeetleQueen:
										vertForce = T3Mult;
										break;
								}

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
									EffectManager.SpawnEffect(GlobalEventManager.CommonAssets.knockbackFinEffect, new EffectData
									{
										origin = victimBody.gameObject.transform.position,
										scale = itemForce / 20f
									}, true);

									Ray aimRay = attackerBody.inputBank.GetAimRay();
									Vector3 vertVector = new Vector3(0f, 1f, 0f);

									vertForce *= victimRigid.mass;
									pushForce *= vertForce;
									vertForce *= -1f;

									Util.PlaySound("Play_item_proc_knockBackHitEnemies", attackerBody.gameObject);

									victimBody.AddBuff(DLC2Content.Buffs.KnockUpHitEnemies);

									victimRigid.AddForce(vertForce * vertVector, ForceMode.Impulse);
									victimRigid.AddForce(pushForce * aimRay.direction, ForceMode.Impulse);

									if (BaseRadius > 0f)
									{
										Components.FinImpactRigid comp = victimBody.GetComponent<Components.FinImpactRigid>();
										if (!comp)
										{
											comp = victimBody.gameObject.AddComponent<Components.FinImpactRigid>();
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

		private void RigidBody_OnImpact (On.RoR2.RigidbodyMotor.orig_OnCollisionEnter orig, RigidbodyMotor self, Collision collision)
        {
			if(collision.gameObject.layer == LayerIndex.world.intVal)
            {
				float resistance = Mathf.Max(self.characterBody.moveSpeed, self.characterBody.baseMoveSpeed) * 2f;
				float magnitude = collision.relativeVelocity.magnitude;
				if (magnitude >= resistance)
                {
					Components.FinImpactRigid comp = self.characterBody.GetComponent<Components.FinImpactRigid>();
					if (comp)
					{
						comp.Detonate(collision.contacts[0].point);
					}
				}
			}
			orig(self, collision);
        }

		internal static float GetImpactDamage(int itemCount)
        {
			return BaseDamage + (StackDamage * itemCount);
		}

		internal static float GetImpactRadius(int itemCount)
		{
			return BaseRadius;
		}
		private void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "KnockBackHitEnemies"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Knockback Fin IL Hook failed");
			}
		}
	}
}