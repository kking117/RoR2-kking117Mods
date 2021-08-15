using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace WarBannerBuff
{
	[BepInPlugin("com.kking117.WarBannerBuff", "WarBannerBuff", "3.0.0")]
	[BepInDependency("com.bepis.r2api")]
	[R2APISubmoduleDependency(new string[]
	{
		"RecalculateStatsAPI",
	})]

	public class warbannerbuff : BaseUnityPlugin
    {
		private float HealInterval = 1f;

		public static ConfigEntry<float> RegenTick;
		public static ConfigEntry<float> RegenMaxHealth;
		public static ConfigEntry<float> RegenLevelHealth;
		public static ConfigEntry<float> RegenMin;
		public static ConfigEntry<float> RechargeMaxShield;
		public static ConfigEntry<float> RechargeLevelShield;
		public static ConfigEntry<float> RechargeMin;

		public static ConfigEntry<float> DamageBonus;
		public static ConfigEntry<float> CritBonus;
		public static ConfigEntry<float> ArmorBonus;

		public static ConfigEntry<float> VoidBanner;
		public static ConfigEntry<float> PillarBanner;
		public static ConfigEntry<float> BossBanner;
		public void Awake()
		{
			ReadConfig();
			//hooks
			On.RoR2.Run.Start += Run_Start;
			On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
			On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
			RecalculateStatsAPI.GetStatCoefficients += CalcBannerStats;
			On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
			On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Phase2_OnEnter;
			On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Phase3_OnEnter;
			On.EntityStates.Missions.Moon.MoonBatteryActive.OnEnter += MoonBatteryActive_OnEnter;
			On.EntityStates.Missions.Arena.NullWard.Active.OnEnter += NullWardActive_OnEnter;
		}
		private void CalcBannerStats(RoR2.CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.HasBuff(RoR2Content.Buffs.Warbanner))
			{
				args.critAdd += CritBonus.Value;
				args.armorAdd += ArmorBonus.Value;
				args.baseDamageAdd += DamageBonus.Value;
			}
		}
		public void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
		{
			orig(self);
			HealInterval = 1f;
		}
		private void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, RoR2.Run self)
		{
			orig(self);
			if (RegenTick.Value > 0f)
			{
				if (HealInterval <= 0f)
				{
					HealInterval += RegenTick.Value;
				}
				HealInterval -= Time.fixedDeltaTime;
			}
		}
		private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
		{
			orig(self);
			if (RegenTick.Value > 0f && HealInterval <= 0f)
			{
				if (self.HasBuff(RoR2Content.Buffs.Warbanner))
				{
					HealthComponent healthComponent = self.healthComponent;
					if (healthComponent)
					{
						float healing = healthComponent.fullHealth * RegenMaxHealth.Value;
						healing += RegenLevelHealth.Value * self.level;
						if (healing < RegenMin.Value)
						{
							healing = RegenMin.Value;
						}
						if (healing > 0f)
						{
							healthComponent.Heal(healing, default, true);
						}

						healing = healthComponent.fullShield * RechargeMaxShield.Value;
						healing += RechargeLevelShield.Value * self.level;
						if (healing < RechargeMin.Value)
						{
							healing = RechargeMin.Value;
						}
						if (healing > 0f)
						{
							healthComponent.RechargeShield(healing);
						}
					}
				}
			}
		}
		private void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
		{
			orig(self);
			if (BossBanner.Value > 0f)
			{
				SpawnTeamWarBanners(TeamIndex.Player, BossBanner.Value);
			}
		}
		private void Phase2_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
		{
			orig(self);
			if (BossBanner.Value > 0f)
			{
				SpawnTeamWarBanners(TeamIndex.Player, BossBanner.Value);
			}
		}
		private void Phase3_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
		{
			orig(self);
			if (BossBanner.Value > 0f)
			{
				SpawnTeamWarBanners(TeamIndex.Player, BossBanner.Value);
			}
		}
		private void MoonBatteryActive_OnEnter(On.EntityStates.Missions.Moon.MoonBatteryActive.orig_OnEnter orig, EntityStates.Missions.Moon.MoonBatteryActive self)
		{
			orig(self);
			if (PillarBanner.Value > 0f)
			{
				SpawnTeamWarBanners(TeamIndex.Player, PillarBanner.Value);
			}
		}
		private void NullWardActive_OnEnter(On.EntityStates.Missions.Arena.NullWard.Active.orig_OnEnter orig, EntityStates.Missions.Arena.NullWard.Active self)
		{
			orig(self);
			if (VoidBanner.Value > 0f)
			{
				SpawnTeamWarBanners(TeamIndex.Player, VoidBanner.Value);
			}
		}
		void SpawnTeamWarBanners(TeamIndex team, float sizemult)
		{
			for (int i = 0; i < RoR2.PlayerCharacterMasterController.instances.Count; i++)
			{
				CharacterBody charbody = RoR2.PlayerCharacterMasterController.instances[i].master.GetBody();
				if (charbody)
				{
					TeamComponent teamComponent = charbody.teamComponent;
					if (teamComponent.teamIndex == team)
					{
						HealthComponent healthComponent = charbody.healthComponent;
						if (healthComponent && healthComponent.health > 0)
						{
							CharacterMaster master = charbody.master;
							if (master)
							{
								int itemCount = master.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
								if (itemCount > 0)
								{
									GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), teamComponent.transform.position, Quaternion.identity);
									gameObject.GetComponent<TeamFilter>().teamIndex = teamComponent.teamIndex;
									gameObject.GetComponent<BuffWard>().Networkradius = (8f + 8f * (float)itemCount) * sizemult;
									NetworkServer.Spawn(gameObject);
								}
							}
						}
					}
				}
			}
		}
		public void ReadConfig()
		{
			RegenTick = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Regen Interval"), 0.5f, new ConfigDescription("Delay between each regen/recharge effect. (0 or less disables this)", null, Array.Empty<object>()));
			RegenMaxHealth = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Regen Max Health"), 0.005f, new ConfigDescription("Regen this % amount of the target's health based on their total combined health per interval. (0.01f = 1%)", null, Array.Empty<object>()));
			RegenLevelHealth = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Regen Level Health"), 0.1f, new ConfigDescription("Regen this amount of health per the target's level per interval.", null, Array.Empty<object>()));
			RegenMin = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Minimum Regen"), 1.0f, new ConfigDescription("The minimum amount of healing the target can gain per interval.", null, Array.Empty<object>()));
			RechargeMaxShield = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Recharge Max Shield"), 0.0f, new ConfigDescription("Recharge this % amount of the target's shield based on their total combined health per interval. (0.01f = 1%)", null, Array.Empty<object>()));
			RechargeLevelShield = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Recharge Level Shield"), 0.0f, new ConfigDescription("Recharge this amount of shield per the target's level per interval.", null, Array.Empty<object>()));
			RechargeMin = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Minimum Recharge"), 0.0f, new ConfigDescription("The minimum amount of recharge the target can gain per interval.", null, Array.Empty<object>()));
			DamageBonus = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Damage Bonus"), 4.0f, new ConfigDescription("How much Damage to grant to the target.", null, Array.Empty<object>()));
			CritBonus = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Crit Bonus"), 10.0f, new ConfigDescription("How much Crit to grant to the target.", null, Array.Empty<object>()));
			ArmorBonus = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Armor Bonus"), 0.0f, new ConfigDescription("How much Armor to grant to the target.", null, Array.Empty<object>()));
			BossBanner = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Mithrix Phase Banners"), 1.0f, new ConfigDescription("Players equipped with war banners will place one down at the start of Mithrix's phases. (Except the item steal phase.) (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			PillarBanner = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Moon Pillar Banners"), 0.25f, new ConfigDescription("Players equipped with war banners will place one down at the start of a Moon Pillar event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			VoidBanner = Config.Bind<float>(new ConfigDefinition("WarBannerBuff", "Void Cell Banners"), 0.25f, new ConfigDescription("Players equipped with war banners will place one down at the start of a Void Cell event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
		}
	}
}
