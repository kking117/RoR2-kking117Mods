using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using FlatItemBuff.Components;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
    class DefenseNucleus_Rework
    {
        internal static bool Enable = false;
        internal static float ShieldBaseDuration = 3.5f;
        internal static float ShieldStackDuration = 1f;
        internal static int BaseHealth = 10;
        internal static int StackHealth = 10;
        internal static int BaseAttack = 6;
        internal static int StackAttack = 0;
        internal static int BaseDamage = 6;
        internal static int StackDamage = 8;
        internal static int SummonCount = 3;
        public DefenseNucleus_Rework()
        {
            if (!Enable)
            {
                new Items.DefenseNucleus();
                return;
            }
            MainPlugin.ModLogger.LogInfo("Changing Defense Nucleus");
            SummonCount = Math.Min(6, SummonCount);
            UpdateText();
            UpdateItemDef();
            Hooks();
            DefenseNucleus_Shared.EnableChanges();
        }
        private void UpdateItemDef()
        {
            ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/MinorConstructOnKill/MinorConstructOnKill.asset").WaitForCompletion();
            if (itemDef)
            {
                List<ItemTag> itemtags = itemDef.tags.ToList();
                itemtags.Add(ItemTag.EquipmentRelated);
                itemtags.Remove(ItemTag.OnKillEffect);
                itemDef.tags = itemtags.ToArray();
            }
        }
        private void UpdateText()
        {
            MainPlugin.ModLogger.LogInfo("Updating item text");
            string pickup = string.Format("Launch additional defensive measures on equipment activation.");
            string desc = string.Format("Activating your equipment deploys ");
            if (ShieldBaseDuration > 0f)
            {
                desc += string.Format("a <style=cIsUtility>projectile shield for <style=cIsUtility>{0}</style> <style=cStack>(+{1} per stack)</style> seconds</style>.", ShieldBaseDuration, ShieldStackDuration);
            }
            if (SummonCount > 0)
            {
                string stats = "";
                List<string> statList = new List<string>();
                if (BaseHealth > 0 || StackHealth > 0)
                {
                    stats = string.Format("<style=cIsHealing>{0}%", (10 + BaseHealth) * 10f);
                    if (StackHealth > 0)
                    {
                        stats += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackHealth * 10f);
                    }
                    stats += " health</style>";
                    statList.Add(stats);
                }
                if (BaseDamage > 0 || BaseDamage > 0)
                {
                    stats = string.Format("<style=cIsDamage>{0}%", (10 + BaseDamage) * 10f);
                    if (StackDamage > 0)
                    {
                        stats += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 10f);
                    }
                    stats += " damage</style>";
                    statList.Add(stats);
                }
                if (BaseAttack > 0 || StackAttack > 0)
                {
                    stats = string.Format("<style=cIsDamage>{0}%", (10 + BaseAttack) * 10f);
                    if (StackAttack > 0)
                    {
                        stats += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackAttack * 10f);
                    }
                    stats += " attack speed</style>";
                    statList.Add(stats);
                }
                stats = "";
                for (int i = 0; i < statList.Count; i++)
                {
                    if (i == statList.Count - 1)
                    {
                        stats += " and ";
                    }
                    else if (i > 0)
                    {
                        stats += ", ";
                    }
                    stats += statList[i];
                }
                if (stats.Length > 0)
                {
                    stats = " with " + stats;
                }
                if (ShieldBaseDuration > 0f)
                {
                    desc += " It also deploys ";
                }
                desc += string.Format("<style=cIsUtility>{0}</style> <style=cIsDamage>Alpha Constructs</style>{1}.", SummonCount, stats);
            }
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_PICKUP", pickup);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_DESC", desc);
        }
        private void Hooks()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            On.RoR2.EquipmentSlot.OnEquipmentExecuted += Equipment_OnExecuted;
            On.RoR2.CharacterMaster.Start += CharacterMaster_Start;
        }
        private void Equipment_OnExecuted(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
        {
            orig(self);
            if (!NetworkServer.active)
            {
                return;
            }
            if (self.characterBody)
            {
                CharacterMaster master = self.characterBody.master;
                if (master)
                {
                    int itemCount = master.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
                    if (itemCount > 0)
                    {
                        if (SummonCount > 0)
                        {
                            DefenseNucleusSummonCooldown comp = self.characterBody.GetComponent<DefenseNucleusSummonCooldown>();
                            if (!comp)
                            {
                                int summonCount = master.GetDeployableSameSlotLimit(DeployableSlot.MinorConstructOnKill);
                                DeployConstructs(master, summonCount);
                                comp = self.characterBody.gameObject.AddComponent<DefenseNucleusSummonCooldown>();
                            }
                        }
                        if (ShieldBaseDuration > 0f)
                        {
                            DefenseNucleusShield comp = self.characterBody.GetComponent<DefenseNucleusShield>();
                            if (!comp)
                            {
                                comp = self.characterBody.gameObject.AddComponent<DefenseNucleusShield>();
                            }
                            comp.duration = ShieldBaseDuration + (ShieldStackDuration * (itemCount - 1));
                        }
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
            return SummonCount;
        }
        internal void SetupConstructInventory(CharacterMaster self, CharacterMaster owner)
        {
            int stackbonus = owner.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) - 1;
            int hpitem = BaseHealth + (StackHealth * stackbonus);
            int atkitem = BaseAttack + (StackAttack * stackbonus);
            int dmgitem = BaseDamage + (StackDamage * stackbonus);
            self.inventory.GiveItem(RoR2Content.Items.BoostHp, hpitem);
            self.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, atkitem);
            self.inventory.GiveItem(RoR2Content.Items.BoostDamage, dmgitem);
        }
        private void DeployConstructs(CharacterMaster owner, int summonCount)
        {
            CharacterBody body = owner.GetBody();
            if (body)
            {
                float baseAngle = UnityEngine.Random.Range(0, 360);
                float rotAngle = 360.0f / summonCount;
                for (int i = 0; i < summonCount; i++)
                {
                    Vector3 forward = Quaternion.AngleAxis(baseAngle, Vector3.up) * Vector3.forward;
                    Vector3 spawnOffset = forward + Vector3.up;
                    forward = Quaternion.AngleAxis(baseAngle, Vector3.up) * Quaternion.AngleAxis(-80f, Vector3.right) * Vector3.forward;
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = GlobalEventManager.CommonAssets.minorConstructOnKillProjectile,
                        position = body.transform.position + spawnOffset,
                        rotation = Util.QuaternionSafeLookRotation(forward),
                        procChainMask = default(ProcChainMask),
                        target = body.gameObject,
                        owner = body.gameObject,
                        damage = 0f,
                        crit = false,
                        force = 0f,
                        damageColorIndex = DamageColorIndex.Item
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                    baseAngle += rotAngle;
                }
            }
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