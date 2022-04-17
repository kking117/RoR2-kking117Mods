using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class WarHorn
	{
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing War Horn");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "Activating your Equipment gives you ";
			desc += string.Format("<style=cIsDamage>+{0}% ", MainPlugin.WarHorn_BaseSpeed.Value * 100f);
			if (MainPlugin.WarHorn_StackSpeed.Value > 0f)
            {
				desc += string.Format("<style=cStack>(+{0}% per stack)</style> ", MainPlugin.WarHorn_StackSpeed.Value * 100f);
			}
			desc += string.Format("attack speed</style> for <style=cIsDamage>{0}s</style> ", MainPlugin.WarHorn_BaseDuration.Value);
			if (MainPlugin.WarHorn_StackDuration.Value > 0f)
			{
				desc += string.Format("<style=cStack>(+{0}s per stack)</style>", MainPlugin.WarHorn_StackDuration.Value);
			}
			desc += ".";
			LanguageAPI.Add("ITEM_ENERGIZEDONEQUIPMENTUSE_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.EquipmentSlot.OnEquipmentExecuted += new ILContext.Manipulator(IL_OnEquipmentExecuted);
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Buffs", "Energized"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "HasBuff")
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Index += 4;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((body) =>
				{
					int itemCount = Math.Max(0, body.inventory.GetItemCount(RoR2Content.Items.EnergizedOnEquipmentUse)-1);
					return MainPlugin.WarHorn_BaseSpeed.Value + (itemCount * MainPlugin.WarHorn_StackSpeed.Value);
				});
			}
		}
		private static void IL_OnEquipmentExecuted(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Buffs", "Energized"),
				x => ILPatternMatchingExt.MatchLdcI4(x, 8),
				x => ILPatternMatchingExt.MatchLdcI4(x, 4)
			);
			if(ilcursor.Index > 0)
            {
				ilcursor.Index += 1;
				ilcursor.RemoveRange(8);
				ilcursor.Emit(OpCodes.Ldloc_1);
				ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
				{
					return MainPlugin.WarHorn_BaseDuration.Value + (MainPlugin.WarHorn_StackDuration.Value * (itemCount - 1));
				});
			}
		}
	}
}
