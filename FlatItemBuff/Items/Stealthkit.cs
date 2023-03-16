using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Stealthkit
	{
		public static BuffDef StealthBuff = RoR2Content.Buffs.Cloak;
		internal static bool Enable = true;
		internal static float BaseRecharge = 30.0f;
		internal static float StackRecharge = 0.5f;
		internal static float BuffDuration = 5.0f;
		internal static float GraceDuration = 1.0f;
		internal static bool CancelCombat = true;
		internal static bool CancelDanger = true;
		internal static bool CleanseDoT = true;
		public Stealthkit()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Old War Stealthkit");
			CreateBuffs();
			Hooks();
			UpdateText();
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Become stealthed at low health.";
			string desc = "";
			desc += "Falling below <style=cIsHealth>25% health</style> causes you to become <style=cIsUtility>stealthed</style>, gaining <style=cIsUtility>40% movement speed</style>";
			if (CancelCombat || CancelDanger)
			{
				desc += ", <style=cIsUtility>invisibility</style> and forces you out of";
				bool DoAnAnd = false;
				if(CancelCombat)
                {
					desc += " <style=cIsDamage>combat</style>";
					DoAnAnd = true;
				}
				if (CancelDanger)
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
			desc += string.Format(" Lasts <style=cIsUtility>{0}s</style> and recharges every <style=cIsUtility>{1} seconds</style> <style=cStack>(-{2}% per stack)</style>.", BuffDuration, BaseRecharge, StackRecharge * 100);
			LanguageAPI.Add("ITEM_PHASING_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PHASING_DESC", desc);
		}
		private void Hooks()
		{
			Stealthkit_Override();
			On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += CharacterBody_GetVisibility;
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			if (CancelDanger || CancelCombat)
			{
				if (GraceDuration > 0.0f)
				{
					if (CancelCombat)
					{
						On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
					}
					if (CancelDanger)
					{
						On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
					}
					On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
				}
			}
		}
		private void CreateBuffs()
        {
			StealthBuff = Modules.Buffs.AddNewBuff("Stealthed", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCloak.asset").WaitForCompletion().iconSprite, new Color(0.266f, 0.368f, 0.713f, 1f), false, false, false);
		}
		private void Stealthkit_Override()
        {
			On.RoR2.Items.PhasingBodyBehavior.FixedUpdate += (orig, self) =>
			{
				CharacterBody body = self.body;
				if (!body.healthComponent.alive)
                {
					return;
                }
				self.rechargeStopwatch += Time.fixedDeltaTime;
				if(body.healthComponent.isHealthLow)
                {
					if (!body.HasBuff(StealthBuff.buffIndex))
					{
						float cooldown = BaseRecharge / (1 + (StackRecharge * (self.stack - 1)));
						if (self.rechargeStopwatch >= cooldown)
						{
							body.AddTimedBuff(StealthBuff.buffIndex, BuffDuration);
							EffectManager.SpawnEffect(self.effectPrefab, new EffectData
							{
								origin = self.transform.position,
								rotation = Quaternion.identity
							}, true);
							self.rechargeStopwatch = 0f;

							if (CleanseDoT)
							{
								Util.CleanseBody(self.body, false, false, false, true, true, false);
							}
							if (CancelCombat || CancelDanger)
							{
								if (CancelCombat)
								{
									self.body.outOfCombat = true;
									self.body.outOfCombatStopwatch = float.PositiveInfinity;
								}
								if (CancelDanger)
								{
									self.body.outOfDanger = true;
									self.body.outOfDangerStopwatch = float.PositiveInfinity;
								}

								if (GraceDuration > 0.0f)
								{
									Components.CancelBuffer comp = self.body.GetComponent<Components.CancelBuffer>();
									if (!comp)
									{
										comp = self.body.gameObject.AddComponent<Components.CancelBuffer>();
									}
									comp.duration = GraceDuration;
								}
							}
						}
					}
                }
			};
		}
		private VisibilityLevel CharacterBody_GetVisibility(On.RoR2.CharacterBody.orig_GetVisibilityLevel_TeamIndex orig, CharacterBody self, TeamIndex observerTeam)
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
		private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
		{
			orig(self, damageReport);
			if (HasBuffer(self))
			{
				self.outOfDanger = true;
				self.outOfDangerStopwatch = float.PositiveInfinity;
			}
		}
		private void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig(self, skill);
			if (HasBuffer(self))
			{
				self.outOfCombat = true;
				self.outOfCombatStopwatch = float.PositiveInfinity;
			}
		}
		private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
		{
			orig(self, buffDef);
			if (buffDef == StealthBuff)
			{
				if (HasBuffer(self))
				{
					Components.CancelBuffer comp = self.GetComponent<Components.CancelBuffer>();
					if (comp)
					{
						comp.duration = -1.0f;
					}
				}
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			if (sender.HasBuff(StealthBuff))
			{
				args.moveSpeedMultAdd += 0.4f;
			}
		}
		private bool HasBuffer(CharacterBody body)
		{
			return body.GetComponent<Components.CancelBuffer>();
		}
	}
}
