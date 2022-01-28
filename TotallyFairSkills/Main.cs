using System;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using TotallyFairSkills.Modules;
using TotallyFairSkills.Components;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TotallyFairSkills
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI",
		"RecalculateStatsAPI",
		"LoadoutAPI"
	})]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class Main : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.TotallyFairSkills";
		public const string MODNAME = "TotallyFairSkills";
		public const string MODTOKEN = "KKING117_TOTALLYFAIRSKILLS_";
		public const string MODVERSION = "1.1.0";

		public static GameObject CommandoBody = Resources.Load<GameObject>("prefabs/characterbodies/CommandoBody");

		public static ConfigEntry<bool> FMJMK2_Enable;
		public static ConfigEntry<float> FMJMK2_Damage;
		public static ConfigEntry<float> FMJMK2_Force;
		public static ConfigEntry<float> FMJMK2_Cooldown;
		public static ConfigEntry<bool> FMJMK2_ActionArmy;

		public static ConfigEntry<bool> ShowTime_Enable;
		public static ConfigEntry<float> ShowTime_Cooldown;
		public static ConfigEntry<float> ShowTime_ReadyDuration;

		public static ConfigEntry<float> ShowOff_ActiveDuration;
		public static ConfigEntry<float> ShowOff_Crit;
		public static ConfigEntry<float> ShowOff_ExcessCritCap;
		public static ConfigEntry<float> ShowOff_ExcessCritBonus;
		public static ConfigEntry<float> ShowOff_Luck;
		public static ConfigEntry<float> ShowOff_CritThresh;
		public static ConfigEntry<float> ShowOff_CritBandLogic;

		public void Awake()
		{
			ReadConfig();
			if (ShowTime_Enable.Value)
			{
				RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
				On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
				On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
				On.RoR2.Util.CheckRoll_float_float_CharacterMaster += Util_CheckRoll_float_float_CharacterMaster;
				On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
			}
			On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
			Logger.LogInfo("Registering Buffs");
			Modules.Buffs.RegisterBuffs();
			Logger.LogInfo("Registering Skills");
			Modules.Skills.RegisterSkills();
			Logger.LogInfo("Registering States");
			Modules.States.RegisterStates();
			Logger.LogInfo("Registering Projectiles");
			Modules.Projectiles.RegisterProjectiles();
			Logger.LogInfo("Changing loadouts");
			EditLoadouts();
			Logger.LogInfo("Initializing ContentPack");
			new Modules.ContentPacks().Initialize();

			/*On.RoR2.Util.PlaySound_string_GameObject += delegate (On.RoR2.Util.orig_PlaySound_string_GameObject orig, string soundString, GameObject gameObject)
			{
				print(soundString);
				uint result = orig(soundString, gameObject);
				return result;
			};*/
			//texToolbotSkillIcons_3
		}
		private void EditLoadouts()
		{
			if (FMJMK2_Enable.Value)
			{
				Modules.Skills.AddSkillToSlot(CommandoBody, Modules.Skills.FMJMK2Skill, SkillSlot.Secondary);
			}
			if (ShowTime_Enable.Value)
			{
				Modules.Skills.AddSkillToSlot(CommandoBody, Modules.Skills.ShowTimeSkill, SkillSlot.Utility);
			}
		}
		private void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, RoR2.CharacterBody self, GenericSkill skill)
		{
			orig(self, skill);
			if (skill.isCombatSkill)
			{
				if (self.HasBuff(Buffs.ShowOff))
				{
					self.AddTimedBuff(Buffs.ShowOffActive, ShowOff_ActiveDuration.Value);
					self.ClearTimedBuffs(Buffs.ShowOff);
				}
			}
		}
		private void CalculateStatsHook(RoR2.CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if(sender.HasBuff(Buffs.ShowOff) || sender.HasBuff(Buffs.ShowOffActive))
            {
				args.critAdd += ShowOff_Crit.Value;
            }
		}
		private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
		{
			orig(self);
			if (self.HasBuff(Buffs.ShowOff) || self.HasBuff(Buffs.ShowOffActive))
			{
				if (self.crit > 100.0f)
                {
					float critbonus = Math.Min(ShowOff_ExcessCritCap.Value, self.crit - 100.0f);
					critbonus *= ShowOff_ExcessCritBonus.Value;
					if (critbonus > 0f)
					{
						self.damage += self.damage * critbonus;
					}
				}
			}
		}
		private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
		{
			if (damageInfo != null)
			{
				if (!damageInfo.rejected)
				{
					if (damageInfo.attacker)
					{
						CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
						if (ShowTime_Enable.Value)
						{
							if (attackerBody)
							{
								if (damageInfo.dotIndex == DotController.DotIndex.None)
								{
									if (self.body.HasBuff(Buffs.ShowOff) || self.body.HasBuff(Buffs.ShowOffActive))
									{
										if (CanCancelShowOff(self, attackerBody, damageInfo.damage))
										{
											damageInfo.crit = true;
											ReadyShowOffCancel(self.body.gameObject, damageInfo);
										}
									}
								}
							}
						}
						if (FMJMK2_Enable.Value)
						{
							if (damageInfo.inflictor)
							{
								GameObject inflictor = damageInfo.inflictor.gameObject;
								if (inflictor.name == Modules.Projectiles.FMJMK2Prefab.name + "(Clone)")
								{
									RoR2.Projectile.ProjectileDamage pd = inflictor.GetComponent<RoR2.Projectile.ProjectileDamage>();
									if (pd)
									{
										if (!self.body.isChampion)
										{
											float baseforce = pd.force * 0.5f;
											float mass = 1f;
											if (self.body.characterMotor)
											{
												mass = self.body.characterMotor.mass;
												if (!self.body.characterMotor.isGrounded)
												{
													baseforce *= 0.5f;
												}
											}
											else if (self.body.rigidbody)
											{
												mass = self.body.rigidbody.mass;
											}
											mass *= 0.01f;
											mass = (float)Math.Pow(mass, 0.9f);
											Vector3 forcedir = damageInfo.force.normalized;
											forcedir *= baseforce * mass;
											forcedir.y *= 0.6f;
											damageInfo.force += forcedir;
										}
									}
								}
							}
						}
					}
				}
			}
			orig(self, damageInfo);
		}
		private bool CanCancelShowOff(HealthComponent self, CharacterBody attacker, float damage)
        {
			if (GetBaseHealthOrHighest(self) * ShowOff_CritThresh.Value <= damage)
			{
				return true;
			}
			else if (ShowOff_CritBandLogic.Value >= 0f)
			{
				if (damage >= attacker.damage * ShowOff_CritBandLogic.Value)
				{
					return true;
				}
			}
			return false;
		}
		private void ReadyShowOffCancel(GameObject gameObject, DamageInfo damageInfo)
        {
			if (gameObject)
            {
				ShowOffCancel component = gameObject.GetComponent<ShowOffCancel>();
				if (!component)
				{
					component = gameObject.AddComponent<ShowOffCancel>();
				}
				component.damageInfo = damageInfo;
			}
        }
		private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
		{
			orig(self, damageReport);
			if (!damageReport.damageInfo.rejected)
            {
				ShowOffCancel component = self.gameObject.GetComponent<ShowOffCancel>();
				if (component)
				{
					if (component.damageInfo == damageReport.damageInfo)
                    {
						self.ClearTimedBuffs(Buffs.ShowOff);
						self.ClearTimedBuffs(Buffs.ShowOffActive);
						component.Remove();
					}
				}
			}
		}
		private float GetBaseHealthOrHighest(HealthComponent self)
        {
			float result = 0f;
			if (self.body)
			{
				result = self.body.baseMaxHealth + (self.body.levelMaxHealth * (self.body.level - 1));
				if (result < self.fullCombinedHealth)
				{
					result = self.fullCombinedHealth;
				}
			}
			return result;
        }
		public bool Util_CheckRoll_float_float_CharacterMaster(On.RoR2.Util.orig_CheckRoll_float_float_CharacterMaster orig, float percentChance, float luck, CharacterMaster effectOriginMaster)
		{
			if (effectOriginMaster)
			{
				CharacterBody body = effectOriginMaster.GetBody();
				if (body)
				{
					if (body.HasBuff(Buffs.ShowOff) || body.HasBuff(Buffs.ShowOffActive))
					{
						luck += ShowOff_Luck.Value;
					}
				}
			}
			return orig(percentChance, luck, effectOriginMaster);
		}
		public void ReadConfig()
		{
			ShowTime_Enable = Config.Bind<bool>(new ConfigDefinition("ShowTime Skill", "Enable"), true, new ConfigDescription("Enables the Showtime Skill and its buffs.", null, Array.Empty<object>()));
			ShowTime_Cooldown = Config.Bind<float>(new ConfigDefinition("ShowTime Skill", "Cooldown"), 8, new ConfigDescription("Cooldown time of this skill.", null, Array.Empty<object>()));
			ShowTime_ReadyDuration = Config.Bind<float>(new ConfigDefinition("ShowTime Skill", "Ready Duration"), 3.5f, new ConfigDescription("How long this skill gives Show-Off for.", null, Array.Empty<object>()));

			ShowOff_ActiveDuration = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Active Duration"), 2.0f, new ConfigDescription("How long Show-Off lasts for when activated.", null, Array.Empty<object>()));
			ShowOff_Crit = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Crit"), 25f, new ConfigDescription("Crit bonus that Show-Off gives.", null, Array.Empty<object>()));
			ShowOff_ExcessCritCap = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Excess Crit Cap"), 25f, new ConfigDescription("The excess crit chance cap bonus that Show-Off gives. (25 = Extra damage at up to 125% crit chance)", null, Array.Empty<object>()));
			ShowOff_ExcessCritBonus = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Excess Crit Bonus Damage"), 0.01f, new ConfigDescription("Extra damage from excess crit damage that Show-Off gives. (0.01 = +1% damage per 1% excess crit chance)", null, Array.Empty<object>()));
			ShowOff_Luck = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Luck"), 1f, new ConfigDescription("Luck bonus that Show-Off gives.", null, Array.Empty<object>()));
			ShowOff_CritThresh = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Crit Threshold"), 0.15f, new ConfigDescription("Percent of base health lost from a hit that will cause critical damage and cancel Show-Off.", null, Array.Empty<object>()));
			ShowOff_CritBandLogic = Config.Bind<float>(new ConfigDefinition("Show-Off Buff", "Crit Damage Percent"), 4f, new ConfigDescription("If an attack deals this much base damage or more (similar to bands) it will cause critical damage and cancel Show-Off. (Set to -1f to disable this.)", null, Array.Empty<object>()));

			ShowOff_ExcessCritCap.Value = Math.Max(0f, ShowOff_ExcessCritCap.Value);

			FMJMK2_Enable = Config.Bind<bool>(new ConfigDefinition("FMJ MkII", "Enable"), true, new ConfigDescription("Enables the FMJ MkII.", null, Array.Empty<object>()));
			FMJMK2_Damage = Config.Bind<float>(new ConfigDefinition("FMJ MkII", "Damage"), 4f, new ConfigDescription("Damage coefficient of this skill.", null, Array.Empty<object>()));
			FMJMK2_Force = Config.Bind<float>(new ConfigDefinition("FMJ MkII", "Knockback"), 2750, new ConfigDescription("Knockback force of this skill.", null, Array.Empty<object>()));
			FMJMK2_Cooldown = Config.Bind<float>(new ConfigDefinition("FMJ MkII", "Cooldown"), 4, new ConfigDescription("Cooldown time of this skill.", null, Array.Empty<object>()));
			FMJMK2_ActionArmy = Config.Bind<bool>(new ConfigDefinition("FMJ MkII", "Gun Twirl"), false, new ConfigDescription("Makes Commando twirl his guns after every use of this skill.", null, Array.Empty<object>()));
		}
	}
}
