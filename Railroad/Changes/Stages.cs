using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Railroad.Changes
{
    public class Stages
    {
        internal static bool Enable = false;

        internal static bool Moon2_Eclipse = false;
        internal static ConfigPortalType Moon2_Portal = ConfigPortalType.NoPortal;
        internal static bool Moon2_Reward = false;

        internal static bool Meridian_Eclipse = false;
        internal static ConfigPortalType Meridian_Portal = ConfigPortalType.NoPortal;
        internal static bool Meridian_Reward = true;
        internal static bool Meridian_AllowRebirth = true;
        internal static bool Meridian_ACPortal = true;

        internal static bool VoidRaid_Eclipse = false;
        internal static ConfigPortalType VoidRaid_Portal = ConfigPortalType.NoPortal;
        internal static bool VoidRaid_Reward = false;
        internal static bool VoidRaid_VoidOutroPortal = true;
        internal static bool VoidRaid_TimeFlows = true;

        internal static bool MS_NeedBeads = true;

        internal static bool Limbo_Eclipse = false;
        internal static ConfigPortalType Limbo_Portal = ConfigPortalType.NoPortal;
        internal static bool Limbo_Reward = true;

        internal static bool Arena_VoidPortal = true;
        internal static bool Arena_TimeFlows = true;

        internal static ConfigGoldPortal GoldShores_MeridianPortal = ConfigGoldPortal.Vanilla;

        //internal static bool MeridianPath_Allow_Seer = true;
        //internal static bool MeridianPath_Allow_Shrine = true;

        //internal static bool VoidPath_Allow_Seer = true;
        //internal static bool VoidPath_Allow_Portal = true;
        private static Vector3 Moon2_Pos = new Vector3(-89f, 493.0f, 1.15f);
        private static Vector3 Limbo_Pos = new Vector3(-6.7f, -8.6f, 87f);

        //"RoR2/DLC1/OptionPickup/OptionPickup.prefab"
        private GameObject VoidPotential = Addressables.LoadAssetAsync<GameObject>("f8e3413a378bd7c44aa09bed0020eaf5").WaitForCompletion();

        //"RoR2/Base/Common/dtTier3Item.asset"
        private RoR2.BasicPickupDropTable Tier3PickupTable = Addressables.LoadAssetAsync<BasicPickupDropTable>("abd505260a23e9b449202c055554b77b").WaitForCompletion();

        //"RoR2/Base/arena/arena.asset"
        private SceneDef Scene_Arena = Addressables.LoadAssetAsync<SceneDef>("a478a034d8da76244b2e1fb463ef1b81").WaitForCompletion();
        //"RoR2/DLC1/voidraid/voidraid.asset"
        private SceneDef Scene_VoidRaid = Addressables.LoadAssetAsync<SceneDef>("223a0f0a86052654a9e473d13f77cb41").WaitForCompletion();

        //"RoR2/Base/PortalShop/iscShopPortal.asset"
        private InteractableSpawnCard Portal_Shop = Addressables.LoadAssetAsync<InteractableSpawnCard>("b7909967e0f972543ab5f7367f45561b").WaitForCompletion();
        //"RoR2/Base/PortalMS/iscMSPortal.asset"
        private InteractableSpawnCard Portal_MS = Addressables.LoadAssetAsync<InteractableSpawnCard>("f6b2da500512ed5478412902e42605be").WaitForCompletion();
        private InteractableSpawnCard Portal_Arena = null;
        //"RoR2/DLC1/PortalVoid/iscVoidPortal.asset"
        private InteractableSpawnCard Portal_Void = Addressables.LoadAssetAsync<InteractableSpawnCard>("4f32e6a9f71d4e44dad18dec8eb07ef8").WaitForCompletion();
        //"RoR2/DLC1/DeepVoidPortal/iscDeepVoidPortal.asset"
        private InteractableSpawnCard Portal_DeepVoid = Addressables.LoadAssetAsync<InteractableSpawnCard>("19dc6bf0a4d213340980ead7d91df95c").WaitForCompletion();
        //"RoR2/DLC1/VoidOutroPortal/iscVoidOutroPortal.asset"
        private InteractableSpawnCard Portal_VoidOutro = Addressables.LoadAssetAsync<InteractableSpawnCard>("c3bd3e121f973f04599d514386333d47").WaitForCompletion();
        //"RoR2/Base/PortalGoldshores/iscGoldshoresPortal.asset"
        private InteractableSpawnCard Portal_Goldshores = Addressables.LoadAssetAsync<InteractableSpawnCard>("824a39e11dff6d847996704ffe6be27f").WaitForCompletion();
        //"RoR2/DLC2/iscColossusPortal.asset"
        private InteractableSpawnCard Portal_Colossus = Addressables.LoadAssetAsync<InteractableSpawnCard>("f5ecc008531950140a8137d2a3637395").WaitForCompletion();
        //"RoR2/DLC2/iscDestinationPortal.asset"
        private InteractableSpawnCard Portal_Destination = Addressables.LoadAssetAsync<InteractableSpawnCard>("644f245483eced544bf8885e0080023f").WaitForCompletion();
        //"RoR2/DLC3/iscHardwareProgPortal.asset"
        private InteractableSpawnCard Portal_HardwareProg = Addressables.LoadAssetAsync<InteractableSpawnCard>("c4e7a9ed6153edf488cf434d843311f5").WaitForCompletion();
        //"RoR2/DLC3/iscHardwareProgPortal_Haunt.asset"
        private InteractableSpawnCard Portal_HardwareProg_Haunt = Addressables.LoadAssetAsync<InteractableSpawnCard>("7c01731f4ba8cb548af2a35bce9105d3").WaitForCompletion();
        //"RoR2/DLC3/iscSolusShopPortal.asset"
        private InteractableSpawnCard Portal_SolusShop = Addressables.LoadAssetAsync<InteractableSpawnCard>("07fc379a8d5d3c44c9211730bf4e1572").WaitForCompletion();
        //"RoR2/DLC3/iscSolusPortalBackout.asset"
        private InteractableSpawnCard Portal_SolusBackout = Addressables.LoadAssetAsync<InteractableSpawnCard>("b95252d60a5a463488b6d8c4fcb4bd4d").WaitForCompletion();
        private InteractableSpawnCard Portal_SolusWeb = null;

        //"RoR2/Base/arena/arena.asset"
        private SceneDef Arena_SceneDef = Addressables.LoadAssetAsync<SceneDef>("a478a034d8da76244b2e1fb463ef1b81").WaitForCompletion();
        //"RoR2/DLC2/meridian/meridian.asset"
        private SceneDef Meridian_SceneDef = Addressables.LoadAssetAsync<SceneDef>("520b764e3ac5743459fd923204083395").WaitForCompletion();
        //"RoR2/DLC1/Common/DLC1.asset"
        private ExpansionDef DLC1Def = Addressables.LoadAssetAsync<ExpansionDef>("d4f30c23b971a9b428e2796dc04ae099").WaitForCompletion();
        //"RoR2/DLC2/Common/DLC2.asset"
        private ExpansionDef DLC2Def = Addressables.LoadAssetAsync<ExpansionDef>("851f234056d389b42822523d1be6a167").WaitForCompletion();
        //"RoR2/DLC3/DLC3.asset"
        private ExpansionDef DLC3Def = Addressables.LoadAssetAsync<ExpansionDef>("234e83997deed274291470be69e7662e").WaitForCompletion();
        public Stages()
        {
            if (!Enable)
            {
                return;
            }
            ClampConfig();
            CreateSpawnCards();
            UpdateSceneDefs();
            Hooks();
        }
        private void ClampConfig()
        {
            Moon2_Portal = (ConfigPortalType)Math.Min((int)Moon2_Portal, (int)ConfigPortalType.SolusWeb);
            VoidRaid_Portal = (ConfigPortalType)Math.Min((int)VoidRaid_Portal, (int)ConfigPortalType.SolusWeb);
            Limbo_Portal = (ConfigPortalType)Math.Min((int)Limbo_Portal, (int)ConfigPortalType.SolusWeb);
            Meridian_Portal = (ConfigPortalType)Math.Min((int)Meridian_Portal, (int)ConfigPortalType.SolusWeb);
            GoldShores_MeridianPortal = (ConfigGoldPortal)Math.Min((int)GoldShores_MeridianPortal, (int)ConfigGoldPortal.Meridian);
        }
        private void UpdateSceneDefs()
        {
            if (Scene_Arena && !Arena_TimeFlows)
            {
                if (Scene_Arena.sceneType == SceneType.TimedIntermission)
                {
                    Scene_Arena.sceneType = SceneType.Intermission;
                }
                if (Scene_Arena.sceneType == SceneType.Stage)
                {
                    Scene_Arena.sceneType = SceneType.UntimedStage;
                }
            }
            if (Scene_VoidRaid && !VoidRaid_TimeFlows)
            {
                if (Scene_VoidRaid.sceneType == SceneType.TimedIntermission)
                {
                    Scene_VoidRaid.sceneType = SceneType.Intermission;
                }
                if (Scene_VoidRaid.sceneType == SceneType.Stage)
                {
                    Scene_VoidRaid.sceneType = SceneType.UntimedStage;
                }
            }
        }
        private void CreateSpawnCards()
        {
            Portal_Arena = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            //"RoR2/Base/PortalArena/PortalArena.prefab"
            Portal_Arena.prefab = Addressables.LoadAssetAsync<GameObject>("36b16aad972162e44a8ab73cca22e16e").WaitForCompletion();
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
            Portal_SolusWeb.prefab = Addressables.LoadAssetAsync<GameObject>("9592265fcd09fc643b2495b5e4ebac8f").WaitForCompletion();
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
        private void Hooks()
        {
            if (Moon2_Portal != ConfigPortalType.NoPortal || Moon2_Reward)
            {
                On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += Moon2Complete;
            }
            if (VoidRaid_Portal != ConfigPortalType.NoPortal || VoidRaid_Reward)
            {
                On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += VoidRaidOnPortal;
            }
            if (!Arena_VoidPortal)
            {
                On.RoR2.PortalSpawner.AttemptSpawnPortalServer += PortalSpawner_SpawnPortal;
            }
            if (Meridian_Portal != ConfigPortalType.Destination)
            {
                On.RoR2.ShrineRebirthController.Start += ShrineRebirthController_Start;
            }
            if (!Meridian_AllowRebirth || !Meridian_ACPortal)
            {
                On.EntityStates.ShrineRebirth.RebirthOrPortalChoice.FixedUpdate += RebirthShrine_Update;
                On.EntityStates.ShrineRebirth.RevealRebirthShriine.FixedUpdate += RebirthShrine_UpdateNoRebirth;
            }
            if (!Meridian_Reward)
            {
                IL.EntityStates.FalseSonBoss.SkyJumpDeathState.GiveColossusItem += new ILContext.Manipulator(IL_FalseSonBossComplete);
            }
            IL.EntityStates.ShrineRebirth.RevealRebirthShriine.OnEnter += new ILContext.Manipulator(IL_RevealRebirthShrine);
            if (!MS_NeedBeads)
            {
                IL.EntityStates.Interactables.MSObelisk.EndingGame.DoFinalAction += new ILContext.Manipulator(IL_OnObliteration);
            }
            if (GoldShores_MeridianPortal == ConfigGoldPortal.Meridian)
            {
                On.EntityStates.Missions.Goldshores.Exit.IsValidStormTier += IsValidStormTier;
                On.EntityStates.Missions.Goldshores.Exit.OnEnter += Goldshores_Exit;
            }
            else if (GoldShores_MeridianPortal == ConfigGoldPortal.Never)
            {
                On.EntityStates.Missions.Goldshores.Exit.IsValidStormTier += IsValidStormTier;
            }
            if (Limbo_Portal != ConfigPortalType.NoPortal || Limbo_Reward)
            {
                On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += LimboComplete;
            }
        }

        private void RebirthShrine_Update(On.EntityStates.ShrineRebirth.RebirthOrPortalChoice.orig_FixedUpdate orig, EntityStates.ShrineRebirth.RebirthOrPortalChoice self)
        {
            if (!self.isACExpansionEnabled || !Meridian_ACPortal)
            {
                return;
            }
            self.timer -= self.GetDeltaTime();
            if (self.timer <= 0f && !self.acPortalTriggered)
            {
                self.acPortalTriggered = true;
                self.SpawnACPortal();
            }
        }
        private void RebirthShrine_UpdateNoRebirth(On.EntityStates.ShrineRebirth.RevealRebirthShriine.orig_FixedUpdate orig, EntityStates.ShrineRebirth.RevealRebirthShriine self)
        {
            if (!self.isACExpansionEnabled || !Meridian_ACPortal)
            {
                return;
            }
            if (!self.isEclipse && Meridian_AllowRebirth)
            {
                return;
            }
            self.timer -= self.GetDeltaTime();
            if (self.timer <= 0f && !self.acPortalTriggered)
            {
                self.acPortalTriggered = true;
                self.SpawnACPortal();
            }
        }
        private bool PortalSpawner_SpawnPortal(On.RoR2.PortalSpawner.orig_AttemptSpawnPortalServer orig, PortalSpawner self)
        {
            SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
            if (scene)
            {
                if (scene == Arena_SceneDef)
                {
                    return false;
                }
            }
            return orig(self);
        }
        private bool IsValidStormTier(On.EntityStates.Missions.Goldshores.Exit.orig_IsValidStormTier orig, EntityStates.Missions.Goldshores.Exit self)
        {
            return false;
        }
        private void Goldshores_Exit(On.EntityStates.Missions.Goldshores.Exit.orig_OnEnter orig, EntityStates.Missions.Goldshores.Exit self)
        {
            orig(self);
            GameObject gameObject = TrySpawnPortal(ConfigPortalType.Colossus, self.transform.position);
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
        private void ShrineRebirthController_Start(On.RoR2.ShrineRebirthController.orig_Start orig, ShrineRebirthController self)
        {
            orig(self);
            if (Meridian_Eclipse || !IsEclipse())
            {
                self.helminthPortalISC = GetSpawnCardFromIndex(Meridian_Portal);
            }
        }
        private void LimboComplete(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
        {
            orig(self);
            if (Limbo_Eclipse || !IsEclipse())
            {
                Vector3 position = Limbo_Pos;
                GameObject portal = TrySpawnPortal(Limbo_Portal, position, DirectorPlacementRule.PlacementMode.Approximate);
                if (portal)
                {
                    position = portal.transform.position;
                }
                if (Limbo_Reward)
                {
                    position.y += 1f;
                    TryDropItems(Run.instance.availableTier3DropList, position);
                }
            }
        }
        private void Moon2Complete(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);
            if (Moon2_Eclipse || !IsEclipse())
            {
                TrySpawnPortal(Moon2_Portal, Moon2_Pos, DirectorPlacementRule.PlacementMode.Direct);
                if (Moon2_Reward)
                {
                    TryDropItems(Run.instance.availableTier3DropList, Moon2_Pos);
                }
            }
        }
        private void VoidRaidOnPortal(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, VoidRaidGauntletController self)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (VoidRaid_Eclipse || !IsEclipse())
            {
                if (VoidRaid_VoidOutroPortal)
                {
                    orig(self);
                }
                Vector3 position = self.currentDonut.returnPoint.transform.position;
                GameObject portal = TrySpawnPortal_FindFloor(VoidRaid_Portal, position, DirectorPlacementRule.PlacementMode.Direct);
                if (portal)
                {
                    position = portal.transform.position;
                }
                if (VoidRaid_Reward)
                {
                    position.y += 1f;
                    TryDropPotential(Tier3PickupTable, position, 3);
                }
            }
            else
            {
                orig(self);
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
        private bool CanSpawnPortal(ConfigPortalType portalType)
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
        private InteractableSpawnCard GetSpawnCardFromIndex(ConfigPortalType portalType)
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

        private GameObject TrySpawnPortal_FindFloor(ConfigPortalType portalType, Vector3 location, DirectorPlacementRule.PlacementMode placementMode = DirectorPlacementRule.PlacementMode.Approximate)
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
                    if (raycastHit.point.y +1f <= location.y)
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
        private GameObject TrySpawnPortal(ConfigPortalType portalType, Vector3 location, DirectorPlacementRule.PlacementMode placementMode = DirectorPlacementRule.PlacementMode.Approximate)
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
        private void TryDropPotential(PickupDropTable dropTable, Vector3 location, int optionCount)
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
        private void TryDropItems(List<PickupIndex> dropList, Vector3 location)
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
        private void IL_OnObliteration(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchLdcI4(0),
                x => x.MatchStloc(0)
            ))
            {
                ilcursor.Remove();
                ilcursor.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": OnObliteration IL Hook failed");
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
                ilcursor.Emit(OpCodes.Ldc_I4_0);
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
                    if (IsEclipse() && !Meridian_Eclipse)
                    {
                        return Run.instance.selectedDifficulty;
                    }
                    if (Meridian_AllowRebirth)
                    {
                        return DifficultyIndex.Normal;
                    }
                    return DifficultyIndex.Eclipse1;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": RevealRebirthShrine IL Hook failed");
            }
        }
    }
}