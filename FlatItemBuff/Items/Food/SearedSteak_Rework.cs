using System;
using System.Linq;
using System.Collections.Generic;
using FlatItemBuff.Utils;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class SearedSteak_Rework
	{
		public static BuffDef SearedRegenBuff;
		private const string LogName = "Seared Steak Rework";
		internal static bool Enable = false;
		internal static float BasePercentHP = 0.05f;
		internal static bool NerfFakeKill = false;
		internal static float BaseRegen = 4f;
		internal static float StackRegen = 0f;
		internal static float BaseDuration = 3f;
		internal static float StackDuration = 3f;
		internal static int BaseCap = 1;
		internal static int StackCap = 1;
		internal static bool RefreshDuration = true;
		internal static bool Comp_AssistManager = true;
		public SearedSteak_Rework()
		{
			if (!Enable)
            {
				new SearedSteak();
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			CreateBuffs();
			UpdateItemDef();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			Hooks();
			if (MainPlugin.AssistManager_Loaded)
			{
				ApplyAssistManager();
			}
		}
		private void ApplyAssistManager()
		{
			if (Comp_AssistManager)
			{
				AssistManager.AssistManager.HandleAssistInventoryActions += AssistManger_OnKill;
			}
		}
		private void ClampConfig()
		{
			BaseRegen = Math.Max(0f, BaseRegen);
			StackRegen = Math.Max(0f, StackRegen);
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			BaseCap = Math.Max(0, BaseCap);
			StackCap = Math.Max(0, StackCap);
			BasePercentHP = Math.Max(0f, BasePercentHP);
		}
		private void CreateBuffs()
		{
			//"RoR2/Junk/Common/bdMeatRegenBoost.asset"
			BuffDef MeatRegen = Addressables.LoadAssetAsync<BuffDef>("1c9bc2e1186d394429928b51b132114f").WaitForCompletion();
			//"RoR2/Base/AttackSpeedOnCrit/bdAttackSpeedOnCrit.asset"
			BuffDef AttackSpeedOncrit = Addressables.LoadAssetAsync<BuffDef>("aabb5fce0f91514429bfa91cb2f790da").WaitForCompletion();
			SearedRegenBuff = Utils.ContentManager.AddBuff("SearedRegen", MeatRegen.iconSprite, AttackSpeedOncrit.buffColor, true, false, false, false, false);
		}
		private void UpdateItemDef()
		{
			//"RoR2/Base/FlatHealth/FlatHealth.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("8834db4eda23c6c42bb4e2abcd94c966").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.AIBlacklist);
				itemTags.Add(ItemTag.OnKillEffect);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void UpdateText()
		{
			string pickup = "";
			string desc = "";
			pickup += "Regenerate health after killing an enemy. Cooked to perfection.";
			if (StackRegen > 0f)
			{
				desc += string.Format("Killing an enemy Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+{0} hp/s <style=cStack>(+{1} hp/s per stack)</style></style>", BaseRegen, StackRegen);
			}
			else
			{
				desc += string.Format("Killing an enemy Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+{0} hp/s</style>", BaseRegen);
			}
			if (BaseCap > 0)
			{
				if (StackCap > 0)
				{
					desc += string.Format(" up to <style=cIsUtility>{0} <style=cStack>(+{1} per stack)</style></style> times, ", BaseCap, StackCap);
				}
				else
				{
					desc += string.Format(" up to <style=cIsUtility>{0}</style> times, ", BaseCap);
				}
			}
			if (StackDuration > 0f)
            {
				desc += string.Format(" for <style=cIsUtility>{0}s <style=cStack>(+{1}s per stack)</style></style>.", BaseDuration, StackDuration);
			}
			else
            {
				desc += string.Format(" for <style=cIsUtility>{0}s</style> after killing an enemy.", BaseDuration);
			}
			if (RefreshDuration)
            {
				desc += string.Format(" Kills also refresh the duration of all stacks.");
			}
			if (BasePercentHP > 0f)
			{
				if (desc.Length > 0)
                {
					desc += " ";
                }
				desc += string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}% <style=cStack>(+{0}% per stack)</style></style>.", BasePercentHP * 100f);
			}
			LanguageAPI.Add("ITEM_COOKEDSTEAK_PICKUP", pickup);
			LanguageAPI.Add("ITEM_COOKEDSTEAK_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			if (BaseDuration > 0f || StackDuration > 0f)
            {
				SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			}
			if (BaseRegen > 0f || StackRegen > 0f)
            {
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int buffCount = sender.GetBuffCount(SearedRegenBuff);
			if (buffCount > 0)
			{
				int itemCount = 0;
				if (sender.inventory)
                {
					itemCount = Math.Max(0, sender.inventory.GetItemCountEffective(DLC3Content.Items.CookedSteak) - 1);
				}
				float itemRegen = BaseRegen + (itemCount * StackRegen);
				float levelRegen = 1f + (sender.level - 1f) * 0.2f;
				args.baseRegenAdd += itemRegen * levelRegen * buffCount;
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			if (attackerBody.inventory)
			{
				int itemCount = attackerBody.inventory.GetItemCountEffective(DLC3Content.Items.CookedSteak);
				if (itemCount > 0)
				{
					float duration = BaseDuration + (Math.Max(0, itemCount - 1) * StackDuration);
					int maxStacks = BaseCap;
					if (maxStacks > 0)
					{
						maxStacks += StackCap * Math.Max(0, itemCount - 1);
					}
					bool refresh = RefreshDuration;
					if (NerfFakeKill && (!damageReport.victimMaster || !damageReport.victimBody))
					{
						refresh = false;
					}
					if (refresh)
					{
						Helpers.RefreshBuffDuration(attackerBody, SearedRegenBuff, duration);
					}
					if (maxStacks < 1 || attackerBody.GetBuffCount(SearedRegenBuff) < maxStacks)
					{
						attackerBody.AddTimedBuff(SearedRegenBuff, duration);
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

			int itemCount = assistInventory.GetItemCountEffective(DLC3Content.Items.CookedSteak);
			if (itemCount > 0)
			{
				float duration = BaseDuration + (Math.Max(0, itemCount - 1) * StackDuration);
				int maxStacks = BaseCap;
				if (maxStacks > 0)
				{
					maxStacks += StackCap * Math.Max(0, itemCount - 1);
				}
				bool refresh = RefreshDuration;
				if (NerfFakeKill && (!assist.victimBody || !assist.victimBody.master))
				{
					refresh = false;
				}
				if (refresh)
				{
					Helpers.RefreshBuffDuration(assistBody, SearedRegenBuff, duration);
				}
				if (maxStacks < 1 || assistBody.GetBuffCount(SearedRegenBuff) < maxStacks)
				{
					assistBody.AddTimedBuff(SearedRegenBuff, duration);
				}
			}
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(93),
				x => x.MatchLdloc(57),
				x => x.MatchConvR4()
			))
			{
				ilcursor.Index += 3;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((body) =>
				{
					return BasePercentHP;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats A - Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(91),
				x => x.MatchLdloc(57),
				x => x.MatchConvR4()
			))
			{
				ilcursor.RemoveRange(9);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats B - Hook failed");
			}
		}
	}
}
