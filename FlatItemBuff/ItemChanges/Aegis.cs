using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class Aegis
	{
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Aegis");
			if (MainPlugin.Aegis_Armor.Value != 0f)
            {
				UpdateText();
			}
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Healing past full grants you a temporary barrier. Reduces damage taken.";
			string desc = String.Format("Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>50% <style=cStack>(+50% per stack)</style></style> of the amount you <style=cIsHealing>healed</style>. <style=cIsHealing>Increases armor</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", MainPlugin.Aegis_Armor.Value);
			LanguageAPI.Add("ITEM_BARRIERONOVERHEAL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BARRIERONOVERHEAL_DESC", desc);
		}
		private static void Hooks()
		{
			if (MainPlugin.Aegis_Regen.Value)
			{
				MainPlugin.ModLogger.LogInfo("Applying IL modifications");
				IL.RoR2.HealthComponent.Heal += new ILContext.Manipulator(IL_Heal);
			}
			if(MainPlugin.Aegis_Armor.Value != 0f)
            {
				RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			}
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
				if (itemCount > 0)
				{
					args.armorAdd += itemCount * MainPlugin.Aegis_Armor.Value;
				}
			}
		}
		private static void IL_Heal(ILContext il)
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
