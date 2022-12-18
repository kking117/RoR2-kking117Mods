using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RiskyMod.Items.Common;

namespace FlatItemBuff.ItemChanges
{
	public class LigmaLenses
	{
		public static BuffDef SeerReady = RoR2Content.Buffs.FullCrit;
		public static BuffDef SeerCooldown = RoR2Content.Buffs.FullCrit;
		private static float BaseDamage = 0.2f;
		private static float StackDamage = 0.2f;
		private static float BaseRadius = 25f;
		private static float StackRadius = 0f;
		private static int Cooldown = 10;
		private static float ProcRate = 0f;
		private static float TriggerThresh = 4f;

		public LigmaLenses()
		{
			MainPlugin.ModLogger.LogInfo("Changing Lost Seer's Lenses");
			SetupConfigValues();
			if (Cooldown > 0)
			{
				CreateBuffs();
			}
			UpdateText();
			Hooks();
		}
		private void SetupConfigValues()
        {
			BaseDamage = MainPlugin.LigmaLenses_BaseDamage.Value;
			StackDamage = MainPlugin.LigmaLenses_StackDamage.Value;
			BaseRadius = MainPlugin.LigmaLenses_BaseRadius.Value;
			StackRadius = MainPlugin.LigmaLenses_StackRadius.Value;
			Cooldown = MainPlugin.LigmaLenses_Cooldown.Value;
			ProcRate = MainPlugin.LigmaLenses_ProcRate.Value;
			TriggerThresh = MainPlugin.LigmaLenses_TriggerThresh.Value;
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string Imagine = "Hits";
			string pickupImagine = "Hits";
			if (TriggerThresh > 0)
            {
				Imagine = string.Format("Hits that deal <style=cIsDamage>more than {0}% damage</style>",TriggerThresh * 100f);
				pickupImagine = "High damage hits";
			}
			string stackA = "";
			if (StackRadius != 0)
            {
				stackA = string.Format(" <style=cStack>(+{0}m per stack)</style>", StackRadius);
			}
			string stackB = "";
			if (StackDamage != 0)
			{
				stackB = string.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 100f);
			}
			string Recharge = "";
			string pickupRecharge = "";
			if (Cooldown > 0)
            {
				Recharge = string.Format(" Recharges every <style=cIsUtility>{0}</style> seconds.", Cooldown);
				pickupRecharge = " Recharges over time.";

			}
			string pickup = string.Format("{0} also release a chaining seeker that 'Critically Strikes'.{1} <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.", pickupImagine, pickupRecharge);
			string desc = string.Format("{0} also release a <style=cIsDamage>seeker</style> that <style=cIsDamage>chains between enemies</style> in a <style=cIsDamage>{1}m</style>{2} radius, dealing <style=cIsDamage>{3}%</style>{4} TOTAL damage that <style=cIsDamage>critically strikes</style>.{5} <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.", Imagine, BaseRadius, stackA, BaseDamage * 100f, stackB, Recharge);
			LanguageAPI.Add("ITEM_CRITGLASSESVOID_PICKUP", pickup);
			LanguageAPI.Add("ITEM_CRITGLASSESVOID_DESC", desc);
		}
		private void CreateBuffs()
		{
			SeerReady = Modules.Buffs.AddNewBuff("Seer Ready", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite, new Color(0.705f, 0.313f, 0.784f, 1f), false, false, false);
			SeerCooldown = Modules.Buffs.AddNewBuff("Seer Cooldown", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite, new Color(0.313f, 0.313f, 0.313f, 1f), true, false, true);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
			if (Cooldown > 0)
			{
				RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			}
			On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
				if (itemCount > 0)
				{
					if (!sender.HasBuff(SeerCooldown) && !sender.HasBuff(SeerReady))
                    {
						sender.AddBuff(SeerReady);
					}
				}
			}
		}
		private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
			if (NetworkServer.active)
			{
				if (!damageInfo.rejected && damageInfo.procCoefficient > 0)
				{
					if (!damageInfo.procChainMask.HasProc(ProcType.Rings))
					{
						if (damageInfo.attacker)
						{
							CharacterBody victimBody = victim.GetComponent<CharacterBody>();
							CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
							if (victimBody && attackerBody && attackerBody.inventory)
							{
								if (HitDamageThresh(damageInfo, victim, attackerBody))
								{
									if (Cooldown > 0)
									{
										if (attackerBody.HasBuff(SeerReady))
										{
											int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
											if (itemCount > 0)
											{
												ApplyCooldown(attackerBody);
												SeerAOE(damageInfo, attackerBody, victimBody, itemCount);
											}
										}
									}
									else
                                    {
										int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
										if (itemCount > 0)
										{
											SeerAOE(damageInfo, attackerBody, victimBody, itemCount);
										}
									}
								}
							}
						}
					}
				}
			}
			orig(self, damageInfo, victim);
		}
		private void ApplyCooldown(CharacterBody characterBody)
        {
			int i = 0;
			while (i < Cooldown)
			{
				i++;
				characterBody.AddTimedBuff(SeerCooldown, (float)i);
			}
			characterBody.RemoveBuff(SeerReady);
		}
		private bool HitDamageThresh(DamageInfo damageInfo, GameObject victim, CharacterBody attackerBody)
        {
			float threshHold = TriggerThresh;
			if (MainPlugin.RiskyModLoaded && attackerBody.inventory)
            {
				if (RiskyModCompat.IsEnabled())
				{
					CharacterBody victimBody = victim.GetComponent<CharacterBody>();
					if (victimBody)
                    {
						HealthComponent hpComp = victimBody.healthComponent;
						if (hpComp)
                        {
							if (DamageAPI.HasModdedDamageType(damageInfo, RiskyModCompat.CustomDamageType()))
                            {
								int bobars = attackerBody.inventory.GetItemCount(RoR2Content.Items.Crowbar);
								threshHold *= RiskyModCompat.GetDamageBoost(bobars);
							}
						}
					}
				}
			}
			return damageInfo.damage / attackerBody.damage >= threshHold;
        }
		private void SeerAOE(DamageInfo damageInfo, CharacterBody attackerBody, CharacterBody victimBody, int itemCount)
		{
			TeamIndex teamIndex = attackerBody.master.teamIndex;
			float damageCo = BaseDamage + (StackDamage * (itemCount - 1));
			float totalDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCo);
			float searchDist = BaseRadius + (StackRadius * (itemCount - 1));
			ProcChainMask procChain = damageInfo.procChainMask;
			procChain.AddProc(ProcType.Rings);

			Components.SeerSeeker Seerker = new Components.SeerSeeker();
			Seerker.origin = damageInfo.position;
			Seerker.damageValue = totalDamage;
			Seerker.isCrit = true;
			Seerker.teamIndex = teamIndex;
			Seerker.attacker = damageInfo.attacker;
			Seerker.procChainMask = procChain;
			Seerker.procCoefficient = ProcRate;
			Seerker.damageColorIndex = DamageColorIndex.Void;
			Seerker.damageType = damageInfo.damageType;
			Seerker.SearchDistance = searchDist;
			Seerker.strikeTime = 0.2f;
			Seerker.isFirst = true;
			HurtBox mainHurtBox2 = victimBody.mainHurtBox;
			if (mainHurtBox2)
			{
				Seerker.target = mainHurtBox2;
				OrbManager.instance.AddOrb(Seerker);
			}
		}
		private void IL_TakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(DLC1Content.Items), "CritGlassesVoid")
			);
			ilcursor.GotoNext(
				x => x.MatchLdcR4(0.5f)
			);
			ilcursor.Next.Operand = 0f;
		}

		public static class RiskyModCompat
        {
			[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
			public static bool IsEnabled()
			{
				return Crowbar.enabled;
			}

			public static DamageAPI.ModdedDamageType CustomDamageType()
			{
				return Crowbar.CrowbarDamage;
			}

			public static float GetDamageBoost(int itemCount)
			{
				return Crowbar.GetCrowbarMult(itemCount);
			}
		}
	}
}
