using System;
using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ConfigurableBenthic
{
	public enum OverspillBonus : int
	{
		None = 0,
		PlusOne = 1,
		Duplicate = 2,
	}

	public enum SelectMethod : int
	{
		Random = 0,
		LeastStacks = 1,
		MostStacks = 2,
	}

	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.ConfigurableBenthic";
		public const string MODNAME = "ConfigurableBenthic";
		public const string MODTOKEN = "KKING117_CONFIGBENTHIC_";
		public const string MODVERSION = "1.1.0";

		public static int Config_BaseCount = 3;
		public static int Config_StackCount = 3;

		public static string Config_BlackList = "";
		public static bool Config_WholeStack = true;

		public static SelectMethod Config_Select_Method = SelectMethod.Random;
		public static bool Config_Select_PreferScrap = false;
		public static bool Config_Select_AllowVoid = false;
		public static bool Config_Select_AllowSelf = true;

		public static bool Config_RefreshOnUpgrade;
		public static bool Config_Overspill_Enable = false;
		public static OverspillBonus Config_Overspill_Bonus = OverspillBonus.PlusOne;

		private static bool NewDesc = false;

		public static List<ItemDef> BanList;

		private void Awake()
		{
			ReadConfig();
			if (Config_BaseCount != 3 || Config_StackCount != 3)
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
			string desc = String.Format("<style=cIsUtility>Upgrades {0}</style> <style=cStack>(+{1} per stack)</style> random items to items of the next <style=cIsUtility>higher rarity</style> at the <style=cIsUtility>start of each stage</style>. <style=cIsVoid>Corrupts all 57 Leaf Clovers</style>.", Config_BaseCount, Config_StackCount);
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
			string[] items = MainPlugin.Config_BlackList.Split(',');
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
				int upgradeCount = Config_BaseCount + (Math.Max(0, itemCount - 1) * Config_StackCount);
				if (upgradeCount > 0)
				{
					List<ItemIndex> itemList = GetAndSortInventory(self);

					int totalUpgrades = 0;
					int i = 0;
					int failCount = 0;
					List<ItemIndex> ConvertList = new List<ItemIndex>();
					while (totalUpgrades < upgradeCount && i < itemList.Count)
					{
						bool refreshList = false;
						ItemDef itemDef = ItemCatalog.GetItemDef(itemList[i]);
						ItemDef newitemDef = null;
						List<ItemIndex> UpgradeList = GetUpgradeList(itemDef);
						if (UpgradeList != null && UpgradeList.Count > 0)
						{
							Util.ShuffleList<ItemIndex>(UpgradeList, self.cloverVoidRng);
							UpgradeList.Sort(CompareTags);
							newitemDef = ItemCatalog.GetItemDef(UpgradeList[0]);
						}
						if (newitemDef != null)
						{
							failCount = 0;
							totalUpgrades++;

							int plusCount = 0;
							int multCount = 1;

							if (itemDef.tier == ItemTier.Tier3 || itemDef.tier == ItemTier.VoidTier3)
							{
								if (Config_Overspill_Bonus == OverspillBonus.PlusOne)
								{
									plusCount = 1;
								}
								if (Config_Overspill_Bonus == OverspillBonus.Duplicate)
								{
									multCount = 2;
								}
							}

							if (Config_WholeStack)
                            {
								int removeCount = self.inventory.GetItemCount(itemDef);
								int addCount = (removeCount * multCount) + plusCount;
								
								self.inventory.GiveItem(newitemDef, addCount);
								self.inventory.RemoveItem(itemDef, removeCount);
								itemList.RemoveAt(i);
							}
							else
                            {
								int addCount = (1 * multCount) + plusCount;
								self.inventory.GiveItem(newitemDef, addCount);
								self.inventory.RemoveItem(itemDef, 1);
								if(self.inventory.GetItemCount(itemDef) < 1)
                                {
									itemList.RemoveAt(i);
								}
								
							}

							i = -1;

							CharacterMasterNotificationQueue.PushItemTransformNotification(self, itemDef.itemIndex, newitemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.CloverVoid);

							if (Config_Select_PreferScrap)
							{
								if (newitemDef.ContainsTag(ItemTag.Scrap) || newitemDef.ContainsTag(ItemTag.PriorityScrap))
								{
									refreshList = true;
								}
							}
							if (Config_Select_Method > SelectMethod.Random && Config_WholeStack)
							{
								if (itemList.Count < 1)
								{
									failCount += 1;
									if (failCount > 3)
                                    {
										break;
                                    }
									refreshList = true;
								}
							}
							else
                            {
								refreshList = true;
							}
                            
							if (refreshList)
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
			switch (Config_Select_Method)
            {
				case SelectMethod.LeastStacks:
					itemList = SortItemList_ByLowest(itemList, master);
					break;
				case SelectMethod.MostStacks:
					itemList = SortItemList_ByHighest(itemList, master);
					break;
				default:
					Util.ShuffleList<ItemIndex>(itemList, master.cloverVoidRng);
					break;
			}
			if(Config_Select_PreferScrap)
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

		private static List<ItemIndex> GetUpgradeList(ItemDef oldItemDef)
        {
			List<ItemIndex> nullList = null;
			if (oldItemDef.tier == ItemTier.Tier3)
			{
				return RemoveClutter_PickupList(Run.instance.availableTier1DropList);
			}
			else if (oldItemDef.tier == ItemTier.Tier2)
			{
				return RemoveClutter_PickupList(Run.instance.availableTier3DropList);
			}
			else if (oldItemDef.tier == ItemTier.Tier1)
			{
				return RemoveClutter_PickupList(Run.instance.availableTier2DropList);
			}
			if (oldItemDef.tier == ItemTier.VoidTier3)
			{
				return RemoveClutter_PickupList(Run.instance.availableVoidTier1DropList);
			}
			else if (oldItemDef.tier == ItemTier.VoidTier2)
			{
				return RemoveClutter_PickupList(Run.instance.availableVoidTier3DropList);
			}
			else if (oldItemDef.tier == ItemTier.VoidTier1)
			{
				return RemoveClutter_PickupList(Run.instance.availableVoidTier2DropList);
			}
			return nullList;
		}
		private static bool IsItemUpgradeable(ItemDef item)
        {
			if (!Config_Select_AllowSelf && item == DLC1Content.Items.CloverVoid)
            {
				return false;
            }
			if (item.tier == ItemTier.Tier1 || item.tier == ItemTier.Tier2)
            {
				return true;
            }
			if (item.tier == ItemTier.Tier3 && Config_Overspill_Enable)
            {
				return true;
            }
			if (Config_Select_AllowVoid)
            {
				if (item.tier == ItemTier.VoidTier1 || item.tier == ItemTier.VoidTier2)
				{
					return true;
				}
				if (item.tier == ItemTier.VoidTier3 && Config_Overspill_Enable)
                {
					return true;
                }
			}
			return false;
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
			Config_BaseCount = Config.Bind("Stacking", "Base Upgrade Count", 3, "How many items to upgrade at a single stack.").Value;
			Config_StackCount = Config.Bind("Stacking", "Stack Upgrade Count", 3, "How many items to upgrade for each additional stack.").Value;
			Config_BlackList = Config.Bind("Upgrading", "Upgrade Blacklist", "", "Prevents upgrading into these specific items. (Example = Clover, ExtraLife, MoreMissile)").Value;
			Config_WholeStack = Config.Bind("Upgrading", "Upgrade Entire Stack", true, "Should the entire stack get upgraded or a single instance of the selected item?").Value;
			Config_Select_Method = Config.Bind("Item Selection", "Selection Method", SelectMethod.Random, "The method to use when selecting items for upgrading.").Value;
			Config_Select_PreferScrap = Config.Bind("Item Selection", "Prioritise Scrap", false, "Should Scrap have priority over the selection method?").Value;
			Config_Select_AllowVoid = Config.Bind("Item Selection", "Allow Void", false, "Allow Void items to be selected for upgrading?").Value;
			Config_Select_AllowSelf = Config.Bind("Item Selection", "Allow Self", false, "Allow Benthic Bloom to be selected for upgrading?").Value;
			Config_Overspill_Enable = Config.Bind("Overflow", "Enable", false, "Allows Legendary items to be selected and 'upgraded' into Common items.").Value;
			Config_Overspill_Bonus = Config.Bind("Overflow", "Bonus Gain", OverspillBonus.PlusOne, "Gain extra items when 'upgrading' an item.").Value;
		}
	}
}
