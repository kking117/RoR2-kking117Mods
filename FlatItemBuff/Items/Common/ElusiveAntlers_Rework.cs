using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class ElusiveAntlers_Rework
	{
		private const string LogName = "Elusive Antlers";
		internal static bool Enable = false;
		internal static float StackArmor = 5f;
		internal static float StackSpeed = 0.07f;
		public ElusiveAntlers_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			UpdateItemDef();
			Hooks();
		}
		private void ClampConfig()
		{
			StackArmor = Math.Max(0f, StackArmor);
			StackSpeed = Math.Max(0f, StackSpeed);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "Slightly increase";
			string desc = "Increases";
			if (StackArmor > 0f)
			{
				pickup += " armor";
				desc += string.Format(" <style=cIsHealing>armor</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>", StackArmor);
				if (StackSpeed > 0f)
				{
					pickup += " and";
					desc += " and";
				}
			}
			if (StackSpeed > 0f)
			{
				pickup += " movement speed";
				desc += string.Format(" <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style> <style=cStack>(+{0}% per stack)</style>", StackSpeed * 100f);
			}
			LanguageAPI.Add("ITEM_SPEEDBOOSTPICKUP_PICKUP", pickup + ".");
			LanguageAPI.Add("ITEM_SPEEDBOOSTPICKUP_DESC", desc + ".");
		}

		private void UpdateItemDef()
		{
			//"RoR2/DLC2/Items/SpeedBoostPickup/SpeedBoostPickup.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("eb5f0448e1b6ee048ba33b62ff78526f").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Remove(ItemTag.AIBlacklist);
				itemTags.Remove(ItemTag.BrotherBlacklist);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.OnInventoryChanged += new ILContext.Manipulator(IL_OnInventoryChanged);
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCountEffective(DLC2Content.Items.SpeedBoostPickup);
				if (itemCount > 0)
				{
					args.moveSpeedMultAdd += itemCount * StackSpeed;
					args.armorAdd += itemCount * StackArmor;
				}
			}
		}
		private void IL_OnInventoryChanged(ILContext il)
		{
			//doing this since it shits the bed otherwise
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "SpeedBoostPickup"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnInventoryChanged - Hook failed.");
			}
		}
	}
}
