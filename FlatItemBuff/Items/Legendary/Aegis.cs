using System;
using RoR2;
using R2API;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class Aegis
	{
		internal static bool Enable = true;
		internal static bool AllowRegen = true;
		internal static float Armor = 20f;
		public Aegis()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Aegis");
			ClampConfig();
			if (Armor != 0f)
            {
				UpdateText();
			}
			Hooks();
		}
		private void ClampConfig()
		{
			Armor = Math.Max(0f, Armor);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Healing past full grants you a temporary barrier. Reduces damage taken.";
			string desc = String.Format("Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>50% <style=cStack>(+50% per stack)</style></style> of the amount you <style=cIsHealing>healed</style>. <style=cIsHealing>Increases armor</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", Armor);
			LanguageAPI.Add("ITEM_BARRIERONOVERHEAL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BARRIERONOVERHEAL_DESC", desc);
		}
		private void Hooks()
		{
			if (AllowRegen)
			{
				MainPlugin.ModLogger.LogInfo("Applying IL modifications");
				IL.RoR2.HealthComponent.Heal += new ILContext.Manipulator(IL_Heal);
			}
			if(Armor != 0f)
            {
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int itemCount = inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
			if (itemCount > 0)
			{
				args.armorAdd += itemCount * Armor;
			}
		}
		private void IL_Heal(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 2),
				x => ILPatternMatchingExt.MatchLdcR4(x, 0.0f)
			);
			if(ilcursor.Index > 0)
            {
				ilcursor.Index += 3;
				ilcursor.RemoveRange(2);
			}
		}
	}
}
