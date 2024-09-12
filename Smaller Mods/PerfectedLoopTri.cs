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
		TierGilded = 1,
		TierTwo = 2,
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
		public const string MODVERSION = "1.0.1";

		internal static BepInEx.Logging.ManualLogSource ModLogger;
		internal static PluginInfo pluginInfo;

		private float Tier_Lunar_Cost = 6f;
		private float Lunar_Perfected_Health = 2f;
		private float Lunar_Perfected_Damage = 2f;

		private bool Tier_Gilded_AllowAny = false;
		private float Tier_Gilded_Cost = 3.5f;
		private int Tier_Gilded_StageCount = 3;
		private bool Tier_Gilded_Fix = true;

		private ConfigAddTo Gilded_Tier = ConfigAddTo.TierGilded;
		private float Gilded_Health = 6f;
		private float Gilded_Damage = 3f;

		private ConfigAddTo Mod_Perfected_Tier = ConfigAddTo.TierTwo;
		private float Mod_Perfected_Health = 14.4f;
		private float Mod_Perfected_Damage = 6f;

		private ConfigAddTo Mod_Void_Tier = ConfigAddTo.Disable;
		private float Mod_Void_Health = 1f;
		private float Mod_Void_Damage = 1f;

		private bool Director_Enable = false;

		private EliteDef voidDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/DLC1/EliteVoid/edVoid.asset").WaitForCompletion();
		private EliteDef perfectedDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/Base/EliteLunar/edLunar.asset").WaitForCompletion();
		private EliteDef gildedDef = Addressables.LoadAssetAsync<EliteDef>("RoR2/DLC2/Elites/EliteAurelionite/edAurelionite.asset").WaitForCompletion();
		public EliteDef Custom_VoidDef = null;
		public EliteDef Custom_PerfectedDef = null;

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
			if (Director_Enable)
            {
				On.RoR2.CombatDirector.ResetEliteType += ResetEliteType;
				On.RoR2.CombatDirector.PrepareNewMonsterWave += OnSelectEliteTier;
            }
		}
		
		private void ResetEliteType(On.RoR2.CombatDirector.orig_ResetEliteType orig, CombatDirector self)
		{
			List<EliteTierDef> anyDefs = new List<EliteTierDef>();
			EliteTierDef cheapestTierDef = null;
			float cheapestDefCost = 9999f;
			self.currentActiveEliteTier = CombatDirector.eliteTiers[0];
			for (int i = 0; i < CombatDirector.eliteTiers.Length; i++)
			{
				CombatDirector.EliteTierDef eliteTierDef = CombatDirector.eliteTiers[i];
				if (eliteTierDef.CanSelect(self.currentMonsterCard.spawnCard.eliteRules))
				{
					if (eliteTierDef.costMultiplier < cheapestDefCost)
                    {
						cheapestTierDef = eliteTierDef;
						cheapestDefCost = eliteTierDef.costMultiplier;
					}
					anyDefs.Add(eliteTierDef);
				}
			}
			self.currentActiveEliteTier = cheapestTierDef;
			if (anyDefs.Count > 0)
            {
				List<EliteTierDef> finalDefs = new List<EliteTierDef>();
				for (int i = 0; i<anyDefs.Count;i++)
                {
					CombatDirector.EliteTierDef eliteTierDef = anyDefs[i];
					if (eliteTierDef.costMultiplier <= cheapestDefCost)
                    {
						int addAmount = 0;
						if (eliteTierDef.canSelectWithoutAvailableEliteDef)
						{
							addAmount += 1;
						}
						if (eliteTierDef.availableDefs != null)
						{
							addAmount += eliteTierDef.eliteTypes.Length;
						}
						while (addAmount > 0)
						{
							finalDefs.Add(eliteTierDef);
							addAmount--;
						}
					}
                }
				if (finalDefs.Count > 0)
                {
					self.currentActiveEliteTier = self.rng.NextElementUniform<EliteTierDef>(finalDefs);
				}
			}
			self.currentActiveEliteDef = self.currentActiveEliteTier.GetRandomAvailableEliteDef(self.rng);
		}
		private void OnSelectEliteTier(On.RoR2.CombatDirector.orig_PrepareNewMonsterWave orig, CombatDirector self, DirectorCard monsterCard)
		{
			//orig(self, monsterCard);
			self.currentMonsterCard = monsterCard;
			self.ResetEliteType();
			if (!(self.currentMonsterCard.spawnCard as CharacterSpawnCard).noElites)
			{
				EliteTierDef mostExpensiveDef = null;
				List<EliteTierDef> validDefs = new List<EliteTierDef>();
				List<EliteTierDef> selectDefs = new List<EliteTierDef>();
				for (int i = 1; i < CombatDirector.eliteTiers.Length; i++)
				{
					CombatDirector.EliteTierDef eliteTierDef = CombatDirector.eliteTiers[i];
					if (eliteTierDef.CanSelect(self.currentMonsterCard.spawnCard.eliteRules))
					{
						float num = self.currentMonsterCard.cost * eliteTierDef.costMultiplier * self.eliteBias;
						if (num <= self.monsterCredit)
						{
							//This is closer to the original behavior but it ignores array priority.
							if (mostExpensiveDef != null)
                            {
								if (mostExpensiveDef.costMultiplier < eliteTierDef.costMultiplier)
                                {
									mostExpensiveDef = eliteTierDef;
								}
                            }
							else
                            {
								mostExpensiveDef = eliteTierDef;
							}
							validDefs.Add(eliteTierDef);
						}
					}
				}
				if (mostExpensiveDef != null)
                {
					float bestCredit = mostExpensiveDef.costMultiplier;
					if (validDefs.Count > 1)
                    {
						//Now get all valid defs that are equal or higher than our most expensive.
						for (int i = 0; i < validDefs.Count; i++)
						{
							CombatDirector.EliteTierDef eliteTierDef = validDefs[i];
							if (eliteTierDef.costMultiplier >= bestCredit)
							{
								int addAmount = 0;
								if (eliteTierDef.canSelectWithoutAvailableEliteDef)
								{
									addAmount += 1;
								}
								if (eliteTierDef.availableDefs != null)
								{
									addAmount += eliteTierDef.eliteTypes.Length;
								}
								while (addAmount > 0)
								{
									selectDefs.Add(eliteTierDef);
									addAmount--;
								}
							}
						}
					}
					if (selectDefs.Count > 0)
                    {
						self.currentActiveEliteTier = self.rng.NextElementUniform<EliteTierDef>(selectDefs);
					}
					else
                    {
						self.currentActiveEliteTier = mostExpensiveDef;
					}
				}
			}
			self.currentActiveEliteDef = self.currentActiveEliteTier.GetRandomAvailableEliteDef(self.rng);
			self.lastAttemptedMonsterCard = self.currentMonsterCard;
			self.spawnCountInCurrentWave = 0;
		}
		private void OnInit(On.RoR2.CombatDirector.orig_Init orig)
        {
			orig();
			//Gilded Tier
			EliteTierDef gildedTierDef = EliteAPI.VanillaEliteTiers[3];
			if (gildedTierDef != null)
			{
				if (Tier_Gilded_Fix)
				{
					List<EliteDef> eliteTypes = gildedTierDef.eliteTypes.ToList();
					//From EliteReworks
					List<EliteDef> blackList = new List<EliteDef>()
					{
						RoR2Content.Elites.Fire,
						RoR2Content.Elites.Lightning,
						RoR2Content.Elites.Ice,
						DLC1Content.Elites.Earth
					};
					foreach (EliteDef eliteDef in blackList)
                    {
						eliteTypes.Remove(eliteDef);
					}
					//
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
						eliteTypes.Add(gildedDef);
					}
					gildedTierDef.eliteTypes = eliteTypes.ToArray();
				}
				if (Tier_Gilded_AllowAny)
				{
					gildedTierDef.isAvailable = (SpawnCard.EliteRules rules) => Run.instance.stageClearCount + 1 >= Tier_Gilded_StageCount;
				}
				else
				{
					gildedTierDef.isAvailable = (SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Default && Run.instance.stageClearCount + 1 >= Tier_Gilded_StageCount;
				}
				gildedTierDef.costMultiplier = Tier_Gilded_Cost;
			}
			//T2 Tier
			EliteTierDef T2Def = EliteAPI.VanillaEliteTiers[4];
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
					eliteTypes.Add(gildedDef);
				}
				T2Def.eliteTypes = eliteTypes.ToArray();
			}
			//Lunar Tier
			EliteTierDef lunarDef = EliteAPI.VanillaEliteTiers[5];
			if (lunarDef != null)
			{
				lunarDef.costMultiplier = Tier_Lunar_Cost;
			}
		}
		private void UpdateGilded()
        {
			perfectedDef.healthBoostCoefficient = Gilded_Health;
			perfectedDef.damageBoostCoefficient = Gilded_Damage;
		}
		private void UpdatePerfected()
		{
			perfectedDef.healthBoostCoefficient = Lunar_Perfected_Health;
			perfectedDef.damageBoostCoefficient = Lunar_Perfected_Damage;
		}
		private void CreateVoid()
		{
			Custom_VoidDef = ScriptableObject.CreateInstance<EliteDef>();
			Custom_VoidDef.name = "edVoidTri";
			Custom_VoidDef.modifierToken = voidDef.modifierToken;
			Custom_VoidDef.shaderEliteRampIndex = voidDef.shaderEliteRampIndex;
			Custom_VoidDef.eliteEquipmentDef = voidDef.eliteEquipmentDef;
			Custom_VoidDef.color = voidDef.color;
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
		}
		private void CreatePerfected()
        {
			Custom_PerfectedDef = ScriptableObject.CreateInstance<EliteDef>();
			Custom_PerfectedDef.name = "edLunarTri";
			Custom_PerfectedDef.modifierToken = perfectedDef.modifierToken;
			Custom_PerfectedDef.shaderEliteRampIndex = perfectedDef.shaderEliteRampIndex;
			Custom_PerfectedDef.eliteEquipmentDef = perfectedDef.eliteEquipmentDef;
			Custom_PerfectedDef.color = perfectedDef.color;
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
		}
		private void ReadConfigs()
        {
			Tier_Lunar_Cost = Config.Bind("Elite Tier - Lunar", "Cost", 6f, "Director cost for spawning. (6 = Vanilla)").Value;
			Lunar_Perfected_Health = Config.Bind("Elite - Vanilla Perfected", "Health", 2f, "Health multiplier. (2 = Vanilla) (Get x1.25 Shield from the Aspect)").Value;
			Lunar_Perfected_Damage = Config.Bind("Elite - Vanilla Perfected", "Damage", 2f, "Damage multiplier. (2 = Vanilla)").Value;

			Tier_Gilded_AllowAny = Config.Bind("Elite Tier - Gilded", "Allow Any", false, "Allows most enemies to spawn as this Elite. (True would be closer to T1 Elite behavior, False would be closer to T2 Elite.)").Value;
			Tier_Gilded_Cost = Config.Bind("Elite Tier - Gilded", "Cost", 9f, "Director cost for spawning. (3.5 = Vanilla)").Value;
			Tier_Gilded_StageCount = Config.Bind("Elite Tier - Gilded", "Stage Count", 3, "The minimun stage count that this elite tier becomes active on. (3 = Vanilla)").Value;
			Tier_Gilded_Fix = Config.Bind("Elite Tier - Gilded", "Fix Tier", true, "Removes Blazing, Glacial, Overloading and Mending from this elite tier.").Value;

			Gilded_Tier = Config.Bind("Elite - Gilded", "Elite Tier", ConfigAddTo.TierGilded, "Add to which elite pool?").Value;
			Gilded_Health = Config.Bind("Elite - Gilded", "Health", 6f, "Health multiplier. (6 = Vanilla)").Value;
			Gilded_Damage = Config.Bind("Elite - Gilded", "Damage", 3f, "Damage multiplier. (3 = Vanilla)").Value;

			Mod_Perfected_Tier = Config.Bind("Elite - Modded Perfected", "Elite Tier", ConfigAddTo.TierTwo, "Add to which elite pool?").Value;
			Mod_Perfected_Health = Config.Bind("Elite - Modded Perfected", "Health", 14.4f, "Health multiplier. (2 = Vanilla) (Gets x1.25 Shield from the Aspect)").Value;
			Mod_Perfected_Damage = Config.Bind("Elite - Modded Perfected", "Damage", 6f, "Damage multiplier. (2 = Vanilla)").Value;

			Mod_Void_Tier = Config.Bind("Elite - Voidtouched", "Elite Tier", ConfigAddTo.Disable, "Add to which elite pool?").Value;
			Mod_Void_Health = Config.Bind("Elite - Voidtouched", "Health", 1f, "Health multiplier. (Gets x1.5 Health from the Aspect)").Value;
			Mod_Void_Damage = Config.Bind("Elite - Voidtouched", "Damage", 1f, "Damage multiplier. (Gets x0.7 Damage from the Aspect)").Value;

			Director_Enable = Config.Bind("Elite Director", "Enable", false, "Enables changes to how the Director selects elites to improve diversity. Mostly affects setups with many elite tiers with similar costs.").Value;
		}
	}
}
