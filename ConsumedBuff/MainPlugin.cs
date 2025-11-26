using System;
using BepInEx;
using BepInEx.Configuration;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ConsumedBuff
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.recalculatestats", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.kking117.ConsumedBuff";
        public const string MODNAME = "ConsumedBuff";
        public const string MODVERSION = "1.4.0";
        public const string MODTOKEN = "KKING117_CONSUMEDBUFF_";

        private const string Section_Elixir = "Empty Bottle";
        private const string Section_Watch = "Delicate Watch (Broken)";
        private const string Section_VoidDio = "Pluripotent Larva (Consumed)";
        private const string Section_Dio = "Dios Best Friend (Consumed)";

        public static bool VoidDio_Enable = false;
        public static float VoidDio_BlockCooldown = 0.9f;
        public static float VoidDio_CollapseChance = 100f;
        public static float VoidDio_CollapseDamage = 4f;
        public static bool VoidDio_CollapseUseTotal = false;
        public static bool VoidDio_Corrupt = true;
        public static float VoidDio_Curse = 0.1f;

        public static bool Dio_Enable = false;
        public static float Dio_BlockChance = 15f;

        public static bool Elixir_Enable = false;
        public static float Elixir_Buff = 2.5f;
        public static float Elixir_Regen = 1f;

        public static bool Watch_Enable = false;
        public static bool Watch_Indicator = true;
        public static float Watch_Damage = 0.2f;
        public static int Watch_HitsToProc = 12;
        public static int Watch_ProcsToDouble = 12;
        public static float Watch_SlowBase = 1f;
        public static float Watch_SlowStack = 0.25f;
        public static bool Watch_VFX = true;
        private void Awake()
        {
            ReadConfig();
            if (Elixir_Enable)
            {
                ItemChanges.EmptyBottle.Enable();
            }
            if(VoidDio_Enable)
            {
                ItemChanges.PluripotentLarvaConsumed.Enable();
            }
            if (Watch_Enable)
            {
                ItemChanges.DelicateWatchBroken.Enable();
            }
            if(Dio_Enable)
            {
                ItemChanges.DiosBestFriendConsumed.Enable();
            }
            new Modules.ContentPacks().Initialize();
        }
        private void ReadConfig()
        {
            Elixir_Enable = Config.Bind(Section_Elixir, "Enable Changes", false, "Allows this mod to make changes to the Empty Bottle item.").Value;
            Elixir_Buff = Config.Bind(Section_Elixir, "Regen Buff Duration", 2.5f,"Duration of the regen buff when the Power Elixir item is consumed.").Value;
            Elixir_Regen = Config.Bind(Section_Elixir, "Passive Regen", 1.0f, "passive regen per stack this item gives.").Value;

            Watch_Enable = Config.Bind(Section_Watch, "Enable Changes", false, "Allows this mod to make changes to the Delicate Watch (Broken) item.").Value;
            Watch_Indicator = Config.Bind(Section_Watch, "Buff Indicator", true, "Enables a buff to help track how many hits the item needs.").Value;
            Watch_HitsToProc = Config.Bind(Section_Watch, "Hit To Proc", 12, "On this number hit, proc its effect.").Value;
            Watch_ProcsToDouble = Config.Bind(Section_Watch, "Proc to Double Proc", 12, "On this number proc, double the proc effect. (1 or less disables this)").Value;
            Watch_Damage = Config.Bind(Section_Watch, "Damage On Proc", 0.2f, "Damage bonus on procs.").Value;
            Watch_SlowBase = Config.Bind(Section_Watch, "Base Slow On Proc", 1f, "Base duration of the slow effect that this applies on proc.").Value;
            Watch_SlowStack = Config.Bind(Section_Watch, "Stack Slow On Proc", 0.25f, "Stack duration of the slow effect.").Value;
            Watch_VFX = Config.Bind(Section_Watch, "Play Proc VFX", true, "Play a VFX on the target when proccing the item?").Value;

            VoidDio_Enable = Config.Bind(Section_VoidDio, "Enable Changes", false, "Allows this mod to make changes to the Pluripotent Larva (Consumed) item.").Value;
            VoidDio_BlockCooldown = Config.Bind(Section_VoidDio, "Block Cooldown", 0.9f, "How much to multiply the block cooldown per stack. (Higher than 1 disables this)").Value;
            VoidDio_CollapseChance = Config.Bind(Section_VoidDio, "Collapse Chance", 10.0f, "The chance to apply Collapse per stack.").Value;
            VoidDio_CollapseDamage = Config.Bind(Section_VoidDio, "Collapse Damage", 4.0f, "How much damage each Collapse stack does.").Value;
            VoidDio_CollapseUseTotal = Config.Bind(Section_VoidDio, "Collapse Total Damage", false, "Should the Collapse stack deal total damage instead of base damage?").Value;
            VoidDio_Corrupt = Config.Bind(Section_VoidDio, "Corrupted Life", true, "Automatically corrupt new items and be classed as void while carrying this item?").Value;
            VoidDio_Curse = Config.Bind(Section_VoidDio, "Health Curse", 0.1f, "Health curse per stack.").Value;

            Dio_Enable = Config.Bind(Section_Dio, "Enable", false, "Allows this mod to make changes to the Dios Best Friend (Consumed) item.").Value;
            Dio_BlockChance = Config.Bind(Section_Dio, "Block Chance", 15.0f, "The chance per stack to block damage.").Value;
        }
    }
}
