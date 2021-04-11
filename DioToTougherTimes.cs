using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace DioToTougherTimes
{
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("com.kking117.DioToTougherTimes", "DioToTougherTimes", "2.0.0")]

	public class MainPlugin : BaseUnityPlugin
	{
		private bool Converted = false;
		public static ConfigEntry <string> Config_SpecificItems;
		private ItemIndex[] Config_Array_SpecificItems;
		public static ConfigEntry<int> Config_ItemTier;
		public static ConfigEntry<int> Config_GiveAmount;
		public static ConfigEntry<bool> Config_GiveDiff;
		public static ConfigEntry<bool> Config_NoLoop;
		public static ConfigEntry<bool> Config_Debug;

		public void Awake()
		{
			ReadConfig();
			On.RoR2.Inventory.RpcItemAdded += Inventory_OnItemAdded;
			On.RoR2.Run.Start += Run_Start;
		}
		private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
		{
			orig(self);
			if (!Converted)
			{
				ConvertSpecItemsToArray();
				Converted = true;
			}
		}
		public ItemIndex GetRandomItemFromTier(int tierlist)
		{
			Run run = Run.instance;
			PickupIndex result = PickupIndex.none;
			if (run)
			{
				PickupIndex[] items;
				switch (tierlist)
				{
					case 1:
						items = run.availableTier1DropList.ToArray();
						if(Config_NoLoop.Value)
                        {
							items = RemoveDioFromList(items);
						}
						if (items.Length > 0)
						{
							result = items[UnityEngine.Random.Range(0, items.Length)];
						}
						break;
					case 2:
						items = run.availableTier2DropList.ToArray();
						if (Config_NoLoop.Value)
						{
							items = RemoveDioFromList(items);
						}
						if (items.Length > 0)
						{
							result = items[UnityEngine.Random.Range(0, items.Length)];
						}
						break;
					case 3:
						items = run.availableTier3DropList.ToArray();
						if (Config_NoLoop.Value)
						{
							items = RemoveDioFromList(items);
						}
						if (items.Length > 0)
						{
							result = items[UnityEngine.Random.Range(0, items.Length)];
						}
						break;
					case 4:
						items = run.availableBossDropList.ToArray();
						if (Config_NoLoop.Value)
						{
							items = RemoveDioFromList(items);
						}
						if (items.Length > 0)
						{
							result = items[UnityEngine.Random.Range(0, items.Length)];
						}
						break;
					case 5:
						items = run.availableLunarDropList.ToArray();
						if (Config_NoLoop.Value)
						{
							items = RemoveDioFromList(items);
						}
						if (items.Length > 0)
						{
							result = items[UnityEngine.Random.Range(0, items.Length)];
						}
						break;
					case 6:
						if (Config_Array_SpecificItems.Length > 0)
						{
							return Config_Array_SpecificItems[UnityEngine.Random.Range(0, Config_Array_SpecificItems.Length)];
						}
						break;
				}
			}
			if (result != PickupIndex.none)
			{
				ItemIndex endresult = PickupCatalog.GetPickupDef(result).itemIndex;
				return endresult;
			}
			else
            {
				return ItemIndex.None;
            }
		}

		private PickupIndex[] RemoveDioFromList(PickupIndex[] thelist)
        {
			for(int i=0; i<thelist.Length; i++)
            {
				if(thelist[i]!=PickupIndex.none)
                {
					ItemIndex item = PickupCatalog.GetPickupDef(thelist[i]).itemIndex;
					if(item == RoR2Content.Items.ExtraLife.itemIndex)
                    {
						thelist[i] = PickupIndex.none;
                    }
				}
            }
			
			PickupIndex[] returnlist = new PickupIndex[0];
			for (int i = 0; i < thelist.Length; i++)
			{
				if (thelist[i] != PickupIndex.none)
				{
					Array.Resize<PickupIndex>(ref returnlist, returnlist.Length + 1);
					returnlist[returnlist.Length - 1] = thelist[i];
				}
			}
			return returnlist;
        }

		private void Inventory_OnItemAdded(On.RoR2.Inventory.orig_RpcItemAdded orig, Inventory self, ItemIndex item)
		{
			orig(self, item);
			if (item == RoR2Content.Items.ExtraLifeConsumed.itemIndex)
			{
				for (int i = 0; i < self.GetItemCount(RoR2Content.Items.ExtraLifeConsumed.itemIndex);)
				{
					if (Config_GiveDiff.Value)
					{
						for (int f = 0; f < Config_GiveAmount.Value; f++)
						{
							ItemIndex givenitem = GetRandomItemFromTier(Config_ItemTier.Value);
							self.GiveItem(givenitem, 1);
							if (Config_Debug.Value)
							{
								 Logger.LogInfo("Gained " + ItemCatalog.GetItemDef(givenitem).name + " from consumed Dio.");
							}
						}
						self.GiveItem(RoR2Content.Items.ExtraLifeConsumed.itemIndex, -1);
					}
					else
					{
						ItemIndex givenitem = GetRandomItemFromTier(Config_ItemTier.Value);
						self.GiveItem(givenitem, Config_GiveAmount.Value);
						self.GiveItem(RoR2Content.Items.ExtraLifeConsumed.itemIndex, -1);
						if (Config_Debug.Value)
						{
							 Logger.LogInfo("Gained " + Config_GiveAmount.Value + "x " + ItemCatalog.GetItemDef(givenitem).name + " from consumed Dio.");
						}
					}
				}
			}
		}

		private void ReadConfig()
        {
			Config_SpecificItems = Config.Bind<string>(new ConfigDefinition("DioToTougherTimes", "Given Items"), "Bear", new ConfigDescription("Custom list of items that a consumed Dio can randomly give. (Example: 'Bear, Syringe, Dagger' = TougherTimes, Soldier Syringe and Ceremonial Dagger.)", null, Array.Empty<object>()));
			Config_ItemTier = Config.Bind<int>(new ConfigDefinition("DioToTougherTimes", "Item Tier"), 6, new ConfigDescription("What tier to randomly select an item from. (1 = white, 2 = green, 3 = red, 4 = boss, 5 = lunar, 6 = custom list)", null, Array.Empty<object>()));
			Config_GiveAmount = Config.Bind<int>(new ConfigDefinition("DioToTougherTimes", "Item Amount"), 1, new ConfigDescription("How many items to give per consumed Dio.", null, Array.Empty<object>()));
			Config_GiveDiff = Config.Bind<bool>(new ConfigDefinition("DioToTougherTimes", "Randomize Each Item"), true, new ConfigDescription("Items given will be randomly selected each instead of giving X amount of a random item.", null, Array.Empty<object>()));
			Config_NoLoop = Config.Bind<bool>(new ConfigDefinition("DioToTougherTimes", "No Infinite"), true, new ConfigDescription("Removes Dio's Best Friend from being randomly selected from all but custom lists.", null, Array.Empty<object>()));
			Config_Debug = Config.Bind<bool>(new ConfigDefinition("DioToTougherTimes", "Spam Console"), false, new ConfigDescription(" Logger.LogInfos console messages for debugging purposes.", null, Array.Empty<object>()));
		}

		private void ConvertSpecItemsToArray()
        {
			if (Config_Debug.Value)
			{
				 Logger.LogInfo("Building Custom list.");
			}
			string[] items = Config_SpecificItems.Value.Split(',');
			Config_Array_SpecificItems = new ItemIndex[0];
			for (int i = 0; i < items.GetLength(0); i++)
			{
				if (!items[i].IsNullOrWhiteSpace())
				{
					items[i] = items[i].Trim();
					ItemIndex item = ItemCatalog.FindItemIndex(items[i]);
					if (item != ItemIndex.None)
					{
						Array.Resize<ItemIndex>(ref Config_Array_SpecificItems, Config_Array_SpecificItems.Length + 1);
						Config_Array_SpecificItems[Config_Array_SpecificItems.Length-1] = item;
						if (Config_Debug.Value)
						{
							 Logger.LogInfo("Added " + items[i] + " to Custom list.");
						}
					}
				}
			}
			if (Config_Debug.Value)
			{
				 Logger.LogInfo("Custom list built with a total of " + Config_Array_SpecificItems.Length + " items.");
			}
		}
	}
}
