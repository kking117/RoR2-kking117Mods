using System;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.CharacterAI;
using UnityEngine.Networking;
using RoR2.Projectile;
using FlatItemBuff.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.ItemChanges
{
    public class DefenseNucleus_Shared
    {
        public static GameObject ConstructBodyObject;
        public static BodyIndex ConstructBodyIndex = BodyIndex.None;
        public static void EnableChanges()
        {
            if (MainPlugin.NucleusShared_TweakAI.Value)
            {
                UpdateAI();
            }
            if (MainPlugin.NucleusShared_BlastRadius.Value > 0f && MainPlugin.NucleusShared_BlastDamage.Value > 0f)
            {
                On.RoR2.CharacterMaster.OnBodyDeath += CharacterMaster_OnBodyDeath;
            }
            On.RoR2.Projectile.ProjectileSpawnMaster.SpawnMaster += Projectile_SpawnMaster;
            MainPlugin.ModLogger.LogInfo("Applying IL modifications");
            IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
        }
        private static void IL_OnCharacterDeath(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchLdloc(x, 17),
                x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Items", "MinorConstructOnKill")
            );
            ilcursor.Index += 3;
            ilcursor.Emit(OpCodes.Ldc_I4_0);
            ilcursor.Emit(OpCodes.Mul);

            /*ilcursor.Index += 3;
            ilcursor.Remove();
            ilcursor.Emit(OpCodes.Ldloc, 16);
            ilcursor.EmitDelegate<Func<Inventory, int>>((inventory) =>
            {
                return inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
            });*/
        }
        public static void ExtraChanges()
        {
            ConstructBodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MinorConstructOnKillBody.prefab").WaitForCompletion();
            if(ConstructBodyObject)
            {
                CharacterBody body = ConstructBodyObject.GetComponent<CharacterBody>();
                if (body)
                {
                    ConstructBodyIndex = body.bodyIndex;
                    if (MainPlugin.NucleusShared_Mechanical.Value)
                    {
                        body.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
                    }
                }
                if (MainPlugin.NucleusShared_ExtraDisplays.Value)
                {
                    ItemDisplayRuleSet itemdisplayruleSet = ConstructBodyObject.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;
                    DisplayRuleGroup displayruleGroup = ConstructBodyObject.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.keyAssetRuleGroups[0].displayRuleGroup;
                    GameObject ArmPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponRobotArm.prefab").WaitForCompletion();
                    GameObject MiniGunPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponMinigun.prefab").WaitForCompletion();
                    GameObject LauncherPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponLauncher.prefab").WaitForCompletion();
                    if (itemdisplayruleSet)
                    {
                        ItemDisplayRuleSet.KeyAssetRuleGroup droneBoostDisplay = new ItemDisplayRuleSet.KeyAssetRuleGroup
                        {
                            keyAsset = DLC1Content.Items.DroneWeaponsBoost,
                            displayRuleGroup = new DisplayRuleGroup
                            {
                                rules = new ItemDisplayRule[]
                                {
                                    new ItemDisplayRule
                                    {
                                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                                        followerPrefab = LauncherPrefab,
                                        childName = "CapTop",
                                        localPos = new Vector3(0F, 0.25F, -0.3F),
                                        localAngles = new Vector3(330F, 180F, 0F),
                                        localScale = new Vector3(1F, 1F, 1F),
                                        limbMask = LimbFlags.None
                                    }
                                }
                            }
                        };
                        ItemDisplayRuleSet.KeyAssetRuleGroup dronePart1Display = new ItemDisplayRuleSet.KeyAssetRuleGroup
                        {
                            keyAsset = DLC1Content.Items.DroneWeaponsDisplay1,
                            displayRuleGroup = new DisplayRuleGroup
                            {
                                rules = new ItemDisplayRule[]
                                {
                                    new ItemDisplayRule
                                    {
                                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                                        followerPrefab = MiniGunPrefab,
                                        childName = "CapTop",
                                        localPos = new Vector3(0F, 0.75F, 0F),
                                        localAngles = new Vector3(0F, 0F, 180F),
                                        localScale = new Vector3(1F, 1F, 1F),
                                        limbMask = LimbFlags.None
                                    }
                                }
                            }
                        };
                        ItemDisplayRuleSet.KeyAssetRuleGroup dronePart2Display = new ItemDisplayRuleSet.KeyAssetRuleGroup
                        {
                            keyAsset = DLC1Content.Items.DroneWeaponsDisplay2,
                            displayRuleGroup = new DisplayRuleGroup
                            {
                                rules = new ItemDisplayRule[]
                                {
                                    new ItemDisplayRule
                                    {
                                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                                        followerPrefab = ArmPrefab,
                                        childName = "CapTop",
                                        localPos = new Vector3(0F, 0.8F, 0F),
                                        localAngles = new Vector3(270F, 0F, 0F),
                                        localScale = new Vector3(1.5F, 1.5F, 1.5F),
                                        limbMask = LimbFlags.None
                                    }
                                }
                            }
                        };
                        Array.Resize(ref itemdisplayruleSet.keyAssetRuleGroups, itemdisplayruleSet.keyAssetRuleGroups.Length + 3);
                        itemdisplayruleSet.keyAssetRuleGroups[itemdisplayruleSet.keyAssetRuleGroups.Length - 3] = droneBoostDisplay;
                        itemdisplayruleSet.keyAssetRuleGroups[itemdisplayruleSet.keyAssetRuleGroups.Length - 2] = dronePart1Display;
                        itemdisplayruleSet.keyAssetRuleGroups[itemdisplayruleSet.keyAssetRuleGroups.Length - 1] = dronePart2Display;
                        itemdisplayruleSet.GenerateRuntimeValues();
                    }
                }
            }
        }
        private static void UpdateAI()
        {
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MinorConstructOnKillMaster.prefab").WaitForCompletion();
            BaseAI baseai = prefab.GetComponent<BaseAI>();
            if (baseai)
            {
                baseai.fullVision = true;
                baseai.neverRetaliateFriendlies = true;
            }
        }
        private static void Projectile_SpawnMaster(On.RoR2.Projectile.ProjectileSpawnMaster.orig_SpawnMaster orig, ProjectileSpawnMaster self)
        {
            if (self.deployableSlot == DeployableSlot.MinorConstructOnKill)
            {
                ProjectileController projController = self.GetComponent<ProjectileController>();
                if (projController && projController.owner)
                {
                    CharacterBody ownerbody = projController.owner.GetComponent<CharacterBody>();
                    if (ownerbody && ownerbody.master)
                    {
                        int killCount = ownerbody.master.GetDeployableCount(DeployableSlot.MinorConstructOnKill) + 1 - ownerbody.master.GetDeployableSameSlotLimit(DeployableSlot.MinorConstructOnKill);
                        if (killCount > 0)
                        {
                            if(MainPlugin.NucleusRework_Enable.Value)
                            {
                                Helpers.KillDeployables(ownerbody.master, DeployableSlot.MinorConstructOnKill, killCount);
                            }
                            else
                            {
                                Helpers.KillDeployableInRange(ownerbody.master, DeployableSlot.MinorConstructOnKill, killCount, self.transform.position, 6f, true);
                            }
                        }
                    }
                }
            }
            orig(self);
        }
        private static void CharacterMaster_OnBodyDeath(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (NetworkServer.active)
            {
                if (body.bodyIndex == ConstructBodyIndex)
                {
                    float damage = body.damage * MainPlugin.NucleusShared_BlastDamage.Value;
                    float radius = MainPlugin.NucleusShared_BlastRadius.Value;
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                    {
                        origin = body.transform.position,
                        scale = radius,
                        rotation = Util.QuaternionSafeLookRotation(body.transform.forward)
                    }, true);
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.position = body.transform.position;
                    blastAttack.baseDamage = damage;
                    blastAttack.baseForce = 0f;
                    blastAttack.radius = radius;
                    blastAttack.attacker = body.gameObject;
                    blastAttack.inflictor = null;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    blastAttack.crit = false;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.damageColorIndex = DamageColorIndex.Default;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.damageType = DamageType.SlowOnHit;
                    blastAttack.Fire();
                }
            }
        }
    }
}
