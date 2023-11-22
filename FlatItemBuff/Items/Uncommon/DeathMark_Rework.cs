using System;
using RoR2;
using RoR2.UI;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class DeathMark_Rework
	{
		public static BuffDef DeathMarkReadyBuff;
		public static BuffDef DeathMarkCooldownBuff;
		private static Color BuffColorR = new Color(1f, 0.164f, 0f, 1f);
		private static Color BuffColorC = new Color(0.345f, 0.360f, 0.372f, 1f);
		internal static bool Enable = false;
		internal static float BaseDuration = 6f;
		internal static float StackDuration = 3f;
		internal static int BaseCooldown = 0;
		internal static float MarkBaseBonus = 0.5f;
		internal static float MarkStackBonus = 0.2f;
		internal static bool AllowMarkStacking = false;
		public DeathMark_Rework()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Death Mark");
			ClampConfig();
			CreateBuff();
			UpdateText();
			Hooks();
		}
		private void CreateBuff()
		{
			BuffDef deathMarkDef = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/DeathMark/bdDeathMark.asset").WaitForCompletion();
			if (BaseCooldown > 0)
			{
				DeathMarkReadyBuff = Modules.Buffs.AddNewBuff("Death Mark Ready", deathMarkDef.iconSprite, BuffColorR, false, false, false);
				DeathMarkCooldownBuff = Modules.Buffs.AddNewBuff("Death Mark Cooldown", deathMarkDef.iconSprite, BuffColorC, true, false, true);
			}
			if (AllowMarkStacking)
			{
				deathMarkDef.canStack = true;
			}
		}
		private void ClampConfig()
		{
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			BaseCooldown = Math.Max(0, BaseCooldown);
			MarkBaseBonus = Math.Max(0f, MarkBaseBonus);
			MarkStackBonus = Math.Max(0f, MarkStackBonus);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string damageBonus = string.Format("<style=cIsDamage>{0}%</style>", MarkBaseBonus * 100f);
			string durationBonus = string.Format("<style=cIsUtility>{0}</style>", BaseDuration);
			string rechargePick = "";
			string rechargeDesc = "";
			if (BaseCooldown > 0)
            {
				rechargePick = " Recharges over time.";
				rechargeDesc = string.Format(" Recharges every <style=cIsUtility>{0}</style> seconds.", BaseCooldown);
			}
			if (StackDuration > 0f)
			{
				durationBonus += string.Format(" <style=cStack>(+{0} per stack)</style>", StackDuration);
			}
			string pickup = string.Format("Pinging an enemy marks them for death, taking bonus damage.{0}", rechargePick);
			string desc = string.Format("Enemies you ping are <style=cIsDamage>marked for death</style>, increasing damage taken by {0} from all sources for {1} seconds.{2}", damageBonus, durationBonus, rechargeDesc);
			LanguageAPI.Add("ITEM_DEATHMARK_PICKUP", pickup);
			LanguageAPI.Add("ITEM_DEATHMARK_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
			IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
			On.RoR2.UI.PingIndicator.RebuildPing += RebuildPing;
			if (BaseCooldown > 0)
            {
				SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
			}
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.DeathMark_Rework>(self.inventory.GetItemCount(RoR2Content.Items.DeathMark));
		}
		private void RebuildPing(On.RoR2.UI.PingIndicator.orig_RebuildPing orig, RoR2.UI.PingIndicator self)
		{
			orig(self);
			if (self.pingType == PingIndicator.PingType.Enemy)
			{
				CharacterMaster pingMaster = self.pingOwner.GetComponent<CharacterMaster>();
				if (pingMaster)
				{
					CharacterBody targetBody = self.pingTarget.GetComponent<CharacterBody>();
					if (targetBody)
                    {
						if (BaseCooldown > 0)
                        {
							TryDeathMarkTargetCooldown(pingMaster, targetBody);
						}
						else
                        {
							TryDeathMarkTarget(pingMaster, targetBody);
						}
					}
				}
			}
		}

		private void TryDeathMarkTarget(CharacterMaster attackerMaster, CharacterBody targetBody)
        {
			int itemCount = attackerMaster.inventory.GetItemCount(RoR2Content.Items.DeathMark);
			CharacterBody attackerBody = attackerMaster.GetBody();
			if (itemCount > 0 && targetBody && attackerBody)
            {
				if (attackerBody.healthComponent.alive)
				{
					TeamComponent targetTeam = targetBody.teamComponent;
					TeamComponent attackerTeam = attackerBody.teamComponent;
					if (attackerTeam && targetTeam)
					{

						if (attackerTeam.teamIndex != targetTeam.teamIndex)
						{
							itemCount = Math.Max(0, itemCount - 1);
							float duration = BaseDuration + (StackDuration * itemCount);
							targetBody.AddTimedBuff(RoR2Content.Buffs.DeathMark, duration);
						}
					}
				}
            }
		}
		private void TryDeathMarkTargetCooldown(CharacterMaster attackerMaster, CharacterBody targetBody)
		{
			int itemCount = attackerMaster.inventory.GetItemCount(RoR2Content.Items.DeathMark);
			CharacterBody attackerBody = attackerMaster.GetBody();
			if (itemCount > 0 && targetBody && attackerBody)
			{
				if (attackerBody.HasBuff(DeathMarkReadyBuff))
				{
					if (attackerBody.healthComponent.alive)
					{
						TeamComponent targetTeam = targetBody.teamComponent;
						TeamComponent attackerTeam = attackerBody.teamComponent;
						if (attackerTeam && targetTeam)
						{
							if (attackerTeam.teamIndex != targetTeam.teamIndex)
							{
								itemCount = Math.Max(0, itemCount - 1);
								float duration = BaseDuration + (StackDuration * itemCount);
								targetBody.AddTimedBuff(RoR2Content.Buffs.DeathMark, duration);
								int i = 1;
								while (i <= BaseCooldown)
								{
									attackerBody.AddTimedBuff(DeathMarkCooldownBuff, i);
									i++;
								}
								attackerBody.RemoveBuff(DeathMarkReadyBuff);
							}
						}
					}
				}
			}
		}
		private void IL_TakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, typeof(RoR2Content.Buffs), "DeathMark")
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 6),
				x => ILPatternMatchingExt.MatchLdcR4(x, 1.5f),
				x => ILPatternMatchingExt.MatchMul(x)
			);
			ilcursor.Index += 1;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
			{
				int buffCount = Math.Max(0, self.body.GetBuffCount(RoR2Content.Buffs.DeathMark)-1);
				return 1f +  (MarkBaseBonus + (MarkStackBonus * buffCount));
			});
		}
		private void IL_OnHitEnemy(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "DeathMark")
			);
			ilcursor.Index += 2;
			ilcursor.Emit(OpCodes.Ldc_I4_0);
			ilcursor.Emit(OpCodes.Mul);
		}
	}
}
