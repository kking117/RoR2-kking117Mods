using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

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
		public WarpedEcho()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseArmor = Math.Max(0f, BaseArmor);
			StackArmor = Math.Max(0f, StackArmor);
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
			IL.RoR2.CharacterBody.UpdateSecondHalfOfDamage += new ILContext.Manipulator(IL_ApplyDelayedDamage);
			On.RoR2.CharacterBody.SecondHalfOfDelayedDamage += CharacterBody_OnDelayDamage;
			SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			//Stop some old updates
			On.RoR2.CharacterBody.UpdateDelayedDamage += CharacterBody_UpdateEcho;
            //On.RoR2.CharacterBody.DelayedDamageBehavior.Update += DelayedDamageBehavior_Update;
		}
        //private void DelayedDamageBehavior_Update(On.RoR2.CharacterBody.DelayedDamageBehavior.orig_Update orig, CharacterBody.DelayedDamageBehavior self)
        //{
		//	return;
        //}
        private void CharacterBody_UpdateEcho(On.RoR2.CharacterBody.orig_UpdateDelayedDamage orig, CharacterBody self, float deltaTime)
		{
			return;
		}
		private void CharacterBody_OnDelayDamage(On.RoR2.CharacterBody.orig_SecondHalfOfDelayedDamage orig, CharacterBody self, DamageInfo damageInfo, float delay)
		{
			HealthComponent hpcomp = self.healthComponent;
			if (hpcomp)
            {
				if (ForceDelayVFX(self) || damageInfo.damage * 20f >= self.healthComponent.combinedHealth)
				{
					//Transform transform = self.mainHurtBox ? self.mainHurtBox.transform : self.transform;
					//UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/DelayedDamageIndicator"), transform.position, Quaternion.identity, transform);
					self.TransmitItemBehavior(new CharacterBody.NetworkItemBehaviorData(DLC2Content.Items.DelayedDamage.itemIndex, 0f));
					Util.PlaySound("Play_item_proc_delayedDamage", self.gameObject);
				}
				self.AddBuff(DLC2Content.Buffs.DelayedDamageBuff);
			}
			orig(self, damageInfo, delay);
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
		private void IL_ApplyDelayedDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "DelayedDamageDebuff")
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldsfld, typeof(DLC2Content.Buffs).GetField("DelayedDamageBuff"));
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Warped Echo - Deal Delayed Damage Override - IL Hook failed");
			}
		}
	}
}
