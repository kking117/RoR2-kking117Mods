using System;
using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using R2API.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ConfigurableBenthic
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.ConfigurableBenthic";
		public const string MODNAME = "ConfigurableBenthic";
		public const string MODTOKEN = "KKING117_CONFIGBENTHIC_";
		public const string MODVERSION = "1.0.0";

		public static ConfigEntry<int> Config_BaseCount;
		public static ConfigEntry<int> Config_StackCount;

		public static ConfigEntry<string> Config_BlackList;
		public static ConfigEntry<bool> Config_WholeStack;

		public static ConfigEntry<int> Config_SelectionMethod;
		public static ConfigEntry<bool> Config_PreferScrap;
		public static ConfigEntry<bool> Config_RefreshOnUpgrade;

		private static bool NewDesc = false;

		public static List<ItemDef> BanList;

		private void Awake()
		{
			ReadConfig();
			if (Config_BaseCount.Value != 3 || Config_StackCount.Value != 3)
			{
				UpdateText();
			}
			Hooks();
		}
		private static void Hooks()
		{
			On.RoR2.ItemCatalog.Init += ItemCatalog_Init;
			On.RoR2.CharacterMaster.TryCloverVoidUpgrades += CloverVoidUpgrade;
		}
		private static void UpdateText()
		{
			string desc = String.Format("<style=cIsUtility>Upgrades {0}</style> <style=cStack>(+{1} per stack)</style> random items to items of the next <style=cIsUtility>higher rarity</style> at the <style=cIsUtility>start of each stage</style>. <style=cIsVoid>Corrupts all 57 Leaf Clovers</style>.", Config_BaseCount.Value, Config_StackCount.Value);
			LanguageAPI.Add(MODTOKEN + "ITEM_CLOVERVOID_DESC", desc);
			NewDesc = true;
		}
		private static void ItemCatalog_Init(On.RoR2.ItemCatalog.orig_Init orig)
		{
			orig();
			if (NewDesc)
			{
				DLC1Content.Items.CloverVoid.descriptionToken = MODTOKEN + "ITEM_CLOVERVOID_DESC";
			}
			BanList = new List<ItemDef>();
			string[] items = MainPlugin.Config_BlackList.Value.Split(',');
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = items[i].Trim();
				ItemIndex itemIndex = ItemCatalog.FindItemIndex(items[i]);
				if (itemIndex > ItemIndex.None)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					if (itemDef)
					{
						BanList.Add(itemDef);
					}
				}
			}
		}
		private static void CloverVoidUpgrade(On.RoR2.CharacterMaster.orig_TryCloverVoidUpgrades orig, CharacterMaster self)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			int itemCount = self.inventory.GetItemCount(DLC1Content.Items.CloverVoid);
			if (itemCount > 0)
			{
				int upgradeCount = Config_BaseCount.Value + ((itemCount - 1) * Config_StackCount.Value);
				if (upgradeCount > 0)
				{
					List<ItemIndex> Tier2List = RemoveClutter_PickupList(Run.instance.availableTier2DropList);
					List<ItemIndex> Tier3List = RemoveClutter_PickupList(Run.instance.availableTier3DropList);

					List<ItemIndex> itemList = GetAndSortInventory(self);

					int totalUpgrades = 0;
					int i = 0;
					List<ItemIndex> ConvertList = new List<ItemIndex>();
					while (totalUpgrades < upgradeCount && i < itemList.Count)
					{
						ItemDef itemDef = ItemCatalog.GetItemDef(itemList[i]);
						ItemDef newitemDef = null;
						List<ItemIndex> UpgradeList = null;
						if (itemDef.tier == ItemTier.Tier2)
						{
							UpgradeList = Tier3List;
						}
						else if (itemDef.tier == ItemTier.Tier1)
						{
							UpgradeList = Tier2List;
						}
						if (UpgradeList != null && UpgradeList.Count > 0)
						{
							Util.ShuffleList<ItemIndex>(UpgradeList, self.cloverVoidRng);
							UpgradeList.Sort(CompareTags);
							newitemDef = ItemCatalog.GetItemDef(UpgradeList[0]);
						}
						if (newitemDef != null)
						{
							totalUpgrades++;
                            
							if(Config_WholeStack.Value)
                            {
								int removeCount = self.inventory.GetItemCount(itemDef);
								self.inventory.GiveItem(newitemDef, removeCount);
								self.inventory.RemoveItem(itemDef, removeCount);
								itemList.RemoveAt(i);
							}
							else
                            {
								self.inventory.GiveItem(newitemDef, 1);
								self.inventory.RemoveItem(itemDef, 1);
								if(self.inventory.GetItemCount(itemDef) < 1)
                                {
									itemList.RemoveAt(i);
								}
							}
							
							CharacterMasterNotificationQueue.PushItemTransformNotification(self, itemDef.itemIndex, newitemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.CloverVoid);

							i = -1;
							if (Config_RefreshOnUpgrade.Value)
                            {
								itemList = GetAndSortInventory(self);
							}
						}
						i++;

						//Ripped Directly from ROR2.dll
						int CompareTags(ItemIndex lhs, ItemIndex rhs)
						{
							int ScoreA = 0;
							int ScoreB = 0;
							ItemDef itemDef2 = ItemCatalog.GetItemDef(lhs);
							ItemDef itemDef3 = ItemCatalog.GetItemDef(rhs);
							if (itemDef.ContainsTag(ItemTag.Damage))
							{
								if (itemDef2.ContainsTag(ItemTag.Damage))
								{
									ScoreA = 1;
								}
								if (itemDef3.ContainsTag(ItemTag.Damage))
								{
									ScoreB = 1;
								}
							}
							if (itemDef.ContainsTag(ItemTag.Healing))
							{
								if (itemDef2.ContainsTag(ItemTag.Healing))
								{
									ScoreA = 1;
								}
								if (itemDef3.ContainsTag(ItemTag.Healing))
								{
									ScoreB = 1;
								}
							}
							if (itemDef.ContainsTag(ItemTag.Utility))
							{
								if (itemDef2.ContainsTag(ItemTag.Utility))
								{
									ScoreA = 1;
								}
								if (itemDef3.ContainsTag(ItemTag.Utility))
								{
									ScoreB = 1;
								}
							}
							return ScoreB - ScoreA;
						}
					}
					//This apparently is a thing?
					if (totalUpgrades > 0)
					{
						GameObject bodyInstanceObject = self.bodyInstanceObject;
						if (bodyInstanceObject)
						{
							Util.PlaySound("Play_item_proc_extraLife", bodyInstanceObject);
						}
					}
				}
			}
		}

		private static List<ItemIndex> GetAndSortInventory(CharacterMaster master)
        {
			List<ItemIndex> itemList = new List<ItemIndex>(master.inventory.itemAcquisitionOrder);
			itemList = RemoveClutter(itemList);
			if (master.cloverVoidRng == null)
			{
				master.cloverVoidRng = new Xoroshiro128Plus(Run.instance.seed);
			}
			Util.ShuffleList<ItemIndex>(itemList, master.cloverVoidRng);
			switch (Config_SelectionMethod.Value)
            {
				case 1:
					itemList = SortItemList_ByLowest(itemList, master);
					break;
				case 2:
					itemList = SortItemList_ByHighest(itemList, master);
					break;
            }
			if(Config_PreferScrap.Value)
            {
				itemList = SortItemList_ByScrap(itemList);
			}
			return itemList;
        }
		private static List<ItemIndex> RemoveClutter(List<ItemIndex> itemList)
        {
			for(int i = 0; i< itemList.Count; i++)
            {
				ItemDef itemDef = ItemCatalog.GetItemDef(itemList[i]);
				if(itemDef)
                {
					if(!IsItemUpgradeable(itemDef))
                    {
						itemList.RemoveAt(i);
						i--;
                    }
                }
            }
			return itemList;
		}
		private static List<ItemIndex> RemoveClutter_PickupList(List<PickupIndex> pickupList)
        {
			List<ItemIndex> itemList = new List<ItemIndex>();
			for (int i = 0; i < pickupList.Count; i++)
			{
				ItemDef itemDef = ItemCatalog.GetItemDef(pickupList[i].itemIndex);
				if (itemDef)
				{
					if(BanList == null || !BanList.Contains(itemDef))
                    {
						itemList.Add(itemDef.itemIndex);
					}
				}
			}
			return itemList;
		}
		private static bool IsItemUpgradeable(ItemDef item)
        {
			return item.tier == ItemTier.Tier1 || item.tier == ItemTier.Tier2;
		}
		private static List<ItemIndex> SortItemList_ByLowest(List<ItemIndex> itemList, CharacterMaster master)
		{
			itemList.Sort(CompareStackSize);
			int CompareStackSize(ItemIndex lhs, ItemIndex rhs)
			{
				int num4 = master.inventory.GetItemCount(lhs);
				int num5 = master.inventory.GetItemCount(rhs);
				return num4 - num5;
			}
			return itemList;
		}
		private static List<ItemIndex> SortItemList_ByHighest(List<ItemIndex> itemList, CharacterMaster master)
		{
			itemList.Sort(CompareStackSize);
			int CompareStackSize(ItemIndex lhs, ItemIndex rhs)
			{
				int num4 = master.inventory.GetItemCount(lhs);
				int num5 = master.inventory.GetItemCount(rhs);
				return num5 - num4;
			}
			return itemList;
		}
		private static List<ItemIndex> SortItemList_ByScrap(List<ItemIndex> itemList)
        {
			itemList.Sort(ByIsScrap);
			int ByIsScrap(ItemIndex lhs, ItemIndex rhs)
			{
				int scoreA = 0;
				int scoreB = 0;
				ItemDef itemDefA = ItemCatalog.GetItemDef(lhs);
				ItemDef itemDefB = ItemCatalog.GetItemDef(rhs);
				if (itemDefA.ContainsTag(ItemTag.PriorityScrap))
				{
					scoreA = 2;
				}
				else if (itemDefA.ContainsTag(ItemTag.Scrap))
				{
					scoreA = 1;
				}
				if (itemDefB.ContainsTag(ItemTag.PriorityScrap))
				{
					scoreB = 2;
				}
				else if (itemDefB.ContainsTag(ItemTag.Scrap))
				{
					scoreB = 1;
				}
				return scoreB - scoreA;
			}
			return itemList;
		}
		private void ReadConfig()
		{
			Config_BaseCount = Config.Bind<int>(new ConfigDefinition("Stacking", "Base Upgrade Count"), 3, new ConfigDescription("How many items to upgrade at a single stack.", null, Array.Empty<object>()));
			Config_StackCount = Config.Bind<int>(new ConfigDefinition("Stacking", "Stack Upgrade Count"), 3, new ConfigDescription("How many items to upgrade for each additional stack.", null, Array.Empty<object>()));
			Config_BlackList = Config.Bind<string>(new ConfigDefinition("Upgrading", "Upgrade Blacklist"), "", new ConfigDescription("Prevents upgrading into these specific items. (Example = Clover, ExtraLife, MoreMissile)", null, Array.Empty<object>()));
			Config_WholeStack = Config.Bind<bool>(new ConfigDefinition("Upgrading", "Upgrade Entire Stack"), true, new ConfigDescription("Should the entire stack get upgraded or a single instance of the selected item?", null, Array.Empty<object>()));
			Config_SelectionMethod = Config.Bind<int>(new ConfigDefinition("Item Selection", "Selection Method"), 0, new ConfigDescription("The method to use when selecting items for upgrading. (0 = random, 1 = lowest stacks first, 2 = highest stacks first)", null, Array.Empty<object>()));
			Config_PreferScrap = Config.Bind<bool>(new ConfigDefinition("Item Selection", "Prioritize Scrap"), false, new ConfigDescription("Should Scrap have priority over the selection method?", null, Array.Empty<object>()));
			Config_RefreshOnUpgrade = Config.Bind<bool>(new ConfigDefinition("Item Selection", "Refresh Selections On Each Upgrade"), true, new ConfigDescription("Should we revaluate our selections after each upgrade? (Vanilla = true) (Recommended to disable if sorting by stack count and upgrading individual items.)", null, Array.Empty<object>()));
		}
	}
}
