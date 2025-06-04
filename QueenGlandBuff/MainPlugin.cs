using System;
using BepInEx;
using BepInEx.Bootstrap;
using RoR2;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace QueenGlandBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.elites", BepInDependency.DependencyFlags.HardDependency)]
	//[BepInDependency("com.RiskyLives.RiskyMod", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.QueenGlandBuff";
		public const string MODNAME = "QueenGlandBuff";
		public const string MODVERSION = "1.5.5";

		public const string MODTOKEN = "KKING117_QUEENGLANDBUFF_";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		public static PluginInfo pluginInfo;

		internal static SceneDef BazaarSceneDef;
		internal static bool RiskyMod_Loaded = false;
		private void Awake()
		{
			ModLogger = Logger;
			pluginInfo = Info;
			
			Configs.Setup();
			Changes.BeetleGuardAlly.Begin();
			if (Changes.QueensGland.Enable)
			{
				Changes.QueensGland.Begin();
			}
			Logger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
		}
		//RiskyMod already supports this mod so we don't need this.
		/*private void LoadCompat()
        {
			RiskyMod_Loaded = Chainloader.PluginInfos.ContainsKey("com.RiskyLives.RiskyMod");
		}*/
		private void PostLoad()
		{
			//LoadCompat();
			if (Changes.QueensGland.Enable)
			{
				Changes.QueensGland.PostLoad();
			}
			BazaarSceneDef = SceneCatalog.FindSceneDef("bazaar");
		}
	}
}
