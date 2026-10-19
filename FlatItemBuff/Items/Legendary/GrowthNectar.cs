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
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			Hooks();
		}
		private void ClampConfig()
		{
			BaseBoost = Math.Max(0f, BaseBoost);
			StackBoost = Math.Max(0f, StackBoost);
			BaseCap = Math.Max(0, BaseCap);
		}
		private void UpdateText()
		{
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
			On.RoR2.Items.BoostAllStatsBehavior.RecalculateBuffCount += RecalculateBuffCount;
			On.RoR2.Items.BoostAllStatsBehavior.OnBuffFirstStackGained += OnBuffFirstStackGained;
			//SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}

		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCountA = self.inventory.GetItemCountEffective(DLC2Content.Items.BoostAllStats);
			if (itemCountA == 0)
            {
				self.ClearAllBuffs(DLC2Content.Buffs.BoostAllStatsBuff);
            }
		}
		internal static void OnBuffFirstStackGained(On.RoR2.Items.BoostAllStatsBehavior.orig_OnBuffFirstStackGained orig, RoR2.Items.BoostAllStatsBehavior self, CharacterBody body, BuffDef buffDef)
		{
			if (body == self.body && self.IsValidBuff(buffDef.buffIndex))
            {
				self.RecalculateBuffCount();
			}
		}
		internal static void RecalculateBuffCount(On.RoR2.Items.BoostAllStatsBehavior.orig_RecalculateBuffCount orig, RoR2.Items.BoostAllStatsBehavior self)
		{
			if (self.body)
			{
				int buffCount = 1;
				foreach (BuffIndex buffIndex in BuffCatalog.nonHiddenBuffIndices)
				{
					if (buffCount < BaseCap)
                    {
						if (self.body.HasBuff(buffIndex) && self.IsValidBuff(buffIndex))
						{
							buffCount++;
						}
					}
					else
                    {
						break;
                    }
				}
				if (buffCount >= BaseCap)
				{
					buffCount = BaseCap;
				}
				self.UpdateBuffCount(buffCount);
			}
		}
		private void IL_ItemBehavior(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(0)
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldc_I4, BaseCap);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_ItemBehavior A - IL Hook failed");
			}
		}
		private void IL_OnRecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcR4(0.04f),
				x => x.MatchLdloc(68)
			))
			{
				//ilcursor.Index -= 2;
				ilcursor.RemoveRange(4);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 68);
				ilcursor.EmitDelegate<Func<CharacterBody, int, float>>((self, buffCount) =>
				{
					if (self.master)
                    {
						Inventory inv = self.master.inventory;
						if (inv)
                        {
							int itemCount = Math.Max(0, inv.GetItemCountEffective(DLC2Content.Items.BoostAllStats) - 1);
							return buffCount * (BaseBoost + (StackBoost * itemCount));
						}
                    }
					return buffCount * BaseBoost;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnRecalculateStats A - IL Hook failed");
			}
		}
	}
}
