using System;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace FlatItemBuff.Items
{
	public class HuntersHarpoon
	{
		public static BuffDef HarpoonBuff = DLC1Content.Buffs.KillMoveSpeed;
		private static Color BuffColor = new Color(0.717f, 0.545f, 0.952f, 1f);
		private static bool DoesCool = true;

		internal static bool Enable = true;
		internal static float BaseDuration = 1.5f;
		internal static float StackDuration = 0.75f;
		internal static float MovementSpeed = 0.2f;
		internal static float CooldownRate = 0.2f;
		internal static bool CoolPrimary = true;
		internal static bool CoolSecondary = true;
		internal static bool CoolUtility = false;
		internal static bool CoolSpecial = false;
		public HuntersHarpoon()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Hunter's Harpoon");
			ClampConfig();
			UpdateText();
			CreateBuff();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			MovementSpeed = Math.Max(0f, MovementSpeed);
			CooldownRate = Math.Max(0f, CooldownRate);
			if (!CoolPrimary && !CoolSecondary && !CoolUtility && !CoolSpecial)
			{
				DoesCool = false;
			}
		}
		private void CreateBuff()
		{
			HarpoonBuff = Modules.Buffs.AddNewBuff("HunterBoost", Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/MoveSpeedOnKill/bdKillMoveSpeed.asset").WaitForCompletion().iconSprite, BuffColor, true, false, false);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickupSpeed = "";
			string descSpeed = "";
			if (MovementSpeed > 0f)
            {
				pickupSpeed = " movement speed";
				descSpeed = string.Format(" <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style>", MovementSpeed * 500);
			}
			string pickupCooldown = "";
			string descCooldown = "";
			if (DoesCool)
            {
				if (MovementSpeed > 0f)
                {
					pickupCooldown += " and";
					descCooldown += " and";
				}
				
				if (!CoolPrimary || !CoolSecondary || !CoolUtility || !CoolSpecial)
                {
					List<string> skillStrings = new List<string>();
					List<string> skillStringsPik = new List<string>();
					if (CoolPrimary)
					{
						skillStringsPik.Add(" primary");
						skillStrings.Add(" <style=cIsUtility>Primary</style>");
					}
					if (CoolSecondary)
					{
						skillStringsPik.Add(" secondary");
						skillStrings.Add(" <style=cIsUtility>Secondary</style>");
					}
					if (CoolUtility)
					{
						skillStringsPik.Add(" utility");
						skillStrings.Add(" <style=cIsUtility>Utility</style>");
					}
					if (CoolSpecial)
					{
						skillStringsPik.Add(" special");
						skillStrings.Add(" <style=cIsUtility>Special</style>");
					}
					for(int i = 0; i<skillStrings.Count; i++)
                    {
						if (i > 0)
                        {
							
							if (i < skillStrings.Count - 1)
                            {
								pickupCooldown += ",";
								descCooldown += ",";
							}
							else
                            {
								pickupCooldown += " and";
								descCooldown += " and";
							}
                        }
						pickupCooldown += skillStringsPik[i];
						descCooldown += skillStrings[i];

					}
				}
				pickupCooldown += " skill cooldown rate";
				descCooldown += string.Format(" <style=cIsUtility>skill cooldown rate</style> by <style=cIsUtility>{0}%</style>", CooldownRate * 500);
			}
			string pickup = string.Format("Killing an enemy gives a burst of{0}{1}.", pickupSpeed, pickupCooldown);
			string desc = string.Format("Killing an enemy increases{0}{1}. Fades over <style=cIsUtility>{2}</style> <style=cStack>(+{3} per stack)</style> seconds.", descSpeed, descCooldown, BaseDuration, StackDuration);
			LanguageAPI.Add("ITEM_MOVESPEEDONKILL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_MOVESPEEDONKILL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			if (MovementSpeed != 0f)
            {
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
			if(CooldownRate != 0f && DoesCool)
            {
				On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            }
		}
		private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
		{
			orig(self);
			if (self)
			{
				int buffCount = self.GetBuffCount(HarpoonBuff.buffIndex);
				if (buffCount > 0)
				{
					SkillLocator skillLocator = self.skillLocator;
					if (skillLocator)
					{
						float cooldown = Time.fixedDeltaTime * (buffCount * CooldownRate);
						if (CoolPrimary)
						{
							GenericSkill primary = skillLocator.primary;
							if (primary)
							{
								primary.RunRecharge(cooldown);
							}
						}
						if (CoolSecondary)
						{
							GenericSkill secondary = skillLocator.secondary;
							if (secondary)
							{
								secondary.RunRecharge(cooldown);
							}
						}
						if (CoolUtility)
						{
							GenericSkill utility = skillLocator.utility;
							if (utility)
							{
								utility.RunRecharge(cooldown);
							}
						}
						if (CoolSpecial)
						{
							GenericSkill special = skillLocator.special;
							if (special)
							{
								special.RunRecharge(cooldown);
							}
						}
					}
				}
			}
        }
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int buffCount = sender.GetBuffCount(HarpoonBuff.buffIndex);
			if (buffCount > 0)
			{
				args.moveSpeedMultAdd += buffCount * MovementSpeed;
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterBody attacker = damageReport.attackerBody;
			if (attacker.inventory)
			{
				int itemCount = attacker.inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
				if (itemCount > 0)
				{
					float duration = BaseDuration;
					duration += (itemCount - 1) * StackDuration;
					if (duration > 0f)
					{
						attacker.ClearTimedBuffs(HarpoonBuff.buffIndex);
						for (int i = 0; i < 5; i++)
						{
							attacker.AddTimedBuff(HarpoonBuff.buffIndex, duration * (i + 1) / 5);
						}
						EffectData effectData = new EffectData();
						effectData.origin = attacker.corePosition;
						effectData.rotation = attacker.transform.rotation;
						CharacterMotor characterMotor = attacker.characterMotor;
						if (characterMotor)
						{
							Vector3 moveDirection = characterMotor.moveDirection;
							if (moveDirection != Vector3.zero)
							{
								effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
							}
						}
						EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MoveSpeedOnKillActivate"), effectData, true);
					}
				}
			}
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(DLC1Content.Items), "MoveSpeedOnKill"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			);
			if(ilcursor.Index > 0)
            {
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
		}
	}
}
