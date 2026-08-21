using System;
using System.IO;
using RoR2;
using R2API;
using UnityEngine;
using BepInEx.Configuration;

namespace Railroad
{
	public enum ConfigRunType : int
    {
		Invalid = -1,
		Always = 0,
		NoEclipse = 1,
    }
	public enum ConfigPortalType : int
	{
		NoPortal = -1,
		Shop = 0,
		MS = 1,
		Null = 2,
		Void = 3,
		DeepVoid = 4,
		VoidOutro = 5,
		Goldshores = 6,
		Colossus = 7,
		Destination = 8,
		HardwareProg = 9,
		HardwareProg_Haunt = 10,
		SolusShop = 11,
		SolusBackout = 12,
		SolusWeb = 13
	}
	public enum ConfigCurrencyType : int
	{
		None = 0,
		LunarCoin = 3,
		VoidCoin = 14
	}
	public enum ConfigGoldPortal : int
	{
		Never = 0,
		Vanilla = 1,
		Meridian = 2
	}

	public enum ConfigEclipseToggle : int
	{
		Never = 0,
		Always = 1,
		PostMithrix = 2
	}
	public static class Configs
	{
		//public static ConfigFile InteractableConfig;

		public static ConfigFile ConfigFile_InputHelp;
		public static ConfigFile ConfigFile_ModeStandard_Stages;
		public static ConfigFile ConfigFile_ModeEclipse_Stages;
		public static ConfigFile ConfigFile_ModeStandard_Looping;
		public static ConfigFile ConfigFile_ModeEclipse_Looping;
		public static ConfigFile ConfigFile_Interactables;
		public static ConfigFile ConfigFile_Misc;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_Stage_VoidRaid = "Planetarium";
		private const string Section_Stage_Moon2 = "Commencement";
		private const string Section_Stage_Meridian = "Prime Meridian";
		private const string Section_Stage_Goldshores = "Gilded Coast";
		private const string Section_Stage_MS = "A Moment, Fractured";
		private const string Section_Stage_Limbo = "A Moment, Whole";
		private const string Section_Stage_SolusWeb = "Solus Web";

		private const string Section_Stage_Arena = "Void Fields";

		private const string Section_Loop_Definition = "Loop Definition";
		private const string Section_Loop_Effects = "Loop Effects";
		private const string Section_Loop_Teleporter = "Teleporter";
		private const string Section_Misc_StageTimed = "Stage Timed";

		private const string Section_Enable = "!Enable Changes";
		private const string Label_Enable = "!Enable Changes";
		private const string Desc_Enable = "Allows this section to function.";
		private const string Desc_Enable_Config = "Allows all changes within this config to happen.";

		private const string Label_Eclipse = "Allow On Eclipse";
		private const string Desc_Eclipse = "Allow changes made in this section even during Eclipse.";

		private const string Label_WinPortal = "Completion Portals";
		private const string Label_WinReward = "Completion Reward";

		private const string Section_GlassFrog = "Glass Frog";

		private const string Section_PortalHelp = "Portals and Tags";

		private static string PortalListHelp;
		private static string PortalTagHelp;
		private static string PortalTagExample;
		private static string PortalStageNumExample;

		public static void Setup()
        {
			//InteractableConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_Interactables.cfg"), true);
			
			ConfigFile_InputHelp = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_InputHelp.cfg"), true);
			ConfigFile_ModeStandard_Stages = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_ModeStandard_Stages.cfg"), true);
			ConfigFile_ModeEclipse_Stages = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_ModeEclipse_Stages.cfg"), true);
			ConfigFile_ModeStandard_Looping = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_ModeStandard_Looping.cfg"), true);
			ConfigFile_ModeEclipse_Looping = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_ModeEclipse_Looping.cfg"), true);
			ConfigFile_Interactables = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_Interactables.cfg"), true);
			ConfigFile_Misc = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_Misc.cfg"), true);

			Read_StageConfig();
			Read_LoopConfig();
			Read_MiscConfig();
			Read_InteractableConfig();
			Read_InputHelpConfig();
		}
		private static void Read_LoopConfig()
        {
			Changes.Looping.ModeStandard_Artifact_Input = ConfigFile_ModeStandard_Looping.Bind(Section_Loop_Effects, "Loop Artifacts", "", "Enables the specified Artifact by their internal name upon looping.").Value;
			Changes.Looping.ModeEclipse_Artifact_Input = ConfigFile_ModeEclipse_Looping.Bind(Section_Loop_Effects, "Loop Artifacts", "", "Enables the specified Artifact by their internal name upon looping.").Value;

			Changes.Looping.ModeStandard_PrimordialTele_ReplaceReq_Input = ConfigFile_ModeStandard_Looping.Bind(Section_Loop_Teleporter, "Primordial Teleporter Conditions", "5", "Stage number for when the Primordial Teleporter will replace the regular Teleporter, can take Portal Tags for additional requirements. 0 allows any Stage number.").Value;
			Changes.Looping.ModeEclipse_PrimordialTele_ReplaceReq_Input = ConfigFile_ModeEclipse_Looping.Bind(Section_Loop_Teleporter, "Primordial Teleporter Conditions", "5", "Stage number for when the Primordial Teleporter will replace the regular Teleporter, can take Portal Tags for additional requirements. 0 allows any Stage number.").Value;
		}
		private static void Read_StageConfig()
        {
			//Changes.Stages.Enable = StageConfig.Bind(Section_Enable, Label_Enable, false, Desc_Enable_Config).Value;

			Changes.Stages.ModeStandard_Moon2_Portal_Input = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Moon2, Label_WinPortal, "", "Portals to spawn upon defeating Mithrix.").Value;
			Changes.Stages.ModeEclipse_Moon2_Portal_Input = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Moon2, Label_WinPortal, "", "Portals to spawn upon defeating Mithrix.").Value;
			Changes.Stages.ModeStandard_Moon2_Reward = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Moon2, Label_WinReward, false, "Drop Legendary items upon defeating Mithrix.").Value;
			Changes.Stages.ModeEclipse_Moon2_Reward = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Moon2, Label_WinReward, false, "Drop Legendary items upon defeating Mithrix.").Value;

			Changes.Stages.ModeStandard_Meridian_Portal_Input = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Meridian, Label_WinPortal, "Destination", "Portals to spawn upon defeating False Son.").Value;
			Changes.Stages.ModeEclipse_Meridian_Portal_Input = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Meridian, Label_WinPortal, "Destination", "Portals to spawn upon defeating False Son.").Value;
			Changes.Stages.ModeStandard_Meridian_Reward = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Meridian, Label_WinReward, true, "Drop Aurelionite Blessings upon defeating False Son.").Value;
			Changes.Stages.ModeEclipse_Meridian_Reward = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Meridian, Label_WinReward, true, "Drop Aurelionite Blessings upon defeating False Son.").Value;
			Changes.Stages.ModeStandard_Meridian_AllowRebirth = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Meridian, "Allow Rebirth", true, "Allows the Rebirth Shrine to spawn.").Value;
			Changes.Stages.ModeEclipse_Meridian_AllowRebirth = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Meridian, "Allow Rebirth", false, "Allows the Rebirth Shrine to spawn.").Value;
			Changes.Stages.ModeStandard_Meridian_ACPortal = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Meridian, "Virtual Portal", true, "Allows the extra Virtual Portal to spawn.").Value;
			Changes.Stages.ModeEclipse_Meridian_ACPortal = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Meridian, "Virtual Portal", true, "Allows the extra Virtual Portal to spawn.").Value;

			Changes.Stages.ModeStandard_VoidRaid_Portal_Input = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_VoidRaid, Label_WinPortal, "", "Portal to spawn upon defeating Voidling.").Value;
			Changes.Stages.ModeEclipse_VoidRaid_Portal_Input = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_VoidRaid, Label_WinPortal, "", "Portal to spawn upon defeating Voidling.").Value;
			Changes.Stages.ModeStandard_VoidRaid_Reward = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_VoidRaid, Label_WinReward, false, "Drop Void Potentials that contain Legendary items upon defeating Voidling.").Value;
			Changes.Stages.ModeEclipse_VoidRaid_Reward = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_VoidRaid, Label_WinReward, false, "Drop Void Potentials that contain Legendary items upon defeating Voidling.").Value;
			Changes.Stages.ModeStandard_VoidRaid_VoidOutroPortal = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_VoidRaid, "Void Outro Portal", true, "Allows the Void Outro Portal to spawn upon defeating Voidling.").Value;
			Changes.Stages.ModeEclipse_VoidRaid_VoidOutroPortal = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_VoidRaid, "Void Outro Portal", true, "Allows the Void Outro Portal to spawn upon defeating Voidling.").Value;
			

			Changes.Stages.ModeStandard_GoldShores_MeridianPortal = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Goldshores, "Colossus Portal", ConfigGoldPortal.Vanilla, "Controls how the Colossus Portal spawns on Gilded Coast. (Meridian = Always spawns and takes you to Prime Meridian)").Value;
			Changes.Stages.ModeEclipse_GoldShores_MeridianPortal = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Goldshores, "Colossus Portal", ConfigGoldPortal.Vanilla, "Controls how the Colossus Portal spawns on Gilded Coast. (Meridian = Always spawns and takes you to Prime Meridian)").Value;

			Changes.Stages.ModeStandard_MS_NeedBeads = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_MS, "Beads Required", true, "Beads of Fealty are required to go to A Moment, Whole.").Value;
			Changes.Stages.ModeEclipse_MS_NeedBeads = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_MS, "Beads Required", true, "Beads of Fealty are required to go to A Moment, Whole.").Value;
			Changes.Stages.ModeStandard_MS_OrbReq_Input = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_MS, "Celestial Orb", "SO3;PostLoop", "Stage number that the Celestial Orb is allowed to spawn on, can take Portal Tags for additional requirements. 0 allows any Stage number.").Value;
			Changes.Stages.ModeEclipse_MS_OrbReq_Input = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_MS, "Celestial Orb", "-1", "Stage number that the Celestial Orb is allowed to spawn on, can take Portal Tags for additional requirements. 0 allows any Stage number.").Value;
			//Changes.Misc.ModeStandard_AllowBeads = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_MS, "Allow Beads of Fealty", true, "Allow Beads of Fealty to appear in normal runs?").Value;
			Changes.Misc.ModeEclipse_AllowBeads = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_MS, "Allow Beads of Fealty", false, "Allow Beads of Fealty to appear in Eclipse runs?").Value;

			Changes.Stages.ModeStandard_Limbo_Portal_Input = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Limbo, Label_WinPortal, "", "Portals to spawn upon defeating the Twisted Scavenger.").Value;
			Changes.Stages.ModeEclipse_Limbo_Portal_Input = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Limbo, Label_WinPortal, "", "Portals to spawn upon defeating the Twisted Scavenger.").Value;
			Changes.Stages.ModeStandard_Limbo_Reward = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Limbo, Label_WinReward, false, "Drop Legendary items upon defeating the Twisted Scavenger.").Value;
			Changes.Stages.ModeEclipse_Limbo_Reward = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Limbo, Label_WinReward, false, "Drop Legendary items upon defeating the Twisted Scavenger.").Value;

			Changes.Stages.ModeStandard_SolusWeb_Portal_Input = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_SolusWeb, Label_WinPortal, "SolusBackout, Void", "Portals to spawn upon defeating the Solus Heart.").Value;
			Changes.Stages.ModeEclipse_SolusWeb_Portal_Input = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_SolusWeb, Label_WinPortal, "SolusBackout", "Portals to spawn upon defeating the Solus Heart.").Value;
			Changes.Stages.ModeStandard_SolusWeb_Reward = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_SolusWeb, Label_WinReward, true, "Drop Legendary Items upon purging Solus Heart.").Value;
			Changes.Stages.ModeEclipse_SolusWeb_Reward = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_SolusWeb, Label_WinReward, true, "Drop Legendary Items upon purging Solus Heart.").Value;
			Changes.Stages.ModeStandard_SolusWeb_AllowDecompile = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_SolusWeb, "Allow Decompile", true, "Allows you to accept Solus Heart's offering and end the run.").Value;
			Changes.Stages.ModeEclipse_SolusWeb_AllowDecompile = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_SolusWeb, "Allow Decompile", false, "Allows you to accept Solus Heart's offering and end the run.").Value;

			Changes.Stages.ModeStandard_Arena_VoidPortal = ConfigFile_ModeStandard_Stages.Bind(Section_Stage_Arena, "Void Portal", true, "Allows the Void Portal to spawn upon completing the Void Fields.").Value;
			Changes.Stages.ModeEclipse_Arena_VoidPortal = ConfigFile_ModeEclipse_Stages.Bind(Section_Stage_Arena, "Void Portal", false, "Allows the Void Portal to spawn upon completing the Void Fields.").Value;
		}

		private static void Read_InteractableConfig()
        {
			//Okay I'm not doing Eclipse configs because I don't want to fiddle with currency and what not between runs.
			//It's totally possible but it could be messy.
			//At most I might add portal options for Standard/Eclipse runs.
			Changes.Interactables.Frog_Enable = ConfigFile_Interactables.Bind(Section_GlassFrog, "Enable Changes", false, "Allows the configurations in this section to apply.").Value;
			Changes.Interactables.Frog_Portal = ConfigFile_Interactables.Bind(Section_GlassFrog, "Portal", ConfigPortalType.DeepVoid, "The portal to spawn upon petting the frog enough times.").Value;
			Changes.Interactables.Frog_Pet_Count = ConfigFile_Interactables.Bind(Section_GlassFrog, "Pet Count", 10, "How many times the frog needs to be pet to spawn the portal.").Value;
			Changes.Interactables.Frog_Stop_Petting = ConfigFile_Interactables.Bind(Section_GlassFrog, "Finish Petting", false, "Disable further interaction after petting the frog enough times.").Value;
			Changes.Interactables.Frog_Pet_Currency = ConfigFile_Interactables.Bind(Section_GlassFrog, "Cost Currency", ConfigCurrencyType.LunarCoin, "The type of currency required to pet the frog.").Value;
		}
		private static void Read_MiscConfig()
		{
			Changes.RunFlags.Looping_RequireDejaVu = ConfigFile_Misc.Bind(Section_Loop_Definition, "Deja Vu Requirement", false, "Mod counts it as looping once you've cleared the minimum amount of stages and are on a stage 1. (The same requirements for the Deja Vu achievement.)").Value;

			Changes.Misc.TimeFlows_Meridian = ConfigFile_Misc.Bind(Section_Misc_StageTimed, "Prime Meridian", false, "Allow time to flow normally at Prime Meridian?").Value;
			Changes.Misc.TimeFlows_SolusWeb = ConfigFile_Misc.Bind(Section_Misc_StageTimed, "Solus Web", false, "Allow time to flow normally at Solus Web?").Value;
			Changes.Misc.TimeFlows_SolutionalHaunt = ConfigFile_Misc.Bind(Section_Misc_StageTimed, "Solutional Haunt", false, "Allow time to flow normally at Solutional Haunt?").Value;
			Changes.Misc.TimeFlows_VoidRaid = ConfigFile_Misc.Bind(Section_Misc_StageTimed, "Planetarium", true, "Allow time to flow normally at the Planetarium?").Value;
			Changes.Misc.TimeFlows_Arena = ConfigFile_Misc.Bind(Section_Misc_StageTimed, "Void Fields", true, "Allow time to flow normally at the Void Fields?").Value;

			Changes.Stages.Limbo_ExtraTime = ConfigFile_Misc.Bind(Section_Stage_Limbo, "Fade Out Extra Time", 8f, "How much extra time it takes for the run to automatically end after defeating the Twisted Scavenger.").Value;
		}

		private static void Read_InputHelpConfig()
        {
			PortalListHelp = ConfigFile_InputHelp.Bind(Section_PortalHelp, "Portal List", "NoPortal, Shop, MS, Null, Void, DeepVoid, VoidOutro, GoldShores, Colossus, Destination, HardwareProg, HardwareProg_Haunt, SolusShop, SolusBackout, SolusWeb", "List of all Portal types this mod uses, check the mod's Thunderstore Page for more details.").Value;
			PortalTagHelp = ConfigFile_InputHelp.Bind(Section_PortalHelp, "Portal Tags", "PreLoop, PostLoop, PreMithrix, PostMithrix, PreTwistedScavenger, PostTwistedScavenger, PreVoidling, PostVoidling, PreFalseSon, PostFalseSon, PreSolusWing, PostSolusWing, PreSolusHeart, PostSolusHeart, PreVoidFields, PostVoidFields", "List of Tags that can be inputed along with portals, used to add additional conditions to when this mod spawns portals. Most configs should specify whether or not they accept Portal Tags.").Value;
			PortalTagExample = ConfigFile_InputHelp.Bind(Section_PortalHelp, "Portal Plus Tags Example", "Shop, VoidOutro;PostMithrix;PreVoidling, Colossus;PreFalseSon", "An example input of multiple portals using tags, this will spawn a Shop Portal, a VoidOutro Portal(If during the run Mithrix has been defeated and Voidling has not.) and Colossus Portal (If during the run False Son has not been defeated.).").Value;
			PortalStageNumExample = ConfigFile_InputHelp.Bind(Section_PortalHelp, "Portal Stage Number and Order", "SO5, 0;PostLoop", "Used by Primordial Teleport Conditions and Celestial Orb settings. SO and SN stand for Stage Order and Stage Number followed by the specific number. Stage Order is the stage's order number, while Stage Number is the run's stage number. In this example SO5 means Stage Order 5, which is Sky Meadow, Helminth Hatchery or anything modded that's a stage 5.").Value;
		}
	}
}
