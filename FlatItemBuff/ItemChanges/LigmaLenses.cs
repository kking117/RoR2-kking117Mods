using System;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class LigmaLenses
	{
		public static BuffDef SeerReady = RoR2Content.Buffs.FullCrit;
		public static BuffDef SeerCooldown = RoR2Content.Buffs.FullCrit;
		private static float BaseDamage = 0.2f;
		private static float StackDamage = 0.2f;
		private static float BaseRadius = 16f;
		private static float StackRadius = 0f;
		private static int Cooldown = 10;
		private static float ProcRate = 0f;
		private static float TriggerThresh = 4f;

		internal static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Lost Seer's Lenses");
			SetupConfigValues();
			CreateBuffs();
			UpdateText();
			Hooks();
		}
		private static void SetupConfigValues()
        {
			BaseDamage = MainPlugin.LigmaLenses_BaseDamage.Value;
			StackDamage = MainPlugin.LigmaLenses_StackDamage.Value;
			BaseRadius = MainPlugin.LigmaLenses_BaseRadius.Value;
			StackRadius = MainPlugin.LigmaLenses_StackRadius.Value;
			Cooldown = MainPlugin.LigmaLenses_Cooldown.Value;
			ProcRate = MainPlugin.LigmaLenses_ProcRate.Value;
			TriggerThresh = MainPlugin.LigmaLenses_TriggerThresh.Value;
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = " also releases seekers that 'Critically Strike'. Recharges over time. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
			if (TriggerThresh > 0)
            {
				pickup = "High damage hits" + pickup;

			}
			else
            {
				pickup = "Hits" + pickup;
			}
			string Imagine = "Hits";
			if (TriggerThresh > 0)
            {
				Imagine = string.Format("Hits that deal <style=cIsDamage>more than {0}% damage</style>",TriggerThresh * 100f);
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
			string desc = string.Format("{0} also releases <style=cIsDamage>seekers</style> in a <style=cIsDamage>{1}m</style>{2} radius, dealing <style=cIsDamage>{3}%</style>{4} TOTAL damage that <style=cIsDamage>critically strikes</style>. Recharges every <style=cIsUtility>{5}</style> seconds. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.", Imagine, BaseRadius, stackA, BaseDamage * 100f, stackB, Cooldown);
			LanguageAPI.Add("ITEM_CRITGLASSESVOID_PICKUP", pickup);
			LanguageAPI.Add("ITEM_CRITGLASSESVOID_DESC", desc);
		}
		private static void CreateBuffs()
		{
			SeerReady = Modules.Buffs.AddNewBuff("Seer Ready", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite, new Color(0.705f, 0.313f, 0.784f, 1f), false, false, false);
			SeerCooldown = Modules.Buffs.AddNewBuff("Seer Cooldown", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite, new Color(0.313f, 0.313f, 0.313f, 1f), false, false, true);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
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
		private static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
			if (NetworkServer.active)
			{
				if (!damageInfo.rejected && damageInfo.procCoefficient > 0)
				{
					if (damageInfo.attacker)
					{
						CharacterBody victimBody = victim.GetComponent<CharacterBody>();
						CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
						if (victimBody && attackerBody && attackerBody.inventory)
						{
							if (damageInfo.damage / attackerBody.damage >= TriggerThresh)
							{
								if (attackerBody.HasBuff(SeerReady))
								{
									int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
									if (itemCount > 0)
									{
										int i = 0;
										while (i < Cooldown)
										{
											attackerBody.AddTimedBuff(SeerCooldown, i * 1f);
											i++;
										}
										attackerBody.RemoveBuff(SeerReady);
										Wreck(damageInfo, attackerBody, victimBody, itemCount);
									}
								}
							}
						}
					}
				}
			}
			orig(self, damageInfo, victim);
		}
		private static void Wreck(DamageInfo damageInfo, CharacterBody attackerBody, CharacterBody victimBody, int itemCount)
        {
			TeamIndex teamIndex = attackerBody.master.teamIndex;
			float damageCo = BaseDamage + (StackDamage * (itemCount - 1));
			float totalDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCo);
			float searchDist = BaseRadius + (StackRadius * (itemCount - 1));
			ProcChainMask procChain = damageInfo.procChainMask;
			procChain.AddProc(ProcType.Rings);

			BullseyeSearch search = new BullseyeSearch();
			search.viewer = attackerBody;
			search.teamMaskFilter = TeamMask.allButNeutral;
			search.teamMaskFilter.RemoveTeam(teamIndex);
			search.sortMode = BullseyeSearch.SortMode.Distance;
			search.minDistanceFilter = 0f;
			search.maxDistanceFilter = searchDist;
			search.searchOrigin = damageInfo.position;
			search.searchDirection = victimBody.inputBank.aimDirection;
			search.maxAngleFilter = 180f;
			search.filterByLoS = false;
			search.RefreshCandidates();
			foreach (HurtBox target in search.GetResults())
			{
				HealthComponent hpcomp = target.healthComponent;
				if (hpcomp)
				{
					CharacterBody targetBody = hpcomp.body;
					if (targetBody && targetBody != victimBody)
					{
						VoidLightningOrb voidLightningOrb = new VoidLightningOrb();
						voidLightningOrb.origin = damageInfo.position;
						voidLightningOrb.damageValue = totalDamage;
						voidLightningOrb.isCrit = true;
						voidLightningOrb.totalStrikes = 1;
						voidLightningOrb.teamIndex = teamIndex;
						voidLightningOrb.attacker = damageInfo.attacker;
						voidLightningOrb.procChainMask = procChain;
						voidLightningOrb.procCoefficient = ProcRate;
						voidLightningOrb.damageColorIndex = DamageColorIndex.Void;
						voidLightningOrb.secondsPerStrike = 0.1f;
						HurtBox mainHurtBox = targetBody.mainHurtBox;
						if (mainHurtBox)
						{
							voidLightningOrb.target = mainHurtBox;
							OrbManager.instance.AddOrb(voidLightningOrb);
						}
					}
				}
			}

			VoidLightningOrb voidLightningOrbSelf = new VoidLightningOrb();
			voidLightningOrbSelf.origin = damageInfo.position;
			voidLightningOrbSelf.damageValue = totalDamage;
			voidLightningOrbSelf.isCrit = true;
			voidLightningOrbSelf.totalStrikes = 1;
			voidLightningOrbSelf.teamIndex = teamIndex;
			voidLightningOrbSelf.attacker = damageInfo.attacker;
			voidLightningOrbSelf.procChainMask = procChain;
			voidLightningOrbSelf.procCoefficient = ProcRate;
			voidLightningOrbSelf.damageColorIndex = DamageColorIndex.Void;
			voidLightningOrbSelf.secondsPerStrike = 0.1f;
			HurtBox mainHurtBox2 = victimBody.mainHurtBox;
			if (mainHurtBox2)
			{
				voidLightningOrbSelf.target = mainHurtBox2;
				OrbManager.instance.AddOrb(voidLightningOrbSelf);
			}
		}
		private static void IL_TakeDamage(ILContext il)
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
	}
}
