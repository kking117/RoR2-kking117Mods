using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using BepInEx.Bootstrap;
using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace FlatItemBuff
{
	[BepInDependency("com.bepis.r2api.content_management", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.damagetype", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.dot", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.items", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.FlatItemBuff";
		public const string MODNAME = "FlatItemBuff";
		public const string MODTOKEN = "KKING117_FLATITEMBUFF_";
		public const string MODVERSION = "1.15.2";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		public static PluginInfo pluginInfo;
		private void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			Configs.Setup();
			EnableChanges();
			SharedHooks.Setup();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
			ModLogger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		private void EnableChanges()
        {
			//Common
			new Items.BisonSteak();
			new Items.TopazBrooch();
			//Uncommon
			new Items.WaxQuail();
			new Items.LeptonDaisy();
			new Items.Infusion_Rework();
			new Items.LeechingSeed_Rework();
			new Items.Stealthkit();
			new Items.HuntersHarpoon();
			new Items.SquidPolyp();
			new Items.WarHorn();
			//Legendary
			new Items.BensRaincoat();
			new Items.LaserScope();
			new Items.Aegis();
			//Boss
			new Items.Planula_Rework();
			new Items.TitanicKnurl_Rework();
			new Items.DefenseNucleus_Rework();
			//Void
			new Items.LigmaLenses();
			new Items.VoidsentFlame();
			//Artifacts
			new Artifacts.Spite();
		}
		private void PostLoad()
        {
			Items.DefenseNucleus_Shared.ExtraChanges();
		}
	}
}