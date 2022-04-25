using R2API;

namespace QueenGlandBuff.Changes
{
    public class QueensGland
    {
		public static void Begin()
        {
			BeetleGuardAlly.Begin();
			UpdateItemDescription();
			QueensGlandHooks.Begin();
		}
		private static void UpdateItemDescription()
		{
			if (MainPlugin.Gland_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing Queen's Gland descriptions.");
			}
			string pickup = "Recruit ";
			string desc = "<style=cIsUtility>Summon ";
			if (MainPlugin.Gland_SpawnAffix.Value == 1)
			{
				pickup += "an Elite Beetle Guard.";
				desc += "an Elite Beetle Guard</style>";
			}
			else
			{
				pickup += "a Beetle Guard.";
				desc += "a Beetle Guard</style>";
			}
			desc += " with <style=cIsHealing>" + (10 + MainPlugin.Gland_BaseHealth.Value) * 10 + "% health</style>";
			desc += " and <style=cIsDamage>" + (10 + MainPlugin.Gland_BaseDamage.Value) * 10 + "% damage</style>.";
			desc += " Can have up to <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> total Guards, up to <style=cIsUtility>" + MainPlugin.Gland_MaxSummons.Value + "</style>. Further stacks give";
			desc += " <style=cStack>+" + MainPlugin.Gland_StackHealth.Value * 10 + "%</style> <style=cIsHealing>health</style>";
			desc += " and <style=cStack>+" + MainPlugin.Gland_StackDamage.Value * 10 + "%</style> <style=cIsDamage>damage</style>.";
			LanguageAPI.Add("ITEM_BEETLEGLAND_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BEETLEGLAND_DESC", desc);
		}
	}
}