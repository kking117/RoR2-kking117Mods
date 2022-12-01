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
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "'Critical Strikes' also release void seekers to nearby enemies. Recharges over time. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.";
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
			string desc = string.Format("Your next hit will be a <style=cIsDamage>Critical Strike</style> that releases <style=cIsDamage>void seekers</style> in a <style=cIsDamage>{0}m</style>{1} radius, dealing <style=cIsDamage>{2}%</style>{3} TOTAL damage. Recharges every <style=cIsUtility>{4}</style> seconds. <style=cIsVoid>Corrupts all Lens-Maker's Glasses</style>.", BaseRadius, stackA, BaseDamage * 100f, stackB, Cooldown);
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
			On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
			GlobalEventManager.onServerDamageDealt += Global_DamageDealt;
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
		private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
			if (!NetworkServer.active)
			{
				return;
			}
			if (!damageInfo.crit)
			{
				if (damageInfo.attacker && damageInfo.procCoefficient > 0f)
				{
					CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
					if (attackerBody)
					{
						CharacterMaster attackerMaster = attackerBody.master;
						if (attackerMaster)
						{
							Inventory inventory = attackerMaster.inventory;
							if (inventory)
							{
								int itemCount = inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
								if (itemCount > 0)
								{
									if (attackerBody.HasBuff(SeerReady))
									{
										damageInfo.crit = true;
									}
								}
							}
						}
					}
				}
			}
			orig(self, damageInfo);
        }
		private static void Global_DamageDealt(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			DamageInfo damageInfo = damageReport.damageInfo;
			if (damageInfo.crit && damageInfo.procCoefficient > 0f)
			{
				CharacterBody victimBody = damageReport.victimBody;
				if (damageReport.victim && victimBody)
				{
					CharacterMaster attackerMaster = damageReport.attackerMaster;
					CharacterBody attackerBody = damageReport.attackerBody;
					if (attackerBody && attackerMaster)
					{
						Inventory inventory = damageReport.attackerBody.inventory;
						if (inventory)
						{
							int itemCount = inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
							if (itemCount > 0)
							{
								if(attackerBody.HasBuff(SeerReady))
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
		private static void Wreck(DamageInfo damageInfo, CharacterBody attackerBody, CharacterBody victimBody, int itemCount)
        {
			TeamIndex teamIndex = attackerBody.master.teamIndex;
			float totalDamage = damageInfo.damage;
			totalDamage *= BaseDamage + (StackDamage * (itemCount - 1));
			float searchDist = BaseRadius + (StackRadius * (itemCount - 1));

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
						voidLightningOrb.procChainMask = damageInfo.procChainMask;
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
			voidLightningOrbSelf.procChainMask = damageInfo.procChainMask;
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
