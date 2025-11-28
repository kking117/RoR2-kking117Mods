using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public static float OverlapDistanceMult;
		public static BuffDef ModdedBuff = RoR2Content.Buffs.Warbanner;
		public static GameObject BannerWard = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WardOnLevel/WarbannerWard.prefab").WaitForCompletion();
		public static GameObject BuffVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WardOnLevel/WarbannerBuffEffect.prefab").WaitForCompletion();
		private static string BannerWard_RefName = "(Clone)";
		private static float NextRecover = 1f;
		public static BuffDef GreaterBannerBuff;
		public WarBanner()
		{
			ClampConfig();
			CreateBuff();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			MainPlugin.Merge_FuseMult = Math.Max(0f, MainPlugin.Merge_MinOverlap);
			MainPlugin.Merge_FuseMult = Math.Min(1f, MainPlugin.Merge_MinOverlap);
			OverlapDistanceMult = 1f - MainPlugin.Merge_FuseMult;
			MainPlugin.Merge_FuseMult = Math.Min(1f, MainPlugin.Merge_MinOverlap);

			MainPlugin.DamageBonus = Math.Max(0f, MainPlugin.DamageBonus);
			MainPlugin.AttackBonus = Math.Max(0f, MainPlugin.AttackBonus);
			MainPlugin.CritBonus = Math.Max(0f, MainPlugin.CritBonus);
			MainPlugin.ArmorBonus = Math.Max(0f, MainPlugin.ArmorBonus);
			MainPlugin.RegenBonus = Math.Max(0f, MainPlugin.RegenBonus);
			MainPlugin.MoveBonus = Math.Max(0f, MainPlugin.MoveBonus);
		}
		private void CreateBuff()
		{
			ModdedBuff = Modules.Buffs.AddNewBuff("WarBanner(Modded)", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/WardOnLevel/bdWarbanner.asset").WaitForCompletion().iconSprite, Color.yellow, false, false, false);
			BannerWard.GetComponent<BuffWard>().buffDef = ModdedBuff;
			On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
			BannerWard_RefName = BannerWard.name + BannerWard_RefName;
			GreaterBannerBuff = ModdedBuff;
		}
		private void Hooks()
		{
			if (MainPlugin.RecoveryTick > 0f)
			{
				On.RoR2.Run.Start += Run_Start;
				On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
				On.RoR2.HealthComponent.ServerFixedUpdate += HealthComponent_FixedUpdate;
			}
			RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsHook;
			if (MainPlugin.BossBanner > 0f)
			{
				On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
				On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Phase2_OnEnter;
				On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Phase3_OnEnter;
			}
			if (MainPlugin.PillarBanner > 0f)
			{
				On.EntityStates.Missions.Moon.MoonBatteryActive.OnEnter += MoonBatteryActive_OnEnter;
			}
			if (MainPlugin.VoidBanner > 0f)
			{
				On.EntityStates.Missions.Arena.NullWard.Active.OnEnter += NullWardActive_OnEnter;
			}
			if (MainPlugin.DeepVoidBanner > 0f)
			{
				On.EntityStates.DeepVoidPortalBattery.BaseDeepVoidPortalBatteryState.OnEnter += DeepVoidPortal_OnEnter;
			}
			if (MainPlugin.FocusBanner > 0f)
			{
				On.RoR2.InfiniteTowerRun.OnSafeWardActivated += InfiniteTowerRun_OnSafeWardActivated;
			}
			if (MainPlugin.HalcyonBanner > 0f)
            {
				On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.OnEnter += Halcyon_OnEnter;
			}
			if (MainPlugin.MeridianBanner > 0f)
            {
				On.EntityStates.MeridianEvent.Phase1.OnEnter += Meridian_Phase1;
				On.EntityStates.MeridianEvent.Phase2.OnEnter += Meridian_Phase2;
				On.EntityStates.MeridianEvent.Phase3.OnEnter += Meridian_Phase3;
			}
			if (MainPlugin.PrisonBanner > 0f)
            {
				On.EntityStates.SolusWing2.Mission2Fight.OnEnter += SolusWing_StartPhase;
				On.EntityStates.SolusWing2.Mission3Darkness.OnEnter += SolusWing_BeamPhase;
				On.EntityStates.SolusWing2.Mission4Desperation.OnEnter += SolusWing_DesperationPhase;
			}
			if (MainPlugin.SolusHeartBanner > 0f)
			{
				On.EntityStates.SolusHeart.Phase0.Mission0.OnEnter += SolusHeart_Phase0;
				On.EntityStates.SolusHeart.Phase1.Mission1.OnEnter += SolusHeart_Phase1;
				On.EntityStates.SolusHeart.Phase2.Mission2.OnEnter += SolusHeart_Phase2;
			}
			if (MainPlugin.Merge_Enable)
			{
				On.RoR2.BuffWard.Start += BuffWard_Start;
			}
		}
		private static void UpdateText()
		{
			string pickup = "Drop a Warbanner on level up or starting the Teleporter event. Grants allies";
			string desc = "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>16m</style> <style=cStack>(+8m per stack)</style>.\n\nThe banner grants:";
			List<string> pickuplist = new List<string>();
			List<string> desclist = new List<string>();
			if (MainPlugin.DamageBonus > 0.0f)
			{
				pickuplist.Add(" damage");
				desclist.Add(string.Format(" <style=cIsDamage>+{0}% damage</style>", MainPlugin.DamageBonus * 100f));
			}
			if (MainPlugin.AttackBonus > 0.0f)
			{
				pickuplist.Add(" attack speed");
				desclist.Add(string.Format(" <style=cIsDamage>+{0}% attack speed</style>", MainPlugin.AttackBonus * 100f));
			}
			if (MainPlugin.CritBonus > 0.0f)
			{
				pickuplist.Add(" crit chance");
				desclist.Add(string.Format(" <style=cIsDamage>+{0}% crit chance</style>", MainPlugin.CritBonus));
			}
			if (MainPlugin.ArmorBonus > 0.0f)
			{
				pickuplist.Add(" armor");
				desclist.Add(string.Format(" <style=cIsHealing>+{0} armor</style>", MainPlugin.ArmorBonus));
			}
			if (MainPlugin.RegenBonus > 0.0f)
			{
				pickuplist.Add(" regen");
				desclist.Add(string.Format(" <style=cIsHealing>+{0} base health regen</style>", MainPlugin.RegenBonus));
			}
			if (MainPlugin.MoveBonus > 0.0f)
			{
				pickuplist.Add(" movement speed");
				desclist.Add(string.Format(" <style=cIsUtility>+{0}% movement speed</style>", MainPlugin.MoveBonus * 100f));
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
		private void RecalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (HasBannerBuff(sender))
			{
				args.damageMultAdd += MainPlugin.DamageBonus;
				args.attackSpeedMultAdd += MainPlugin.AttackBonus;
				args.moveSpeedMultAdd += MainPlugin.MoveBonus;
				args.critAdd += MainPlugin.CritBonus;
				args.armorAdd += MainPlugin.ArmorBonus;
				if (MainPlugin.RegenBonus > 0f)
				{
					args.baseRegenAdd += MainPlugin.RegenBonus;
					args.levelRegenAdd += MainPlugin.RegenBonus / 5f;
				}
			}
		}
		private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
		{
			orig(self);
			NextRecover = 1f;
		}
		private void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, RoR2.Run self)
		{
			orig(self);
			if (MainPlugin.RecoveryTick > 0.0f)
			{
				if (NextRecover <= 0f)
				{
					NextRecover += MainPlugin.RecoveryTick;
				}
				NextRecover -= Time.fixedDeltaTime;
			}
		}
		private void HealthComponent_FixedUpdate(On.RoR2.HealthComponent.orig_ServerFixedUpdate orig, HealthComponent self, float deltaTime)
		{
			orig(self, deltaTime);
			if (self.body)
			{
				if (HasBannerBuff(self.body))
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
				healing = self.fullHealth * MainPlugin.HealBase;
				float levelBonus = self.body.level - 1.0f;
				healing += MainPlugin.HealLevel * levelBonus;
				if (healing < MainPlugin.HealMin)
				{
					healing = MainPlugin.HealMin;
				}
			}
			return healing;
		}
		private static float CalculateRecharge(HealthComponent self)
		{
			float healing = 0.0f;
			if (self.body)
			{
				healing = self.fullShield * MainPlugin.RechargeBase;
				float levelBonus = self.body.level - 1.0f;
				healing += MainPlugin.RechargeLevel * levelBonus;
				if (healing < MainPlugin.RechargeMin)
				{
					healing = MainPlugin.RechargeMin;
				}
			}
			return healing;
		}

		private static void Halcyon_OnEnter(On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.orig_OnEnter orig, EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.HalcyonBanner);
		}
		private static void SolusHeart_Phase0(On.EntityStates.SolusHeart.Phase0.Mission0.orig_OnEnter orig, EntityStates.SolusHeart.Phase0.Mission0 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.SolusHeartBanner);
			}
		}
		private static void SolusHeart_Phase1(On.EntityStates.SolusHeart.Phase1.Mission1.orig_OnEnter orig, EntityStates.SolusHeart.Phase1.Mission1 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.SolusHeartBanner);
			}
		}
		private static void SolusHeart_Phase2(On.EntityStates.SolusHeart.Phase2.Mission2.orig_OnEnter orig, EntityStates.SolusHeart.Phase2.Mission2 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.SolusHeartBanner);
			}
		}
		private static void SolusWing_StartPhase(On.EntityStates.SolusWing2.Mission2Fight.orig_OnEnter orig, EntityStates.SolusWing2.Mission2Fight self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.PrisonBanner);
			}
		}
		private static void SolusWing_BeamPhase(On.EntityStates.SolusWing2.Mission3Darkness.orig_OnEnter orig, EntityStates.SolusWing2.Mission3Darkness self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.PrisonBanner);
			}
		}
		private static void SolusWing_DesperationPhase(On.EntityStates.SolusWing2.Mission4Desperation.orig_OnEnter orig, EntityStates.SolusWing2.Mission4Desperation self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.PrisonBanner);
			}
		}
		private static void Meridian_Phase1(On.EntityStates.MeridianEvent.Phase1.orig_OnEnter orig, EntityStates.MeridianEvent.Phase1 self)
		{
			orig(self);
			if (NetworkServer.active)
            {
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.MeridianBanner);
			}
		}
		private static void Meridian_Phase2(On.EntityStates.MeridianEvent.Phase2.orig_OnEnter orig, EntityStates.MeridianEvent.Phase2 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.MeridianBanner);
			}
		}
		private static void Meridian_Phase3(On.EntityStates.MeridianEvent.Phase3.orig_OnEnter orig, EntityStates.MeridianEvent.Phase3 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.MeridianBanner);
			}
		}
		private static void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.BossBanner);
		}
		private static void Phase2_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.BossBanner);
		}
		private static void Phase3_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.BossBanner);
		}
		private static void MoonBatteryActive_OnEnter(On.EntityStates.Missions.Moon.MoonBatteryActive.orig_OnEnter orig, EntityStates.Missions.Moon.MoonBatteryActive self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.PillarBanner);
		}
		private static void NullWardActive_OnEnter(On.EntityStates.Missions.Arena.NullWard.Active.orig_OnEnter orig, EntityStates.Missions.Arena.NullWard.Active self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.VoidBanner);
		}
		private static void DeepVoidPortal_OnEnter(On.EntityStates.DeepVoidPortalBattery.BaseDeepVoidPortalBatteryState.orig_OnEnter orig, EntityStates.DeepVoidPortalBattery.BaseDeepVoidPortalBatteryState self)
		{
			orig(self);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.DeepVoidBanner);
		}
		private static void InfiniteTowerRun_OnSafeWardActivated(On.RoR2.InfiniteTowerRun.orig_OnSafeWardActivated orig, InfiniteTowerRun self, InfiniteTowerSafeWardController safeWard)
		{
			orig(self, safeWard);
			SpawnTeamWarBanners(TeamIndex.Player, MainPlugin.FocusBanner);
		}
		private static void BuffWard_Start(On.RoR2.BuffWard.orig_Start orig, BuffWard self)
		{
			orig(self);
			TryMergeWithWard(self);
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
									int itemCount = body.inventory.GetItemCountEffective(RoR2Content.Items.WardOnLevel);
									if (itemCount > 0)
									{
										GameObject gameObject = UnityEngine.Object.Instantiate(BannerWard, teamlist[i].transform.position, Quaternion.identity);
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
		private static void TryMergeWithWard(BuffWard thisbuffWard)
		{
			if (NetworkServer.active)
			{
				if (thisbuffWard.gameObject.name == BannerWard_RefName && !thisbuffWard.expires)
				{
					TeamIndex teamIndex = thisbuffWard.teamFilter.teamIndex;
					List<BuffWard> validWardList = new List<BuffWard>();
					BuffWard largestWard = thisbuffWard;
					foreach (BuffWard buffWard in UnityEngine.Object.FindObjectsOfType<BuffWard>())
					{
						if (buffWard != thisbuffWard && !buffWard.expires)
						{
							if (buffWard.name == thisbuffWard.name)
							{
								if (buffWard.invertTeamFilter == thisbuffWard.invertTeamFilter)
								{
									if (buffWard.teamFilter.teamIndex == teamIndex)
									{
										if (IsWardInRange(buffWard, thisbuffWard))
										{
											validWardList.Add(buffWard);
											if (buffWard.radius > largestWard.radius)
											{
												largestWard = buffWard;
											}
										}
									}
								}
							}
						}
					}
					if (validWardList.Count > 0)
					{
						float baseRadius = largestWard.radius;
						float addRadius = 0f;
						if (thisbuffWard != largestWard)
						{
							addRadius += thisbuffWard.radius;
						}
						foreach (BuffWard buffWard in validWardList)
						{
							if (buffWard != largestWard)
							{
								addRadius += buffWard.radius;
							}
							buffWard.expires = true;
							UnityEngine.Object.Destroy(buffWard.gameObject);
						}
						thisbuffWard.radius = baseRadius + addRadius * MainPlugin.Merge_FuseMult;
						TryMergeWithWard(thisbuffWard);
					}
				}
			}
		}
		private static bool IsWardInRange(BuffWard target, BuffWard searcher)
        {
			float searcherRadius = searcher.radius * OverlapDistanceMult;
			float targetRadius = target.radius * OverlapDistanceMult;
			float distance = Vector3.Distance(target.gameObject.transform.position, searcher.gameObject.transform.position);
			if (distance - targetRadius <= searcherRadius)
            {
				return true;
            }
			if (distance - searcherRadius <= targetRadius)
			{
				return true;
			}
			return false;
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
					GameObject gameObject = UnityEngine.Object.Instantiate(BuffVFX, self.corePosition, Quaternion.identity);
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

		private bool HasBannerBuff(CharacterBody body)
        {
			if (body.HasBuff(ModdedBuff) || body.HasBuff(GreaterBannerBuff))
            {
				return true;
            }
			return false;
        }
	}
}
