using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ConsumedBuff
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "LanguageAPI",
        "RecalculateStatsAPI"
    })]
    public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.kking117.ConsumedBuff";
        public const string MODNAME = "ConsumedBuff";
        public const string MODVERSION = "1.2.2";
        public const string MODTOKEN = "KKING117_CONSUMEDBUFF_";

        public static ConfigEntry<bool> VoidDio_Enable;
        public static ConfigEntry<int> VoidDio_BearWorth;
        public static ConfigEntry<int> VoidDio_BleedWorth;
        public static ConfigEntry<bool> VoidDio_Corrupt;
        public static ConfigEntry<float> VoidDio_Curse;

        public static ConfigEntry<bool> Dio_Enable;
        public static ConfigEntry<int> Dio_BearWorth;

        public static ConfigEntry<bool> Elixir_Enable;
        public static ConfigEntry<float> Elixir_Buff;
        public static ConfigEntry<float> Elixir_Regen;

        public static ConfigEntry<bool> Watch_Enable;
        public static ConfigEntry<bool> Watch_Indicator;
        public static ConfigEntry<float> Watch_Damage;
        public static ConfigEntry<int> Watch_HitsToProc;
        public static ConfigEntry<int> Watch_ProcsToDouble;
        public static ConfigEntry<float> Watch_SlowBase;
        public static ConfigEntry<float> Watch_SlowStack;
        private void Awake()
        {
            ReadConfig();
            if (Elixir_Enable.Value)
            {
                ItemChanges.EmptyBottle.Enable();
            }
            if(VoidDio_Enable.Value)
            {
                ItemChanges.PluripotentLarvaConsumed.Enable();
            }
            if (Watch_Enable.Value)
            {
                ItemChanges.DelicateWatchBroken.Enable();
            }
            if(Dio_Enable.Value)
            {
                ItemChanges.DiosBestFriendConsumed.Enable();
            }
            new Modules.ContentPacks().Initialize();
        }
        private void ReadConfig()
        {
            Elixir_Enable = Config.Bind<bool>(new ConfigDefinition("Empty Bottle", "Enable Changes"), true, new ConfigDescription("Allows this mod to make changes to the Empty Bottle item.", null, Array.Empty<object>()));
            Elixir_Buff = Config.Bind<float>(new ConfigDefinition("Empty Bottle", "Regen Buff Duration"), 2.5f, new ConfigDescription("Duration of the regen buff when the Power Elixir item is consumed.", null, Array.Empty<object>()));
            Elixir_Regen = Config.Bind<float>(new ConfigDefinition("Empty Bottle", "Passive Regen"), 0.6f, new ConfigDescription("passive regen per stack this item gives.", null, Array.Empty<object>()));

            Watch_Enable = Config.Bind<bool>(new ConfigDefinition("Delicate Watch (Broken)", "Enable Changes"), true, new ConfigDescription("Allows this mod to make changes to the Delicate Watch (Broken) item.", null, Array.Empty<object>()));
            Watch_Indicator = Config.Bind<bool>(new ConfigDefinition("Delicate Watch (Broken)", "Buff Indicator"), true, new ConfigDescription("Enables a buff to help track how many hits the item needs.", null, Array.Empty<object>()));
            Watch_HitsToProc = Config.Bind<int>(new ConfigDefinition("Delicate Watch (Broken)", "Hit To Proc"), 12, new ConfigDescription("On this number hit, proc its effect.", null, Array.Empty<object>()));
            Watch_ProcsToDouble = Config.Bind<int>(new ConfigDefinition("Delicate Watch (Broken)", "Proc to Double Proc"), 12, new ConfigDescription("On this number proc, double the proc effect. (1 or less disables this)", null, Array.Empty<object>()));
            Watch_Damage = Config.Bind<float>(new ConfigDefinition("Delicate Watch (Broken)", "Damage On Proc"), 0.2f, new ConfigDescription("Damage bonus on procs.", null, Array.Empty<object>()));
            Watch_SlowBase = Config.Bind<float>(new ConfigDefinition("Delicate Watch (Broken)", "Base Slow On Proc"), 1f, new ConfigDescription("Base duration of the slow effect that this applies on proc.", null, Array.Empty<object>()));
            Watch_SlowStack = Config.Bind<float>(new ConfigDefinition("Delicate Watch (Broken)", "Stack Slow On Proc"), 0.25f, new ConfigDescription("Stack duration of the slow effect.", null, Array.Empty<object>()));

            VoidDio_Enable = Config.Bind<bool>(new ConfigDefinition("Pluripotent Larva (Consumed)", "Enable Changes"), true, new ConfigDescription("Allows this mod to make changes to the Pluripotent Larva (Consumed) item.", null, Array.Empty<object>()));
            VoidDio_BearWorth = Config.Bind<int>(new ConfigDefinition("Pluripotent Larva (Consumed)", "Safer Spaces Worth"), 1, new ConfigDescription("How many Safer Spaces this is worth.", null, Array.Empty<object>()));
            VoidDio_BleedWorth = Config.Bind<int>(new ConfigDefinition("Pluripotent Larva (Consumed)", "Needle Tick Worth"), 1, new ConfigDescription("How many Needle Ticks this is worth.", null, Array.Empty<object>()));
            VoidDio_Corrupt = Config.Bind<bool>(new ConfigDefinition("Pluripotent Larva (Consumed)", "Corrupted Life"), true, new ConfigDescription("Automatically corrupt new items and be classed as void while carrying this item?", null, Array.Empty<object>()));
            VoidDio_Curse = Config.Bind<float>(new ConfigDefinition("Pluripotent Larva (Consumed)", "Health Curse"), 0.1f, new ConfigDescription("Health curse per stack.", null, Array.Empty<object>()));

            Dio_Enable = Config.Bind<bool>(new ConfigDefinition("Dios Best Friend (Consumed)", "Enable"), true, new ConfigDescription("Allows this mod to make changes to the Dios Best Friend (Consumed) item.", null, Array.Empty<object>()));
            Dio_BearWorth = Config.Bind<int>(new ConfigDefinition("Dios Best Friend (Consumed)", "Tougher Times Worth"), 1, new ConfigDescription("How many Tougher Times this is worth.", null, Array.Empty<object>()));
        }
    }
}
