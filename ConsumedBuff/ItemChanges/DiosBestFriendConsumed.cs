using RoR2;
using R2API;

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
        private static void UpdateText()
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
                    //ToDo: Just give the item it's own dodge chance
                    //So it doesn't break with a mod that changes how tougher times works
                    self.itemCounts.bear += self.body.inventory.GetItemCount(RoR2Content.Items.ExtraLifeConsumed) * MainPlugin.Dio_BearWorth.Value;
                }
            }
        }
    }
}
