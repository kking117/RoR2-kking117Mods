using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace WarBannerBuff
{
	[BepInPlugin("com.kking117.WarBannerBuff", "WarBannerBuff", "4.1.0")]
	[BepInDependency("com.bepis.r2api")]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI",
		"BuffAPI",
	})]

	public class warbannerbuff : BaseUnityPlugin
    {
		private float HealInterval = 1f;
		private float RechargeInterval = 1f;
		private CustomBuff WarBannerBuffBuff;
		private BuffDef ModdedBuff = RoR2Content.Buffs.Warbanner;

		public static ConfigEntry<bool> CreateNewBuff;

		public static ConfigEntry<float> RegenTick;
		public static ConfigEntry<float> RegenMaxHealth;
		public static ConfigEntry<float> RegenLevelHealth;
		public static ConfigEntry<float> RegenMin;
		public static ConfigEntry<bool> RegenIsRegen;

		public static ConfigEntry<float> RechargeTick;
		public static ConfigEntry<float> RechargeMaxShield;
		public static ConfigEntry<float> RechargeLevelShield;
		public static ConfigEntry<float> RechargeMin;

		public static ConfigEntry<float> DamageBonus;
		public static ConfigEntry<float> CritBonus;
		public static ConfigEntry<float> ArmorBonus;
		public static ConfigEntry<float> AttackBonus;
		public static ConfigEntry<float> MoveBonus;

		public static ConfigEntry<float> VoidBanner;
		public static ConfigEntry<float> PillarBanner;
		public static ConfigEntry<float> BossBanner;
		public void Awake()
		{
			ReadConfig();
			if (CreateNewBuff.Value)
			{
				SetupNewBannerBuff();
				On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
			}
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
			Update_ItemDescs();
		}
		private void Update_ItemDescs()
		{
			string pickup = "Drop a Warbanner on level up or starting the Teleporter event. Grants allies";
			string desc = "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>16m</style> <style=cStack>(+8m per stack)</style>. Grants";
			List<string> pickuplist = new List<string>();
			List<string> desclist = new List<string>();
			if (RegenTick.Value > 0.0f)
            {
				if (RegenIsRegen.Value)
                {
					pickuplist.Add(" healing ");
					desclist.Add(" <style=cIsHealing>healing</style>");
				}
				else
                {
					pickuplist.Add(" regen");
					desclist.Add(" <style=cIsHealing>regen</style>");
				}
			}
			if (DamageBonus.Value > 0.0f)
			{
				pickuplist.Add(" damage");
				desclist.Add(" <style=cIsDamage>damage</style>");
			}
			if (CritBonus.Value > 0.0f)
			{
				pickuplist.Add(" crit chance");
				desclist.Add(" <style=cIsDamage>crit chance</style>");
			}
			if (AttackBonus.Value > 0.0f)
            {
				pickuplist.Add(" attack speed");
				desclist.Add(" <style=cIsDamage>attack speed</style>");
			}
			if (MoveBonus.Value > 0.0f)
			{
				pickuplist.Add(" movement speed");
				desclist.Add(" <style=cIsUtility>movement speed</style>");
			}
			if (ArmorBonus.Value > 0.0f)
			{
				pickuplist.Add(" armor");
				desclist.Add(" <style=cIsUtility>armor</style>");
			}
			for(int i = 0; i < pickuplist.Count; i++)
            {
				if (i == pickuplist.Count - 1)
                {
					pickup += " and";
					desc += " and";
				}
				else if ( i > 0)
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
		private void SetupNewBannerBuff()
        {
			BuffDef OldBuff = RoR2Content.Buffs.Warbanner;

			WarBannerBuffBuff = new CustomBuff(OldBuff.name + "(Buffed)", OldBuff.iconSprite, OldBuff.buffColor, OldBuff.isDebuff, false);
			BuffAPI.Add(WarBannerBuffBuff);

			ModdedBuff = WarBannerBuffBuff.BuffDef;

			Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard").GetComponent<BuffWard>().buffDef = WarBannerBuffBuff.BuffDef;
		}
		private void CalcBannerStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.HasBuff(ModdedBuff))
			{
				args.critAdd += CritBonus.Value;
				args.armorAdd += ArmorBonus.Value;
				args.baseDamageAdd += DamageBonus.Value;
				if (CreateNewBuff.Value)
				{
					args.attackSpeedMultAdd += AttackBonus.Value;
					args.moveSpeedMultAdd += MoveBonus.Value;
				}
				else
                {
					args.attackSpeedMultAdd += AttackBonus.Value - 0.3f;
					args.moveSpeedMultAdd += MoveBonus.Value - 0.3f;
				}
					
				if (RegenIsRegen.Value && RegenTick.Value > 0.0f)
                {
					float regen = CalculateCharacterRegen(sender);
					if (regen > 0.0f)
					{
						args.baseRegenAdd += regen * (1.0f / RegenTick.Value);
					}
                }
			}
		}
		public void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
		{
			orig(self);
			HealInterval = 1f;
			RechargeInterval = 1f;
		}
		private void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, RoR2.Run self)
		{
			orig(self);
			if (RegenTick.Value > 0.0f)
			{
				if (HealInterval <= 0f)
				{
					HealInterval += RegenTick.Value;
				}
				HealInterval -= Time.fixedDeltaTime;
			}
			else
			{
				HealInterval = 1f;
			}
			if (RechargeTick.Value > 0.0f)
			{
				if (RechargeInterval <= 0f)
				{
					RechargeInterval += RechargeTick.Value;
				}
				RechargeInterval -= Time.fixedDeltaTime;
			}
			else
            {
				RechargeInterval = 1f;
			}
		}
		private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
		{
			orig(self);
			HealthComponent healthComponent = self.healthComponent;
			if (healthComponent)
            {
				if (self.HasBuff(ModdedBuff))
                {
					if (RegenIsRegen.Value == false && HealInterval <= 0f)
                    {
						float healing = CalculateCharacterRegen(self);
						if (healing > 0f)
						{
							healthComponent.Heal(healing, default, true);
						}
					}
					if (RechargeInterval <= 0f)
					{
						float healing = CalculateCharacterRecharge(self);
						if (healing > 0f)
						{
							healthComponent.RechargeShield(healing);
						}
					}
				}
			}
		}

		private void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, RoR2.CharacterBody self)
        {
			orig(self);
			if (self.HasBuff(WarBannerBuffBuff.BuffDef) && !self.HasBuff(RoR2Content.Buffs.Warbanner))
            {
				NewBuffFX fxcomponent = self.GetComponent<NewBuffFX>();
				if (fxcomponent == null)
				{
					fxcomponent = self.gameObject.AddComponent<NewBuffFX>();
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
						component.targetCharacter = base.gameObject;
					}
				}
			}
            else
            {
				NewBuffFX fxcomponent = self.GetComponent<NewBuffFX>();
				if (fxcomponent != null)
				{
					fxcomponent.effect.visualState = TemporaryVisualEffect.VisualState.Exit;
				}
			}
		}

		float CalculateCharacterRegen(CharacterBody self)
        {
			float healing = 0.0f;
			HealthComponent healthComponent = self.healthComponent;
			if (healthComponent)
			{
				healing = healthComponent.fullHealth * RegenMaxHealth.Value;
				healing += RegenLevelHealth.Value * self.level;
				if (healing < RegenMin.Value)
				{
					healing = RegenMin.Value;
				}
			}
			return healing;
		}

		float CalculateCharacterRecharge(CharacterBody self)
		{
			float healing = 0.0f;
			HealthComponent healthComponent = self.healthComponent;
			if (healthComponent)
			{
				healing = healthComponent.fullShield * RechargeMaxShield.Value;
				healing += RechargeLevelShield.Value * self.level;
				if (healing < RechargeMin.Value)
				{
					healing = RechargeMin.Value;
				}
			}
			return healing;
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
			if (NetworkServer.active)
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
									GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), body.transform.position, Quaternion.identity);
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
		public void ReadConfig()
		{
			CreateNewBuff = Config.Bind<bool>(new ConfigDefinition("Buffs", "Create New Buff"), true, new ConfigDescription("Moves all changes to an entirely new buff. (Suggest enabling if you have mods such as Beetle Queen Plus or EliteVariety to keep their balance.)", null, Array.Empty<object>()));
			RegenTick = Config.Bind<float>(new ConfigDefinition("Healing Stats", "Heal Interval"), 0.5f, new ConfigDescription("Delay in seconds between each regen tick. (0 or less disables healing)", null, Array.Empty<object>()));
			RegenMaxHealth = Config.Bind<float>(new ConfigDefinition("Healing Stats", "Heal Max Health"), 0.005f, new ConfigDescription("Regen this % amount of the target's health based on their total combined health per interval. (0.01 = 1%)", null, Array.Empty<object>()));
			RegenLevelHealth = Config.Bind<float>(new ConfigDefinition("Healing Stats", "Heal Level Health"), 0.1f, new ConfigDescription("Regen this amount of health per the target's level per interval.", null, Array.Empty<object>()));
			RegenMin = Config.Bind<float>(new ConfigDefinition("Healing Stats", "Minimum Heal"), 1.0f, new ConfigDescription("The minimum amount of healing the target can gain per interval.", null, Array.Empty<object>()));
			RegenIsRegen = Config.Bind<bool>(new ConfigDefinition("Healing Stats", "Heal is Regen"), true, new ConfigDescription("The above healing configurations are applied to the regen stat instead of as healing.", null, Array.Empty<object>()));
			RechargeTick = Config.Bind<float>(new ConfigDefinition("Recharge Stats", "Recharge Interval"), 0.0f, new ConfigDescription("Delay in seconds between each recharge tick. (0 or less disables recharging)", null, Array.Empty<object>()));
			RechargeMaxShield = Config.Bind<float>(new ConfigDefinition("Recharge Stats", "Recharge Max Shield"), 0.0f, new ConfigDescription("Recharge this % amount of the target's shield based on their total combined health per interval. (0.01 = 1%)", null, Array.Empty<object>()));
			RechargeLevelShield = Config.Bind<float>(new ConfigDefinition("Recharge Stats", "Recharge Level Shield"), 0.0f, new ConfigDescription("Recharge this amount of shield per the target's level per interval.", null, Array.Empty<object>()));
			RechargeMin = Config.Bind<float>(new ConfigDefinition("Recharge Stats", "Minimum Recharge"), 0.0f, new ConfigDescription("The minimum amount of recharge the target can gain per interval.", null, Array.Empty<object>()));
			AttackBonus = Config.Bind<float>(new ConfigDefinition("Other Stats", "Attack Speed Bonus"), 0.3f, new ConfigDescription("How much Attack Speed to grant to the target.", null, Array.Empty<object>()));
			MoveBonus = Config.Bind<float>(new ConfigDefinition("Other Stats", "Move Speed Bonus"), 0.3f, new ConfigDescription("How much Move Speed to grant to the target.", null, Array.Empty<object>()));
			DamageBonus = Config.Bind<float>(new ConfigDefinition("Other Stats", "Damage Bonus"), 4.0f, new ConfigDescription("How much Damage to grant to the target.", null, Array.Empty<object>()));
			CritBonus = Config.Bind<float>(new ConfigDefinition("Other Stats", "Crit Bonus"), 10.0f, new ConfigDescription("How much Crit to grant to the target.", null, Array.Empty<object>()));
			ArmorBonus = Config.Bind<float>(new ConfigDefinition("Other Stats", "Armor Bonus"), 0.0f, new ConfigDescription("How much Armor to grant to the target.", null, Array.Empty<object>()));
			BossBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Mithrix Phase Banners"), 1.0f, new ConfigDescription("Players equipped with war banners will place one down at the start of Mithrix's phases. (Except the item steal phase.) (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			PillarBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Moon Pillar Banners"), 0.5f, new ConfigDescription("Players equipped with war banners will place one down at the start of a Moon Pillar event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
			VoidBanner = Config.Bind<float>(new ConfigDefinition("Placement Events", "Void Cell Banners"), 0.3f, new ConfigDescription("Players equipped with war banners will place one down at the start of a Void Cell event. (X = Banner radius multiplier for banners placed from this.) (0.0 or less disables this.)", null, Array.Empty<object>()));
		}
	}
	public class NewBuffFX : MonoBehaviour
	{
		public TemporaryVisualEffect effect;
	}
}
