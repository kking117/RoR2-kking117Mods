using System;
using System.Collections.Generic;
using RoR2;
using RoR2.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.CharacterAI;
using RoR2.Projectile;
using FlatItemBuff.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
    public class DefenseNucleus_Shared
    {
        //"RoR2/DLC1/MajorAndMinorConstruct/MegaConstructBubbleShield.prefab"
        public static GameObject ShieldPrefab = Addressables.LoadAssetAsync<GameObject>("0e0129d9b272aff448748afd9b501e4c").WaitForCompletion();
        private static CharacterSpawnCard ConstructCSC;
        private static GameObject ConstructBodyObject;
        public static BodyIndex ConstructBodyIndex = BodyIndex.None;

        private const string LogName = "Defense Nucleus Shared";
        internal static bool ForceMechanical = false;
        internal static bool ExtraDisplays = false;
        internal static bool TweakAI = false;
        public static void EnableChanges()
        {
            if (TweakAI)
            {
                UpdateAI();
            }
            On.RoR2.Projectile.ProjectileSpawnMaster.SpawnMaster += Projectile_SpawnMaster;
            On.RoR2.Projectile.ProjectileSpawnMaster.OnProjectileImpact += OnProjImpact;
            MainPlugin.ModLogger.LogInfo("Applying IL");
            IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
        }
        private static void IL_OnCharacterDeath(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchLdloc(18),
                x => x.MatchLdsfld(typeof(DLC1Content.Items), "MinorConstructOnKill")
            ))
            {
                ilcursor.Index += 3;
                ilcursor.Emit(OpCodes.Ldc_I4_0);
                ilcursor.Emit(OpCodes.Mul);
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnCharacterDeath - Hook failed");
            }
        }
        public static void ExtraChanges()
        {
            //"RoR2/DLC1/MajorAndMinorConstruct/cscMinorConstructOnKill.asset"
            ConstructCSC = Addressables.LoadAssetAsync<CharacterSpawnCard>("99c6cbb2de90481409528b3d12294dc6").WaitForCompletion();
            if (ConstructCSC)
            {
                for (int i = 0; i< ConstructCSC.itemsToGrant.Length; i++)
                {
                    if(ConstructCSC.itemsToGrant[i].itemDef == RoR2Content.Items.BoostHp)
                    {
                        ConstructCSC.itemsToGrant[i].count = 0;
                    }
                    else if (ConstructCSC.itemsToGrant[i].itemDef == RoR2Content.Items.BoostDamage)
                    {
                        ConstructCSC.itemsToGrant[i].count = 0;
                    }
                }
            }
            //"RoR2/DLC1/MajorAndMinorConstruct/MinorConstructOnKillBody.prefab"
            ConstructBodyObject = Addressables.LoadAssetAsync<GameObject>("bf5b53aa9306bce4b910220a43b30062").WaitForCompletion();
            if(ConstructBodyObject)
            {
                CharacterBody body = ConstructBodyObject.GetComponent<CharacterBody>();
                if (body)
                {
                    ConstructBodyIndex = body.bodyIndex;
                    if (ForceMechanical)
                    {
                        body.bodyFlags |= CharacterBody.BodyFlags.Mechanical;
                    }
                }
                if (ExtraDisplays)
                {
                    ItemDisplayRuleSet itemdisplayruleSet = ConstructBodyObject.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;
                    //DisplayRuleGroup displayruleGroup = ConstructBodyObject.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.keyAssetRuleGroups[0].displayRuleGroup;
                    
                    if (itemdisplayruleSet)
                    {
                        //"RoR2/DLC1/DroneWeapons/DisplayDroneWeaponRobotArm.prefab"
                        //"RoR2/DLC1/DroneWeapons/DisplayDroneWeaponMinigun.prefab"
                        //"RoR2/DLC1/DroneWeapons/DisplayDroneWeaponRobotArm.prefab"
                        GameObject ArmPrefab = Addressables.LoadAssetAsync<GameObject>("6a50b73e668dfe14e9330be5485494be").WaitForCompletion();
                        GameObject MiniGunPrefab = Addressables.LoadAssetAsync<GameObject>("7083017a4d0e96a4dac36e9a88450989").WaitForCompletion();
                        GameObject LauncherPrefab = Addressables.LoadAssetAsync<GameObject>("6a50b73e668dfe14e9330be5485494be").WaitForCompletion();
                        ItemDisplayRuleSet.KeyAssetRuleGroup droneBoostDisplay = new ItemDisplayRuleSet.KeyAssetRuleGroup
                        {
                            keyAsset = DLC1Content.Items.DroneWeaponsBoost,
                            //"RoR2/DLC1/DroneWeapons/DroneWeaponsBoost.asset"
                            keyAssetAddress = new IDRSKeyAssetReference("280de78e6d8946f45a3361ef2e925efc"),
                            displayRuleGroup = new DisplayRuleGroup
                            {
                                rules = new ItemDisplayRule[]
                                {
                                    new ItemDisplayRule
                                    {
                                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                                        //followerPrefab = LauncherPrefab,
                                        followerPrefabAddress = new AssetReferenceGameObject("6a50b73e668dfe14e9330be5485494be"),
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
                            //"RoR2/DLC1/DroneWeapons/DroneWeaponsDisplay1.asset"
                            keyAssetAddress = new IDRSKeyAssetReference("648a1d9c63c34b044b60ce3b2e99a6b8"),
                            displayRuleGroup = new DisplayRuleGroup
                            {
                                rules = new ItemDisplayRule[]
                                {
                                    new ItemDisplayRule
                                    {
                                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                                        //followerPrefab = MiniGunPrefab,
                                        followerPrefabAddress = new AssetReferenceGameObject("7083017a4d0e96a4dac36e9a88450989"),
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
                            //"RoR2/DLC1/DroneWeapons/DroneWeaponsDisplay2.asset"
                            keyAssetAddress = new IDRSKeyAssetReference("1aaf36d4754a593458e921a3de3bd00b"),
                            displayRuleGroup = new DisplayRuleGroup
                            {
                                rules = new ItemDisplayRule[]
                                {
                                    new ItemDisplayRule
                                    {
                                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                                        //followerPrefab = ArmPrefab,
                                        followerPrefabAddress = new AssetReferenceGameObject("6a50b73e668dfe14e9330be5485494be"),
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
                        itemdisplayruleSet.GenerateRuntimeValuesAsync();
                    }
                }
            }
        }
        private static void UpdateAI()
        {
            //"RoR2/DLC1/MajorAndMinorConstruct/MinorConstructOnKillMaster.prefab"
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>("f55edee9fe6792a43817ec99fa2615a1").WaitForCompletion();
            BaseAI baseai = prefab.GetComponent<BaseAI>();
            if (baseai)
            {
                baseai.fullVision = true;
                baseai.neverRetaliateFriendlies = true;
            }
        }
        private static void OnProjImpact(On.RoR2.Projectile.ProjectileSpawnMaster.orig_OnProjectileImpact orig, ProjectileSpawnMaster self, ProjectileImpactInfo impactInfo)
        {
            if (self.deployableSlot == DeployableSlot.MinorConstructOnKill)
            {
                if (impactInfo.collider.gameObject.layer != LayerIndex.world.intVal)
                {
                    return;
                }
            }
            orig(self, impactInfo);
        }
        private static void Projectile_SpawnMaster(On.RoR2.Projectile.ProjectileSpawnMaster.orig_SpawnMaster orig, ProjectileSpawnMaster self)
        {
            if (self.deployableSlot == DeployableSlot.MinorConstructOnKill)
            {
                ProjectileController projController = self.GetComponent<ProjectileController>();
                if (projController && projController.owner)
                {
                    CharacterBody ownerbody = projController.owner.GetComponent<CharacterBody>();
                    if (ownerbody)
                    {
                        CharacterMaster ownerMaster = ownerbody.master;
                        if(ownerMaster)
                        {
                            int killCount = (ownerMaster.GetDeployableCount(DeployableSlot.MinorConstructOnKill) + 1) - ownerMaster.GetDeployableSameSlotLimit(DeployableSlot.MinorConstructOnKill);
                            if (killCount > 0)
                            {
                                Helpers.KillDeployables(ownerMaster, DeployableSlot.MinorConstructOnKill, killCount);
                            }
                        }
                    }
                }
            }
            orig(self);
        }
    }
}
