using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class RoseBuckler
	{
		private const string LogName = "Rose Buckler";
		internal static bool Enable = false;
		internal static float BaseArmor = 10f;
		internal static float StackArmor = 10f;
		internal static float BaseActiveArmor = 20f;
		internal static float StackActiveArmor = 20f;
		public RoseBuckler()
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
			BaseArmor = Math.Max(0f, BaseArmor);
			StackArmor = Math.Max(0f, StackArmor);
			BaseActiveArmor = Math.Max(0f, BaseActiveArmor);
			StackActiveArmor = Math.Max(0f, StackActiveArmor);
		}
		private void UpdateText()
		{
			string pickup = "";
			string description = "Increases <style=cIsHealing>armor</style>";
			if (BaseArmor > 0f)
            {
				pickup += "Increases armor";
				if (StackArmor > 0f)
                {
					description += string.Format(" by <style=cIsHealing>{0}</style> <style=cStack>(+{1} per stack)</style>", BaseArmor, StackArmor);
				}
				else
                {
					description += string.Format(" by <style=cIsHealing>{0}</style>", BaseArmor);
				}
				if (BaseActiveArmor > 0f)
                {
					description += ", further increased";
				}
				else
                {
					description += ".";
                }
			}
			if (BaseActiveArmor > 0f)
            {
				if (BaseArmor > 0f)
                {
					pickup += ", further increased while sprinting.";
				}
				else
                {
					pickup += "Increases armor while sprinting.";
				}
				if (StackArmor > 0f)
				{
					description += string.Format(" by <style=cIsHealing>{0}</style> <style=cStack>(+{1} per stack)</style> <style=cIsUtility>while sprinting</style>.", BaseActiveArmor, StackActiveArmor);
				}
				else
				{
					description += string.Format(" by <style=cIsHealing>{0}</style> <style=cIsUtility>while sprinting</style>.", BaseActiveArmor);
				}
			}
			LanguageAPI.Add("ITEM_SPRINTARMOR_PICKUP", pickup);
			LanguageAPI.Add("ITEM_SPRINTARMOR_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_OnRecalculateStats);
			if (BaseArmor > 0f)
            {
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
			int itemCount = inventory.GetItemCountEffective(RoR2Content.Items.SprintArmor);
			if (itemCount > 0)
			{
				args.armorAdd += BaseArmor + (Math.Max(0, itemCount - 1) * StackArmor);
			}
		}

		private void IL_OnRecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(23),
				x => x.MatchLdcI4(30)
			))
			{
				ilcursor.Index += 1;
				ilcursor.RemoveRange(3);
				ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
				{
					return BaseActiveArmor + (Math.Max(0, itemCount - 1) * StackActiveArmor);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnRecalculateStats - Hook failed");
			}
		}
	}
}
