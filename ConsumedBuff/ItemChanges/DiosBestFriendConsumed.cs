﻿using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;

namespace ConsumedBuff.ItemChanges
{
    public class DiosBestFriendConsumed
    {
        public static void Enable()
        {
            UpdateText();
            if (MainPlugin.Dio_BearWorth.Value != 0)
            {
                On.RoR2.HealthComponent.OnInventoryChanged += OnInventoryChanged;
            }
        }
        public static void UpdateText()
        {
            string pickup = string.Format("");
            string desc = string.Format("");
            if (MainPlugin.Dio_BearWorth.Value != 0)
            {
                pickup = string.Format("Chance to block incoming damage.");
                desc = string.Format("Counts as <style=cIsUtility>{0}<style=cStack> (+{0} per stack)</style> Tougher Times</style>.", MainPlugin.Dio_BearWorth.Value);
            }
            else
            {
                pickup = string.Format("A spent item with no remaining power.");
                desc = string.Format("A spent item with no remaining power.");
            }
            LanguageAPI.Add("ITEM_EXTRALIFECONSUMED_PICKUP", pickup);
            LanguageAPI.Add("ITEM_EXTRALIFECONSUMED_DESC", desc);
        }
        private static void OnInventoryChanged(On.RoR2.HealthComponent.orig_OnInventoryChanged orig, HealthComponent self)
        {
            orig(self);
            if(self.body)
            {
                if(self.body.inventory)
                {
                    self.itemCounts.bear += self.body.inventory.GetItemCount(RoR2Content.Items.ExtraLifeConsumed) * MainPlugin.Dio_BearWorth.Value;
                }
            }
        }
    }
}