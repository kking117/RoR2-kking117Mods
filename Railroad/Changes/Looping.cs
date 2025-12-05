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
    public class Looping
    {
        internal static bool Enable = false;

        //public static int Loop_MinStageCount = 5;
        public static bool Loop_DejaVu = false;

        internal static bool Loop_LoopTeleporter = false;
        internal static int Loop_OrderTeleporter = 0;

        public static bool AreLooping = false;

        private static InteractableSpawnCard BaseTeleporter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Teleporters/iscTeleporter.asset").WaitForCompletion();
        private static InteractableSpawnCard LunarTeleporter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Teleporters/iscLunarTeleporter.asset").WaitForCompletion();

        internal static string Loop_ArtifactRaw = "";
        internal static List<ArtifactDef> Loop_Artifacts = null;
        public Looping()
        {
            if (!Enable)
            {
                return;
            }
            Hooks();
        }
        private void Hooks()
        {
            On.RoR2.Run.BeginStage += Run_BeginStage;
            if (Loop_LoopTeleporter || Loop_OrderTeleporter > 0)
            {
                SceneDirector.onPrePopulateSceneServer += OnPrePopulateScene;
            }
            if (Loop_ArtifactRaw.Length > 0)
            {
                On.RoR2.ArtifactCatalog.Init += ArtifactCatalog_Init;
            }
        }
        internal static ArtifactDef ConvertStringToArtifactDef(string artifactName)
        {
            ArtifactDef returnDef = null;
            returnDef = ArtifactCatalog.FindArtifactDef(artifactName);
            if (returnDef == null)
            {
                for(int i = 0; i<ArtifactCatalog.artifactCount; i++)
                {
                    if (ArtifactCatalog.artifactDefs[i])
                    {
                        string enName = Language.GetString(ArtifactCatalog.artifactDefs[i].nameToken, "en").Replace(" ", "");
                        if (enName == artifactName)
                        {
                            returnDef = ArtifactCatalog.artifactDefs[i];
                            break;
                        }
                    }
                }
            }
            if (returnDef == null || returnDef.artifactIndex <= ArtifactIndex.None)
            {
                return null;
            }
            return returnDef;
        }
        internal static void ArtifactCatalog_Init(On.RoR2.ArtifactCatalog.orig_Init orig)
        {
            orig();
            Loop_Artifacts = new List<ArtifactDef>();
            string[] items = Loop_ArtifactRaw.Split(',');
            for (int i = 0; i < items.Length; i++)
            {
                string artifactName = items[i].Trim();
                ArtifactDef artifactDef = ConvertStringToArtifactDef(artifactName);
                if (artifactDef != null)
                {
                    if (!Loop_Artifacts.Contains(artifactDef))
                    {
                        Loop_Artifacts.Add(artifactDef);
                    }
                }
                else
                {
                    MainPlugin.ModLogger.LogWarning("Could not find ArtifactDef: [" + artifactName + "]");
                }
            }
            if (Loop_Artifacts.Count < 1)
            {
                Loop_Artifacts = null;
            }
        }
        private void OnPrePopulateScene(SceneDirector self)
        {
            if (!self.teleporterSpawnCard)
            {
                return;
            }
            SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
            if (AreLooping)
            {
                if (Loop_LoopTeleporter)
                {
                    self.teleporterSpawnCard = LunarTeleporter;
                    return;
                }
            }
            if (Loop_OrderTeleporter > 0)
            {
                if (scene.stageOrder < Loop_OrderTeleporter)
                {
                    self.teleporterSpawnCard = BaseTeleporter;
                }
                else
                {
                    self.teleporterSpawnCard = LunarTeleporter;
                }
            }
        }
        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
            if (self.loopClearCount < 1)
            {
                AreLooping = false;
            }
            if (scene)
            {
                if (StageCountsForLoop(scene))
                {
                    AreLooping = true;
                    //MainPlugin.ModLogger.LogDebug("Current stage counts as looping.");
                }
            }
            if (AreLooping)
            {
                if (Loop_Artifacts != null)
                {
                    for(int i = 0; i < Loop_Artifacts.Count; i++)
                    {
                        ExpansionDef reqDLC = Loop_Artifacts[i].requiredExpansion;
                        if (reqDLC == null || Run.instance.IsExpansionEnabled(reqDLC))
                        {
                            RunArtifactManager.instance.SetArtifactEnabledServer(Loop_Artifacts[i], true);
                        }
                    }
                }
            }
        }

        private bool StageCountsForLoop(SceneDef scene)
        {
            //MainPlugin.ModLogger.LogDebug(scene.nameToken + ".stageOrder = " + scene.stageOrder);
            //MainPlugin.ModLogger.LogDebug(scene.nameToken + ".sceneType = " + scene.sceneType);
            //Void Field = 97-Intermission
            //Prime Meridian = 96-Untimed
            //Gilded Coast = 96-Intermission
            //Moon = 6-Invalid
            //Moon2 = 6-Stage
            //Void Locus = 99-Stage
            //Planetarium = 99-Stage
            //Bazaar = 98-Intermission
            //A Moment, Fractured = 100-Intermission
            //A Moment, Whole = 99-Intermission
            //So from this:
            //Stage = Time Moves and it counts as a Stage Clear
            //Untimed = Time does not move and it counts as a Stage Clear
            //Intermission = Time does not move and it does not count as a Stage Clear
            if (Loop_DejaVu)
            {
                if (scene.sceneType != SceneType.Stage)
                {
                    return false;
                }
                if (scene.isFinalStage)
                {
                    return false;
                }
                if (scene.stageOrder == 1)
                {
                    return Run.instance.loopClearCount > 0;
                }
                return false;
            }
            return Run.instance.loopClearCount > 0;
        }
    }
}