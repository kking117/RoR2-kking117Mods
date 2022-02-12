using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.ItemChanges
{
	public class TopazBrooch
	{
		private static string IL_ItemName = "BarrierOnKill";
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Topaz Brooch");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "Gain a <style=cIsHealing>temporary barrier</style> on kill for ";
			if (MainPlugin.Brooch_BaseFlatBarrier.Value > 0f || MainPlugin.Brooch_StackFlatBarrier.Value > 0f)
			{
				desc += string.Format("<style=cIsHealing>{0} health <style=cStack>(+{1} per stack)</style></style>", MainPlugin.Brooch_BaseFlatBarrier.Value, MainPlugin.Brooch_StackFlatBarrier.Value);
			}
			if (MainPlugin.Brooch_BaseCentBarrier.Value > 0f || MainPlugin.Brooch_StackCentBarrier.Value > 0f)
			{
				if (MainPlugin.Brooch_BaseFlatBarrier.Value > 0f || MainPlugin.Brooch_StackCentBarrier.Value > 0f)
				{
					desc += " plus an additional ";
				}
				desc += string.Format("<style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style></style> of <style=cIsHealing>maximum health</style>", MainPlugin.Brooch_BaseCentBarrier.Value * 100f, MainPlugin.Brooch_StackCentBarrier.Value * 100f);
			}
			desc += ".";
			LanguageAPI.Add("ITEM_BARRIERONKILL_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
		}
		private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			CharacterBody attacker = damageReport.attackerBody;
			if (attacker)
			{
				Inventory inventory = attacker.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(RoR2.RoR2Content.Items.BarrierOnKill);
					if (itemCount > 0)
					{
						itemCount--;
						float barrier = attacker.healthComponent.fullCombinedHealth * (MainPlugin.Brooch_BaseCentBarrier.Value + (MainPlugin.Brooch_StackCentBarrier.Value * itemCount));
						barrier += MainPlugin.Brooch_BaseFlatBarrier.Value + (MainPlugin.Brooch_StackFlatBarrier.Value * itemCount);
						attacker.healthComponent.AddBarrier(barrier);
					}
				}
			}
		}
		private static void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 15),
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", IL_ItemName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<RoR2.Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, 40),
				x => ILPatternMatchingExt.MatchLdloc(x, 40)
			);
			ilcursor.Index += 4;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldc_I4, 0);
		}
	}
}
