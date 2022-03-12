using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TestTest
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.kking117.VoidInfestorTweak";
        public const string MODNAME = "VoidInfestorTweak";
        public const string MODVERSION = "1.1.0";

        public static ConfigEntry<bool> InfestFilter_Boss;
        public static ConfigEntry<bool> InfestFilter_Boss_OnPlayer;
        public static ConfigEntry<bool> InfestFilter_Mechanical;
        public static ConfigEntry<bool> InfestFilter_Player;
        public static ConfigEntry<bool> InfestFilter_Void;
        public static ConfigEntry<bool> InfestFilter_Self;

        public const string MODTOKEN = "KKING117_VOIDINFESTORTWEAK_";
		public void Awake()
        {
            ReadConfig();
            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
            //On.RoR2.CharacterMaster.OnBodyStart += OnBodyStart;
        }

        /*private void OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if(self.playerCharacterMasterController)
            {
                self.inventory.SetEquipmentIndex(DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex);
            }
        }*/
        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();
            Characters.VoidInfestor.Begin();
            new Modules.ContentPacks().Initialize();
        }
        private void ReadConfig()
        {
            InfestFilter_Boss = Config.Bind<bool>(new ConfigDefinition("Global Infest Filters", "Boss"), true, new ConfigDescription("Should Void Infestors be able to infest bosses? (Red health bar)", null, Array.Empty<object>()));
            InfestFilter_Player = Config.Bind<bool>(new ConfigDefinition("Global Infest Filters", "Player"), true, new ConfigDescription("Should Void Infestors be able to infest characters on the player team?", null, Array.Empty<object>()));
            InfestFilter_Mechanical = Config.Bind<bool>(new ConfigDefinition("Global Infest Filters", "Void Infestor"), false, new ConfigDescription("Should Void Infestors be able to infest Mechanical characters?", null, Array.Empty<object>()));
            InfestFilter_Void = Config.Bind<bool>(new ConfigDefinition("Global Infest Filters", "Void Creatures"), false, new ConfigDescription("Should Void Infestors be able to infest characters that are considered void? (Purple health bar)", null, Array.Empty<object>()));
            InfestFilter_Self = Config.Bind<bool>(new ConfigDefinition("Global Infest Filters", "Void Infestor"), false, new ConfigDescription("Should Void Infestors be able to infest Void Infestors? (I don't even want to know.)", null, Array.Empty<object>()));
        }
    }
}