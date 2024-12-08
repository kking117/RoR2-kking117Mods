using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.Projectile;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
	public class SymbioticScorpion_Rework
	{
		public static BuffDef VenomBuff;
		private static Color BuffColor = new Color(0.694f, 0.219f, 0.498f, 1f);
		public static DotController.DotDef VenomDotDef;
		private static DotController.DotIndex VenomDotIndex;
		public static GameObject OrbVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoDiseaseOrbEffect.prefab").WaitForCompletion();
		internal static bool Enable = false;
		internal static float Slayer_BaseDamage = 2f;
		internal static float Slayer_StackDamage = 0f;
		internal static bool SlayerDot_AffectTotalDamage = false;
		internal static float Radius = 13f;
		internal static float VenomBaseDamage = 6f;
		internal static float VenomStackDamage = 6f;
		internal static float Cooldown = 5f;
		internal static DamageAPI.ModdedDamageType ScorpionVenomOnHit;

		public SymbioticScorpion_Rework()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo("Changing Symbiotic Scorpion");
			ClampConfig();
			UpdateItemDef();
			CreateBuff();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			Slayer_BaseDamage = Math.Max(0f, Slayer_BaseDamage);
			Slayer_StackDamage = Math.Max(0f, Slayer_StackDamage);
			VenomBaseDamage = Math.Max(0f, VenomBaseDamage);
			VenomStackDamage = Math.Max(0f, VenomStackDamage);
			Radius = Math.Max(0f, Radius);
			Cooldown = Math.Max(0.5f, Cooldown);
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/PermanentDebuffOnHit/PermanentDebuffOnHit.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.AIBlacklist);
				itemTags.Add(ItemTag.BrotherBlacklist);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void CreateBuff()
		{
			BuffDef refBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/PermanentDebuffOnHit/bdPermanentDebuff.asset").WaitForCompletion();
			VenomBuff = Utils.ContentManager.AddBuff("Venom", refBuff.iconSprite, BuffColor, true, false, false);
			VenomDotDef = new DotController.DotDef
			{
				associatedBuff = VenomBuff,
				damageCoefficient = 1f / 4f,
				damageColorIndex = DamageColorIndex.Poison,
				interval = 0.25f
			};
			VenomDotIndex = DotAPI.RegisterDotDef(VenomDotDef);
			ScorpionVenomOnHit = DamageAPI.ReserveDamageType();
			OrbAPI.AddOrb(typeof(Orbs.ScorpionOrb));
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string description = "";
			string slayer_pickup = "";
			string venom_pickup = "";
			string slayer_description = "";
			string venom_description = "";
			if (Slayer_BaseDamage > 0f)
			{
				slayer_pickup = "Damage over time deals more damage to enemies with less health.";
				if (Slayer_StackDamage > 0f)
                {
					slayer_description = string.Format("Damage over time deals <style=cIsDamage>+{0}% <style=cStack>(+{1}% per stack)</style> damage</style> per <style=cIsHealth>1% health the enemy is missing</style>.", Slayer_BaseDamage, Slayer_StackDamage);
				}
				else
                {
					slayer_description = string.Format("Damage over time deals <style=cIsDamage>+{0}% damage</style> per <style=cIsHealth>1% health the enemy is missing</style>.", Slayer_BaseDamage);
				}
			}
			if (Radius > 0f)
			{
				if (Slayer_BaseDamage > 0f)
				{
					venom_pickup = " ";
					venom_description = " ";
				}
				venom_pickup += "Periodically envenom a nearby enemy.";
				if (VenomStackDamage > 0f)
				{
					venom_description += string.Format("Every <style=cIsUtility>{0}s</style> <style=cIsDamage>Envenom</style> an enemy within <style=cIsDamage>{1}m</style> for <style=cIsDamage>{2}% <style=cStack>(+{3}% per stack)</style></style> base damage.", Cooldown, Radius, VenomBaseDamage * 100f, VenomStackDamage * 100f);
				}
				else
				{
					venom_description += string.Format("Every <style=cIsUtility>{0}s</style> <style=cIsDamage>Envenom</style> an enemy within <style=cIsDamage>{1}m</style> for <style=cIsDamage>{2}%</style> base damage.", Cooldown, Radius, VenomBaseDamage * 100f);
				}
			}
			pickup = string.Format("{0}{1}", slayer_pickup, venom_pickup);
			description = string.Format("{0}{1}", slayer_description, venom_description);
			LanguageAPI.Add("ITEM_PERMANENTDEBUFFONHIT_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PERMANENTDEBUFFONHIT_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_OnTakeDamage);
			if (Radius > 0f)
			{
				SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
			}
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCount = self.inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit);
			self.AddItemBehavior<Behaviors.SymbioticScorpion_Rework>(itemCount);
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			float procRate = damageReport.damageInfo.procCoefficient;
			if (procRate > 0f)
			{
				CharacterBody victimBody = damageReport.victimBody;
				CharacterBody attackerBody = damageReport.attackerBody;
				if (attackerBody && victimBody)
				{
					if (damageReport.damageInfo.HasModdedDamageType(ScorpionVenomOnHit) || Helpers.InflictorHasModdedDamageType(damageReport.damageInfo.inflictor, ScorpionVenomOnHit))
					{
						int itemCount = 1;
						Inventory inventory = damageReport.attackerBody.inventory;
						if (inventory)
						{
							itemCount = inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit);
						}
						DotController.InflictDot(victimBody.gameObject, attackerBody.gameObject, VenomDotIndex, VenomDuration(itemCount) * procRate, 1f);
					}
				}
			}
		}
		
		private float VenomDuration(int itemCount)
		{
			itemCount = Math.Max(0, itemCount - 1);
			return VenomBaseDamage + (VenomStackDamage * itemCount);
		}
		private void IL_OnTakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (Slayer_BaseDamage > 0f)
            {
				if (ilcursor.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld(typeof(DamageInfo), "damage"),
					x => x.MatchStloc(7)
				))
				{
					ilcursor.Index += 3;
					ilcursor.Emit(OpCodes.Ldarg, 0);
					ilcursor.Emit(OpCodes.Ldarg, 1);
					ilcursor.Emit(OpCodes.Ldloc, 7);
					ilcursor.EmitDelegate<Func<HealthComponent, DamageInfo, float, float>>((self, damageInfo, damage) =>
					{
						if (damageInfo.dotIndex != DotController.DotIndex.None)
						{
							if (damageInfo.attacker)
                            {
								CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
								if (attackerBody && attackerBody.inventory)
                                {
									int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.PermanentDebuffOnHit);
									if (itemCount > 0)
                                    {
										itemCount = Math.Max(0, itemCount - 1);
										float slayerDmg = Slayer_BaseDamage + (itemCount * Slayer_StackDamage);
										if (slayerDmg > 0f)
										{
											float dmgMult = Mathf.Lerp(1f + slayerDmg, 1f, self.combinedHealthFraction);
											if (SlayerDot_AffectTotalDamage)
											{
												damageInfo.damage *= dmgMult;
											}
											return damage * dmgMult;
										}
                                    }
								}
							}
						}
						return damage;
					});
					ilcursor.Emit(OpCodes.Stloc, 7);
				}
				else
				{
					UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Symbiotic Scorpion - DoT Damage - IL Hook failed");
				}
			}
				
			if(ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC1Content.Items), "PermanentDebuffOnHit"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
            {
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Symbiotic Scorpion - Effect Override - IL Hook failed");
			}
		}
	}
}
