using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using RoR2.Projectile;
using FlatItemBuff.Components;
using UnityEngine.Networking;
using FlatItemBuff.Utils;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
    public class DefenseNucleus
    {
        internal static bool Enable = true;
        internal static int MaxSummons = 4;
        internal static float SummonCooldown = 1f;
        internal static float SummonFullCooldown = 6f;
        internal static int BaseHealth = 10;
        internal static int StackHealth = 10;
        internal static int BaseAttack = 3;
        internal static int StackAttack = 0;
        internal static int BaseDamage = 0;
        internal static int StackDamage = 5;
        internal static bool Comp_AssistManager = false;
        public DefenseNucleus()
        {
            if (!Enable)
            {
                return;
            }
            MainPlugin.ModLogger.LogInfo("Changing Defense Nucleus");
            ClampConfig();
            UpdateText();
            UpdateItemDef();
            Hooks();
            if (MainPlugin.AssistManager_Loaded)
            {
                ApplyAssistManager();
            }
            DefenseNucleus_Shared.EnableChanges();
        }
        private void ApplyAssistManager()
        {
            if (Comp_AssistManager)
            {
                AssistManager.AssistManager.HandleAssistInventoryCompatibleActions += AssistManger_OnKill;
            }
        }
        private void ClampConfig()
        {
            BaseHealth = Math.Max(0, BaseHealth);
            StackHealth = Math.Max(0, StackHealth);
            BaseAttack = Math.Max(0, BaseAttack);
            StackAttack = Math.Max(0, StackAttack);
            BaseDamage = Math.Max(0, BaseDamage);
            StackDamage = Math.Max(0, StackDamage);
        }
        private void UpdateItemDef()
        {
            ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/MinorConstructOnKill/MinorConstructOnKill.asset").WaitForCompletion();
            if (itemDef)
            {
                List<ItemTag> itemtags = itemDef.tags.ToList();
                itemtags.Add(ItemTag.CannotCopy);
                itemDef.tags = itemtags.ToArray();
            }
        }
        private void UpdateText()
        {
            MainPlugin.ModLogger.LogInfo("Updating item text");
            string deployDesc = string.Format("Activating your equipment");
            string statDesc = "";

            if (StackHealth > 0 || StackAttack > 0 || StackDamage > 0)
            {
                string stats = "";
                List<string> statList = new List<string>();

                if (StackHealth > 0)
                {
                    stats = string.Format(" <style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style> health</style>", (10 + BaseHealth) * 10, StackHealth * 10);
                    statList.Add(stats);
                }
                if (StackAttack > 0)
                {
                    stats = string.Format(" <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style> attack speed</style>", (10 + BaseAttack) * 10, StackAttack * 10);
                    statList.Add(stats);
                }
                if (StackDamage > 0)
                {
                    stats = string.Format(" <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style> damage</style>", (10 + BaseDamage) * 10, StackDamage * 10);
                    statList.Add(stats);
                }
                stats = " with";
                for (int i = 0; i < statList.Count; i++)
                {
                    if (i == statList.Count - 1)
                    {
                        stats += " and";
                    }
                    else if (i > 0)
                    {
                        stats += ",";
                    }
                    stats += statList[i];
                }
                statDesc += stats;
            }

            string pickupText = string.Format("Summon an Alpha Construct on kill.");
            string descriptionText = string.Format("On kill spawn an <style=cIsDamage>Alpha Construct</style>{0}. Limited to <style=cIsUtility>4</style>.", statDesc);

            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_PICKUP", pickupText);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_DESC", descriptionText);
        }
        private void Hooks()
        {
            MainPlugin.ModLogger.LogInfo("Applying IL modifications");
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
            On.RoR2.CharacterMaster.Start += CharacterMaster_Start;
        }
        private void AssistManger_OnKill(CharacterBody assistBody, CharacterBody victimBody, DamageType? damageType, HashSet<R2API.DamageAPI.ModdedDamageType> modDamageType, Inventory assistInventory, CharacterBody killerBody, DamageInfo damageInfo)
        {
            if (assistBody == killerBody)
            {
                return;
            }

            int itemCount = assistInventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
            if (itemCount > 0)
            {
                CharacterMaster assistMaster = assistBody.master;
                Construct_CooldownManager comp = assistMaster.GetComponent<Construct_CooldownManager>();
                if (!comp)
                {
                    comp = assistMaster.gameObject.AddComponent<Construct_CooldownManager>();
                    comp.duration = SummonCooldown;
                    comp.maxSummons = 4;
                    if (assistMaster.GetDeployableCount(DeployableSlot.MinorConstructOnKill) + 1 >= MaxSummons)
                    {
                        comp.duration += SummonFullCooldown;
                    }
                    DeployConstructFromCorpse(assistMaster, victimBody);
                }
            }
        }
        private void GlobalKillEvent(DamageReport damageReport)
        {
            CharacterMaster attackerMaster = damageReport.attackerMaster;
            if (damageReport.victimBody && attackerMaster)
            {
                if (attackerMaster.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) > 0)
                {
                    Construct_CooldownManager comp = attackerMaster.GetComponent<Construct_CooldownManager>();
                    if (!comp)
                    {
                        comp = attackerMaster.gameObject.AddComponent<Construct_CooldownManager>();
                        comp.duration = SummonCooldown;
                        comp.maxSummons = 4;
                        if (attackerMaster.GetDeployableCount(DeployableSlot.MinorConstructOnKill) + 1 >= MaxSummons)
                        {
                            comp.duration += SummonFullCooldown;
                        }
                        DeployConstructFromCorpse(attackerMaster, damageReport.victimBody);
                    }
                }
            }
        }
        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            var result = orig(self, slot);
            if (slot != DeployableSlot.MinorConstructOnKill)
            {
                return result;
            }
            return 4;
        }
        internal void SetupConstructInventory(CharacterMaster self, CharacterMaster owner)
        {
            int stackbonus = owner.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) - 1;
            int hpitem = BaseHealth + (StackHealth * stackbonus);
            int atkitem = BaseAttack + (StackAttack * stackbonus);
            int dmgitem = BaseDamage + (StackDamage * stackbonus);
            self.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, atkitem);
            self.inventory.GiveItem(RoR2Content.Items.BoostHp, hpitem);
            self.inventory.GiveItem(RoR2Content.Items.BoostDamage, dmgitem);
        }
        private void DeployConstructFromCorpse(CharacterMaster owner, CharacterBody victim)
        {
            Vector3 forward = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up) * Quaternion.AngleAxis(-80f, Vector3.right) * Vector3.forward;
            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
            {
                projectilePrefab = GlobalEventManager.CommonAssets.minorConstructOnKillProjectile,
                position = victim.transform.position,
                rotation = Util.QuaternionSafeLookRotation(forward),
                procChainMask = default(ProcChainMask),
                target = victim.healthComponent.gameObject,
                owner = owner.GetBody().gameObject,
                damage = 0f,
                crit = false,
                force = 0f,
                damageColorIndex = DamageColorIndex.Item
            };
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }
        private void CharacterMaster_Start(On.RoR2.CharacterMaster.orig_Start orig, CharacterMaster self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if (self)
                {
                    CharacterMaster owner = Helpers.GetOwnerAsDeployable(self, DeployableSlot.MinorConstructOnKill);
                    if (owner)
                    {
                        SetupConstructInventory(self, owner);
                    }
                }
            }
        }
    }
}