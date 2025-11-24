using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
	public class HappiestMask_Rework
	{
		//"RoR2/Base/DeathProjectile/DeathProjectileTickEffect.prefab"
		public static GameObject GhostSpawnEffect = Addressables.LoadAssetAsync<GameObject>("3e7d16b341412e64db6467a3682cb0ac").WaitForCompletion();
		public static ItemDef GhostCloneIdentifier;
		public static DeployableSlot Ghost_DeployableSlot;
		public DeployableAPI.GetDeployableSameSlotLimit Ghost_DeployableLimit;
		private const string LogName = "Happiest Mask Rework";
		internal static bool Enable = false;
		internal static float BaseDamage = 2f;
		internal static float StackDamage = 3f;
		internal static float BaseMoveSpeed = 0.45f;
		internal static int BaseDuration = 30;
		internal static int BaseCooldown = 3;
		internal static bool OnKillOnDeath = true;
		internal static bool PassKillCredit = true;
		public HappiestMask_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			CreateDeployableSlot();
			UpdateItemDef();
			CreateItemDef();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseMoveSpeed = Math.Max(0f, BaseMoveSpeed);
			BaseDuration = Math.Max(0, BaseDuration);
			BaseCooldown = Math.Max(0, BaseCooldown);
		}
		private void CreateDeployableSlot()
        {
			Ghost_DeployableSlot = DeployableAPI.RegisterDeployableSlot(new DeployableAPI.GetDeployableSameSlotLimit(GetGhost_DeployableLimit));
		}
		private int GetGhost_DeployableLimit(CharacterMaster self, int deployableMult)
        {
			return 1 * deployableMult;
        }
		private void UpdateItemDef()
		{
			//"RoR2/Base/GhostOnKill/GhostOnKill.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("732b3b5bb70153148a4f8060a8c3cfc9").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.CannotCopy);
				itemTags.Add(ItemTag.AIBlacklist);
				itemTags.Remove(ItemTag.OnKillEffect);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void CreateItemDef()
		{
			GhostCloneIdentifier = ScriptableObject.CreateInstance<ItemDef>();
			GhostCloneIdentifier.canRemove = false;
			GhostCloneIdentifier.name = MainPlugin.MODNAME + "_GhostCloneIdentifier";
			GhostCloneIdentifier.deprecatedTier = ItemTier.NoTier;
			GhostCloneIdentifier.tier = ItemTier.NoTier;
			GhostCloneIdentifier.descriptionToken = "";
			GhostCloneIdentifier.nameToken = MainPlugin.MODNAME + "_GhostCloneIdentifier";
			GhostCloneIdentifier.pickupToken = "";
			GhostCloneIdentifier.hidden = true;
			GhostCloneIdentifier.pickupIconSprite = null;
			GhostCloneIdentifier.tags = new[]
			{
				ItemTag.BrotherBlacklist,
				ItemTag.CannotSteal,
				ItemTag.CannotDuplicate,
				ItemTag.AIBlacklist
			};
			ContentManager.AddItem(GhostCloneIdentifier);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "";
			string description = "";
			string baseDmg = "";
			string deathFX = "";
			string duration = "";
			float displaydmg = (1f + BaseDamage) * 100f;
			if (BaseDuration > 0f)
            {
				duration = string.Format(" for <style=cIsUtility>{0}s</style>", BaseDuration);
			}
			if (StackDamage > 0f || BaseDamage != 1f)
            {
				if (StackDamage > 0f)
                {
					baseDmg = string.Format(" with <style=cIsDamage>{0}%</style><style=cStack> (+{1}% per stack)</style> damage", displaydmg, StackDamage * 100f);
				}
				else
                {
					baseDmg = string.Format(" with <style=cIsDamage>{0}%</style> damage", displaydmg);
				}
            }
			if (OnKillOnDeath || PassKillCredit)
            {
				deathFX = " The ghost's";
				if (OnKillOnDeath && PassKillCredit)
                {
					deathFX += " <style=cIsDamage>kills</style> and <style=cIsHealth>deaths</style> are <style=cIsDamage>credited as your kills</style>.";
                }
				else if(PassKillCredit)
                {
					deathFX += " <style=cIsDamage>kills</style> are <style=cIsDamage>credited as your own</style>.";
				}
				else
                {
					deathFX += " <style=cIsHealth>deaths</style> are <style=cIsDamage>credited as your kills</style>.";
				}
            }
			pickup = "Summon a ghostly doppelganger.";
			description = string.Format("Summon a <style=cIsUtility>ghostly doppelganger</style>{0}{1}.{2}", duration, baseDmg, deathFX);
			LanguageAPI.Add("ITEM_GHOSTONKILL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_GHOSTONKILL_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			if (PassKillCredit)
            {
				On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
			}
			if (OnKillOnDeath)
			{
				SharedHooks.Handle_CharacterMaster_OnBodyDeath_Actions += CharacterMasterBodyDeath;
			}
		}
		private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager instance, DamageReport damageReport)
		{
			CharacterMaster attackerMaster = damageReport.attackerMaster;
			CharacterMaster victimMaster = damageReport.victimMaster;
			CharacterBody victimBody = damageReport.victimBody;
			if (!victimMaster || victimMaster != attackerMaster)
			{
				if (attackerMaster)
				{
					bool attackerIsGhost = attackerMaster.inventory.GetItemCountEffective(GhostCloneIdentifier) > 0;
					if (attackerIsGhost)
					{
						CharacterMaster ownerMaster = GetGhostOwner(attackerMaster);
						if (attackerMaster != ownerMaster)
						{
							if (SetNewDamageReportAttacker(ownerMaster, damageReport))
							{
								return;
							}
						}
					}
				}
			}
			orig(instance, damageReport);
		}
		private void CharacterMasterBodyDeath(CharacterMaster victimMaster, CharacterBody victimBody)
		{
			int itemCount = victimMaster.inventory.GetItemCountEffective(GhostCloneIdentifier);
			if (itemCount > 0)
			{
				CharacterMaster attackerMaster = Utils.Helpers.GetOwnerAsDeployable(victimMaster, Ghost_DeployableSlot);
				if (attackerMaster && attackerMaster != victimMaster)
				{
					CharacterBody attackerBody = attackerMaster.GetBody();
					if (attackerBody)
					{
						DamageInfo damageInfo = new DamageInfo();
						damageInfo.attacker = attackerBody.gameObject;
						damageInfo.damage = attackerBody.damage * GetGhostDamageMult(itemCount);
						damageInfo.damageColorIndex = DamageColorIndex.Item;
						damageInfo.position = Util.GetCorePosition(victimBody);
						damageInfo.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
						DamageReport damageReport = new DamageReport(damageInfo, victimBody.healthComponent, attackerBody.damage, 0f);

						GlobalEventManager.instance.OnCharacterDeath(damageReport);
					}
				}
			}
		}
		
		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCountA = self.inventory.GetItemCountEffective(GhostCloneIdentifier);
			int itemCountB = self.inventory.GetItemCountEffective(RoR2Content.Items.GhostOnKill);
			if (itemCountA > 0)
			{
				itemCountB = 0;
			}
			self.AddItemBehavior<Behaviors.GhostCloneIdentifier>(itemCountA);
			self.AddItemBehavior<Behaviors.HappiestMask_Rework>(itemCountB);
		}

		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
			int itemCount = inventory.GetItemCountEffective(GhostCloneIdentifier);
			if (itemCount > 0)
			{
				args.damageMultAdd += GetGhostDamageMult(itemCount, 0f);
				args.moveSpeedMultAdd += BaseMoveSpeed;
			}
		}

		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "GhostOnKill"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective")
			))
            {
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnCharacterDeath - Hook failed");
			}
		}

		private CharacterMaster GetGhostOwner(CharacterMaster master)
        {
			CharacterMaster owner = Utils.Helpers.GetOwnerAsDeployable(master, Ghost_DeployableSlot);
			if (owner)
            {
				if (owner != master)
                {
					return owner;
                }
            }
			return master;
        }
		private bool SetNewDamageReportAttacker(CharacterMaster attackerMaster, DamageReport damageReport)
        {
			if (attackerMaster)
			{
				CharacterBody attackerBody = attackerMaster.GetBody();
				if (attackerBody)
				{
					DamageInfo damageInfo = damageReport.damageInfo;
					DamageInfo newInfo = new DamageInfo();
					newInfo.attacker = attackerBody.gameObject;
					newInfo.canRejectForce = damageInfo.canRejectForce;
					newInfo.crit = damageInfo.crit;
					newInfo.damage = damageInfo.damage;
					newInfo.damageColorIndex = damageInfo.damageColorIndex;
					newInfo.damageType = damageInfo.damageType;
					newInfo.dotIndex = damageInfo.dotIndex;
					newInfo.force = damageInfo.force;
					newInfo.inflictor = damageInfo.inflictor;
					newInfo.position = damageInfo.position;
					newInfo.procChainMask = damageInfo.procChainMask;
					newInfo.procCoefficient = damageInfo.procCoefficient;
					newInfo.rejected = damageInfo.rejected;
					DamageReport newReport = new DamageReport(newInfo, damageReport.victim, damageReport.damageInfo.damage, damageReport.combinedHealthBeforeDamage);
					GlobalEventManager.instance.OnCharacterDeath(newReport);
					return true;
				}
			}
			return false;
        }
		private float GetGhostDamageMult(int stacks, float baseValue = 1f, float finalMult = 1f)
        {
			stacks = Math.Max(0, stacks - 1);
			float result = baseValue + (BaseDamage + (StackDamage * stacks));
			return result * finalMult;
        }
	}
}
