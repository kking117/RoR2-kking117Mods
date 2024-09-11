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

        internal static bool VoidRaid_Eclipse = false;
        internal static ConfigPortalType VoidRaid_Portal = ConfigPortalType.NoPortal;
        internal static bool VoidRaid_Reward = false;
        internal static bool VoidRaid_VoidOutroPortal = true;

        internal static bool MS_NeedBeads = true;

        internal static bool Limbo_Eclipse = false;
        internal static ConfigPortalType Limbo_Portal = ConfigPortalType.NoPortal;
        internal static bool Limbo_Reward = true;

        internal static bool Arena_VoidPortal = true;

        internal static ConfigGoldPortal GoldShores_MeridianPortal = ConfigGoldPortal.Vanilla;

        //internal static bool MeridianPath_Allow_Seer = true;
        //internal static bool MeridianPath_Allow_Shrine = true;

        //internal static bool VoidPath_Allow_Seer = true;
        //internal static bool VoidPath_Allow_Portal = true;
        private static Vector3 Moon2_Pos = new Vector3(-89f, 491.5f, 1.15f);
        private static Vector3 Limbo_Pos = new Vector3(-6.7f, -8.6f, 87f);

        private InteractableSpawnCard Portal_Shop = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/PortalShop/iscShopPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_MS = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/PortalMS/iscMSPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_Arena = null;
        private InteractableSpawnCard Portal_Void = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/PortalVoid/iscVoidPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_DeepVoid = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/DeepVoidPortal/iscDeepVoidPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_VoidOutro = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidOutroPortal/iscVoidOutroPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_Goldshores = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/PortalGoldshores/iscGoldshoresPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_Colossus = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscColossusPortal.asset").WaitForCompletion();
        private InteractableSpawnCard Portal_Destination = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC2/iscDestinationPortal.asset").WaitForCompletion();

        private SceneDef Arena_SceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
        private SceneDef Meridian_SceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/meridian/meridian.asset").WaitForCompletion();
        private ExpansionDef DLC1Def = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();
        private ExpansionDef DLC2Def = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC2/Common/DLC2.asset").WaitForCompletion();
        public Stages()
        {
            if (!Enable)
            {
                return;
            }
            ClampConfig();
            CreateSpawnCards();
            Hooks();
        }
        private void ClampConfig()
        {
            Moon2_Portal = (ConfigPortalType)Math.Min((int)Moon2_Portal, (int)ConfigPortalType.Destination);
            VoidRaid_Portal = (ConfigPortalType)Math.Min((int)VoidRaid_Portal, (int)ConfigPortalType.Destination);
            Limbo_Portal = (ConfigPortalType)Math.Min((int)Limbo_Portal, (int)ConfigPortalType.Destination);
            Meridian_Portal = (ConfigPortalType)Math.Min((int)Meridian_Portal, (int)ConfigPortalType.Destination);
            GoldShores_MeridianPortal = (ConfigGoldPortal)Math.Min((int)GoldShores_MeridianPortal, (int)ConfigGoldPortal.Meridian);
        }
        private void CreateSpawnCards()
        {
            Portal_Arena = ScriptableObject.CreateInstance<InteractableSpawnCard>();
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
                GameObject portal = TrySpawnPortal(VoidRaid_Portal, position, DirectorPlacementRule.PlacementMode.Direct);
                if (portal)
                {
                    position = portal.transform.position;
                }
                if (VoidRaid_Reward)
                {
                    position.y += 1f;
                    TryDropItems(Run.instance.availableTier3DropList, position);
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

        private void TryDropItems(List<PickupIndex> dropList, Vector3 location)
        {
            int playerCount = Run.instance.participatingPlayerCount;
            if (playerCount > 0)
            {
                int maxItems = dropList.Count;
                if (maxItems > 0)
                {
                    PickupIndex pickupIndex = dropList[UnityEngine.Random.Range(0, maxItems - 1)];

                    float horiAngle = 360f / playerCount;
                    Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                    Quaternion quaternion = Quaternion.AngleAxis(horiAngle, Vector3.up);
                    int i = 0;
                    while (i < playerCount)
                    {
                        PickupDropletController.CreatePickupDroplet(pickupIndex, location, vector);
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
                    if (Meridian_Eclipse || IsEclipse())
                    {
                        if (Meridian_AllowRebirth)
                        {
                            return DifficultyIndex.Invalid;
                        }
                        return (DifficultyIndex)99;
                    }
                    return Run.instance.selectedDifficulty;
                });
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": RevealRebirthShrine IL Hook failed");
            }
        }
    }
}