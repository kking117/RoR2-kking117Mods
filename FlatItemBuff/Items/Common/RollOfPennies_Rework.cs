using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;

namespace FlatItemBuff.Items
{
	public class RollOfPennies_Rework
	{
		private const string LogName = "Roll of Pennies Rework";
		public static BuffDef PennyArmorBuff;
		private static Color BuffColor = new Color(1f, 0.788f, 0.055f, 1f);
		internal static bool Enable = false;
		internal static float BaseGold = 3f;
		internal static float StackGold = 0f;
		internal static float BaseArmor = 5f;
		internal static float StackArmor = 0f;
		internal static float BaseDuration = 2f;
		internal static float StackDuration = 2f;
		internal static float GoldDuration = 0.5f;
		internal static int VFXAmount = 10;
		public RollOfPennies_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			if (BaseArmor > 0f || StackArmor > 0f)
			{
				CreateBuff();
			}
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			Hooks();
		}
		private void ClampConfig()
		{
			BaseGold = Math.Max(0f, BaseGold);
			StackGold = Math.Max(0f, StackGold);
			BaseArmor = Math.Max(0f, BaseArmor);
			StackArmor = Math.Max(0f, StackArmor);
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			GoldDuration = Math.Max(0f, GoldDuration);
			VFXAmount = Math.Max(0, VFXAmount);
		}
		private void CreateBuff()
		{
			//"RoR2/Base/Grandparent/bdOverheat.asset"
			PennyArmorBuff = Utils.ContentManager.AddBuff("Penny Armor", Addressables.LoadAssetAsync<BuffDef>("63aa6c6bd0ad3e944b87f6546d0cb7e4").WaitForCompletion().iconSprite, BuffColor, true, false, false, false, false);
		}
		private void UpdateText()
		{
			string pickup = "";
			string description = "";
			string goldPick = "";
			string armorPick = "";
			string goldText = "";
			string armorText = "";
			if (BaseGold > 0f || StackGold > 0f)
			{
				if (StackGold > 0f)
                {
					goldText = string.Format("Gain <style=cIsUtility>{0} <style=cStack>(+{0} per stack)</style> gold</style> when <style=cIsDamage>hit</style>, <style=cIsUtility>scaling with time</style>.", BaseGold, StackGold);
				}
				else
                {
					goldText = string.Format("Gain <style=cIsUtility>{0} gold</style> when <style=cIsDamage>hit</style>, <style=cIsUtility>scaling with time</style>.", BaseGold);
				}
				goldPick = "Gain gold when hit.";
				if (BaseArmor > 0f || StackArmor > 0f)
                {
					goldText += " ";
					goldPick += " ";
                }
			}
			if (BaseArmor > 0f || StackArmor > 0f)
			{
				string temparmorText;
				if (StackArmor > 0f)
				{
					temparmorText = string.Format(" gain <style=cIsHealing>{0} <style=cStack>(+{1} per stack)</style> armor</style>", BaseArmor, StackArmor);
				}
				else
                {
					temparmorText = string.Format(" gain <style=cIsHealing>{0} armor</style>", BaseArmor);
				}
				string durationText = " with duration based on the <style=cIsUtility>gold's value</style>";
				if (BaseDuration > 0 || StackDuration > 0)
				{
					if (StackDuration > 0)
					{
						durationText = string.Format(" for <style=cIsUtility>{0}s <style=cStack>(+{1}s per stack)</style></style> plus the <style=cIsUtility>gold's value</style>", BaseDuration, StackDuration);
					}
					else
                    {
						durationText = string.Format(" for <style=cIsUtility>{0}s</style> plus the <style=cIsUtility>gold's value</style>", BaseDuration);
					}
				}
				armorText += string.Format("When <style=cIsUtility>collecting gold</style>{0}{1}.", temparmorText, durationText);
				armorPick += "Gain temporary armor when collecting gold.";
			}
			pickup = string.Format("{0}{1}", goldPick, armorPick);
			description = string.Format("{0}{1}", goldText, armorText);
			LanguageAPI.Add("ITEM_GOLDONHURT_PICKUP", pickup);
			LanguageAPI.Add("ITEM_GOLDONHURT_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_OnTakeDamage);
			if (BaseArmor > 0f || StackArmor > 0f)
            {
				On.RoR2.Stats.StatManager.OnGoldCollected += OnGoldCollected;
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
			if (BaseGold > 0f || StackGold > 0f)
            {
				SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			}
			if (VFXAmount > 0)
            {
				IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += new ILContext.Manipulator(IL_VisualEffects);
			}
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			float procRate = damageReport.damageInfo.procCoefficient;
			CharacterBody victimMaster = damageReport.victimBody;
			CharacterBody victimBody = damageReport.victimBody;
			if (procRate > 0f)
			{
				if (victimBody && victimMaster)
                {
					Inventory inventory = victimMaster.inventory;
					if (inventory)
					{
						int itemCount = inventory.GetItemCountEffective(DLC1Content.Items.GoldOnHurt);
						if (itemCount > 0)
                        {
							uint gold = (uint)GetGoldFromHit(itemCount, procRate);
							if (gold > 0)
                            {
								GoldOrb goldOrb = new GoldOrb();
								goldOrb.origin = damageReport.damageInfo.position;
								goldOrb.target = victimBody.mainHurtBox;
								goldOrb.goldAmount = gold;
								OrbManager.instance.AddOrb(goldOrb);
								EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.gainCoinsImpactEffectPrefab, damageReport.damageInfo.position, Vector3.up, true);
							}
						}
					}
				}
			}
		}
		private void OnGoldCollected(On.RoR2.Stats.StatManager.orig_OnGoldCollected orig, CharacterMaster master, ulong amount)
        {
			orig(master, amount);
			if (amount > 0)
			{
				int itemCount = master.inventory.GetItemCountEffective(DLC1Content.Items.GoldOnHurt);
				if (itemCount > 0)
				{
					CharacterBody characterBody = master.GetBody();
					if (characterBody && characterBody.healthComponent)
					{
						float buffDuration = GetBuffDurationFromGold(itemCount, (int)amount);
						if (buffDuration > 0f)
                        {
							characterBody.AddTimedBuff(PennyArmorBuff, buffDuration);
                        }
					}
				}
			}
        }
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int buffCount = sender.GetBuffCount(PennyArmorBuff.buffIndex);
			if (buffCount > 0)
			{
				int itemCount = 0;
				if (sender.inventory)
                {
					itemCount = Math.Max(0, sender.inventory.GetItemCountEffective(DLC1Content.Items.GoldOnHurt) - 1);
                }
				args.armorAdd += buffCount * (BaseArmor + (StackArmor * itemCount));
			}
		}
		private void IL_OnTakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "goldOnHurt")
			))
			{
				ilcursor.Index += 1;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
            {
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnTakeDamage - Hook failed");
			}
		}
		private void IL_VisualEffects(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "AurelioniteBlessing")
			))
			{
				ilcursor.RemoveRange(2);
				ilcursor.EmitDelegate<Func<CharacterBody, bool>>((self) =>
				{
					return self.HasBuff(DLC2Content.Buffs.AurelioniteBlessing) || self.GetBuffCount(PennyArmorBuff) >= VFXAmount;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_VisualEffects - Hook failed");
			}
		}
		private float GetGoldFromHit(int itemCount, float procRate)
		{
			itemCount = Math.Max(0, itemCount);
			float gold = BaseGold + (StackGold * itemCount);
			gold *= procRate;
			gold *= Run.instance.difficultyCoefficient;
			return Math.Max(1f, gold);
		}
		private float GetBuffDurationFromGold(int itemCount, int gold)
		{
			itemCount = Math.Max(0, itemCount-1);
			float duration = BaseDuration + (StackDuration * itemCount);
			if (GoldDuration > 0f)
            {
				float goldValue = gold * GoldDuration / Run.instance.difficultyCoefficient;
				duration += (float)Math.Round(goldValue, 1);
			}
			return duration;
		}
	}
}
