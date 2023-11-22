using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class Chronobauble
	{
		public static BuffDef ChronoDebuff = RoR2Content.Buffs.Slow60;
		private static Color BuffColor = new Color(0.678f, 0.612f, 0.412f, 1f);
		internal static bool Enable = false;
		internal static float SlowDown = 0.6f;
		internal static float AttackDown = 0.3f;
		internal static float BaseDuration = 2f;
		internal static float StackDuration = 2f;
		public Chronobauble()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Chronobauble");
			ClampConfig();
			CreateBuff();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			SlowDown = Math.Max(0f, SlowDown);
			AttackDown = Math.Max(0f, AttackDown);
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
		}
		private void CreateBuff()
		{
			ChronoDebuff = Modules.Buffs.AddNewBuff("ChronoDebuff", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SlowOnHit/bdSlow60.asset").WaitForCompletion().iconSprite, BuffColor, false, true, false);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			//string pickup = "";
			string desc = "";
			string slowText = "";
			string durText = "";
			if(SlowDown > 0f)
            {
				slowText = string.Format(" for <style=cIsUtility>-{0}% movement speed</style>", SlowDown * 100f);
			}
			if(AttackDown > 0f)
            {
				if (SlowDown > 0f)
                {
					slowText += string.Format(" and <style=cIsDamage>-{0}% attack speed</style>", AttackDown * 100f);
				}
				else
                {
					slowText = string.Format(" for <style=cIsDamage>-{0}% attack speed</style>", AttackDown * 100f);
				}
			}
			durText = string.Format(" for <style=cIsUtility>{0}s</style>", BaseDuration);
			if (StackDuration > 0f)
            {
				durText += string.Format(" <style=cStack>(+{0}s per stack)</style>", StackDuration);
			}
			desc = string.Format("<style=cIsUtility>Slow</style> enemies on hit{0}{1}.", slowText, durText);
			//LanguageAPI.Add("ITEM_SLOWONHIT_PICKUP", pickup);
			LanguageAPI.Add("ITEM_SLOWONHIT_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			float procrate = damageReport.damageInfo.procCoefficient;
			
			if (procrate > 0f && damageReport.victim)
			{
				CharacterBody victimBody = damageReport.victim.body;
				if (victimBody)
				{
					Inventory inventory = damageReport.attackerBody.inventory;
					if (inventory)
					{
						int itemCount = inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
						if (itemCount > 0)
						{
							if (damageReport.victim)
							{
								victimBody.AddTimedBuff(ChronoDebuff, SlowDuration(itemCount));
							}
						}
					}
				}
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int buffCount = sender.GetBuffCount(ChronoDebuff);
			if (buffCount > 0)
			{
				args.attackSpeedReductionMultAdd += AttackDown;
				args.moveSpeedReductionMultAdd += SlowDown;
			}
		}
		private static void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, CharacterBody self)
		{
			orig(self);
			if (self.HasBuff(ChronoDebuff) && !self.HasBuff(RoR2Content.Buffs.Slow60))
			{
				self.UpdateSingleTemporaryVisualEffect(ref self.slowDownTimeTempEffectInstance, CharacterBody.AssetReferences.slowDownTimeTempEffectPrefab, self.radius, true, "");
			}
		}
		private float SlowDuration(float itemCount)
        {
			itemCount = Math.Max(0, itemCount - 1);
			return BaseDuration + (StackDuration * itemCount);
		}
		private void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "SlowOnHit")
			);
			ilcursor.Index += 2;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
		}
	}
}
