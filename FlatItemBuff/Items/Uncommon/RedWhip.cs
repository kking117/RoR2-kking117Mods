using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class RedWhip
	{
		private const string LogName = "Red Whip";
		internal static bool Enable = false;
		internal static float BaseSpeed = 0.1f;
		internal static float StackSpeed = 0.1f;
		internal static float BaseActiveSpeed = 0.2f;
		internal static float StackActiveSpeed = 0.2f;
		public RedWhip()
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
			BaseSpeed = Math.Max(0f, BaseSpeed);
			StackSpeed = Math.Max(0f, StackSpeed);
			BaseActiveSpeed = Math.Max(0f, BaseActiveSpeed);
			StackActiveSpeed = Math.Max(0f, StackActiveSpeed);
		}
		private void UpdateText()
		{
			string pickup = "";
			string description = "Increases <style=cIsUtility>movement speed</style>";
			if (BaseSpeed > 0f)
            {
				pickup += "Increases movement speed";
				if (StackSpeed > 0f)
                {
					description += string.Format(" by <style=cIsUtility>{0}%</style> <style=cStack>(+{1}% per stack)</style>", BaseSpeed * 100f, StackSpeed * 100f);
				}
				else
                {
					description += string.Format(" by <style=cIsUtility>{0}%</style>", BaseSpeed * 100f);
				}
				if (BaseActiveSpeed > 0f)
                {
					description += ", further increased";
				}
				else
                {
					description += ".";
                }
			}
			if (BaseActiveSpeed > 0f)
            {
				if (BaseSpeed > 0f)
                {
					pickup += ", further increased while out of combat.";
				}
				else
                {
					pickup += "Increases movement speed while out of combat.";
				}
				if (StackSpeed > 0f)
				{
					description += string.Format(" by <style=cIsUtility>{0}%</style> <style=cStack>(+{1}% per stack)</style> while out of combat.", BaseActiveSpeed * 100f, StackActiveSpeed * 100f);
				}
				else
				{
					description += string.Format(" by <style=cIsUtility>{0}%</style> while out of combat.", BaseActiveSpeed * 100f);
				}
			}
			LanguageAPI.Add("ITEM_SPRINTOUTOFCOMBAT_PICKUP", pickup);
			LanguageAPI.Add("ITEM_SPRINTOUTOFCOMBAT_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_OnRecalculateStats);
			if (BaseSpeed > 0f)
            {
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
			int itemCount = inventory.GetItemCountEffective(RoR2Content.Items.SprintOutOfCombat);
			if (itemCount > 0)
			{
				args.moveSpeedMultAdd += BaseSpeed + (Math.Max(0, itemCount - 1) * StackSpeed);
			}
		}

		private void IL_OnRecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "WhipBoost")
			))
			{
				ilcursor.Index += 4;
				ilcursor.RemoveRange(4);
				ilcursor.Emit(OpCodes.Ldloc, 7);
				ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
				{
					return BaseActiveSpeed + (Math.Max(0, itemCount - 1) * StackActiveSpeed);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnRecalculateStats - Hook failed");
			}
		}
	}
}
