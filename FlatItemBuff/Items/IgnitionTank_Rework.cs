using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using EntityStates;
using UnityEngine;

namespace FlatItemBuff.Items
{
	public class IgnitionTank_Rework
	{
		internal static bool Enable = false;
		internal static float BurnChance = 10f;
		internal static float BurnBaseDamage = 0.8f;
		internal static float BurnDuration = 3f;

		internal static float BlastChance = 10f;
		internal static float BlastBaseDamage = 2.5f;
		internal static float BlastStackDamage = 2.0f;
		internal static float BlastBaseRadius = 12f;
		internal static float BlastStackRadius = 2.4f;
		internal static bool BlastAddBodyRadius = false;
		internal static bool BlastScaleTickrate = false;
		internal static bool BlastInheritDamageType = false;
		internal static bool BlastRollCrit = true;
		public IgnitionTank_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Ignition Tank");
			UpdateText();
			Hooks();
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string burnDesc = "";
			string blastDesc = "";

			string blastRadius = "";
			string blastDamage = "";

			if (BurnChance > 0f)
            {
				burnDesc += string.Format("<style=cIsDamage>{0}%</style> chance to <style=cIsDamage>burn</style> an enemy for <style=cIsDamage>{1}%</style> base damage.", BurnChance, BurnBaseDamage * BurnDuration * 100f);
				blastDesc += " ";
			}

			blastRadius = string.Format(" <style=cIsDamage>explosion</style> in a <style=cIsDamage>{0}m</style>", BlastBaseRadius);
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

			blastDesc += string.Format("When dealing <style=cIsUtility>status damage</style> you have a <style=cIsDamage>{0}%</style> chance to cause an{1}{2}.", BlastChance, blastRadius, blastDamage);

			string pickup = "Chance to burn enemies on hit. Status damage has a chance to cause explosions.";
			string desc = string.Format("{0}{1}", burnDesc, blastDesc);

			LanguageAPI.Add("ITEM_STRENGTHENBURN_PICKUP", pickup);
			LanguageAPI.Add("ITEM_STRENGTHENBURN_DESC", desc);
		}
		private void Hooks()
		{
			if (BurnChance > 0f)
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
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			float procrate = damageReport.damageInfo.procCoefficient;
			Inventory inventory = damageReport.attackerBody.inventory;
			CharacterBody victimBody = damageReport.victimBody;
			CharacterBody attackerBody = damageReport.attackerBody;
			bool isDot = damageReport.dotType != DotController.DotIndex.None;

			if (victimBody)
			{
				if (attackerBody && inventory)
				{
					int itemCount = inventory.GetItemCount(DLC1Content.Items.StrengthenBurn);
					if (itemCount > 0)
					{
						if (procrate > 0f)
						{
							if (Util.CheckRoll(procrate * BurnChance, damageReport.attackerMaster))
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
						}
						if (isDot)
                        {
							TryTankExplosion(attackerBody, victimBody, victimBody.transform.position, itemCount, damageReport);
						}
					}
				}
			}
		}
		private void TryTankExplosion(CharacterBody attackerBody, CharacterBody victimBody, Vector3 blastLocation, int itemCount, DamageReport damageReport)
        {
			float tickMult = 1f;
			if (BlastScaleTickrate)
            {
				if (damageReport.dotType != DotController.DotIndex.None)
				{
					DotController.DotDef dotDef = DotController.GetDotDef(damageReport.dotType);
					tickMult = dotDef.interval;
				}
			}
			if (Util.CheckRoll(BlastChance * tickMult, attackerBody.master))
			{
				itemCount = Math.Max(0, itemCount - 1);
				float blastDamage = BlastBaseDamage + (BlastStackDamage * itemCount);
				float blastRadius = BlastBaseRadius + (BlastStackRadius * itemCount);
				//blastDamage *= damageReport.damageDealt;
				blastDamage /= tickMult;
				blastDamage = Util.OnHitProcDamage(damageReport.damageInfo.damage, attackerBody.damage, blastDamage);
				if (BlastAddBodyRadius)
				{
					blastRadius += victimBody.radius;
				}
				if (blastDamage > 0f)
				{
					bool isCrit = damageReport.damageInfo.crit;
					if (BlastRollCrit && !isCrit)
                    {
						isCrit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
					}
					GameObject blastVFX = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), blastLocation, Quaternion.identity);
					blastVFX.transform.localScale = new Vector3(blastRadius, blastRadius, blastRadius);
					DelayBlast delayBlast = blastVFX.GetComponent<DelayBlast>();
					if (delayBlast)
					{
						delayBlast.position = blastLocation;
						delayBlast.radius = blastRadius;
						delayBlast.baseDamage = blastDamage;
						delayBlast.baseForce = 0f;
						delayBlast.procCoefficient = 0f;
						delayBlast.crit = isCrit;
						delayBlast.damageColorIndex = damageReport.damageInfo.damageColorIndex;
						delayBlast.falloffModel = BlastAttack.FalloffModel.SweetSpot;
						delayBlast.maxTimer = 0f;
						delayBlast.attacker = attackerBody.gameObject;
						delayBlast.inflictor = null;
						delayBlast.teamFilter.teamIndex = attackerBody.teamComponent.teamIndex;
						delayBlast.explosionEffect = GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab;
						if (BlastInheritDamageType)
                        {
							delayBlast.damageType = damageReport.damageInfo.damageType;
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
		}
	}
}
