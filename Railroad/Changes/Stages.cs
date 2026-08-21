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
    public class Stages
    {
        internal static bool Enable = true;

        internal static string ModeStandard_Moon2_Portal_Input = "";
        internal static string ModeEclipse_Moon2_Portal_Input = "";
        internal static List<PortalSpawnData> ModeStandard_Moon2_Portals;
        internal static List<PortalSpawnData> ModeEclipse_Moon2_Portals;
        internal static bool ModeStandard_Moon2_Reward = false;
        internal static bool ModeEclipse_Moon2_Reward = false;

        internal static string ModeStandard_Meridian_Portal_Input = "";
        internal static string ModeEclipse_Meridian_Portal_Input = "";
        internal static List<PortalSpawnData> ModeStandard_Meridian_Portals;
        internal static List<PortalSpawnData> ModeEclipse_Meridian_Portals;
        internal static bool ModeStandard_Meridian_Modify_Portal = false;
        internal static bool ModeEclipse_Meridian_Modify_Portal = false;
        internal static bool ModeStandard_Meridian_Reward = true;
        internal static bool ModeEclipse_Meridian_Reward = true;
        internal static bool ModeStandard_Meridian_AllowRebirth = true;
        internal static bool ModeEclipse_Meridian_AllowRebirth = false;
        internal static bool ModeStandard_Meridian_ACPortal = true;
        internal static bool ModeEclipse_Meridian_ACPortal = true;

        internal static string ModeStandard_VoidRaid_Portal_Input = "";
        internal static string ModeEclipse_VoidRaid_Portal_Input = "";
        internal static List<PortalSpawnData> ModeStandard_VoidRaid_Portals;
        internal static List<PortalSpawnData> ModeEclipse_VoidRaid_Portals;
        internal static bool ModeStandard_VoidRaid_Reward = false;
        internal static bool ModeEclipse_VoidRaid_Reward = false;
        internal static bool ModeStandard_VoidRaid_VoidOutroPortal = true;
        internal static bool ModeEclipse_VoidRaid_VoidOutroPortal = true;

        internal static bool ModeStandard_MS_NeedBeads = true;
        internal static bool ModeEclipse_MS_NeedBeads = true;
        internal static string ModeStandard_MS_OrbReq_Input = "";
        internal static string ModeEclipse_MS_OrbReq_Input = "";
        internal static List<ReqAllowData> ModeStandard_MS_OrbReq_Data = null;
        internal static List<ReqAllowData> ModeEclipse_MS_OrbReq_Data = null;

        internal static string ModeStandard_Limbo_Portal_Input = "";
        internal static string ModeEclipse_Limbo_Portal_Input = "";
        internal static List<PortalSpawnData> ModeStandard_Limbo_Portals;
        internal static List<PortalSpawnData> ModeEclipse_Limbo_Portals;
        internal static bool ModeStandard_Limbo_Reward = false;
        internal static bool ModeEclipse_Limbo_Reward = false;
        internal static float Limbo_ExtraTime = 8f;

        internal static string ModeStandard_SolusWeb_Portal_Input = "";
        internal static string ModeEclipse_SolusWeb_Portal_Input = "";
        internal static List<PortalSpawnData> ModeStandard_SolusWeb_Portals;
        internal static List<PortalSpawnData> ModeEclipse_SolusWeb_Portals;
        internal static bool ModeStandard_SolusWeb_Reward = true;
        internal static bool ModeEclipse_SolusWeb_Reward = true;
        internal static bool ModeStandard_SolusWeb_AllowDecompile = true;
        internal static bool ModeEclipse_SolusWeb_AllowDecompile = false;

        internal static ConfigPortalType Bazaar_ArenaRepeat_Portal = ConfigPortalType.Void;

        internal static bool ModeStandard_Arena_VoidPortal = true;
        internal static bool ModeEclipse_Arena_VoidPortal = false;
        //internal static bool Arena_TimeFlows = true;

        internal static ConfigGoldPortal ModeStandard_GoldShores_MeridianPortal = ConfigGoldPortal.Vanilla;
        internal static ConfigGoldPortal ModeEclipse_GoldShores_MeridianPortal = ConfigGoldPortal.Vanilla;

        //internal static bool MeridianPath_Allow_Seer = true;
        //internal static bool MeridianPath_Allow_Shrine = true;

        //internal static bool VoidPath_Allow_Seer = true;
        //internal static bool VoidPath_Allow_Portal = true;
        private static Vector3 Bazaar_Pos = new Vector3(280.83f, -445.67f, -126.48f);
        private static Vector3 Moon2_Pos = new Vector3(-89f, 493.0f, 1.15f);
        private static Vector3 Limbo_Pos = new Vector3(-6.7f, -8.6f, 87f);
        private static Vector3 Meridian_Pos = new Vector3(109.21f, 146.5f, -121.78f);

        //"RoR2/DLC1/OptionPickup/OptionPickup.prefab"
        private GameObject VoidPotential = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OptionPickup/OptionPickup.prefab").WaitForCompletion();

        //"RoR2/Base/Common/dtTier3Item.asset"
        private RoR2.BasicPickupDropTable Tier3PickupTable = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/Base/Common/dtTier3Item.asset").WaitForCompletion();

        //"RoR2/Base/arena/arena.asset"
        private SceneDef Scene_Arena = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
        //"RoR2/DLC1/voidraid/voidraid.asset"
        private SceneDef Scene_VoidRaid = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidraid/voidraid.asset").WaitForCompletion();

        //"RoR2/Base/arena/arena.asset"
        private SceneDef Arena_SceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
        //"RoR2/DLC2/meridian/meridian.asset"
        private SceneDef Meridian_SceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/meridian/meridian.asset").WaitForCompletion();
        public Stages()
        {
            if (!Enable)
            {
                return;
            }
            ClampConfig();
            BuildLists();
            Hooks();
        }
        private void BuildLists()
        {
            if (ModeStandard_MS_OrbReq_Input.Length > 0)
            {
                ModeStandard_MS_OrbReq_Data = ReqList.ReadStageNumberInput(ModeStandard_MS_OrbReq_Input, "Stages|Standard|Celestial Orb");
            }
            if (ModeEclipse_MS_OrbReq_Input.Length > 0)
            {
                ModeEclipse_MS_OrbReq_Data = ReqList.ReadStageNumberInput(ModeEclipse_MS_OrbReq_Input, "Stages|Eclipse|Celestial Orb");
            }

            ModeEclipse_Moon2_Portals = PortalUtility.BuildPortalList(ModeEclipse_Moon2_Portal_Input, "Eclipse, Commencement");
            ModeStandard_Moon2_Portals = PortalUtility.BuildPortalList(ModeStandard_Moon2_Portal_Input, "Standard, Commencement");

            ModeEclipse_Limbo_Portals = PortalUtility.BuildPortalList(ModeEclipse_Limbo_Portal_Input, "Eclipse, A Moment Whole");
            ModeStandard_Limbo_Portals = PortalUtility.BuildPortalList(ModeStandard_Limbo_Portal_Input, "Standard, A Moment Whole");

            ModeEclipse_VoidRaid_Portals = PortalUtility.BuildPortalList(ModeEclipse_VoidRaid_Portal_Input, "Eclipse, Planetarium");
            ModeStandard_VoidRaid_Portals = PortalUtility.BuildPortalList(ModeStandard_VoidRaid_Portal_Input, "Standard, Planetarium");
            
            //Overhaul how socket Portals are handled please
            ModeEclipse_Meridian_Portals = PortalUtility.BuildPortalList(ModeEclipse_Meridian_Portal_Input, "Eclipse, Prime Meridian");
            ModeStandard_Meridian_Portals = PortalUtility.BuildPortalList(ModeStandard_Meridian_Portal_Input, "Standard, Prime Meridian");
            ModeStandard_Meridian_Modify_Portal = true;
            if (ModeStandard_Meridian_Portals != null && ModeStandard_Meridian_Portals.Count == 1)
            {
                if (ModeStandard_Meridian_Portals[0].PortalType == ConfigPortalType.Destination)
                {
                    ModeStandard_Meridian_Modify_Portal = false;
                }
            }
            ModeEclipse_Meridian_Modify_Portal = true;
            if (ModeEclipse_Meridian_Portals != null && ModeEclipse_Meridian_Portals.Count == 1)
            {
                if (ModeEclipse_Meridian_Portals[0].PortalType == ConfigPortalType.Destination)
                {
                    ModeEclipse_Meridian_Modify_Portal = false;
                }
            }

            ModeEclipse_SolusWeb_Portals = PortalUtility.BuildPortalList(ModeEclipse_SolusWeb_Portal_Input, "Eclipse, Solus Web");
            ModeStandard_SolusWeb_Portals = PortalUtility.BuildPortalList(ModeStandard_SolusWeb_Portal_Input, "Standard, Solus Web");
        }
        private void ClampConfig()
        {
            ModeStandard_GoldShores_MeridianPortal = (ConfigGoldPortal)Math.Min((int)ModeStandard_GoldShores_MeridianPortal, (int)ConfigGoldPortal.Meridian);
            ModeEclipse_GoldShores_MeridianPortal = (ConfigGoldPortal)Math.Min((int)ModeEclipse_GoldShores_MeridianPortal, (int)ConfigGoldPortal.Meridian);
        }
        
        private void Hooks()
        {
            SharedHooks.Handle_Mithrix_Clear_Actions += Mithrix_Clear;
            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += VoidRaidOnPortal;
            if (ModeStandard_VoidRaid_VoidOutroPortal == false || ModeEclipse_VoidRaid_VoidOutroPortal == false)
            {
                IL.RoR2.VoidRaidGauntletController.SpawnOutroPortal += new ILContext.Manipulator(IL_VoidRaid_SpawnOutroPortal);
            }
            On.RoR2.ArenaMissionController.GeneratePlayerSpawnPointsServer += ArenaMissionController_GeneratePlayerSpawnPointsServer;
            On.RoR2.ShrineRebirthController.Start += ShrineRebirthController_Start;
            if (!ModeStandard_Meridian_Reward || !ModeEclipse_Meridian_Reward)
            {
                IL.EntityStates.FalseSonBoss.SkyJumpDeathState.GiveColossusItem += new ILContext.Manipulator(IL_FalseSonBossComplete);
            }
            IL.EntityStates.ShrineRebirth.RebirthOrPortalChoice.OnEnter += new ILContext.Manipulator(IL_RebirthOrPortalChoice);
            IL.EntityStates.ShrineRebirth.RevealRebirthShriine.OnEnter += new ILContext.Manipulator(IL_RevealRebirthShrine);
            if (!ModeStandard_MS_NeedBeads || !ModeEclipse_MS_NeedBeads)
            {
                IL.EntityStates.Interactables.MSObelisk.EndingGame.DoFinalAction += new ILContext.Manipulator(IL_OnObliteration);
            }
            On.EntityStates.Missions.Goldshores.Exit.IsValidStormTier += IsValidStormTier;
            On.EntityStates.Missions.Goldshores.Exit.OnEnter += Goldshores_Exit;
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += LimboComplete;
            On.RoR2.SolusWebMissionController.SpawnExitPortal += SolusWeb_SpawnExitPortals;
            if (!ModeStandard_SolusWeb_AllowDecompile)
            {
                On.RoR2.Run.Start += RunStart;
            }
            SharedHooks.Handle_EclipseRun_Start_Actions += EclipseRun_Start;
            /*if (Bazaar_ArenaRepeat_Portal != ConfigPortalType.NoPortal)
            {
                On.RoR2.OnPlayerEnterEvent.OnTriggerEnter += OnPlayerEnterEvent;
            }*/
            IL.EntityStates.SolusHeart.Death.SolusHeartFinaleSequence.Death.OnEnter += new ILContext.Manipulator(IL_SolusHeartDeath);
            IL.RoR2.TeleporterInteraction.Start += new ILContext.Manipulator(IL_TeleInteractionStart);

        }
        private void RunStart(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            self.SetEventFlag("NoSolusHeartOffer");
        }
        private void EclipseRun_Start(EclipseRun self)
        {
            if (NetworkServer.active)
            {
                if (ModeEclipse_SolusWeb_AllowDecompile)
                {
                    self.ResetEventFlag("NoSolusHeartOffer");
                }
            }
        }

        /*private void OnPlayerEnterEvent(On.RoR2.OnPlayerEnterEvent.orig_OnTriggerEnter orig, OnPlayerEnterEvent self, Collider other)
        {
            orig(self, other);
            if (self.gameObject)
            {
                MainPlugin.ModLogger.LogInfo("Trigger Name = " + self.gameObject.name);
                MainPlugin.ModLogger.LogInfo("ServerOnly = " + self.serverOnly);
                GameObject parentObject = self.gameObject.GetComponentInParent<GameObject>();
                if (parentObject)
                {
                    MainPlugin.ModLogger.LogInfo("Parent Object = " + parentObject.name);
                }
            }
        }*/
        private void SolusWeb_SpawnExitPortals(On.RoR2.SolusWebMissionController.orig_SpawnExitPortal orig, RoR2.SolusWebMissionController self)
        {
            if (IsEclipse())
            {
                self.PortalPrefab = null;
                self.VoidPrefab = null;
                if (ModeEclipse_SolusWeb_Portals != null)
                {
                    Vector3 position = self.EscapePortalPos.transform.position;
                    if (self.offeringMaster)
                    {
                        CharacterBody offerBody = self.offeringMaster.GetBody();
                        if (offerBody)
                        {
                            position = offerBody.footPosition;
                        }
                    }
                    position.y += 1f;
                    List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeEclipse_SolusWeb_Portals);
                    PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct);
                }
            }
            else
            {
                self.PortalPrefab = null;
                self.VoidPrefab = null;
                if (ModeStandard_SolusWeb_Portals != null)
                {
                    Vector3 position = self.EscapePortalPos.transform.position;
                    if (self.offeringMaster)
                    {
                        CharacterBody offerBody = self.offeringMaster.GetBody();
                        if (offerBody)
                        {
                            position = offerBody.footPosition;
                        }
                    }
                    position.y += 1f;
                    List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeStandard_SolusWeb_Portals);
                    PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct);
                }
            }
            orig(self);
        }
        private void ArenaMissionController_GeneratePlayerSpawnPointsServer(On.RoR2.ArenaMissionController.orig_GeneratePlayerSpawnPointsServer orig, ArenaMissionController self)
        {
            orig(self);
            //self.totalRoundsMax = 1;
            if (self.completionPortalSpawners != null)
            {
                //MainPlugin.ModLogger.LogInfo("WillSpawn = " + self.completionPortalSpawners[0].willSpawn);
                if (IsEclipse())
                {
                    if (ModeEclipse_Arena_VoidPortal)
                    {
                        self.completionPortalSpawners[0].willSpawn = true;
                    }
                    else
                    {
                        self.completionPortalSpawners = new PortalSpawner[0];
                    }
                }
                else if (ModeStandard_Arena_VoidPortal == false)
                {
                    self.completionPortalSpawners = new PortalSpawner[0];
                }
                //some reason setting "willspawn = false" does not work
                //Finding the entry with the void portal and reappending the array without it would be the correct choice, but I'm lazy.
            }
            else
            {
                MainPlugin.ModLogger.LogInfo("Arena Portal List is Empty or Null");
            }
        }
        private bool IsValidStormTier(On.EntityStates.Missions.Goldshores.Exit.orig_IsValidStormTier orig, EntityStates.Missions.Goldshores.Exit self)
        {
            if (IsEclipse())
            {
                if (ModeEclipse_GoldShores_MeridianPortal == ConfigGoldPortal.Never)
                {
                    return false;
                }
            }
            else
            {
                if (ModeStandard_GoldShores_MeridianPortal == ConfigGoldPortal.Never)
                {
                    return false;
                }
            }
            return orig(self);
        }
        private void Goldshores_Exit(On.EntityStates.Missions.Goldshores.Exit.orig_OnEnter orig, EntityStates.Missions.Goldshores.Exit self)
        {
            orig(self);
            if (IsEclipse())
            {
                if (ModeEclipse_GoldShores_MeridianPortal == ConfigGoldPortal.Meridian)
                {
                    GameObject gameObject = PortalUtility.TrySpawnPortal(ConfigPortalType.Colossus, self.transform.position);
                    if (gameObject)
                    {
                        SceneExitController comp = gameObject.GetComponent<SceneExitController>();
                        if (comp)
                        {
                            comp.destinationScene = Meridian_SceneDef;
                            comp.tier1AlternateDestinationScene = null;
                            comp.tier2AlternateDestinationScene = null;
                            comp.tier3AlternateDestinationScene = null;
                            comp.tier4AlternateDestinationScene = null;
                            comp.tier5AlternateDestinationScene = null;
                        }
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "PORTAL_STORM_OPEN"
                        });
                    }
                }
            }
            else if (ModeStandard_GoldShores_MeridianPortal == ConfigGoldPortal.Meridian)
            {
                GameObject gameObject = PortalUtility.TrySpawnPortal(ConfigPortalType.Colossus, self.transform.position);
                if (gameObject)
                {
                    SceneExitController comp = gameObject.GetComponent<SceneExitController>();
                    if (comp)
                    {
                        comp.destinationScene = Meridian_SceneDef;
                        comp.tier1AlternateDestinationScene = null;
                        comp.tier2AlternateDestinationScene = null;
                        comp.tier3AlternateDestinationScene = null;
                        comp.tier4AlternateDestinationScene = null;
                        comp.tier5AlternateDestinationScene = null;
                    }
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "PORTAL_STORM_OPEN"
                    });
                }
            }
        }
        private void ShrineRebirthController_Start(On.RoR2.ShrineRebirthController.orig_Start orig, ShrineRebirthController self)
        {
            orig(self);
            if (IsEclipse())
            {
                if (ModeEclipse_Meridian_Portals.Count == 1)
                {
                    if (PortalUtility.CanSpawnPortal(ModeEclipse_Meridian_Portals[0].PortalType))
                    {
                        self.helminthPortalISC = PortalUtility.GetSpawnCardFromIndex(ModeEclipse_Meridian_Portals[0].PortalType);
                    }
                    else
                    {
                        self.helminthPortalISC = null;
                    }
                }
                else
                {
                    self.helminthPortalISC = null;
                }
            }
            else
            {
                if (ModeStandard_Meridian_Portals.Count == 1)
                {
                    if (PortalUtility.CanSpawnPortal(ModeStandard_Meridian_Portals[0].PortalType))
                    {
                        self.helminthPortalISC = PortalUtility.GetSpawnCardFromIndex(ModeStandard_Meridian_Portals[0].PortalType);
                    }
                    else
                    {
                        self.helminthPortalISC = null;
                    }
                }
                else
                {
                    self.helminthPortalISC = null;
                }
            }
        }
        private void LimboComplete(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
        {
            orig(self);
            self.startTime -= Limbo_ExtraTime;
            if (IsEclipse())
            {
                Vector3 position = Limbo_Pos;
                if (ModeEclipse_Limbo_Portals != null)
                {
                    List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeEclipse_Limbo_Portals);
                    PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Approximate);
                }
                if (ModeEclipse_Limbo_Reward)
                {
                    position.y += 1f;
                    PortalUtility.TryDropItems(Run.instance.availableTier3DropList, position);
                }
            }
            else
            {
                Vector3 position = Limbo_Pos;
                if (ModeStandard_Limbo_Portals != null)
                {
                    List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeStandard_Limbo_Portals);
                    PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Approximate);
                }
                if (ModeStandard_Limbo_Reward)
                {
                    position.y += 1f;
                    PortalUtility.TryDropItems(Run.instance.availableTier3DropList, position);
                }
            }
        }
        private void Mithrix_Clear(EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            Vector3 position = Moon2_Pos;
            if (IsEclipse())
            {
                if (ModeEclipse_Moon2_Portals != null)
                {
                    List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeEclipse_Moon2_Portals);
                    PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct);
                }
                if (ModeEclipse_Moon2_Reward)
                {
                    PortalUtility.TryDropItems(Run.instance.availableTier3DropList, Moon2_Pos);
                }
            }
            else
            {
                
                if (ModeStandard_Moon2_Portals != null)
                {
                    List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeStandard_Moon2_Portals);
                    PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct);
                }
                if (ModeStandard_Moon2_Reward)
                {
                    PortalUtility.TryDropItems(Run.instance.availableTier3DropList, Moon2_Pos);
                }
            }
        }
        private void VoidRaidOnPortal(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, VoidRaidGauntletController self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if (IsEclipse())
                {
                    bool needOutroPortal = true;
                    Vector3 position = self.currentDonut.returnPoint.transform.position;
                    if (ModeEclipse_VoidRaid_Portals != null)
                    {
                        /*if (ModeEclipse_VoidRaid_Portals.Contains(ConfigPortalType.VoidOutro))
                        {
                            needOutroPortal = false;
                        }*/
                        List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeEclipse_VoidRaid_Portals);
                        PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct, 20f, true);
                    }
                    if (ModeEclipse_VoidRaid_Reward)
                    {
                        position.y += 1f;
                        PortalUtility.TryDropPotential(Tier3PickupTable, position, 3);
                    }
                    if (needOutroPortal && ModeEclipse_VoidRaid_VoidOutroPortal == false)
                    {
                        //Spawn the outro portal somewhere in Narnia to finish the boss music.
                        PortalUtility.TrySpawnPortal(ConfigPortalType.VoidOutro, new Vector3(99999f, 99999f, 99999f), DirectorPlacementRule.PlacementMode.Direct);
                    }
                }
                else
                {
                    bool needOutroPortal = true;
                    Vector3 position = self.currentDonut.returnPoint.transform.position;
                    if (ModeStandard_VoidRaid_Portals != null)
                    {
                        /*if (ModeStandard_VoidRaid_Portals.Contains(ConfigPortalType.VoidOutro))
                        {
                            needOutroPortal = false;
                        }*/
                        List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeStandard_VoidRaid_Portals);
                        PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct, 20f, true);
                    }
                    if (ModeStandard_VoidRaid_Reward)
                    {
                        position.y += 1f;
                        PortalUtility.TryDropPotential(Tier3PickupTable, position, 3);
                    }
                    if (needOutroPortal && ModeStandard_VoidRaid_VoidOutroPortal == false)
                    {
                        //Spawn the outro portal somewhere in Narnia to finish the boss music.
                        PortalUtility.TrySpawnPortal(ConfigPortalType.VoidOutro, new Vector3(99999f, 99999f, 99999f), DirectorPlacementRule.PlacementMode.Direct);
                    }
                }
            }
        }
        private bool IsEclipse()
        {
            Run runInstance = Run.instance;
            if (runInstance)
            {
                return runInstance.selectedDifficulty >= DifficultyIndex.Eclipse1;
            }
            return false;
        }
        
        private void IL_OnObliteration(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchLdcI4(0),
                x => x.MatchStloc(0)
            ))
            {
                ilcursor.Remove();
                ilcursor.EmitDelegate<Func<bool>>(() =>
                {
                    if (IsEclipse() && !ModeEclipse_MS_NeedBeads)
                    {
                        return true;
                    }
                    if (!IsEclipse() && !ModeStandard_MS_NeedBeads)
                    {
                        return true;
                    }
                    return false;
                });
                //ilcursor.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": OnObliteration IL Hook failed");
            }
        }
        private void IL_VoidRaid_SpawnOutroPortal(ILContext il)
        {
            //Prevents the outro portal from spawning, we'll place it manually.
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchLdfld(typeof(VoidRaidGauntletController), "currentDonut")
            ))
            {
                ilcursor.Remove();
                ilcursor.EmitDelegate<Func<VoidRaidGauntletController, bool>>((self) =>
                {
                    return false;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": VoidRaid_SpawnOutroPortal IL Hook failed");
            }
        }
        private void IL_FalseSonBossComplete(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchCallOrCallvirt(typeof(RoR2.Run), "get_participatingPlayerCount")
            ))
            {
                ilcursor.Index += 1;
                ilcursor.EmitDelegate<Func<int>>(() =>
                {
                    if (IsEclipse() && ModeEclipse_Meridian_Reward)
                    {
                        return 1;
                    }
                    if (!IsEclipse() && ModeStandard_Meridian_Reward)
                    {
                        return 1;
                    }
                    return 0;
                });
                ilcursor.Emit(OpCodes.Mul);
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": GiveColossusItem IL Hook failed");
            }
        }
        private void IL_RevealRebirthShrine(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchCallOrCallvirt(typeof(RoR2.Run), "get_selectedDifficulty")
            ))
            {
                ilcursor.Index -=1 ;
                ilcursor.RemoveRange(2);
                ilcursor.EmitDelegate<Func<DifficultyIndex>>(() =>
                {
                    if (IsEclipse())
                    {
                        if (ModeEclipse_Meridian_Modify_Portal && ModeEclipse_Meridian_Portals != null && ModeEclipse_Meridian_Portals.Count > 1)
                        {
                            Vector3 position = Meridian_Pos;
                            List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeEclipse_Meridian_Portals);
                            PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct, 30f, false);
                        }
                        if (!ModeEclipse_Meridian_AllowRebirth)
                        {
                            return DifficultyIndex.Eclipse1;
                        }
                    }
                    else
                    {
                        if (ModeStandard_Meridian_Modify_Portal && ModeStandard_Meridian_Portals != null && ModeStandard_Meridian_Portals.Count > 1)
                        {
                            Vector3 position = Meridian_Pos;
                            List<ConfigPortalType> UsablePortals = PortalUtility.GetValidPortals_NEO(ModeStandard_Meridian_Portals);
                            PortalUtility.TrySpawnPortalCircle(UsablePortals, position, DirectorPlacementRule.PlacementMode.Direct, 30f, false);
                        }
                        if (!ModeStandard_Meridian_AllowRebirth)
                        {
                            return DifficultyIndex.Eclipse1;
                        }
                    }
                    return DifficultyIndex.Normal;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": RevealRebirthShrine IL Hook A failed");
            }
            if (ilcursor.TryGotoNext(
                x => x.MatchStfld(typeof(EntityStates.ShrineRebirth.RevealRebirthShriine), "isACExpansionEnabled")
            ))
            {
                ilcursor.Index -= 5;
                ilcursor.RemoveRange(5);
                ilcursor.EmitDelegate<Func<bool>>(() =>
                {
                    return false;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": RevealRebirthShrine IL Hook B failed");
            }
        }

        private void IL_RebirthOrPortalChoice(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchStfld(typeof(EntityStates.ShrineRebirth.RebirthOrPortalChoice), "isACExpansionEnabled")
            ))
            {
                ilcursor.Index -= 5;
                ilcursor.RemoveRange(5);
                ilcursor.EmitDelegate<Func<bool>>(() =>
                {
                    return false;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": RebirthOrPortalChoice IL Hook failed");
            }
        }

        private void IL_SolusHeartDeath(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchCallOrCallvirt(typeof(Run), "get_participatingPlayerCount")
            ))
            {
                ilcursor.Index -= 1;
                ilcursor.RemoveRange(2);
                ilcursor.EmitDelegate<Func<int>>(() =>
                {
                    if (IsEclipse())
                    {
                        if (!ModeEclipse_SolusWeb_Reward)
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (!ModeStandard_SolusWeb_Reward)
                        {
                            return 0;
                        }
                    }
                    return Run.instance.participatingPlayerCount;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": SolusHeartDeath IL Hook failed");
            }
        }

        private void IL_TeleInteractionStart(ILContext il)
        {
            //override the spawn conditions for the Celestial Orb
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchCallOrCallvirt(typeof(Run), "get_stageClearCountInCurrentLoop")
            ))
            {
                ilcursor.Index += 3;
                ilcursor.Remove();
                ilcursor.Emit(OpCodes.Ldarg_0);
                ilcursor.EmitDelegate<Func<TeleporterInteraction, int>>((self) =>
                {
                    bool result = false;
                    SceneDef sceneDef = SceneCatalog.GetSceneDefForCurrentScene();
                    if (!Run.instance.GetEventFlag("NoMysterySpace"))
                    {
                        if (IsEclipse())
                        {
                            if (ReqList.PassesReqDataList(ModeEclipse_MS_OrbReq_Data, Run.instance.stageClearCount + 1, sceneDef))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            if (ReqList.PassesReqDataList(ModeStandard_MS_OrbReq_Data, Run.instance.stageClearCount + 1, sceneDef))
                            {
                                result = true;
                            }
                        }
                    }
                    self.shouldAttemptToSpawnMSPortal = result;
                    return Run.instance.stageClearCountInCurrentLoop - 3;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Celestial Orb IL Hook failed");
            }
        }
    }
}