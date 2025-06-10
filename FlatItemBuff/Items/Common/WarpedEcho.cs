using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class WarpedEcho
	{
		private const string LogName = "Warped Echo";
		internal static bool Enable = false;
		internal static float BaseArmor = 6f;
		internal static float StackArmor = 6f;
		internal static bool InCountArmor = true;
		internal static bool InCountBlock = false;
		internal static bool OutIgnoreArmor = false;
		internal static bool OutIgnoreBlock = false;
		internal static bool UseOldVisual = false;
		internal static bool HealthDisplay = true;
		public WarpedEcho()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			UpdateBuff();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseArmor = Math.Max(0f, BaseArmor);
			StackArmor = Math.Max(0f, StackArmor);
		}
		private void UpdateBuff()
		{
			//"RoR2/DLC2/Items/DelayedDamage/bdDelayedDamageBuff.asset"
			BuffDef DelayBuff = Addressables.LoadAssetAsync<BuffDef>("062a0ccbc4669814dad5207a1e14946e").WaitForCompletion();
			DelayBuff.canStack = true;
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = string.Format("Delays half of incoming damage. Increases armor until all delayed damage passes.");
			string description = string.Format("<style=cIsDamage>50%</style> of incoming damage is delayed for <style=cIsDamage>3</style> seconds. Increases <style=cIsHealing>armor</style> by <style=cIsHealing>{0} <style=cStack>(+{1} per stack)</style></style> until all delayed damage has passed.", BaseArmor, StackArmor);
			LanguageAPI.Add("ITEM_DELAYEDDAMAGE_PICKUP", pickup);
			LanguageAPI.Add("ITEM_DELAYEDDAMAGE_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_OnTakeDamage);
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			On.RoR2.CharacterBody.UpdateDelayedDamage += CharacterBody_UpdateEcho;
			On.RoR2.CharacterBody.SecondHalfOfDelayedDamage += CharacterBody_OnDelayDamage;
			On.RoR2.CharacterBody.UpdateSecondHalfOfDamage += CharacterBody_UpdateSecondHalfOfDamage;
			IL.RoR2.HealthComponent.GetHealthBarValues += new ILContext.Manipulator(IL_GetHealthBarValues);
		}
		private void CharacterBody_UpdateEcho(On.RoR2.CharacterBody.orig_UpdateDelayedDamage orig, CharacterBody self, float deltaTime)
		{
			return;
		}
		private void CharacterBody_OnDelayDamage(On.RoR2.CharacterBody.orig_SecondHalfOfDelayedDamage orig, CharacterBody self, DamageInfo damageInfo, float delay)
		{
			HealthComponent hpcomp = self.healthComponent;
			if (hpcomp)
			{
				if (UseOldVisual)
                {
					if (ForceDelayVFX(self) || damageInfo.damage * 20f >= self.healthComponent.combinedHealth)
					{
						//Transform transform = self.mainHurtBox ? self.mainHurtBox.transform : self.transform;
						//UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/DelayedDamageIndicator"), transform.position, Quaternion.identity, transform);
						self.TransmitItemBehavior(new CharacterBody.NetworkItemBehaviorData(DLC2Content.Items.DelayedDamage.itemIndex, 0f));
						Util.PlaySound("Play_item_proc_delayedDamage", self.gameObject);
					}
				}
				self.AddBuff(DLC2Content.Buffs.DelayedDamageBuff);
			}
			orig(self, damageInfo, delay);
		}

		private void CharacterBody_UpdateSecondHalfOfDamage(On.RoR2.CharacterBody.orig_UpdateSecondHalfOfDamage orig, CharacterBody self, float deltaTime)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (self.halfDamageReady)
			{
				for (int i = 0; i < self.incomingDamageList.Count; i++)
				{
					CharacterBody.DelayedDamageInfo delayedDamageInfo = self.incomingDamageList[i];
					DamageInfo halfDamage = delayedDamageInfo.halfDamage;
					float subDamage = halfDamage.damage;
					halfDamage.position = self.GetBody().corePosition;
					delayedDamageInfo.timeUntilDamage -= deltaTime;
					if (delayedDamageInfo.timeUntilDamage <= 0f)
					{
						new EffectData
						{
							origin = self.transform.position
						}.SetNetworkedObjectReference(self.gameObject);
						self.healthComponent.TakeDamage(halfDamage);
						HealthComponent healthComponent = self.healthComponent;
						healthComponent.NetworkechoDamage = healthComponent.echoDamage - subDamage;
						self.RemoveBuff(DLC2Content.Buffs.DelayedDamageBuff);
						self.incomingDamageList.RemoveAt(i);
					}
				}
			}
			return;
		}

		private bool ForceDelayVFX(CharacterBody self)
        {
			if (self.incomingDamageList.Count > 0)
            {
				for(int i = 0; i < self.incomingDamageList.Count; i++)
                {
					if (self.incomingDamageList[i].timeUntilDamage > 2.8f)
                    {
						return false;
					}
                }
            }
			return true;
        }
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			if (sender.HasBuff(DLC2Content.Buffs.DelayedDamageBuff))
			{
				int itemCount = 0;
				if (sender.inventory)
                {
					itemCount = Math.Max(0, sender.inventory.GetItemCount(DLC2Content.Items.DelayedDamage) - 1);
                }
				args.armorAdd += BaseArmor + (StackArmor * itemCount);
			}
		}
		private void IL_OnTakeDamage(ILContext il)
		{
			//Moved entire chunk back so that A) it works properly with Eclipse 8 and B) It fixes a bug with shields and barrier
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(

				x => x.MatchLdloc(7),
				x => x.MatchStloc(8)
			))
			{
				ilcursor.Index += 1;
				//ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldarg, 1);
				ilcursor.Emit(OpCodes.Ldloc, 6);
				//ilcursor.Emit(OpCodes.Ldloc, 7);
				ilcursor.EmitDelegate<Func<float, HealthComponent, DamageInfo, bool, float>>((returnValue, self, damageInfo, ignoreBlock) =>
				{
					if (self.body && self.body.inventory)
                    {
						if (!InCountArmor || (damageInfo.damageType & DamageType.BypassArmor) == DamageType.Generic)
                        {
							if (!InCountBlock || !ignoreBlock)
							{
								if (self.body.inventory.GetItemCount(DLC2Content.Items.DelayedDamage) > 0 && damageInfo.damage > 0f && !damageInfo.delayedDamageSecondHalf && !damageInfo.rejected)
								{
									returnValue *= 0.5f;
									DamageType damageType = DamageType.Generic;
									if (OutIgnoreArmor)
									{
										damageType |= DamageType.BypassArmor;
									}
									if (OutIgnoreBlock)
									{
										damageType |= DamageType.BypassBlock;
									}
									if (self.ospTimer > 0f || (damageInfo.damageType & DamageType.NonLethal) > DamageType.Generic)
									{
										damageType |= DamageType.NonLethal;
									}
									if (self.body.hasOneShotProtection && (damageInfo.damageType & DamageType.BypassOneShotProtection) != DamageType.BypassOneShotProtection)
                                    {
										damageType |= DamageType.NonLethal;
									}
									if ((damageInfo.damageType & DamageType.Silent) > DamageType.Generic)
									{
										damageType |= DamageType.Silent;
									}
									DamageInfo damageInfo2 = new DamageInfo
									{
										crit = damageInfo.crit,
										damage = returnValue,
										damageType = damageType,
										attacker = damageInfo.attacker,
										position = damageInfo.position,
										inflictor = damageInfo.inflictor,
										procChainMask = damageInfo.procChainMask,
										procCoefficient = 0f,
										damageColorIndex = damageInfo.damageColorIndex,
										delayedDamageSecondHalf = true,
										firstHitOfDelayedDamageSecondHalf = false
									};
									self.body.SecondHalfOfDelayedDamage(damageInfo2, 3f);
								}
							}
						}
					}
					return returnValue;
				});
				ilcursor.Emit(OpCodes.Stloc, 7);
				ilcursor.Emit(OpCodes.Ldloc, 7);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnTakeDamage A - Hook failed");
			}
			//Making the original code not run
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "DelayedDamageBuff")
			))
			{
				ilcursor.Index -= 1;
				ilcursor.RemoveRange(3);
				ilcursor.EmitDelegate<Func<HealthComponent, bool>>((self) =>
				{
					return false;
				});
			}
			else
            {
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnTakeDamage B - IL Hook failed");
			}
		}
		/*private void IL_ApplyDelayedDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			//ensures that the correct damage amount is subtracted from the echo damage tracker.
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(0),
				x => x.MatchLdfld(typeof(DamageInfo), "damage"),
				x => x.MatchSub()
			))
			{
				ilcursor.RemoveRange(2);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 1);
				ilcursor.EmitDelegate<Func<CharacterBody, int, float>>((body, index) =>
				{
					return body.incomingDamageList[index].halfDamage.damage;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Warped Echo - Deal Delayed Damage Override A - IL Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "DelayedDamageDebuff")
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldsfld, typeof(DLC2Content.Buffs).GetField("DelayedDamageBuff"));
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Warped Echo - Deal Delayed Damage Override B - IL Hook failed");
			}
		}*/

		private float GetArmorMult(float armor)
        {
			if (armor >= 0f)
            {
				return 1f - armor / (armor + 100f);
			}
			return 2f + 100f / (100f - armor);
		}
		private void IL_GetHealthBarValues(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (HealthDisplay)
            {
				if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(typeof(HealthComponent), "echoDamage"),
				x => x.MatchLdloc(1),
				x => x.MatchMul()
				))
				{
					ilcursor.Index += 1;
					ilcursor.RemoveRange(1);
					ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
					{
						float mult = 1f;
						if (OutIgnoreArmor == false)
                        {
							mult = GetArmorMult(self.body.armor);
						}
						return Math.Max(0f, (self.echoDamage * mult) - self.barrier);
					});
				}
				else
				{
					UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_GetHealthBarValues A - Hook failed");
				}
			}
			else
            {
				if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(typeof(HealthComponent), "echoDamage"),
				x => x.MatchLdloc(1),
				x => x.MatchMul()
				))
				{
					ilcursor.Index += 1;
					ilcursor.RemoveRange(1);
					ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
					{
						return 0f;
					});
				}
				else
				{
					UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_GetHealthBarValues B - Hook failed");
				}
			}
		}
	}
}
