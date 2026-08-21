using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Railroad.Changes
{
    public enum PortalProgReqTags : int
    { //this is stupid, just use a list if this needs to be expanded
        None = 0,
        PreLoop = 1,
        PostLoop = 2,
        PreMithrix = 4,
        PostMithrix = 8,
        PreTwistedScavenger = 16,
        PostTwistedScavenger = 32,
        PreVoidling = 64,
        PostVoidling = 128,
        PreFalseSon = 256,
        PostFalseSon = 512,
        PreSolusWing = 1024,
        PostSolusWing = 2048,
        PreSolusHeart = 4096,
        PostSolusHeart = 8192,
        PreVoidFields = 16384,
        PostVoidFields = 32768
    }

    public class PortalSpawnData
    {
        internal ConfigPortalType PortalType = ConfigPortalType.NoPortal;
        internal PortalProgReqTags ReqTags = PortalProgReqTags.None;
    }
    public class PortalUtility
    {
        internal static string Portal_Default_Open = "";
        internal static string Portal_Shop_Open = "PORTAL_SHOP_OPEN";
        internal static string Portal_GoldCoast_Open = "PORTAL_GOLDSHORES_OPEN";
        internal static string Portal_MS_Open = "PORTAL_MS_OPEN";
        internal static string Portal_DeepVoid_Open = "PORTAL_DEEPVOID_OPEN";
        internal static string Portal_Storm_Open = "PORTAL_STORM_OPEN";
        internal static string Portal_HardwareProg_Open = "PORTAL_SOLUSSHOP_OPEN";
        internal static string Portal_HardwareProg_Haunt_Open = "PORTAL_SOLUSSHOP_OPEN";
        internal static string Portal_SolusShop_Open = "PORTAL_SOLUSSHOP_OPEN";

        //"RoR2/DLC1/OptionPickup/OptionPickup.prefab"
        static GameObject VoidPotential = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OptionPickup/OptionPickup.prefab").WaitForCompletion();

        //"RoR2/Base/Common/dtTier3Item.asset"
        static RoR2.BasicPickupDropTable Tier3PickupTable = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/Base/Common/dtTier3Item.asset").WaitForCompletion();

        //"RoR2/Base/PortalShop/iscShopPortal.asset"
        static InteractableSpawnCard Portal_Shop = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/PortalShop/iscShopPortal.asset").WaitForCompletion();
        //"RoR2/Base/PortalMS/iscMSPortal.asset"
        static InteractableSpawnCard Portal_MS = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/PortalMS/iscMSPortal.asset").WaitForCompletion();
        static InteractableSpawnCard Portal_Arena = null;
        //"RoR2/DLC1/PortalVoid/iscVoidPortal.asset"
        static InteractableSpawnCard Portal_Void = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/PortalVoid/iscVoidPortal.asset").WaitForCompletion();
        //"RoR2/DLC1/DeepVoidPortal/iscDeepVoidPortal.asset"
        static InteractableSpawnCard Portal_DeepVoid = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/DeepVoidPortal/iscDeepVoidPortal.asset").WaitForCompletion();
        //"RoR2/DLC1/VoidOutroPortal/iscVoidOutroPortal.asset"
        static InteractableSpawnCard Portal_VoidOutro = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidOutroPortal/iscVoidOutroPortal.asset").WaitForCompletion();
        //"RoR2/Base/PortalGoldshores/iscGoldshoresPortal.asset"
        static InteractableSpawnCard Portal_Goldshores = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/PortalGoldshores/iscGoldshoresPortal.asset").WaitForCompletion();
        //"RoR2/DLC2/iscColossusPortal.asset"
        static InteractableSpawnCard Portal_Colossus = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscColossusPortal.asset").WaitForCompletion();
        //"RoR2/DLC2/iscDestinationPortal.asset"
        static InteractableSpawnCard Portal_Destination = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscDestinationPortal.asset").WaitForCompletion();
        //"RoR2/DLC3/iscHardwareProgPortal.asset"
        static InteractableSpawnCard Portal_HardwareProg = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscHardwareProgPortal.asset").WaitForCompletion();
        //"RoR2/DLC3/iscHardwareProgPortal_Haunt.asset"
        static InteractableSpawnCard Portal_HardwareProg_Haunt = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscHardwareProgPortal_Haunt.asset").WaitForCompletion();
        //"RoR2/DLC3/iscSolusShopPortal.asset"
        static InteractableSpawnCard Portal_SolusShop = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscSolusShopPortal.asset").WaitForCompletion();
        //"RoR2/DLC3/iscSolusPortalBackout.asset"
        static InteractableSpawnCard Portal_SolusBackout = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC3/iscSolusPortalBackout.asset").WaitForCompletion();
        static InteractableSpawnCard Portal_SolusWeb = null;

        //"RoR2/Base/arena/arena.asset"
        static SceneDef Arena_SceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
        //"RoR2/DLC2/meridian/meridian.asset"
        static SceneDef Meridian_SceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/meridian/meridian.asset").WaitForCompletion();
        //"RoR2/DLC1/Common/DLC1.asset"
        static ExpansionDef DLC1Def = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();
        //"RoR2/DLC2/Common/DLC2.asset"
        static ExpansionDef DLC2Def = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC2/Common/DLC2.asset").WaitForCompletion();
        //"RoR2/DLC3/DLC3.asset"
        static ExpansionDef DLC3Def = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC3/DLC3.asset").WaitForCompletion();
        public PortalUtility()
        {
            CreateSpawnCards();
        }

        private void CreateSpawnCards()
        {
            Portal_Arena = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            //"RoR2/Base/PortalArena/PortalArena.prefab"
            Portal_Arena.prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/PortalArena/PortalArena.prefab").WaitForCompletion();
            Portal_Arena.name = Portal_Arena.prefab.name;
            Portal_Arena.sendOverNetwork = Portal_Destination.sendOverNetwork;
            Portal_Arena.hullSize = Portal_Destination.hullSize;
            Portal_Arena.nodeGraphType = Portal_Destination.nodeGraphType;
            Portal_Arena.requiredFlags = Portal_Destination.requiredFlags;
            Portal_Arena.forbiddenFlags = Portal_Destination.forbiddenFlags;
            Portal_Arena.directorCreditCost = Portal_Destination.directorCreditCost;
            Portal_Arena.occupyPosition = Portal_Destination.occupyPosition;
            Portal_Arena.orientToFloor = Portal_Destination.orientToFloor;
            Portal_Arena.skipSpawnWhenDevotionArtifactEnabled = Portal_Destination.skipSpawnWhenDevotionArtifactEnabled;
            Portal_Arena.skipSpawnWhenSacrificeArtifactEnabled = Portal_Destination.skipSpawnWhenSacrificeArtifactEnabled;

            Portal_SolusWeb = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            //"RoR2/DLC3/SolusWebPortal.prefab"
            Portal_SolusWeb.prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC3/SolusWebPortal.prefab").WaitForCompletion();
            Portal_SolusWeb.name = Portal_SolusWeb.prefab.name;
            Portal_SolusWeb.sendOverNetwork = Portal_Destination.sendOverNetwork;
            Portal_SolusWeb.hullSize = Portal_Destination.hullSize;
            Portal_SolusWeb.nodeGraphType = Portal_Destination.nodeGraphType;
            Portal_SolusWeb.requiredFlags = Portal_Destination.requiredFlags;
            Portal_SolusWeb.forbiddenFlags = Portal_Destination.forbiddenFlags;
            Portal_SolusWeb.directorCreditCost = Portal_Destination.directorCreditCost;
            Portal_SolusWeb.occupyPosition = Portal_Destination.occupyPosition;
            Portal_SolusWeb.orientToFloor = Portal_Destination.orientToFloor;
            Portal_SolusWeb.skipSpawnWhenDevotionArtifactEnabled = Portal_Destination.skipSpawnWhenDevotionArtifactEnabled;
            Portal_SolusWeb.skipSpawnWhenSacrificeArtifactEnabled = Portal_Destination.skipSpawnWhenSacrificeArtifactEnabled;
        }
        internal static List<PortalSpawnData> BuildPortalList(string portalList, string errorCode = "Not Specified")
        {
            List<PortalSpawnData> returnList = new List<PortalSpawnData>();
            if (portalList.Length > 0)
            {
                string[] items = portalList.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string portalType = items[i].Trim();
                    string[] portalTags = null;
                    if (portalType.Contains(";"))
                    {
                        portalTags = portalType.Split(';');
                        portalType = portalTags[0];
                    }
                    ConfigPortalType enumPortalType = ConfigPortalType.NoPortal;
                    if (!Enum.TryParse(portalType, out enumPortalType))
                    {
                        //check if the portal type exists as option
                        MainPlugin.ModLogger.LogWarning("[" + errorCode + "] Could not find Portal Type: [" + portalType + "]");
                    }
                    else
                    {
                        //then check if this portal type was already registerted
                        bool newPortalData = true;
                        for (int z = 0; z < returnList.Count; z++)
                        {
                            if (returnList[z].PortalType == enumPortalType)
                            {
                                newPortalData = false;
                                break;
                            }
                        }
                        if (newPortalData)
                        {
                            PortalSpawnData thisPortalData = new PortalSpawnData();
                            thisPortalData.PortalType = enumPortalType;

                            //Here is where we check and gather its portal tags
                            PortalProgReqTags thisPortalTags = PortalProgReqTags.None;
                            if (portalTags != null && portalTags.Length > 1)
                            {
                                for (int j = 1; j < portalTags.Length; j++)
                                {
                                    PortalProgReqTags enumPortalProgReqTag = PortalProgReqTags.None;
                                    if (!Enum.TryParse(portalTags[j], out enumPortalProgReqTag))
                                    {
                                        //check if the portal tag exists as option
                                        MainPlugin.ModLogger.LogWarning("[" + errorCode + ", " + portalType + "] Could not find Portal Tag: [" + portalTags[j] + "]");
                                    }
                                    else
                                    {
                                        if ((thisPortalTags & enumPortalProgReqTag) != enumPortalProgReqTag)
                                        {
                                            thisPortalTags |= enumPortalProgReqTag;
                                            //MainPlugin.ModLogger.LogWarning("Registered PortalTag: " + enumPortalProgReqTag);
                                        }
                                        else
                                        {
                                            MainPlugin.ModLogger.LogWarning("[" + errorCode + ", " + portalType + "] Duplicate Portal Tag in config: [" + enumPortalProgReqTag + "]");
                                        }
                                    }
                                }
                            }
                            thisPortalData.ReqTags = thisPortalTags;
                            returnList.Add(thisPortalData);
                            //MainPlugin.ModLogger.LogWarning("Registered PortalType: " + portalType);
                        }
                        else
                        {
                            MainPlugin.ModLogger.LogWarning("[" + errorCode + "] Duplicate Portal Type in config: [" + portalType + "]");
                        }
                    }
                }
                if (returnList.Count < 1)
                {
                    returnList = null;
                }
            }
            return returnList;
        }
        internal static List<ConfigPortalType> GetValidPortals_NEO(List<PortalSpawnData> PortalList)
        {
            //MainPlugin.ModLogger.LogWarning("RunFlags = " + RunFlags.SaveData_RunProgressFlags);
            List<ConfigPortalType> returnList = new List<ConfigPortalType>();
            for (int i = 0; i < PortalList.Count; i++)
            {
                if (AllowedToSpawnPortal(PortalList[i].PortalType, PortalList[i].ReqTags))
                {
                    returnList.Add(PortalList[i].PortalType);
                }
                else
                {
                    returnList.Add(ConfigPortalType.NoPortal);
                }
            }
            return returnList;
        }
        internal static List<ConfigPortalType> GetValidPortals(List<ConfigPortalType> PortalList)
        {
            List<ConfigPortalType> returnList = new List<ConfigPortalType>();
            for (int i = 0; i < PortalList.Count; i++)
            {
                if (CanSpawnPortal(PortalList[i]))
                {
                    returnList.Add(PortalList[i]);
                }
                else
                {
                    returnList.Add(ConfigPortalType.NoPortal);
                }
            }
            return returnList;
        }

        internal static bool PassesPortalTags(PortalProgReqTags portalReqs)
        {
            //This works for now, but surely there's a more concise way of doing this?
            //No this is not a performance concern, the function runs once a stage at most.
            //It just looks bad and reads worse.
            if ((portalReqs & PortalProgReqTags.PostSolusHeart) == PortalProgReqTags.PostSolusWing && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.SolusHeart) != RunProgressFlags.SolusHeart)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Solus Heart Defeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreSolusHeart) == PortalProgReqTags.PreSolusHeart && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.SolusHeart) == RunProgressFlags.SolusHeart)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Solus Heart Undefeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostSolusWing) == PortalProgReqTags.PostSolusWing && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.SolusWing) != RunProgressFlags.SolusWing)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Solus Wing Defeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreSolusWing) == PortalProgReqTags.PreSolusWing && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.SolusWing) == RunProgressFlags.SolusWing)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Solus Wing Undefeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostFalseSon) == PortalProgReqTags.PostFalseSon && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.FalseSon) != RunProgressFlags.FalseSon)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires False Son Defeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreFalseSon) == PortalProgReqTags.PreFalseSon && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.FalseSon) == RunProgressFlags.FalseSon)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires False Son Undefeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostVoidling) == PortalProgReqTags.PostFalseSon && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Voidling) != RunProgressFlags.Voidling)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Voidling Defeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreVoidling) == PortalProgReqTags.PreFalseSon && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Voidling) == RunProgressFlags.Voidling)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Voidling Undefeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostTwistedScavenger) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.TwistedScavenger) != RunProgressFlags.TwistedScavenger)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Twisted Scavenger Defeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreTwistedScavenger) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.TwistedScavenger) == RunProgressFlags.TwistedScavenger)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Twisted Scavenger Undefeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostMithrix) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Mithrix) != RunProgressFlags.Mithrix)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Mithrix Defeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreMithrix) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Mithrix) == RunProgressFlags.Mithrix)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Mithrix Undefeated to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostVoidFields) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.VoidFields) != RunProgressFlags.VoidFields)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Void Fields Cleared to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreVoidFields) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.VoidFields) == RunProgressFlags.VoidFields)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires Void Fields Uncleared to Spawn");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PostLoop) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Looping) != RunProgressFlags.Looping)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires run to be Looping.");
                return false;
            }
            if ((portalReqs & PortalProgReqTags.PreLoop) != 0 && (RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Looping) == RunProgressFlags.Looping)
            {
                //MainPlugin.ModLogger.LogWarning("Skipped Portal, requires run to be on first Loop.");
                return false;
            }
            return true;
        }
        internal static bool AllowedToSpawnPortal(ConfigPortalType portalType, PortalProgReqTags portalReqs)
        {
            if (PassesPortalTags(portalReqs) == false)
            {
                return false;
            }
            if (portalType == ConfigPortalType.NoPortal)
            {
                return false;
            }
            if (portalType == ConfigPortalType.Void || portalType == ConfigPortalType.DeepVoid || portalType == ConfigPortalType.VoidOutro)
            {
                return Run.instance.IsExpansionEnabled(DLC1Def);
            }
            if (portalType == ConfigPortalType.Colossus || portalType == ConfigPortalType.Destination)
            {
                return Run.instance.IsExpansionEnabled(DLC2Def);
            }
            if (portalType == ConfigPortalType.HardwareProg || portalType == ConfigPortalType.HardwareProg_Haunt || portalType == ConfigPortalType.SolusShop || portalType == ConfigPortalType.SolusBackout || portalType == ConfigPortalType.SolusWeb)
            {
                return Run.instance.IsExpansionEnabled(DLC3Def);
            }
            return true;
        }
        internal static bool CanSpawnPortal(ConfigPortalType portalType)
        {
            if (portalType == ConfigPortalType.NoPortal)
            {
                return false;
            }
            if (portalType == ConfigPortalType.Void || portalType == ConfigPortalType.DeepVoid || portalType == ConfigPortalType.VoidOutro)
            {
                return Run.instance.IsExpansionEnabled(DLC1Def);
            }
            if (portalType == ConfigPortalType.Colossus || portalType == ConfigPortalType.Destination)
            {
                return Run.instance.IsExpansionEnabled(DLC2Def);
            }
            if (portalType == ConfigPortalType.HardwareProg || portalType == ConfigPortalType.HardwareProg_Haunt || portalType == ConfigPortalType.SolusShop || portalType == ConfigPortalType.SolusBackout || portalType == ConfigPortalType.SolusWeb)
            {
                return Run.instance.IsExpansionEnabled(DLC3Def);
            }
            return true;
        }
        internal static InteractableSpawnCard GetSpawnCardFromIndex(ConfigPortalType portalType)
        {
            switch (portalType)
            {
                case ConfigPortalType.Shop:
                    return Portal_Shop;
                case ConfigPortalType.MS:
                    return Portal_MS;
                case ConfigPortalType.Null:
                    return Portal_Arena;
                case ConfigPortalType.Void:
                    return Portal_Void;
                case ConfigPortalType.DeepVoid:
                    return Portal_DeepVoid;
                case ConfigPortalType.VoidOutro:
                    return Portal_VoidOutro;
                case ConfigPortalType.Goldshores:
                    return Portal_Goldshores;
                case ConfigPortalType.Colossus:
                    return Portal_Colossus;
                case ConfigPortalType.Destination:
                    return Portal_Destination;
                case ConfigPortalType.HardwareProg:
                    return Portal_HardwareProg;
                case ConfigPortalType.HardwareProg_Haunt:
                    return Portal_HardwareProg_Haunt;
                case ConfigPortalType.SolusShop:
                    return Portal_SolusShop;
                case ConfigPortalType.SolusBackout:
                    return Portal_SolusBackout;
                case ConfigPortalType.SolusWeb:
                    return Portal_SolusWeb;
            }
            return null;
        }

        internal static string GetSpawnMessageFromIndex(ConfigPortalType portalType)
        {
            switch (portalType)
            {
                case ConfigPortalType.Shop:
                    return Portal_Shop_Open;
                case ConfigPortalType.MS:
                    return Portal_MS_Open;
                case ConfigPortalType.DeepVoid:
                    return Portal_DeepVoid_Open;
                case ConfigPortalType.Goldshores:
                    return Portal_GoldCoast_Open;
                case ConfigPortalType.Colossus:
                    return Portal_Storm_Open;
                case ConfigPortalType.HardwareProg:
                    return Portal_HardwareProg_Open;
                case ConfigPortalType.HardwareProg_Haunt:
                    return Portal_HardwareProg_Haunt_Open;
                case ConfigPortalType.SolusShop:
                    return Portal_SolusShop_Open;
            }
            return Portal_Default_Open;
        }

        internal static GameObject TrySpawnPortal_FindFloor(ConfigPortalType portalType, Vector3 location, DirectorPlacementRule.PlacementMode placementMode = DirectorPlacementRule.PlacementMode.Approximate)
        {
            if (CanSpawnPortal(portalType))
            {
                InteractableSpawnCard spawnCard = GetSpawnCardFromIndex(portalType);
                if (spawnCard)
                {
                    RaycastHit raycastHit;
                    Physics.Raycast(location, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
                    {
                    "World"
                    }));
                    if (raycastHit.point.y + 1f <= location.y)
                    {
                        location.y = raycastHit.point.y + 1f;
                    }
                    else
                    {
                        location.y = raycastHit.point.y;
                    }

                    GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                    {
                        maxDistance = 30f,
                        minDistance = 10f,
                        placementMode = placementMode,
                        position = location
                    }, Run.instance.stageRng));
                    return gameObject;
                }
            }
            return null;
        }

        internal static void TrySpawnPortalCircle(List<ConfigPortalType> portalList, Vector3 baselocation, DirectorPlacementRule.PlacementMode placementMode = DirectorPlacementRule.PlacementMode.Approximate, float baseDistance = 20f, bool groundPortals = false, bool randomAngle = false)
        {
            int portalCount = portalList.Count;
            if (portalCount == 1)
            {
                if (groundPortals)
                {
                    TrySpawnPortal_FindFloor(portalList[0], baselocation, placementMode);
                }
                else
                {
                    TrySpawnPortal(portalList[0], baselocation, placementMode);
                }
            }
            else if (portalCount > 0)
            {
                //MainPlugin.ModLogger.LogInfo("Spawning total portals: " + Limbo_Portals.Count);
                float horiAngle = 360f / portalCount;

                float baseAngle = 90.0f;
                if (randomAngle)
                {
                    baseAngle = UnityEngine.Random.Range(0, 360);
                }
                Vector3 vector = Quaternion.AngleAxis(baseAngle, Vector3.up) * (Vector3.forward);
                Quaternion quaternion = Quaternion.AngleAxis(horiAngle, Vector3.up);
                int i = 0;
                while (i < portalCount)
                {
                    if (CanSpawnPortal(portalList[i]))
                    {
                        InteractableSpawnCard spawnCard = GetSpawnCardFromIndex(portalList[i]);
                        if (spawnCard)
                        {
                            Vector3 placeLoc = baselocation + (vector * baseDistance);
                            //MainPlugin.ModLogger.LogInfo("Portal Loc: " + placeLoc.x + " " + placeLoc.y + " " + placeLoc.z);
                            if (groundPortals)
                            {
                                RaycastHit raycastHit;
                                Physics.Raycast(placeLoc, Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
                                {
                                    "World"
                                }));
                                if (raycastHit.point.y + 1f <= placeLoc.y)
                                {
                                    placeLoc.y = raycastHit.point.y + 1f;
                                }
                                else
                                {
                                    placeLoc.y = raycastHit.point.y;
                                }
                            }
                            GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                            {
                                maxDistance = 30f,
                                minDistance = 10f,
                                placementMode = placementMode,
                                position = placeLoc,
                                rotation = Quaternion.AngleAxis(0f, vector)
                            }, Run.instance.stageRng));
                        }
                    }
                    i++;
                    vector = quaternion * vector;
                }
            }
        }
        internal static GameObject TrySpawnPortal(ConfigPortalType portalType, Vector3 location, DirectorPlacementRule.PlacementMode placementMode = DirectorPlacementRule.PlacementMode.Approximate)
        {
            if (CanSpawnPortal(portalType))
            {
                InteractableSpawnCard spawnCard = GetSpawnCardFromIndex(portalType);
                if (spawnCard)
                {
                    GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                    {
                        maxDistance = 30f,
                        minDistance = 10f,
                        placementMode = placementMode,
                        position = location
                    }, Run.instance.stageRng));
                    return gameObject;
                }
            }
            return null;
        }
        internal static void TryDropPotential(PickupDropTable dropTable, Vector3 location, int optionCount)
        {
            if (dropTable == null)
            {
                return;
            }
            int playerCount = Run.instance.participatingPlayerCount;
            if (playerCount > 0)
            {
                List<UniquePickup> list = new List<UniquePickup>();
                dropTable.GenerateDistinctPickups(list, optionCount, Run.instance.treasureRng, true);

                if (list.Count > 0)
                {
                    ItemTier itemTier = PickupCatalog.GetPickupDef(list[0].pickupIndex).itemTier;

                    float horiAngle = 360f / playerCount;
                    Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                    Quaternion quaternion = Quaternion.AngleAxis(horiAngle, Vector3.up);
                    int i = 0;
                    while (i < playerCount)
                    {
                        GenericPickupController.CreatePickupInfo pickupInfo = new GenericPickupController.CreatePickupInfo
                        {
                            pickerOptions = PickupPickerController.GenerateOptionsFromList<List<UniquePickup>>(list),
                            prefabOverride = VoidPotential,
                            position = location,
                            rotation = Quaternion.identity,
                            pickup = new UniquePickup(PickupCatalog.FindPickupIndex(itemTier))
                        };

                        PickupDropletController.CreatePickupDroplet(pickupInfo, location, vector);
                        i++;
                        vector = quaternion * vector;
                    }
                }
            }
        }
        internal static void TryDropItems(List<PickupIndex> dropList, Vector3 location)
        {
            int playerCount = Run.instance.participatingPlayerCount;
            if (playerCount > 0)
            {
                int maxItems = dropList.Count;
                if (maxItems > 0)
                {
                    //PickupIndex pickupIndex = null;
                    UniquePickup pickupNew = new UniquePickup();
                    pickupNew.pickupIndex = dropList[UnityEngine.Random.Range(0, maxItems - 1)];

                    float horiAngle = 360f / playerCount;
                    Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                    Quaternion quaternion = Quaternion.AngleAxis(horiAngle, Vector3.up);
                    int i = 0;
                    while (i < playerCount)
                    {
                        PickupDropletController.CreatePickupDroplet(pickupNew, location, vector, false);
                        i++;
                        vector = quaternion * vector;
                    }
                }
            }
        }
    }
}