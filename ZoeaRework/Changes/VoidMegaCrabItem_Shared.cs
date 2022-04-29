using System.Collections.Generic;
using System.Linq;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZoeaRework.Changes
{
    public class VoidMegaCrabItem_Shared
    {
        public static void Begin()
        {
            EditTags();
            SetupTokens();
        }
        private static void SetupTokens()
        {
            LanguageAPI.Add(MainPlugin.MODTOKEN + "UTILITY_TELEPORT_NAME", "Recall");
            LanguageAPI.Add(MainPlugin.MODTOKEN + "UTILITY_TELEPORT_DESC", "Return to your owner.");
        }
        private static void EditTags()
        {
            ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/VoidMegaCrabItem.asset").WaitForCompletion();
            if(itemDef)
            {
                List<ItemTag> itemtags = itemDef.tags.ToList();
                itemtags.Add(ItemTag.CannotCopy);
                //itemtags.Add(ItemTag.BrotherBlacklist);
                itemDef.tags = itemtags.ToArray();

                itemDef.pickupToken = MainPlugin.MODTOKEN + "ITEM_VOIDMEGACRABITEM_PICKUP";
                itemDef.descriptionToken = MainPlugin.MODTOKEN + "ITEM_VOIDMEGACRABITEM_DESC";
            }
        }
    }
}
