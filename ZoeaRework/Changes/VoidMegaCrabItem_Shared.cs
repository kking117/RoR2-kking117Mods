using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZoeaRework.Changes
{
    public class VoidMegaCrabItem_Shared
    {
        public static void Begin()
        {
            EditTags();
            Hooks();
        }
        private static void Hooks()
        {
            if (MainPlugin.Config_Shared_OnlyGlands.Value)
            {
                //Replace the Zoea's relationship with only Queen's Gland
                On.RoR2.ItemCatalog.SetItemRelationships += SetItemRelationships;
            }
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
                        if (itempair.itemDef2 != DLC1Content.Items.VoidMegaCrabItem)
                        {
                            newitempair.Add(itempair);
                        }
                    }
                    itemrelation.relationships = newitempair.ToArray();
                }
                newprovider.Add(itemrelation);
            }

            ItemRelationshipProvider newItemRelationship = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
            newItemRelationship.relationshipType = DLC1Content.ItemRelationshipTypes.ContagiousItem;
            newItemRelationship.relationships = new[] {new ItemDef.Pair {
                    itemDef1 = RoR2Content.Items.BeetleGland,
                    itemDef2 = DLC1Content.Items.VoidMegaCrabItem
                }};
            newprovider.Add(newItemRelationship);
            orig(newprovider.ToArray());
        }
    }
}
