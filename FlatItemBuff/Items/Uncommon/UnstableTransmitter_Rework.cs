using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class UnstableTransmitter_Rework
	{
		public static DeployableSlot Drone_DeployableSlot;
		public DeployableAPI.GetDeployableSameSlotLimit Drone_DeployableLimit;
		//"RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab"
		internal static GameObject BlinkPrefab = Addressables.LoadAssetAsync<GameObject>("8fccbb1eb99f91849ba3b6076e1f220b").WaitForCompletion();

		private const string LogName = "Unstable Transmitter Rework";
		internal static bool Enable = false;
		internal static float BaseCooldown = 30f;
		internal static float AllyStackCooldown = 0.5f;
		internal static float CapCooldown = 2f;
		internal static float BaseDamage = 3.5f;
		internal static float StackDamage = 2.8f;
		internal static float BaseRadius = 16f;
		internal static float StackRadius = 0f;
		internal static float ProcRate = 1f;
		internal static bool ProcBands = true;
		internal static bool AllyOwnsDamage = false;
		internal static bool Respawns = false;
		internal static float TeleportRadius = 40f;
		internal static float TeleFragRadius = 60f;
		internal static bool TeleImmobile = true;
		public UnstableTransmitter_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateItemDef();
			CreateDeployableSlot();
			Hooks();
			UpdateText();
		}
		private void ClampConfig()
		{
			BaseCooldown = Math.Max(0f, BaseCooldown);
			AllyStackCooldown = Math.Max(0f, AllyStackCooldown);
			CapCooldown = Math.Max(0f, CapCooldown);
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseRadius = Math.Max(0f, BaseRadius);
			StackRadius = Math.Max(0f, StackRadius);
			TeleportRadius = Math.Max(0f, TeleportRadius);
			TeleFragRadius = Math.Max(0f, TeleFragRadius);
			ProcRate = Math.Max(0f, ProcRate);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "Allies periodically teleport to you";
			string desc = "Gain a <style=cIsDamage>Strike Drone</style>.\n";
			if (AllyStackCooldown > 0f)
            {
				desc += string.Format("Every <style=cIsUtility>{0}s</style> <style=cStack>(-{1}% per ally)</style> an ally you own will <style=cIsUtility>teleport nearby</style>.", BaseCooldown, AllyStackCooldown * 100f);
			}
			else
            {
				desc += string.Format("Every <style=cIsUtility>{0}s</style> a random ally you own will <style=cIsUtility>teleport nearby</style>.", BaseCooldown);
			}
			if (BaseRadius > 0f)
			{
				string descRadius = string.Format(" The ally <style=cIsDamage>explodes</style> upon arrival in a <style=cIsDamage>{0}m</style> radius", BaseRadius);
				if (StackRadius > 0f)
                {
					descRadius = string.Format(" The ally <style=cIsDamage>explodes</style> upon arrival in a <style=cIsDamage>{0}m</style> <style=cStack>(+{1}m per stack)</style> radius", BaseRadius, StackRadius);
				}
				desc += descRadius;
				pickup += " with a bang";
				if (StackDamage > 0f)
                {
					desc += string.Format(" for <style=cIsDamage>{1}%</style> <style=cStack>(+{2}% per stack)</style> base damage.", BaseRadius, BaseDamage * 100f, StackDamage * 100f);
				}
				else
                {
					desc += string.Format(" for <style=cIsDamage>{1}%</style> base damage.", BaseRadius * 100f, BaseDamage * 100f);
				}
			}
			LanguageAPI.Add("ITEM_TELEPORTONLOWHEALTH_PICKUP", pickup + ".");
			LanguageAPI.Add("ITEM_TELEPORTONLOWHEALTH_DESC", desc);
		}
		private void UpdateItemDef()
		{
			//"RoR2/DLC2/Items/TeleportOnLowHealth/TeleportOnLowHealth.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("354a6df4ce8249f4cbe9c0e207e2b674").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.AIBlacklist);
				itemTags.Add(ItemTag.BrotherBlacklist);
				itemTags.Add(ItemTag.CannotCopy);
				itemTags.Remove(ItemTag.LowHealth);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void CreateDeployableSlot()
		{
			Drone_DeployableSlot = DeployableAPI.RegisterDeployableSlot(new DeployableAPI.GetDeployableSameSlotLimit(GetDrone_DeployableLimit));
		}
		private int GetDrone_DeployableLimit(CharacterMaster self, int deployableMult)
		{
			return 1 * deployableMult;
		}
		private void Hooks()
		{
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
			On.RoR2.TeleportOnLowHealthBehavior.GetItemDef += GetItemDef;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.UnstableTransmitter_Rework>(self.inventory.GetItemCountEffective(DLC2Content.Items.TeleportOnLowHealth));
		}

		private ItemDef GetItemDef(On.RoR2.TeleportOnLowHealthBehavior.orig_GetItemDef orig)
		{
			return JunkContent.Items.PlasmaCore;
		}
	}
}
