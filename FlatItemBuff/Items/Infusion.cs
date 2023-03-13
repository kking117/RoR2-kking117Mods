using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Events;
using EntityStates.GummyClone;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Infusion
	{
		public static ItemDef BloodCloneItem;
		internal static DeployableSlot InfusionDeployable = (DeployableSlot)209063;
		private static float BaseGain = 1f;
		private static float StackGain = 0.5f;
		private static int FakeBaseGain = 35;
		private static int CloneCost = 800;
		private static int LevelCost = 1000;
		public Infusion()
		{
			MainPlugin.ModLogger.LogInfo("Changing Infusion");
			SetupConfigValues();
			UpdateItemDef();
			CreateItemDef();
			UpdateText();
			Hooks();
		}
		private void SetupConfigValues()
		{
			StackGain = MainPlugin.Infusion_StackGain.Value;
			FakeBaseGain = MainPlugin.Infusion_FakeBaseGain.Value;
			CloneCost = MainPlugin.Infusion_CloneCost.Value;
			LevelCost = MainPlugin.Infusion_LevelCost.Value;
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/Infusion/Infusion.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.Utility);
				itemTags.Remove(ItemTag.Healing);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void CreateItemDef()
        {
			BloodCloneItem = ScriptableObject.CreateInstance<ItemDef>();
			BloodCloneItem.canRemove = false;
			BloodCloneItem.name = "FlatItemBuffBloodClone";
			BloodCloneItem.deprecatedTier = ItemTier.NoTier;
			BloodCloneItem.descriptionToken = "";
			BloodCloneItem.nameToken = "Blood Clone";
			BloodCloneItem.pickupToken = "";
			BloodCloneItem.hidden = true;
			BloodCloneItem.pickupIconSprite = null;
			BloodCloneItem.tags = new[]
			{
				ItemTag.WorldUnique,
				ItemTag.BrotherBlacklist,
				ItemTag.CannotSteal,
				ItemTag.CannotDuplicate,
				ItemTag.AIBlacklist
			};
			ItemDisplayRule[] idr = new ItemDisplayRule[0];
			ItemAPI.Add(new CustomItem(BloodCloneItem, idr));
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Create a clone by killing enemies, further kills increases its level.";
			string desc = string.Format("Killing an enemy gives <style=cIsHealth>{0}% <style=cStack>(+{1}% per stack)</style></style> blood. Collecting enough blood creates a <style=cIsUtility>clone of yourself</style>, further blood collected increases the <style=cIsHealing>clone's level</style>.", 100f, StackGain * 100f);
			LanguageAPI.Add("ITEM_INFUSION_PICKUP", pickup);
			LanguageAPI.Add("ITEM_INFUSION_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			MainPlugin.ModLogger.LogInfo("Changing stacking behaviour");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			MainPlugin.ModLogger.LogInfo("Changing proc behaviour");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			MainPlugin.ModLogger.LogInfo("Changing health bar behavior");
			IL.RoR2.HealthComponent.GetHealthBarValues += new ILContext.Manipulator(IL_GetHealthBarValues);
			GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			On.RoR2.CharacterBody.RecalculateStats += OnRecalculateStats;
		}
		private void OnRecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				Inventory inventory = self.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(BloodCloneItem);
					if (itemCount < 1)
					{
						itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
						if (itemCount > 0)
						{
							UpdateClone(self);
						}
					}
				}
			}
		}
		internal void UpdateClone(CharacterBody self)
        {
			CharacterMaster master = self.master;
			if (master)
			{
				Inventory inventory = self.inventory;
				if (inventory)
				{
					if (inventory.infusionBonus > CloneCost)
					{
						if (master.GetDeployableCount(InfusionDeployable) < 1)
						{
							CreateClone(master, self);
						}
						else
                        {
							CharacterMaster clone = GetClone(master);
							if (clone)
                            {
								FeedClone(clone, inventory);
							}
                        }
					}
				}
			}
		}
		private void CreateClone(CharacterMaster ownerMaster, CharacterBody ownerBody)
        {
			if (ownerBody && ownerMaster)
            {
				MasterCopySpawnCard spawnCard = MasterCopySpawnCard.FromMaster(ownerMaster, false, false, null);
				if (!spawnCard)
				{
					return;
				}
				spawnCard.GiveItem(RoR2Content.Items.MinionLeash, 1);
				spawnCard.GiveItem(BloodCloneItem, 1);
				spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;

				DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Approximate,
					minDistance = 8f,
					maxDistance = 24f,
					position = ownerBody.corePosition
				}, RoR2Application.rng);
				directorSpawnRequest.summonerBodyObject = ownerBody.gameObject;
				directorSpawnRequest.teamIndexOverride = ownerMaster.teamIndex;
				directorSpawnRequest.ignoreTeamMemberLimit = true;

				directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
				{
					CharacterMaster summonMaster = result.spawnedInstance.GetComponent<CharacterMaster>();
					Deployable deployable = result.spawnedInstance.AddComponent<Deployable>();
					ownerMaster.AddDeployable(deployable, InfusionDeployable);
					deployable.onUndeploy = (deployable.onUndeploy ?? new UnityEvent());
					deployable.onUndeploy.AddListener(new UnityAction(summonMaster.TrueKill));
					GameObject bodyObject = summonMaster.GetBodyObject();
					if (bodyObject)
					{
						foreach (EntityStateMachine entityStateMachine in bodyObject.GetComponents<EntityStateMachine>())
						{
							if (entityStateMachine.customName == "Body")
							{
								entityStateMachine.SetState(new GummyCloneSpawnState());
								return;
							}
						}
					}
					FeedClone(summonMaster, ownerMaster.inventory);
				}));
				DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
				UnityEngine.Object.Destroy(spawnCard);
			}
        }
		private CharacterMaster GetClone(CharacterMaster owner)
        {
			CharacterMaster clone = null;
			if (owner.deployablesList != null)
			{
				int maxSummons = 1;
				int curSummons = 0;
				for (int i = 0; i < owner.deployablesList.Count; i++)
				{
					if (owner.deployablesList[i].slot == InfusionDeployable)
					{
						Deployable deploy = owner.deployablesList[i].deployable;
						if (deploy)
						{
							CharacterMaster master = deploy.GetComponent<CharacterMaster>();
							if (master)
							{
								if (master.teamIndex == owner.teamIndex)
								{
									curSummons += 1;
									if (curSummons > maxSummons)
									{
										master.TrueKill();
									}
									else
									{
										clone = master;
									}
								}
							}
						}
					}
				}
			}
			return clone;
		}
		private void FeedClone(CharacterMaster clone, Inventory inventory)
        {
			if (!NetworkServer.active)
			{
				return;
			}
			if (clone && inventory)
            {
				Inventory cloneInv = clone.inventory;
				if (cloneInv)
                {
					cloneInv.AddInfusionBonus(inventory.infusionBonus);
					inventory.infusionBonus = 0;
				}
            }
		}
		private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			CharacterBody attacker = damageReport.attackerBody;
			if (attacker)
			{
				Inventory inventory = attacker.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
					if (itemCount > 0)
					{
						float mult = BaseGain + (StackGain * (itemCount - 1));
						
						CharacterBody victimBody = damageReport.victimBody;
						float blood = GetBaseBloodValue(victimBody);
						blood *= mult;
						GiveInfusionOrb(attacker, victimBody, (int)Math.Ceiling(blood));
					}
				}
			}
		}
		private float GetBaseBloodValue(CharacterBody body)
		{
			if (body.master)
			{
				return (body.baseMaxHealth + body.baseMaxShield) * 0.5f;
			}
			return FakeBaseGain;
		}
		private void GiveInfusionOrb(CharacterBody target, CharacterBody victim,  int amount)
        {
			if (amount > 0)
			{
				RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
				infusionOrb.origin = victim.gameObject.transform.position;
				infusionOrb.target = Util.FindBodyMainHurtBox(target);
				infusionOrb.maxHpValue = amount;
				RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
			}
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
			);
			ilcursor.Index += 2;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
		}
		private void IL_RecalculateStats(ILContext il)
		{
			//Kill old function
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
			);
			ilcursor.Index -= 2;
			ilcursor.RemoveRange(5);
			//Add new
			ilcursor.GotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdarg(0),
				x => x.MatchCallOrCallvirt<CharacterBody>("get_level"),
				x => x.MatchLdloc(2),
				x => x.MatchConvR4(),
				x => x.MatchAdd(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_level")
			);
			ilcursor.Index += 5;
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
			{
				float levelBonus = 0.0f;
				if (self.inventory)
				{
					int itemCount = self.inventory.GetItemCount(BloodCloneItem);
					if (itemCount > 0)
					{
						float bloodBonus = (int)self.inventory.infusionBonus - CloneCost;
						if (bloodBonus > 0)
						{
							levelBonus += self.level * (bloodBonus / LevelCost);
						}
					}
				}
				return levelBonus;
			});
			ilcursor.Emit(OpCodes.Add);
		}
		private void IL_GetHealthBarValues(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", nameof(RoR2.HealthComponent.itemCounts.infusion)),
				x => x.MatchConvR4(),
				x => x.MatchLdcR4(0f),
				x => x.MatchCgt()
			);
			ilcursor.Index -= 1;
			ilcursor.RemoveRange(5);
			ilcursor.EmitDelegate<Func<HealthComponent, bool>>((self) =>
			{
				if (self.body)
                {
					if (self.body.inventory)
                    {
						return self.body.inventory.GetItemCount(BloodCloneItem) > 0;
                    }
				}
				return false;
			});
		}
	}
}
