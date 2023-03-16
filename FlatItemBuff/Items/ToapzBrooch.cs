using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class TopazBrooch
	{
		internal static bool Enable = true;
		internal static float BaseFlatBarrier = 15.0f;
		internal static float StackFlatBarrier = 15.0f;
		internal static float BaseCentBarrier = 0.005f;
		internal static float StackCentBarrier = 0.005f;
		public TopazBrooch()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Topaz Brooch");
			UpdateText();
			Hooks();
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "Gain a <style=cIsHealing>temporary barrier</style> on kill for ";
			if (BaseFlatBarrier > 0f || StackFlatBarrier > 0f)
			{
				desc += string.Format("<style=cIsHealing>{0} health <style=cStack>(+{1} per stack)</style></style>", BaseFlatBarrier, StackFlatBarrier);
			}
			if (BaseCentBarrier > 0f || StackCentBarrier > 0f)
			{
				if (BaseFlatBarrier > 0f || StackCentBarrier > 0f)
				{
					desc += " plus an additional ";
				}
				desc += string.Format("<style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style></style> of <style=cIsHealing>maximum health</style>", BaseCentBarrier * 100f, StackCentBarrier * 100f);
			}
			desc += ".";
			LanguageAPI.Add("ITEM_BARRIERONKILL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 15),
				x => ILPatternMatchingExt.MatchLdloc(x, 49),
				x => ILPatternMatchingExt.MatchConvR4(x),
				x => ILPatternMatchingExt.MatchMul(x)
			);
			ilcursor.RemoveRange(4);
			ilcursor.Emit(OpCodes.Ldarg_1);
			ilcursor.Emit(OpCodes.Ldloc, 49);
			ilcursor.EmitDelegate<Func<DamageReport, int, float>>((dr, itemCount) =>
			{
				itemCount--;
				float basebarrier = BaseFlatBarrier;
				float stackbarrier = StackFlatBarrier;
				if (dr.attackerBody.healthComponent)
				{
					basebarrier += dr.attackerBody.healthComponent.fullCombinedHealth * BaseCentBarrier;
					stackbarrier += dr.attackerBody.healthComponent.fullCombinedHealth * StackCentBarrier;
				}
				stackbarrier *= itemCount;
				return basebarrier + stackbarrier;
			});
		}
	}
}
