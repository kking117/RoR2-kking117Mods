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
	public class AntlerShield
	{
		internal static bool Enable = true;
		internal static float StackArmor = 7.5f;
		internal static float StackSpeed = 0.07f;
		public AntlerShield()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Antler Shield");
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
			MainPlugin.ModLogger.LogInfo("Updating item text");
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
			LanguageAPI.Add("ITEM_NEGATEATTACK_PICKUP", pickup + ".");
			LanguageAPI.Add("ITEM_NEGATEATTACK_DESC", desc + ".");
		}

		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC2/Items/NegateAttack/NegateAttack.asset").WaitForCompletion();
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
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_OnTakeDamage);
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(DLC2Content.Items.NegateAttack);
				if (itemCount > 0)
				{
					args.moveSpeedMultAdd += itemCount * StackSpeed;
					args.armorAdd += itemCount * StackArmor;
				}
			}
		}
		private void IL_OnTakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "antlerShield")
			))
			{
				ilcursor.Index += 1;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Antler Shield IL Hook failed");
			}
		}
	}
}
