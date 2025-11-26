using System;
using RoR2;
using R2API;
using UnityEngine.Networking;

namespace ConsumedBuff.ItemChanges
{
    public class EmptyBottle
    {
        public static void Enable()
        {
            UpdateText();
            if (MainPlugin.Elixir_Regen != 0f)
            {
                RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
            }
            if (MainPlugin.Elixir_Buff > 0f)
            {
                On.RoR2.CharacterMasterNotificationQueue.PushItemTransformNotification += OnItemAdded;
            }
            UpdateText();
        }
        private static void UpdateText()
        {
            string pickup = string.Format("");
            string desc = string.Format("");
            if(MainPlugin.Elixir_Regen != 0f)
            {
                pickup = string.Format("Increases health regeneration.");
                desc = string.Format("Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+{0} hp/s</style> <style=cStack>(+{0} per stack)</style>.", MainPlugin.Elixir_Regen);
            }
            else
            {
                pickup = string.Format("An empty container from an Elixir you consumed. Does nothing.");
                desc = string.Format("A spent item with no remaining power.");
            }
            LanguageAPI.Add("ITEM_HEALINGPOTIONCONSUMED_PICKUP", pickup);
            LanguageAPI.Add("ITEM_HEALINGPOTIONCONSUMED_DESC", desc);
            LanguageAPI.Add("ITEM_HEALINGPOTIONCONSUMED_PICKUP", pickup, "en");
            LanguageAPI.Add("ITEM_HEALINGPOTIONCONSUMED_DESC", desc, "en");
        }
        private static void OnItemAdded(On.RoR2.CharacterMasterNotificationQueue.orig_PushItemTransformNotification orig, CharacterMaster self, ItemIndex oldItem, ItemIndex newItem, CharacterMasterNotificationQueue.TransformationType transformationType)
        {
            orig(self, oldItem, newItem, transformationType);
            if (NetworkServer.active)
            {
                if (transformationType == CharacterMasterNotificationQueue.TransformationType.Default)
                {
                    if (self.GetBody())
                    {
                        if (oldItem == DLC1Content.Items.HealingPotion.itemIndex)
                        {
                            if (newItem == DLC1Content.Items.HealingPotionConsumed.itemIndex)
                            {
                                self.GetBody().AddTimedBuff(RoR2Content.Buffs.CrocoRegen, MainPlugin.Elixir_Buff);
                            }
                        }
                    }
                }
            }
        }
        private static void CalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                float levelBonus = Math.Max(0f, sender.level - 1f);
                int itemCount = sender.inventory.GetItemCountEffective(DLC1Content.Items.HealingPotionConsumed);
                if(itemCount > 0)
                {
                    levelBonus = MainPlugin.Elixir_Regen * 0.2f * levelBonus;
                    args.baseRegenAdd += itemCount * (MainPlugin.Elixir_Regen + levelBonus);
                }
            }
        }
    }
}