using System;
using BepInEx;
using RoR2;
using R2API.Utils;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Railroad
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.Railroad";
		public const string MODNAME = "Railroad";
		public const string MODVERSION = "1.0.3";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		internal static PluginInfo pluginInfo;
		
		public void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			Configs.Setup();
			EnableChanges();
		}
		private void EnableChanges()
		{
			new Changes.Looping();
			new Changes.Stages();
		}
	}
}
