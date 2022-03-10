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
		}
		private static void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 15),
				x => ILPatternMatchingExt.MatchLdloc(x, 43),
				x => ILPatternMatchingExt.MatchConvR4(x),
				x => ILPatternMatchingExt.MatchMul(x)
			);
			ilcursor.RemoveRange(4);
			ilcursor.Emit(OpCodes.Ldarg_1);
			ilcursor.Emit(OpCodes.Ldloc, 43);
			ilcursor.EmitDelegate<Func<DamageReport, int, float>>((dr, itemCount) =>
			{
				itemCount--;
				float basebarrier = MainPlugin.Brooch_BaseFlatBarrier.Value;
				float stackbarrier = MainPlugin.Brooch_StackFlatBarrier.Value;
				if (dr.attackerBody.healthComponent)
				{
					basebarrier += dr.attackerBody.healthComponent.fullCombinedHealth * MainPlugin.Brooch_BaseCentBarrier.Value;
					stackbarrier += dr.attackerBody.healthComponent.fullCombinedHealth * MainPlugin.Brooch_StackCentBarrier.Value;
				}
				stackbarrier *= itemCount;
				return basebarrier + stackbarrier;
			});
		}
	}
}
