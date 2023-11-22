using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class BisonSteak
	{
		internal static bool Enable = true;
		internal static float BaseHP = 20f;
		internal static float LevelHP = 2f;
		internal static float BaseDuration = 3f;
		internal static float StackDuration = 3f;
		public BisonSteak()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Bison Steak");
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseHP = Math.Max(0f, BaseHP);
			LevelHP = Math.Max(0f, LevelHP);
			BaseDuration = Math.Max(0, BaseDuration);
			StackDuration = Math.Max(0, StackDuration);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			bool DoASpace = false;
			if (BaseHP > 0f)
            {
				pickup += "Gain max health.";
				desc += string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", BaseHP);
				DoASpace = true;
			}
			if (BaseDuration > 0f)
			{
				if(DoASpace)
                {
					pickup += " ";
					desc += " ";
				}
				pickup += "Regenerate health after killing an enemy.";
				desc += string.Format("Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+2 hp/s</style> for <style=cIsUtility>{0}s</style>", BaseDuration);
				if (StackDuration > 0)
                {
					desc += string.Format(" <style=cStack>(+{0}s per stack)</style>", StackDuration);
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
			if (BaseDuration > 0f)
            {
				SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterBody attacker = damageReport.attackerBody;
			if (attacker.inventory)
			{
				int itemCount = attacker.inventory.GetItemCount(ItemCatalog.FindItemIndex("FlatHealth"));
				if (itemCount > 0)
				{
					float duration = BaseDuration;
					duration += (itemCount - 1) * StackDuration;
					if (duration > 0f)
					{
						attacker.AddTimedBuff(JunkContent.Buffs.MeatRegenBoost, duration);
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
				if(BaseHP > 0f)
                {
					return BaseHP + ((bs.level - 1f) * LevelHP);
				}
				return 0f;
			});
		}
	}
}
