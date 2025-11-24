using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class SearedSteak
	{
		private const string LogName = "Seared Steak";
		internal static bool Enable = false;
		internal static float BaseHP = 20f;
		internal static float LevelHP = 6f;
		internal static float BasePercentHP = 0.05f;
		public SearedSteak()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseHP = Math.Max(0f, BaseHP);
			LevelHP = Math.Max(0f, LevelHP);
			BasePercentHP = Math.Max(0f, BasePercentHP);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "";
			string desc = "";
			pickup += "Gain greatly increased max health. Cooked to perfection.";
			if (LevelHP > 0f)
            {
				desc += string.Format("Increases <style=cIsHealing>base health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>", BaseHP);
			}
			else
            {
				desc += string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>", BaseHP);
			}
			if (BasePercentHP > 0f)
            {
				if (LevelHP > 0f)
				{
					desc += string.Format(", plus an additional <style=cIsHealing>{0}% <style=cStack>(+{0}% per stack)</style></style> of your <style=cIsHealing>max health</style>.", BasePercentHP * 100f);
				}
				else
                {
					desc += string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}% <style=cStack>(+{0}% per stack)</style></style>.", BasePercentHP * 100f);
				}	
			}
			else
            {
				desc += ".";
			}
			LanguageAPI.Add("ITEM_COOKEDSTEAK_PICKUP", pickup);
			LanguageAPI.Add("ITEM_COOKEDSTEAK_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(93),
				x => x.MatchLdloc(57),
				x => x.MatchConvR4()
			))
			{
				ilcursor.Index += 3;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((body) =>
				{
					return BasePercentHP;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats A - Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(91),
				x => x.MatchLdloc(57),
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
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats B - Hook failed");
			}
		}
	}
}
