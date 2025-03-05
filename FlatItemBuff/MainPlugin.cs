using System;
using System.Reflection;
using BepInEx;
using RoR2;
using BepInEx.Bootstrap;
using System.Security.Permissions;
using UnityEngine;

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
	[BepInDependency("com.Moffein.AssistManager", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.RiskyLives.RiskyMod", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.FlatItemBuff";
		public const string MODNAME = "FlatItemBuff";
		public const string MODTOKEN = "KKING117_FLATITEMBUFF_";
		public const string MODVERSION = "1.24.9";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		public static PluginInfo pluginInfo;
		public static AssetBundle assetBundle;

		internal static bool AssistManager_Loaded = false;
		internal static bool RiskyMod_Loaded = false;
		private void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FlatItemBuff.AssetBundles.flatitembuff_assets"))
			{
				assetBundle = AssetBundle.LoadFromStream(stream);
			}
			Configs.Setup();
			AssistManager_Loaded = Chainloader.PluginInfos.ContainsKey("com.Moffein.AssistManager");
			RiskyMod_Loaded = Chainloader.PluginInfos.ContainsKey("com.RiskyLives.RiskyMod");
			EnableChanges();
			SharedHooks.Setup();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad_GameModeCatalog));
		}
		private void EnableChanges()
        {
			new GeneralChanges();
			//Common
			new Items.ElusiveAntlers_Rework();
			new Items.BisonSteak_Rework();
			new Items.TopazBrooch();
			new Items.RollOfPennies_Rework();
			new Items.WarpedEcho();
			//Uncommon
			new Items.BreachingFin_Rework();
			new Items.Chronobauble();
			new Items.DeathMark();
			new Items.HuntersHarpoon();
			new Items.IgnitionTank_Rework();
			new Items.Infusion();
			new Items.LeechingSeed_Rework();
			new Items.LeptonDaisy();
			new Items.UnstableTransmitter_Rework();
			new Items.RedWhip();
			new Items.RoseBuckler();
			new Items.Stealthkit();
			new Items.SquidPolyp();
			new Items.WarHorn();
			new Items.WaxQuail();
			//Legendary
			new Items.Aegis();
			new Items.BensRaincoat();
			new Items.GrowthNectar();
			new Items.HappiestMask_Rework();
			new Items.LaserScope();
			new Items.PocketICBM();
			new Items.SonorousWhispers_Rework();
			new Items.SymbioticScorpion_Rework();
			//Boss
			new Items.DefenseNucleus_Rework();
			new Items.Planula_Rework();
			new Items.TitanicKnurl_Rework();
			//Void
			new Items.LigmaLenses();
			new Items.NewlyHatchedZoea_Rework();
			new Items.VoidsentFlame();
		}
		private void PostLoad_GameModeCatalog()
        {
			Items.DefenseNucleus_Shared.ExtraChanges();
		}
	}
}