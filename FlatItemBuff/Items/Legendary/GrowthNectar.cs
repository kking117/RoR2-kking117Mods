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
		internal static bool Enable = true;
		internal static float BaseBoost = 0.04f;
		internal static float StackBoost = 0.04f;
		internal static int BaseCap = 5;
		internal static int VFXCap = 5;
		public GrowthNectar()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo("Changing Growth Nectar");
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
			VFXCap = Math.Max(1, VFXCap);
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
			MainPlugin.ModLogger.LogInfo("Updating item text");
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
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCount = self.inventory.GetItemCount(DLC2Content.Items.BoostAllStats);
			if (itemCount < 1)
            {
				if (self.HasBuff(DLC2Content.Buffs.BoostAllStatsBuff))
				{
					self.RemoveBuff(DLC2Content.Buffs.BoostAllStatsBuff);
				}
				if (self.HasBuff(ImperfectBuff))
				{
					self.RemoveBuff(ImperfectBuff);
				}
			}
		}
		private void IL_OnRecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);	
			if(ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "BoostAllStatsBuff")
			))
            {
				ilcursor.RemoveRange(2);
				ilcursor.Emit(OpCodes.Ldloc, 53);
				ilcursor.EmitDelegate<Func<CharacterBody, int, bool>>((self, itemCount) =>
				{
					return itemCount > 0;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Growth Nectar - Conditional Override - IL Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(53),
				x => x.MatchConvR4(),
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(typeof(CharacterBody), "boostAllStatsMultiplier")
			))
			{
				ilcursor.RemoveRange(5);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 53);
				ilcursor.EmitDelegate<Func<CharacterBody, int, float>>((self, itemCount) =>
				{
					int buffCount = 0;
					foreach (BuffIndex buffIndex in BuffCatalog.nonHiddenBuffIndices)
					{
						if (self.HasBuff(buffIndex) && !BuffCatalog.ignoreGrowthNectarIndices.Contains(buffIndex))
						{
							buffCount++;
							if (buffCount >= BaseCap)
							{
								buffCount = BaseCap;
								break;
							}
						}
					}
					if (buffCount >= VFXCap)
					{
						if (self.HasBuff(ImperfectBuff))
						{
							self.RemoveBuff(ImperfectBuff);
						}
						if (!self.HasBuff(DLC2Content.Buffs.BoostAllStatsBuff))
						{
							self.AddBuff(DLC2Content.Buffs.BoostAllStatsBuff);
						}
					}
					else
					{
						if (self.HasBuff(DLC2Content.Buffs.BoostAllStatsBuff))
						{
							self.RemoveBuff(DLC2Content.Buffs.BoostAllStatsBuff);
						}
						if (!self.HasBuff(ImperfectBuff))
						{
							self.AddBuff(ImperfectBuff);
						}
					}
					return (BaseBoost + (StackBoost * Math.Max(0, itemCount - 1))) * buffCount;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Growth Nectar - Stats Override - IL Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "BoostAllStatsBuff")
			))
			{
				ilcursor.RemoveRange(2);
				ilcursor.EmitDelegate<Func<CharacterBody, bool>>((self) =>
				{
					return true;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Growth Nectar - Buff Check Override - IL Hook failed");
			}
		}
	}
}
