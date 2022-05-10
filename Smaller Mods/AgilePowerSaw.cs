using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace AgilePowerSaw
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.AgilePowerSaw";
		public const string MODNAME = "AgilePowerSaw";
		public const string MODVERSION = "1.2.1";

		public static ConfigEntry<bool> PowerSaw;
		public static ConfigEntry<bool> BounceGrenade;
		public static ConfigEntry<bool> Flamethrower;
		public static ConfigEntry<bool> NanoBomb;
		public static ConfigEntry<bool> NanoSpear;
		public static ConfigEntry<bool> LaserGlaive;
		public static ConfigEntry<bool> ChargedGauntlet;
		public static ConfigEntry<bool> Flood;

		private bool AncientScepter = false;
		public void Awake()
		{
			ReadConfig();
			AncientScepter = Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
			On.RoR2.Skills.SkillCatalog.Init += SkillCatalog_Init;
		}
		public void SkillCatalog_Init(On.RoR2.Skills.SkillCatalog.orig_Init orig)
        {
			orig();
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
			if (ChargedGauntlet.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/ChargeFist.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
				if (AncientScepter)
				{
					skillDef = GetSkillDefByNameToken("ANCIENTSCEPTER_LOADER_CHARGEFISTNAME");
					if (skillDef)
					{
						skillDef.canceledFromSprinting = false;
						skillDef.cancelSprintingOnActivation = false;
					}
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
			if(LaserGlaive.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyGlaive.asset").WaitForCompletion();
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
				if (AncientScepter)
				{
					skillDef = GetSkillDefByNameToken("ANCIENTSCEPTER_MAGE_FLAMETHROWERNAME");
					if (skillDef)
					{
						skillDef.canceledFromSprinting = false;
						skillDef.cancelSprintingOnActivation = false;
					}
				}
			}
			if(Flood.Value)
            {
				SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/ChargeMegaBlaster.asset").WaitForCompletion();
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
		}
		private SkillDef GetSkillDefByNameToken(string token)
        {
			List<SkillDef> skillDefs = SkillCatalog.allSkillDefs.ToList();
			for (int i = 0; i < skillDefs.Count; i++)
            {
				if (skillDefs[i].skillNameToken == token)
                {
					return skillDefs[i];
                }
            }
			return null;
        }
		public void ReadConfig()
		{
			LaserGlaive = Config.Bind<bool>(new ConfigDefinition("Huntress", "Laser Glaive"), false, new ConfigDescription("Should Huntress's Laser Glaive skill be agile?", null, Array.Empty<object>()));
			PowerSaw = Config.Bind<bool>(new ConfigDefinition("Mul-T", "Power-Saw"), true, new ConfigDescription("Should Mul-T's Power-Saw skill be agile?", null, Array.Empty<object>()));
			BounceGrenade = Config.Bind<bool>(new ConfigDefinition("Engineer", "Bouncing Grenade"), false, new ConfigDescription("Should Engineer's Bouncing Grenade skill be agile?", null, Array.Empty<object>()));
			ChargedGauntlet = Config.Bind<bool>(new ConfigDefinition("Loader", "Charged Gauntlet"), false, new ConfigDescription("Should Loader's Charged Gauntlet skill be agile?", null, Array.Empty<object>()));
			NanoBomb = Config.Bind<bool>(new ConfigDefinition("Artificer", "Charged Nano-Bomb"), false, new ConfigDescription("Should Artificer's Charged Nano-Bomb skill be agile?", null, Array.Empty<object>()));
			NanoSpear = Config.Bind<bool>(new ConfigDefinition("Artificer", "Cast Nano-Spear"), false, new ConfigDescription("Should Artificer's Cast Nano-Spear skill be agile?", null, Array.Empty<object>()));
			Flamethrower = Config.Bind<bool>(new ConfigDefinition("Artificer", "Flamethrower"), false, new ConfigDescription("Should Artificer's Flamethrower skill be agile?", null, Array.Empty<object>()));
			Flood = Config.Bind<bool>(new ConfigDefinition("Void Fiend", "Flood"), false, new ConfigDescription("Should Void Fiend's Flood skill be agile?", null, Array.Empty<object>()));
		}
	}
}
