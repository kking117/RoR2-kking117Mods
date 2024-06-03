using System;
using BepInEx;
using RoR2;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace VoidDamageConfig
{
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.kking117.VoidDamageConfig";
        public const string MODNAME = "VoidDamageConfig";
        public const string MODTOKEN = "KKING117_VOIDDAMAGECONFIG_";
        public const string MODVERSION = "1.0.0";
        public const string LOGNAME = "[VoidDamageConfig] ";

        internal static BepInEx.Logging.ManualLogSource ModLogger;
        public static PluginInfo pluginInfo;

        private void Awake()
        {
            ModLogger = this.Logger;
            pluginInfo = Info;
            Configs.Setup();
            EnableChanges();
            GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
        }

        private void EnableChanges()
        {
            new Changes.EffectA();
        }
        private void PostLoad()
        {
            Changes.EffectA.PostLoad();
        }
    }
}
