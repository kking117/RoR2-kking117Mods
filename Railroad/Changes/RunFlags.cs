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
    public enum RunProgressFlags : int
    {
        None = 0,
        Looping = 1,
        VoidFields = 2,
        Mithrix = 4,
        TwistedScavenger = 8,
        Voidling = 16,
        FalseSon = 32,
        SolusWing = 64,
        SolusHeart = 128
    }
    public class RunFlags
    {
        internal static bool Enable = true;
        public static bool Looping_RequireDejaVu = false;

        public static RunProgressFlags SaveData_RunProgressFlags = RunProgressFlags.None;
        //1 = Mithrix
        //2 = Lunar Scavengers
        //4 = Voidling
        //8 = False Son
        //16 = Solus Wing
        //32 = Solus Heart
        public RunFlags()
        {
            if (!Enable)
            {
                return;
            }
            //ClampConfig();
            Hooks();
        }

        internal static void LogRunData()
        {
            MainPlugin.ModLogger.LogInfo("====LOGGING RUN DATA====");
            if ((SaveData_RunProgressFlags & RunProgressFlags.Looping) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Currently Looping.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.VoidFields) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Void Fields, cleared.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.Mithrix) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Mithrix, defeated.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.TwistedScavenger) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Twisted Scavenger, defeated.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.Voidling) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Voidling, defeated.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.FalseSon) != 0)
            {
                MainPlugin.ModLogger.LogInfo("False Son, defeated.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.SolusWing) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Solus Wing, defeated.");
            }
            if ((SaveData_RunProgressFlags & RunProgressFlags.SolusHeart) != 0)
            {
                MainPlugin.ModLogger.LogInfo("Solus Heart, defeated.");
            }
            MainPlugin.ModLogger.LogInfo("====LOGGING END====");
        }
        private void Hooks()
        {
            SharedHooks.Handle_Mithrix_Clear_Actions += Mithrix_Clear;
            SharedHooks.Handle_TwistedScavenger_Clear_Actions += TwistedScavenger_Clear;
            SharedHooks.Handle_Voidling_Clear_Actions += Voidling_Clear;
            SharedHooks.Handle_FalseSon_Clear_Actions += FalseSon_Clear;
            SharedHooks.Handle_SolusWing_Clear_Actions += SolusWing_Clear;
            SharedHooks.Handle_SolusHeart_Clear_Actions += SolusHeart_Clear;
            On.RoR2.ArenaMissionController.EndRound += Arena_EndRound;
            On.RoR2.Run.Start += Run_Start;
            On.RoR2.Run.BeginStage += Run_BeginStage;
        }
        private void Mithrix_Clear(EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            SaveData_RunProgressFlags |= RunProgressFlags.Mithrix;
            //MainPlugin.ModLogger.LogInfo("Mithrix, defeated.");
        }
        private void TwistedScavenger_Clear(EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
        {
            SaveData_RunProgressFlags |= RunProgressFlags.TwistedScavenger;
            //MainPlugin.ModLogger.LogInfo("Twisted Scavenger, defeated.");
        }
        private void Voidling_Clear(RoR2.VoidRaidGauntletController self)
        {
            SaveData_RunProgressFlags |= RunProgressFlags.Voidling;
            //MainPlugin.ModLogger.LogInfo("Voidling, defeated.");
        }
        private void FalseSon_Clear(EntityStates.MeridianEvent.Phase3 self)
        {
            SaveData_RunProgressFlags |= RunProgressFlags.FalseSon;
            //MainPlugin.ModLogger.LogInfo("False Son, defeated.");
        }
        private void SolusWing_Clear(EntityStates.SolusWing2.Mission5Death self)
        {
            SaveData_RunProgressFlags |= RunProgressFlags.SolusWing;
            //MainPlugin.ModLogger.LogInfo("Solus Wing, defeated.");
        }
        private void SolusHeart_Clear(EntityStates.SolusHeart.Death.MissionCompleted self)
        {
            SaveData_RunProgressFlags |= RunProgressFlags.SolusHeart;
            //MainPlugin.ModLogger.LogInfo("Solus Heart, defeated.");
        }
        private void Arena_EndRound(On.RoR2.ArenaMissionController.orig_EndRound orig, RoR2.ArenaMissionController self)
        {
            orig(self);
            if (self.clearedRounds >= self.totalRoundsMax)
            {
                SaveData_RunProgressFlags |= RunProgressFlags.VoidFields;
            }
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, RoR2.Run self)
        {
            orig(self);
            SaveData_RunProgressFlags = 0;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            SceneDef scene = SceneCatalog.GetSceneDefForCurrentScene();
            bool LoopFlag = false;
            if (scene)
            {
                if (StageCountsForLoop(scene))
                {
                    LoopFlag = true;
                }
            }
            if (LoopFlag)
            {
                SaveData_RunProgressFlags |= RunProgressFlags.Looping;
            }
            else
            {
                SaveData_RunProgressFlags = SaveData_RunProgressFlags & ~RunProgressFlags.Looping;
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
            if (Looping_RequireDejaVu)
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