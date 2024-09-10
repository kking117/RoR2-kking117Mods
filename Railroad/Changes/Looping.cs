using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace Railroad.Changes
{
    public class Looping
    {
        internal static bool Enable = false;

        public static int Loop_MinStageCount = 5;
        public static bool Loop_DejaVu = false;

        internal static bool Loop_EnableHonor = false;

        internal static bool Loop_LoopTeleporter = false;
        internal static int Loop_OrderTeleporter = 0;

        public static bool AreLooping = false;

        private static ArtifactDef HonorDef = Addressables.LoadAssetAsync<ArtifactDef>("RoR2/Base/EliteOnly/EliteOnly.asset").WaitForCompletion();
        private static InteractableSpawnCard BaseTeleporter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Teleporters/iscTeleporter.asset").WaitForCompletion();
        private static InteractableSpawnCard LunarTeleporter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Teleporters/iscLunarTeleporter.asset").WaitForCompletion();
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
            if (Run.instance.stageClearCount < 1)
            {
                AreLooping = false;
            }
            SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
            if (scene)
            {
                if (StageCountsForLoop(scene))
                {
                    AreLooping = true;
                    MainPlugin.ModLogger.LogDebug("Current stage counts as looping.");
                }
            }
            if (Loop_EnableHonor && AreLooping)
            {
                RunArtifactManager.instance.SetArtifactEnabledServer(HonorDef, true);
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
                    return Run.instance.stageClearCount >= Loop_MinStageCount;
                }
                return false;
            }
            return Run.instance.stageClearCount >= Loop_MinStageCount;
        }
    }
}