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
		public static BuffDef ChronoDebuff;
		private static Color BuffColor = new Color(0.678f, 0.612f, 0.412f, 1f);
		private const string LogName = "Chronobauble";
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
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			CreateBuff();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
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
			//"RoR2/Base/SlowOnHit/bdSlow60.asset"
			ChronoDebuff = Utils.ContentManager.AddBuff("ChronoDebuff", Addressables.LoadAssetAsync<BuffDef>("6a0d533aa91d36340bcea5a2f90b1a78").WaitForCompletion().iconSprite, BuffColor, false, true, false);
		}
		private void UpdateText()
		{
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
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += new ILContext.Manipulator(IL_VisualEffects);
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
						int itemCount = inventory.GetItemCountEffective(RoR2Content.Items.SlowOnHit);
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
		private float SlowDuration(float itemCount)
        {
			itemCount = Math.Max(0, itemCount - 1);
			return BaseDuration + (StackDuration * itemCount);
		}
		private void IL_VisualEffects(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Slow60")
			))
			{
				ilcursor.RemoveRange(2);
				ilcursor.EmitDelegate<Func<CharacterBody, bool>>((self) =>
				{
					return self.HasBuff(RoR2Content.Buffs.Slow60) || self.HasBuff(ChronoDebuff);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_VisualEffect - Hook failed");
			}
		}
		
		private void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if(ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "SlowOnHit")
			))
            {
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnHitEnemy - Hook failed");
			}
		}
	}
}
