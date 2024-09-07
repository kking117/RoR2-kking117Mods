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
		internal static float BaseHP = 10f;
		internal static float LevelHP = 3f;
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
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			pickup += "Gain max health.";
			if (LevelHP > 0f)
            {
				desc += string.Format("Increases <style=cIsHealing>base health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>. <style=cStack>Health increases further with level</style>.", BaseHP);
			}
			else
            {
				desc += string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", BaseHP);
			}
			LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", pickup);
			LanguageAPI.Add("ITEM_FLATHEALTH_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(70),
				x => x.MatchLdloc(36),
				x => x.MatchConvR4()
			))
            {
				ilcursor.Index += 3;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((body) =>
				{
					float result = BaseHP;
					if (LevelHP > 0f)
					{
						result += (body.level - 1f) * LevelHP;
					}
					return result;
				});
			}
			else
            {
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Bison Steak - Health Stat - IL Hook failed");
			}
		}
	}
}
