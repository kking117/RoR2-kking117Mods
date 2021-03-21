using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace ItemPoolRestriction
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.kking117.ItemPoolLimiter", "ItemPoolLimiter", "1.0.0")]
    //yeah a name change, restriction didn't sound right
    public class ItemPoolRestriction : BaseUnityPlugin
    {
        private PickupIndex[,] T1_List = new PickupIndex[3, 100];
        private PickupIndex[,] T2_List = new PickupIndex[3, 100];
        private PickupIndex[,] T3_List = new PickupIndex[3, 100];
        private PickupIndex[] TB_List = new PickupIndex[300];
        private PickupIndex[] TL_List = new PickupIndex[300];
        private PickupIndex[] TEquip_List = new PickupIndex[300];
        private PickupIndex[] LEquip_List = new PickupIndex[300];
        private int[] List_Cat_T1 = new int[3];
        private int[] List_Cat_T2 = new int[3];
        private int[] List_Cat_T3 = new int[3];
        private int[] List_Max = new int[4];
        private bool BuiltList = false;

        public static ConfigEntry<float> DamageCent; //0.36
        public static ConfigEntry<float> UtilityCent; //0.36
        public static ConfigEntry<float> HealCent; //0.28

        public static ConfigEntry<int> MaxT1; //24
        public static ConfigEntry<int> MaxT2; //25
        public static ConfigEntry<int> MaxT3; //20
        public static ConfigEntry<int> MaxTB; //7
        public static ConfigEntry<int> MaxTL; //12
        public static ConfigEntry<int> MaxEquip; //21
        public static ConfigEntry<int> MaxLEquip; //4
        public static ConfigEntry<bool> DumpList; //false
        public void Awake()
        {
            ReadConfig();

            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                if (!BuiltList)
                {
                    BuildEntireTierLists();
                }
                GenerateNewRunList(self);
            };

            void BuildEntireTierLists()
            {
                for (int i = 0; i < ItemCatalog.itemCount; i++)
                {
                    ItemDef iteminfo = ItemCatalog.GetItemDef((ItemIndex)i);
                    if (ValidItemTags(iteminfo))
                    {
                        PickupIndex index = PickupCatalog.FindPickupIndex((ItemIndex)i);
                        ItemTag itemtype = GetItemType(iteminfo);
                        switch (iteminfo.tier)
                        {
                            case ItemTier.Tier1:
                                switch (itemtype)
                                {
                                    case ItemTag.Damage:
                                        T1_List[0, List_Cat_T1[0]] = index;
                                        List_Cat_T1[0] += 1;
                                        break;
                                    case ItemTag.Utility:
                                        T1_List[1, List_Cat_T1[1]] = index;
                                        List_Cat_T1[1] += 1;
                                        break;
                                    case ItemTag.Healing:
                                        T1_List[2, List_Cat_T1[2]] = index;
                                        List_Cat_T1[2] += 1;
                                        break;
                                }
                                break;
                            case ItemTier.Tier2:
                                switch (itemtype)
                                {
                                    case ItemTag.Damage:
                                        T2_List[0, List_Cat_T2[0]] = index;
                                        List_Cat_T2[0] += 1;
                                        break;
                                    case ItemTag.Utility:
                                        T2_List[1, List_Cat_T2[1]] = index;
                                        List_Cat_T2[1] += 1;
                                        break;
                                    case ItemTag.Healing:
                                        T2_List[2, List_Cat_T2[2]] = index;
                                        List_Cat_T2[2] += 1;
                                        break;
                                }
                                break;
                            case ItemTier.Tier3:
                                switch (itemtype)
                                {
                                    case ItemTag.Damage:
                                        T3_List[0, List_Cat_T3[0]] = index;
                                        List_Cat_T3[0] += 1;
                                        break;
                                    case ItemTag.Utility:
                                        T3_List[1, List_Cat_T3[1]] = index;
                                        List_Cat_T3[1] += 1;
                                        break;
                                    case ItemTag.Healing:
                                        T3_List[2, List_Cat_T3[2]] = index;
                                        List_Cat_T3[2] += 1;
                                        break;
                                }
                                break;
                            case ItemTier.Boss:
                                TB_List[List_Max[0]] = index;
                                List_Max[0] += 1;
                                break;
                            case ItemTier.Lunar:
                                TL_List[List_Max[1]] = index;
                                List_Max[1] += 1;
                                break;
                        }
                    }
                }
                for (int i = 0; i < EquipmentCatalog.equipmentCount; i++)
                {
                    EquipmentDef equipinfo = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i);
                    if (equipinfo.canDrop)
                    {
                        PickupIndex index = PickupCatalog.FindPickupIndex((EquipmentIndex)i);
                        if (equipinfo.isLunar)
                        {
                            LEquip_List[List_Max[3]] = index;
                            List_Max[3] += 1;
                        }
                        else
                        {
                            TEquip_List[List_Max[2]] = index;
                            List_Max[2] += 1;
                        }
                    }
                }
                BuiltList = false;
            }

            void BuildWhitePool(Run run)
            {
                int actualmax = 0;
                print("-=WHITE=-");
                int maxitem = MaxT1.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Cat_T1[0] + List_Cat_T1[1] + List_Cat_T1[2];
                }
                PickupIndex[] new_t1 = new PickupIndex[maxitem];
                GetNewItemsForTierCat(new_t1, T1_List, maxitem, List_Cat_T1);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_t1[i] != PickupIndex.none)
                    {
                        run.availableTier1DropList.Add(new_t1[i]);
                        if (DumpList.Value)
                        {
                            print(new_t1[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + (List_Cat_T1[0] + List_Cat_T1[1] + List_Cat_T1[2]));
            }
            void BuildGreenPool(Run run)
            {
                int actualmax = 0;
                print("-=GREEN=-");
                int maxitem = MaxT2.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Cat_T2[0] + List_Cat_T2[1] + List_Cat_T2[2];
                }
                PickupIndex[] new_t2 = new PickupIndex[maxitem];
                GetNewItemsForTierCat(new_t2, T2_List, maxitem, List_Cat_T2);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_t2[i] != PickupIndex.none)
                    {
                        run.availableTier2DropList.Add(new_t2[i]);
                        if (DumpList.Value)
                        {
                            print(new_t2[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + (List_Cat_T2[0] + List_Cat_T2[1] + List_Cat_T2[2]));
            }
            void BuildRedPool(Run run)
            {
                int actualmax = 0;
                int maxitem = MaxT3.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Cat_T3[0] + List_Cat_T3[1] + List_Cat_T3[2];
                }
                print("-=RED=-");
                PickupIndex[] new_t3 = new PickupIndex[maxitem];
                GetNewItemsForTierCat(new_t3, T3_List, maxitem, List_Cat_T3);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_t3[i] != PickupIndex.none)
                    {
                        run.availableTier3DropList.Add(new_t3[i]);
                        if (DumpList.Value)
                        {
                            print(new_t3[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + (List_Cat_T3[0] + List_Cat_T3[1] + List_Cat_T3[2]));
            }
            void BuildBossPool(Run run)
            {
                int actualmax = 0;
                int maxitem = MaxTB.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Max[0];
                }
                print("--BOSS--");
                PickupIndex[] new_tb = new PickupIndex[maxitem];
                GetNewItemsForTier(new_tb, TB_List, maxitem, List_Max[0]);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_tb[i] != PickupIndex.none)
                    {
                        run.availableBossDropList.Add(new_tb[i]);
                        if (DumpList.Value)
                        {
                            print(new_tb[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + List_Max[0]);
            }
            void BuildLunarPool(Run run)
            {
                int actualmax = 0;
                int maxitem = MaxTL.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Max[1];
                }
                print("--LUNAR--");
                PickupIndex[] new_tl = new PickupIndex[maxitem];
                GetNewItemsForTier(new_tl, TL_List, maxitem, List_Max[1]);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_tl[i] != PickupIndex.none)
                    {
                        run.availableLunarDropList.Add(new_tl[i]);
                        if (DumpList.Value)
                        {
                            print(new_tl[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + List_Max[1]);
            }
            void BuildEquipPool(Run run)
            {
                int actualmax = 0;
                int maxitem = MaxEquip.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Max[2];
                }
                print("--EQUIPMENT--");
                PickupIndex[] new_te = new PickupIndex[maxitem];
                GetNewItemsForTier(new_te, TEquip_List, maxitem, List_Max[2]);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_te[i] != PickupIndex.none)
                    {
                        run.availableEquipmentDropList.Add(new_te[i]);
                        if (DumpList.Value)
                        {
                            print(new_te[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + List_Max[2]);
            }
            void BuildLEquipPool(Run run)
            {
                int actualmax = 0;
                int maxitem = MaxLEquip.Value;
                if (maxitem < 1)
                {
                    maxitem = List_Max[3];
                }
                print("--LUNAR EQUIPMENT--");
                PickupIndex[] new_le = new PickupIndex[maxitem];
                GetNewItemsForTier(new_le, LEquip_List, maxitem, List_Max[3]);
                for (int i = 0; i < maxitem; i++)
                {
                    if (new_le[i] != PickupIndex.none)
                    {
                        run.availableLunarEquipmentDropList.Add(new_le[i]);
                        run.availableLunarDropList.Add(new_le[i]);
                        if (DumpList.Value)
                        {
                            print(new_le[i]);
                        }
                        actualmax += 1;
                    }
                }
                print("TOTAL: " + actualmax + "/" + List_Max[3]);
            }

            void GenerateNewRunList(Run run)
            {
                ClearOldRunList(run);
                print("Rebuilding item pools...");
                //Rebuild pools
                BuildWhitePool(run);
                BuildGreenPool(run);
                BuildRedPool(run);
                BuildBossPool(run);
                BuildLunarPool(run);
                BuildEquipPool(run);
                BuildLEquipPool(run);
                //Add new pools to chests
                AddListToChests(run);
                print("Done! Enjoy your run.");
            };

            PickupIndex[] GetNewItemsForTier(PickupIndex[] new_table, PickupIndex[] base_table, int new_max, int old_max)
            {
                if (new_max < 1)
                {
                    new_max = old_max;
                }
                //Set all values of the table to none for later sanity checks
                for (int i = 0; i < new_max; i++)
                {
                    new_table[i] = PickupIndex.none;
                }
                if (new_max > old_max)
                {
                    new_max = old_max;
                }
                //I'll just shuffle the original array and give the new array the first x results
                //I could do a proper random order, but the only way I know involves multiple tables and headaches.
                for (int i = 0; i < old_max; i++)
                {
                    if(base_table[i] != PickupIndex.none)
                    {
                        int newslot = UnityEngine.Random.Range(0, old_max);
                        PickupIndex oldval = base_table[newslot];
                        base_table[newslot] = base_table[i];
                        base_table[i] = oldval;
                    }
                }
                for (int i = 0; i < new_max; i++)
                {
                    new_table[i] = base_table[i];
                }
                return new_table;
            }

            PickupIndex[] GetNewItemsForTierCat(PickupIndex[] new_table, PickupIndex[,] base_table, int new_max, int[] old_max)
            {
                //Set all values of the table to none for later sanity checks
                for (int i = 0; i < new_max; i++)
                {
                    new_table[i] = PickupIndex.none;
                }
                //Get Category Amounts
                int DamageCount = (int)(new_max * DamageCent.Value);
                int UtilityCount = (int)(new_max * UtilityCent.Value);
                int HealCount = (int)(new_max * HealCent.Value);
                int diff = (DamageCount + HealCount + UtilityCount) - new_max;

                //Fix the Difference
                if(diff>0)
                {
                    int nextcat = 0;
                    for (int i = 0; i < diff; i++)
                    {
                        switch (nextcat)
                        {
                            case 0:
                                DamageCount -= 1;
                                nextcat += 1;
                                break;
                            case 1:
                                UtilityCount -= 1;
                                nextcat += 1;
                                break;
                            case 2:
                                HealCount -= 1;
                                nextcat = 0;
                                break;
                        }
                    }
                }
                else if(diff<0)
                {
                    diff *= -1;
                    int nextcat = 0;
                    for (int i = 0; i < diff; i++)
                    {
                        switch (nextcat)
                        {
                            case 0:
                                DamageCount += 1;
                                nextcat += 1;
                                break;
                            case 1:
                                UtilityCount += 1;
                                nextcat += 1;
                                break;
                            case 2:
                                HealCount += 1;
                                nextcat = 0;
                                break;
                        }
                    }
                }

                //Clamp categories to the actual max amount
                //Also grab the excess
                diff = 0;
                if (DamageCount > old_max[0])
                {
                    diff = DamageCount - old_max[0];
                    DamageCount = old_max[0];
                }
                if (UtilityCount > old_max[1])
                {
                    diff = UtilityCount - old_max[1];
                    UtilityCount = old_max[1];
                }
                if (HealCount > old_max[2])
                {
                    diff = HealCount - old_max[2];
                    HealCount = old_max[2];
                }
                //Relocate the excess to valid groups
                if (diff > 0)
                {
                    for (; ; )
                    {
                        int i = diff;
                        if (diff > 0 && DamageCount < old_max[0])
                        {
                            DamageCount += 1;
                            diff -= 1;
                        }
                        if (diff > 0 && UtilityCount < old_max[1])
                        {
                            UtilityCount += 1;
                            diff -= 1;
                        }
                        if (diff > 0 && HealCount < old_max[2])
                        {
                            HealCount += 1;
                            diff -= 1;
                        }
                        if (i == diff || diff == 0)
                        {
                            break;
                        }
                    }
                }
                //Shuffle tier pool
                for (int z = 0; z < 3; z++)
                {
                    for (int i = 0; i < old_max[z]; i++)
                    {
                        if (base_table[z, i] != PickupIndex.none)
                        {
                            int newslot = UnityEngine.Random.Range(0, old_max[z]);
                            PickupIndex oldval = base_table[z, newslot];
                            base_table[z, newslot] = base_table[z, i];
                            base_table[z, i] = oldval;
                        }
                    }
                }
                //Add items to the returning table
                if (DamageCount > 0)
                {
                    for (int i = 0; i < DamageCount; i++)
                    {
                        new_table[i] = base_table[0, i];
                    }
                }
                if (UtilityCount > 0)
                {
                    for (int i = 0; i < UtilityCount; i++)
                    {
                        new_table[i + DamageCount] = base_table[1, i];
                    }
                }
                if (HealCount > 0)
                {
                    for (int i = 0; i < HealCount; i++)
                    {
                        new_table[i + DamageCount + UtilityCount] = base_table[2, i];
                    }
                }
                return new_table;
            }

            void ClearOldRunList(Run run)
            {
                run.availableTier1DropList.Clear();
                run.availableTier2DropList.Clear();
                run.availableTier3DropList.Clear();
                run.availableLunarDropList.Clear();
                run.availableLunarEquipmentDropList.Clear();
                run.availableEquipmentDropList.Clear();
                run.availableBossDropList.Clear();
                run.smallChestDropTierSelector.Clear();
                run.mediumChestDropTierSelector.Clear();
                run.largeChestDropTierSelector.Clear();
            };

            void AddListToChests(Run run)
            {
                run.smallChestDropTierSelector.AddChoice(run.availableTier1DropList, 0.8f);
                run.smallChestDropTierSelector.AddChoice(run.availableTier2DropList, 0.2f);
                run.smallChestDropTierSelector.AddChoice(run.availableTier3DropList, 0.01f);
                run.mediumChestDropTierSelector.AddChoice(run.availableTier2DropList, 0.8f);
                run.mediumChestDropTierSelector.AddChoice(run.availableTier3DropList, 0.2f);
                run.largeChestDropTierSelector.AddChoice(run.availableTier3DropList, 1.0f);
            };

            bool ValidItemTags(ItemDef iteminfo)
            {
                ItemTag[] itemtags = iteminfo.tags;
                if (itemtags.Length > 0)
                {
                    for (int i = 0; i < itemtags.Length; i++)
                    {
                        if (itemtags[i] == ItemTag.Scrap)
                        {
                            return false;
                        }
                        else if (itemtags[i] == ItemTag.WorldUnique)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            ItemTag GetItemType(ItemDef iteminfo)
            {
                ItemTag[] itemtags = iteminfo.tags;
                if (itemtags.Length > 0)
                {
                    for (int i = 0; i < itemtags.Length; i++)
                    {
                        if (itemtags[i] == ItemTag.Damage || itemtags[i] == ItemTag.Healing || itemtags[i] == ItemTag.Utility)
                        {
                            return itemtags[i];
                        }
                    }
                }
                return ItemTag.Any;
            }
        }
        public void ReadConfig()
        {
            MaxT1 = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max White Tier"), 0, new ConfigDescription("Max amount of different white items avaliable per run. (0 or less disables this)", null, Array.Empty<object>()));
            MaxT2 = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max Green Tier"), 0, new ConfigDescription("Max amount of different green items avaliable per run. (0 or less disables this)", null, Array.Empty<object>()));
            MaxT3 = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max Red Tier"), 0, new ConfigDescription("Max amount of different red items avaliable per run. (0 or less disables this)", null, Array.Empty<object>()));
            MaxTB = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max Boss Tier"), 0, new ConfigDescription("Max amount of different boss items avaliable per run. (0 or less disables this) (I have honestly no idea how this effects the game.)", null, Array.Empty<object>()));
            MaxTL = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max Lunar Tier"), 0, new ConfigDescription("Max amount of different lunar items avaliable per run. (0 or less disables this) (This doesn't include lunar equipment.)", null, Array.Empty<object>()));
            MaxEquip = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max Equip Tier"), 0, new ConfigDescription("Max amount of different equipment items avaliable per run. (0 or less disables this) (This doesn't include lunar equipment.)", null, Array.Empty<object>()));
            MaxLEquip = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Max Lunar Equip Tier"), 0, new ConfigDescription("Max amount of different lunar equipment items avaliable per run (0 or less disables this).", null, Array.Empty<object>()));
            DamageCent = Config.Bind<float>(new ConfigDefinition("ItemPoolRestrictor", "Damage Item Percent"), 0f, new ConfigDescription("Sets the percentage of selected white, green and red tier items as Damage items. (1.0 = 100%) (Max amount of items in each tier is prioritized over this.)", null, Array.Empty<object>()));
            UtilityCent = Config.Bind<float>(new ConfigDefinition("ItemPoolRestrictor", "Utility Item Percent"), 0f, new ConfigDescription("Sets the percentage of selected white, green and red tier items as Utility items. (1.0 = 100%) (Max amount of items in each tier is prioritized over this.)", null, Array.Empty<object>()));
            HealCent = Config.Bind<float>(new ConfigDefinition("ItemPoolRestrictor", "Healing Item Percent"), 0f, new ConfigDescription("Sets the percentage of selected white, green and red tier items as Healing items. (1.0 = 100%) (Max amount of items in each tier is prioritized over this.)", null, Array.Empty<object>()));
            DumpList = Config.Bind<bool>(new ConfigDefinition("ItemPoolRestrictor", "Dump Pool"), false, new ConfigDescription("Prints a list of every item avaliable in the console at the start of a run.", null, Array.Empty<object>()));
        }
    }
}
