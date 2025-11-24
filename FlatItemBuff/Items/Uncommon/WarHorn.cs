using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class WarHorn
	{
		private const string LogName = "War Horn";
		internal static bool Enable = false;
		internal static float BaseDuration = 6f;
		internal static float StackDuration = 3f;
		internal static float BaseAttack = 0.6f;
		internal static float StackAttack = 0.15f;
		public WarHorn()
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
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			BaseAttack = Math.Max(0f, BaseAttack);
			StackAttack = Math.Max(0f, StackAttack);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string desc = "Activating your Equipment gives you ";
			desc += string.Format("<style=cIsDamage>+{0}% ", BaseAttack * 100f);
			if (StackAttack > 0f)
            {
				desc += string.Format("<style=cStack>(+{0}% per stack)</style> ", StackAttack * 100f);
			}
			desc += string.Format("attack speed</style> for <style=cIsDamage>{0}s</style> ", BaseDuration);
			if (StackDuration > 0f)
			{
				desc += string.Format("<style=cStack>(+{0}s per stack)</style>", StackDuration);
			}
			desc += ".";
			LanguageAPI.Add("ITEM_ENERGIZEDONEQUIPMENTUSE_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.EquipmentSlot.OnEquipmentExecuted_byte_byte_EquipmentIndex += new ILContext.Manipulator(IL_OnEquipmentExecuted);
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Energized"),
				x => x.MatchCallOrCallvirt(typeof(CharacterBody), "HasBuff")
			))
			{
				ilcursor.Index += 4;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((body) =>
				{
					int itemCount = Math.Max(0, body.inventory.GetItemCountEffective(RoR2Content.Items.EnergizedOnEquipmentUse) - 1);
					return BaseAttack + (itemCount * StackAttack);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats - Hook failed");
			}
		}
		private void IL_OnEquipmentExecuted(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Energized"),
				x => x.MatchLdcI4(8),
				x => x.MatchLdcI4(4)
			))
			{
				ilcursor.Index += 1;
				ilcursor.RemoveRange(8);
				ilcursor.Emit(OpCodes.Ldloc_0);
				ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
				{
					return BaseDuration + (StackDuration * (itemCount - 1));
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnEquipmentExecuted - Hook failed");
			}
		}
	}
}
