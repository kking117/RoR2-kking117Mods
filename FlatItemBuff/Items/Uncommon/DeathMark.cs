using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace FlatItemBuff.Items
{
	public class DeathMark
	{
		private const string LogName = "Death Mark";
		internal static bool Enable = false;
		internal static float DamagePerDebuff = 0.1f;
		internal static int MaxDebuffs = 5;
		internal static float BaseDuration = 6f;
		internal static float StackDuration = 4f;
		public DeathMark()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
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
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_TakeDamage);
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			On.RoR2.GlobalEventManager.ProcDeathMark += CheckProcDeathMark;
		}
		private void CheckProcDeathMark(On.RoR2.GlobalEventManager.orig_ProcDeathMark orig, GameObject victim, CharacterBody victimBody, CharacterMaster attackerMaster)
		{
			return;
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
					if (attackerBody != victimBody)
                    {
						Inventory inventory = damageReport.attackerBody.inventory;
						if (inventory)
						{
							int itemCount = inventory.GetItemCountEffective(RoR2Content.Items.DeathMark);
							if (itemCount > 0)
							{
								float duration = GetDeathMarkDuration(itemCount) * procRate;
								if (duration > 0f)
								{
									victimBody.AddTimedBuff(RoR2Content.Buffs.DeathMark, duration);
								}
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
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "DeathMark")
			)
			&&
			ilcursor.TryGotoNext(
				x => x.MatchLdloc(9),
				x => x.MatchLdcR4(1.5f),
				x => x.MatchMul()
			))
			{
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
						for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < (DotController.DotIndex)(DotAPI.VanillaDotCount + DotAPI.CustomDotCount); dotIndex++)
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
			else
            {
				MainPlugin.ModLogger.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_TakeDamage - Hook failed");
			}
		}
	}
}
