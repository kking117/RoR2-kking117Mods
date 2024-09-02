using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class BisonSteak_Rework
	{
		public static BuffDef FreshRegenBuff;
		internal static bool Enable = true;
		internal static bool NerfFakeKill = false;
		internal static float ExtendDuration = 1f;
		internal static float BaseRegen = 1f;
		internal static float StackRegen = 0f;
		internal static float BaseDuration = 3f;
		internal static float StackDuration = 3f;
		public BisonSteak_Rework()
		{
			if (!Enable)
            {
				new BisonSteak();
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Bison Steak");
			ClampConfig();
			CreateBuffs();
			UpdateItemDef();
			UpdateText();
			Hooks();
		}
		
		private void ClampConfig()
		{
			BaseRegen = Math.Max(0f, BaseRegen);
			StackRegen = Math.Max(0f, StackRegen);
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			ExtendDuration = Math.Max(0f, ExtendDuration);
		}
		private void CreateBuffs()
		{
			BuffDef MeatRegen = Addressables.LoadAssetAsync<BuffDef>("RoR2/Junk/Common/bdMeatRegenBoost.asset").WaitForCompletion();
			FreshRegenBuff = Utils.ContentManager.AddBuff("MeatRegen", MeatRegen.iconSprite, MeatRegen.buffColor, true, false, false);
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/FlatHealth/FlatHealth.asset").WaitForCompletion();
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
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "";
			string desc = "";
			pickup += "Regenerate health after killing an enemy.";
			if (StackRegen > 0f)
			{
				desc += string.Format("Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+{0} hp/s <style=cStack>(+{1} hp/s per stack)</style></style>", BaseRegen, StackRegen);
			}
			else
			{
				desc += string.Format("Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+{0} hp/s</style>", BaseRegen);
			}
			if (StackDuration > 0f)
            {
				desc += string.Format(" for <style=cIsUtility>{0}s <style=cStack>(+{1}s per stack)</style></style> after killing an enemy.", BaseDuration, StackDuration);
			}
			else
            {
				desc += string.Format(" for <style=cIsUtility>{0}s</style> after killing an enemy.", BaseDuration);
			}
			if (ExtendDuration > 0f)
            {
				desc += string.Format(" Consecutive kills extend the duration by <style=cIsUtility>{0}s</style>.", ExtendDuration);
			}
			LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", pickup);
			LanguageAPI.Add("ITEM_FLATHEALTH_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
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
			int buffCount = sender.GetBuffCount(FreshRegenBuff);
			if (buffCount > 0)
			{
				int itemCount = 0;
				if (sender.inventory)
                {
					itemCount = Math.Max(0, sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth) - 1);
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
				int itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
				if (itemCount > 0)
				{
					if (NerfFakeKill)
                    {
						if (damageReport.victimMaster && damageReport.victimBody)
                        {
							Utils.Helpers.Add_ExtendBuffDuration(attackerBody, FreshRegenBuff, ExtendDuration);
						}
					}
					else
                    {
						Utils.Helpers.Add_ExtendBuffDuration(attackerBody, FreshRegenBuff, ExtendDuration);
					}
					float duration = BaseDuration + (Math.Max(0, itemCount - 1) * StackDuration);
					if (duration > 0f)
					{
						attackerBody.AddTimedBuff(FreshRegenBuff, duration);
					}
				}
			}
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "FlatHealth"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Bison Steak Rework - Effect Override - IL Hook failed");
			}
		}
	}
}
