using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Planula
	{
		private const string LogName = "Planula";
		internal static bool Enable = false;
		internal static float BaseFlatHeal = 10.0f;
		internal static float StackFlatHeal = 10.0f;
		internal static float BaseMaxHeal = 0.01f;
		internal static float StackMaxHeal = 0.01f;
		public Planula()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateItemDef();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			Hooks();
		}
		private void ClampConfig()
		{
			BaseFlatHeal = Math.Max(0f, BaseFlatHeal);
			StackFlatHeal = Math.Max(0f, StackFlatHeal);
			BaseMaxHeal = Math.Max(0f, BaseMaxHeal);
			StackMaxHeal = Math.Max(0f, StackMaxHeal);
		}
		private void UpdateItemDef()
		{
			//"RoR2/Base/ParentEgg/ParentEgg.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("2ed530121eace7240ad4716be1e4688c").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.BrotherBlacklist);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void UpdateText()
		{
			string flat = "";
			string max = "";

			if (BaseFlatHeal > 0f)
			{
				flat = string.Format("<style=cIsHealing>{0}", BaseFlatHeal);
				if (StackFlatHeal > 0f)
				{
					flat += string.Format(" <style=cStack>(+{0} per stack)</style>", StackFlatHeal);
				}
				flat += string.Format("</style>");
			}

			if (BaseMaxHeal > 0f)
            {
				if (BaseFlatHeal > 0f)
                {
					max = " plus an additional";
                }
				max += string.Format(" <style=cIsHealing>{0}%", BaseMaxHeal * 100f);
				if (StackFlatHeal > 0f)
				{
					max += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackMaxHeal * 100f);
				}
				max += string.Format("</style> of <style=cIsHealing>maximum health</style>");
			}

			string pickup = "Receive flat healing when taking damage.";
			string desc = string.Format("Heal from <style=cIsDamage>incoming damage</style> for {0}{1}.", flat, max);

			if (BaseMaxHeal > 0f)
			{
				pickup = "Receive healing when taking damage.";
			}

			LanguageAPI.Add("ITEM_PARENTEGG_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PARENTEGG_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_HealthTakeDamage);
		}
		private void IL_HealthTakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "parentEgg"),
				x => x.MatchConvR4(),
				x => x.MatchLdcR4(15f),
				x => x.MatchMul()
			))
			{
				ilcursor.Index -= 1;
				ilcursor.RemoveRange(5);
				ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
				{
					int itemCount = self.itemCounts.parentEgg - 1;
					float healing = 0f;
					if (BaseFlatHeal > 0f)
					{
						healing += BaseFlatHeal + (StackFlatHeal * itemCount);
					}
					if (BaseMaxHeal > 0f)
					{
						float percentHeal = BaseMaxHeal + (StackMaxHeal * itemCount);
						healing += percentHeal * self.fullCombinedHealth;
					}
					return healing;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_HealthTakeDamage - Hook failed");
			}
		}
	}
}
