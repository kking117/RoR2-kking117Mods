using System;
using BepInEx;
using RoR2;
using BepInEx.Bootstrap;
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
	[BepInDependency("com.Moffein.AssistManager", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.FlatItemBuff";
		public const string MODNAME = "FlatItemBuff";
		public const string MODTOKEN = "KKING117_FLATITEMBUFF_";
		public const string MODVERSION = "1.22.5";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		public static PluginInfo pluginInfo;

		internal static bool AssistManager_Loaded = false;
		private void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			Configs.Setup();
			AssistManager_Loaded = Chainloader.PluginInfos.ContainsKey("com.Moffein.AssistManager");
			EnableChanges();
			SharedHooks.Setup();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad_GameModeCatalog));
		}
		private void EnableChanges()
        {
			new GeneralChanges();
			//Common
			new Items.AntlerShield_Rework();
			new Items.BisonSteak_Rework();
			new Items.KnockbackFin();
			new Items.TopazBrooch();
			new Items.RollOfPennies_Rework();
			//Uncommon
			new Items.Chronobauble();
			new Items.DeathMark();
			new Items.HuntersHarpoon();
			new Items.IgnitionTank_Rework();
			new Items.Infusion();
			new Items.LeechingSeed_Rework();
			new Items.UnstableTransmitter_Rework();
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