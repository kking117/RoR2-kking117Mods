using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class IgnitionTank_Rework
	{
		private const string LogName = "Ignition Tank Rework";

		internal static bool Enable = false;
		internal static float BurnChance = 10f;
		internal static float BurnBaseDamage = 0.8f;
		internal static float BurnDuration = 3f;

		internal static float BlastChance = 0f;
		internal static int BlastTicks = 10;
		internal static int HalfBlastTicks = 5;
		internal static float BlastBaseDamage = 3f;
		internal static float BlastStackDamage = 2f;
		internal static float BlastBaseRadius = 12f;
		internal static float BlastStackRadius = 2.4f;
		internal static bool BlastInheritDamageType = false;
		internal static bool BlastRollCrit = false;

		private static GameObject BlastEffect;
		public IgnitionTank_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateVFX();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			Hooks();
		}
		private void ClampConfig()
		{
			BurnChance = Math.Max(0f, BurnChance);
			BurnBaseDamage = Math.Max(0f, BurnBaseDamage);
			BurnDuration = Math.Max(0, BurnDuration);

			BlastTicks = Math.Max(0, BlastTicks);
			if (BlastTicks > 0)
            {
				HalfBlastTicks = Math.Max(0, BlastTicks / 2);
			}
			BlastBaseDamage = Math.Max(0f, BlastBaseDamage);
			BlastStackDamage = Math.Max(0f, BlastStackDamage);
			BlastBaseRadius = Math.Max(0f, BlastBaseRadius);
			BlastStackRadius = Math.Max(0f, BlastStackRadius);
		}
		private void UpdateVFX()
        {
			//"RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab"
			BlastEffect = Addressables.LoadAssetAsync<GameObject>("fd33680df35a2ab4db22b33a0e161f90").WaitForCompletion();
		}
		private void UpdateText()
		{
			string burnPickup = "";
			string burnDesc = "";

			string blastPickup = "";
			string blastDesc = "";
			
			string blastCondition = "";
			string blastRadius = "";
			string blastDamage = "";

			if (BurnChance > 0f)
            {
				burnPickup += "Chance to burn enemies on hit.";
				burnDesc += string.Format("<style=cIsDamage>{0}%</style> chance to <style=cIsDamage>burn</style> an enemy for <style=cIsDamage>{1}%</style> base damage.", BurnChance, BurnBaseDamage * BurnDuration * 100f);
				if (BlastTicks > 0)
                {
					blastPickup += " ";
					blastCondition += " ";
				}
			}
			if (BlastChance > 0f)
            {
				blastPickup += "Damage over time causes explosions.";
				blastCondition += string.Format("Damage over time has a <style=cIsDamage>{0}%</style> chance to", BlastChance);

				blastRadius = string.Format(" <style=cIsDamage>explode</style> in a <style=cIsDamage>{0}m</style>", BlastBaseRadius);
				if (BlastStackRadius > 0f)
				{
					blastRadius += string.Format(" <style=cStack>(+{0}m per stack)</style>", BlastStackRadius);
				}
				blastRadius += " radius";

				blastDamage = string.Format(" for <style=cIsDamage>{0}%</style>", BlastBaseDamage * 100f);
				if (BlastStackDamage > 0f)
				{
					blastDamage += string.Format(" <style=cStack>(+{0}% per stack)</style>", BlastStackDamage * 100f);
				}
				blastDamage += " TOTAL damage";
			}
			else
            {
				if (BlastTicks > 0)
				{
					blastPickup += "Damage over time causes explosions.";
					if (BlastTicks > 1)
					{
						blastCondition += string.Format("Every <style=cIsDamage>{0} hits</style> of damage over time causes", BlastTicks);
					}
					else
					{
						blastCondition += string.Format("Damage over time causes");
					}

					blastRadius = string.Format(" an <style=cIsDamage>explosion</style> in a <style=cIsDamage>{0}m</style>", BlastBaseRadius);
					if (BlastStackRadius > 0f)
					{
						blastRadius += string.Format(" <style=cStack>(+{0}m per stack)</style>", BlastStackRadius);
					}
					blastRadius += " radius";

					blastDamage = string.Format(" for <style=cIsDamage>{0}%</style>", BlastBaseDamage * 100f);
					if (BlastStackDamage > 0f)
					{
						blastDamage += string.Format(" <style=cStack>(+{0}% per stack)</style>", BlastStackDamage * 100f);
					}
					blastDamage += " TOTAL damage";
				}
			}
			blastDesc += string.Format("{0}{1}{2}.", blastCondition, blastRadius, blastDamage);

			string pickupText = string.Format("{0}{1}", burnPickup, blastPickup);
			string descriptionText = string.Format("{0}{1}", burnDesc, blastDesc);

			LanguageAPI.Add("ITEM_STRENGTHENBURN_PICKUP", pickupText);
			LanguageAPI.Add("ITEM_STRENGTHENBURN_DESC", descriptionText);
		}
		private void Hooks()
		{
			if (BlastChance > 0f)
            {
				SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent_Chance;
			}
			else
            {
				SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			}
			On.RoR2.StrengthenBurnUtils.CheckDotForUpgrade += CheckBurnUpgrade;
		}
		private void CheckBurnUpgrade(On.RoR2.StrengthenBurnUtils.orig_CheckDotForUpgrade orig, Inventory inventory, ref InflictDotInfo dotInfo)
        {
			//this should only be used by mods that upgrade a dot using ignition tank so this should be fine to mute like this
			return;
        }
		private void GlobalDamageEvent_Chance(DamageReport damageReport)
		{
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			float procRate = damageReport.damageInfo.procCoefficient;
			bool isDot = damageReport.dotType != DotController.DotIndex.None;
			if (attackerBody && victimBody)
			{
				Inventory inventory = damageReport.attackerBody.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCountEffective(DLC1Content.Items.StrengthenBurn);
					if (itemCount > 0)
					{
						
						if (BurnChance > 0f && Util.CheckRoll(procRate * BurnChance, damageReport.attackerMaster))
						{
							float baseDamage = attackerBody.damage * BurnDuration * 0.5f;
							float dmgMult = 1f + BurnBaseDamage;
							InflictDotInfo inflictDotInfo = new InflictDotInfo
							{
								victimObject = damageReport.victim.gameObject,
								attackerObject = damageReport.attacker,
								totalDamage = baseDamage * dmgMult,
								dotIndex = DotController.DotIndex.Burn,
								damageMultiplier = dmgMult
							};
							DotController.InflictDot(ref inflictDotInfo);
						}
						if (isDot)
						{
							float baseChance = Math.Max(1f, procRate);
							if (Util.CheckRoll(baseChance * BlastChance, damageReport.attackerMaster))
							{
								DoTankExplosion(attackerBody, victimBody, itemCount, damageReport.damageInfo);
							}
						}
					}
				}
			}
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			float procRate = damageReport.damageInfo.procCoefficient;
			bool isDot = damageReport.dotType != DotController.DotIndex.None;
			if (attackerBody && victimBody)
			{
				Inventory inventory = damageReport.attackerBody.inventory;
				if (inventory)
				{
					int itemCount = inventory.GetItemCountEffective(DLC1Content.Items.StrengthenBurn);
					if (itemCount > 0)
					{
						if (BurnChance > 0f && Util.CheckRoll(procRate * BurnChance, damageReport.attackerMaster))
						{
							float baseDamage = attackerBody.damage * BurnDuration * 0.5f;
							float dmgMult = 1f + BurnBaseDamage;
							InflictDotInfo inflictDotInfo = new InflictDotInfo
							{
								victimObject = damageReport.victim.gameObject,
								attackerObject = damageReport.attacker,
								totalDamage = baseDamage * dmgMult,
								dotIndex = DotController.DotIndex.Burn,
								damageMultiplier = dmgMult
							};
							DotController.InflictDot(ref inflictDotInfo);
						}
						if (isDot)
						{
							if (TickUpExplosion(victimBody, attackerBody))
                            {
								DoTankExplosion(attackerBody, victimBody, itemCount, damageReport.damageInfo);
							}
						}
					}
				}
			}
		}
		private void DoTankExplosion(CharacterBody attackerBody, CharacterBody victimBody, int itemCount, DamageInfo damageInfo)
        {
			itemCount = Math.Max(0, itemCount - 1);
			float blastDamage = BlastBaseDamage + (BlastStackDamage * itemCount);
			float blastRadius = BlastBaseRadius + (BlastStackRadius * itemCount) + victimBody.radius;
			blastDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, blastDamage);
			if (blastDamage > 0f)
			{
				bool isCrit = damageInfo.crit;
				if (BlastRollCrit && !isCrit)
				{
					isCrit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
				}
				Vector3 blastPosition = victimBody.transform.position;
				GameObject blastVFX = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), blastPosition, Quaternion.identity);
				blastVFX.transform.localScale = new Vector3(blastRadius, blastRadius, blastRadius);
				DelayBlast delayBlast = blastVFX.GetComponent<DelayBlast>();
				if (delayBlast)
				{
					delayBlast.position = blastPosition;
					delayBlast.radius = blastRadius;
					delayBlast.baseDamage = blastDamage;
					delayBlast.baseForce = 0f;
					delayBlast.procCoefficient = 0f;
					delayBlast.crit = isCrit;
					delayBlast.falloffModel = BlastAttack.FalloffModel.SweetSpot;
					delayBlast.maxTimer = 0f;
					delayBlast.attacker = attackerBody.gameObject;
					delayBlast.inflictor = null;
					delayBlast.teamFilter.teamIndex = attackerBody.teamComponent.teamIndex;
					delayBlast.explosionEffect = BlastEffect;
					if (BlastInheritDamageType)
					{
						delayBlast.damageType = damageInfo.damageType;
						delayBlast.damageColorIndex = damageInfo.damageColorIndex;
					}
					else
					{
						delayBlast.damageColorIndex = DamageColorIndex.Item;
					}
					TeamFilter teamFilter = blastVFX.GetComponent<TeamFilter>();
					if (teamFilter)
					{
						teamFilter.teamIndex = attackerBody.teamComponent.teamIndex;
					}
					NetworkServer.Spawn(blastVFX);
				}
			}
		}

		private bool TickUpExplosion(CharacterBody victimBody, CharacterBody attackerBody)
        {
			if (BlastTicks > 1)
			{
				Components.IgnitionTankTracker comp = victimBody.GetComponent<Components.IgnitionTankTracker>();
				if (!comp)
				{
					comp = victimBody.gameObject.AddComponent<Components.IgnitionTankTracker>();
				}
				return comp.TickUp(attackerBody.master);
			}
			return true;
		}
	}
}
