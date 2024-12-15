using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
	public enum ConfigAddTo : int
	{
		Disable = 0,
		TierOne = 1,
		TierGilded = 2,
		TierTwo = 3,
	}

	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.content_management", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.bepis.r2api.elites", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class PerfectedLoopTri : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.PerfectedLoop3";
		public const string MODNAME = "PerfectedLoop3";
		public const string MODVERSION = "1.0.3";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		internal static PluginInfo pluginInfo;

		private float Tier_Lunar_Cost = 6f;
		private float Lunar_Perfected_Health = 2f;
		private float Lunar_Perfected_Damage = 2f;

		private bool Tier_Gilded_AllowAny = false;
		private float Tier_Gilded_Cost = 6f;
		private float Tier_Gilded_CostHonor = 3f;
		private int Tier_Gilded_StartStageCount = 3;
		private bool Tier_Gilded_Clear = false;

		private int Tier_One_EndStageCount = 0;

		private ConfigAddTo Gilded_Tier = ConfigAddTo.TierGilded;
		private float Gilded_Health = 6f;
		private float Gilded_Damage = 3f;
		private float GildedHonor_Health = 6f;
		private float GildedHonor_Damage = 3f;

		private ConfigAddTo Mod_Perfected_Tier = ConfigAddTo.TierTwo;
		private float Mod_Perfected_Health = 14.4f;
		private float Mod_Perfected_Damage = 6f;
		private float Mod_PerfectedHonor_Health = 14.4f;
		private float Mod_PerfectedHonor_Damage = 6f;

		private ConfigAddTo Mod_Void_Tier = ConfigAddTo.Disable;
		private float Mod_Void_Health = 1f;
		private float Mod_Void_Damage = 1f;
		private float Mod_VoidHonor_Health = 1f;
		private float Mod_VoidHonor_Damage = 1f;

		private EliteDef Base_VoidDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/DLC1/EliteVoid/edVoid.asset").WaitForCompletion();
		private EliteDef Base_PerfectedDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/Base/EliteLunar/edLunar.asset").WaitForCompletion();
		private EliteDef Base_GildedDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/DLC2/Elites/EliteAurelionite/edAurelionite.asset").WaitForCompletion();
		private EliteDef Base_GildedHonorDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/DLC2/Elites/EliteAurelionite/edAurelioniteHonor.asset").WaitForCompletion();
		public EliteDef Custom_VoidDef = null;
		public EliteDef Custom_VoidHonorDef = null;
		public EliteDef Custom_PerfectedDef = null;
		public EliteDef Custom_PerfectedHonorDef = null;

		public void Awake()
		{
			ModLogger = this.Logger;
			pluginInfo = Info;
			ReadConfigs();
			UpdatePerfected();
			UpdateGilded();
			if (Mod_Perfected_Tier != ConfigAddTo.Disable)
			{
				CreatePerfected();
			}
			if (Mod_Void_Tier != ConfigAddTo.Disable)
			{
				CreateVoid();
			}
			On.RoR2.CombatDirector.Init += OnInit;
		}
	
		private void OnInit(On.RoR2.CombatDirector.orig_Init orig)
        {
			orig();
			//Gilded Tier
			EliteTierDef T1Def = EliteAPI.VanillaEliteTiers[1];
			if (T1Def != null)
			{
				List<EliteDef> eliteTypes = T1Def.eliteTypes.ToList();
				if (Mod_Perfected_Tier == ConfigAddTo.TierOne)
				{
					eliteTypes.Add(Custom_PerfectedDef);
				}
				if (Mod_Void_Tier == ConfigAddTo.TierOne)
				{
					eliteTypes.Add(Custom_VoidDef);
				}
				if (Gilded_Tier == ConfigAddTo.TierOne)
				{
					eliteTypes.Add(Base_GildedDef);
				}
				if (Tier_One_EndStageCount != 0)
                {
					if (Tier_One_EndStageCount < 0)
                    {
						T1Def.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive() && rules == SpawnCard.EliteRules.Default;
					}
					else
                    {
						T1Def.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive() && rules == SpawnCard.EliteRules.Default && Run.instance.stageClearCount + 1 <= Tier_One_EndStageCount;
					}
                }
				T1Def.eliteTypes = eliteTypes.ToArray();
			}
			EliteTierDef T1HonorDef = EliteAPI.VanillaEliteTiers[2];
			if (T1HonorDef != null)
			{
				List<EliteDef> eliteTypes = T1HonorDef.eliteTypes.ToList();
				if (Mod_Perfected_Tier == ConfigAddTo.TierOne)
				{
					eliteTypes.Add(Custom_PerfectedHonorDef);
				}
				if (Mod_Void_Tier == ConfigAddTo.TierOne)
				{
					eliteTypes.Add(Custom_VoidHonorDef);
				}
				if (Gilded_Tier == ConfigAddTo.TierOne)
				{
					eliteTypes.Add(Base_GildedHonorDef);
				}
				if (Tier_One_EndStageCount != 0)
				{
					if (Tier_One_EndStageCount < 0)
					{
						T1HonorDef.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive();
					}
					else
					{
						T1HonorDef.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive() && Run.instance.stageClearCount + 1 <= Tier_One_EndStageCount;
					}
				}
				T1HonorDef.eliteTypes = eliteTypes.ToArray();
			}
			EliteTierDef gildedHonorTierDef = EliteAPI.VanillaEliteTiers[3];
			if (gildedHonorTierDef != null)
			{
				List<EliteDef> eliteTypes = gildedHonorTierDef.eliteTypes.ToList();
				if (Tier_Gilded_Clear)
				{
					//From EliteReworks
					List<EliteDef> blackList = new List<EliteDef>()
					{
						RoR2Content.Elites.FireHonor,
						RoR2Content.Elites.LightningHonor,
						RoR2Content.Elites.IceHonor,
						DLC1Content.Elites.EarthHonor,
						DLC2Content.Elites.AurelioniteHonor
					};
					foreach (EliteDef eliteDef in blackList)
					{
						eliteTypes.Remove(eliteDef);
					}
				}
				if (Mod_Perfected_Tier == ConfigAddTo.TierGilded)
				{
					eliteTypes.Add(Custom_PerfectedHonorDef);
				}
				if (Mod_Void_Tier == ConfigAddTo.TierGilded)
				{
					eliteTypes.Add(Custom_VoidHonorDef);
				}
				if (Gilded_Tier == ConfigAddTo.TierGilded)
				{
					eliteTypes.Add(Base_GildedHonorDef);
				}
				gildedHonorTierDef.eliteTypes = eliteTypes.ToArray();
				if (Tier_Gilded_AllowAny)
				{
					gildedHonorTierDef.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive() && Run.instance.stageClearCount + 1 >= Tier_Gilded_StartStageCount;
				}
				else
				{
					gildedHonorTierDef.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive() && rules == SpawnCard.EliteRules.Default && Run.instance.stageClearCount + 1 >= Tier_Gilded_StartStageCount;
				}
				gildedHonorTierDef.costMultiplier = Tier_Gilded_CostHonor;
			}
			EliteTierDef gildedTierDef = EliteAPI.VanillaEliteTiers[4];
			if (gildedTierDef != null)
			{
				List<EliteDef> eliteTypes = gildedTierDef.eliteTypes.ToList();
				if (Tier_Gilded_Clear)
				{
					//From EliteReworks
					List<EliteDef> blackList = new List<EliteDef>()
					{
						RoR2Content.Elites.Fire,
						RoR2Content.Elites.Lightning,
						RoR2Content.Elites.Ice,
						DLC1Content.Elites.Earth,
						DLC2Content.Elites.Aurelionite
					};
					foreach (EliteDef eliteDef in blackList)
                    {
						eliteTypes.Remove(eliteDef);
					}
				}
				if (Mod_Perfected_Tier == ConfigAddTo.TierGilded)
				{
					eliteTypes.Add(Custom_PerfectedDef);
				}
				if (Mod_Void_Tier == ConfigAddTo.TierGilded)
				{
					eliteTypes.Add(Custom_VoidDef);
				}
				if (Gilded_Tier == ConfigAddTo.TierGilded)
				{
					eliteTypes.Add(Base_GildedDef);
				}
				gildedTierDef.eliteTypes = eliteTypes.ToArray();
				gildedTierDef.isAvailable = (SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive() && rules == SpawnCard.EliteRules.Default && Run.instance.stageClearCount + 1 >= Tier_Gilded_StartStageCount;
				gildedTierDef.costMultiplier = Tier_Gilded_Cost;
			}
			//T2 Tier
			EliteTierDef T2Def = EliteAPI.VanillaEliteTiers[5];
			if (T2Def != null)
			{
				List<EliteDef> eliteTypes = T2Def.eliteTypes.ToList();
				if (Mod_Perfected_Tier == ConfigAddTo.TierTwo)
				{
					eliteTypes.Add(Custom_PerfectedDef);
				}
				if (Mod_Void_Tier == ConfigAddTo.TierTwo)
				{
					eliteTypes.Add(Custom_VoidDef);
				}
				if (Gilded_Tier == ConfigAddTo.TierTwo)
				{
					eliteTypes.Add(Base_GildedDef);
				}
				T2Def.eliteTypes = eliteTypes.ToArray();
			}
			//Lunar Tier
			EliteTierDef lunarDef = EliteAPI.VanillaEliteTiers[6];
			if (lunarDef != null)
			{
				lunarDef.costMultiplier = Tier_Lunar_Cost;
			}
		}
		private void UpdateGilded()
        {
			Base_GildedDef.healthBoostCoefficient = Gilded_Health;
			Base_GildedDef.damageBoostCoefficient = Gilded_Damage;
			Base_GildedHonorDef.healthBoostCoefficient = GildedHonor_Health;
			Base_GildedHonorDef.damageBoostCoefficient = GildedHonor_Damage;
		}
		private void UpdatePerfected()
		{
			Base_PerfectedDef.healthBoostCoefficient = Lunar_Perfected_Health;
			Base_PerfectedDef.damageBoostCoefficient = Lunar_Perfected_Damage;
		}
		private void CreateVoid()
		{
			Custom_VoidDef = ScriptableObject.CreateInstance<EliteDef>();
			Custom_VoidDef.name = "edVoidTri";
			Custom_VoidDef.modifierToken = Base_VoidDef.modifierToken;
			Custom_VoidDef.shaderEliteRampIndex = Base_VoidDef.shaderEliteRampIndex;
			Custom_VoidDef.eliteEquipmentDef = Base_VoidDef.eliteEquipmentDef;
			Custom_VoidDef.color = Base_VoidDef.color;
			Custom_VoidDef.healthBoostCoefficient = Mod_Void_Health;
			Custom_VoidDef.damageBoostCoefficient = Mod_Void_Damage;

			EliteTierDef[] eliteTierDef = new EliteTierDef[1];
			eliteTierDef[0] = new EliteTierDef();
			eliteTierDef[0].eliteTypes = new EliteDef[]
			{
				Custom_VoidDef
			};
			R2API.CustomElite customElite = new R2API.CustomElite(Custom_VoidDef, eliteTierDef);
			R2API.EliteAPI.Add(customElite);

			if (Mod_Void_Tier != ConfigAddTo.TierTwo)
            {
				Custom_VoidHonorDef = ScriptableObject.CreateInstance<EliteDef>();
				Custom_VoidHonorDef.name = "edVoidHonorTri";
				Custom_VoidHonorDef.modifierToken = Base_VoidDef.modifierToken;
				Custom_VoidHonorDef.shaderEliteRampIndex = Base_VoidDef.shaderEliteRampIndex;
				Custom_VoidHonorDef.eliteEquipmentDef = Base_VoidDef.eliteEquipmentDef;
				Custom_VoidHonorDef.color = Base_VoidDef.color;
				Custom_VoidHonorDef.healthBoostCoefficient = Mod_VoidHonor_Health;
				Custom_VoidHonorDef.damageBoostCoefficient = Mod_VoidHonor_Damage;

				EliteTierDef[] eliteHonorTierDef = new EliteTierDef[1];
				eliteHonorTierDef[0] = new EliteTierDef();
				eliteHonorTierDef[0].eliteTypes = new EliteDef[]
				{
					Custom_VoidHonorDef
				};
				R2API.CustomElite customHonorElite = new R2API.CustomElite(Custom_VoidHonorDef, eliteHonorTierDef);
				R2API.EliteAPI.Add(customHonorElite);
			}
		}
		private void CreatePerfected()
        {
			Custom_PerfectedDef = ScriptableObject.CreateInstance<EliteDef>();
			Custom_PerfectedDef.name = "edLunarTri";
			Custom_PerfectedDef.modifierToken = Base_PerfectedDef.modifierToken;
			Custom_PerfectedDef.shaderEliteRampIndex = Base_PerfectedDef.shaderEliteRampIndex;
			Custom_PerfectedDef.eliteEquipmentDef = Base_PerfectedDef.eliteEquipmentDef;
			Custom_PerfectedDef.color = Base_PerfectedDef.color;
			Custom_PerfectedDef.healthBoostCoefficient = Mod_Perfected_Health;
			Custom_PerfectedDef.damageBoostCoefficient = Mod_Perfected_Damage;

			EliteTierDef[] eliteTierDef = new EliteTierDef[1];
			eliteTierDef[0] = new EliteTierDef();
			eliteTierDef[0].eliteTypes = new EliteDef[]
			{
				Custom_PerfectedDef
			};
			R2API.CustomElite customElite = new R2API.CustomElite(Custom_PerfectedDef, eliteTierDef);
			R2API.EliteAPI.Add(customElite);

			if (Mod_Perfected_Tier != ConfigAddTo.TierTwo)
			{
				Custom_PerfectedHonorDef = ScriptableObject.CreateInstance<EliteDef>();
				Custom_PerfectedHonorDef.name = "edLunarHonorTri";
				Custom_PerfectedHonorDef.modifierToken = Base_PerfectedDef.modifierToken;
				Custom_PerfectedHonorDef.shaderEliteRampIndex = Base_PerfectedDef.shaderEliteRampIndex;
				Custom_PerfectedHonorDef.eliteEquipmentDef = Base_PerfectedDef.eliteEquipmentDef;
				Custom_PerfectedHonorDef.color = Base_PerfectedDef.color;
				Custom_PerfectedHonorDef.healthBoostCoefficient = Mod_PerfectedHonor_Health;
				Custom_PerfectedHonorDef.damageBoostCoefficient = Mod_PerfectedHonor_Damage;

				EliteTierDef[] eliteHonorTierDef = new EliteTierDef[1];
				eliteHonorTierDef[0] = new EliteTierDef();
				eliteHonorTierDef[0].eliteTypes = new EliteDef[]
				{
					Custom_PerfectedHonorDef
				};
				R2API.CustomElite customHonorElite = new R2API.CustomElite(Custom_PerfectedHonorDef, eliteHonorTierDef);
				R2API.EliteAPI.Add(customHonorElite);
			}
		}
		private void ReadConfigs()
        {
			Tier_Lunar_Cost = Config.Bind("Elite Tier - Lunar", "Cost", 6f, "Director cost for spawning. (6 = Vanilla)").Value;
			Lunar_Perfected_Health = Config.Bind("Elite - Vanilla Perfected", "Health", 2f, "Health multiplier. (2 = Vanilla) (Get x1.25 Shield from the Aspect)").Value;
			Lunar_Perfected_Damage = Config.Bind("Elite - Vanilla Perfected", "Damage", 2f, "Damage multiplier. (2 = Vanilla)").Value;

			Tier_One_EndStageCount = Config.Bind("Elite Tier - One", "Stage End Count", 0, "The minimun stage count that this elite tier stops being used on. (0 = Don't Change, -1 = Never Stop)").Value;

			Tier_Gilded_AllowAny = Config.Bind("Elite Tier - Gilded", "Allow Any", true, "Allows most enemies to have affixes from this Elite Tier with Honor enabled. (True would be closer to T1 Elite behavior, False would be closer to T2 Elite.)").Value;
			Tier_Gilded_Cost = Config.Bind("Elite Tier - Gilded", "Cost", 6f, "Director cost for spawning. (6 = Vanilla)").Value;
			Tier_Gilded_CostHonor = Config.Bind("Elite Tier - Gilded", "Honor Cost", 3f, "Director cost for spawning. (3 = Vanilla)").Value;
			Tier_Gilded_StartStageCount = Config.Bind("Elite Tier - Gilded", "Stage Count", 3, "The minimun stage count that this elite tier becomes active on. (3 = Vanilla)").Value;
			Tier_Gilded_Clear = Config.Bind("Elite Tier - Gilded", "Clear Tier", false, "Removes Blazing, Glacial, Overloading, Mending and Gilded from this Elite Tier and its Honor version.").Value;

			Gilded_Tier = Config.Bind("Elite - Gilded", "Elite Tier", ConfigAddTo.TierGilded, "Add to which elite pool?").Value;
			Gilded_Health = Config.Bind("Elite - Gilded", "Health", 5f, "Health multiplier. (5 = Vanilla)").Value;
			Gilded_Damage = Config.Bind("Elite - Gilded", "Damage", 2.5f, "Damage multiplier. (2.5 = Vanilla)").Value;
			GildedHonor_Health = Config.Bind("Elite - Gilded", "Honor Health", 3.5f, "Health multiplier. (3.5 = Vanilla)").Value;
			GildedHonor_Damage = Config.Bind("Elite - Gilded", "Honor Damage", 2f, "Damage multiplier. (2 = Vanilla)").Value;

			Mod_Perfected_Tier = Config.Bind("Elite - Modded Perfected", "Elite Tier", ConfigAddTo.TierTwo, "Add to which elite pool?").Value;
			Mod_Perfected_Health = Config.Bind("Elite - Modded Perfected", "Health", 14.4f, "Health multiplier. (2 = Vanilla) (Gets x1.25 Shield from the Aspect)").Value;
			Mod_Perfected_Damage = Config.Bind("Elite - Modded Perfected", "Damage", 6f, "Damage multiplier. (2 = Vanilla)").Value;
			Mod_PerfectedHonor_Health = Config.Bind("Elite - Modded Perfected", "Honor Health", 14.4f, "Health multiplier. (2 = Vanilla) (Gets x1.25 Shield from the Aspect)").Value;
			Mod_PerfectedHonor_Damage = Config.Bind("Elite - Modded Perfected", "Honor Damage", 6f, "Damage multiplier. (2 = Vanilla)").Value;

			Mod_Void_Tier = Config.Bind("Elite - Voidtouched", "Elite Tier", ConfigAddTo.Disable, "Add to which elite pool?").Value;
			Mod_Void_Health = Config.Bind("Elite - Voidtouched", "Health", 1f, "Health multiplier. (Gets x1.5 Health from the Aspect)").Value;
			Mod_Void_Damage = Config.Bind("Elite - Voidtouched", "Damage", 1f, "Damage multiplier. (Gets x0.7 Damage from the Aspect)").Value;
			Mod_VoidHonor_Health = Config.Bind("Elite - Voidtouched", "Honor Health", 1f, "Health multiplier. (Gets x1.5 Health from the Aspect)").Value;
			Mod_VoidHonor_Damage = Config.Bind("Elite - Voidtouched", "Honor Damage", 1f, "Damage multiplier. (Gets x0.7 Damage from the Aspect)").Value;
		}
	}
}
