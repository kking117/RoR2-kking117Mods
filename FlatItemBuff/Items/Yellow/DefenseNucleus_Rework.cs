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
    public class DefenseNucleus_Rework
    {
        private const string LogName = "Defense Nucleus Rework";
        internal static bool Enable = false;
        internal static float ShieldBaseDuration = 5f;
        internal static float ShieldStackDuration = 1f;
        internal static float SummonCooldown = 7f;
        internal static int BaseHealth = 10;
        internal static int StackHealth = 10;
        internal static int BaseAttack = 3;
        internal static int StackAttack = 0;
        internal static int BaseDamage = 0;
        internal static int StackDamage = 5;
        internal static int SummonCount = 3;
        public DefenseNucleus_Rework()
        {
            if (!Enable)
            {
                new Items.DefenseNucleus();
                return;
            }
            MainPlugin.ModLogger.LogInfo(LogName);
            ClampConfig();
            
            UpdateText();
            UpdateItemDef();
            Hooks();
            DefenseNucleus_Shared.EnableChanges();
        }
        private void ClampConfig()
        {
            BaseHealth = Math.Max(0, BaseHealth);
            StackHealth = Math.Max(0, StackHealth);
            BaseAttack = Math.Max(0, BaseAttack);
            StackAttack = Math.Max(0, StackAttack);
            BaseDamage = Math.Max(0, BaseDamage);
            StackDamage = Math.Max(0, StackDamage);
            SummonCount = Math.Clamp(SummonCount, 0, 8);
        }
        private void UpdateItemDef()
        {
            ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/MinorConstructOnKill/MinorConstructOnKill.asset").WaitForCompletion();
            if (itemDef)
            {
                List<ItemTag> itemtags = itemDef.tags.ToList();
                itemtags.Add(ItemTag.EquipmentRelated);
                itemtags.Add(ItemTag.CannotCopy);
                itemtags.Remove(ItemTag.OnKillEffect);
                itemDef.tags = itemtags.ToArray();
            }
        }
        private void UpdateText()
        {
            MainPlugin.ModLogger.LogInfo("Updating Text");
            string deployDesc = string.Format("Activating your equipment");
            string shieldDesc = "";
            string summonDesc = "";

            if (ShieldBaseDuration > 0f)
            {
                if (ShieldStackDuration > 0f)
                {
                    shieldDesc += string.Format(" generates a <style=cIsUtility>projectile shield</style> for <style=cIsUtility>{0}s</style> <style=cStack>(+{1}s per stack)</style>.", ShieldBaseDuration, ShieldStackDuration);
                }
                else
                {
                    shieldDesc += string.Format(" generates a <style=cIsUtility>projectile shield</style> for <style=cIsUtility>{0}s</style>.", ShieldBaseDuration);
                }
            }
            if (SummonCount > 0)
            {
                if (ShieldBaseDuration > 0f)
                {
                    summonDesc += string.Format(" Additionally you deploy <style=cIsUtility>{0}</style> <style=cIsDamage>Alpha Constructs</style>", SummonCount);
                }
                else
                {
                    summonDesc += string.Format(" deploys <style=cIsUtility>{0}</style> <style=cIsDamage>Alpha Constructs</style>", SummonCount);
                }
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
                    summonDesc += stats + ".";
                }
                else
                {
                    summonDesc += string.Format(".");
                }
            }

            string pickupText = string.Format("Launch additional defensive measures on equipment activation.");
            string descriptionText = string.Format("{0}{1}{2}", deployDesc, shieldDesc, summonDesc);

            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_PICKUP", pickupText);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_DESC", descriptionText);
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
            CharacterBody body = self.characterBody;
            if (body)
            {
                CharacterMaster master = body.master;
                if (master)
                {
                    int itemCount = master.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
                    if (itemCount > 0)
                    {
                        if (SummonCount > 0)
                        {
                            Construct_CooldownManager comp = master.GetComponent<Construct_CooldownManager>();
                            if (!comp)
                            {
                                comp = master.gameObject.AddComponent<Construct_CooldownManager>();
                                comp.duration = SummonCooldown;
                                comp.maxSummons = SummonCount;
                                int summonCount = master.GetDeployableSameSlotLimit(DeployableSlot.MinorConstructOnKill);
                                DeployConstructs(master, summonCount);
                            }
                        }
                        if (ShieldBaseDuration > 0f)
                        {
                            DefenseNucleusShield comp = body.GetComponent<DefenseNucleusShield>();
                            if (!comp)
                            {
                                comp = body.gameObject.AddComponent<DefenseNucleusShield>();
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
                Vector3 normalized = Vector3.ProjectOnPlane(body.transform.forward, Vector3.up).normalized;
                Vector3 angledVector = Vector3.RotateTowards(Vector3.up, normalized, 0.45f, float.PositiveInfinity);
                for (int i = 0; i < summonCount; i++)
                {
                    Vector3 forward = Quaternion.AngleAxis(baseAngle, Vector3.up) * angledVector;
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = GlobalEventManager.CommonAssets.minorConstructOnKillProjectile,
                        position = body.transform.position + forward,
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