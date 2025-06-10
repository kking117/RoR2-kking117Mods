using System;
using RoR2;
using R2API;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class LigmaLenses
	{
		private const string LogName = "Lost Seer's Lenses";
		internal static bool Enable = false;
		internal static float BaseDamage = 50.0f;
		internal static float StackDamage = 0.0f;
		internal static float BaseChance = 0.5f;
		internal static float StackChance = 0.5f;
		internal static bool UseTotalDamage = false;
		private static GameObject HitEffect;
		public LigmaLenses()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateVFX();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseChance = Math.Max(0f, BaseChance);
			StackChance = Math.Max(0f, StackChance);
		}
		private void UpdateVFX()
        {
			//Remove the goofy nubme
			//"RoR2/DLC1/CritGlassesVoid/CritGlassesVoidExecuteEffect.prefab"
			HitEffect = Addressables.LoadAssetAsync<GameObject>("9a17948773f51b749824026e541c9c1a").WaitForCompletion();
			if (HitEffect)
			{
				UnityEngine.Object.Destroy(HitEffect.transform.Find("FakeDamageNumbers").gameObject);
			}
			else
			{
				MainPlugin.ModLogger.LogInfo("Hit effect not found, expect errors.");
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");

			string descChance = string.Format("<style=cIsDamage>{0}%", BaseChance);
			if (StackChance > 0.0f)
            {
				descChance += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackChance);
			}
			descChance += "</style>";

			string descDamage = string.Format("<style=cIsDamage>{0}%", BaseDamage * 100f);
			if (StackDamage > 0.0f)
			{
				descDamage += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 100f);
			}
			if (UseTotalDamage)
            {
				descDamage += "</style> TOTAL damage";
			}
			else
            {
				descDamage += "</style> base damage";
			}

			string pickup = "Chance to detain enemies on hit. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
			string desc = string.Format("Your attacks have a {0} chance to <style=cIsHealth>detain</style> an enemy for {1}. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.", descChance, descDamage);
			LanguageAPI.Add("ITEM_CRITGLASSESVOID_PICKUP", pickup);
			LanguageAPI.Add("ITEM_CRITGLASSESVOID_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_TakeDamage);
			SharedHooks.Handle_GlobalHitEvent_Actions += GlobalEventManager_HitEnemy;
		}
		private void GlobalEventManager_HitEnemy(CharacterBody victimBody, CharacterBody attackerBody, DamageInfo damageInfo)
		{
			if (damageInfo.procCoefficient <= 0f || damageInfo.rejected)
			{
				return;
			}
			if (victimBody.healthComponent.alive)
			{
				CharacterMaster attackerMaster = attackerBody.master;
				if (attackerMaster)
				{
					Inventory inventory = attackerMaster.inventory;
					int itemCount = inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
					if (itemCount > 0)
					{
						float effectChance = BaseChance + (StackChance * (itemCount - 1));
						if (Util.CheckRoll(effectChance * damageInfo.procCoefficient, attackerMaster))
						{
							float coefDamage = BaseDamage + (StackDamage * (itemCount - 1));
							float baseDamage = attackerBody.damage;
							if (UseTotalDamage)
                            {
								baseDamage = damageInfo.damage;
                            }
							DamageInfo detainInfo = new DamageInfo
							{
								damage = baseDamage * coefDamage,
								damageColorIndex = DamageColorIndex.Void,
								damageType = DamageType.Generic,
								attacker = damageInfo.attacker,
								crit = damageInfo.crit,
								force = Vector3.zero,
								inflictor = null,
								position = damageInfo.position,
								procChainMask = damageInfo.procChainMask,
								procCoefficient = 0f
							};
							victimBody.healthComponent.TakeDamage(detainInfo);
							victimBody.healthComponent.killingDamageType = DamageType.VoidDeath;

							EffectManager.SpawnEffect(HitEffect, new EffectData
							{
								origin = damageInfo.position,
								scale = victimBody.radius
							}, true);
						}
					}
				}
			}
		}
		private void IL_TakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC1Content.Items), "CritGlassesVoid")
			)
			&&
			ilcursor.TryGotoNext(
				x => x.MatchLdcR4(0.5f)
			))
			{
				ilcursor.Next.Operand = 0f;
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_TakeDamage - Hook failed");
			}
		}
	}
}
