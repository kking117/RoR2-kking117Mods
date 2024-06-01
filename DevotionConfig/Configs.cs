using System;
using System.IO;
using RoR2;
using R2API;
using UnityEngine;
using BepInEx.Configuration;
using DevotionConfig.Changes;

namespace DevotionConfig
{
	//99% of this is practically ripped off from RiskyMod, ain't gonna lie.
	public static class Configs
	{
		public static ConfigFile HeavyConfig;
		public static ConfigFile LightConfig;
		public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, MainPlugin.pluginInfo.Metadata.GUID); }

		private const string Section_EnableConfig = "!Enable Config";
		private const string Section_BaseStats = "Minion Base Stats";
		private const string Section_Evolution = "Evolution Settings";
		private const string Section_Death = "Death Settings";
		
		private const string Section_Misc = "Misc Settings";

		private const string Section_ItemDrop = "Item Drop";
		private const string Section_Blacklist = "Item Blacklist";

		private const string Label_Enable = "!Enable Changes";

		private const string Desc_Enable = "Enables changes for this section.";

		public static void Setup()
        {
			LightConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Light.cfg"), true);
			HeavyConfig = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"Heavy.cfg"), true);
			Read_LightConfig();
			Read_HeavyConfig();
		}
		private static void Read_HeavyConfig()
        {
			//Enable
			HeavyChanges.Enable = HeavyConfig.Bind(Section_EnableConfig, "Enable Config", false, "Enables this entire part of the mod to function, may cause mod incompats.").Value;
			//Base Stats
			HeavyChanges.Evo_HPStages_Raw = HeavyConfig.Bind(Section_BaseStats, "Health Multiplier", "E1, 1, E2, 2, E3, 1, E4, 2", "Health multiplier for devotion minions, per evolution stage. (1 = +100% health or effectively 200% health)").Value;
			HeavyChanges.Evo_DmgStages_Raw = HeavyConfig.Bind(Section_BaseStats, "Damage Multiplier", "E1, 1, E2, 2, E3, 1, E4, 2", "Damage multiplier for devotion minions, per evolution stage. (1 = +100% damage or effectively 200% damage)").Value;
			HeavyChanges.Evo_RegenStages_Raw = HeavyConfig.Bind(Section_BaseStats, "Base Regen", "E1, 0, E2, 0, E3, 0, E4, 0", "Base health regen added to devotion minions, per evolution stage. Scales with level.").Value;
			//Evolution
			HeavyChanges.EvoMax = HeavyConfig.Bind(Section_Evolution, "Evolution Level Cap", 0, "The maximum evolution level devotion minions can reach. (0 = Unlimited/Vanilla)").Value;
			HeavyChanges.EliteList_Raw = HeavyConfig.Bind(Section_Evolution, "Elite Tiers", "edFire, 1, edIce, 1, edLightning, 1, edEarth, 1, edPoison, 2, edHaunted, 2, edLunar, 2", "List of Elite Types and their Tier.").Value;
			HeavyChanges.Evo_BodyStages_Raw = HeavyConfig.Bind(Section_Evolution, "Evolution Body Stages", "LemurianBruiserBody, 3", "Change the Body type at specific evolution levels with a specific body name.").Value;
			HeavyChanges.Evo_EliteStages_Raw = HeavyConfig.Bind(Section_Evolution, "Evolution Elite Stages", "T1, 2, T0, 3, T2, 4", "Change the Elite type at specific evolution levels with a maximum elite tier.").Value;
			HeavyChanges.Evo_BaseMaster_Raw = HeavyConfig.Bind(Section_Evolution, "Base Stage Master", "DevotedLemurianMaster", "Master Name for the basic form, used for evolution stage 1.").Value;
			//Death
			HeavyChanges.Death_Penalty = HeavyConfig.Bind(Section_Death, "Penalty", 0, "What to do to a devotion minion when it dies. (0 = Kill/Vanilla, 1 = Devolve, 2 = Reset Evolution Level to 1) (For 1 and 2 it will kill them if they're at Evolution Level 1.)").Value;
			HeavyChanges.Death_EggDrop = HeavyConfig.Bind(Section_Death, "Egg On Death", false, "Should minions revert to an egg when they are killed off?").Value;
			//Misc
			HeavyChanges.EvoLevelTrack = HeavyConfig.Bind(Section_Misc, "Display Evolution Level", false, "Shows the evolution level of each minion on their name.").Value;
			HeavyChanges.Misc_LeashDistance = HeavyConfig.Bind(Section_Misc, "Leash Distance", 400f, "Minion leash distance.").Value;
		}
		private static void Read_LightConfig()
		{
			LightChanges.ItemDrop_Enable = LightConfig.Bind(Section_ItemDrop, Label_Enable, false, Desc_Enable).Value;
			LightChanges.ItemDrop_Type = LightConfig.Bind(Section_ItemDrop, "Item Drop", 0, "What kind of item to drop when minions are removed. (0 = Scrap/Custom, 1 = Original Item, 2 = Nothing").Value;
			LightChanges.ItemDrop_CustomDropList_Raw = LightConfig.Bind(Section_ItemDrop, "Custom Drop List", "Tier1Def, ScrapWhite, Tier2Def, ScrapGreen, Tier3Def, ScrapRed, BossTierDef, ScrapYellow, LunarTierDef, LunarTrinket, VoidTier1Def, TreasureCacheVoid, VoidTier2Def, TreasureCacheVoid, VoidTier3Def, TreasureCacheVoid, VoidBossDef, TreasureCacheVoid", "The item to drop for each tier when Item Drop is set to 0.").Value;
			//Blacklist
			LightChanges.Blacklist_Enable = LightConfig.Bind(Section_Blacklist, Label_Enable, false, Desc_Enable).Value;
			LightChanges.BlackList_Filter_CannotCopy = LightConfig.Bind(Section_Blacklist, "Include CannotCopy", true, "Automatically blacklist items that are tagged as CannotCopy. (The same filter used for Engineer Turrets)").Value;
			LightChanges.BlackList_Filter_Scrap = LightConfig.Bind(Section_Blacklist, "Include Scrap", true, "Automatically blacklist items that are tagged as Scrap.").Value;
			LightChanges.BlackList_ItemList_Raw = LightConfig.Bind(Section_Blacklist, "Item Blacklist", "WardOnLevel, BeetleGland", "Items to blacklist from being picked.").Value;
			LightChanges.BlackList_TierList_Raw = LightConfig.Bind(Section_Blacklist, "Tier Blacklist", "LunarTierDef, VoidTier1Def, VoidTier2Def, VoidTier3Def, VoidBossDef", "Tiers to blacklist from being picked.").Value;
			//Misc
			LightChanges.Misc_FixEvo = LightConfig.Bind(Section_Misc, "Fix When Disabled", true, "Fixes the item orb not showing when giving items to eggs and allows devotion minions to evolve even when the Artifact is disabled.").Value;
		}
	}
}
