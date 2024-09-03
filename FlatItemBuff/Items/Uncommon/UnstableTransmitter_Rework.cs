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

		internal static bool Enable = true;
		internal static float BaseCooldown = 30f;
		internal static float AllyStackCooldown = 0.5f;
		internal static float CapCooldown = 2f;
		internal static float BaseDamage = 3.5f;
		internal static float StackDamage = 2.8f;
		internal static float BaseRadius = 15f;
		internal static float StackRadius = 0f;
		internal static bool TeleImmobile = true;
		public UnstableTransmitter_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Unstable Transmitter");
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
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
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
					descRadius = string.Format(" The ally <style=cIsDamage>explodes</style> upon arrival in a <style=cIsDamage>{0}m</style> <style=cStack>(+{1}% per stack)</style> radius", BaseRadius, StackRadius);
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
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC2/Items/TeleportOnLowHealth/TeleportOnLowHealth.asset").WaitForCompletion();
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
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.OnInventoryChanged += new ILContext.Manipulator(IL_OnInventoryChanged);
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.UnstableTransmitter_Rework>(self.inventory.GetItemCount(DLC2Content.Items.TeleportOnLowHealth));
		}
		private void IL_OnInventoryChanged(ILContext il)
		{
			//doing this since it shits the bed otherwise
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "TeleportOnLowHealth"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Unstable Transmitter Rework - Effect Override - IL Hook failed");
			}
		}
	}
}
