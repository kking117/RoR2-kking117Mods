using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ZoeaRework.Changes
{
    public class VoidMegaCrabItem_Buff
    {
        public static void Begin()
        {
            VoidMegaCrabItem_Shared.Begin();
            Hooks();
            UpdateText();
            VoidMegaCrabAlly.Begin();
            VoidJailerAlly.Begin();
            NullifierAlly.Begin();
        }
        private static void UpdateText()
        {
            string stat_text = String.Format("<style=cIsDamage>{0}% ", (MainPlugin.Config_Buff_DamageBase.Value + 10) * 10);
            if(MainPlugin.Config_Buff_DamageStack.Value != 0)
            {
                stat_text += String.Format("<style=cStack>(+{0}% per stack)</style> ", MainPlugin.Config_Buff_DamageStack.Value * 10);
            }
            stat_text += "damage</style>";
            if (MainPlugin.Config_Buff_HealthBase.Value != 0 || MainPlugin.Config_Buff_HealthStack.Value != 0)
            {
                if(stat_text.Length > 0)
                {
                    stat_text += " and ";
                }
                stat_text += String.Format("<style=cIsHealing>{0}% ", (MainPlugin.Config_Buff_HealthBase.Value + 10) * 10);
                if (MainPlugin.Config_Buff_HealthStack.Value != 0)
                {
                    stat_text += String.Format("<style=cStack>(+{0}% per stack)</style> ", MainPlugin.Config_Buff_HealthStack.Value * 10);
                }
                stat_text += "health</style>";
            }

            string pickup_text = "";
            string desc_text = "";
            pickup_text += "Recruit allies from the <style=cIsVoid>Void</style>.";
            desc_text += String.Format("Every <style=cIsUtility>{0}</style> seconds, gain a random <style=cIsVoid>Void</style> ally with {1}.", MainPlugin.Config_Buff_SpawnTime.Value, stat_text);
            desc_text += String.Format(" Can have up to <style=cIsUtility>3</style> allies at a time.");
            if (MainPlugin.Config_Buff_CorruptText.Value.Length > 0)
            {
                pickup_text += " " + MainPlugin.Config_Buff_CorruptText.Value;
                desc_text += " " + MainPlugin.Config_Buff_CorruptText.Value;
            }
            R2API.LanguageAPI.Add(MainPlugin.MODTOKEN + "ITEM_VOIDMEGACRABITEM_PICKUP", pickup_text);
            R2API.LanguageAPI.Add(MainPlugin.MODTOKEN + "ITEM_VOIDMEGACRABITEM_DESC", desc_text);
        }
        private static void Hooks()
        {
            On.RoR2.ItemCatalog.SetItemRelationships += SetItemRelationships;
            //Prevent the old behaviour from running, run our own.
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChange;
            //Deployable slot changes
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
        }
        private static void SetItemRelationships(On.RoR2.ItemCatalog.orig_SetItemRelationships orig, ItemRelationshipProvider[] providers)
        {
            List<ItemDef> CorruptList = new List<ItemDef>();
            string[] items = MainPlugin.Config_Buff_CorruptList.Value.Split(' ');
            for (int i = 0; i < items.Length; i++)
            {
                ItemIndex itemIndex = ItemCatalog.FindItemIndex(items[i]);
                if (itemIndex > ItemIndex.None)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                    if (itemDef)
                    {
                        CorruptList.Add(itemDef);
                    }
                }
            }

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

            if (CorruptList.Count > 0)
            {
                ItemRelationshipProvider newItemRelationship = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
                newItemRelationship.relationshipType = DLC1Content.ItemRelationshipTypes.ContagiousItem;
                newItemRelationship.relationships = new ItemDef.Pair[CorruptList.Count];
                for (int i = 0; i < CorruptList.Count; i++)
                {
                    newItemRelationship.relationships[i] = new ItemDef.Pair
                    {
                        itemDef1 = CorruptList[i],
                        itemDef2 = DLC1Content.Items.VoidMegaCrabItem
                    };
                }
                newprovider.Add(newItemRelationship);
            }
            orig(newprovider.ToArray());
        }
        private static int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            var result = orig(self, slot);
            if (slot != DeployableSlot.VoidMegaCrabItem)
            {
                return result;
            }

            int amount = 0;
            if (self.inventory)
            {
                if(self.inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem) > 0)
                {
                    amount = 3;
                }
            }
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
                if (Stage.instance.sceneDef == MainPlugin.BazaarSceneDef)
                {
                    itemCount = 0;
                }
                self.AddItemBehavior<VoidMegaCrabItemBehavior>(0);
                self.AddItemBehavior<ZoeaBehavior_Buff>(itemCount);
                UpdateAllSummonInventory(self.master);
            }
        }
        internal static void CullSummons(CharacterMaster owner)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (owner.deployablesList != null)
            {
                int maxSummons = owner.GetDeployableSameSlotLimit(DeployableSlot.VoidMegaCrabItem);
                int curSummons = 0;
                for (int i = 0; i < owner.deployablesList.Count; i++)
                {
                    if (owner.deployablesList[i].slot == DeployableSlot.VoidMegaCrabItem)
                    {
                        Deployable deploy = owner.deployablesList[i].deployable;
                        if (deploy)
                        {
                            CharacterMaster master = deploy.GetComponent<CharacterMaster>();
                            if (master)
                            {
                                if (master.teamIndex == owner.teamIndex)
                                {
                                    curSummons += 1;
                                    if (curSummons > maxSummons)
                                    {
                                        master.TrueKill();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        internal static void UpdateAllSummonInventory(CharacterMaster owner)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (owner.deployablesList != null)
            {
                int maxSummons = owner.GetDeployableSameSlotLimit(DeployableSlot.VoidMegaCrabItem);
                int curSummons = 0;
                for (int i = 0; i < owner.deployablesList.Count; i++)
                {
                    if (owner.deployablesList[i].slot == DeployableSlot.VoidMegaCrabItem)
                    {
                        Deployable deploy = owner.deployablesList[i].deployable;
                        if (deploy)
                        {
                            CharacterMaster master = deploy.GetComponent<CharacterMaster>();
                            if (master)
                            {
                                if (master.teamIndex == owner.teamIndex)
                                {
                                    curSummons += 1;
                                    if (curSummons > maxSummons)
                                    {
                                        master.TrueKill();
                                    }
                                    else
                                    {
                                        UpdateInventory(owner, master);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        internal static void UpdateInventory(CharacterMaster owner, CharacterMaster summon)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            Inventory owneritems = owner.inventory;
            if (owneritems)
            {
                Inventory inv = summon.inventory;
                int itemCount = Math.Max(0, owneritems.GetItemCount(DLC1Content.Items.VoidMegaCrabItem) - 1);
                inv.ResetItem(RoR2Content.Items.BoostDamage);
                inv.ResetItem(RoR2Content.Items.BoostHp);
                int dmg = MainPlugin.Config_Buff_DamageBase.Value + (MainPlugin.Config_Buff_DamageStack.Value * itemCount);
                int hp = MainPlugin.Config_Buff_HealthBase.Value + (MainPlugin.Config_Buff_HealthStack.Value * itemCount);
                inv.GiveItem(RoR2Content.Items.BoostDamage, dmg);
                inv.GiveItem(RoR2Content.Items.BoostHp, hp);
            }
        }
    }
}
