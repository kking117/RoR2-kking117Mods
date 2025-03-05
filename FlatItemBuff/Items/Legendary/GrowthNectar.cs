using System;
using System.Linq;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class GrowthNectar
	{
		public static BuffDef ImperfectBuff;
		private static Color BuffColor = new Color(0.5f, 0.5f, 0.5f, 1f);
		private const string LogName = "Growth Nectar";
		internal static bool Enable = false;
		internal static float BaseBoost = 0.04f;
		internal static float StackBoost = 0.04f;
		internal static int BaseCap = 5;
		public GrowthNectar()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			CreateBuff();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseBoost = Math.Max(0f, BaseBoost);
			StackBoost = Math.Max(0f, StackBoost);
			BaseCap = Math.Max(0, BaseCap);
		}
		private void CreateBuff()
		{
			BuffDef BoostAllStatsBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC2/bdBoostAllStatsBuff.asset").WaitForCompletion();
			if (BoostAllStatsBuff)
            {
				BoostAllStatsBuff.ignoreGrowthNectar = false;
				ImperfectBuff = Utils.ContentManager.AddBuff("BoostAllStatsWeak", BoostAllStatsBuff.iconSprite, BuffColor, false, false, false, false, false);
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "Become marked for greatness, increasing ALL of your stats for each unique buff.";
			string description = "Become <style=cIsUtility>marked for greatness</style>,";
			if (BaseBoost > 0f)
			{
				description += string.Format(" increasing <style=cIsUtility>ALL stats</style> by <style=cIsUtility>{0}% <style=cStack>(+{1}% per stack)</style></style> for each unique buff", BaseBoost * 100f, StackBoost * 100f);
			}
			else
            {
				description += string.Format(" increasing <style=cIsUtility>ALL stats</style> by <style=cIsUtility>{0}%</style> for each unique buff", BaseBoost * 100f);
			}
			description += string.Format(" up to <style=cIsUtility>{0}% <style=cStack>(+{1}% per stack)</style></style>.", BaseCap * BaseBoost * 100f, BaseCap * StackBoost * 100f);
			LanguageAPI.Add("ITEM_BOOSTALLSTATS_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BOOSTALLSTATS_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_OnRecalculateStats);
		}
		private void IL_OnRecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);	
			if(ilcursor.TryGotoNext(
				x => x.MatchStfld(typeof(RoR2.CharacterBody).GetField("maxGrowthNectarBuffCount"))
			))
            {
				ilcursor.Index -= 3;
				ilcursor.RemoveRange(3);
				ilcursor.Emit(OpCodes.Ldc_I4, BaseCap);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnRecalculateStats A - IL Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcI4(0)
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.EmitDelegate<Func<CharacterBody, int>>((self) =>
				{
					if (!self.HasBuff(DLC2Content.Buffs.BoostAllStatsBuff))
					{
						self.AddBuff(DLC2Content.Buffs.BoostAllStatsBuff);
					}
					return 1;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnRecalculateStats B - IL Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcR4(0.04f),
				x => x.MatchLdarg(0),
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "BoostAllStatsBuff")
			))
			{
				ilcursor.Index += 1;
				ilcursor.RemoveRange(5);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 54);
				ilcursor.EmitDelegate<Func<float, CharacterBody, int, float>>((oldvalue, self, itemCount) =>
				{
					itemCount = Math.Max(0, itemCount - 1);
					float statPerBuff = BaseBoost + (StackBoost * itemCount);
					return statPerBuff * self.GetBuffCount(DLC2Content.Buffs.BoostAllStatsBuff);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnRecalculateStats C - IL Hook failed");
			}
		}
	}
}
