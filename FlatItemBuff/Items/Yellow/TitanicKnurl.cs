using System;
using RoR2;
using R2API;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class TitanicKnurl
	{
		internal static bool Enable = true;
		internal static float BaseHP = 30f;
		internal static float LevelHP = 9f;
		internal static float BaseRegen = 1.6f;
		internal static float LevelRegen = 0.32f;
		public TitanicKnurl()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Titanic Knurl");
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseHP = Math.Max(0f, BaseHP);
			LevelHP = Math.Max(0f, LevelHP);
			BaseRegen = Math.Max(0f, BaseRegen);
			LevelRegen = Math.Max(0f, LevelRegen);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("");
			string desc = string.Format("");
			if(BaseHP > 0f)
            {
				pickup = string.Format("Boosts health");
				desc = string.Format("<style=cIsHealing>Increase maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>", BaseHP);
				if (BaseRegen <= 0f)
                {
					pickup += string.Format(".");
					desc += string.Format(".");
				}
			}
			if (BaseRegen > 0f)
			{
				if(BaseHP > 0f)
                {
					pickup += string.Format(" and regeneration.");
					desc += string.Format(" and <style=cIsHealing> base health regeneration</style> by <style=cIsHealing>+{0} hp/s <style=cStack>(+{0} per stack)</style></style>.", BaseRegen);
				}
				else
                {
					pickup += string.Format("Boosts regeneration.");
					desc += string.Format("<style=cIsHealing>Increases base health regeneration</style> by <style=cIsHealing>+{0} hp/s <style=cStack>(+{0} per stack)</style></style>.", BaseRegen);
				}
			}
			LanguageAPI.Add("ITEM_KNURL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNURL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if(ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Knurl"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
			{
				ilcursor.Index -= 2;
				ilcursor.RemoveRange(5);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Titanic Knurl - Effect Override - IL Hook failed");
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int itemCount = inventory.GetItemCount(RoR2Content.Items.Knurl);
			if (itemCount > 0)
			{
				float levelBonus = sender.level - 1f;
				if (BaseRegen > 0f)
				{
					args.baseRegenAdd += itemCount * (BaseRegen + (levelBonus * LevelRegen));
				}
				if (BaseHP > 0f)
				{
					args.baseHealthAdd += itemCount * (BaseHP + (levelBonus * LevelHP));
				}
			}
		}
	}
}
