using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff
{
	//This is also ripped off from RiskyMod.
	public class SharedHooks
	{
		public delegate void Handle_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory);
		public static Handle_GetStatCoefficients Handle_GetStatCoefficients_Actions;

		public delegate void Handle_GlobalKillEvent(DamageReport damageReport);
		public static Handle_GlobalKillEvent Handle_GlobalKillEvent_Actions;

		public delegate void Handle_GlobalDamageEvent(DamageReport damageReport);
		public static Handle_GlobalDamageEvent Handle_GlobalDamageEvent_Actions;

		public delegate void Handle_GlobalHitEvent(CharacterBody victim, CharacterBody attacker, DamageInfo damageInfo);
		public static Handle_GlobalHitEvent Handle_GlobalHitEvent_Actions;

		public delegate void Handle_CharacterMaster_OnBodyDeath(CharacterMaster master, CharacterBody body);
		public static Handle_CharacterMaster_OnBodyDeath Handle_CharacterMaster_OnBodyDeath_Actions;

		public delegate void Handle_GlobalInventoryChangedEvent(CharacterBody self);
		public static Handle_GlobalInventoryChangedEvent Handle_GlobalInventoryChangedEvent_Actions;
		public static void Setup()
        {
			if (Handle_GetStatCoefficients_Actions != null)
			{
				RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			}
			if (Handle_GlobalKillEvent_Actions != null)
			{
				GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			}
			if (Handle_GlobalDamageEvent_Actions != null)
			{
				GlobalEventManager.onServerDamageDealt += GlobalEventManager_DamageDealt;
			}
			if (Handle_GlobalHitEvent_Actions != null)
			{
				On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_HitEnemy;
			}
			if (Handle_GlobalInventoryChangedEvent_Actions != null)
            {
				CharacterBody.onBodyInventoryChangedGlobal += GlobalEventManager_OnInventoryChanged;
			}
			if (Handle_CharacterMaster_OnBodyDeath_Actions != null)
			{
				On.RoR2.CharacterMaster.OnBodyDeath += CharacterMaster_OnBodyDeath;
			}
		}
		
		internal static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				Handle_GetStatCoefficients_Actions.Invoke(sender, args, sender.inventory);
			}
		}

		internal static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageReport.attacker && damageReport.attackerBody)
            {
				Handle_GlobalKillEvent_Actions.Invoke(damageReport);
			}
		}

		internal static void GlobalEventManager_DamageDealt(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageReport.attacker && damageReport.attackerBody)
			{
				Handle_GlobalDamageEvent_Actions.Invoke(damageReport);
			}
		}

		internal static void GlobalEventManager_HitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
		{
			orig(self, damageInfo, victim);
			if (!NetworkServer.active)
			{
				return;
			}
			if (victim && damageInfo.attacker)
			{
				CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody victimBody = victim.GetComponent<CharacterBody>();
				if (attackerBody && victimBody)
                {
					Handle_GlobalHitEvent_Actions.Invoke(victimBody, attackerBody, damageInfo);
				}
			}
		}

		internal static void CharacterMaster_OnBodyDeath(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body)
		{
			orig(self, body);
			if (!NetworkServer.active)
			{
				return;
			}
			Handle_CharacterMaster_OnBodyDeath_Actions.Invoke(self, body);
		}

		internal static void GlobalEventManager_OnInventoryChanged(CharacterBody self)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Handle_GlobalInventoryChangedEvent_Actions.Invoke(self);
		}
	}
}
