using System;
using BepInEx;
using BepInEx.Bootstrap;
using RoR2;
using R2API.Utils;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Railroad
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[BepInDependency("com.KingEnderBrine.ProperSave", BepInDependency.DependencyFlags.SoftDependency)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.Railroad";
		public const string MODNAME = "Railroad";
		public const string MODVERSION = "1.3.0";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		internal static PluginInfo pluginInfo;

		internal static bool ProperSave_Loaded = false;

		public void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			ProperSave_Loaded = Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ProperSave");
			Configs.Setup();
			EnableChanges();
			SharedHooks.Setup();
		}
		private void EnableChanges()
		{
			new Changes.PortalUtility();
			new Changes.ReqList();
			new Changes.RunFlags();
			new Changes.Looping();
			new Changes.Stages();
			new Changes.Interactables();
			new Changes.Misc();
			//new Changes.InteractablePaths();
			if (ProperSave_Loaded)
			{
				Compat.ProperSaveCompat.Init();
			}
		}
	}
}
