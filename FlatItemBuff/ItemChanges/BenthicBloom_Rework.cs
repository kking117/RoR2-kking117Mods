using System;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class BenthicBloom_Rework
	{
		private static List<ItemDef> BanList;
		private static List<ItemTier> TierChance;
		private static bool BuffStats = false;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Benthic Bloom");
			UpdateText();
			Hooks();
			EditItem();
		}
		private static void EditItem()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion();
			if (itemDef)
			{
				itemDef.pickupToken = MainPlugin.MODTOKEN + "ITEM_CLOVERVOID_PICKUP";
				itemDef.descriptionToken = MainPlugin.MODTOKEN + "ITEM_CLOVERVOID_DESC";
			}
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");

			string desc_stats = "";
			string pickup_stats = "";
			List<string> desc_statlist = new List<string>();
			List<string> pickup_statlist = new List<string>();

			if (MainPlugin.BenthicRework_BuffDamage.Value)
			{
				desc_statlist.Add("<style=cIsDamage>damage</style>");
				pickup_statlist.Add("damage");
			}
			if (MainPlugin.BenthicRework_BuffHealth.Value)
			{
				desc_statlist.Add("<style=cIsHealing>health</style>");
				pickup_statlist.Add("health");
			}
			if (MainPlugin.BenthicRework_BuffSpeed.Value)
			{
				desc_statlist.Add("<style=cIsUtility>movement speed</style>");
				pickup_statlist.Add("movement speed");
			}

			for (int i = 0; i < desc_statlist.Count; i++)
			{
				desc_stats += desc_statlist[i];
				pickup_stats += pickup_statlist[i];
				if (i == desc_statlist.Count - 2)
				{
					desc_stats += " and ";
					pickup_stats += " and ";
				}
				else if (i < desc_statlist.Count - 1)
				{
					desc_stats += ", ";
					pickup_stats += ", ";
				}
			}

			string pickup = "<style=cIsVoid>Corrupts</style> your items at the start of each stage.";
			string desc = String.Format("At the start of each stage, <style=cIsVoid>corrupt {0}</style> <style=cStack>(+{1} per stack)</style> random items into a <style=cIsVoid>random Void item</style> of the <style=cIsUtility>same rarity</style>.", MainPlugin.BenthicRework_BaseCount.Value, MainPlugin.BenthicRework_StackCount.Value);
			if (desc_stats.Length > 0)
			{
				pickup_stats = String.Format(" Increases " + pickup_stats + " for each <style=cIsVoid>Void item</style> you have.");
				pickup += pickup_stats;
				desc_stats = String.Format(" Increases " + desc_stats + " by <style=cIsDamage>{0}%</style> for each <style=cIsVoid>Void item</style> you have.", MainPlugin.BenthicRework_VoidManBonus.Value * 100f);
				desc += desc_stats;
				BuffStats = true;
			}
			LanguageAPI.Add(MainPlugin.MODTOKEN + "ITEM_CLOVERVOID_PICKUP", pickup);
			LanguageAPI.Add(MainPlugin.MODTOKEN + "ITEM_CLOVERVOID_DESC", desc);
		}
		private static void Hooks()
		{
			On.RoR2.ItemCatalog.Init += ItemCatalog_Init;
			On.RoR2.CharacterMaster.TryCloverVoidUpgrades += CloverVoidUpgrade;
			On.RoR2.ItemCatalog.SetItemRelationships += SetItemRelationships;
			if (BuffStats)
			{
				RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			}
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(DLC1Content.Items.CloverVoid);
				if (itemCount > 0)
				{
					int voidCount = sender.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier1) + sender.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier2) + sender.inventory.GetTotalItemCountOfTier(ItemTier.VoidTier3) + sender.inventory.GetTotalItemCountOfTier(ItemTier.VoidBoss);
					if (voidCount > 0)
					{
						float statbonus = HyperbolicResult(voidCount, 1);
						if (MainPlugin.BenthicRework_BuffSpeed.Value)
						{
							args.moveSpeedMultAdd += statbonus;
						}
						if (MainPlugin.BenthicRework_BuffHealth.Value)
						{
							args.healthMultAdd += statbonus;
						}
						if (MainPlugin.BenthicRework_BuffDamage.Value)
						{
							args.damageMultAdd += statbonus;
						}
					}
				}
			}
		}
		private static float HyperbolicResult(int itemCount, int hardCap)
		{
			float result = hardCap - hardCap / (1 + MainPlugin.BenthicRework_VoidManBonus.Value * itemCount);
			return result;
		}
		private static void ItemCatalog_Init(On.RoR2.ItemCatalog.orig_Init orig)
		{
			orig();
			BanList = new List<ItemDef>();
			string[] items = MainPlugin.BenthicRework_BanList.Value.Split(',');
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
			DLC1Content.Items.CloverVoid._itemTierDef = ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier1);
			SetupChanceTable();
		}
		private static void SetupChanceTable()
        {
			TierChance = new List<ItemTier>();
			ItemTier tierIndex = ItemTier.Tier1;
			string[] tiers = MainPlugin.BenthicRework_TierFavour.Value.Split(',');
			for (int i = 0; i < tiers.Length; i++)
			{
				tiers[i] = tiers[i].Trim();
				int total = int.Parse(tiers[i]);

				for (int z = 0; z < total; z++)
				{
					TierChance.Add(tierIndex);
				}

				if (tierIndex == ItemTier.Tier1)
                {
					tierIndex = ItemTier.Tier2;
				}
				else if (tierIndex == ItemTier.Tier2)
				{
					tierIndex = ItemTier.Tier3;
				}
				else if (tierIndex == ItemTier.Tier3)
				{
					tierIndex = ItemTier.Boss;
				}
				else if (tierIndex == ItemTier.Boss)
				{
					break;
				}
			}
		}
		private static void SetItemRelationships(On.RoR2.ItemCatalog.orig_SetItemRelationships orig, ItemRelationshipProvider[] providers)
		{
			List<ItemRelationshipProvider> newprovider = new List<ItemRelationshipProvider>();
			foreach (ItemRelationshipProvider itemrelation in providers)
			{
				if (itemrelation.relationshipType == DLC1Content.ItemRelationshipTypes.ContagiousItem)
				{
					List<ItemDef.Pair> newitempair = new List<ItemDef.Pair>();
					foreach (ItemDef.Pair itempair in itemrelation.relationships)
					{
						if (itempair.itemDef2 != DLC1Content.Items.CloverVoid)
						{
							newitempair.Add(itempair);
						}
					}
					itemrelation.relationships = newitempair.ToArray();
				}
				newprovider.Add(itemrelation);
			}
			orig(newprovider.ToArray());
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
				int upgradeCount = MainPlugin.BenthicRework_BaseCount.Value + ((itemCount - 1) * MainPlugin.BenthicRework_StackCount.Value);
				if (upgradeCount > 0)
				{
					List<ItemIndex> Tier1List = DropListToRandomCorruptList(Run.instance.availableVoidTier1DropList);
					List<ItemIndex> Tier2List = DropListToRandomCorruptList(Run.instance.availableVoidTier2DropList);
					List<ItemIndex> Tier3List = DropListToRandomCorruptList(Run.instance.availableVoidTier3DropList);
					List<ItemIndex> TierBList = DropListToRandomCorruptList(Run.instance.availableVoidBossDropList);

					List<ItemIndex> itemList = GetSortedInventory(self);

					int hitCount = 0;
					int i = 0;
					List<ItemIndex> ConvertList = new List<ItemIndex>();
					while (hitCount < upgradeCount && i < itemList.Count)
					{
						ItemDef itemDef = ItemCatalog.GetItemDef(itemList[i]);
						ItemDef newitemDef = null;
						List<ItemIndex> UpgradeList = null;
						if (itemDef.tier == ItemTier.Boss)
						{
							UpgradeList = TierBList;
						}
						else if (itemDef.tier == ItemTier.Tier3)
						{
							UpgradeList = Tier3List;
						}
						else if (itemDef.tier == ItemTier.Tier2)
						{
							UpgradeList = Tier2List;
						}
						else if (itemDef.tier == ItemTier.Tier1)
						{
							UpgradeList = Tier1List;
						}
						if (UpgradeList != null && UpgradeList.Count > 0)
						{
							Util.ShuffleList<ItemIndex>(UpgradeList, self.cloverVoidRng);
							newitemDef = ItemCatalog.GetItemDef(UpgradeList[0]);
						}
						if (newitemDef != null)
						{
							hitCount++;
							i = -1;
							self.inventory.GiveItem(newitemDef, 1);
							self.inventory.RemoveItem(itemDef, 1);
							CharacterMasterNotificationQueue.PushItemTransformNotification(self, itemDef.itemIndex, newitemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.CloverVoid);
							itemList = GetSortedInventory(self);
						}
						i++;
					}
				}
			}
		}
		private static List<ItemIndex> GetSortedInventory(CharacterMaster master)
		{
			if (master.cloverVoidRng == null)
			{
				master.cloverVoidRng = new Xoroshiro128Plus(Run.instance.seed);
			}
			List<ItemIndex> itemList = new List<ItemIndex>(master.inventory.itemAcquisitionOrder);
			itemList = RemoveClutter(itemList);
			Util.ShuffleList<ItemIndex>(itemList, master.cloverVoidRng);
			switch (MainPlugin.BenthicRework_SortMethod.Value)
			{
				case 1:
					itemList.Sort(SortByTier);
					break;
				case 2:
					itemList = RandomlyHighTier(itemList, master.cloverVoidRng);
					break;
			}

			return itemList;
		}
		private static List<ItemIndex> RemoveClutter(List<ItemIndex> itemList)
		{
			for (int i = 0; i < itemList.Count; i++)
			{
				if (!CanBeCorrupted(itemList[i]))
				{
					itemList.RemoveAt(i);
					i--;
				}
			}
			return itemList;
		}
		private static List<ItemIndex> DropListToRandomCorruptList(List<PickupIndex> pickupList)
		{
			List<ItemIndex> itemList = new List<ItemIndex>();
			for (int i = 0; i < pickupList.Count; i++)
			{
				ItemDef def = ItemCatalog.GetItemDef(pickupList[i].itemIndex);
				if (BanList == null || !BanList.Contains(def))
				{
					itemList.Add(pickupList[i].itemIndex);
				}
			}
			return itemList;
		}
		private static bool CanBeCorrupted(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			if (itemDef.tier == ItemTier.Tier1)
			{
				return true;
			}
			if (itemDef.tier == ItemTier.Tier2)
			{
				return true;
			}
			if (itemDef.tier == ItemTier.Tier3)
			{
				return true;
			}
			if (itemDef.tier == ItemTier.Boss)
			{
				return true;
			}
			return false;
		}
		private static List<ItemIndex> RandomlyHighTier(List<ItemIndex> itemList, Xoroshiro128Plus seed)
		{
			Util.ShuffleList<ItemTier>(TierChance, seed);
			ItemTier priority = TierChance[0];

			itemList.Sort(SortByTierPriority);

			int SortByTierPriority(ItemIndex lhs, ItemIndex rhs)
			{
				int ScoreA = 0;
				int ScoreB = 0;
				ItemDef itemDefA = ItemCatalog.GetItemDef(lhs);
				ItemDef itemDefB = ItemCatalog.GetItemDef(rhs);
				if (itemDefA.tier == priority)
				{
					ScoreA = 5;
				}
				else
				{
					switch (itemDefA.tier)
					{
						case ItemTier.Boss:
							ScoreA = 1;
							break;
						case ItemTier.Tier3:
							ScoreA = 2;
							break;
						case ItemTier.Tier2:
							ScoreA = 3;
							break;
						case ItemTier.Tier1:
							ScoreA = 4;
							break;
					}
				}
				if (itemDefB.tier == priority)
				{
					ScoreB = 5;
				}
				else
                {
					switch (itemDefB.tier)
					{
						case ItemTier.Boss:
							ScoreB = 1;
							break;
						case ItemTier.Tier3:
							ScoreB = 2;
							break;
						case ItemTier.Tier2:
							ScoreB = 3;
							break;
						case ItemTier.Tier1:
							ScoreB = 4;
							break;
					}
				}
				return ScoreB - ScoreA;
			}
			return itemList;
		}
		private static int SortByTier(ItemIndex lhs, ItemIndex rhs)
		{
			int ScoreA = 0;
			int ScoreB = 0;
			ItemDef itemDefA = ItemCatalog.GetItemDef(lhs);
			ItemDef itemDefB = ItemCatalog.GetItemDef(rhs);
			switch(itemDefA.tier)
            {
				case ItemTier.Boss:
					ScoreA = 1;
					break;
				case ItemTier.Tier3:
					ScoreA = 2;
					break;
				case ItemTier.Tier2:
					ScoreA = 3;
					break;
				case ItemTier.Tier1:
					ScoreA = 4;
					break;
			}
			switch (itemDefB.tier)
			{
				case ItemTier.Boss:
					ScoreB = 1;
					break;
				case ItemTier.Tier3:
					ScoreB = 2;
					break;
				case ItemTier.Tier2:
					ScoreB = 3;
					break;
				case ItemTier.Tier1:
					ScoreB = 4;
					break;
			}
			return ScoreB - ScoreA;
		}
	}
}
