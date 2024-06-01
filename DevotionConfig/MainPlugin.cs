using System;
using BepInEx;
using RoR2;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DevotionConfig
{
    [BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.kking117.DevotionConfig";
        public const string MODNAME = "DevotionConfig";
        public const string MODTOKEN = "KKING117_DEVOTIONCONFIG_";
        public const string MODVERSION = "1.0.0";
        public const string LOGNAME = "[DevotionConfig] ";

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
            new Changes.HeavyChanges();
            new Changes.LightChanges();
        }
        private void PostLoad()
        {
            Changes.LightChanges.PostLoad();
            Changes.HeavyChanges.PostLoad();
        }
    }
}
