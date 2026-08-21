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
        internal static bool Loop_LoopTeleporter = false;
        internal static int Loop_OrderTeleporter = 0;

        private static InteractableSpawnCard BaseTeleporter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Teleporters/iscTeleporter.asset").WaitForCompletion();
        private static InteractableSpawnCard LunarTeleporter = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Teleporters/iscLunarTeleporter.asset").WaitForCompletion();
        private static string LunarTeleporterOBJName = "LunarTeleporter Variant(Clone)";

        internal static string ModeStandard_PrimordialTele_ReplaceReq_Input = "";
        internal static string ModeEclipse_PrimordialTele_ReplaceReq_Input = "";
        internal static List<ReqAllowData> ModeStandard_PrimordialTele_Data = null;
        internal static List<ReqAllowData> ModeEclipse_PrimordialTele_Data = null;

        internal static string ModeStandard_Artifact_Input = "";
        internal static string ModeEclipse_Artifact_Input = "";
        internal static List<ArtifactDef> ModeStandard_Artifacts = null;
        internal static List<ArtifactDef> ModeEclipse_Artifacts = null;
        public Looping()
        {
            ProcessConfig();
            Hooks();
        }
        private void ProcessConfig()
        {
            if (ModeStandard_PrimordialTele_ReplaceReq_Input.Length > 0)
            {
                ModeStandard_PrimordialTele_Data = ReqList.ReadStageNumberInput(ModeStandard_PrimordialTele_ReplaceReq_Input, "Looping|Standard|Primordial Teleporter Conditions");
            }
            if (ModeEclipse_PrimordialTele_ReplaceReq_Input.Length > 0)
            {
                ModeEclipse_PrimordialTele_Data = ReqList.ReadStageNumberInput(ModeEclipse_PrimordialTele_ReplaceReq_Input, "Looping|Eclipse|Primordial Teleporter Conditions");
            }
        }
        private void Hooks()
        {
            On.RoR2.Run.BeginStage += Run_BeginStage;
            SceneDirector.onPrePopulateSceneServer += OnPrePopulateScene;
            On.RoR2.ArtifactCatalog.Init += ArtifactCatalog_Init;
            //On.RoR2.Run.OnServerTeleporterPlaced += OnTeleporterPlaced;
            //To Lock the Primordial Teleporter Prongs
            //Discuss this with Matsan post update, to discuss the viability of doing something like this, since portal orbs exist.
            //On.EntityStates.LunarTeleporter.Active.OnEnter += LunarProngs_Active;
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
            ModeStandard_Artifacts = new List<ArtifactDef>();
            if (ModeStandard_Artifact_Input.Length > 0)
            {
                string[] items = ModeStandard_Artifact_Input.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string artifactName = items[i].Trim();
                    ArtifactDef artifactDef = ConvertStringToArtifactDef(artifactName);
                    if (artifactDef != null)
                    {
                        if (!ModeStandard_Artifacts.Contains(artifactDef))
                        {
                            ModeStandard_Artifacts.Add(artifactDef);
                        }
                    }
                    else
                    {
                        MainPlugin.ModLogger.LogWarning("Could not find ArtifactDef: [" + artifactName + "]");
                    }
                }
                if (ModeStandard_Artifacts.Count < 1)
                {
                    ModeStandard_Artifacts = null;
                }
            }
            ModeEclipse_Artifacts = new List<ArtifactDef>();
            if (ModeEclipse_Artifact_Input.Length > 0)
            {
                string[] items = ModeEclipse_Artifact_Input.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string artifactName = items[i].Trim();
                    ArtifactDef artifactDef = ConvertStringToArtifactDef(artifactName);
                    if (artifactDef != null)
                    {
                        if (!ModeEclipse_Artifacts.Contains(artifactDef))
                        {
                            ModeEclipse_Artifacts.Add(artifactDef);
                        }
                    }
                    else
                    {
                        MainPlugin.ModLogger.LogWarning("Could not find ArtifactDef: [" + artifactName + "]");
                    }
                }
                if (ModeEclipse_Artifacts.Count < 1)
                {
                    ModeEclipse_Artifacts = null;
                }
            }
        }
        /*private void OnTeleporterPlaced(On.RoR2.Run.orig_OnServerTeleporterPlaced orig, Run self, SceneDirector sceneDirector, GameObject teleporter)
        {
            orig(self, sceneDirector, teleporter);
            if (teleporter)
            {
                if (teleporter.name == LunarTeleporterOBJName)
                {
                    MainPlugin.ModLogger.LogInfo("Lunar Teleporter");
                }
            }
        }*/
        /*private void LunarProngs_Active(On.EntityStates.LunarTeleporter.Active.orig_OnEnter orig, EntityStates.LunarTeleporter.Active self)
        {
            orig(self);
            if ((RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Mithrix) == 0)
            {
                self.preferredInteractability = Interactability.ConditionsNotMet;
            }
        }*/
        private void OnPrePopulateScene(SceneDirector self)
        {
            if (!self.teleporterSpawnCard)
            {
                return;
            }
            //This may cause issues if another mod adds special teleporters.
            //It may not however, Conduit Canyon seems to work properly.
            //I assume these cases use their own systems.
            SceneDef sceneDef = SceneCatalog.GetSceneDefForCurrentScene();
            if (IsEclipse())
            {
                if (ReqList.PassesReqDataList(ModeEclipse_PrimordialTele_Data, Run.instance.stageClearCount+1, sceneDef))
                {
                    self.teleporterSpawnCard = LunarTeleporter;
                    return;
                }
                self.teleporterSpawnCard = BaseTeleporter;
            }
            else
            {
                if (ReqList.PassesReqDataList(ModeStandard_PrimordialTele_Data, Run.instance.stageClearCount + 1, sceneDef))
                {
                    self.teleporterSpawnCard = LunarTeleporter;
                    return;
                }
                self.teleporterSpawnCard = BaseTeleporter;
            }
        }
        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            if ((RunFlags.SaveData_RunProgressFlags & RunProgressFlags.Looping) != 0)
            {
                if (IsEclipse())
                {
                    for (int i = 0; i < ModeEclipse_Artifacts.Count; i++)
                    {
                        ExpansionDef reqDLC = ModeEclipse_Artifacts[i].requiredExpansion;
                        if (reqDLC == null || Run.instance.IsExpansionEnabled(reqDLC))
                        {
                            RunArtifactManager.instance.SetArtifactEnabledServer(ModeEclipse_Artifacts[i], true);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < ModeStandard_Artifacts.Count; i++)
                    {
                        ExpansionDef reqDLC = ModeStandard_Artifacts[i].requiredExpansion;
                        if (reqDLC == null || Run.instance.IsExpansionEnabled(reqDLC))
                        {
                            RunArtifactManager.instance.SetArtifactEnabledServer(ModeStandard_Artifacts[i], true);
                        }
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
    }
}