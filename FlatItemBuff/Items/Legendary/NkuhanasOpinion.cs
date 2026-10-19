using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.Orbs;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class NkuhanasOpinion
	{
		private const string LogName = "N'kuhana's Opinion";
		public static BuffDef SoulSkullBuff;
		internal static bool Enable = false;
		internal static float BaseGain = 1.5f;
		internal static float StackGain = 0.75f;
		internal static float BaseSoul = 1.5f;
		internal static float StackSoul = 0.75f;
		internal static float BaseSkullBase = 0.4f;
		internal static float StackSkullBase = 0.0f;
		internal static float BaseSkullSoul = 2.0f;
		internal static float StackSkullSoul = 0.0f;
		internal static float SoulFireMin = 0.1f;
		internal static float SoulFireRate = 0.4f;
		internal static float SkullProc = 0.2f;
		internal static float LifeSteal = 0.25f;
		internal static float EnemyMult = 0.05f;
		internal static float WeakDuration = 1.5f;
		private static List<HealthComponent> NovaOnHealOwners = null;
		internal static float LastUpdateRun = 0f;
		public NkuhanasOpinion()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			CreateBuffs();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			Hooks();
		}
		private void ClampConfig()
		{
			BaseGain = Math.Max(0f, BaseGain);
			StackGain = Math.Max(0f, StackGain);
			BaseSoul = Math.Max(0f, BaseSoul);
			StackSoul = Math.Max(0f, StackSoul);
			BaseSkullBase = Math.Max(0f, BaseSkullBase);
			StackSkullBase = Math.Max(0f, StackSkullBase);
			BaseSkullSoul = Math.Max(0f, BaseSkullSoul);
			StackSkullSoul = Math.Max(0f, StackSkullSoul);
			SoulFireMin = Math.Max(0.1f, SoulFireMin);
			SoulFireRate = Math.Max(0.2f, SoulFireRate);
			SkullProc = Math.Max(0f, SkullProc);
			LifeSteal = Math.Max(0f, LifeSteal);
			LifeSteal = Math.Min(LifeSteal, 1f);
			WeakDuration = Math.Max(0f, WeakDuration);
		}
		private void CreateBuffs()
		{
			//"RoR2/Base/TeamWarCry/bdTeamWarCry.asset"
			BuffDef WarCryBuff = Addressables.LoadAssetAsync<BuffDef>("1c28951aea37fec47910c9693fb6ab38").WaitForCompletion();
			//"RoR2/Junk/Common/bdMeatRegenBoost.asset"
			BuffDef MeatRegen = Addressables.LoadAssetAsync<BuffDef>("1c9bc2e1186d394429928b51b132114f").WaitForCompletion();
			SoulSkullBuff = Utils.ContentManager.AddBuff("SoulCharge", WarCryBuff.iconSprite, MeatRegen.buffColor, true, false, false, false, false);
		}
		private void UpdateText()
		{
			string pickup = "Fire haunting skulls when healed.";
			string description = "";
			if (StackGain > 0f)
			{
				description += string.Format("Store <style=cIsHealing>{0}%</style> <style=cStack>(+{1}% per stack)</style> of healing as <style=cIsHealing>Soul Energy</style>,", BaseGain * 100f, StackGain * 100f);
			}
			else
            {
				description += string.Format("Store <style=cIsHealing>{0}%</style> <style=cStack>(+{1}% per stack)</style> of healing as <style=cIsHealing>Soul Energy</style>,", BaseGain * 100f);
			}
			if (StackSoul > 0f)
			{
				description += string.Format(" up to <style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style></style> of your <style=cIsHealing>maximum health</style>.", BaseSoul * 100f, StackSoul * 100f);
			}
			else
			{
				description += string.Format(" up to <style=cIsHealing>{0}%</style> of your <style=cIsHealing>maximum health</style>.", BaseSoul * 100f);
			}
			description += string.Format(" After your <style=cIsHealing>Soul Energy</style> reaches <style=cIsHealing>{0}%</style> of your <style=cIsHealing>maximum health</style>, ", SoulFireMin * 100f);
			if (WeakDuration > 0f)
            {
				description += string.Format("release <style=cIsDamage>weakening skulls</style> that deal");
			}
			else
            {
				description += string.Format("release <style=cIsDamage>skulls</style> that deal");
			}

			if (BaseSkullBase > 0f)
			{
				if (StackSkullBase > 0f)
                {
					description += string.Format(" <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style> damage</style>", BaseSkullBase * 100f, StackSkullBase * 100f);
				}
				else
                {
					description += string.Format(" <style=cIsDamage>{0}% damage</style>", BaseSkullBase * 100f);
				}
			}
			if (BaseSkullSoul > 0f)
            {
				if (BaseSkullBase > 0f)
				{
					description += " plus";
				}

				if (StackSkullBase > 0f)
				{
					description += string.Format(" <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style></style> of the spent <style=cIsHealing>Soul Energy</style>.", BaseSkullSoul * 100f, StackSkullSoul * 100f);
				}
				else
				{
					description += string.Format(" <style=cIsDamage>{0}%</style> of the spent <style=cIsHealing>Soul Energy</style>.", BaseSkullSoul * 100f);
				}
			}
			else
            {
				description += ".";
            }
			if (LifeSteal > 0f)
            {
				pickup += " When <color=#FF7F7F>ANYONE ELSE</color> heals, steal some of it.";
				description += string.Format(" When <color=#FF7F7F>ANYONE ELSE</color> heals, steal <color=#FF7F7F>{0}%</color> of it.", LifeSteal * 100f);
			}
			LanguageAPI.Add("ITEM_NOVAONHEAL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_NOVAONHEAL_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.Heal += new ILContext.Manipulator(IL_OnHeal);
			IL.RoR2.HealthComponent.ServerFixedUpdate += new ILContext.Manipulator(IL_HPFixedUpdate);
			if (WeakDuration > 0f)
            {
				IL.RoR2.Orbs.DevilOrb.OnArrival += new ILContext.Manipulator(IL_DevilOrbArrival);
			}
		}

		private void UpdateListNovaOnHeal(bool Force)
        {
			if (LastUpdateRun >= Time.fixedTime + 10f)
            {
				LastUpdateRun = 0f;
			}
			//Skip this if it was already done in the same time.
			if (Force || LastUpdateRun + 1f <= Time.fixedTime)
            {
				LastUpdateRun = Time.fixedTime;
				NovaOnHealOwners = new List<HealthComponent>();
				foreach (var characterBody in CharacterBody.readOnlyInstancesList)
				{
					if (characterBody.healthComponent && characterBody.healthComponent.alive && characterBody.inventory)
					{
						if (characterBody.inventory.GetItemCountEffective(RoR2Content.Items.NovaOnHeal) > 0)
						{
							NovaOnHealOwners.Add(characterBody.healthComponent);
						}
					}
				}
				//MainPlugin.ModLogger.LogInfo("User = " + NovaOnHealOwners.Count);
			}
		}

		private void IL_DevilOrbArrival(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			//intervene with the timer conditional and create our result
			if (ilcursor.TryGotoNext(
				x => x.MatchLdfld(typeof(DevilOrb), "damageValue"),
				x => x.MatchStfld(typeof(DamageInfo), "damage")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 1);
				ilcursor.EmitDelegate<Func<DevilOrb, DamageInfo, DamageInfo>>((self, damageInfo) =>
				{
					if (self.procChainMask.HasProc(ProcType.HealNova))
					{
						CharacterBody body = self.target.healthComponent.body;
						if (body)
                        {
							body.AddTimedBuff(RoR2Content.Buffs.Weak, WeakDuration);
                        }
					}
					return damageInfo;
				});
				ilcursor.Emit(OpCodes.Stloc, 1);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_DevilOrbArrival A - IL Hook failed");
			}
		}
		private void IL_HPFixedUpdate(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			//intervene with the timer conditional and create our result
			if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(typeof(HealthComponent), "devilOrbTimer"),
				x => x.MatchLdcR4(0.0f)
			))
			{
				ilcursor.Index += 1;
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
				{
					float soulPerShot = self.fullCombinedHealth / 10f;
					if (self.devilOrbTimer <= 0.0f)
                    {
						self.devilOrbTimer += SoulFireRate;
						int itemCount = self.itemCounts.novaOnHeal;
						if (itemCount > 0f && self.gameObject && self.body)
                        {
							float soulDamage = BaseSkullBase + (StackSkullBase * Math.Max(0, itemCount - 1));
							soulDamage *= self.body.damage;
							soulDamage += soulPerShot * (BaseSkullSoul + (StackSkullSoul * Math.Max(0, itemCount - 1)));

							TeamIndex teamIndex = TeamComponent.GetObjectTeam(self.gameObject);
							if (teamIndex != TeamIndex.Player)
							{
								soulDamage *= EnemyMult;
							}
							int totalShots = (int)Math.Floor(self.devilOrbHealPool / soulPerShot);
							if (soulDamage > 0f && totalShots > 0)
							{
								Vector3 shotPosition = self.body.aimOriginTransform.position;
								List<HurtBox> targets = HealNovaTargetList(self, shotPosition, 40f, totalShots);
								if (targets != null && targets.Count > 0)
								{
									bool isCrit = Util.CheckRoll(self.body.crit, self.body.master);
									for (int i = 0; i < targets.Count; i++)
									{
										if (targets[i])
										{
											self.devilOrbHealPool -= soulPerShot;
											DevilOrb devilOrb = new DevilOrb();
											devilOrb.origin = shotPosition;
											devilOrb.damageValue = soulDamage;
											devilOrb.teamIndex = teamIndex;
											devilOrb.attacker = self.gameObject;
											devilOrb.damageColorIndex = DamageColorIndex.Poison;
											devilOrb.scale = 1f;
											devilOrb.procCoefficient = SkullProc;
											devilOrb.procChainMask.AddProc(ProcType.HealNova);
											devilOrb.effectType = DevilOrb.EffectType.Skull;
											devilOrb.target = targets[i];
											devilOrb.isCrit = isCrit;
											OrbManager.instance.AddOrb(devilOrb);
										}
									}
								}
							}
							Components.NovaOnHealBatch novaHealBatch = self.GetComponent<Components.NovaOnHealBatch>();
							if (novaHealBatch)
							{
								novaHealBatch.DoHeal();
							}
						}
					}
					int remainShots = (int)Math.Floor(self.devilOrbHealPool / soulPerShot);
					if (remainShots != self.body.GetBuffCount(SoulSkullBuff))
                    {
						self.body.SetBuffCount(SoulSkullBuff.buffIndex, remainShots);
                    }
					return 1f;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_HPFixedUpdate A - IL Hook failed");
			}
		}

		private List<HurtBox> HealNovaTargetList(HealthComponent owner, Vector3 position, float range, int maxShots)
        {
			TeamIndex teamIndex = TeamComponent.GetObjectTeam(owner.gameObject);
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = position;
			bullseyeSearch.searchDirection = Vector3.zero;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(teamIndex);
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.maxDistanceFilter = range;
			bullseyeSearch.RefreshCandidates();
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			
			List<HurtBox> filterList = new List<HurtBox>();
			for(int i = 0; i<list.Count; i++)
            {
				if (list[i] && list[i].healthComponent)
                {
					filterList.Add(list[i]);
				}
			}
			if (filterList.Count <= 0)
			{
				return null;
			}
			else if (filterList.Count == 1)
			{
				return filterList;
			}
			List<HurtBox> finalList = new List<HurtBox>();
			while (finalList.Count < maxShots)
            {
				int i = UnityEngine.Random.Range(0, filterList.Count);
				finalList.Add(filterList[i]);
				filterList.RemoveAt(i);
				if (filterList.Count < 1)
                {
					break;
                }
			}
			return finalList;
		}
		private void IL_OnHeal(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			//float num3 = amount;
			//num3 = the final healing value, before it gets capped and certain post heal effects happen.
			if (LifeSteal > 0f && ilcursor.TryGotoNext(
				x => x.MatchLdarg(1),
				x => x.MatchStloc(3)
			))
            {
				ilcursor.Index += 1;
				//ilcursor.RemoveRange(3);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldarg, 1);
				ilcursor.Emit(OpCodes.Ldarg, 2);
				ilcursor.Emit(OpCodes.Ldarg, 3);
				ilcursor.EmitDelegate<Func<HealthComponent, float, ProcChainMask, bool, float>>((self, amount, procChainMask, nonRegen) =>
				{
					if (!nonRegen || procChainMask.HasProc(ProcType.SharedSuffering))
                    {
						return amount;
                    }
					UpdateListNovaOnHeal(false);
					if (NovaOnHealOwners == null)
					{
						return amount;
					}
					List<HealthComponent> itemUsers = new List<HealthComponent>();
					for (int i = 0; i<NovaOnHealOwners.Count; i++)
					{
						if (NovaOnHealOwners[i] != self)
                        {
							if (NovaOnHealOwners[i].alive)
                            {
								itemUsers.Add(NovaOnHealOwners[i]);
							}
                        }
					}
					if (itemUsers.Count > 0)
                    {
						float repeatSteal = 1f - LifeSteal;
						float stealAmount = 1f - (float)Math.Pow(repeatSteal, itemUsers.Count);
						//MainPlugin.ModLogger.LogInfo("repeatSteal [" + repeatSteal + "] stealAmount [" + stealAmount + "] amount [" + amount + "]");
						stealAmount *= amount;
						if (stealAmount > 0f)
                        {
							procChainMask.AddProc(ProcType.SharedSuffering);
							amount -= stealAmount;
							//MainPlugin.ModLogger.LogInfo("Stole [" + stealAmount + "] for " + itemUsers.Count + " users");
							stealAmount /= itemUsers.Count;
							for (int i = 0; i < itemUsers.Count; i++)
							{
								if (itemUsers[i])
                                {
									Components.NovaOnHealBatch novaHealBatch = itemUsers[i].GetComponent<Components.NovaOnHealBatch>();
									if (!novaHealBatch)
									{
										novaHealBatch = itemUsers[i].gameObject.AddComponent<Components.NovaOnHealBatch>();
									}
									if (novaHealBatch)
									{
										novaHealBatch.healAmount += stealAmount;
										//shoddy fix to kick start the effect properly.
										if (itemUsers[i].devilOrbHealPool <= 0f)
										{
											itemUsers[i].devilOrbHealPool = 0.001f;
										}
									}
								}
							}
						}
					}
					return amount;
				});
				ilcursor.Emit(OpCodes.Starg, 1);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnHeal A - IL Hook failed");
			}

			if (ilcursor.TryGotoNext(
				x => x.MatchStfld(typeof(HealthComponent), "devilOrbHealPool")
			))
			{
				ilcursor.Index -= 12;
				ilcursor.RemoveRange(12);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldarg, 1);
				ilcursor.EmitDelegate<Func<HealthComponent, float, float>>((self, amount) =>
				{
					int itemCount = Math.Max(0, self.itemCounts.novaOnHeal -1);
					float soulMult = BaseGain + (StackGain * itemCount);
					float soulHold = BaseSoul + (StackSoul * itemCount);
					float soulCharge = amount * soulMult;
					soulHold *= self.fullCombinedHealth;
					return Math.Min(self.devilOrbHealPool + soulCharge, soulHold);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnHeal B - IL Hook failed");
			}
		}
	}
}
