using System;
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
		public static BuffDef LeechBuff = RoR2Content.Buffs.Warbanner;
		public static DotController.DotDef LeechDotDef;
		private static DotController.DotIndex LeechDotIndex;
		internal static bool Enable = false;
		internal static float BaseDoTHeal = 4f;
		internal static float StackDoTHeal = 4f;
		internal static float LeechChance = 0.25f;
		internal static float LeechLifeSteal = 0.02f;
		internal static float LeechMinLifeSteal = 0.5f;
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
			if (LeechChance > 0f)
			{
				CreateBuff();
			}
			UpdateText();
			Hooks();
		}
		private void CreateBuff()
		{
			LeechBuff = Modules.Buffs.AddNewBuff("Leech(FlatItemBuff)", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Bandit2/bdSuperBleed.asset").WaitForCompletion().iconSprite, new Color(0.53f, 0.77f, 0.31f, 1f), false, false, false);
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
				pickup += string.Format("Dealing status damage heals you.");
				desc += string.Format("Dealing status damage <style=cIsHealing>heals</style> you for <style=cIsHealing>{0} <style=cStack>(+{1} per stack)</style> health</style>.", BaseDoTHeal, StackDoTHeal);
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
			IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			float procrate = damageReport.damageInfo.procCoefficient;
			ProcChainMask procChainMask = damageReport.damageInfo.procChainMask;
			Inventory inventory = damageReport.attackerBody.inventory;
			bool isDot = false;
			float totalHealing = 0f;
			float tickMult = 1f;
			if (damageReport.dotType != DotController.DotIndex.None)
			{
				isDot = true;
				DotController.DotDef dotDef = DotController.GetDotDef(damageReport.dotType);
				tickMult = dotDef.interval;
			}

			if (LeechChance > 0f)
			{
				if (damageReport.dotType != LeechDotIndex && damageReport.victimBody.HasBuff(LeechBuff))
				{
					totalHealing += LeechHealing(damageReport.damageDealt, procrate, isDot);
				}
			}
			if (inventory)
			{
				int itemCount = inventory.GetItemCount(RoR2Content.Items.Seed);
				if (itemCount > 0)
				{
					if (LeechChance > 0f && procrate > 0f)
                    {
						if (damageReport.victim)
						{
							if (Util.CheckRoll(procrate * LeechChance, damageReport.attackerMaster))
							{
								DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, LeechDotIndex, LeechDuration(itemCount) * procrate, 1f, 1);
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
			if (totalHealing != 0f)
            {
				damageReport.attackerBody.healthComponent.Heal(totalHealing, procChainMask, true);
			}
		}
		private float LeechHealing(float damage, float procrate, bool isDot = false)
        {
			if (isDot)
			{
				if (procrate < 0.5f)
				{
					procrate = 0.5f;
				}
			}
			return Math.Max(LeechMinLifeSteal, damage * LeechLifeSteal * procrate);
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
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Seed")
			);
			ilcursor.Index += 2;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
			/*
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStloc(x, 19),
				x => ILPatternMatchingExt.MatchLdloc(x, 19)
			);
			ilcursor.Index -= 3;
			ilcursor.RemoveRange(3);
			ilcursor.Emit(OpCodes.Ldc_I4, 0);*/
		}
	}
}
