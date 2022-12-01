using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class LeechingSeed_Rework
	{
		public static BuffDef LeechBuff = RoR2Content.Buffs.Warbanner;
		public static DotController.DotDef LeechDotDef;
		private static DotController.DotIndex LeechDotIndex;

		private static float FlatHealing = 2f;
		private static float LeechChance = 0.25f;
		private static float LifeSteal = 0.02f;
		private static float MinLifeStealPerLevel = 0.1f;
		private static float LeechBaseDamage = 0.5f;
		private static float LeechBaseDuration = 5f;
		private static float LeechStackDuration = 0f;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Leeching Seed");
			if (LeechChance > 0f)
			{
				CreateBuff();
			}
			SetupConfigValues();
			UpdateText();
			Hooks();
		}
		private static void SetupConfigValues()
        {
			LifeSteal = MainPlugin.LeechingSeedRework_DoTLifeSteal.Value;
			MinLifeStealPerLevel = MainPlugin.LeechingSeedRework_DoTMinLifeSteal.Value;
			LeechChance = MainPlugin.LeechingSeedRework_DoTChance.Value;
			FlatHealing = MainPlugin.LeechingSeedRework_DoTFlatHeal.Value;
			LeechBaseDamage = MainPlugin.LeechingSeedRework_DoTBaseDamage.Value;
			LeechBaseDuration = MainPlugin.LeechingSeedRework_DoTBaseDuration.Value;
			LeechStackDuration = MainPlugin.LeechingSeedRework_DoTStackDuration.Value;
		}
		private static void CreateBuff()
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
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			if(FlatHealing > 0f)
            {
				pickup += string.Format("Dealing status damage heals you.");
				desc += string.Format("Dealing status damage <style=cIsHealing>heals</style> you for <style=cIsHealing>{0} <style=cStack>(+{0} per stack)</style> health</style>.", FlatHealing);
			}
			if(LeechChance > 0f)
            {
				if (FlatHealing > 0f)
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
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			GlobalEventManager.onServerDamageDealt += Global_DamageDealt;
		}
		private static void Global_DamageDealt(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageReport.attacker && damageReport.attackerBody)
			{
				float healing = 0f;
				float procrate = damageReport.damageInfo.procCoefficient;
				ProcChainMask procChainMask = damageReport.damageInfo.procChainMask;
				Inventory inventory = damageReport.attackerBody.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(RoR2Content.Items.Seed);
					if (itemCount > 0)
					{
						if (LeechChance > 0f)
                        {
							if (procrate > 0f)
							{
								if (damageReport.victim)
								{
									if (Util.CheckRoll(procrate * LeechChance, damageReport.attackerMaster))
									{
										float duration = LeechBaseDuration + (LeechStackDuration * (itemCount - 1));
										DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, LeechDotIndex, duration * procrate, 1f, 1);
									}
								}
							}
						}
						if (FlatHealing > 0f)
						{
							if (damageReport.dotType != DotController.DotIndex.None)
							{
								healing += FlatHealing * itemCount;
							}
						}
					}
				}
				if (LeechChance > 0f)
				{
					if (damageReport.victimBody.HasBuff(LeechBuff))
					{
						healing += Math.Max(damageReport.attackerBody.level * MinLifeStealPerLevel, damageReport.damageDealt * LifeSteal * procrate);
					}
				}
				if (healing > 0f)
				{
					damageReport.attackerBody.healthComponent.Heal(healing, procChainMask, true);
				}
			}
			
		}
		private static void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStloc(x, 19),
				x => ILPatternMatchingExt.MatchLdloc(x, 19)
			);
			ilcursor.Index -= 3;
			ilcursor.RemoveRange(3);
			ilcursor.Emit(OpCodes.Ldc_I4, 0);
		}
	}
}
