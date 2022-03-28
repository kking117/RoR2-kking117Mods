using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.ItemChanges
{
	public class LeechingSeed
	{
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Leeching Seed");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "";
			if (MainPlugin.LeechingSeed_ProcHeal.Value > 0f)
			{
				desc += string.Format("Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>{0} <style=cStack>(+{0} per stack)</style> health</style>.", MainPlugin.LeechingSeed_ProcHeal.Value + MainPlugin.LeechingSeed_NoProcHeal.Value);
			}
			LanguageAPI.Add("ITEM_SEED_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			if (MainPlugin.LeechingSeed_ProcHeal.Value > 0f)
			{
				IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			}
			if (MainPlugin.LeechingSeed_NoProcHeal.Value > 0f)
			{
				GlobalEventManager.onServerDamageDealt += Global_DamageDealt;
			}
		}
		private static void Global_DamageDealt(DamageReport damageReport)
		{
			//This is pretty much a copy paste of Withor's LeechingSeedBuff
			if(damageReport.attacker && damageReport.attackerBody)
            {
				ProcChainMask procChainMask = damageReport.damageInfo.procChainMask;
				Inventory inventory = damageReport.attackerBody.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(RoR2Content.Items.Seed);
					if (itemCount > 0)
					{
						damageReport.attackerBody.healthComponent.Heal(MainPlugin.LeechingSeed_NoProcHeal.Value * itemCount, procChainMask, true);
					}
				}
			}
		}
		private static void IL_OnHitEnemy(ILContext il)
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
				return itemCount * MainPlugin.LeechingSeed_ProcHeal.Value;
			});
		}
	}
}
