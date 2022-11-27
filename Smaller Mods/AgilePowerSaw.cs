using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using RoR2.Skills;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace AgilePowerSaw
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.AgilePowerSaw";
		public const string MODNAME = "AgilePowerSaw";
		public const string MODVERSION = "1.4.0";

		internal static bool RiskOfOptions = false;

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
		public static ConfigEntry<string> Other_ReferenceList;

		private static List<SkillDef> CulledSkillList;
		private static List<string> CustomTokenList;
		public void Awake()
		{
			RiskOfOptions = Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
			ReadConfig();
			if (RiskOfOptions)
			{
				SetupOptions();
			}
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
		}
		public void PostLoad()
		{
			ApplyModChanges();
		}
		private void ApplyModChanges()
		{
			if (FragGrenade.Value)
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
			if (CrocoBite.Value)
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

			UpdateTokenReference();
			ApplyCustomAgileList();
		}
		private void UpdateTokenReference()
		{
			string allthenames = "";
			bool first = true;
			CulledSkillList = new List<SkillDef>();
			List<SkillDef> skillDefs = SkillCatalog.allSkillDefs.ToList();
			for (int i = 0; i < skillDefs.Count; i++)
			{
				if (skillDefs[i].skillNameToken.Length > 0)
				{
					if (first)
                    {
						allthenames = skillDefs[i].skillNameToken;
						first = false;
					}
					else
                    {
						allthenames += ", " + skillDefs[i].skillNameToken;
					}
					CulledSkillList.Add(skillDefs[i]);
				}
			}
			Other_ReferenceList.Value = allthenames;


			CustomTokenList = new List<string>();
			string[] skills = MainPlugin.Other_CustomList.Value.Split(',');
			for (int i = 0; i < skills.Length; i++)
			{
				skills[i] = skills[i].Trim();
				if (skills[i].Length > 0)
                {
					CustomTokenList.Add(skills[i]);
				}
			}
		}
		private void ApplyCustomAgileList()
        {
			Logger.LogInfo("Total Custom List Entries: " + CustomTokenList.Count);
			for (int i = 0; i < CulledSkillList.Count; i++)
			{
				for (int z = 0; z < CustomTokenList.Count; z++)
				{
					if(CulledSkillList[i].skillNameToken == CustomTokenList[z])
                    {
						Logger.LogInfo("Found and tried enabling Agile for: " + CustomTokenList[z]);
						CulledSkillList[i].canceledFromSprinting = false;
						CulledSkillList[i].cancelSprintingOnActivation = false;
						break;
					}
				}
			}
		}
		public void ReadConfig()
		{
			FragGrenade = Config.Bind<bool>(new ConfigDefinition("Commando", "Frag Grenade"), false, new ConfigDescription("Try to make Commando's Frag Grenade skill agile?", null, Array.Empty<object>()));

			LaserGlaive = Config.Bind<bool>(new ConfigDefinition("Huntress", "Laser Glaive"), false, new ConfigDescription("Try to make Huntress's Laser Glaive skill agile?", null, Array.Empty<object>()));

			PowerSaw = Config.Bind<bool>(new ConfigDefinition("Mul-T", "Power-Saw"), true, new ConfigDescription("Try to make Mul-T's Power-Saw skill agile?", null, Array.Empty<object>()));

			BounceGrenade = Config.Bind<bool>(new ConfigDefinition("Engineer", "Bouncing Grenade"), false, new ConfigDescription("Try to make Engineer's Bouncing Grenade skill agile?", null, Array.Empty<object>()));
			PressureMine = Config.Bind<bool>(new ConfigDefinition("Engineer", "Pressure Mine"), false, new ConfigDescription("Try to make Engineer's Pressure Mine skill agile?", null, Array.Empty<object>()));
			SpiderMine = Config.Bind<bool>(new ConfigDefinition("Engineer", "Spider Mine"), false, new ConfigDescription("Try to make Engineer's Spider Mine skill agile?", null, Array.Empty<object>()));
			GaussTurret = Config.Bind<bool>(new ConfigDefinition("Engineer", "TR12 Gauss Auto-Turret"), false, new ConfigDescription("Try to make Engineer's TR12 Gauss Auto-Turret skill agile?", null, Array.Empty<object>()));
			CarbonTurret = Config.Bind<bool>(new ConfigDefinition("Engineer", "TR58 Carbonizer Turret"), false, new ConfigDescription("Try to make Engineer's TR58 Carbonizer Turret skill agile?", null, Array.Empty<object>()));

			GrappleFist = Config.Bind<bool>(new ConfigDefinition("Loader", "Grapple Fist"), false, new ConfigDescription("Try to make Loader's Grapple Fist skill agile?", null, Array.Empty<object>()));
			SpikedFist = Config.Bind<bool>(new ConfigDefinition("Loader", "Spiked Fist"), false, new ConfigDescription("Try to make Loader's Spiked Fist skill agile?", null, Array.Empty<object>()));
			ChargedGauntlet = Config.Bind<bool>(new ConfigDefinition("Loader", "Charged Gauntlet"), false, new ConfigDescription("Try to make Loader's Charged Gauntlet skill agile?", null, Array.Empty<object>()));
			ThunderGauntlet = Config.Bind<bool>(new ConfigDefinition("Loader", "Thunder Gauntlet"), false, new ConfigDescription("Try to make Loader's Thunder Gauntlet skill agile?", null, Array.Empty<object>()));
			ThrowPylon = Config.Bind<bool>(new ConfigDefinition("Loader", "M551 Pylon"), false, new ConfigDescription("Try to make Loader's M551 Pylon skill agile?", null, Array.Empty<object>()));
			Thunderslam = Config.Bind<bool>(new ConfigDefinition("Loader", "Thunderslam"), false, new ConfigDescription("Try to make Loader's Thunderslam skill agile?", null, Array.Empty<object>()));

			Whirlwind = Config.Bind<bool>(new ConfigDefinition("Mercenary", "Whirlwind"), false, new ConfigDescription("Try to make Mercenary's Whirlwind skill agile?", null, Array.Empty<object>()));
			RisingThunder = Config.Bind<bool>(new ConfigDefinition("Mercenary", "Rising Thunder"), false, new ConfigDescription("Try to make Mercenary's Rising Thunder skill agile?", null, Array.Empty<object>()));
			SlicingWinds = Config.Bind<bool>(new ConfigDefinition("Mercenary", "Slicing Winds"), false, new ConfigDescription("Try to make Mercenary's Slicing Winds skill agile?", null, Array.Empty<object>()));

			Disperse = Config.Bind<bool>(new ConfigDefinition("REX", "DIRECTIVE: Disperse"), false, new ConfigDescription("Try to make REX's DIRECTIVE: Disperse skill agile?", null, Array.Empty<object>()));
			BrambleVolley = Config.Bind<bool>(new ConfigDefinition("REX", "Bramble Volley"), false, new ConfigDescription("Try to make REX's Bramble Volley skill agile?", null, Array.Empty<object>()));
			Tangling = Config.Bind<bool>(new ConfigDefinition("REX", "Tangling Growth"), false, new ConfigDescription("Try to make REX's Tangling Growth skill agile?", null, Array.Empty<object>()));
			Harvest = Config.Bind<bool>(new ConfigDefinition("REX", "DIRECTIVE: Harvest"), false, new ConfigDescription("Try to make REX's DIRECTIVE: Harvest skill agile?", null, Array.Empty<object>()));

			NanoBomb = Config.Bind<bool>(new ConfigDefinition("Artificer", "Charged Nano-Bomb"), false, new ConfigDescription("Try to make Artificer's Charged Nano-Bomb skill agile?", null, Array.Empty<object>()));
			NanoSpear = Config.Bind<bool>(new ConfigDefinition("Artificer", "Cast Nano-Spear"), false, new ConfigDescription("Try to make Artificer's Cast Nano-Spear skill agile?", null, Array.Empty<object>()));
			Flamethrower = Config.Bind<bool>(new ConfigDefinition("Artificer", "Flamethrower"), false, new ConfigDescription("Try to make Artificer's Flamethrower skill agile?", null, Array.Empty<object>()));

			CrocoBite = Config.Bind<bool>(new ConfigDefinition("Acrid", "Ravenous Bite"), false, new ConfigDescription("Try to make Acrid's Ravenous Bite skill agile?", null, Array.Empty<object>()));

			VulcanShotgun = Config.Bind<bool>(new ConfigDefinition("Captain", "Vulcan Shotgun"), false, new ConfigDescription("Try to make Captain's Vulcan Shotgun skill agile?", null, Array.Empty<object>()));
			PowerTazer = Config.Bind<bool>(new ConfigDefinition("Captain", "Power Tazer"), false, new ConfigDescription("Try to make Captain's Power Tazer skill agile?", null, Array.Empty<object>()));

			Supercharge = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Supercharge"), false, new ConfigDescription("Try to make Railgunner's Supercharge skill agile?", null, Array.Empty<object>()));
			Supershot = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Supercharged Railgun"), false, new ConfigDescription("Try to make Railgunner's Supercharged Railgun skill agile?", null, Array.Empty<object>()));
			Cryocharge = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Cryocharge"), false, new ConfigDescription("Try to make Railgunner's Cryocharge skill agile?", null, Array.Empty<object>()));
			Cryoshot = Config.Bind<bool>(new ConfigDefinition("Railgunner", "Cryocharged Railgun"), false, new ConfigDescription("Try to make Railgunner's Cryocharged Railgun skill agile?", null, Array.Empty<object>()));

			Flood = Config.Bind<bool>(new ConfigDefinition("Void Fiend", "Flood"), false, new ConfigDescription("Try to make Void Fiend's Flood skill agile?", null, Array.Empty<object>()));

			Other_CustomList = Config.Bind<string>(new ConfigDefinition("Other", "Custom List"), "", new ConfigDescription("List of skills (by nameToken) to try and make agile. Highly suggest configuring this through a text editor or r2modman, as the config file contains a list of all nameTokens.", null, Array.Empty<object>()));
			Other_ReferenceList = Config.Bind<string>(new ConfigDefinition("Other", "NameToken Reference"), "", new ConfigDescription("Contains a list of all Skill Tokens in use for Custom List configuration. Rebuilt every time the game is launched.", null, Array.Empty<object>()));
		}
		public void SetupOptions()
		{
			ModSettingsManager.SetModDescription("Doktor, turn on my carpal tunnel inhibitors!");

			ModSettingsManager.AddOption(new StringInputFieldOption(Config.Bind("Other",
				"Custom List",
				"")));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Commando",
				"Frag Grenade",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Huntress",
				"Laser Glaive",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Mul-T",
				"Power-Saw",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Engineer",
				"Bouncing Grenade",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Engineer",
				"Pressure Mine",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Engineer",
				"Spider Mine",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Engineer",
				"TR12 Gauss Auto-Turret",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Engineer",
				"TR58 Carbonizer Turret",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Loader",
				"Grapple Fist",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Loader",
				"Spiked Fist",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Loader",
				"Charged Gauntlet",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Loader",
				"Thunder Gauntlet",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Loader",
				"M551 Pylon",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Loader",
				"Thunderslam",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Mercenary",
				"Whirlwind",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Mercenary",
				"Rising Thunder",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Mercenary",
				"Slicing Winds",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("REX",
				"DIRECTIVE: Disperse",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("REX",
				"Bramble Volley",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("REX",
				"Tangling Growth",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("REX",
				"DIRECTIVE: Harvest",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Artificer",
				"Charged Nano-Bomb",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Artificer",
				"Cast Nano-Spear",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Artificer",
				"Flamethrower",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Acrid",
				"Ravenous Bite",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Captain",
				"Vulcan Shotgun",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Captain",
				"Power Tazer",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Railgunner",
				"Supercharge",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Railgunner",
				"Supercharged Railgun",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Railgunner",
				"Cryocharge",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Railgunner",
				"Cryocharged Railgun",
				true)));

			ModSettingsManager.AddOption(new CheckBoxOption(Config.Bind("Void Fiend",
				"Flood",
				true)));
		}
	}
}
