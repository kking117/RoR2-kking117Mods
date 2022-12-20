using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class BisonSteak
	{
		public BisonSteak()
		{
			MainPlugin.ModLogger.LogInfo("Changing Bison Steak");
			UpdateText();
			Hooks();
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			bool DoASpace = false;
			if (MainPlugin.Steak_BaseHP.Value > 0f)
            {
				pickup += string.Format("Gain <style=cIsHealing>{0}</style> max health.", MainPlugin.Steak_BaseHP.Value);
				desc += string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", MainPlugin.Steak_BaseHP.Value);
				DoASpace = true;
			}
			if (MainPlugin.Steak_BaseBuffDur.Value > 0f)
			{
				if(DoASpace)
                {
					DoASpace = false;
					pickup += " ";
					desc += " ";
				}
				pickup += "<style=cIsHealing>Regenerate health</style> after killing an enemy.";
				desc += string.Format("Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+2 hp/s</style> for <style=cIsUtility>{0}s</style>", MainPlugin.Steak_BaseBuffDur.Value);
				if (MainPlugin.Steak_StackBuffDur.Value > 0)
                {
					desc += string.Format(" <style=cStack>(+{0}s per stack)</style>", MainPlugin.Steak_StackBuffDur.Value);
				}
				desc += " after killing an enemy.";
			}
			LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", pickup);
			LanguageAPI.Add("ITEM_FLATHEALTH_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			if (MainPlugin.Steak_BaseBuffDur.Value > 0f)
            {
				GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			}
		}
		private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageReport.attacker && damageReport.attackerBody)
            {
				CharacterBody attacker = damageReport.attackerBody;
				if (attacker.inventory)
				{
					int itemCount = attacker.inventory.GetItemCount(ItemCatalog.FindItemIndex("FlatHealth"));
					if (itemCount > 0)
					{
						float duration = MainPlugin.Steak_BaseBuffDur.Value;
						duration += (itemCount-1) * MainPlugin.Steak_StackBuffDur.Value;
						if (duration > 0f)
						{
							attacker.AddTimedBuff(JunkContent.Buffs.MeatRegenBoost, duration);
						}
					}
				}
            }
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "FlatHealth"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, 36)
			);
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 36)
			});
			ilcursor.Index += 2;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((bs) =>
			{
				if(MainPlugin.Steak_BaseHP.Value > 0f)
                {
					return MainPlugin.Steak_BaseHP.Value + ((bs.level - 1f) * MainPlugin.Steak_LevelHP.Value);
				}
				return 0f;
			});
		}
	}
}
