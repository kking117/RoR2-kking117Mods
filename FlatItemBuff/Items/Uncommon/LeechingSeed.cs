using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class LeechingSeed
	{
		private const string LogName = "Leeching Seed";
		internal static bool Enable = false;
		internal static float ProcHeal = 1f;
		internal static float BaseHeal = 1f;
		public LeechingSeed()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
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
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string desc = string.Format("Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>{0} <style=cStack>(+{0} per stack)</style> health</style>.", ProcHeal + BaseHeal);
			LanguageAPI.Add("ITEM_SEED_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += new ILContext.Manipulator(IL_RemoveOldFunction);
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
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RemoveOldFunction - Hook failed");
			}
		}
	}
}
