using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;

namespace FlatItemBuff.ItemChanges
{
	public class TitanicKnurl
	{
		private static string IL_ItemName = "Knurl";
		private static int IL_LocationOffset = 2;
		private static int IL_Location = 16;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Titanic Knurl");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
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
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
		}
		private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, RoR2.CharacterBody self)
		{
			orig(self);
			bool active = NetworkServer.active;
			if(NetworkServer.active)
            {
				self.AddItemBehavior<TitanicKnurl_Behavior>(self.inventory.GetItemCount(RoR2Content.Items.Knurl));
			}
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", IL_ItemName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, IL_Location)
			);
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, IL_Location)
			});
			ilcursor.Index += IL_LocationOffset;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((bs) =>
			{
				if (MainPlugin.Knurl_BaseHP.Value <= 0f)
                {
					return 0f;
                }
				return MainPlugin.Knurl_BaseHP.Value + ((bs.level - 1f) * MainPlugin.Knurl_LevelHP.Value);
			});
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, IL_Location)
			});
			ilcursor.Index += IL_LocationOffset;
			ilcursor.RemoveRange(3);
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((bs) =>
			{
				if (MainPlugin.Knurl_BaseRegen.Value <= 0f)
                {
					return 0f;
                }
				return MainPlugin.Knurl_BaseRegen.Value + ((bs.level - 1f) * MainPlugin.Knurl_LevelRegen.Value);
			});
			
		}
	}
}
