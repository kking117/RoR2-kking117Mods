using System;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;

namespace WarBannerBuff
{
	[BepInPlugin("com.kking117.WarBannerBuff", "WarBannerBuff", "1.0.0")]
	[BepInDependency("com.bepis.r2api", "2.5.14")]
	[BepInDependency("com.ThinkInvisible.TILER2", "2.2.2")]
	public class warbannerbuff : BaseUnityPlugin
    {
		private float HealInterval = 1f;
		public void Awake()
		{
			StatHooks.GetStatCoefficients += new StatHooks.StatHookEventHandler(AddBannerBuffs);

			void AddBannerBuffs(CharacterBody sender, StatHooks.StatHookEventArgs args)
			{
				if (sender.HasBuff(BuffIndex.Warbanner))
                {
					args.armorAdd += armorbonus;
					args.baseDamageAdd += damagebonus;
					args.critAdd += critbonus;
                }
			}

			On.RoR2.Run.Start += delegate (On.RoR2.Run.orig_Start orig, Run self)
			{
				orig(self);
				HealInterval = 1f;
			};
			On.RoR2.Run.FixedUpdate += delegate (On.RoR2.Run.orig_FixedUpdate orig, Run self)
			{
				orig(self);
				if (HealInterval <= 0f)
                {
					HealInterval += regeninterval;
				}
				HealInterval -= Time.fixedDeltaTime;
			};
			On.RoR2.CharacterBody.FixedUpdate += delegate (On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
			{
				orig(self);
				if (HealInterval<=0f)
				{
					if (self.HasBuff(BuffIndex.Warbanner))
					{
						HealthComponent healthComponent = self.healthComponent;
						if (healthComponent)
						{
							float healing = healthComponent.fullHealth * regenmaxhealth;
							healing += regenaddhealth * self.level;
							if (healing < minheal)
							{
								healing = minheal;
							}
							healthComponent.Heal(healing, default, true);

							healing = healthComponent.fullShield * regenmaxshield;
							healing += regenaddshield * self.level;
							if (healing < minshield)
							{
								healing = minshield;
							}
							healthComponent.RechargeShield(healing);
						}
					}
				}
			};

			On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += delegate (On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
			{
				orig(self);
				if (bossbanner)
				{
					SpawnTeamWarBanners(TeamIndex.Player);
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += delegate (On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
			{
				orig(self);
				if (bossbanner)
				{
					SpawnTeamWarBanners(TeamIndex.Player);
				}
			};
			On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += delegate (On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
			{
				orig(self);
				if (bossbanner)
				{
					SpawnTeamWarBanners(TeamIndex.Player);
				}
			};

			void SpawnTeamWarBanners(TeamIndex team)
            {
				for (int i = 0; i < RoR2.PlayerCharacterMasterController.instances.Count; i++)
				{
					CharacterBody charbody = RoR2.PlayerCharacterMasterController.instances[i].master.GetBody();
					if(charbody)
                    {
						TeamComponent teamComponent = charbody.teamComponent;
						if(teamComponent.teamIndex == team)
                        {
							HealthComponent healthComponent = charbody.healthComponent;
							if (healthComponent && healthComponent.health > 0)
							{
								CharacterMaster master = charbody.master;
								if (master)
								{
									int itemCount = master.inventory.GetItemCount(ItemIndex.WardOnLevel);
									if (itemCount > 0)
									{
										GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), teamComponent.transform.position, Quaternion.identity);
										gameObject.GetComponent<TeamFilter>().teamIndex = teamComponent.teamIndex;
										gameObject.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount;
										NetworkServer.Spawn(gameObject);
									}
								}
							}
						}
					}
				}
			};
		}
		public static ConfigFile CustomConfigFile = new ConfigFile(Paths.ConfigPath + "\\kking117.WarBannerBuff.cfg", true);

		public static ConfigEntry<float> regeninterval_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Regen Inteval", 0.5f, "Banners apply the configured regen/recharge effect every Xth second.");
		public float regeninterval = regeninterval_config.Value;

		public static ConfigEntry<float> regenmaxhealth_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Regen Max Health", 0.01f, "Banners regen this % of the target's max health per interval. (0.01f = 1%)");
		public float regenmaxhealth = regenmaxhealth_config.Value;

		public static ConfigEntry<float> regenaddhealth_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Regen Level Health", 1f, "Banners regen this amount of health multiplied by the target's level.");
		public float regenaddhealth = regenaddhealth_config.Value;

		public static ConfigEntry<float> minheal_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Minimum Heal", 1f, "The lowest possible amount of health banners can heal per interval.");
		public float minheal = minheal_config.Value;

		public static ConfigEntry<float> regenmaxshield_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Regen Max Shield", 0.0f, "Banners recharge this % of the target's max shield per interval. (0.01f = 1%)");
		public float regenmaxshield = regenmaxshield_config.Value;

		public static ConfigEntry<float> regenaddshield_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Regen Level Shield", 0f, "Banners recharge this amount of shield multiplied by the target's level.");
		public float regenaddshield = regenaddshield_config.Value;

		public static ConfigEntry<float> minshield_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Minimum Shield", 0f, "The lowest possible amount of shield banners can recharge per interval.");
		public float minshield = minshield_config.Value;

		public static ConfigEntry<int> armorbonus_config = CustomConfigFile.Bind<int>("WarBannerBuff Config", "Armor Bonus", 10, "Banners grant X armor to the target. (https://riskofrain2.gamepedia.com/Armor)");
		public int armorbonus = armorbonus_config.Value;

		public static ConfigEntry<int> damagebonus_config = CustomConfigFile.Bind<int>("WarBannerBuff Config", "Damage Bonus", 4, "Banners grant X base damage to the target.");
		public int damagebonus = damagebonus_config.Value;

		public static ConfigEntry<float> critbonus_config = CustomConfigFile.Bind<float>("WarBannerBuff Config", "Crit Bonus", 9.0f, "Banners grant X crit chance to the target.");
		public float critbonus = critbonus_config.Value;

		public static ConfigEntry<bool> bossbanner_config = CustomConfigFile.Bind<bool>("WarBannerBuff Config", "Mithrix Phase Banners", true, "Makes players equipped with war banners to place one down at the start of each of Mithrix's phases. (Except the item steal phase.)");
		public bool bossbanner = bossbanner_config.Value;
	}
}
