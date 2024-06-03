using BepInEx.Configuration;
using VoidDamageConfig.Changes;

namespace VoidDamageConfig
{
	public static class Configs
	{
		public static ConfigFile MainConfig;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_VoidFog = "Void Fog Settings";
		private const string Section_VoidKill = "Void Kill Settings";
		private const string Section_Minion = "Minion Filter Settings";

		private const string Label_Enable = "!Enable Changes";
		private const string Desc_Enable = "Enables this section to function.";

		public static void Setup()
        {
			MainConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"MainConfig.cfg"), true);
			Read_LightConfig();
		}
		private static void Read_LightConfig()
		{
			EffectA.VoidFog_Enable = MainConfig.Bind(Section_VoidFog, Label_Enable, false, Desc_Enable).Value;
			EffectA.VoidFog_OnlyMinion = MainConfig.Bind(Section_VoidFog, "Only Minions", true, "Apply all of this to just Minions.").Value;
			EffectA.VoidFog_DamageType = MainConfig.Bind(Section_VoidFog, "Fog Damage", 0, "How void fog damage is dealt. (0 = Vanilla, 1 = Nonlethal, 2 = No Damage)").Value;
			EffectA.VoidFog_AllowArena = MainConfig.Bind(Section_VoidFog, "Minions In Void Fields", false, "Allow minions to spawn in Void Fields?").Value;
			EffectA.VoidFog_AllowLocus = MainConfig.Bind(Section_VoidFog, "Minions In Void Locus", true, "Allow minions to spawn in Void Locus?").Value;
			EffectA.VoidFog_NerfVoidLocus = MainConfig.Bind(Section_VoidFog, "Nerf Void Locus", false, "Makes Void Locus fog deal the same amount of damage as Void Fields fog.").Value;

			EffectA.VoidDamage_Enable = MainConfig.Bind(Section_VoidKill, Label_Enable, false, Desc_Enable).Value;
			EffectA.VoidDamage_OnlyMinion = MainConfig.Bind(Section_VoidKill, "Only Minions", true, "Apply all of this to just Minions.").Value;
			EffectA.VoidDamage_MaxPercent = MainConfig.Bind(Section_VoidKill, "Max Percent Damage", 0.05f, "Percent of Max Health to take as damage when hit by Void Instant Kills.").Value;
			EffectA.VoidDamage_CurPercent = MainConfig.Bind(Section_VoidKill, "Current Percent Damage", 0.45f, "Percent of Current Health to take as damage when hit by Void Instant Kills.").Value;
			EffectA.VoidDamage_NonLethal = MainConfig.Bind(Section_VoidKill, "Nonlethal", false, "Makes Void Instant Kills nonlethal.").Value;
			EffectA.VoidDamage_Nullify_Duration = MainConfig.Bind(Section_VoidKill, "Nullify Duration", 3f, "Apply the Nullify debuff for this long when hit by Void Instant Kills").Value;
			EffectA.VoidDamage_AllowOverride = MainConfig.Bind(Section_VoidKill, "Override Suicide", false, "Attempt to override Void attacks that force kill the target instead of damaging them, may cause stuttering and massive incompats, not recommended. (The only vanilla case is Voidling's Vacuum attack.)").Value;
			EffectA.VoidDamage_OverrideDamage_Raw = MainConfig.Bind(Section_VoidKill, "Override Suicide Divider", "MiniVoidRaidCrabBodyBase, 25, MiniVoidRaidCrabBodyPhase1, 25, MiniVoidRaidCrabBodyPhase2, 25, MiniVoidRaidCrabBodyPhase3, 25", "List of character bodies that get their Void damage divided when attempting to force kill a target.").Value;

			EffectA.Minion_AllowPlayer = MainConfig.Bind(Section_Minion, "Allow Player", false, "Allow minions currently being controlled by a player to pass as a minion.").Value;
			EffectA.Minion_BodyWhiteList_Raw = MainConfig.Bind(Section_Minion, "Body Whitelist", "TitanGoldBody", "List of bodies that will always count as minions for the purpose of this mod. (Has priority over Blacklists)").Value;
			EffectA.Minion_BodyBlackList_Raw = MainConfig.Bind(Section_Minion, "Body Blacklist", "EngiTurretBody, EngiWalkerTurretBody", "List of bodies that will never count as minions for the purpose of this mod.").Value;
			EffectA.Minion_ItemWhiteList_Raw = MainConfig.Bind(Section_Minion, "Item Whitelist", "", "List of items that will make characters always count as minions for the purpose of this mod. (Has priority over Blacklists)").Value;
			EffectA.Minion_ItemBlackList_Raw = MainConfig.Bind(Section_Minion, "Item Blacklist", "", "List of items that will make characters never count as minions for the purpose of this mod.").Value;
		}
	}
}
