using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class Stealthkit
	{
		public static BuffDef StealthBuff = RoR2Content.Buffs.Cloak;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Old War Stealthkit");
			CreateBuff();
			Hooks();
			UpdateText();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Become stealthed at low health.";
			string desc = "";
			desc += "Falling below <style=cIsHealth>25% health</style> causes you to become <style=cIsUtility>stealthed</style>, gaining <style=cIsUtility>40% movement speed</style> and <style=cIsUtility>invisibility</style>.";
			if (MainPlugin.StealthKit_CancelCombat.Value || MainPlugin.StealthKit_CancelDanger.Value)
			{
				bool DoAnAnd = false;
				desc += " Being <style=cIsUtility>stealthed</style> forces you out of";
				if(MainPlugin.StealthKit_CancelCombat.Value)
                {
					desc += " <style=cIsDamage>combat</style>";
					DoAnAnd = true;
				}
				if (MainPlugin.StealthKit_CancelDanger.Value)
				{
					if(DoAnAnd)
                    {
						desc += " and";
					}
					desc += " <style=cIsHealing>danger</style>";
				}
				desc += ".";
			}
			desc += " Lasts <style=cIsUtility>5s</style> and recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.";
			LanguageAPI.Add("ITEM_PHASING_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PHASING_DESC", desc);
		}
		private static void Hooks()
		{
			On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
			On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += CharacterBody_GetVisibility;
			if (MainPlugin.StealthKit_CancelCombat.Value)
			{
				On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
			}
			if (MainPlugin.StealthKit_CancelDanger.Value)
			{
				On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
			}
			Stealthkit_Override();
			RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsHook;
		}
		private static void CreateBuff()
        {
			StealthBuff = Modules.Buffs.AddNewBuff("Stealthed", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCloak.asset").WaitForCompletion().iconSprite, Color.blue, false, false, false);
		}
		private static void Stealthkit_Override()
        {
			On.RoR2.Items.PhasingBodyBehavior.FixedUpdate += (orig, self) =>
			{
				if(!self.body.healthComponent.alive)
                {
					return;
                }
				self.rechargeStopwatch += Time.fixedDeltaTime;
				if(self.body.healthComponent.isHealthLow) //&& !self.body.hasCloakBuff)
                {
					//Mathf.Pow(self.rechargeReductionMultiplierPerStack, self.stack - 1);
					float cooldown = self.baseRechargeSeconds / (1 + ((self.stack-1) * 0.5f));

					if (self.rechargeStopwatch >= self.buffDuration + cooldown) //self.baseRechargeSeconds * cooldown)
                    {
						self.body.AddTimedBuff(StealthBuff.buffIndex, self.buffDuration);
						//self.body.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, self.buffDuration);
						//self.body.AddTimedBuff(StealthBuff.buffIndex, self.buffDuration);
						EffectManager.SpawnEffect(self.effectPrefab, new EffectData
						{
							origin = self.transform.position,
							rotation = Quaternion.identity
						}, true);
						self.rechargeStopwatch = 0f;
					}
                }
			};
		}
		private static void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
			orig(self, damageReport);
			if (self.HasBuff(StealthBuff.buffIndex))
			{
				self.outOfDanger = true;
				if (self.outOfDangerStopwatch < 7f)
				{
					self.outOfDangerStopwatch = 7f;
				}
			}
		}
		private static void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig(self, skill);
			if (self.HasBuff(StealthBuff.buffIndex))
            {
				self.outOfCombat = true;
				if (self.outOfCombatStopwatch < 5f)
				{
					self.outOfCombatStopwatch = 5f;
				}
			}
        }
		private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
			if(self.HasBuff(StealthBuff.buffIndex))
            {
				if (MainPlugin.StealthKit_CancelCombat.Value)
				{
					self.outOfCombat = true;
					if (self.outOfCombatStopwatch < 5f)
					{
						self.outOfCombatStopwatch = 5f;
					}
				}
				if (MainPlugin.StealthKit_CancelDanger.Value)
				{
					self.outOfDanger = true;
					if (self.outOfDangerStopwatch < 7f)
					{
						self.outOfDangerStopwatch = 7f;
					}
				}
			}
			orig(self);
        }
		private static VisibilityLevel CharacterBody_GetVisibility(On.RoR2.CharacterBody.orig_GetVisibilityLevel_TeamIndex orig, CharacterBody self, TeamIndex observerTeam)
        {
			var result = orig(self, observerTeam);
			if (self.HasBuff(StealthBuff.buffIndex))
			{
				if (observerTeam != self.teamComponent.teamIndex)
				{
					return VisibilityLevel.Cloaked;
				}
				return VisibilityLevel.Revealed;
			}
			return result;
		}
		private static void RecalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.HasBuff(StealthBuff))
			{
				args.moveSpeedMultAdd += 0.4f;
			}
		}
	}
}
