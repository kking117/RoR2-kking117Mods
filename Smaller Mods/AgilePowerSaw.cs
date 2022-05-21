using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2.Skills;
using UnityEngine.AddressableAssets;
using R2API.Utils;

namespace AgilePowerSaw
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.AgilePowerSaw";
		public const string MODNAME = "AgilePowerSaw";
		public const string MODVERSION = "1.3.0";

		public static ConfigEntry<bool> FragGrenade;

		public static ConfigEntry<bool> LaserGlaive;

		public static ConfigEntry<bool> PowerSaw;

		public static ConfigEntry<bool> BounceGrenade;
		public static ConfigEntry<bool> PressureMine;
		public static ConfigEntry<bool> SpiderMine;
		public static ConfigEntry<bool> GaussTurret;
		public static ConfigEntry<bool> CarbonTurret;

		public static ConfigEntry<bool> Flamethrower;
		public static ConfigEntry<bool> NanoBomb;
		public static ConfigEntry<bool> NanoSpear;

		public static ConfigEntry<bool> Whirlwind;
		public static ConfigEntry<bool> RisingThunder;
		public static ConfigEntry<bool> SlicingWinds;

		public static ConfigEntry<bool> Disperse;
		public static ConfigEntry<bool> BrambleVolley;
		public static ConfigEntry<bool> Harvest;
		public static ConfigEntry<bool> Tangling;

		public static ConfigEntry<bool> GrappleFist;
		public static ConfigEntry<bool> ChargedGauntlet;
		public static ConfigEntry<bool> ThunderGauntlet;
		public static ConfigEntry<bool> SpikedFist;
		public static ConfigEntry<bool> ThrowPylon;
		public static ConfigEntry<bool> Thunderslam;

		public static ConfigEntry<bool> CrocoBite;

		public static ConfigEntry<bool> VulcanShotgun;
		public static ConfigEntry<bool> PowerTazer;

		public static ConfigEntry<bool> Supercharge;
		public static ConfigEntry<bool> Supershot;
		public static ConfigEntry<bool> Cryocharge;
		public static ConfigEntry<bool> Cryoshot;

		public static ConfigEntry<bool> Flood;

		public static ConfigEntry<string> Other_CustomList;
		public static ConfigEntry<bool> Other_PrintHelp;
		public void Awake()
		{
			ReadConfig();
			On.RoR2.Skills.SkillCatalog.Init += SkillCatalog_Init;
		}
		public void SkillCatalog_Init(On.RoR2.Skills.SkillCatalog.orig_Init orig)
        {
			orig();
			if(FragGrenade.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/ThrowGrenade.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (LaserGlaive.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyGlaive.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (PowerSaw.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodyFireBuzzsaw.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (BounceGrenade.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyFireGrenade.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (PressureMine.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceMine.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (SpiderMine.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceSpiderMine.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (GaussTurret.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceTurret.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (CarbonTurret.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceWalkerTurret.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (NanoBomb.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyNovaBomb.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (NanoSpear.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyIceBomb.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Flamethrower.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlamethrower.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (GrappleFist.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/FireHook.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (SpikedFist.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/FireYankHook.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (ChargedGauntlet.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/ChargeFist.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (ThunderGauntlet.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/ChargeZapFist.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (ThrowPylon.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/ThrowPylon.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Thunderslam.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/GroundSlam.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Whirlwind.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyWhirlwind.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (RisingThunder.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyUppercut.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (SlicingWinds.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyEvisProjectile.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}

			if (Disperse.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodySonicBoom.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (BrambleVolley.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyPlantSonicBoom.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Tangling.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyFireFlower2.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Harvest.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyFireFruitSeed.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if(CrocoBite.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoBite.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (VulcanShotgun.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CaptainShotgun.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (PowerTazer.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CaptainTazer.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Supercharge.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyChargeSnipeSuper.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Supershot.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyFireSnipeSuper.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Cryocharge.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyChargeSnipeCryo.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Cryoshot.Value)
			{
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyFireSnipeCryo.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Flood.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/ChargeMegaBlaster.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}

			string[] skills = MainPlugin.Other_CustomList.Value.Split(',');
			for (int i = 0; i < skills.Length; i++)
			{
				skills[i] = skills[i].Trim();
				ForceAgileByNameToken(skills[i]);
			}
			if (Other_PrintHelp.Value)
			{
				PrintSkillNames();
			}
		}
		private void PrintSkillNames()
		{
			Logger.LogInfo("===SKILL NAME LIST===");
			List<SkillDef> skillDefs = SkillCatalog.allSkillDefs.ToList();
			for (int i = 0; i < skillDefs.Count; i++)
			{
				if (skillDefs[i].skillNameToken.Length > 0)
				{
					Logger.LogInfo(skillDefs[i].skillNameToken);
				}
			}
			Logger.LogInfo("===END OF LIST===");
		}
		private void ForceAgileByNameToken(string token)
		{
			List<SkillDef> skillDefs = SkillCatalog.allSkillDefs.ToList();
			for (int i = 0; i < skillDefs.Count; i++)
			{
				if (skillDefs[i].skillNameToken == token)
				{
					skillDefs[i].canceledFromSprinting = false;
					skillDefs[i].cancelSprintingOnActivation = false;
				}
			}
		}
		public void ReadConfig()
		{
			FragGrenade = Config.Bind<bool>(new ConfigDefinition("Commando", "Frag Grenade"), false, new ConfigDescription("Should Commando's Frag Grenade skill be agile?", null, Array.Empty<object>()));

			LaserGlaive = Config.Bind<bool>(new ConfigDefinition("Huntress", "Laser Glaive"), false, new ConfigDescription("Should Huntress's Laser Glaive skill be agile?", null, Array.Empty<object>()));

			PowerSaw = Config.Bind<bool>(new ConfigDefinition("Mul-T", "Power-Saw"), true, new ConfigDescription("Should Mul-T's Power-Saw skill be agile?", null, Array.Empty<object>()));

			BounceGrenade = Config.Bind<bool>(new ConfigDefinition("Engineer", "Bouncing Grenade"), false, new ConfigDescription("Should Engineer's Bouncing Grenade skill be agile?", null, Array.Empty<object>()));
			PressureMine = Config.Bind<bool>(new ConfigDefinition("Engineer", "Pressure Mine"), false, new ConfigDescription("Should Engineer's Pressure Mine skill be agile?", null, Array.Empty<object>()));
			SpiderMine = Config.Bind<bool>(new ConfigDefinition("Engineer", "Spider Mine"), false, new ConfigDescription("Should Engineer's Spider Mine skill be agile?", null, Array.Empty<object>()));
			GaussTurret = Config.Bind<bool>(new ConfigDefinition("Engineer", "TR12 Gauss Auto-Turret"), false, new ConfigDescription("Should Engineer's TR12 Gauss Auto-Turret skill be agile?", null, Array.Empty<object>()));
			CarbonTurret = Config.Bind<bool>(new ConfigDefinition("Engineer", "TR58 Carbonizer Turret"), false, new ConfigDescription("Should Engineer's TR58 Carbonizer Turret skill be agile?", null, Array.Empty<object>()));

			GrappleFist = Config.Bind<bool>(new ConfigDefinition("Loader", "Grapple Fist"), false, new ConfigDescription("Should Loader's Grapple Fist skill be agile?", null, Array.Empty<object>()));
			SpikedFist = Config.Bind<bool>(new ConfigDefinition("Loader", "Spiked Fist"), false, new ConfigDescription("Should Loader's Spiked Fist skill be agile?", null, Array.Empty<object>()));
			ChargedGauntlet = Config.Bind<bool>(new ConfigDefinition("Loader", "Charged Gauntlet"), false, new ConfigDescription("Should Loader's Charged Gauntlet skill be agile?", null, Array.Empty<object>()));
			ThunderGauntlet = Config.Bind<bool>(new ConfigDefinition("Loader", "Thunder Gauntlet"), false, new ConfigDescription("Should Loader's Thunder Gauntlet skill be agile?", null, Array.Empty<object>()));
			ThrowPylon = Config.Bind<bool>(new ConfigDefinition("Loader", "M551 Pylon"), false, new ConfigDescription("Should Loader's M551 Pylon skill be agile?", null, Array.Empty<object>()));
			Thunderslam = Config.Bind<bool>(new ConfigDefinition("Loader", "Thunderslam"), false, new ConfigDescription("Should Loader's Thunderslam skill be agile?", null, Array.Empty<object>()));

			Whirlwind = Config.Bind<bool>(new ConfigDefinition("Mercenary", "Whirlwind"), false, new ConfigDescription("Should Mercenary's Whirlwind skill be agile?", null, Array.Empty<object>()));
			RisingThunder = Config.Bind<bool>(new ConfigDefinition("Mercenary", "Rising Thunder"), false, new ConfigDescription("Should Mercenary's Rising Thunder skill be agile?", null, Array.Empty<object>()));
			SlicingWinds = Config.Bind<bool>(new ConfigDefinition("Mercenary", "Slicing Winds"), false, new ConfigDescription("Should Mercenary's Slicing Winds skill be agile?", null, Array.Empty<object>()));

			Disperse = Config.Bind<bool>(new ConfigDefinition("REX", "DIRECTIVE: Disperse"), false, new ConfigDescription("Should REX's DIRECTIVE: Disperse skill be agile?", null, Array.Empty<object>()));
			BrambleVolley = Config.Bind<bool>(new ConfigDefinition("REX", "Bramble Volley"), false, new ConfigDescription("Should REX's Bramble Volley skill be agile?", null, Array.Empty<object>()));
			Tangling = Config.Bind<bool>(new ConfigDefinition("REX", "Tangling Growth"), false, new ConfigDescription("Should REX's Tangling Growth skill be agile?", null, Array.Empty<object>()));
			Harvest = Config.Bind<bool>(new ConfigDefinition("REX", "DIRECTIVE: Harvest"), false, new ConfigDescription("Should REX's DIRECTIVE: Harvest skill be agile?", null, Array.Empty<object>()));

			NanoBomb = Config.Bind<bool>(new ConfigDefinition("Artificer", "Charged Nano-Bomb"), false, new ConfigDescription("Should Artificer's Charged Nano-Bomb skill be agile?", null, Array.Empty<object>()));
			NanoSpear = Config.Bind<bool>(new ConfigDefinition("Artificer", "Cast Nano-Spear"), false, new ConfigDescription("Should Artificer's Cast Nano-Spear skill be agile?", null, Array.Empty<object>()));
			Flamethrower = Config.Bind<bool>(new ConfigDefinition("Artificer", "Flamethrower"), false, new ConfigDescription("Should Artificer's Flamethrower skill be agile?", null, Array.Empty<object>()));

			CrocoBite = Config.Bind<bool>(new ConfigDefinition("Acrid", "Ravenous Bite"), false, new ConfigDescription("Should Acrid's Ravenous Bite skill be agile?", null, Array.Empty<object>()));

			VulcanShotgun = Config.Bind<bool>(new ConfigDefinition("Captain", "Vulcan Shotgun"), false, new ConfigDescription("Should Captain's Vulcan Shotgun skill be agile?", null, Array.Empty<object>()));
			PowerTazer = Config.Bind<bool>(new ConfigDefinition("Captain", "Power Tazer"), false, new ConfigDescription("Should Captain's Power Tazer skill be agile?", null, Array.Empty<object>()));

			Supercharge = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Supercharge"), false, new ConfigDescription("Should Railgunner's Supercharge skill be agile?", null, Array.Empty<object>()));
			Supershot = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Superchared Railgun"), false, new ConfigDescription("Should Railgunner's Supercharged Railgun skill be agile?", null, Array.Empty<object>()));
			Cryocharge = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Cryocharge"), false, new ConfigDescription("Should Railgunner's Cryocharge skill be agile?", null, Array.Empty<object>()));
			Cryoshot = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Cryocharged Railgun"), false, new ConfigDescription("Should Railgunner's Cryocharged Railgun skill be agile?", null, Array.Empty<object>()));

			Flood = Config.Bind<bool>(new ConfigDefinition("Void Fiend", "Flood"), false, new ConfigDescription("Should Void Fiend's Flood skill be agile?", null, Array.Empty<object>()));

			Other_CustomList = Config.Bind<string>(new ConfigDefinition("Other", "Custom List"), "", new ConfigDescription("List of skills that will be made agile on start up. (Uses the skill's nameToken)", null, Array.Empty<object>()));
			Other_PrintHelp = Config.Bind<bool>(new ConfigDefinition("Other", "List Skills"), false, new ConfigDescription("Lists all skill names when loading up the game, for Custom List config use.", null, Array.Empty<object>()));
		}
	}
}
