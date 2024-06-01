using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.CharacterAI;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace DevotionConfig.Changes
{
	public class LightChanges
	{
		internal static string BlackList_TierList_Raw;
		internal static string BlackList_ItemList_Raw;
		internal static string ItemDrop_CustomDropList_Raw;

		internal static bool Blacklist_Enable = false;
		internal static bool BlackList_Filter_CannotCopy = true;
		internal static bool BlackList_Filter_Scrap = true;
		internal static List<ItemTier> BlackList_TierList;
		internal static List<ItemDef> BlackList_ItemList;
		internal static bool FilterItems = false;
		internal static bool FilterTiers = false;
		
		internal static bool ItemDrop_Enable = false;
		internal static ItemIndex[] ItemDrop_CustomDropList;
		internal static int ItemDrop_Type = 0;

		internal static bool Misc_FixEvo = true;

		internal static GameObject ItemTakeOrb = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/ItemTakenOrbEffect");
		public LightChanges()
		{
			ClampConfig();
			UpdateItemDef();
			Hooks();
		}
		private void ClampConfig()
		{
			Math.Max(0, ItemDrop_Type);
			Math.Min(2, ItemDrop_Type);
		}
		private void UpdateItemDef()
		{
			//Fix up the tags on the Harness
			MainPlugin.ModLogger.LogInfo("Changing Lemurian Harness");
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/CU8/Harness/items.LemurianHarness.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.BrotherBlacklist);
				itemTags.Add(ItemTag.CannotCopy);
				itemTags.Add(ItemTag.CannotSteal);
				itemDef.tags = itemTags.ToArray();
			}
		}
		internal static void PostLoad()
        {
			if (Blacklist_Enable)
            {
				BlackList_ItemList = new List<ItemDef>();
				string[] itemString = BlackList_ItemList_Raw.Split(',');
				for (int i = 0; i < itemString.Length; i++)
				{
					ItemIndex itemIndex = ItemCatalog.FindItemIndex(itemString[i].Trim());
					if (itemIndex > ItemIndex.None)
					{
						ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
						if (itemDef)
						{
							BlackList_ItemList.Add(itemDef);
							FilterItems = true;
						}
					}
				}

				BlackList_TierList = new List<ItemTier>();
				itemString = BlackList_TierList_Raw.Split(',');
				for (int i = 0; i < itemString.Length; i++)
				{

					ItemTierDef itemTier = ItemTierCatalog.FindTierDef(itemString[i].Trim());
					if (itemTier)
					{
						BlackList_TierList.Add(itemTier.tier);
						FilterTiers = true;
					}
				}

				ItemDrop_CustomDropList = new ItemIndex[ItemTierCatalog.allItemTierDefs.Length];
				for (int i = 0; i < ItemDrop_CustomDropList.Length; i++)
                {
					ItemDrop_CustomDropList[i] = ItemIndex.None;
				}
				itemString = ItemDrop_CustomDropList_Raw.Split(',');
				for (int i = 0; i+1 < itemString.Length; i += 2)
				{
					ItemTierDef itemTier = ItemTierCatalog.FindTierDef(itemString[i].Trim());
					if (itemTier)
					{
						int tierIndex = GetItemTierIndex(itemTier);
						if (tierIndex > -1 && tierIndex < ItemDrop_CustomDropList.Length)
						{
							ItemIndex itemIndex = ItemCatalog.FindItemIndex(itemString[i + 1].Trim());
							if (itemIndex > ItemIndex.None)
							{
								ItemDrop_CustomDropList[tierIndex] = itemIndex;
							}
						}
						else
						{
							MainPlugin.ModLogger.LogWarning(string.Format("{0}Could not find Tier '{1}' for Custom Drop List.", MainPlugin.LOGNAME, itemString[i].Trim()));
						}
					}
				}
			}
        }
		private static int GetItemTierIndex(ItemTierDef itemTier)
        {
			for (int i = 0; i < ItemTierCatalog.allItemTierDefs.Length; i++)
            {
				if (ItemTierCatalog.allItemTierDefs[i] == itemTier)
                {
					return i;
                }
            }
			return -1;
		}
		private void Hooks()
		{
			if (Blacklist_Enable)
            {
				On.RoR2.PickupPickerController.SetOptionsFromInteractor += SetPickupPicker;
			}
			if (ItemDrop_Enable)
            {
				On.RoR2.DevotionInventoryController.DropScrapOnDeath += DropScrapOnDeath;
			}
			if (Misc_FixEvo)
            {
				On.RoR2.DevotionInventoryController.OnBossGroupDefeatedServer += OldBossGroupDefeatedServer;
				RoR2.BossGroup.onBossGroupDefeatedServer += BossGroupDefeatedServer;
				On.RoR2.CharacterAI.LemurianEggController.CreateItemTakenOrb += CreateItemTakeOrb_Egg;
			}
		}
		private void BossGroupDefeatedServer(BossGroup group)
		{
			if (!SceneCatalog.GetSceneDefForCurrentScene().needSkipDevotionRespawn)
			{
				DevotionInventoryController.ActivateDevotedEvolution();
			}
		}
		private void OldBossGroupDefeatedServer(On.RoR2.DevotionInventoryController.orig_OnBossGroupDefeatedServer orig, BossGroup bossGroup)
		{
			return; //Disabling this since it requires the Artifact active to work.
		}
		private void CreateItemTakeOrb_Egg(On.RoR2.CharacterAI.LemurianEggController.orig_CreateItemTakenOrb orig, LemurianEggController self, Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
        {
			DevotionInventoryController.s_effectPrefab = ItemTakeOrb;
			orig(self, effectOrigin, targetObject, itemIndex);
        }
		private void DropScrapOnDeath(On.RoR2.DevotionInventoryController.orig_DropScrapOnDeath orig, DevotionInventoryController self, ItemIndex devotionItem, CharacterBody minionBody)
		{
			if (ItemDrop_Type == 2)
			{
				return;
			}
			PickupIndex pickupIndex = PickupIndex.none;
			ItemDef itemDef = ItemCatalog.GetItemDef(devotionItem);
			if (itemDef != null)
			{
				int tierIndex = GetItemTierIndex(ItemTierCatalog.GetItemTierDef(itemDef.tier));
				if (ItemDrop_Type == 1)
				{
					if (tierIndex > -1)
                    {
						pickupIndex = PickupCatalog.FindPickupIndex(devotionItem);
					}
				}
				else
				{
					
					if (tierIndex > -1 && ItemDrop_CustomDropList != null)
                    {
						if (tierIndex < ItemDrop_CustomDropList.Length)
                        {
							pickupIndex = PickupCatalog.FindPickupIndex(ItemDrop_CustomDropList[tierIndex]);
						}
                    }
				}
			}
			if (pickupIndex != PickupIndex.none)
			{
				Vector3 angle = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up) * minionBody.transform.forward;
				angle.y += 8f;
				PickupDropletController.CreatePickupDroplet(pickupIndex, minionBody.footPosition + (Vector3.up * 1f), angle.normalized * 25f);
			}
			return;
		}
		private void SetPickupPicker(On.RoR2.PickupPickerController.orig_SetOptionsFromInteractor orig, PickupPickerController self, Interactor activator)
		{
			LemurianEggController eggController = self.GetComponent<LemurianEggController>();
			if (eggController)
            {
				if (!activator)
				{
					return;
				}
				CharacterBody component = activator.GetComponent<CharacterBody>();
				if (!component)
				{
					return;
				}
				Inventory inventory = component.inventory;
				if (!inventory)
				{
					return;
				}
				List<PickupPickerController.Option> list = new List<PickupPickerController.Option>();
				for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
				{
					ItemIndex itemIndex = inventory.itemAcquisitionOrder[i];
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					ItemTierDef itemTierDef = ItemTierCatalog.GetItemTierDef(itemDef.tier);
					PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(itemIndex);
					if (itemTierDef && !itemDef.hidden && itemDef.canRemove)
                    {
						if (!FilterTiers || !BlackList_TierList.Contains(itemTierDef.tier))
                        {
							if ((!BlackList_Filter_Scrap || (!itemDef.ContainsTag(ItemTag.Scrap) && !itemDef.ContainsTag(ItemTag.PriorityScrap))) && (!BlackList_Filter_CannotCopy || !itemDef.ContainsTag(ItemTag.CannotCopy)))
							{
								if (!FilterItems || !BlackList_ItemList.Contains(itemDef))
								{
									list.Add(new PickupPickerController.Option
									{
										available = true,
										pickupIndex = pickupIndex
									});
								}
							}
						}
					}
				}
				self.SetOptionsServer(list.ToArray());
				return;
            }
			orig(self, activator);
		}
	}
}
