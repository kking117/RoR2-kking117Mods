using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.ItemChanges
{
	public class BisonSteak
	{
		private static string IL_ItemName = "FlatHealth";
		private static int IL_LocationOffset = 2;
		private static int IL_Location = 35;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Bison Steak");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("Gain <style=cIsHealing>{0}</style> max health.", MainPlugin.Steak_BaseHP.Value);
			string desc = string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", MainPlugin.Steak_BaseHP.Value);
			if (MainPlugin.Steak_OnKillDur.Value > 0f)
			{
				pickup += " <style=cIsHealing>Regenerate health</style> after killing an enemy.";
				desc += string.Format(" Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+2 hp/s</style> for <style=cIsUtility>{1}s</style> <style=cStack>(+{1}s per stack)</style> after killing an enemy.", MainPlugin.Steak_BaseHP.Value, MainPlugin.Steak_OnKillDur.Value);
			}
			LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", pickup);
			LanguageAPI.Add("ITEM_FLATHEALTH_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			if (MainPlugin.Steak_OnKillDur.Value > 0f)
            {
				GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			}
		}
		private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageReport.attacker && damageReport.attackerBody)
            {
				CharacterBody attacker = damageReport.attackerBody;
				int itemCount = attacker.inventory.GetItemCount(ItemCatalog.FindItemIndex("FlatHealth"));
				if(itemCount > 0)
                {
					attacker.AddTimedBuff(RoR2Content.Buffs.MeatRegenBoost, 3f * itemCount);
				}
            }
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", IL_ItemName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<RoR2.Inventory>(x, "GetItemCount"),
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
				return MainPlugin.Steak_BaseHP.Value + ((bs.level - 1f) * MainPlugin.Steak_LevelHP.Value);
			});
		}
	}
}
