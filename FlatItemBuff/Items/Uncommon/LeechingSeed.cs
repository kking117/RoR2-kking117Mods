using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class LeechingSeed
	{
		internal static bool Enable = true;
		internal static float ProcHeal = 0.75f;
		internal static float BaseHeal = 0.75f;
		public LeechingSeed()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo("Changing Leeching Seed");
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			ProcHeal = Math.Max(0f, ProcHeal);
			BaseHeal = Math.Max(0, BaseHeal);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = string.Format("Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>{0} <style=cStack>(+{0} per stack)</style> health</style>.", ProcHeal + BaseHeal);
			LanguageAPI.Add("ITEM_SEED_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			if (BaseHeal > 0f)
			{
				SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
				IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_RemoveOldFunction);
			}
			else
			{
				IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_ModOldFunction);
			}
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			//This is pretty much a copy paste of Withor's LeechingSeedBuff
			Inventory inventory = damageReport.attackerBody.inventory;
			if (inventory)
			{
				int itemCount = inventory.GetItemCount(RoR2Content.Items.Seed);
				if (itemCount > 0)
				{
					float healing = BaseHeal + (damageReport.damageInfo.procCoefficient * ProcHeal);
					damageReport.attackerBody.healthComponent.Heal(healing * itemCount, damageReport.damageInfo.procChainMask, true);
				}
			}
		}
		private void IL_RemoveOldFunction(ILContext il)
		{
			//Stop the old code
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Seed")
			);
			ilcursor.Index += 2;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
		}
		private void IL_ModOldFunction(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 20),
				x => ILPatternMatchingExt.MatchLdloc(x, 19),
				x => ILPatternMatchingExt.MatchConvR4(x)
			);
			ilcursor.Index += 2;
			ilcursor.Remove();
			ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
			{
				return itemCount * ProcHeal;
			});
		}
	}
}
