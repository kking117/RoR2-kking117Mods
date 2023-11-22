using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Planula_Rework
	{
		internal static bool Enable = false;
		internal static float BaseDamage = 0.8f;
		internal static float StackDamage = 0.6f;
		internal static float Duration = 5f;
		internal static float Radius = 15f;

		internal static GameObject BurnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab").WaitForCompletion();
		public Planula_Rework()
		{
			if (!Enable)
            {
				new Planula();
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Planula");
			ClampConfig();
			UpdateItemDef();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			Duration = Math.Max(0f, Duration);
			Radius = Math.Max(0f, Radius);
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ParentEgg/ParentEgg.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.BrotherBlacklist);
				itemTags.Add(ItemTag.Damage);
				itemTags.Remove(ItemTag.Healing);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string radius = "";
			string damage = "";

			radius = string.Format("<style=cIsDamage>{0}m</style>", Radius);

			damage = string.Format("<style=cIsDamage>{0}%", BaseDamage * Duration * 100f);
			if (StackDamage > 0.0f)
            {
				damage += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * Duration * 100f);
			}
			damage += "</style> base damage";

			string pickup = "Burn nearby enemies while in combat.";
			string desc = string.Format("While in combat, <style=cIsDamage>burn</style> all enemies within {0} for {1}.", radius, damage);
			
			LanguageAPI.Add("ITEM_PARENTEGG_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PARENTEGG_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_HealthTakeDamage);
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.Planula_Rework>(self.inventory.GetItemCount(RoR2Content.Items.ParentEgg));
		}
		private void IL_HealthTakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", nameof(RoR2.HealthComponent.itemCounts.parentEgg))
			);
			ilcursor.Index += 1;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
		}
		internal static float GetTotalDamage(int itemCount)
        {
			return BaseDamage + (StackDamage * (itemCount - 1));
        }
		internal static float GetDuration()
		{
			return 0.5f * Duration;
		}
	}
}
