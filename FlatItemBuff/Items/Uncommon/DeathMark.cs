using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class DeathMark
	{
		private BuffDef DeathMarkDef = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/DeathMark/bdDeathMark.asset").WaitForCompletion();
		internal static bool Enable = false;
		internal static float DamagePerDebuff = 0.1f;
		internal static int MaxDebuffs = 5;
		internal static float BaseDuration = 7f;
		internal static float StackDuration = 7f;
		public DeathMark()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Death Mark");
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			DamagePerDebuff = Math.Max(0f, DamagePerDebuff);
			MaxDebuffs = Math.Max(0, MaxDebuffs);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("Mark enemies for death on hit");

			string desc = string.Format("Enemies are <style=cIsDamage>marked for death</style> on hit for <style=cIsUtility>{0}s</style>", BaseDuration);
			if (StackDuration > 0)
            {
				desc += string.Format(" <style=cStack>(+{0}s per stack)</style>", StackDuration);
			}
			desc += string.Format(".");
			if (DamagePerDebuff > 0f)
            {
				pickup += string.Format(", increasing all damage they take for each debuff");
				if (MaxDebuffs > 1)
				{
					desc += string.Format(" Marked targets take <style=cIsDamage>{0}%</style> more damage for each debuff they have up to <style=cIsDamage>{1}%</style>.", DamagePerDebuff * 100f, DamagePerDebuff * MaxDebuffs * 100f);
				}
				else
                {
					desc += string.Format(" Marked targets take <style=cIsDamage>{0}%</style> more damage.", DamagePerDebuff * 100f);
				}
			}
			pickup += ".";
			LanguageAPI.Add("ITEM_DEATHMARK_PICKUP", pickup);
			LanguageAPI.Add("ITEM_DEATHMARK_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_TakeDamage);
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			float procRate = damageReport.damageInfo.procCoefficient;
			bool isDot = damageReport.dotType != DotController.DotIndex.None;
			if (procRate > 0f)
			{
				if (attackerBody && victimBody)
				{
					Inventory inventory = damageReport.attackerBody.inventory;
					if (inventory)
					{
						int itemCount = inventory.GetItemCount(RoR2Content.Items.DeathMark);
						if (itemCount > 0)
						{
							float duration = GetDeathMarkDuration(itemCount);
							if (duration > 0f)
                            {
								victimBody.AddTimedBuff(RoR2Content.Buffs.DeathMark, duration);
							}
						}
					}
				}
				
			}
		}
		private float GetDeathMarkDuration(int itemCount)
        {
			itemCount = Math.Max(0, itemCount - 1);
			return BaseDuration + (StackDuration * itemCount);
		}
		private void IL_TakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, typeof(RoR2Content.Buffs), "DeathMark")
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 7),
				x => ILPatternMatchingExt.MatchLdcR4(x, 1.5f),
				x => ILPatternMatchingExt.MatchMul(x)
			);
			ilcursor.Index += 1;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
			{
				int debuffCount = 0;
				DotController dotController = DotController.FindDotController(self.body.gameObject);
				List<BuffIndex> debuffList = BuffCatalog.debuffBuffIndices.ToList<BuffIndex>();
				if (dotController)
				{
					for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < (DotController.DotIndex)(DotAPI.VanillaDotCount+ DotAPI.CustomDotCount); dotIndex++)
					{
						if (dotController.HasDotActive(dotIndex))
						{
							BuffDef buffDef = DotController.GetDotDef(dotIndex).associatedBuff;
							if (buffDef)
                            {
								debuffList.Remove(buffDef.buffIndex);
								debuffCount++;
							}
						}
					}
				}
				foreach (BuffIndex buffType in debuffList)
				{
					if (self.body.HasBuff(buffType))
					{
						debuffCount++;
					}
				}
				debuffCount = Math.Min(debuffCount, MaxDebuffs);
				return 1f + (debuffCount * DamagePerDebuff);
			});
		}
		private void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "DeathMark")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				MainPlugin.ModLogger.LogError(MainPlugin.MODNAME + ": Death Mark - Effect Override - IL Hook failed");
			}
		}
	}
}
