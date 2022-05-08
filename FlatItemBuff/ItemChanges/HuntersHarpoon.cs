using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace FlatItemBuff.ItemChanges
{
	public class HuntersHarpoon
	{
		public static BuffDef HarpoonBuff = DLC1Content.Buffs.KillMoveSpeed;
		private static Color BuffColor = new Color(0.717f, 0.545f, 0.952f, 1f);
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Hunter's Harpoon");
			UpdateText();
			CreateBuff();
			Hooks();
		}
		private static void CreateBuff()
		{
			HarpoonBuff = Modules.Buffs.AddNewBuff("HunterBoost", Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/MoveSpeedOnKill/bdKillMoveSpeed.asset").WaitForCompletion().iconSprite, BuffColor, true, false, false);
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Killing an enemy gives you a burst of";
			string desc = "Killing an enemy increases";
			bool AND = false;
			if (MainPlugin.Harpoon_MoveSpeed.Value > 0f)
            {
				pickup += " movement speed";
				desc += string.Format(" <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style>", MainPlugin.Harpoon_MoveSpeed.Value * 500);
				AND = true;
			}
			if (MainPlugin.Harpoon_BaseDuration.Value > 0f)
			{
				string pickup_skill = " primary and secondary skill cooldown rate";
				string desc_skill = " <style=cIsUtility>primary</style> and <style=cIsUtility>secondary skill cooldown rate</style>";
				if (AND)
                {
					pickup += " and also";
					desc += " and also";
				}
				pickup += pickup_skill;
				desc += string.Format("{0} by <style=cIsUtility>{1}%</style>", desc_skill, MainPlugin.Harpoon_CooldownRate.Value * 500);
			}
			pickup += ".";
			desc += ".";
			desc += string.Format(" Fades over <style=cIsUtility>{0}</style> <style=cStack>(+{1} per stack)</style> seconds.", MainPlugin.Harpoon_BaseDuration.Value, MainPlugin.Harpoon_StackDuration.Value);
			LanguageAPI.Add("ITEM_MOVESPEEDONKILL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_MOVESPEEDONKILL_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
			if(MainPlugin.Harpoon_MoveSpeed.Value != 0f)
            {
				RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			}
			if(MainPlugin.Harpoon_CooldownRate.Value != 0f)
            {
				On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            }
		}
		private static void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
			orig(self);
			int buffCount = self.GetBuffCount(HarpoonBuff.buffIndex);
			if (buffCount > 0)
			{
				SkillLocator skillLocator = self.skillLocator;
				if(skillLocator)
                {
					float cooldown = Time.fixedDeltaTime * (buffCount * MainPlugin.Harpoon_CooldownRate.Value);
					GenericSkill primary = skillLocator.primary;
					if (primary)
					{
						primary.RunRecharge(cooldown);
					}
					GenericSkill secondary = skillLocator.secondary;
					if (secondary)
                    {
						secondary.RunRecharge(cooldown);
					}
                }
			}
        }
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			int buffCount = sender.GetBuffCount(HarpoonBuff.buffIndex);
			if (buffCount > 0)
			{
				args.moveSpeedMultAdd += buffCount * MainPlugin.Harpoon_MoveSpeed.Value;
			}
		}
		private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (damageReport.attacker && damageReport.attackerBody)
            {
				CharacterBody attacker = damageReport.attackerBody;
				if (attacker.inventory)
				{
					int itemCount = attacker.inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
					if (itemCount > 0)
					{
						float duration = MainPlugin.Harpoon_BaseDuration.Value;
						duration += (itemCount - 1) * MainPlugin.Harpoon_StackDuration.Value;
						if (duration > 0f)
						{
							attacker.ClearTimedBuffs(HarpoonBuff.buffIndex);
							for(int i = 0; i<5; i++)
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
		}
		private static void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Items", "MoveSpeedOnKill"),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount")
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
