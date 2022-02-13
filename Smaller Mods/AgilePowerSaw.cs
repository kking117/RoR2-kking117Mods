using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using RoR2.Skills;
using UnityEngine;

namespace AgilePowerSaw
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.AgilePowerSaw";
		public const string MODNAME = "AgilePowerSaw";
		public const string MODVERSION = "1.0.0";

		public static ConfigEntry<bool> PowerSaw;
		public static ConfigEntry<bool> Grenade;
		public static ConfigEntry<bool> Flamethrower;
		public static ConfigEntry<bool> NanoSphere;
		public static ConfigEntry<bool> IceSpear;

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
				SkillDef skillDef = Resources.Load<SkillDef>("skilldefs/toolbotbody/toolbotbodyfirebuzzsaw");
				if (skillDef)
				{
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Grenade.Value)
			{
				SkillDef skillDef = Resources.Load<SkillDef>("skilldefs/engibody/engibodyfiregrenade");
				if (skillDef)
				{
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (NanoSphere.Value)
            {
				SkillDef skillDef = Resources.Load<SkillDef>("skilldefs/magebody/magebodynovabomb");
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (IceSpear.Value)
			{
				SkillDef skillDef = Resources.Load<SkillDef>("skilldefs/magebody/magebodyicebomb");
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
			}
			if (Flamethrower.Value)
			{
				SkillDef skillDef = Resources.Load<SkillDef>("skilldefs/magebody/MageBodyFlamethrower");
				if (skillDef)
				{
					skillDef.canceledFromSprinting = false;
					skillDef.cancelSprintingOnActivation = false;
				}
				if (AncientScepter)
				{
					skillDef = GetSkillDefNameToken("ANCIENTSCEPTER_MAGE_FLAMETHROWERNAME");
					if (skillDef)
					{
						skillDef.canceledFromSprinting = false;
						skillDef.cancelSprintingOnActivation = false;
					}
				}
			}
		}
		private SkillDef GetSkillDefNameToken(string token)
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
			PowerSaw = Config.Bind<bool>(new ConfigDefinition("Mul-T", "Power-Saw"), true, new ConfigDescription("Makes Mul-T's Power-Saw skill agile.", null, Array.Empty<object>()));
			Grenade = Config.Bind<bool>(new ConfigDefinition("Engineer", "Bouncing Grenade"), false, new ConfigDescription("Makes Engineer's Bouncing Grenade skill agile.", null, Array.Empty<object>()));
			NanoSphere = Config.Bind<bool>(new ConfigDefinition("Artificer", "Charged Nano-Bomb"), false, new ConfigDescription("Makes Artificer's Charged Nano-Bomb skill agile.", null, Array.Empty<object>()));
			IceSpear = Config.Bind<bool>(new ConfigDefinition("Artificer", "Cast Nano-Spear"), false, new ConfigDescription("Makes Artificer's Cast Nano-Spear skill agile.", null, Array.Empty<object>()));
			Flamethrower = Config.Bind<bool>(new ConfigDefinition("Artificer", "Flamethrower"), false, new ConfigDescription("Makes Artificer's Flamethrower skill agile.", null, Array.Empty<object>()));
		}
	}
}
