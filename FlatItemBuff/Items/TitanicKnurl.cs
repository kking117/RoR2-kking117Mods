using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;

namespace FlatItemBuff.Items
{
	public class TitanicKnurl
	{
		public TitanicKnurl()
		{
			MainPlugin.ModLogger.LogInfo("Changing Titanic Knurl");
			UpdateText();
			Hooks();
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("");
			string desc = string.Format("");
			if(MainPlugin.Knurl_BaseHP.Value > 0f)
            {
				pickup = string.Format("Boosts health");
				desc = string.Format("<style=cIsHealing>Increase maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>", MainPlugin.Knurl_BaseHP.Value);
				if (MainPlugin.Knurl_BaseRegen.Value <= 0f)
                {
					pickup += string.Format(".");
					desc += string.Format(".");
				}
			}
			if (MainPlugin.Knurl_BaseRegen.Value > 0f)
			{
				if(MainPlugin.Knurl_BaseHP.Value > 0f)
                {
					pickup += string.Format(" and regeneration.");
					desc += string.Format(" and <style=cIsHealing> base health regeneration</style> by <style=cIsHealing>+{0} hp/s <style=cStack>(+{0} per stack)</style></style>.", MainPlugin.Knurl_BaseRegen.Value);
				}
				else
                {
					pickup += string.Format("Boosts regeneration.");
					desc += string.Format("<style=cIsHealing>Increases base health regeneration</style> by <style=cIsHealing>+{0} hp/s <style=cStack>(+{0} per stack)</style></style>.", MainPlugin.Knurl_BaseRegen.Value);
				}
			}
			LanguageAPI.Add("ITEM_KNURL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNURL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "Knurl"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount")
			);
			ilcursor.Index -= 2;
			ilcursor.RemoveRange(5);
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
				if (itemCount > 0)
				{
					float levelBonus = sender.level - 1f;
					if (MainPlugin.Knurl_BaseRegen.Value>0f)
					{
						args.baseRegenAdd += itemCount * (MainPlugin.Knurl_BaseRegen.Value + (levelBonus * MainPlugin.Knurl_LevelRegen.Value));
					}
					if (MainPlugin.Knurl_BaseHP.Value > 0f)
					{
						args.baseHealthAdd += itemCount * (MainPlugin.Knurl_BaseHP.Value + (levelBonus * MainPlugin.Knurl_LevelHP.Value));
					}
				}
			}
		}
	}
}
