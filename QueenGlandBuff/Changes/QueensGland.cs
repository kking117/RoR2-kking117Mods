using System;
using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using QueenGlandBuff.Utils;

namespace QueenGlandBuff.Changes
{
    public class QueensGland
    {
		public static List<EquipmentDef> StageEliteEquipmentDefs = new List<EquipmentDef>();
		public static EquipmentIndex Gland_DefaultAffix_Var;
		private static int BaseDamage = 20;
		private static int StackDamage = 20;
		private static int BaseHealth = 10;
		private static int StackHealth = 10;
		private static int MaxSummons = 1;
		private static float RespawnTime = 20f;
		internal static void Begin()
        {
			SetupConfigValues();
			UpdateItemDescription();
			if (MainPlugin.Config_QueensGland_SpawnAffix.Value != 0)
			{
				Stage.onServerStageBegin += ServerStageBegin;
			}
			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
			CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_OnInventoryChanged;
			BeetleGland_Override();
		}

		private static void SetupConfigValues()
		{
			BaseDamage = MainPlugin.Config_QueensGland_BaseDamage.Value;
			StackDamage = MainPlugin.Config_QueensGland_StackDamage.Value;
			BaseHealth = MainPlugin.Config_QueensGland_BaseHealth.Value;
			StackHealth = MainPlugin.Config_QueensGland_StackHealth.Value;
			MaxSummons = MainPlugin.Config_QueensGland_MaxSummons.Value;
			RespawnTime = MainPlugin.Config_QueensGland_RespawnTime.Value;
		}
		
		private static void UpdateItemDescription()
		{
			if (MainPlugin.Config_Debug.Value)
			{
				MainPlugin.ModLogger.LogInfo("Changing Queen's Gland descriptions.");
			}
			string pickup = "Recruit ";
			string desc = "<style=cIsUtility>Summon ";
			if (MainPlugin.Config_QueensGland_SpawnAffix.Value == 1)
			{
				pickup += "an Elite Beetle Guard.";
				desc += "an Elite Beetle Guard</style>";
			}
			else
			{
				pickup += "a Beetle Guard.";
				desc += "a Beetle Guard</style>";
			}
			if (MaxSummons > 1)
            {
				desc += string.Format(" with bonus <style=cIsDamage>{0}% damage</style>", (10 + BaseDamage) * 10);
				desc += string.Format(" and <style=cIsHealing>{0}% health</style>.", (10 + BaseHealth) * 10);

				desc += string.Format(" Can have <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> total Guards, up to <style=cIsUtility>{0}</style>.", MaxSummons);
				if (StackHealth != 0 || StackDamage != 0)
                {
					desc += " Further stacks give";
					if (StackDamage != 0)
					{
						desc += string.Format(" <style=cStack>+{0}%</style> <style=cIsDamage>damage</style>", StackDamage * 10);
						if (StackHealth != 0)
						{
							desc += " and";
						}
						else
                        {
							desc += ".";
                        }
					}
					if (StackHealth != 0)
					{
						desc += string.Format(" <style=cStack>+{0}%</style> <style=cIsHealing>health</style>", StackHealth * 10);
					}
				}
			}
			else
            {
				desc += string.Format(" with bonus <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style> damage</style>", (10 + BaseDamage) * 10, StackDamage * 10);
				desc += string.Format(" and <style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style> health</style>.", (10 + BaseHealth) * 10, StackHealth * 10);
			}
			LanguageAPI.Add("ITEM_BEETLEGLAND_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BEETLEGLAND_DESC", desc);
		}
		private static void ServerStageBegin(Stage self)
		{
			UpdateEliteList();
		}
		private static void UpdateEliteList()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Gland_DefaultAffix_Var = EquipmentCatalog.FindEquipmentIndex(MainPlugin.Config_QueensGland_DefaultAffix.Value);
			if (Gland_DefaultAffix_Var != EquipmentIndex.None)
			{
				if (Run.instance.IsEquipmentExpansionLocked(Gland_DefaultAffix_Var))
				{
					Gland_DefaultAffix_Var = EquipmentIndex.None;
				}
			}
			StageEliteEquipmentDefs.Clear();
			CombatDirector.EliteTierDef[] DirectorElite = EliteAPI.GetCombatDirectorEliteTiers();
			bool IsMoon = Stage.instance.sceneDef.cachedName.Contains("moon");
			bool IsHonor = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly);
			bool IsLoop = Run.instance.loopClearCount > 0;

			SpawnCard.EliteRules eliterules = SpawnCard.EliteRules.Default;
			if (IsHonor)
			{
				eliterules = SpawnCard.EliteRules.ArtifactOnly;
			}
			if (IsMoon)
			{
				eliterules = SpawnCard.EliteRules.Lunar;
			}

			for (int i = 0; i < DirectorElite.Length; i++)
			{
				if (DirectorElite[i].isAvailable.Invoke(eliterules))
				{
					for (int z = 0; z < DirectorElite[i].eliteTypes.GetLength(0); z++)
					{
						if (DirectorElite[i].eliteTypes[z])
						{
							StageEliteEquipmentDefs.Add(DirectorElite[i].eliteTypes[z].eliteEquipmentDef);
						}
					}
				}
			}
		}
		
		private static int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
		{
			var result = orig(self, slot);
			if (slot != DeployableSlot.BeetleGuardAlly)
			{
				return result;
			}
			return Math.Min(MaxSummons, self.inventory.GetItemCount(RoR2Content.Items.BeetleGland));
		}
		private static void CharacterBody_OnInventoryChanged(CharacterBody self)
		{
			UpdateBeetleGuardStacks(self.master);
		}
		private static void UpdateBeetleGuardStacks(CharacterMaster owner)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (owner)
			{
				if (owner.deployablesList != null)
				{
					int deployableCount = owner.GetDeployableSameSlotLimit(DeployableSlot.BeetleGuardAlly);
					int itemCount = owner.inventory.GetItemCount(RoR2Content.Items.BeetleGland);
					int stackBonus = Math.Max(0, itemCount - MaxSummons);
					int dmgitem = BaseDamage + (StackDamage * stackBonus);
					int hpitem = BaseHealth + (StackHealth * stackBonus);
					int summonCount = 0;
					for (int i = 0; i < owner.deployablesList.Count; i++)
					{
						if (owner.deployablesList[i].slot == DeployableSlot.BeetleGuardAlly)
						{
							Deployable deployable = owner.deployablesList[i].deployable;
							if (deployable)
							{
								CharacterMaster deployableMaster = deployable.GetComponent<CharacterMaster>();
								if (deployableMaster)
								{
									summonCount++;
									if (summonCount > deployableCount)
									{
										deployableMaster.TrueKill();
									}
									else
									{
										Inventory inv = deployableMaster.inventory;
										if (inv)
										{
											inv.ResetItem(RoR2Content.Items.BoostDamage);
											inv.ResetItem(RoR2Content.Items.BoostHp);
											inv.GiveItem(RoR2Content.Items.BoostDamage, dmgitem);
											inv.GiveItem(RoR2Content.Items.BoostHp, hpitem);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		private static void BeetleGland_Override()
		{
			On.RoR2.Items.BeetleGlandBodyBehavior.FixedUpdate += (orig, self) =>
			{
				if (Stage.instance.sceneDef == MainPlugin.BazaarSceneDef)
				{
					return;
				}
				if (self.body && self.body.inventory)
				{
					CharacterMaster owner = self.body.master;
					if (owner)
					{
						int extraglands = Math.Max(0, owner.inventory.GetItemCount(RoR2Content.Items.BeetleGland) - MaxSummons);
						int deployableCount = owner.GetDeployableCount(DeployableSlot.BeetleGuardAlly);
						int maxdeployable = owner.GetDeployableSameSlotLimit(DeployableSlot.BeetleGuardAlly);
						if (deployableCount < maxdeployable)
						{
							self.guardResummonCooldown -= Time.fixedDeltaTime;
							if (self.guardResummonCooldown <= 0f)
							{
								self.guardResummonCooldown = 2f;
								DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
								{
									placementMode = DirectorPlacementRule.PlacementMode.Approximate,
									minDistance = 3f,
									maxDistance = 40f,
									spawnOnTarget = self.transform,
								}, RoR2Application.rng);
								directorSpawnRequest.summonerBodyObject = self.gameObject;
								directorSpawnRequest.teamIndexOverride = TeamIndex.Player;
								directorSpawnRequest.ignoreTeamMemberLimit = true;
								directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
								{
									if (SetupSummonedBeetleGuard(spawnResult, owner, extraglands))
									{
										self.guardResummonCooldown = RespawnTime;
									}
								}));
								DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
							}
						}
					}
				}
			};
		}
		private static bool SetupSummonedBeetleGuard(SpawnCard.SpawnResult spawnResult, CharacterMaster owner, int itemcount)
		{
			GameObject spawnedInstance = spawnResult.spawnedInstance;
			if (!spawnedInstance)
			{
				return false;
			}
			CharacterMaster spawnMaster = spawnedInstance.GetComponent<CharacterMaster>();
			if (spawnMaster)
			{
				Deployable deployable = spawnMaster.GetComponent<Deployable>();
				if (!deployable)
				{
					deployable = spawnMaster.gameObject.AddComponent<Deployable>();
				}
				if (owner)
				{
					CharacterBody spawnBody = spawnMaster.GetBody();
					if (spawnBody)
					{
						spawnMaster.teamIndex = owner.teamIndex;
						spawnBody.teamComponent.teamIndex = owner.teamIndex;
					}
					Helpers.GiveRandomEliteAffix(spawnMaster);
					BeetleGuardAlly.UpdateAILeash(spawnMaster);
					deployable.onUndeploy.AddListener(new UnityEngine.Events.UnityAction(spawnMaster.TrueKill));
					owner.AddDeployable(deployable, DeployableSlot.BeetleGuardAlly);
					UpdateBeetleGuardStacks(owner);
					return true;
				}
			}
			return false;
		}
	}
}