using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using RoR2;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HalcyonSeedBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.HalcyonSeedBuff";
		public const string MODNAME = "HalcyonSeedBuff";
		public const string MODTOKEN = "KKING117_HALCYONSEEDBUFF_";
		public const string MODVERSION = "1.1.2";

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static int Halcyon_ItemMult = 2;
		public static bool Halcyon_CanBeStolen = true;
		public static bool Halcyon_ChannelTeamFix = false;

		public static int ChannelOn_MoonPhase = 2;
		public static int ChannelOn_MeridianPhase = 3;
		public static bool ChannelOn_Focus = true;
		public static bool ChannelOn_VoidRaid = true;

		public static bool FalseSon_PlayerLoyal = true;
		public static bool FalseSon_BossLoyal = true;
		public static string FalseSon_BodyList = "FalseSonBody, FalseSonBossBody, FalseSonBossBodyLunarShard, FalseSonBossBodyBrokenLunarShard";
		public void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();
			Changes.HalcyonSeed.EnableChanges();
		}
		public void ReadConfig()
		{
			Halcyon_ItemMult = Config.Bind("Halcyon Seed", "Effect Multiplier", 2, "Multiplies the Halcyon Seed count by this much when calculating Aurelionite's health and damage.").Value;
			Halcyon_CanBeStolen = Config.Bind("Halcyon Seed", "Can Be Stolen", true, "Allows Mithrix to steal and channel Halcyon Seeds. (Set Spawn Against Mithrix to 0-3 for this to work.)").Value;

			ChannelOn_Focus = Config.Bind("Channel", "Spawn On Focus", true, "Attempt to channel Aurelionite when activating the Focus.").Value;
			ChannelOn_VoidRaid = Config.Bind("Channel", "Spawn On Void Raid", true, "Attempt to channel Aurelionite at the start of each Voidling phase.").Value;
			ChannelOn_MoonPhase = Config.Bind("Channel", "Spawn Against Mithrix", 2, "Attempt to channel Aurelionite on this phase during the Mithrix fight. (0-4) (0 = Don't spawn, 4 = Vanilla)").Value;
			ChannelOn_MeridianPhase = Config.Bind("Channel", "Spawn Against False Son", 3, "Attempt to channel Aurelionite on this phase during the False Son fight. (0-3) (0 = Don't spawn, 3 = Vanilla)").Value;
			
			FalseSon_PlayerLoyal = Config.Bind("False Son", "Loyal to Playable False Son", true, "If Aurelionite is channeled and a False Son Player exists then Aurelionite will be forced onto their team instead. (Has priority over 'Loyal to NPC False Son'.)").Value;
			FalseSon_BossLoyal = Config.Bind("False Son", "Loyal to NPC False Son", true, "If Aurelionite is channeled and a False Son NPC exists then Aurelionite will be forced onto their team instead.").Value;
			FalseSon_BodyList = Config.Bind("False Son", "Body List", "FalseSonBody, FalseSonBossBody, FalseSonBossBodyLunarShard, FalseSonBossBodyBrokenLunarShard", "List of bodies that count as False Son.").Value;
		}
	}
}