using System;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using UnityEngine;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
	public class HuntersHarpoon
	{
		public static BuffDef ReduceCooldownBuff;
		public static BuffDef HarpoonBuff;
		private static Color BuffColor = new Color(0.717f, 0.545f, 0.952f, 1f);
		private static bool DoesCool = true;
		private static bool SingleCool = true;
		public static GameObject BoostVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MoveSpeedOnKill/MoveSpeedOnKillActivate.prefab").WaitForCompletion();

		private const string LogName = "Hunter's Harpoon";
		internal static bool Enable = false;
		internal static float BaseDuration = 1f;
		internal static float StackDuration = 1f;
		internal static float MovementSpeed = 1.25f;
		internal static float BaseCooldownReduction = 1f;
		internal static bool ExtendDuration = true;
		internal static bool CoolPrimary = true;
		internal static bool CoolSecondary = true;
		internal static bool CoolUtility = false;
		internal static bool CoolSpecial = false;
		internal static bool Comp_AssistManager = true;
		public HuntersHarpoon()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			CreateBuff();
			Hooks();
			if (MainPlugin.AssistManager_Loaded)
			{
				ApplyAssistManager();
			}
		}
		private void ApplyAssistManager()
		{
			AssistManager.VanillaTweaks.Harpoon.Instance.SetEnabled(false);
			if (Comp_AssistManager)
			{
				AssistManager.AssistManager.HandleAssistInventoryActions += AssistManger_OnKill;
			}
		}
		private void ClampConfig()
		{
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			MovementSpeed = Math.Max(0f, MovementSpeed);
			BaseCooldownReduction = Math.Max(0f, BaseCooldownReduction);
			if (!CoolPrimary && !CoolSecondary && !CoolUtility && !CoolSpecial)
			{
				
			}
			int total = 0;
			if (CoolPrimary)
            {
				total++;
            }
			if (CoolSecondary)
			{
				total++;
			}
			if (CoolUtility)
			{
				total++;
			}
			if (CoolSpecial)
			{
				total++;
			}
			if (total < 1)
			{
				DoesCool = false;
			}
			if (total > 1)
            {
				SingleCool = false;
			}
		}
		private void CreateBuff()
		{
			BuffDef refBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/MoveSpeedOnKill/bdKillMoveSpeed.asset").WaitForCompletion();
			HarpoonBuff = Utils.ContentManager.AddBuff("HunterBoost", refBuff.iconSprite, BuffColor, false, false, false, false, false);
			ReduceCooldownBuff = Utils.ContentManager.AddBuff("HunterSkillReduction", refBuff.iconSprite, BuffColor, true, false, false);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string cooldown_pickup = "";
			string speed_pickup = "";
			string cooldown_description = "";
			string speed_description = "";
			string duration_description = "";
			string extend_description = "";
			if (DoesCool)
			{
				if (!CoolPrimary || !CoolSecondary || !CoolUtility || !CoolSpecial)
				{
					cooldown_pickup = " reduces";
					cooldown_description = " <style=cIsUtility>reduces";
					List<string> skillStrings = new List<string>();
					if (CoolPrimary)
					{
						skillStrings.Add(" Primary");
					}
					if (CoolSecondary)
					{
						skillStrings.Add(" Secondary");
					}
					if (CoolUtility)
					{
						skillStrings.Add(" Utility");
					}
					if (CoolSpecial)
					{
						skillStrings.Add(" Special");
					}
					
					for (int i = 0; i < skillStrings.Count; i++)
					{
						if (i > 0)
						{
							if (i < skillStrings.Count - 1)
							{
								cooldown_pickup += ",";
								cooldown_description += ",";
							}
							else
							{
								cooldown_pickup += " and";
								cooldown_description += " and";
							}
						}
						cooldown_pickup += skillStrings[i];
						cooldown_description += skillStrings[i];
					}
					if (SingleCool)
                    {
						cooldown_pickup += " skill cooldown";
						cooldown_description += string.Format(" skill cooldown</style> by <style=cIsUtility>{0}s</style>", BaseCooldownReduction);
					}
					else
                    {
						cooldown_pickup += " skill cooldowns";
						cooldown_description += string.Format(" skill cooldowns</style> by <style=cIsUtility>{0}s</style>", BaseCooldownReduction);
					}
				}
				else
                {
					cooldown_pickup = " reduces all skill cooldowns";
					cooldown_description = string.Format(" <style=cIsUtility>reduces all skill cooldowns</style> by <style=cIsUtility>{0}s</style>", BaseCooldownReduction);
				}
			}
			if (MovementSpeed > 0f)
            {
				if (DoesCool)
                {
					speed_pickup = " and";
					speed_description = " and";
				}
				speed_pickup += " grants a temporary burst of movement speed";
				speed_description += string.Format(" increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style>", MovementSpeed * 100f);
			}
			if (BaseDuration > 0f)
			{
				if (StackDuration > 0f)
				{
					duration_description += string.Format(" for <style=cIsUtility>{0}s <style=cStack>(+{1}s per stack)</style></style>", BaseDuration, StackDuration);
				}
				else
				{
					duration_description += string.Format(" for <style=cIsUtility>{0}s</style>", BaseDuration);
				}
			}
			if (ExtendDuration)
            {
				extend_description = " <style=cIsUtility>Consecutive kills increase the duration</style>.";
			}
			string pickup = string.Format("Killing an enemy{0}{1}.", cooldown_pickup, speed_pickup);
			string desc = string.Format("Killing an enemy{0}{1}{2}.{3}", cooldown_description, speed_description, duration_description, extend_description);
			LanguageAPI.Add("ITEM_MOVESPEEDONKILL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_MOVESPEEDONKILL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			if (MovementSpeed != 0f)
            {
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			if (sender.HasBuff(HarpoonBuff))
			{
				args.moveSpeedMultAdd += MovementSpeed;
			}
			if (sender.HasBuff(ReduceCooldownBuff))
            {
				ReduceCooldowns(sender);
			}
		}
		private void ReduceCooldowns(CharacterBody body)
        {
			int buffcount = body.GetBuffCount(ReduceCooldownBuff);
			body.SetBuffCount(ReduceCooldownBuff.buffIndex, 0);
			if (!DoesCool)
            {
				return;
            }
			SkillLocator skillLocator = body.skillLocator;
			if (skillLocator)
			{
				
				float cooldown = BaseCooldownReduction * buffcount;
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
		private void AssistManger_OnKill(AssistManager.AssistManager.Assist assist, Inventory assistInventory, CharacterBody killerBody, DamageInfo damageInfo)
		{
			CharacterBody assistBody = assist.attackerBody;
			if (assistBody == killerBody)
			{
				return;
			}

			int itemCount = assistInventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
			if (itemCount > 0)
			{
				itemCount = Math.Max(0, itemCount - 1);
				float duration = BaseDuration + (itemCount * StackDuration);
				if (duration > 0f)
				{
					if (ExtendDuration)
					{
						Helpers.AddOrExtendBuff(assistBody, HarpoonBuff, duration);
					}
					else
					{
						assistBody.AddTimedBuff(HarpoonBuff, duration);
					}

					EffectData effectData = new EffectData();
					effectData.origin = assistBody.corePosition;
					effectData.rotation = assistBody.transform.rotation;
					CharacterMotor characterMotor = assistBody.characterMotor;
					if (characterMotor)
					{
						Vector3 moveDirection = characterMotor.moveDirection;
						if (moveDirection != Vector3.zero)
						{
							effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
						}
					}
					EffectManager.SpawnEffect(BoostVFX, effectData, true);
				}
				if (DoesCool)
				{
					assistBody.AddBuff(ReduceCooldownBuff);
				}
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			if (attackerBody.inventory)
			{
				int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
				if (itemCount > 0)
				{
					itemCount = Math.Max(0, itemCount - 1);
					float duration = BaseDuration + (itemCount * StackDuration);
					if (duration > 0f)
					{
						if (ExtendDuration)
                        {
							Helpers.AddOrExtendBuff(attackerBody, HarpoonBuff, duration);
						}
						else
                        {
							attackerBody.AddTimedBuff(HarpoonBuff, duration);
                        }
						
						EffectData effectData = new EffectData();
						effectData.origin = attackerBody.corePosition;
						effectData.rotation = attackerBody.transform.rotation;
						CharacterMotor characterMotor = attackerBody.characterMotor;
						if (characterMotor)
						{
							Vector3 moveDirection = characterMotor.moveDirection;
							if (moveDirection != Vector3.zero)
							{
								effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
							}
						}
						EffectManager.SpawnEffect(BoostVFX, effectData, true);
					}
					if (DoesCool)
                    {
						attackerBody.AddBuff(ReduceCooldownBuff);
					}
				}
			}
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC1Content.Items), "MoveSpeedOnKill"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnCharacterDeath - Hook failed");
			}
		}
	}
}
