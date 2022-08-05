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
		private static bool CanCancel = false;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Old War Stealthkit");
			if (MainPlugin.StealthKit_CancelDuration.Value > 0.0f)
            {
				if (MainPlugin.StealthKit_CancelCombat.Value || MainPlugin.StealthKit_CancelDanger.Value)
				{
					CanCancel = true;
				}
			}
			CreateBuffs();
			Hooks();
			UpdateText();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Become stealthed at low health.";
			string desc = "";
			desc += "Falling below <style=cIsHealth>25% health</style> causes you to become <style=cIsUtility>stealthed</style>, gaining <style=cIsUtility>40% movement speed</style>";
			if (CanCancel)
			{
				desc += ", <style=cIsUtility>invisibility</style> and forces you out of";
				bool DoAnAnd = false;
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
			else
            {
				desc += "and <style=cIsUtility>invisibility</style>.";
			}
			desc += string.Format(" Lasts <style=cIsUtility>{0}s</style> and recharges every <style=cIsUtility>{1} seconds</style> <style=cStack>(-{2}% per stack)</style>.", MainPlugin.StealthKit_BuffDuration.Value, MainPlugin.StealthKit_BaseRecharge.Value, MainPlugin.StealthKit_StackRecharge.Value * 100);
			LanguageAPI.Add("ITEM_PHASING_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PHASING_DESC", desc);
		}
		private static void Hooks()
		{
			if (CanCancel)
            {
				On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
				if (MainPlugin.StealthKit_CancelCombat.Value)
				{
					On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
				}
				if (MainPlugin.StealthKit_CancelDanger.Value)
				{
					On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
				}
			}
			Stealthkit_Override();
			On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += CharacterBody_GetVisibility;
			RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsHook;
		}
		private static void CreateBuffs()
        {
			StealthBuff = Modules.Buffs.AddNewBuff("Stealthed", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCloak.asset").WaitForCompletion().iconSprite, new Color(0.266f, 0.368f, 0.713f, 1f), false, false, false);
			//Hate the idea of using a component for this, but the grace period just isn't suitable as a buff/debuff/cooldown.
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
				if(self.body.healthComponent.isHealthLow)
                {
					if (!self.body.HasBuff(StealthBuff.buffIndex))
					{
						float cooldown = MainPlugin.StealthKit_BaseRecharge.Value / (1 + (MainPlugin.StealthKit_StackRecharge.Value * (self.stack - 1)));
						if (self.rechargeStopwatch >= cooldown)
						{
							self.body.AddTimedBuff(StealthBuff.buffIndex, MainPlugin.StealthKit_BuffDuration.Value);
							if (CanCancel)
							{
								Components.CancelBuffer comp = self.body.GetComponent<Components.CancelBuffer>();
								if (!comp)
								{
									comp = self.body.gameObject.AddComponent<Components.CancelBuffer>();
								}
								comp.duration = MainPlugin.StealthKit_CancelDuration.Value;
							}
							EffectManager.SpawnEffect(self.effectPrefab, new EffectData
							{
								origin = self.transform.position,
								rotation = Quaternion.identity
							}, true);
							self.rechargeStopwatch = 0f;
						}
					}
                }
			};
		}
		private static bool HasBuffer(CharacterBody body)
        {
			return body.GetComponent<Components.CancelBuffer>();
		}
		private static void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
			orig(self, damageReport);
			if (HasBuffer(self))
			{
				self.outOfDanger = true;
				self.outOfDangerStopwatch = float.PositiveInfinity;
			}
		}
		private static void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig(self, skill);
			if (HasBuffer(self))
			{
				self.outOfCombat = true;
				self.outOfCombatStopwatch = float.PositiveInfinity;
			}
		}
		private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
			if (HasBuffer(self))
            {
				if(self.HasBuff(StealthBuff.buffIndex))
                {
					if (MainPlugin.StealthKit_CancelCombat.Value)
					{
						self.outOfCombat = true;
						self.outOfCombatStopwatch = float.PositiveInfinity;
					}
					if (MainPlugin.StealthKit_CancelDanger.Value)
					{
						self.outOfDanger = true;
						self.outOfDangerStopwatch = float.PositiveInfinity;
					}
				}
				else
                {
					Components.CancelBuffer comp = self.GetComponent<Components.CancelBuffer>();
					if (comp)
					{
						comp.duration = -1.0f;
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
