using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using R2API.Utils;
using RoR2;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HalcyonSeedBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI"
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.HalcyonSeedBuff";
		public const string MODNAME = "HalcyonSeedBuff";
		public const string MODTOKEN = "KKING117_HALCYONSEEDBUFF_";
		public const string MODVERSION = "1.0.3";

		internal static bool GoldenCoastPlus = false;

		internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<bool> Config_Halcyon_Enable;
		public static ConfigEntry<int> Config_Halcyon_ItemMult;
		public static ConfigEntry<bool> Config_Halcyon_ChannelOnFocus;
		public static ConfigEntry<bool> Config_Halcyon_ChannelTeamFix;
		public static ConfigEntry<int> Config_Halcyon_ChannelOnMoonPhase;
		public static ConfigEntry<bool> Config_Halcyon_ChannelOnVoidRaid;
		public static ConfigEntry<bool> Config_Halcyon_CanBeStolen;
		public static ConfigEntry<float> Config_Halcyon_HealthMult;
		public static ConfigEntry<float> Config_Halcyon_DamageMult;
		public void Awake()
		{
			ModLogger = this.Logger;
			ReadConfig();
			if(Config_Halcyon_Enable.Value)
            {
				Changes.HalcyonSeed.EnableChanges();
            }
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
		}
		public void PostLoad()
		{
			GoldenCoastPlus = Chainloader.PluginInfos.ContainsKey("com.Skell.GoldenCoastPlus");
			Changes.HalcyonSeed.PostLoadHooks();
		}
		public void ReadConfig()
		{
			Config_Halcyon_Enable = Config.Bind<bool>(new ConfigDefinition("Halcyon Seed", "Enable Changes"), true, new ConfigDescription("Enables changes to Halcyon Seed.", null, Array.Empty<object>()));
			Config_Halcyon_ItemMult = Config.Bind<int>(new ConfigDefinition("Halcyon Seed", "Effect Multiplier"), 2, new ConfigDescription("Multiplies the Halcyon Seed count by this much when calculating Aurelionite's health and damage. (Ignored if running GoldenCoastPlus)", null, Array.Empty<object>()));
			Config_Halcyon_ChannelTeamFix = Config.Bind<bool>(new ConfigDefinition("Halcyon Seed", "Team Selection Fix"), true, new ConfigDescription("Fixes the Halcyon Seed so that the Aurelionite spawns on the team with the highest Halcyon Seed count. (Vanilla makes it always summon on the Player's team but uses the team with the highest amount to determine its power.)", null, Array.Empty<object>()));
			Config_Halcyon_ChannelOnFocus = Config.Bind<bool>(new ConfigDefinition("Halcyon Seed", "Spawn On Focus"), true, new ConfigDescription("Attempt to channel Aurelionite when activating the Focus.", null, Array.Empty<object>()));
			Config_Halcyon_ChannelOnVoidRaid = Config.Bind<bool>(new ConfigDefinition("Halcyon Seed", "Spawn On Void Raid"), true, new ConfigDescription("Attempt to channel Aurelionite at the start of each Voidling phase.", null, Array.Empty<object>()));
			Config_Halcyon_ChannelOnMoonPhase = Config.Bind<int>(new ConfigDefinition("Halcyon Seed", "Spawn Against Mithrix"), 2, new ConfigDescription("Attempt to channel Aurelionite on this phase during the Mithrix Moon fight. (0-4) (0 = Don't spawn, 4 = Do the same as usual)", null, Array.Empty<object>()));
			Config_Halcyon_CanBeStolen = Config.Bind<bool>(new ConfigDefinition("Halcyon Seed", "Can Be Stolen"), true, new ConfigDescription("Allows Mithrix to steal and channel Halcyon Seeds. (Requires Team Selection Fix for it to actually spawn on his team.)(Set Spawn Against Mithrix to 0-3 for this to work.)", null, Array.Empty<object>()));
		}
	}
}