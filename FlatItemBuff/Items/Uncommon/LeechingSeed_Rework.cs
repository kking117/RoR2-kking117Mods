﻿using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class LeechingSeed_Rework
	{
		public static BuffDef LeechBuff;
		private static Color BuffColor = new Color(0.53f, 0.77f, 0.31f, 1f);
		public static DotController.DotDef LeechDotDef;
		private static DotController.DotIndex LeechDotIndex;
		internal static bool Enable = false;
		internal static float BaseDoTHeal = 4f;
		internal static float StackDoTHeal = 4f;
		internal static float LeechChance = 0.25f;
		internal static float LeechLifeSteal = 0.1f;
		internal static float LeechMinLifeSteal = 1f;
		internal static float LeechBaseDamage = 0.5f;
		internal static float LeechBaseDuration = 5f;
		internal static float LeechStackDuration = 0f;
		public LeechingSeed_Rework()
		{
			if (!Enable)
            {
				new LeechingSeed();
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Leeching Seed");
			ClampConfig();
			if (LeechChance > 0f)
			{
				CreateBuff();
			}
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDoTHeal = Math.Max(0f, BaseDoTHeal);
			StackDoTHeal = Math.Max(0f, StackDoTHeal);
			LeechChance = Math.Max(0f, LeechChance);
			LeechLifeSteal = Math.Max(0f, LeechLifeSteal);
			LeechMinLifeSteal = Math.Max(0f, LeechMinLifeSteal);
			LeechBaseDamage = Math.Max(0f, LeechBaseDamage);
			LeechBaseDuration = Math.Max(0f, LeechBaseDuration);
			LeechStackDuration = Math.Max(0f, LeechStackDuration);
		}
		private void CreateBuff()
		{
			LeechBuff = Utils.ContentManager.AddBuff("Leech", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Bandit2/bdSuperBleed.asset").WaitForCompletion().iconSprite, BuffColor, false, false, false);
			LeechDotDef = new DotController.DotDef
			{
				associatedBuff = LeechBuff,
				damageCoefficient = LeechBaseDamage / 4f,
				damageColorIndex = DamageColorIndex.Item,
				interval = 0.25f
			};
			LeechDotIndex = DotAPI.RegisterDotDef(LeechDotDef);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			if(BaseDoTHeal > 0f)
            {
				pickup += string.Format("Dealing damage over time heals you.");
				desc += string.Format("Dealing damage over time <style=cIsHealing>heals</style> you for <style=cIsHealing>{0} <style=cStack>(+{1} per stack)</style> health</style>.", BaseDoTHeal, StackDoTHeal);
			}
			if(LeechChance > 0f)
            {
				if (BaseDoTHeal > 0f)
                {
					pickup += " ";
					desc += " ";
                }
				pickup += "Chance to leech enemies on hit.";
				string StackB = "";
				if (LeechStackDuration != 0f)
				{
					StackB += string.Format(" <style=cStack>(+{0}% per stack)</style>", LeechBaseDamage * LeechStackDuration * 100f);
				}
				desc += string.Format("<style=cIsDamage>{0}%</style> chance to <style=cIsHealing>Leech</style> an enemy for <style=cIsDamage>{1}%</style>{2} base damage.", LeechChance, LeechBaseDamage * LeechBaseDuration * 100f, StackB);
			}
			LanguageAPI.Add("ITEM_SEED_PICKUP", pickup);
			LanguageAPI.Add("ITEM_SEED_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			if (attackerBody)
			{
				ProcChainMask procChainMask = damageReport.damageInfo.procChainMask;
				float procRate = damageReport.damageInfo.procCoefficient;
				bool isDot = false;

				float tickMult = 1f;
				if (damageReport.dotType != DotController.DotIndex.None)
				{
					isDot = true;
					DotController.DotDef dotDef = DotController.GetDotDef(damageReport.dotType);
					tickMult = dotDef.interval;
				}

				float totalHealing = 0f;
				if (LeechChance > 0f)
				{
					if (damageReport.dotType != LeechDotIndex && damageReport.victimBody.HasBuff(LeechBuff))
					{
						totalHealing += LeechHealing(damageReport.damageDealt, procRate, isDot);
					}
				}
				Inventory inventory = attackerBody.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(RoR2Content.Items.Seed);
					if (itemCount > 0)
					{
						if (LeechChance > 0f && procRate > 0f)
						{
							if (damageReport.victim)
							{
								if (Util.CheckRoll(procRate * LeechChance, damageReport.attackerMaster))
								{
									DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, LeechDotIndex, LeechDuration(itemCount) * procRate, 1f, 1);
								}
							}
						}
						if (BaseDoTHeal > 0f)
						{
							if (isDot)
							{

								totalHealing += LeechDoTHealing(itemCount) * tickMult;
							}
						}
					}
				}
				//For visual clarity and since they're both very much related to the same item
				if (totalHealing > 0f)
				{
					damageReport.attackerBody.healthComponent.Heal(totalHealing, procChainMask, true);
				}
			}
		}
		private float LeechHealing(float damage, float procRate, bool isDot = false)
        {
			if (isDot)
			{
				if (procRate < 0.25f)
				{
					procRate = 0.25f;
				}
			}
			float result = damage * LeechLifeSteal * procRate;
			result /= Run.instance.difficultyCoefficient;
			return Math.Max(LeechMinLifeSteal, result);
		}
		private float LeechDoTHealing(float itemCount)
        {
			itemCount = Math.Max(0, itemCount - 1);
			return BaseDoTHeal + (StackDoTHeal * itemCount);
		}

		private float LeechDuration(int itemCount)
        {
			itemCount = Math.Max(0, itemCount - 1);
			return LeechBaseDuration + (LeechStackDuration * itemCount);
		}
		private void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Seed")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Leeching Seed Rework - Effect Override - IL Hook failed");
			}
		}
	}
}
