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
    public class Misc
    {
        internal static bool ModeStandard_AllowBeads = true;
        internal static bool ModeEclipse_AllowBeads = false;

        internal static bool TimeFlows_Meridian = false;
        internal static bool TimeFlows_SolusWeb = false;
        internal static bool TimeFlows_SolutionalHaunt = false;
        internal static bool TimeFlows_VoidRaid = false;
        internal static bool TimeFlows_Arena = false;
        internal static int Loop_OrderTeleporter = 0;

        //"RoR2/Base/arena/arena.asset"
        private SceneDef SceneDef_Arena = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
        //"RoR2/DLC1/voidraid/voidraid.asset"
        private SceneDef SceneDef_VoidRaid = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidraid/voidraid.asset").WaitForCompletion();
        //"RoR2/DLC2/meridian/meridian.asset"
        private SceneDef SceneDef_Meridian = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/meridian/meridian.asset").WaitForCompletion();
        private SceneDef SceneDef_SolutionalHaunt = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC3/solutionalhaunt/solutionalhaunt.asset").WaitForCompletion();
        private SceneDef SceneDef_SolusWeb = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC3/solusweb/solusweb.asset").WaitForCompletion();
        public Misc()
        {
            Hooks();
        }
        private void Hooks()
        {
            UpdateSceneDefs();
            if (ModeEclipse_AllowBeads)
            {
                IL.RoR2.EclipseRun.OverrideRuleChoices += new ILContext.Manipulator(IL_EclipseRunRules);
            }
            //Allows Celestial Orb to Show Up in Eclipse Runs, move this somewhere else?
            SharedHooks.Handle_EclipseRun_Start_Actions += EclipseRun_Start;
        }

        private void UpdateSceneDefs()
        {
            if (SceneDef_Arena)
            {
                SceneDef_Arena.sceneType = AdjustSceneTimeFlow(SceneDef_Arena, TimeFlows_Arena);
            }
            if (SceneDef_VoidRaid)
            {
                SceneDef_VoidRaid.sceneType = AdjustSceneTimeFlow(SceneDef_VoidRaid, TimeFlows_VoidRaid);
            }
            if (SceneDef_Meridian)
            {
                SceneDef_Meridian.sceneType = AdjustSceneTimeFlow(SceneDef_Meridian, TimeFlows_Meridian);
            }
            if (SceneDef_SolutionalHaunt)
            {
                SceneDef_SolutionalHaunt.sceneType = AdjustSceneTimeFlow(SceneDef_SolutionalHaunt, TimeFlows_SolutionalHaunt);
            }
            if (SceneDef_SolusWeb)
            {
                SceneDef_SolusWeb.sceneType = AdjustSceneTimeFlow(SceneDef_SolusWeb, TimeFlows_SolusWeb);
            }
        }

        private SceneType AdjustSceneTimeFlow(SceneDef sceneDef, bool AllowTimeFlow)
        {
            if (AllowTimeFlow)
            {
                if (sceneDef.sceneType == SceneType.Intermission)
                {
                    return SceneType.TimedIntermission;
                }
                if (sceneDef.sceneType == SceneType.UntimedStage)
                {
                    return SceneType.Stage;
                }
            }
            else
            {
                if (sceneDef.sceneType == SceneType.TimedIntermission)
                {
                    return SceneType.Intermission;
                }
                if (sceneDef.sceneType == SceneType.Stage)
                {
                    return SceneType.UntimedStage;
                }
            }
            return sceneDef.sceneType;
        }
        private void EclipseRun_Start(EclipseRun self)
        {
            if (NetworkServer.active)
            {
                self.ResetEventFlag("NoMysterySpace");
            }
        }
        private void IL_EclipseRunRules(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            if (ilcursor.TryGotoNext(
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "LunarTrinket")
            ))
            {
                ilcursor.Index -= 4;
                ilcursor.RemoveRange(9);
            }
            else
            {
                UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": EclipseRunRules LunarTrinket IL Hook failed");
            }
        }
    }
}