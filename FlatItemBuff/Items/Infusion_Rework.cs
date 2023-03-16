using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Infusion_Rework
	{
		public static ItemDef BloodCloneItem;
		internal static DeployableSlot InfusionDeployable = (DeployableSlot)209063;
		internal static bool Enable = false;
		internal static float BaseGain = 1f;
		internal static float StackGain = 0.5f;
		internal static int FakeBaseGain = 10;
		internal static int CloneCost = 400;
		internal static int LevelCost = 4000;
		internal static float CustomLeash = 90f;
		public Infusion_Rework()
		{
			if (!Enable)
            {
				new Infusion();
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Infusion");
			CustomLeash *= CustomLeash;
			UpdateItemDef();
			CreateItemDef();
			UpdateText();
			Hooks();
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
			SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCount = self.inventory.GetItemCount(BloodCloneItem);
			if (itemCount > 0 && CustomLeash > 0f)
			{
				self.AddItemBehavior<Behaviors.BloodClone>(itemCount);
			}
			else
            {
				self.AddItemBehavior<Behaviors.Infusion_Rework>(self.inventory.GetItemCount(RoR2Content.Items.Infusion));
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			Inventory inventory = attackerBody.inventory;
			if (inventory)
			{
				int itemCount = inventory.GetItemCount(RoR2Content.Items.Infusion);
				if (itemCount > 0)
				{
					float mult = BaseGain + (StackGain * (itemCount - 1));

					CharacterBody victimBody = damageReport.victimBody;
					float blood = GetBaseBloodValue(victimBody);
					blood *= mult;
					GiveInfusionOrb(attackerBody, victimBody, (int)Math.Ceiling(blood));
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
		private bool IsValidBloodClone(CharacterMaster self)
		{
			Deployable deployable = self.GetComponent<Deployable>();
			if (deployable)
			{
				CharacterMaster owner = deployable.ownerMaster;
				if (owner)
				{
					if(owner.inventory)
                    {
						if (owner.inventory.GetItemCount(BloodCloneItem) > 0)
						{
							return true;
						}
                    }
					if (owner.deployablesList != null)
					{
						for (int i = 0; i < owner.deployablesList.Count; i++)
						{
							if (owner.deployablesList[i].slot == InfusionDeployable)
							{
								if (owner.deployablesList[i].deployable == deployable)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
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
				Inventory inventory = self.inventory;
				if (inventory)
				{
					CharacterMaster master = self.master;
					int itemCount = inventory.GetItemCount(BloodCloneItem);
					if (itemCount > 0)
					{
						float bloodBonus = (int)inventory.infusionBonus - CloneCost;
						if (bloodBonus > 0)
						{
							levelBonus += self.level * (bloodBonus / LevelCost);
						}
						MasterSuicideOnTimer component = master.gameObject.GetComponent<MasterSuicideOnTimer>();
						if (component)
						{
							component.hasDied = false;
						}
						else
                        {
							if (!IsValidBloodClone(master))
							{
								master.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = 5f;
							}
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
