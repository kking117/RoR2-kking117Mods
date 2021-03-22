using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace ItemPoolRestriction
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.kking117.ItemPoolLimiter", "ItemPoolLimiter", "1.1.0")]
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

        private bool PoolsRestored;
        private int LastStageReroll;

        public static ConfigEntry<float> DamageCent;
        public static ConfigEntry<float> UtilityCent;
        public static ConfigEntry<float> HealCent;

        public static ConfigEntry<int> MaxT1;
        public static ConfigEntry<int> MaxT2;
        public static ConfigEntry<int> MaxT3;
        public static ConfigEntry<int> MaxTB;
        public static ConfigEntry<int> MaxTL;
        public static ConfigEntry<int> MaxEquip;
        public static ConfigEntry<int> MaxLEquip;
        public static ConfigEntry<bool> DumpList;
        public static ConfigEntry<int> RemoveOnStage;
        public static ConfigEntry<int> RerollOnStage;
        public void Awake()
        {
            ReadConfig();

            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                LastStageReroll = -1;
                PoolsRestored = false;
                BuildDropLists(self);
                GenerateNewRunList(self);
            };
            On.RoR2.Run.BeginStage += (orig, self) =>
            {
                orig(self);
                if (self.NetworkstageClearCount > 0)
                {
                    if (!PoolsRestored)
                    {
                        if (RemoveOnStage.Value > 0 && self.NetworkstageClearCount == RemoveOnStage.Value)
                        {
                            RestoreOriginalRunList(self);
                        }
                        else if (RerollOnStage.Value > 0)
                        {
                            if (self.NetworkstageClearCount != LastStageReroll && self.NetworkstageClearCount % RerollOnStage.Value == 0)
                            {
                                LastStageReroll = self.NetworkstageClearCount;
                                GenerateNewRunList(self);
                            }
                        }
                    }
                }
            };

            void BuildDropLists(Run run)
            {
                ClearDropLists();
                Logger.LogInfo("Duplicating normal drop list arrays...");
                //Whites
                for (int i = 0; i < run.availableTier1DropList.Count; i++)
                {
                    PickupIndex index = run.availableTier1DropList[i];
                    PickupDef pickupdef = PickupCatalog.GetPickupDef(index);
                    ItemIndex itemindex = pickupdef.itemIndex;
                    ItemTag itemtype = GetItemType(ItemCatalog.GetItemDef(itemindex));
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
                }
                //Greens
                for (int i = 0; i < run.availableTier2DropList.Count; i++)
                {
                    PickupIndex index = run.availableTier2DropList[i];
                    PickupDef pickupdef = PickupCatalog.GetPickupDef(index);
                    ItemIndex itemindex = pickupdef.itemIndex;
                    ItemTag itemtype = GetItemType(ItemCatalog.GetItemDef(itemindex));
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
                }
                //Reds
                for (int i = 0; i < run.availableTier3DropList.Count; i++)
                {
                    PickupIndex index = run.availableTier3DropList[i];
                    PickupDef pickupdef = PickupCatalog.GetPickupDef(index);
                    ItemIndex itemindex = pickupdef.itemIndex;
                    ItemTag itemtype = GetItemType(ItemCatalog.GetItemDef(itemindex));
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
                }
                //Boss
                for (int i = 0; i < run.availableBossDropList.Count; i++)
                {
                    PickupIndex index = run.availableBossDropList[i];
                    TB_List[List_Max[0]] = index;
                    List_Max[0] += 1;
                }
                //Lunar & Lunar Equipment
                for (int i = 0; i < run.availableLunarDropList.Count; i++)
                {
                    PickupIndex index = run.availableLunarDropList[i];
                    PickupDef pickupdef = PickupCatalog.GetPickupDef(index);
                    if(pickupdef.itemIndex == ItemIndex.None) //None means it's lunar equipment (probably)
                    {
                        if (pickupdef.equipmentIndex != EquipmentIndex.None) //Okay maybe it's actually equipment
                        {
                            LEquip_List[List_Max[3]] = index;
                            List_Max[3] += 1;
                        }
                    }
                    else
                    {
                        TL_List[List_Max[1]] = index;
                        List_Max[1] += 1;
                    }
                }
                //Equipment
                for (int i = 0; i < run.availableEquipmentDropList.Count; i++)
                {
                    PickupIndex index = run.availableEquipmentDropList[i];
                    TEquip_List[List_Max[2]] = index;
                    List_Max[2] += 1;
                }
            }

            void BuildWhitePool(Run run)
            {
                Logger.LogInfo("-=WHITE=-");
                if (MaxT1.Value > 0)
                {
                    int actualmax = 0;
                    PickupIndex[] new_t1 = new PickupIndex[MaxT1.Value];
                    GetNewItemsForTierCat(new_t1, T1_List, MaxT1.Value, List_Cat_T1);
                    for (int i = 0; i < MaxT1.Value; i++)
                    {
                        if (new_t1[i] != PickupIndex.none)
                        {
                            run.availableTier1DropList.Add(new_t1[i]);
                            if (DumpList.Value)
                            {
                                Logger.LogInfo(new_t1[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + (List_Cat_T1[0] + List_Cat_T1[1] + List_Cat_T1[2]));
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + (List_Cat_T1[0]+List_Cat_T1[1]+List_Cat_T1[2]));
                }
            }
            void BuildGreenPool(Run run)
            {
                Logger.LogInfo("-=GREEN=-");
                if (MaxT2.Value > 0)
                {
                    int actualmax = 0;
                    PickupIndex[] new_t2 = new PickupIndex[MaxT2.Value];
                    GetNewItemsForTierCat(new_t2, T2_List, MaxT2.Value, List_Cat_T2);
                    for (int i = 0; i < MaxT2.Value; i++)
                    {
                        if (new_t2[i] != PickupIndex.none)
                        {
                            run.availableTier2DropList.Add(new_t2[i]);
                            if (DumpList.Value)
                            {
                                Logger.LogInfo(new_t2[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + (List_Cat_T2[0] + List_Cat_T2[1] + List_Cat_T2[2]));
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + (List_Cat_T2[0] + List_Cat_T2[1] + List_Cat_T2[2]));
                }
            }
            void BuildRedPool(Run run)
            {
                Logger.LogInfo("-=RED=-");
                if (MaxT3.Value > 0)
                {
                    int actualmax = 0;
                    PickupIndex[] new_t3 = new PickupIndex[MaxT3.Value];
                    GetNewItemsForTierCat(new_t3, T3_List, MaxT3.Value, List_Cat_T3);
                    for (int i = 0; i < MaxT3.Value; i++)
                    {
                        if (new_t3[i] != PickupIndex.none)
                        {
                            run.availableTier3DropList.Add(new_t3[i]);
                            if (DumpList.Value)
                            {
                                Logger.LogInfo(new_t3[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + (List_Cat_T3[0] + List_Cat_T3[1] + List_Cat_T3[2]));
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + (List_Cat_T3[0] + List_Cat_T3[1] + List_Cat_T3[2]));
                }
            }
            void BuildBossPool(Run run)
            {
                Logger.LogInfo("-=BOSS=-");
                if (MaxTB.Value > 0)
                {
                    int actualmax = 0;
                    PickupIndex[] new_tb = new PickupIndex[MaxTB.Value];
                    GetNewItemsForTier(new_tb, TB_List, MaxTB.Value, List_Max[0]);
                    for (int i = 0; i < MaxTB.Value; i++)
                    {
                        if (new_tb[i] != PickupIndex.none)
                        {
                            run.availableBossDropList.Add(new_tb[i]);
                            if (DumpList.Value)
                            {
                                Logger.LogInfo(new_tb[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + List_Max[0]);
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + List_Max[0]);
                }
            }
            void BuildLunarPool(Run run)
            {
                Logger.LogInfo("-=LUNAR=-");
                //Lunar equipment kinda fucky
                if (MaxTL.Value > 0 || MaxLEquip.Value > 0)
                {
                    int actualmax = 0;
                    int maxitem = MaxTL.Value;
                    if (maxitem < 1)
                    {
                        maxitem = List_Max[1];
                    }
                    PickupIndex[] new_tl = new PickupIndex[maxitem];
                    GetNewItemsForTier(new_tl, TL_List, maxitem, List_Max[1]);
                    for (int i = 0; i < maxitem; i++)
                    {
                        if (new_tl[i] != PickupIndex.none)
                        {
                            run.availableLunarDropList.Add(new_tl[i]);
                            if (DumpList.Value)
                            {
                                Logger.LogInfo(new_tl[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + List_Max[1]);
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + List_Max[1]);
                }
            }
            void BuildEquipPool(Run run)
            {
                Logger.LogInfo("-=EQUIPMENT=-");
                if (MaxEquip.Value > 0)
                {
                    int actualmax = 0;
                    PickupIndex[] new_te = new PickupIndex[MaxEquip.Value];
                    GetNewItemsForTier(new_te, TEquip_List, MaxEquip.Value, List_Max[2]);
                    for (int i = 0; i < MaxEquip.Value; i++)
                    {
                        if (new_te[i] != PickupIndex.none)
                        {
                            run.availableEquipmentDropList.Add(new_te[i]);
                            if (DumpList.Value)
                            {
                                Logger.LogInfo(new_te[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + List_Max[2]);
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + List_Max[2]);
                }
            }
            void BuildLEquipPool(Run run)
            {
                Logger.LogInfo("-=LUNAR EQUIPMENT=-");
                if (MaxTL.Value > 0 || MaxLEquip.Value > 0)
                {
                    int actualmax = 0;
                    int maxitem = MaxLEquip.Value;
                    if (maxitem < 1)
                    {
                        maxitem = List_Max[3];
                    }
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
                                Logger.LogInfo(new_le[i]);
                            }
                            actualmax += 1;
                        }
                    }
                    Logger.LogInfo("TOTAL: " + actualmax + "/" + List_Max[3]);
                }
                else
                {
                    Logger.LogInfo("UNCHANGED, TOTAL: " + List_Max[3]);
                }
            }

            void GenerateNewRunList(Run run)
            {
                ClearOldRunList(run);
                Logger.LogInfo("Rebuilding item pools...");
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
                Logger.LogInfo("Item pool changes complete!");
            };

            void RestoreOriginalRunList(Run run)
            {
                ClearOldRunList(run);
                Logger.LogInfo("Restoring original item pools...");
                //Rebuild pools
                //Whites
                if (MaxT1.Value > 0)
                {
                    for (int i = 0; i < T1_List.GetLength(0); i++)
                    {
                        for (int z = 0; z < T1_List.GetLength(1); z++)
                        {
                            if (T1_List[i, z] != PickupIndex.none)
                            {
                                run.availableTier1DropList.Add(T1_List[i, z]);
                            }
                        }
                    }
                }
                //Greens
                if (MaxT2.Value > 0)
                {
                    for (int i = 0; i < T2_List.GetLength(0); i++)
                    {
                        for (int z = 0; z < T2_List.GetLength(1); z++)
                        {
                            if (T2_List[i, z] != PickupIndex.none)
                            {
                                run.availableTier2DropList.Add(T2_List[i, z]);
                            }
                        }
                    }
                }
                //Reds
                if (MaxT3.Value > 0)
                {
                    for (int i = 0; i < T3_List.GetLength(0); i++)
                    {
                        for (int z = 0; z < T3_List.GetLength(1); z++)
                        {
                            if (T3_List[i, z] != PickupIndex.none)
                            {
                                run.availableTier3DropList.Add(T3_List[i, z]);
                            }
                        }
                    }
                }
                //Boss
                if (MaxTB.Value > 0)
                {
                    for (int i = 0; i < TB_List.Length; i++)
                    {
                        if (TB_List[i] != PickupIndex.none)
                        {
                            run.availableBossDropList.Add(TB_List[i]);
                        }
                    }
                }
                //Equip
                if (MaxEquip.Value > 0)
                {
                    for (int i = 0; i < TEquip_List.Length; i++)
                    {
                        if (TEquip_List[i] != PickupIndex.none)
                        {
                            run.availableEquipmentDropList.Add(TEquip_List[i]);
                        }
                    }
                }
                //Lunar
                if (MaxTL.Value > 0 || MaxLEquip.Value > 0)
                {
                    for (int i = 0; i < TL_List.Length; i++)
                    {
                        if (TL_List[i] != PickupIndex.none)
                        {
                            run.availableLunarDropList.Add(TL_List[i]);
                        }
                    }
                    for (int i = 0; i < LEquip_List.Length; i++)
                    {
                        if (LEquip_List[i] != PickupIndex.none)
                        {
                            run.availableLunarDropList.Add(LEquip_List[i]);
                            run.availableLunarEquipmentDropList.Add(LEquip_List[i]);
                        }  
                    }
                }
                //Add new pools to chests
                AddListToChests(run);
                Logger.LogInfo("Item pool restored!");
                PoolsRestored = true;
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
                if (MaxT1.Value > 0)
                {
                    run.availableTier1DropList.Clear();
                }
                if (MaxT2.Value > 0)
                {
                    run.availableTier2DropList.Clear();
                }
                if (MaxT3.Value > 0)
                {
                    run.availableTier3DropList.Clear();
                }
                if (MaxTL.Value > 0 || MaxLEquip.Value > 0)
                {
                    run.availableLunarDropList.Clear();
                    //This doesn't seem to be used?
                    //Still changing it to be safe
                    run.availableLunarEquipmentDropList.Clear();
                }
                if (MaxEquip.Value > 0)
                {
                    run.availableEquipmentDropList.Clear();
                }
                if (MaxTB.Value > 0)
                {
                    run.availableBossDropList.Clear();
                }
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

            void ClearDropLists()
            {
                Logger.LogInfo("Clearing drop list arrays...");
                for(int i = 0; i<T1_List.GetLength(1); i++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        T1_List[z, i] = PickupIndex.none;
                        T2_List[z, i] = PickupIndex.none;
                        T3_List[z, i] = PickupIndex.none;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    List_Cat_T1[i] = 0;
                    List_Cat_T2[i] = 0;
                    List_Cat_T3[i] = 0;
                }
                for (int i = 0; i < TB_List.Length; i++)
                {
                    TB_List[i] = PickupIndex.none;
                    TL_List[i] = PickupIndex.none;
                    TEquip_List[i] = PickupIndex.none;
                    LEquip_List[i] = PickupIndex.none;
                }
                for (int i = 0; i < List_Max.Length; i++)
                {
                    List_Max[i] = 0;
                }
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
            RerollOnStage = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Reroll Stage"), 0, new ConfigDescription("Rerolls the item pool upon clearing every Nth stage. (0 or less disables this)", null, Array.Empty<object>()));
            RemoveOnStage = Config.Bind<int>(new ConfigDefinition("ItemPoolRestrictor", "Disable Stage"), 5, new ConfigDescription("Removes all the item pool changes when clearing this stage, effectively disabling this mod for the rest of the run. (0 or less disables this) (When this comes into effect Reroll Stage is disabled.)", null, Array.Empty<object>()));
            DumpList = Config.Bind<bool>(new ConfigDefinition("ItemPoolRestrictor", "Dump Pool"), false, new ConfigDescription("Prints a list of every item avaliable in the console at the start of a run.", null, Array.Empty<object>()));
        }
    }
}
