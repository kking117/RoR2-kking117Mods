using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using FlatItemBuff.Components;
using FlatItemBuff.Utils;

namespace FlatItemBuff.ItemChanges
{
    class DefenseNucleus_Rework
    {
        public static GameObject ShieldPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MegaConstructBubbleShield.prefab").WaitForCompletion();
        public static GameObject SummonProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MinorConstructOnKill/MinorConstructOnKillProjectile.prefab").WaitForCompletion();
        public static void EnableChanges()
        {
            MainPlugin.ModLogger.LogInfo("Changing Defense Nucleus");
            MainPlugin.NucleusRework_SummonCount.Value = Math.Max(0, MainPlugin.NucleusRework_SummonCount.Value);
            MainPlugin.NucleusRework_SummonCount.Value = Math.Min(6, MainPlugin.NucleusRework_SummonCount.Value);
            UpdateText();
            UpdateItemDef();
            Hooks();
            DefenseNucleus_Shared.EnableChanges();
        }
        private static void UpdateText()
        {
            MainPlugin.ModLogger.LogInfo("Updating item text");
            string pickup = string.Format("Launch additional defensive measures on equipment activation.");
            string desc = string.Format("Activating your equipment deploys ");
            bool addto = false;
            if (MainPlugin.NucleusRework_SummonCount.Value > 0)
            {
                desc += string.Format("<style=cIsUtility>{0}</style> <style=cIsDamage>Alpha Constructs</style> with <style=cIsHealing>{1}% <style=cStack>(+{2}% per stack)</style> health</style> and <style=cIsDamage>{3}% <style=cStack>(+{4}% per stack)</style> attack speed</style>.", MainPlugin.NucleusRework_SummonCount.Value, (10 + MainPlugin.NucleusRework_BaseHealth.Value) * 10f, MainPlugin.NucleusRework_StackHealth.Value * 10f, (10 + MainPlugin.NucleusRework_BaseAttack.Value) * 10f, MainPlugin.NucleusRework_StackAttack.Value * 10f);
                addto = true;
            }
            if (MainPlugin.NucleusRework_ShieldBaseDuration.Value > 0f)
            {
                if(addto)
                {
                    desc += string.Format(" Also forms a <style=cIsUtility>projectile shield for <style=cIsUtility>{0}</style> <style=cStack>(+{1} per stack)</style> seconds</style>.", MainPlugin.NucleusRework_ShieldBaseDuration.Value, MainPlugin.NucleusRework_ShieldStackDuration.Value);
                }
                else
                {
                    desc += string.Format("a <style=cIsUtility>projectile shield for <style=cIsUtility>{0}</style> <style=cStack>(+{1} per stack)</style> seconds</style>.", MainPlugin.NucleusRework_ShieldBaseDuration.Value, MainPlugin.NucleusRework_ShieldStackDuration.Value);
                }
            }
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_PICKUP", pickup);
            LanguageAPI.Add("ITEM_MINORCONSTRUCTONKILL_DESC", desc);
        }
        private static void UpdateItemDef()
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
        private static void Hooks()
        {
            MainPlugin.ModLogger.LogInfo("Applying IL modifications");
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            if (MainPlugin.NucleusRework_SummonCount.Value > 0)
            {
                On.RoR2.CharacterMaster.Start += CharacterMaster_Start;
            }
            On.RoR2.EquipmentSlot.OnEquipmentExecuted += Equipment_OnExecuted;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
        }
        private static void IL_OnCharacterDeath(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchLdloc(x, 16),
                x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Items", "MinorConstructOnKill")
            );
            ilcursor.Index += 3;
            ilcursor.Remove();
            ilcursor.Emit(OpCodes.Ldloc, 16);
            ilcursor.EmitDelegate<Func<Inventory, int>>((inventory) =>
            {
                return inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
            });
        }
        private static void Equipment_OnExecuted(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
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
                        if (MainPlugin.NucleusRework_SummonCount.Value > 0)
                        {
                            DefenseNucleusSummonCooldown comp = self.characterBody.GetComponent<DefenseNucleusSummonCooldown>();
                            if (!comp)
                            {
                                int summonCount = master.GetDeployableSameSlotLimit(DeployableSlot.MinorConstructOnKill);
                                DeployConstructs(master, summonCount);
                                comp = self.characterBody.gameObject.AddComponent<DefenseNucleusSummonCooldown>();
                            }
                        }
                        if (MainPlugin.NucleusRework_ShieldBaseDuration.Value > 0f)
                        {
                            DefenseNucleusShield comp = self.characterBody.GetComponent<DefenseNucleusShield>();
                            if (!comp)
                            {
                                comp = self.characterBody.gameObject.AddComponent<DefenseNucleusShield>();
                            }
                            comp.duration = MainPlugin.NucleusRework_ShieldBaseDuration.Value + (MainPlugin.NucleusRework_ShieldStackDuration.Value * (itemCount - 1));
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
            return MainPlugin.NucleusRework_SummonCount.Value;
        }
        private static void CharacterMaster_Start(On.RoR2.CharacterMaster.orig_Start orig, CharacterMaster self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if (self)
                {
                    CharacterMaster nukeowner = Helpers.GetOwnerAsDeployable(self, DeployableSlot.MinorConstructOnKill);
                    if (nukeowner)
                    {
                        int stackbonus = nukeowner.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) - 1;
                        int hpitem = MainPlugin.NucleusRework_BaseHealth.Value + (MainPlugin.NucleusRework_StackHealth.Value * stackbonus);
                        int atkitem = MainPlugin.NucleusRework_BaseAttack.Value + (MainPlugin.NucleusRework_StackAttack.Value * stackbonus);
                        if (atkitem > 50)
                        {
                            self.inventory.GiveItem(RoR2Content.Items.BoostDamage, atkitem-50);
                            atkitem = 50;
                        }
                        self.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, atkitem);
                        self.inventory.GiveItem(RoR2Content.Items.BoostHp, hpitem);
                    }
                }
            }
        }
        private static void DeployConstructs(CharacterMaster owner, int summonCount)
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
                        projectilePrefab = SummonProjectile,
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
    }
}