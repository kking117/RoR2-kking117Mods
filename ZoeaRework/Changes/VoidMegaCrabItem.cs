using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace ZoeaRework.Changes
{
    public class VoidMegaCrabItem
    {
        public static void Begin()
        {
            UpdateText();
            EditTags();
            Hooks();
            VoidMegaCrabAlly.Begin();
        }
        private static void UpdateText()
        {
            string stat_text = String.Format("<style=cIsDamage>{0}% ", (MainPlugin.Config_Zoea_DamageBase.Value + 10) * 10);
            if(MainPlugin.Config_Zoea_DamageBonus.Value != 0)
            {
                stat_text += String.Format("<style=cStack>(+{0}% per stack)</style>", MainPlugin.Config_Zoea_DamageBonus.Value * 10);
            }
            stat_text += " damage</style>";
            if (MainPlugin.Config_Zoea_HealthBase.Value != 0 || MainPlugin.Config_Zoea_HealthBonus.Value != 0)
            {
                if(stat_text.Length > 0)
                {
                    stat_text += " and ";
                }
                stat_text += String.Format("<style=cIsHealing>{0}% ", (MainPlugin.Config_Zoea_HealthBase.Value + 10) * 10);
                if (MainPlugin.Config_Zoea_HealthBonus.Value != 0)
                {
                    stat_text += String.Format("<style=cStack>(+{0}% per stack)</style>", MainPlugin.Config_Zoea_HealthBonus.Value * 10);
                }
                stat_text += " health</style>";
            }

            string pickup_text = "";
            string desc_text = "";
            pickup_text += "Recruit a <style=cIsVoid>Void Devastator</style>";
            desc_text += String.Format("<style=cIsUtility>Summon</style> a <style=cIsVoid>Void Devastator</style> with {0}", stat_text);
            if(MainPlugin.Config_Zoea_Inherit.Value)
            {
                pickup_text += " that inherits your items";
                desc_text += " that <style=cIsUtility>inherits your items</style>";
            }
            pickup_text += ".";
            desc_text += ".";
            if (MainPlugin.Config_Zoea_OnlyGlands.Value)
            {
                pickup_text += " <style=cIsVoid>Corrupts all </style><style=cIsTierBoss>Queen's Glands</style>.";
                desc_text += " <style=cIsVoid>Corrupts all </style><style=cIsTierBoss>Queen's Glands</style>.";
            }
            else
            {
                pickup_text += " <style=cIsVoid>Corrupts most </style><style=cIsTierBoss>yellow items</style>.";
                desc_text += " <style=cIsVoid>Corrupts most </style><style=cIsTierBoss>yellow items</style>.";
            }
            R2API.LanguageAPI.Add("ITEM_VOIDMEGACRABITEM_PICKUP", pickup_text, "en");
            R2API.LanguageAPI.Add("ITEM_VOIDMEGACRABITEM_DESC", desc_text, "en");
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
            }
        }
        private static void Hooks()
        {
            if (MainPlugin.Config_Zoea_OnlyGlands.Value)
            {
                //Replace the Zoea's relationship with only Queen's Gland
                On.RoR2.ItemCatalog.SetItemRelationships += SetItemRelationships;
            }
            //Prevent the old behaviour from running, run our own.
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChange;
            //Deployable slot changes
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
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
        private static int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            var result = orig(self, slot);
            if (slot != DeployableSlot.VoidMegaCrabItem)
            {
                return result;
            }
            int amount = 1;
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef))
            {
                amount *= 2;
            }
            return amount;
        }
        private static void OnInventoryChange(CharacterBody self)
        {
            if (NetworkServer.active)
            {
                int itemCount = self.inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem);
                self.AddItemBehavior<VoidMegaCrabItemBehavior>(0);
                self.AddItemBehavior<ZoeaBehavior>(itemCount);
                if(itemCount < 1)
                {
                    VoidMegaCrabAlly.UpdateAllSummonInventory(self.master);
                }
            }
        }
    }
}
