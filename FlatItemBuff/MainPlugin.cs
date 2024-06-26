﻿using System;
using BepInEx;
using RoR2;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace FlatItemBuff
{
	[BepInDependency("com.bepis.r2api.content_management", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.dot", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.deployable", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.orb", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.damagetype", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.FlatItemBuff";
		public const string MODNAME = "FlatItemBuff";
		public const string MODTOKEN = "KKING117_FLATITEMBUFF_";
		public const string MODVERSION = "1.20.4";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		public static PluginInfo pluginInfo;
		private void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			Configs.Setup();
			EnableChanges();
			SharedHooks.Setup();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad_GameModeCatalog));
			ModLogger.LogInfo("Initializing ContentPack.");
			new Modules.ContentPacks().Initialize();
		}
		private void EnableChanges()
        {
			new GeneralChanges();
			//Common
			new Items.BisonSteak_Rework();
			new Items.TopazBrooch();
			new Items.RollOfPennies_Rework();
			//Uncommon
			new Items.Chronobauble();
			new Items.DeathMark_Rework();
			new Items.HuntersHarpoon();
			new Items.IgnitionTank_Rework();
			new Items.Infusion();
			new Items.LeechingSeed_Rework();
			new Items.LeptonDaisy();
			new Items.Stealthkit();
			new Items.SquidPolyp();
			new Items.WarHorn();
			new Items.WaxQuail();
			//Legendary
			new Items.Aegis();
			new Items.BensRaincoat();
			new Items.HappiestMask_Rework();
			new Items.LaserScope();
			new Items.SymbioticScorpion_Rework();
			//Boss
			new Items.DefenseNucleus_Rework();
			new Items.Planula_Rework();
			new Items.TitanicKnurl_Rework();
			//Void
			new Items.LigmaLenses();
			new Items.NewlyHatchedZoea_Rework();
			new Items.VoidsentFlame();
			//Artifacts
			new Artifacts.Spite();
		}
		private void PostLoad_GameModeCatalog()
        {
			Items.DefenseNucleus_Shared.ExtraChanges();
		}
	}
}