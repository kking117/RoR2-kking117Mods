using System;
using RoR2;
using R2API;

namespace FlatItemBuff.Items
{
	public class LaserScope
	{
		internal static bool Enable = true;
		internal static float BaseCrit = 5f;
		public LaserScope()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Laser Scope");
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseCrit = Math.Max(0f, BaseCrit);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "";
			desc += String.Format("Gain <style=cIsDamage>{0}% critical chance</style>.", BaseCrit);
			desc += " <style=cIsDamage>Critical strikes</style> deal an additional <style=cIsDamage>100% damage</style><style=cStack> (+100% per stack)</style>.";
			LanguageAPI.Add("ITEM_CRITDAMAGE_DESC", desc);
		}
		private void Hooks()
		{
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
			int itemCount = inventory.GetItemCount(DLC1Content.Items.CritDamage);
			if (itemCount > 0)
			{
				args.critAdd += BaseCrit;
			}
		}
	}
}
