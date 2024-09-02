using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Stealthkit
	{
		public static BuffDef StealthBuff;
		private static Color StealthBuff_Color = new Color(0.266f, 0.368f, 0.713f, 1f);
		internal static bool Enable = true;

		internal static float BuffDuration = 5.0f;
		internal static float BaseRecharge = 30.0f;
		internal static float StackRecharge = 0.5f;
		
		internal static float Stealth_MoveSpeed = 0.4f;
		internal static float Stealth_ArmorPerBuff = 20f;
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
			ClampConfig();
			CreateBuffs();
			Hooks();
			UpdateText();
		}
		private void ClampConfig()
		{
			BuffDuration = Math.Max(0f, BuffDuration);
			BaseRecharge = Math.Max(0f, BaseRecharge);
			StackRecharge = Math.Max(0f, StackRecharge);
			Stealth_MoveSpeed = Math.Max(0f, Stealth_MoveSpeed);
			Stealth_ArmorPerBuff = Math.Max(0f, Stealth_ArmorPerBuff);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Become stealthed at low health.";
			string desc = "";
			string desc_stealth = " While <style=cIsUtility>stealthed</style> gain <style=cIsUtility>invisibility</style>";
			if (Stealth_MoveSpeed > 0f || Stealth_ArmorPerBuff > 0f)
			{
				if (Stealth_MoveSpeed > 0f)
				{
					if (Stealth_ArmorPerBuff > 0f)
                    {
						desc_stealth += string.Format(", <style=cIsUtility>{0}% movement speed</style>", Stealth_MoveSpeed * 100f);
					}
					else
                    {
						desc_stealth += string.Format(" and <style=cIsUtility>{0}% movement speed</style>", Stealth_MoveSpeed * 100f);
					}
				}
				if (Stealth_ArmorPerBuff > 0f)
				{
					desc_stealth += string.Format(" and <style=cIsHealing>{0} fading armor</style>", Stealth_ArmorPerBuff * 5f);
				}
				desc_stealth += ".";
			}
			string desc_cooldown = string.Format(" Recharges every <style=cIsUtility>{0} seconds</style> <style=cStack>(-{1}% per stack)</style>.", BaseRecharge, StackRecharge * 100);

			desc = string.Format("Falling below <style=cIsHealth>25% health</style> causes you to become <style=cIsUtility>stealthed</style> for <style=cIsUtility>{0}s</style>.{1}{2}", BuffDuration, desc_stealth, desc_cooldown);
			LanguageAPI.Add("ITEM_PHASING_PICKUP", pickup);
			LanguageAPI.Add("ITEM_PHASING_DESC", desc);
		}
		private void Hooks()
		{
			Stealthkit_Override();
			On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += CharacterBody_GetVisibility;
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			if (CancelCombat)
			{
				On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
			}
			if (CancelDanger)
			{
				On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
			}
		}
		private void CreateBuffs()
        {
			StealthBuff = Utils.ContentManager.AddBuff("Stealthed", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCloak.asset").WaitForCompletion().iconSprite, StealthBuff_Color, true, false, false);
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
							for (int i = 0; i < 5; i++)
							{
								body.AddTimedBuff(StealthBuff.buffIndex, BuffDuration * (i + 1) / 5);
							}
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
			if (HasStealthBuffer(self))
			{
				self.outOfDanger = true;
				self.outOfDangerStopwatch = float.PositiveInfinity;
			}
		}
		private void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
		{
			orig(self, skill);
			if (HasStealthBuffer(self))
			{
				self.outOfCombat = true;
				self.outOfCombatStopwatch = float.PositiveInfinity;
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int buffcount = sender.GetBuffCount(StealthBuff);
			if (buffcount > 0)
			{
				args.moveSpeedMultAdd += Stealth_MoveSpeed;
				args.armorAdd = Stealth_ArmorPerBuff * buffcount;
			}
		}
		private bool HasStealthBuffer(CharacterBody body)
		{
			int buffcount = body.GetBuffCount(StealthBuff);
			return buffcount > 4;
		}
	}
}
