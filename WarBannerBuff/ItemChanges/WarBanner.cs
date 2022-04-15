using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using WarBannerBuff.Components;

namespace WarBannerBuff.ItemChanges
{
    public class WarBanner
    {
        private static float NextRecover = 1f;
        private static BuffDef ModdedBuff = RoR2Content.Buffs.Warbanner;
        public static void Begin()
        {
			CreateBuff();
			UpdateText();
			Hooks();
        }
		private static void CreateBuff()
        {
			ModdedBuff = Modules.Buffs.AddNewBuff("WarBanner(Modded)", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/WardOnLevel/bdWarbanner.asset").WaitForCompletion().iconSprite, Color.yellow, false, false, false);
			LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard").GetComponent<BuffWard>().buffDef = ModdedBuff;
			On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
		}
		private static void Hooks()
        {
			if (MainPlugin.RecoveryTick.Value > 0f)
			{
				On.RoR2.Run.Start += Run_Start;
				On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
				On.RoR2.HealthComponent.FixedUpdate += HealthComponent_FixedUpdate;
			}
			RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsHook;
			if (MainPlugin.BossBanner.Value > 0f)
			{
				On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
				On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Phase2_OnEnter;
				On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Phase3_OnEnter;
			}
			if (MainPlugin.PillarBanner.Value > 0f)
            {
				On.EntityStates.Missions.Moon.MoonBatteryActive.OnEnter += MoonBatteryActive_OnEnter;
			}
			if (MainPlugin.VoidBanner.Value > 0f)
			{
				On.EntityStates.Missions.Arena.NullWard.Active.OnEnter += NullWardActive_OnEnter;
			}
			if (MainPlugin.DeepVoidBanner.Value > 0f)
			{
				On.EntityStates.DeepVoidPortalBattery.BaseDeepVoidPortalBatteryState.OnEnter += DeepVoidPortal_OnEnter;
			}
			if(MainPlugin.FocusBanner.Value > 0f)
            {
				On.RoR2.InfiniteTowerRun.OnSafeWardActivated += InfiniteTowerRun_OnSafeWardActivated;
			}
		}
		private static void UpdateText()
		{
			string pickup = "Drop a Warbanner on level up or starting the Teleporter event. Grants allies";
			string desc = "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>16m</style> <style=cStack>(+8m per stack)</style>. Grants";
			List<string> pickuplist = new List<string>();
			List<string> desclist = new List<string>();
			if (MainPlugin.RegenBonus.Value > 0.0f)
			{
				pickuplist.Add(" regen");
				desclist.Add(" <style=cIsHealing>regen</style>");
			}
			if (MainPlugin.DamageBonus.Value > 0.0f)
			{
				pickuplist.Add(" damage");
				desclist.Add(" <style=cIsDamage>damage</style>");
			}
			if (MainPlugin.CritBonus.Value > 0.0f)
			{
				pickuplist.Add(" crit chance");
				desclist.Add(" <style=cIsDamage>crit chance</style>");
			}
			if (MainPlugin.AttackBonus.Value > 0.0f)
			{
				pickuplist.Add(" attack speed");
				desclist.Add(" <style=cIsDamage>attack speed</style>");
			}
			if (MainPlugin.MoveBonus.Value > 0.0f)
			{
				pickuplist.Add(" movement speed");
				desclist.Add(" <style=cIsUtility>movement speed</style>");
			}
			if (MainPlugin.ArmorBonus.Value > 0.0f)
			{
				pickuplist.Add(" armor");
				desclist.Add(" <style=cIsHealing>armor</style>");
			}
			for (int i = 0; i < pickuplist.Count; i++)
			{
				if (i == pickuplist.Count - 1)
				{
					pickup += " and";
					desc += " and";
				}
				else if (i > 0)
				{
					pickup += ",";
					desc += ",";
				}
				pickup += pickuplist[i];
				desc += desclist[i];
			}
			pickup += ".";
			desc += ".";
			LanguageAPI.Add("ITEM_WARDONLEVEL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_WARDONLEVEL_DESC", desc);
		}
		private static void RecalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.HasBuff(ModdedBuff))
			{
				float levelBonus = sender.level - 1f;
				args.critAdd += MainPlugin.CritBonus.Value;
				args.armorAdd += MainPlugin.ArmorBonus.Value;
				args.baseDamageAdd += MainPlugin.DamageBonus.Value * (1 + (levelBonus * 0.2f));
				if(MainPlugin.UseBaseAttackSpeed.Value)
                {
					args.baseAttackSpeedAdd += MainPlugin.AttackBonus.Value;
				}
				else
                {
					args.attackSpeedMultAdd += MainPlugin.AttackBonus.Value;
				}
				args.moveSpeedMultAdd += MainPlugin.MoveBonus.Value;
				args.baseRegenAdd += MainPlugin.RegenBonus.Value * (1 + (levelBonus * 0.2f));
			}
		}
		private static void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
		{
			orig(self);
			NextRecover = 1f;
		}
		private static void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, RoR2.Run self)
		{
			orig(self);
			if (MainPlugin.RecoveryTick.Value > 0.0f)
			{
				if (NextRecover <= 0f)
				{
					NextRecover += MainPlugin.RecoveryTick.Value;
				}
				NextRecover -= Time.fixedDeltaTime;
			}
		}
		private static void HealthComponent_FixedUpdate(On.RoR2.HealthComponent.orig_FixedUpdate orig, HealthComponent self)
		{
			orig(self);
			if (self.body)
			{
				if (self.body.HasBuff(ModdedBuff))
				{
					if (NextRecover <= 0.0f)
					{
						float healing = CalculateHeal(self);
						if (healing > 0f)
						{
							self.Heal(healing, default, true);
						}
						healing = CalculateRecharge(self);
						if (healing > 0f)
						{
							self.RechargeShield(healing);
						}
					}
				}
			}
		}
		private static float CalculateHeal(HealthComponent self)
		{
			float healing = 0.0f;
			if (self.body)
			{
				healing = self.fullHealth * MainPlugin.HealBase.Value;
				float levelBonus = self.body.level - 1.0f;
				healing += MainPlugin.HealLevel.Value * levelBonus;
				if (healing < MainPlugin.HealMin.Value)
				{
					healing = MainPlugin.HealMin.Value;
				}
			}
			return healing;
		}
		private static float CalculateRecharge(HealthComponent self)
		{
			float healing = 0.0f;
			if (self.body)
			{
				healing = self.fullShield * MainPlugin.RechargeBase.Value;
				float levelBonus = self.body.level - 1.0f;
				healing += MainPlugin.RechargeLevel.Value * levelBonus;
				if (healing < MainPlugin.RechargeMin.Value)
				{
					healing = MainPlugin.RechargeMin.Value;
				}
			}
			return healing;
		}
		private static void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.BossBanner.Value);
		}
		private static void Phase2_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.BossBanner.Value);
		}
		private static void Phase3_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.BossBanner.Value);
		}
		private static void MoonBatteryActive_OnEnter(On.EntityStates.Missions.Moon.MoonBatteryActive.orig_OnEnter orig, EntityStates.Missions.Moon.MoonBatteryActive self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.PillarBanner.Value);
		}
		private static void NullWardActive_OnEnter(On.EntityStates.Missions.Arena.NullWard.Active.orig_OnEnter orig, EntityStates.Missions.Arena.NullWard.Active self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.VoidBanner.Value);
		}
		private static void DeepVoidPortal_OnEnter(On.EntityStates.DeepVoidPortalBattery.BaseDeepVoidPortalBatteryState.orig_OnEnter orig, EntityStates.DeepVoidPortalBattery.BaseDeepVoidPortalBatteryState self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.DeepVoidBanner.Value);
		}
		private static void InfiniteTowerRun_OnSafeWardActivated(On.RoR2.InfiniteTowerRun.orig_OnSafeWardActivated orig, InfiniteTowerRun self, InfiniteTowerSafeWardController safeWard)
		{
			orig(self, safeWard);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.FocusBanner.Value);
		}
		private static void SpawnTeamWarBanners(TeamIndex team, float sizemult)
		{
			if (NetworkServer.active)
			{
				if (sizemult > 0f)
				{
					ReadOnlyCollection<TeamComponent> teamlist = TeamComponent.GetTeamMembers(team);
					for (int i = 0; i < teamlist.Count; i++)
					{
						CharacterBody body = teamlist[i].body;
						if (body)
						{
							HealthComponent healthComponent = body.healthComponent;
							if (healthComponent && healthComponent.alive)
							{
								if (body.inventory)
								{
									int itemCount = body.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
									if (itemCount > 0)
									{
										GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), teamlist[i].transform.position, Quaternion.identity);
										gameObject.GetComponent<TeamFilter>().teamIndex = team;
										gameObject.GetComponent<BuffWard>().Networkradius = (8f + 8f * (float)itemCount) * sizemult;
										NetworkServer.Spawn(gameObject);
									}
								}
							}
						}
					}
				}
			}
		}
		private static void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, CharacterBody self)
		{
			orig(self);
			if (self.HasBuff(ModdedBuff) && !self.HasBuff(RoR2Content.Buffs.Warbanner))
			{
				WarBannerBuffVFX fxcomponent = self.GetComponent<WarBannerBuffVFX>();
				if (fxcomponent == null)
				{
					fxcomponent = self.gameObject.AddComponent<WarBannerBuffVFX>();
				}
				if (fxcomponent.effect == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TemporaryVisualEffects/WarbannerBuffEffect"), self.corePosition, Quaternion.identity);
					fxcomponent.effect = gameObject.GetComponent<TemporaryVisualEffect>();
					fxcomponent.effect.parentTransform = self.coreTransform;
					fxcomponent.effect.visualState = TemporaryVisualEffect.VisualState.Enter;
					fxcomponent.effect.healthComponent = self.healthComponent;
					fxcomponent.effect.radius = self.radius;
					LocalCameraEffect component = gameObject.GetComponent<LocalCameraEffect>();
					if (component)
					{
						component.targetCharacter = self.gameObject;
					}
				}
			}
			else
			{
				WarBannerBuffVFX fxcomponent = self.GetComponent<WarBannerBuffVFX>();
				if (fxcomponent != null)
				{
					fxcomponent.effect.visualState = TemporaryVisualEffect.VisualState.Exit;
				}
			}
		}
	}
}
