using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace DioToTougherTimes
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("com.kking117.DioToTougherTimes", "DioToTougherTimes", "1.0.1")]

	public class DioToTougherTimes : BaseUnityPlugin
	{
		private int[] tierlengths = new int[6];
		private ItemIndex[] t1list = new ItemIndex[300];
		private ItemIndex[] t2list = new ItemIndex[300];
		private ItemIndex[] t3list = new ItemIndex[300];
		private ItemIndex[] tblist = new ItemIndex[300];
		private ItemIndex[] tylist = new ItemIndex[300];
		private ItemIndex[] tnlist = new ItemIndex[300];
		public void Awake()
		{
			//generate tier lists manually
			//for my brain's sake
			On.RoR2.Run.Start += delegate (On.RoR2.Run.orig_Start orig, Run self)
			{
				orig.Invoke(self);
				CleanLists();
				for (int i = 0; i < ItemCatalog.itemCount; i++)
				{
					ItemDef iteminfo = ItemCatalog.GetItemDef((ItemIndex)i);
					if (ValidItemTags(iteminfo))
                    {
						if (!dioconsumednoloop || (ItemIndex)i != ItemIndex.ExtraLife)
						{
							switch (iteminfo.tier)
							{
								case ItemTier.Tier1:
									tierlengths[0]++;
									t1list[tierlengths[0]] = (ItemIndex)i;
									break;
								case ItemTier.Tier2:
									tierlengths[1]++;
									t2list[tierlengths[1]] = (ItemIndex)i;
									break;
								case ItemTier.Tier3:
									tierlengths[2]++;
									t3list[tierlengths[2]] = (ItemIndex)i;
									break;
								case ItemTier.Lunar:
									tierlengths[3]++;
									tblist[tierlengths[3]] = (ItemIndex)i;
									break;
								case ItemTier.Boss:
									tierlengths[4]++;
									tylist[tierlengths[4]] = (ItemIndex)i;
									break;
								case ItemTier.NoTier:
									tierlengths[5]++;
									tnlist[tierlengths[5]] = (ItemIndex)i;
									break;
							}
						}
					}
				}
				if (spamconsole)
				{
					MonoBehaviour.print("DioToTougherTimes - Item Tiers recorded for this run.");
				}
			};

			void CleanLists()
            {
				for (int i = 0; i < 300; i++)
                {
					t1list[i] = ItemIndex.None;
					t2list[i] = ItemIndex.None;
					t3list[i] = ItemIndex.None;
					tblist[i] = ItemIndex.None;
					tylist[i] = ItemIndex.None;
					tnlist[i] = ItemIndex.None;
				}
				tierlengths[0] = -1;
				tierlengths[1] = -1;
				tierlengths[2] = -1;
				tierlengths[3] = -1;
				tierlengths[4] = -1;
				tierlengths[5] = -1;
			}

			bool ValidItemTags(ItemDef iteminfo)
            {
				bool result = true;
				bool IsScrap = false;
				bool IsUnique = false;
				ItemTag[] itemtags = iteminfo.tags;
				if (itemtags.Length > 0)
				{
					for (int i = 0; i < itemtags.Length; i++)
					{
						if (itemtags[i] == ItemTag.Scrap)
						{
							IsScrap = true;
						}
						else if (itemtags[i] == ItemTag.WorldUnique)
						{
							IsUnique = true;
						}
					}
				}
				if(IsScrap)
				{
					if (dioconsumednoscrap)
					{
						result = false;
					}
				}
				else if(IsUnique)
                {
					if (dioconsumednounique)
					{
						result = false;
					}
                }
				return result;
            }

			ItemIndex GetRandomFromTier(int tierlist)
            {
				ItemIndex result = ItemIndex.None;
				int length = ItemCatalog.itemCount;
				switch (dioconsumedtier)
				{
					case 0:
						result = (ItemIndex)UnityEngine.Random.Range(0, ItemCatalog.itemCount);
						break;
					case 1:
						result = (ItemIndex)t1list[UnityEngine.Random.Range(0, tierlengths[0]+1)];
						break;
					case 2:
						result = (ItemIndex)t2list[UnityEngine.Random.Range(0, tierlengths[1]+1)];
						break;
					case 3:
						result = (ItemIndex)t3list[UnityEngine.Random.Range(0, tierlengths[2]+1)];
						break;
					case 4:
						result = (ItemIndex)tblist[UnityEngine.Random.Range(0, tierlengths[3]+1)];
						break;
					case 5:
						result = (ItemIndex)tylist[UnityEngine.Random.Range(0, tierlengths[4]+1)];
						break;
					case 6:
						result = (ItemIndex)tnlist[UnityEngine.Random.Range(0, tierlengths[5]+1)];
						break;
				}
				return result;
			}
			On.RoR2.Inventory.RpcItemAdded += delegate (On.RoR2.Inventory.orig_RpcItemAdded orig, Inventory self, ItemIndex item)
			{
				if (item == ItemIndex.ExtraLifeConsumed)
                {
					ItemIndex givenitem = dioconsumeditem;
					if (dioconsumeditem == ItemIndex.None)
					{
						for (int i = 0; i < self.GetItemCount(ItemIndex.ExtraLifeConsumed);)
                        {
							if (dioconsumedunique)
							{
								for (int f = 0; f < dioconsumedamount; f++)
								{
									givenitem = GetRandomFromTier(dioconsumedtier);
									self.GiveItem((ItemIndex)givenitem, 1);
									if (spamconsole)
									{
										MonoBehaviour.print("DioToTougherTimes - Gained " + givenitem + " from consumed Dio.");
									}
								}
								self.GiveItem(ItemIndex.ExtraLifeConsumed, -1);
							}
							else
							{
								givenitem = GetRandomFromTier(dioconsumedtier);
								self.GiveItem((ItemIndex)givenitem, dioconsumedamount);
								self.GiveItem(ItemIndex.ExtraLifeConsumed, -1);
								if (spamconsole)
								{
									MonoBehaviour.print("DioToTougherTimes - Gained " + dioconsumedamount + "x " + givenitem + " from consumed Dio.");
								}
							}
						}
					}
					else
                    {
						int diomult = self.GetItemCount(ItemIndex.ExtraLifeConsumed);
						self.GiveItem((ItemIndex)givenitem, dioconsumedamount*self.GetItemCount(ItemIndex.ExtraLifeConsumed));
						self.GiveItem(ItemIndex.ExtraLifeConsumed, diomult*-1);
						if (spamconsole)
						{
							MonoBehaviour.print("DioToTougherTimes - Gained " + dioconsumedamount*diomult + "x " + givenitem + " from consumed Dio.");
						}
					}
					
				}
				orig(self, item);
			};
		}

		public static ConfigFile CustomConfigFile = new ConfigFile(Paths.ConfigPath + "\\kking117.DioToTougherTimes.cfg", true);

		public static ConfigEntry<ItemIndex> dioconsumeditem_config = CustomConfigFile.Bind<ItemIndex>("DioToTougherTimes Config", "Given Item", ItemIndex.Bear, "Identifier of the item to give to the player when removing Consumed Dios.(Set to ''None'' to get a random item in the configured tier.)");
		public ItemIndex dioconsumeditem = dioconsumeditem_config.Value;

		public static ConfigEntry<int> dioconsumedamount_config = CustomConfigFile.Bind<int>("DioToTougherTimes Config", "Item Amount", 1, "How many of the selected item/tier to give per Consumed Dio.");
		public int dioconsumedamount = dioconsumedamount_config.Value;

		public static ConfigEntry<int> dioconsumedtier_config = CustomConfigFile.Bind<int>("DioToTougherTimes Config", "Item Tier", 1, "What Tier to randomly select an item from when ''Given Item'' is -1 or less. (0 = anything tier even no tier, 1 = white, 2 = green, 3 = red, 4 = lunar, 5 = boss, 6 = no tier)");
		public int dioconsumedtier = dioconsumedtier_config.Value;

		public static ConfigEntry<bool> dioconsumedunique_config = CustomConfigFile.Bind<bool>("DioToTougherTimes Config", "Randomize Each Item", true, "When enabled all items given will be randomly selected instead of giving X amount of a random item.");
		public bool dioconsumedunique = dioconsumedunique_config.Value;

		public static ConfigEntry<bool> dioconsumednoscrap_config = CustomConfigFile.Bind<bool>("DioToTougherTimes Config", "No Scrap", true, "Will blacklist Scrap from being randomly given.");
		public bool dioconsumednoscrap = dioconsumednoscrap_config.Value;

		public static ConfigEntry<bool> dioconsumednoloop_config = CustomConfigFile.Bind<bool>("DioToTougherTimes Config", "No Infinite", true, "Will blacklist Dio's Best Friend from being randomly given.");
		public bool dioconsumednoloop = dioconsumednoloop_config.Value;

		public static ConfigEntry<bool> dioconsumednounique_config = CustomConfigFile.Bind<bool>("DioToTougherTimes Config", "No Unique", true, "Will blacklist any unique items from being randomly given. (Vanilla items tagged as unique are: Halcyon Seed, Artifact Key, Pearl, Irradiant Pearl and Microbots)");
		public bool dioconsumednounique = dioconsumednounique_config.Value;

		public static ConfigEntry<bool> spamconsole_config = CustomConfigFile.Bind<bool>("DioToTougherTimes Config", "Spam Console", false, "Enables all console debugging messages.");
		public bool spamconsole = spamconsole_config.Value;
	}
}
