using BepInEx.Configuration;
using QueenGlandBuff.Changes;

namespace QueenGlandBuff
{
    public static class Configs
    {
		public static ConfigFile MainConfig;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_Queens_Gland = "Queens Gland";
        private const string Section_Primary_Skill = "Primary Skill - Slam";
        private const string Section_Secondary_Skill = "Secondary Skill - Sunder";
        private const string Section_Special_Skill = "Special Skill - Valor";
		private const string Section_Stats_Base = "Beetle Guard Ally Stats Base";
		private const string Section_Stats_Level = "Beetle Guard Ally Stats Level";
		private const string Section_Misc = "Misc";
		public static void Setup()
        {
			MainConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"MainConfig.cfg"), true);
			Read_Queens_Gland();
			Read_Primary_SKill();
			Read_Secondary_SKill();
			Read_Special_SKill();
			Read_Beetle_Stats();
			Read_Misc();
		}
		private static void Read_Queens_Gland()
        {
			QueensGland.Enable = MainConfig.Bind(Section_Queens_Gland, "Enable Changes", true, "Enable changes to the Queens Gland item.").Value;
			QueensGland.MaxSummons = MainConfig.Bind(Section_Queens_Gland, "Max Summons", 1, "The Max amount of Beetle Guards each player can have.").Value;
			QueensGland.RespawnTime = MainConfig.Bind(Section_Queens_Gland, "Respawn Time", 30, "How long it takes for Beetle Guards to respawn.").Value;
			QueensGland.BaseHealth = MainConfig.Bind(Section_Queens_Gland, "BaseHealth", 10, "Extra health at a single stack. (1 = +10%)").Value;
			QueensGland.StackHealth = MainConfig.Bind(Section_Queens_Gland, "StackHealth", 10, "Extra Health to give per stack after capping out the max summons. (1 = +10%)").Value;
			QueensGland.BaseDamage = MainConfig.Bind(Section_Queens_Gland, "BaseDamage", 10, "Extra damage at a single stack. (1 = +10%)").Value;
			QueensGland.StackDamage = MainConfig.Bind(Section_Queens_Gland, "StackDamage", 20, "Extra Damage to give per stack after capping out the max summons. (1 = +10%)").Value;
			QueensGland.AffixMode = MainConfig.Bind(Section_Queens_Gland, "Become Elite", 1, "Makes Beetle Guard Ally spawn with an Elite Affix. (0 = never, 1 = always, 2 = only during Honor)").Value;
			QueensGland.DefaultAffixName = MainConfig.Bind(Section_Queens_Gland, "Default Elite", "EliteFireEquipment", "The Fallback equipment to give if an Elite Affix wasn't selected. (Set to None to disable)").Value;
		}
		private static void Read_Primary_SKill()
        {
			BeetleGuardAlly.Enable_Slam_Skill = MainConfig.Bind(Section_Primary_Skill, "Enable Slam Changes", true, "Enable changes to its Slam skill.").Value;
		}
		private static void Read_Secondary_SKill()
		{
			BeetleGuardAlly.Enable_Sunder_Skill = MainConfig.Bind(Section_Secondary_Skill, "Enable Sunder Changes", true, "Enable changes to its Sunder skill.").Value;
		}
		private static void Read_Special_SKill()
		{
			BeetleGuardAlly.Enable_Valor_Skill = MainConfig.Bind(Section_Special_Skill, "Enable Valor Changes", true, "Enable the custom Valor skill.").Value;
			ValorBuff.Aggro_Range = MainConfig.Bind(Section_Special_Skill, "Aggro Range", 60f, "Regular enemies within this range will aggro onto affected characters.").Value;
			ValorBuff.Buff_Armor = MainConfig.Bind(Section_Special_Skill, "Armor", 100f, "The amount of armor the user of this buff gets.").Value;
		}
		private static void Read_Beetle_Stats()
        {
			BeetleGuardAlly.Stats_Base_Health = MainConfig.Bind(Section_Stats_Base, "Health", 480f, "Health at level 1. (Vanilla = 480)").Value;
			BeetleGuardAlly.Stats_Base_Damage = MainConfig.Bind(Section_Stats_Base, "Damage", 12f, "Damage at level 1. (Vanilla = 14)").Value;
			BeetleGuardAlly.Stats_Base_Regen = MainConfig.Bind(Section_Stats_Base, "Regen", 5f, "Regen at level 1. (Vanilla = 0.6)").Value;

			BeetleGuardAlly.Stats_Level_Health = MainConfig.Bind(Section_Stats_Level, "Health", 144f, "Health gained per level. (Vanilla = 144)").Value;
			BeetleGuardAlly.Stats_Level_Damage = MainConfig.Bind(Section_Stats_Level, "Damage", 2.4f, "Damage gained per level. (Vanilla = 2.4)").Value;
			BeetleGuardAlly.Stats_Level_Regen = MainConfig.Bind(Section_Stats_Level, "Regen", 1f, "Regen gained per level. (Vanilla = 0.12)").Value;
		}
		private static void Read_Misc()
		{
			BeetleGuardAlly.Elite_Skills = MainConfig.Bind(Section_Misc, "Elite Skills", true, "Changes how some of its skills function based on its Aspect.").Value;
		}
	}
}
