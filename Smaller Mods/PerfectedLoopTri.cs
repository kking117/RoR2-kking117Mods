using System;
using System.Linq;
using System.Collections;
using BepInEx;
using RoR2;
using R2API;
using R2API.Utils;
using static RoR2.CombatDirector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace PerfectedLoopTri
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.elites", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.PerfectedLoop3";
		public const string MODNAME = "PerfectedLoop3";
		public const string MODVERSION = "1.0.0";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		internal static PluginInfo pluginInfo;

		private int Perfected_StageCount = 5;
		private float Perfected_Cost = 6f;
		private float Perfected_Health = 2f;
		private float Perfected_Damage = 2f;

		private bool HonorGilded_Enable = true;
		private int HonorGilded_StageCount = 3;
		private float HonorGilded_Cost = 3.5f;
		private float HonorGilded_Health = 3.75f;
		private float HonorGilded_Damage = 2.25f;

		private EliteDef perfectedDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/Base/EliteLunar/edLunar.asset").WaitForCompletion();
		public EliteDef GildedHonor = null;
		public CombatDirector.EliteTierDef GildedHonorTier = null;

		public void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			ReadConfigs();
			if (HonorGilded_Enable)
            {
				CreateHonorGilded();
            }
			if (perfectedDef)
            {
				UpdatePerfected();
				if (Perfected_StageCount > -1 || Perfected_Cost != 6f)
				{
					On.RoR2.CombatDirector.Init += OnInit;
				}
			}
		}
		private void UpdatePerfected()
        {
			perfectedDef.healthBoostCoefficient = Perfected_Health;
			perfectedDef.damageBoostCoefficient = Perfected_Damage;
		}
		private void OnInit(On.RoR2.CombatDirector.orig_Init orig)
        {
			orig();
			if (perfectedDef)
			{
				EliteTierDef lunarDef = EliteAPI.VanillaEliteTiers[5];
				if (lunarDef != null)
                {
					lunarDef.costMultiplier = Perfected_Cost;
					lunarDef.isAvailable = (SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Lunar || (Run.instance.stageClearCount > Perfected_StageCount-1 && rules == SpawnCard.EliteRules.Default);
				}
			}
		}
		private void CreateHonorGilded()
        {
			EliteDef refDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/DLC2/Elites/EliteAurelionite/edAurelionite.asset").WaitForCompletion();
			GildedHonor = ScriptableObject.CreateInstance<EliteDef>();
			GildedHonor.name = "edAurelioniteHonor";
			GildedHonor.modifierToken = refDef.modifierToken;
			GildedHonor.shaderEliteRampIndex = refDef.shaderEliteRampIndex;
			GildedHonor.eliteEquipmentDef = refDef.eliteEquipmentDef;
			GildedHonor.color = refDef.color;
			GildedHonor.healthBoostCoefficient = HonorGilded_Health;
			GildedHonor.damageBoostCoefficient = HonorGilded_Damage;

			CombatDirector.EliteTierDef[] eliteTierDefs = new CombatDirector.EliteTierDef[1];

			GildedHonorTier = new CombatDirector.EliteTierDef();
			GildedHonorTier.eliteTypes = new EliteDef[]
			{
				GildedHonor
			};
			GildedHonorTier.costMultiplier = HonorGilded_Cost;
			GildedHonorTier.canSelectWithoutAvailableEliteDef = false;
			GildedHonorTier.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive() && Run.instance.stageClearCount >= HonorGilded_StageCount-1;
			eliteTierDefs[0] = GildedHonorTier;
			R2API.CustomElite customHGilded = new R2API.CustomElite(GildedHonor, eliteTierDefs);
			R2API.EliteAPI.Add(customHGilded);
			R2API.EliteAPI.AddCustomEliteTier(GildedHonorTier);
		}
		private void ReadConfigs()
        {
			Perfected_StageCount = Config.Bind("Perfected", "Stage Count", 5, "Allows Perfected Elites to spawn on other stages after this many stages. (-1 = Disabled)").Value;
			Perfected_Cost = Config.Bind("Perfected", "Cost", 6f, "Director cost for spawning. (6 = Vanilla)").Value;
			Perfected_Health = Config.Bind("Perfected", "Health", 2f, "Health multiplier. (2 = Vanilla)").Value;
			Perfected_Damage = Config.Bind("Perfected", "Damage", 2f, "Damage multiplier. (2 = Vanilla)").Value;

			HonorGilded_Enable = Config.Bind("Gilded Honor", "Enable", true, "Enables Gilded Elites to appear during Artifact of Honor.").Value;
			HonorGilded_Cost = Config.Bind("Gilded Honor", "Cost", 3.5f, "Director cost for spawning. (3.5 = Vanilla)").Value;
			HonorGilded_Health = Config.Bind("Gilded Honor", "Health", 3.75f, "Health multiplier. (6 = Vanilla)").Value;
			HonorGilded_Damage = Config.Bind("Gilded Honor", "Damage", 2.25f, "Damage multiplier. (3 = Vanilla)").Value;
			HonorGilded_StageCount = Config.Bind("Gilded Honor", "Stage Count", 3, "The minimun stage count before spawning. (3 = Vanilla)").Value;
		}
	}
}
