using System.Collections.Generic;
using System.Linq;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.CharacterAI;

namespace ZoeaRework.Changes
{
    public class VoidMegaCrabItem_Shared
    {
        public static void Begin()
        {
            EditItemDef();
            SetupTokens();
        }
        private static void SetupTokens()
        {
            LanguageAPI.Add(MainPlugin.MODTOKEN + "UTILITY_TELEPORT_NAME", "Recall");
            LanguageAPI.Add(MainPlugin.MODTOKEN + "UTILITY_TELEPORT_DESC", "Return to your owner.");
        }
        private static void EditItemDef()
        {
            ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/VoidMegaCrabItem.asset").WaitForCompletion();
            if(itemDef)
            {
                List<ItemTag> itemtags = itemDef.tags.ToList();
                itemtags.Add(ItemTag.CannotCopy);
                itemDef.tags = itemtags.ToArray();

                itemDef.pickupToken = MainPlugin.MODTOKEN + "ITEM_VOIDMEGACRABITEM_PICKUP";
                itemDef.descriptionToken = MainPlugin.MODTOKEN + "ITEM_VOIDMEGACRABITEM_DESC";
            }
        }
        internal static void UpdateAILeash(CharacterMaster master)
        {
            if(master)
            {
                foreach (AISkillDriver driver in master.GetComponentsInChildren<AISkillDriver>())
                {
                    if(driver.customName == "ReturnToOwnerLeash")
                    {
                        driver.minDistance = GetLeashDistance();
                        break;
                    }
                }
            }
        }
        internal static float GetLeashDistance()
        {
            Run run = Run.instance;
            float distance = MainPlugin.Config_AIShared_MinRecallDist;
            if (run)
            {
                float diff = (run.difficultyCoefficient - 1f) * MainPlugin.Config_AIShared_RecallDistDiff;
                if (diff > 0f)
                {
                    distance += diff;
                }
                distance = Mathf.Min(MainPlugin.Config_AIShared_MaxRecallDist, distance);
                distance = Mathf.Max(MainPlugin.Config_AIShared_MinRecallDist, distance);
            }
            //MainPlugin.ModLogger.LogInfo("Recall distance: " + distance + "m.");
            return distance;
        }
    }
}
