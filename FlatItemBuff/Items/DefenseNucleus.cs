using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using RoR2.Projectile;
using FlatItemBuff.Components;
using UnityEngine.Networking;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
    class DefenseNucleus
    {
        private static float Cooldown = 1.5f;
        private static int BaseHealth = 10;
        private static int StackHealth = 10;
        private static int BaseAttack = 6;
        private static int StackAttack = 0;
        private static int BaseDamage = 6;
        private static int StackDamage = 8;
        public DefenseNucleus()
        {
            SetupConfigValues();
            MainPlugin.ModLogger.LogInfo("Changing Defense Nucleus");
            UpdateText();
            Hooks();
            DefenseNucleus_Shared.EnableChanges();
        }
        private void SetupConfigValues()
        {
            if (MainPlugin.Nucleus_Cooldown.Value < 0.25f)
            {
                MainPlugin.Nucleus_Cooldown.Value = 0.25f;
            }
            Cooldown = MainPlugin.Nucleus_Cooldown.Value;

            BaseHealth = MainPlugin.Nucleus_BaseHealth.Value;
            StackHealth = MainPlugin.Nucleus_StackHealth.Value;

            BaseAttack = MainPlugin.Nucleus_BaseAttack.Value;
            StackAttack = MainPlugin.Nucleus_StackAttack.Value;

            BaseDamage = MainPlugin.Nucleus_BaseDamage.Value;
            StackDamage = MainPlugin.Nucleus_StackDamage.Value;
        }
        private void UpdateText()
        {
            MainPlugin.ModLogger.LogInfo("Updating item text");
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
            string pickup = string.Format("Summon an Alpha Construct on kill.");
            string desc = string.Format("On kill spawn an <style=cIsDamage>Alpha Construct</style>{0}. Limited to <style=cIsUtility>4</style>.", stats);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_PICKUP", pickup);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_DESC", desc);
        }
        private void Hooks()
        {
            MainPlugin.ModLogger.LogInfo("Applying IL modifications");
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEvent_CharacterDeath;
            On.RoR2.CharacterMaster.Start += CharacterMaster_Start;
        }
        private void GlobalEvent_CharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if(damageReport.attackerMaster)
            {
                if (damageReport.victimBody)
                {
                    if (damageReport.attackerMaster.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) > 0)
                    {
                        CharacterMaster deployer = damageReport.attackerMaster;
                        if (deployer && deployer.GetBody())
                        {
                            if (deployer.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) > 0)
                            {
                                DefenseNucleusSummonCooldown comp = deployer.GetComponent<DefenseNucleusSummonCooldown>();
                                if (!comp)
                                {
                                    comp = deployer.gameObject.AddComponent<DefenseNucleusSummonCooldown>();
                                    comp.duration = Cooldown;
                                    DeployConstructFromCorpse(deployer, damageReport.victimBody);
                                }
                            }
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
                        SummonDeclutter component = self.gameObject.AddComponent<SummonDeclutter>();
                        component.slot = DeployableSlot.MinorConstructOnKill;
                    }
                }
            }
        }
    }
}