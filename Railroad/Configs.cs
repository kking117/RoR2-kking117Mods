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
	public enum ConfigGoldPortal : int
	{
		Never = 0,
		Vanilla = 1,
		Meridian = 2
	}
	public static class Configs
	{
		public static ConfigFile LoopConfig;
		public static ConfigFile StageConfig;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_Stage_VoidRaid = "Planetarium";
		private const string Section_Stage_Moon2 = "Commencement";
		private const string Section_Stage_Meridian = "Prime Meridian";
		private const string Section_Stage_Goldshores = "Gilded Coast";
		private const string Section_Stage_MS = "A Moment, Fractured";
		private const string Section_Stage_Limbo = "A Moment, Whole";

		private const string Section_Stage_Arena = "Void Fields";

		private const string Section_Loop_Definition = "!Loop Definition";
		private const string Section_Loop_Effects = "Loop Effects";
		private const string Section_Loop_Teleporter = "Teleporter";

		private const string Section_Enable = "!Enable Changes";
		private const string Label_Enable = "!Enable Changes";
		private const string Desc_Enable = "Allows this section to function.";
		private const string Desc_Enable_Config = "Allows all changes within this config to happen.";

		private const string Label_Eclipse = "Allow On Eclipse";
		private const string Desc_Eclipse = "Allow changes made in this section even during Eclipse.";

		private const string Label_WinPortal = "Completion Portal";
		private const string Label_WinReward = "Completion Reward";

		public static void Setup()
        {
			LoopConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_Looping.cfg"), true);
			StageConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Railroad_Stages.cfg"), true);
			Read_StageConfig();
			Read_LoopConfig();
		}
		private static void Read_StageConfig()
        {
			Changes.Looping.Enable = LoopConfig.Bind(Section_Enable, Label_Enable, false, "Allows all changes within this config to happen. Note that everything here affects this mod and has no actual effect on what the game considers as a loop.").Value;

			//Changes.Looping.Loop_MinStageCount = LoopConfig.Bind(Section_Loop_Definition, "Min Stage Clears", 5, "The minimun number of stage clears to even consider that we've looped.").Value;
			Changes.Looping.Loop_DejaVu = LoopConfig.Bind(Section_Loop_Definition, "Deja Vu Requirement", false, "Counts as looping once you've cleared the minimum amount of stages and are on a stage 1. (The same requirements for the Deja Vu achievement.)").Value;

			Changes.Looping.Loop_ArtifactRaw = LoopConfig.Bind(Section_Loop_Effects, "Loop Artifacts", "EliteOnly, WeakAssKnees", "Enables the specified Artifact by their internal name upon looping.").Value;

			Changes.Looping.Loop_LoopTeleporter = LoopConfig.Bind(Section_Loop_Teleporter, "Loop Primordial Teleporter", false, "Replaces the regular Teleporter with the Primordial Teleporter while considered looping.").Value;
			Changes.Looping.Loop_OrderTeleporter = LoopConfig.Bind(Section_Loop_Teleporter, "Primordial Stage Order", 0, "Prevents the Primordial Teleporter from spawning if the Stage's order number is less than this. ('Loop Primordial Teleporter' has priority over this) (Set to 0 for vanilla behavior)").Value;
		}
		private static void Read_LoopConfig()
        {
			Changes.Stages.Enable = StageConfig.Bind(Section_Enable, Label_Enable, false, Desc_Enable_Config).Value;

			Changes.Stages.Moon2_Eclipse = StageConfig.Bind(Section_Stage_Moon2, Label_Eclipse, false, Desc_Eclipse).Value;
			Changes.Stages.Moon2_Portal = StageConfig.Bind(Section_Stage_Moon2, Label_WinPortal, ConfigPortalType.NoPortal, "Portal to spawn upon defeating Mithrix.").Value;
			Changes.Stages.Moon2_Reward = StageConfig.Bind(Section_Stage_Moon2, Label_WinReward, false, "Drop Legendary items upon defeating Mithrix.").Value;

			Changes.Stages.Meridian_Eclipse = StageConfig.Bind(Section_Stage_Meridian, Label_Eclipse, false, Desc_Eclipse).Value;
			Changes.Stages.Meridian_Portal = StageConfig.Bind(Section_Stage_Meridian, Label_WinPortal, ConfigPortalType.Destination, "Portal to spawn upon defeating False Son.").Value;
			Changes.Stages.Meridian_Reward = StageConfig.Bind(Section_Stage_Meridian, Label_WinReward, true, "Drop Aurelionite Blessings upon defeating False Son.").Value;
			Changes.Stages.Meridian_AllowRebirth = StageConfig.Bind(Section_Stage_Meridian, "Allow Rebirth", true, "Allows the Rebirth Shrine to spawn.").Value;
			Changes.Stages.Meridian_ACPortal = StageConfig.Bind(Section_Stage_Meridian, "Virtual Portal", true, "Allows the extra Virtual Portal to spawn.").Value;

			Changes.Stages.VoidRaid_Eclipse = StageConfig.Bind(Section_Stage_VoidRaid, Label_Eclipse, false, Desc_Eclipse).Value;
			Changes.Stages.VoidRaid_Portal = StageConfig.Bind(Section_Stage_VoidRaid, Label_WinPortal, ConfigPortalType.NoPortal, "Portal to spawn upon defeating Voidling.").Value;
			Changes.Stages.VoidRaid_Reward = StageConfig.Bind(Section_Stage_VoidRaid, Label_WinReward, false, "Drop Void Potentials that contain Legendary items upon defeating Voidling.").Value;
			Changes.Stages.VoidRaid_VoidOutroPortal = StageConfig.Bind(Section_Stage_VoidRaid, "Void Outro Portal", true, "Allows the Void Outro Portal to spawn upon defeating Voidling.").Value;
			Changes.Stages.VoidRaid_TimeFlows = StageConfig.Bind(Section_Stage_VoidRaid, "Time Flow", true, "Allows time to flow normally in the Planetarium.").Value;

			Changes.Stages.GoldShores_MeridianPortal = StageConfig.Bind(Section_Stage_Goldshores, "Colossus Portal", ConfigGoldPortal.Vanilla, "Controls how the Colossus Portal spawns on Gilded Coast. (Meridian = Always spawns and takes you to Prime Meridian)").Value;

			Changes.Stages.MS_NeedBeads = StageConfig.Bind(Section_Stage_MS, "Beads Required", true, "Beads of Fealty are required to go to A Moment, Whole.").Value;

			Changes.Stages.Limbo_Eclipse = StageConfig.Bind(Section_Stage_Limbo, Label_Eclipse, false, Desc_Eclipse).Value;
			Changes.Stages.Limbo_Portal = StageConfig.Bind(Section_Stage_Limbo, Label_WinPortal, ConfigPortalType.NoPortal, "Portal to spawn upon defeating the Twisted Scavenger.").Value;
			Changes.Stages.Limbo_Reward = StageConfig.Bind(Section_Stage_Limbo, Label_WinReward, false, "Drop Legendary items upon defeating the Twisted Scavenger.").Value;

			Changes.Stages.Arena_VoidPortal = StageConfig.Bind(Section_Stage_Arena, "Void Portal", true, "Allows the Void Portal to spawn upon completing the Void Fields.").Value;
			Changes.Stages.Arena_TimeFlows = StageConfig.Bind(Section_Stage_Arena, "Time Flow", true, "Allows time to flow normally in the Void Fields.").Value;
		}
	}
}
