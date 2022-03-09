using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API;
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
        public const string MODVERSION = "1.0.0";
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
        public static ConfigEntry<bool> Watch_Damage;
        public static ConfigEntry<int> Watch_Hits;
        public static ConfigEntry<int> Watch_SuperHits;
        public static ConfigEntry<float> Watch_SlowBase;
        public static ConfigEntry<float> Watch_SlowStack;

        public void Awake()
        {
            ReadConfig();
            if (Elixir_Enable.Value)
            {
                ItemChanges.PowerElixir.Enable();
            }
            if(VoidDio_Enable.Value)
            {
                ItemChanges.PluripotentLarva.Enable();
            }
            if (Watch_Enable.Value)
            {
                ItemChanges.DelicateWatch.Enable();
            }
            if(Dio_Enable.Value)
            {
                ItemChanges.DiosBestFriend.Enable();
            }
        }
        private void ReadConfig()
        {
            Elixir_Enable = Config.Bind<bool>(new ConfigDefinition("Power Elixir Changes", "Enable"), true, new ConfigDescription("Allows this mod to make changes to the Power Elixir item.", null, Array.Empty<object>()));
            Elixir_Buff = Config.Bind<float>(new ConfigDefinition("Power Elixir Changes", "Regen Buff Duration"), 1.25f, new ConfigDescription("Duration of the regen buff when the item is consumed.", null, Array.Empty<object>()));
            Elixir_Regen = Config.Bind<float>(new ConfigDefinition("Power Elixir Changes", "Passive Regen"), 1.0f, new ConfigDescription("Passive regen per stack that the consumed version gives.", null, Array.Empty<object>()));

            VoidDio_Enable = Config.Bind<bool>(new ConfigDefinition("Pluripotent Larva Changes", "Enable"), true, new ConfigDescription("Allows this mod to make changes to the Pluripotent Larva item.", null, Array.Empty<object>()));
            VoidDio_BearWorth = Config.Bind<int>(new ConfigDefinition("Pluripotent Larva Changes", "Safer Spaces Worth"), 1, new ConfigDescription("How many Safer Spaces the consumed version is worth.", null, Array.Empty<object>()));
            VoidDio_BleedWorth = Config.Bind<int>(new ConfigDefinition("Pluripotent Larva Changes", "Needle Tick Worth"), 1, new ConfigDescription("How many Needle Ticks the consumed version is worth.", null, Array.Empty<object>()));
            VoidDio_Corrupt = Config.Bind<bool>(new ConfigDefinition("Pluripotent Larva Changes", "Corrupted Life"), true, new ConfigDescription("Automatically corrupt new items and be classed as void while carrying the consumed version.", null, Array.Empty<object>()));
            VoidDio_Curse = Config.Bind<float>(new ConfigDefinition("Pluripotent Larva Changes", "Health Curse"), 0.1f, new ConfigDescription("Health curse per stack that the consumed version gives.", null, Array.Empty<object>()));

            Watch_Enable = Config.Bind<bool>(new ConfigDefinition("Delicate Watch Changes", "Enable"), true, new ConfigDescription("Allows this mod to make changes to the Delicate Watch item.", null, Array.Empty<object>()));
            Watch_Hits = Config.Bind<int>(new ConfigDefinition("Delicate Watch Changes", "Hits To Proc"), 12, new ConfigDescription("How many hits are needed to proc the consumed version's effect.", null, Array.Empty<object>()));
            Watch_SuperHits = Config.Bind<int>(new ConfigDefinition("Delicate Watch Changes", "Hits To Proc Super"), 144, new ConfigDescription("How many hits are needed to proc the consumed version's super effect.", null, Array.Empty<object>()));
            Watch_Damage = Config.Bind<bool>(new ConfigDefinition("Delicate Watch Changes", "Damage On Proc"), true, new ConfigDescription("Should the consumed version apply its damage bonus on proc?", null, Array.Empty<object>()));
            Watch_SlowBase = Config.Bind<float>(new ConfigDefinition("Delicate Watch Changes", "Base Slow On Proc"), 0.75f, new ConfigDescription("Base duration of the slow effect the consumed version applies.", null, Array.Empty<object>()));
            Watch_SlowStack = Config.Bind<float>(new ConfigDefinition("Delicate Watch Changes", "Stack Slow On Proc"), 0.25f, new ConfigDescription("How much extra duration each stack gives to the slow effect.", null, Array.Empty<object>()));

            Dio_Enable = Config.Bind<bool>(new ConfigDefinition("Dios Best Friend Changes", "Enable"), true, new ConfigDescription("Allows this mod to make changes to the Dios Best Friend item.", null, Array.Empty<object>()));
            Dio_BearWorth = Config.Bind<int>(new ConfigDefinition("Dios Best Friend Changes", "Tougher Times Worth"), 1, new ConfigDescription("How many Tougher Times the consumed version is worth.", null, Array.Empty<object>()));
        }
        public static void AddTimeToBuff(CharacterBody body, BuffDef buff, float duration, bool onlyonce)
        {
            if (body && body.GetBuffCount(buff) > 0)
            {
                foreach (CharacterBody.TimedBuff timedBuff in body.timedBuffs)
                {
                    if (timedBuff.buffIndex == buff.buffIndex)
                    {
                        timedBuff.timer += duration;
                        if (onlyonce)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                body.AddTimedBuff(buff, 2.5f);
            }
        }
    }
}
