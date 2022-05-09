using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using FlatItemBuff.Utils;
using FlatItemBuff.Components;

namespace FlatItemBuff.ItemChanges
{
    class DefenseNucleus
    {
        public static GameObject SummonProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MinorConstructOnKill/MinorConstructOnKillProjectile.prefab").WaitForCompletion();
        public static void EnableChanges()
        {
            MainPlugin.ModLogger.LogInfo("Changing Defense Nucleus");
            UpdateText();
            Hooks();
            DefenseNucleus_Shared.EnableChanges();
        }
        private static void UpdateText()
        {
            MainPlugin.ModLogger.LogInfo("Updating item text");
            string pickup = string.Format("Summon an Alpha Construct on kill.");
            string desc = string.Format("On kill spawn an <style=cIsDamage>Alpha Construct</style> with <style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style> health</style> and <style=cIsDamage>{2}% <style=cStack>(+{3}% per stack)</style> attack speed</style>. Limited to <style=cIsUtility>4</style>.", (10 + MainPlugin.Nucleus_BaseHealth.Value) * 10f, MainPlugin.Nucleus_StackHealth.Value * 10f, (10 + MainPlugin.Nucleus_BaseAttack.Value) * 10f, MainPlugin.Nucleus_StackAttack.Value * 10f);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_PICKUP", pickup);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_DESC", desc);
        }
        private static void Hooks()
        {
            MainPlugin.ModLogger.LogInfo("Applying IL modifications");
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEvent_CharacterDeath;
        }
        private static void GlobalEvent_CharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if(damageReport.attackerMaster)
            {
                if (damageReport.victimBody)
                {
                    CharacterMaster deployer = null;
                    CharacterMaster owner = null;
                    if (MainPlugin.Nucleus_Infinite.Value)
                    {
                        owner = Helpers.GetOwnerAsDeployable(damageReport.attackerMaster, DeployableSlot.MinorConstructOnKill);
                    }
                    if (owner)
                    {
                        deployer = owner;
                    }
                    else if (damageReport.attackerMaster.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) > 0)
                    {
                        MinionOwnership ownership = damageReport.attackerMaster.minionOwnership;
                        deployer = damageReport.attackerMaster;
                        if (ownership)
                        {
                            owner = Helpers.GetTrueOwner(ownership);
                            if (owner)
                            {
                                deployer = owner;
                            }
                        }
                    }
                    if (deployer && deployer.GetBody())
                    {
                        if (deployer.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) > 0)
                        {
                            DefenseNucleusSummonCooldown comp = deployer.GetComponent<DefenseNucleusSummonCooldown>();
                            if (!comp)
                            {
                                comp = deployer.gameObject.AddComponent<DefenseNucleusSummonCooldown>();
                                comp.duration = 0.25f;
                                DeployConstructFromCorpse(deployer, damageReport.victimBody);
                            }
                        }
                    }
                }
            }
        }
        private static int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            var result = orig(self, slot);
            if (slot != DeployableSlot.MinorConstructOnKill)
            {
                return result;
            }
            return 4;
        }
        internal static void SetupConstructInventory(CharacterMaster self, CharacterMaster owner)
        {
            int stackbonus = owner.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) - 1;
            int hpitem = MainPlugin.Nucleus_BaseHealth.Value + (MainPlugin.Nucleus_StackHealth.Value * stackbonus);
            int atkitem = MainPlugin.Nucleus_BaseAttack.Value + (MainPlugin.Nucleus_StackAttack.Value * stackbonus);
            if (atkitem > 50)
            {
                self.inventory.GiveItem(RoR2Content.Items.BoostDamage, atkitem - 50);
                atkitem = 50;
            }
            self.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, atkitem);
            self.inventory.GiveItem(RoR2Content.Items.BoostHp, hpitem);
        }
        private static void DeployConstructFromCorpse(CharacterMaster owner, CharacterBody victim)
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
    }
}