using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.CharacterAI;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace DevotionConfig.Changes
{
	public class HeavyChanges
	{
		internal static bool Enable = true;

		internal static string Evo_HPStages_Raw;
		internal static string Evo_DmgStages_Raw;
		internal static string Evo_RegenStages_Raw;

		internal static float[] Evo_HPStages;
		internal static float[] Evo_DmgStages;
		internal static float[] Evo_RegenStages;

		internal static int EvoMax = -1;

		internal static string EliteList_Raw;
		internal static int[] EliteList;
		internal static List<int[]> EliteAspects = new List<int[]>();

		internal static string Evo_BodyStages_Raw;
		internal static List<BodyEvolutionStage> Evo_BodyStages;

		internal static string Evo_EliteStages_Raw;
		internal static List<EliteEvolutionStage> Evo_EliteStages;

		internal static string Evo_BaseMaster_Raw = "DevotedLemurianMaster";
		internal static GameObject Evo_BaseMaster;

		internal static string Evo_BaseBody_Raw = "LemurianBody";

		internal static int Death_Penalty = 0;
		internal static bool Death_EggDrop = false;

		internal static bool EvoLevelTrack = false;
		internal static float Misc_LeashDistance = 400f;

		private static string SFX_Evolve = "Play_obj_devotion_egg_evolve";
		internal static SpawnCard EggSpawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/CU8/LemurianEgg/iscLemurianEgg.asset").WaitForCompletion();
		internal static GameObject VFX_Hatch = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/LemurianEggHatching");
		
		public HeavyChanges()
		{
			if (!Enable)
            {
				return;
            }
			ClampConfig();
			Hooks();
		}
		private void ClampConfig()
		{
			EvoMax -= 1;
			Math.Max(0, Death_Penalty);
			Math.Min(2, Death_Penalty);
		}
		private void Hooks()
		{
			On.RoR2.CharacterAI.LemurianEggController.SummonLemurian += OnHatchEgg;
			On.RoR2.DevotionInventoryController.UpdateAllMinions += UpdateAllMinions;
			On.RoR2.DevotionInventoryController.UpdateMinionInventory += UpdateMinionInventory;
			On.RoR2.DevotionInventoryController.EvolveDevotedLumerian += Hook_EvolveDevotedLumerian;
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			if (EvoLevelTrack)
            {
				On.RoR2.CharacterBody.GetDisplayName += GetDisplayName;
			}
			Run.onRunStartGlobal += OnRunStart;
			if (Death_Penalty > 0)
            {
				On.DevotedLemurianController.OnDevotedBodyDead += OnDevotedBodyDead_Penalty;
			}
			else
            {
				On.DevotedLemurianController.OnDevotedBodyDead += OnDevotedBodyDead_EggDrop;
			}
			if (Misc_LeashDistance != 400f)
            {
				On.DevotedLemurianController.CheckIfNeedTeleport += CheckIfNeedTeleport;
            }
		}
		internal static void PostLoad()
		{
			if (Enable)
			{
				float[,] hpData = new float[999, 2];
				string[] itemString = Evo_HPStages_Raw.Split(',');
				int maxLength = 0;
				for (int i = 0; i + 1 < itemString.Length; i += 2)
				{
					itemString[i] = itemString[i].Trim();
					itemString[i + 1] = itemString[i + 1].Trim();
					string[] resultString = itemString[i].Split('E');
					if (resultString.Length == 2)
					{
						int ResultA;
						bool isNumber = int.TryParse(resultString[1], out ResultA);
						if (isNumber)
						{
							float ResultB;
							isNumber = float.TryParse(itemString[i + 1], out ResultB);
							if (isNumber)
							{
								if (ResultA > -1 && ResultA < hpData.Length)
								{
									hpData[ResultA, 0] = ResultB;
									hpData[ResultA, 1] = 1;
									if (maxLength < ResultA)
									{
										maxLength = ResultA;
									}
								}
							}
							else
							{
								MainPlugin.ModLogger.LogWarning(string.Format("{0} [Health Multiplier] '{1}' Is not a valid float.", MainPlugin.LOGNAME, resultString[1]));
							}

						}
						else
						{
							MainPlugin.ModLogger.LogWarning(string.Format("{0} [Health Multiplier] '{1}' Is not a valid Evolution Stage number.", MainPlugin.LOGNAME, resultString[0]));
						}
					}
				}
				if (maxLength > 0)
				{
					Evo_HPStages = new float[maxLength + 1];
					float lastValue = 0f;
					for (int i = 0; i < maxLength + 1; i++)
					{
						if (hpData[i, 1] > 0f)
						{
							Evo_HPStages[i] = hpData[i, 0];
							lastValue = hpData[i, 0];
						}
						else
						{
							Evo_HPStages[i] = lastValue;
						}
					}
				}
				float[,] dmgData = new float[999, 2];
				itemString = Evo_DmgStages_Raw.Split(',');
				maxLength = 0;
				for (int i = 0; i + 1 < itemString.Length; i += 2)
				{
					itemString[i] = itemString[i].Trim();
					itemString[i + 1] = itemString[i + 1].Trim();
					string[] resultString = itemString[i].Split('E');
					if (resultString.Length == 2)
					{
						int ResultA;
						bool isNumber = int.TryParse(resultString[1], out ResultA);
						if (isNumber)
						{
							float ResultB;
							isNumber = float.TryParse(itemString[i + 1], out ResultB);
							if (isNumber)
							{
								if (ResultA > -1 && ResultA < dmgData.Length)
								{
									dmgData[ResultA, 0] = ResultB;
									dmgData[ResultA, 1] = 1;
									if (maxLength < ResultA)
									{
										maxLength = ResultA;
									}
								}
							}
							else
							{
								MainPlugin.ModLogger.LogWarning(string.Format("{0} [Damage Multiplier] '{1}' Is not a valid float.", MainPlugin.LOGNAME, resultString[1]));
							}

						}
						else
						{
							MainPlugin.ModLogger.LogWarning(string.Format("{0} [Damage Multiplier] '{1}' Is not a valid Evolution Stage number.", MainPlugin.LOGNAME, resultString[0]));
						}
					}
				}
				if (maxLength > 0)
				{
					Evo_DmgStages = new float[maxLength + 1];
					float lastValue = 0f;
					for (int i = 0; i < maxLength + 1; i++)
					{
						if (dmgData[i, 1] > 0f)
						{
							Evo_DmgStages[i] = dmgData[i, 0];
							lastValue = dmgData[i, 0];
						}
						else
						{
							Evo_DmgStages[i] = lastValue;
						}
					}
				}
				float[,] regenData = new float[999, 2];
				itemString = Evo_RegenStages_Raw.Split(',');
				maxLength = 0;
				for (int i = 0; i + 1 < itemString.Length; i += 2)
				{
					itemString[i] = itemString[i].Trim();
					itemString[i + 1] = itemString[i + 1].Trim();
					string[] resultString = itemString[i].Split('E');
					if (resultString.Length == 2)
					{
						int ResultA;
						bool isNumber = int.TryParse(resultString[1], out ResultA);
						if (isNumber)
						{
							float ResultB;
							isNumber = float.TryParse(itemString[i+1], out ResultB);
							if (isNumber)
							{
								if (ResultA > -1 && ResultA < regenData.Length)
								{
									regenData[ResultA, 0] = ResultB;
									regenData[ResultA, 1] = 1;
									if (maxLength < ResultA)
									{
										maxLength = ResultA;
									}
								}
							}
							else
							{
								MainPlugin.ModLogger.LogWarning(string.Format("{0} [Base Regen] '{1}' Is not a valid float.", MainPlugin.LOGNAME, resultString[1]));
							}

						}
						else
						{
							MainPlugin.ModLogger.LogWarning(string.Format("{0} [Base Regen] '{1}' Is not a valid Evolution Stage number.", MainPlugin.LOGNAME, resultString[0]));
						}
					}
				}
				if (maxLength > 0)
				{
					Evo_RegenStages = new float[maxLength + 1];
					float lastValue = 0f;
					for (int i = 0; i < maxLength + 1; i++)
					{
						if (regenData[i, 1] > 0f)
						{
							Evo_RegenStages[i] = regenData[i, 0];
							lastValue = regenData[i, 0];
						}
						else
						{
							Evo_RegenStages[i] = lastValue;
						}
					}
                }

				EliteList = new int[EliteCatalog.eliteList.Capacity];
				for (int i = 0; i < EliteList.Length; i++)
				{
					EliteList[i] = -1;
				}
				itemString = EliteList_Raw.Split(',');
				for (int i = 0; i + 1 < itemString.Length; i += 2)
				{
					itemString[i] = itemString[i].Trim();
					itemString[i+1] = itemString[i+1].Trim();
					int eliteIndex = GetEliteIndex(itemString[i]);
					if (eliteIndex > -1)
					{
						if (eliteIndex > -1 && eliteIndex < EliteList.Length)
						{
							int tier = -1;
							bool isNumber = int.TryParse(itemString[i+1], out tier);
							if (isNumber && tier > 0)
							{
								EliteList[eliteIndex] = tier;
							}
						}
					}
					else
					{
						MainPlugin.ModLogger.LogWarning(string.Format("{0}Could not find EliteDef '{1}' for Elite Tiers.", MainPlugin.LOGNAME, itemString[i].Trim()));
					}
				}

				itemString = Evo_EliteStages_Raw.Split(',');
				Evo_EliteStages = new List<EliteEvolutionStage>();
				for (int i = 0; i + 1 < itemString.Length; i += 2)
				{
					itemString[i] = itemString[i].Trim();
					itemString[i+1] = itemString[i+1].Trim();
					string[] resultString = itemString[i].Split('T');
					if (resultString.Length == 2)
					{
						int intResult;
						bool isNumber = int.TryParse(resultString[1], out intResult);
						if (isNumber)
						{
							EliteEvolutionStage eliteStage = new EliteEvolutionStage();
							eliteStage.TierIndex = intResult;
							isNumber = int.TryParse(itemString[i+1], out intResult);
							if (isNumber)
                            {
								eliteStage.EvolutionStage = intResult;
								Evo_EliteStages.Add(eliteStage);
                            }
						}
						else
                        {
							MainPlugin.ModLogger.LogWarning(string.Format("{0}'{1}' Is not a valid Tier number.", MainPlugin.LOGNAME, resultString[0]));
						}
					}
				}

				itemString = Evo_BodyStages_Raw.Split(',');
				Evo_BodyStages = new List<BodyEvolutionStage>();
				for (int i = 0; i + 1 < itemString.Length; i += 2)
				{
					itemString[i] = itemString[i].Trim();
					itemString[i+1] = itemString[i+1].Trim();
					int intResult;
					bool isNumber = int.TryParse(itemString[i+1], out intResult);
					if (isNumber)
					{
						if (intResult > 1)
                        {
							BodyEvolutionStage bodyStage = new BodyEvolutionStage();
							bodyStage.BodyName = itemString[i];
							bodyStage.EvolutionStage = intResult;
							Evo_BodyStages.Add(bodyStage);
						}
						else
                        {
							MainPlugin.ModLogger.LogWarning(string.Format("{0}'{1}' Is being assigned to an evolution stage lower than 2, ignoring.", MainPlugin.LOGNAME, itemString[i]));
						}
					}
				}

				Evo_BaseMaster = MasterCatalog.FindMasterPrefab(Evo_BaseMaster_Raw);
				if (Evo_BaseMaster)
				{
					MainPlugin.ModLogger.LogInfo("Set [" + Evo_BaseMaster_Raw + "] as base master form.");
				}
				else
                {
					MainPlugin.ModLogger.LogWarning(string.Format("{0} Could not find master for [{1}] this will cause errors.", MainPlugin.LOGNAME, Evo_BaseMaster_Raw));
				}
				CharacterMaster master = Evo_BaseMaster.GetComponent<CharacterMaster>();
				if (master)
				{
					GameObject bodyPrefab = master.bodyPrefab;
					if (bodyPrefab)
					{
						Evo_BaseBody_Raw = bodyPrefab.name;
						MainPlugin.ModLogger.LogInfo("Base Master has [" + Evo_BaseBody_Raw + "] as base body form.");
					}
				}
			}
		}
		private static int GetEliteIndex(string eliteDef)
		{
			for (int i = 0; i < EliteCatalog.eliteDefs.Length; i++)
			{
				if (EliteCatalog.eliteDefs[i].name == eliteDef)
				{
					return (int)EliteCatalog.eliteDefs[i].eliteIndex;
				}
			}
			return -1;
		}
		private void OnRunStart(Run self)
		{
			CreateEliteList();
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(CU8Content.Items.LemurianHarness);
				if (itemCount > 0)
                {
					args.healthMultAdd += GetHPMult(itemCount);
					args.damageMultAdd += GetDmgMult(itemCount);
					float regen = GetRegenAdd(itemCount);
					if (regen != 0)
                    {
						args.baseRegenAdd += regen;
						args.levelRegenAdd += regen / 5f;
					}
				}
			}
		}
		private float GetHPMult(int itemCount)
        {
			float result = 0f;
			if (Evo_HPStages != null)
			{
				if (itemCount > Evo_HPStages.Length - 1)
				{
					itemCount = Evo_HPStages.Length - 1;
				}
				return Evo_DmgStages[itemCount];
			}
			return result;
		}
		private float GetDmgMult(int itemCount)
		{
			float result = 0f;
			if (Evo_DmgStages != null)
			{
				if (itemCount > Evo_DmgStages.Length - 1)
				{
					itemCount = Evo_DmgStages.Length - 1;
				}
				return Evo_DmgStages[itemCount];
			}
			return result;
		}
		private float GetRegenAdd(int itemCount)
		{
			float result = 0f;
			if (Evo_RegenStages != null)
			{
				if (itemCount > Evo_RegenStages.Length - 1)
				{
					itemCount = Evo_RegenStages.Length - 1;
				}
				return Evo_RegenStages[itemCount];
			}
			return result;
		}
		private string GetDisplayName(On.RoR2.CharacterBody.orig_GetDisplayName orig, CharacterBody self)
        {
			Inventory inventory = self.inventory;
			if (inventory)
            {
				int itemCount = inventory.GetItemCount(CU8Content.Items.LemurianHarness);
				if (itemCount > 0)
                {
					return Language.GetString(self.baseNameToken) + string.Format("({0})", itemCount);
                }
			}
			return orig(self);
		}
		private void OnDevotedBodyDead_Penalty(On.DevotedLemurianController.orig_OnDevotedBodyDead orig, DevotedLemurianController self)
		{
			int evoLevel = self._devotedEvolutionLevel;
			int levelAdd = 0;
			CharacterMaster devotedMaster = self._lemurianMaster;
			if (devotedMaster.IsDeadAndOutOfLivesServer())
            {
				bool shouldDie = true;
				if (Death_Penalty > 0)
				{
					if (evoLevel > 0)
					{
						shouldDie = false;
						if (Death_Penalty == 2)
						{
							levelAdd -= evoLevel;
						}
						else
						{
							levelAdd = -1;
						}
					}
					else
					{
						levelAdd = -1;
					}
				}
				if (shouldDie)
				{
					self.DevotedEvolutionLevel = -1;
					devotedMaster.destroyOnBodyDeath = true;
					self._devotionInventoryController.DropScrapOnDeath(self._devotionItem, self.LemurianBody);
					if (Death_EggDrop)
					{
						PlaceDevotionEgg(self.LemurianBody.footPosition);
					}
					UnityEngine.Object.Destroy(self._lemurianMaster.gameObject, 1f);
				}
				else
				{
					EvolveDevotedLemurian(self, levelAdd);
					devotedMaster.Invoke("RespawnExtraLife", 2f);
				}
			}
			self._devotionInventoryController.UpdateAllMinions(false);
		}
		private void OnDevotedBodyDead_EggDrop(On.DevotedLemurianController.orig_OnDevotedBodyDead orig, DevotedLemurianController self)
		{
			if (self._lemurianMaster.IsDeadAndOutOfLivesServer())
			{
				self._devotionInventoryController.DropScrapOnDeath(self._devotionItem, self.LemurianBody);
				if (Death_EggDrop)
				{
					PlaceDevotionEgg(self.LemurianBody.footPosition);
				}
			}
			self._devotionInventoryController.UpdateAllMinions(false);
		}
		private void UpdateAllMinions(On.RoR2.DevotionInventoryController.orig_UpdateAllMinions orig, DevotionInventoryController self, bool shouldEvolve)
        {
			if(!NetworkServer.active)
            {
				return;
            }
			if (self._summonerMaster)
			{
				MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(self._summonerMaster.netId);
				if (minionGroup != null)
				{
					foreach (MinionOwnership minionOwnership in minionGroup.members)
					{
						DevotedLemurianController devotedController;
						if (minionOwnership && minionOwnership.GetComponent<CharacterMaster>().TryGetComponent<DevotedLemurianController>(out devotedController))
						{
							CharacterMaster devotedMaster = devotedController._lemurianMaster;
							CharacterBody devotedBody = devotedController.LemurianBody;
							if (shouldEvolve)
                            {
								EvolveDevotedLemurian(devotedController, 1);
							}
							if (Death_Penalty > 0)
							{
								if (devotedController.DevotedEvolutionLevel > -1)
								{
									devotedMaster.destroyOnBodyDeath = false;
								}
								else
								{
									devotedMaster.destroyOnBodyDeath = true;
								}
							}
							
						}
					}
				}
				RefreshDevotedInventory(self);
			}
			return;
        }
		private void UpdateMinionInventory(On.RoR2.DevotionInventoryController.orig_UpdateMinionInventory orig, DevotionInventoryController self, DevotedLemurianController controller, bool shouldEvolve)
        {
			return; //Moved to RefreshDevotedInventory and EvolveDevotedLemurian
		}
		private void Hook_EvolveDevotedLumerian(On.RoR2.DevotionInventoryController.orig_EvolveDevotedLumerian orig, DevotionInventoryController self, DevotedLemurianController controller)
        {
			return; //Moved to EvolveDevotedLemurian
		}
		private void RefreshDevotedInventory(DevotionInventoryController controller)
        {
			Inventory devotionInventory = controller._devotionMinionInventory;
			devotionInventory.CleanInventory();
			if (controller._summonerMaster)
			{
				MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(controller._summonerMaster.netId);
				if (minionGroup != null)
				{
					foreach (MinionOwnership minionOwnership in minionGroup.members)
					{
						DevotedLemurianController devotedController;
						if (minionOwnership && minionOwnership.GetComponent<CharacterMaster>().TryGetComponent<DevotedLemurianController>(out devotedController))
						{
							if (devotedController.DevotionItem != ItemIndex.None)
                            {
								devotionInventory.GiveItem(devotedController.DevotionItem, devotedController.DevotedEvolutionLevel + 1);
							}
						}
					}
					foreach (MinionOwnership minionOwnership in minionGroup.members)
					{
						DevotedLemurianController devotedController;
						if (minionOwnership && minionOwnership.GetComponent<CharacterMaster>().TryGetComponent<DevotedLemurianController>(out devotedController))
						{
							Inventory devotedInventory = devotedController.LemurianInventory;
							devotedInventory.CleanInventory();
							devotedInventory.AddItemsFrom(devotionInventory);
							devotedInventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
							devotedInventory.GiveItem(CU8Content.Items.LemurianHarness, devotedController.DevotedEvolutionLevel + 1);
						}
					}
				}
			}
		}
		private bool CheckIfNeedTeleport (On.DevotedLemurianController.orig_CheckIfNeedTeleport orig, DevotedLemurianController self)
        {
			if (!self.LemurianBody.hasEffectiveAuthority)
			{
				return false;
			}
			CharacterMaster characterMaster = self._lemurianMaster ? self._lemurianMaster.minionOwnership.ownerMaster : null;
			CharacterBody characterBody = characterMaster ? characterMaster.GetBody() : null;
			if (!characterBody)
			{
				return false;
			}
			Vector3 corePosition = characterBody.corePosition;
			Vector3 corePosition2 = self.LemurianBody.corePosition;
			return ((self.LemurianBody.characterMotor && self.LemurianBody.characterMotor.walkSpeed > 0f) || self.LemurianBody.moveSpeed > 0f) && (corePosition2 - corePosition).magnitude > Misc_LeashDistance;
		}
		private void OnHatchEgg(On.RoR2.CharacterAI.LemurianEggController.orig_SummonLemurian orig, LemurianEggController self, PickupIndex pickupIndex)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			ItemIndex itemIndex = pickupIndex.itemIndex;
			MasterSummon masterSummon = new MasterSummon();
			masterSummon.masterPrefab = Evo_BaseMaster;
			masterSummon.position = self.transform.position;
			masterSummon.rotation = self.transform.rotation;
			Interactor interactor = self.interactor;
			masterSummon.summonerBodyObject = ((interactor != null) ? interactor.gameObject : null);
			masterSummon.ignoreTeamMemberLimit = true;
			masterSummon.useAmbientLevel = true;
			CharacterMaster characterMaster = masterSummon.Perform();
			EffectData effectData = new EffectData
			{
				origin = self.gameObject.transform.position
			};
			DevotionInventoryController devotionInventoryController = DevotionInventoryController.GetOrCreateDevotionInventoryController(interactor);
			if (devotionInventoryController)
            {
				if (characterMaster)
				{
					//characterMaster.bodyPrefab = BodyCatalog.FindBodyPrefab(BaseFormName);
					//characterMaster.Respawn(self.transform.position, self.transform.rotation);
					if (itemIndex == RoR2Content.Items.ExtraLife.itemIndex)
                    {
						CreateTwin_ExtraLife(self.gameObject, Evo_BaseMaster, devotionInventoryController);
						itemIndex = RoR2Content.Items.ScrapRed.itemIndex;
					}
					else if (itemIndex == DLC1Content.Items.ExtraLifeVoid.itemIndex)
					{
						CreateTwin_ExtraLifeVoid(self.gameObject, Evo_BaseMaster, devotionInventoryController);
						itemIndex = DLC1Content.Items.BearVoid.itemIndex;
					}
					SetDontDestroyOnLoad comp = characterMaster.GetComponent<SetDontDestroyOnLoad>();
					if (!comp)
                    {
						comp = characterMaster.gameObject.AddComponent<SetDontDestroyOnLoad>();
					}
					DevotedLemurianController devotedController = characterMaster.GetComponent<DevotedLemurianController>();
					if (!devotedController)
					{
						devotedController = characterMaster.gameObject.AddComponent<DevotedLemurianController>();
					}
					devotedController.InitializeDevotedLemurian(itemIndex, devotionInventoryController);
					Util.PlaySound(self.sfxLocator.openSound, self.gameObject);
					EffectManager.SpawnEffect(VFX_Hatch, effectData, true);
					devotionInventoryController.UpdateAllMinions(false);
				}
			}
			UnityEngine.Object.Destroy(self.gameObject);
			return;
		}
		private void EvolveDevotedLemurian(DevotedLemurianController controller, int levelAmount)
        {
			if (!NetworkServer.active)
            {
				return;
            }
			
			CharacterBody devotedBody = controller.LemurianBody;
			CharacterMaster devotedMaster = controller._lemurianMaster;
			bool isAlive = devotedBody.healthComponent.alive;
			int beforeLevel = controller.DevotedEvolutionLevel;
			int evoLevel = controller.DevotedEvolutionLevel;
			evoLevel += levelAmount;
			if (EvoMax > -1)
            {
				evoLevel += levelAmount;
				Math.Min(EvoMax, evoLevel);
			}
			Math.Max(0, evoLevel);
			if (evoLevel != controller.DevotedEvolutionLevel)
            {
				if (devotedBody)
                {
					Util.PlaySound(SFX_Evolve, devotedBody.gameObject);
				}
				controller.DevotedEvolutionLevel = evoLevel;
				string bodyName = FindLatestEvoBody(evoLevel + 1);
				int eliteTier = FindLatestEvoElite(evoLevel+1, beforeLevel+1);
				BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyName);
				if (bodyIndex > BodyIndex.None)
                {
					//MainPlugin.ModLogger.LogInfo("NewBody = " + bodyName);
					if (isAlive)
					{
						if (bodyIndex != devotedBody.bodyIndex)
                        {
							devotedMaster.TransformBody(bodyName);
						}
					}
					else
					{
						devotedMaster.bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
					}
				}
				if (eliteTier > 0)
                {
					GenerateEliteBuff(devotedBody, controller, eliteTier);
				}
				else if (eliteTier == 0)
				{
					devotedBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
				}
			}
		}
		private string FindLatestEvoBody(int newLevel)
        {
			int record = -1;
			string result = Evo_BaseBody_Raw;
			for(int i = 0; i < Evo_BodyStages.Count; i++)
            {
				if (Evo_BodyStages[i].EvolutionStage > record && Evo_BodyStages[i].EvolutionStage <= newLevel)
                {
					record = Evo_BodyStages[i].EvolutionStage;
					result = Evo_BodyStages[i].BodyName;
				}
            }
			return result;
        }
		private int FindLatestEvoElite(int newLevel, int oldLevel)
		{
			int oldrecord = -1;
			for (int i = 0; i < Evo_EliteStages.Count; i++)
			{
				if (Evo_EliteStages[i].EvolutionStage > oldrecord && Evo_EliteStages[i].EvolutionStage <= oldLevel)
				{
					oldrecord = Evo_EliteStages[i].EvolutionStage;
				}
			}
			int record = -1;
			int result = 0;
			for (int i = 0; i < Evo_EliteStages.Count; i++)
			{
				if (Evo_EliteStages[i].EvolutionStage > record && Evo_EliteStages[i].EvolutionStage <= newLevel)
				{
					record = Evo_EliteStages[i].EvolutionStage;
					result = Evo_EliteStages[i].TierIndex;
				}
			}
			if (oldrecord != record)
            {
				return result;
            }
			return -1;
		}
		private void GenerateEliteBuff(CharacterBody body, DevotedLemurianController controller, int eliteTier)
        {
			if (!NetworkServer.active)
			{
				return;
			}
			List<int> validElites = new List<int>();
			for(int i = 0; i<EliteAspects.Count; i++)
            {
				int[] aspectData = EliteAspects[i];
				if (aspectData[1] <= eliteTier)
                {
					validElites.Add(aspectData[0]);
				}
			}
			if (validElites.Count > 0)
            {
				int randomIndex = UnityEngine.Random.Range(0, validElites.Count);
				body.inventory.SetEquipmentIndex((EquipmentIndex)validElites[randomIndex]);
			}
		}
		private void CreateEliteList()
        {
			EliteAspects = new List<int[]>();
			for(int i = 0; i< EliteList.Length; i++)
            {
				if (EliteList[i] > 0)
                {
					EliteDef eliteDef = EliteCatalog.GetEliteDef((EliteIndex)i);
					if (eliteDef)
                    {
						if (eliteDef.IsAvailable())
                        {
							RegisterEliteAspect(eliteDef.eliteEquipmentDef.equipmentIndex, EliteList[i]);
						}
                    }
				}
            }
		}
		private void RegisterEliteAspect(EquipmentIndex aspect, int tier)
        {
			EliteAspects.Add(new int[] { (int)aspect, tier });
		}
		private void PlaceDevotionEgg(Vector3 spawnLoc)
        {
			if (EggSpawnCard != null)
			{
				RaycastHit raycastHit;
				bool didHit = Physics.Raycast(spawnLoc + (Vector3.up * 1f), Vector3.down, out raycastHit, float.PositiveInfinity, LayerMask.GetMask(new string[]
				{
				"World"
				}));
				if (didHit)
                {
					spawnLoc = raycastHit.point;
                }

				DirectorPlacementRule placementRule = new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Direct,
					position = spawnLoc
				};
				DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(EggSpawnCard, placementRule, new Xoroshiro128Plus(0UL)));
			}
		}

		private void CreateTwin_ExtraLife(GameObject targetLocation, GameObject masterPrefab, DevotionInventoryController devotionInventoryController)
        {
			ItemIndex itemIndex = RoR2Content.Items.ExtraLifeConsumed.itemIndex;
			if (devotionInventoryController)
			{
				CharacterMaster ownerMaster = devotionInventoryController._summonerMaster;
				if (ownerMaster)
				{
					CharacterBody ownerBody = ownerMaster.GetBody();
					if (ownerBody)
					{
						MasterSummon masterSummon = new MasterSummon();
						masterSummon.masterPrefab = masterPrefab;
						masterSummon.position = targetLocation.transform.position;
						masterSummon.rotation = targetLocation.transform.rotation;
						masterSummon.summonerBodyObject = ownerBody.gameObject;
						masterSummon.ignoreTeamMemberLimit = true;
						masterSummon.useAmbientLevel = true;
						CharacterMaster twinMaster = masterSummon.Perform();

						if (twinMaster)
						{
							DevotedLemurianController devotedController = twinMaster.GetComponent<DevotedLemurianController>();
							if (!devotedController)
							{
								devotedController = twinMaster.gameObject.AddComponent<DevotedLemurianController>();
							}
							devotedController.InitializeDevotedLemurian(itemIndex, devotionInventoryController);
						}
					}
				}
			}
		}

		private void CreateTwin_ExtraLifeVoid(GameObject targetLocation, GameObject masterPrefab, DevotionInventoryController devotionInventoryController)
		{
			ItemIndex itemIndex = DLC1Content.Items.BleedOnHitVoid.itemIndex;
			if (devotionInventoryController)
			{
				CharacterMaster ownerMaster = devotionInventoryController._summonerMaster;
				if (ownerMaster)
				{
					CharacterBody ownerBody = ownerMaster.GetBody();
					if (ownerBody)
					{
						MasterSummon masterSummon = new MasterSummon();
						masterSummon.masterPrefab = masterPrefab;
						masterSummon.position = targetLocation.transform.position;
						masterSummon.rotation = targetLocation.transform.rotation;
						masterSummon.summonerBodyObject = ownerBody.gameObject;
						masterSummon.ignoreTeamMemberLimit = true;
						masterSummon.useAmbientLevel = true;
						CharacterMaster twinMaster = masterSummon.Perform();

						if (twinMaster)
						{
							DevotedLemurianController devotedController = twinMaster.GetComponent<DevotedLemurianController>();
							if (!devotedController)
							{
								devotedController = twinMaster.gameObject.AddComponent<DevotedLemurianController>();
							}
							devotedController.InitializeDevotedLemurian(itemIndex, devotionInventoryController);
						}
					}
				}
			}
		}
	}

	public class EliteEvolutionStage
	{
		public int TierIndex = 0;
		public int EvolutionStage = -1;
	}

	public class BodyEvolutionStage
	{
		public string BodyName = "LemurianBody";
		public int EvolutionStage = -1;
	}
}
