using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;

namespace ConsumedBuff.ItemChanges
{
    public class DiosBestFriendConsumed
    {
        public static void Enable()
        {
            if (MainPlugin.Dio_BlockChance > 0.0f)
            {
                UpdateText();
                On.RoR2.HealthComponent.TakeDamageProcess += OnTakeDamage;
            }
        }
        private static void UpdateText()
        {
            string pickup = "";
            string desc = "";

            pickup = string.Format("Chance to block incoming damage.");
            desc = string.Format("<style=cIsHealing>{0}%</style> <style=cStack>(+{0}% per stack)</style> chance to <style=cIsHealing>block</style> incoming damage. <style=cIsUtility>Unaffected by luck</style>.", MainPlugin.Dio_BlockChance);

            LanguageAPI.Add("ITEM_EXTRALIFECONSUMED_PICKUP", pickup);
            LanguageAPI.Add("ITEM_EXTRALIFECONSUMED_DESC", desc);
        }
        private static void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                if (self.alive && !self.godMode && !damageInfo.rejected)
                {
                    if (self.ospTimer <= 0f)
                    {
                        bool IgnoreBlock = (damageInfo.damageType & DamageType.BypassBlock) > DamageType.Generic;
                        if (!IgnoreBlock)
                        {
                            if (self.body && self.body.master)
                            {
                                Inventory inventory = self.body.inventory;
                                if (inventory)
                                {
                                    int itemCount = inventory.GetItemCount(RoR2Content.Items.ExtraLifeConsumed);
                                    if (itemCount > 0 && Util.CheckRoll(Util.ConvertAmplificationPercentageIntoReductionPercentage(MainPlugin.Dio_BlockChance * itemCount), 0f, null))
                                    {
                                        EffectData effectData = new EffectData
                                        {
                                            origin = damageInfo.position,
                                            rotation = Util.QuaternionSafeLookRotation((damageInfo.force != Vector3.zero) ? damageInfo.force : UnityEngine.Random.onUnitSphere)
                                        };
                                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.bearEffectPrefab, effectData, true);
                                        damageInfo.rejected = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}
