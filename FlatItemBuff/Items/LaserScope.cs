using System;
using RoR2;
using R2API;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class LaserScope
	{
		public LaserScope()
		{
			if (MainPlugin.LaserScope_Crit.Value != 0f)
            {
				MainPlugin.ModLogger.LogInfo("Changing Laser Scope");
				UpdateText();
				Hooks();
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "";
			desc += String.Format("Gain <style=cIsDamage>{0}% critical chance</style>.", MainPlugin.LaserScope_Crit.Value);
			desc += " <style=cIsDamage>Critical Strikes</style> deal an additional <style=cIsDamage>100% damage</style><style=cStack> (+100% per stack)</style>.";
			LanguageAPI.Add("ITEM_CRITDAMAGE_DESC", desc);
		}
		private void Hooks()
		{
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(DLC1Content.Items.CritDamage);
				if (itemCount > 0)
				{
					args.critAdd += MainPlugin.LaserScope_Crit.Value;
				}
			}
		}
	}
}
