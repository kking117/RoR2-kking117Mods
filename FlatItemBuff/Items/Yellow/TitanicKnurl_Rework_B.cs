using System;
using RoR2;
using R2API;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class TitanicKnurl_Rework_B
	{
		public static GameObject LaserShotProjectile;
		public static BuffDef LaserChargeBuff;
		private static Color BuffColor = new Color(0.827f, 0.196f, 0.098f, 1f);
		private const string LogName = "Titanic Knurl Rework B";
		internal static bool Enable = false;
		internal static int ChargeCap = 99;
		internal static float LaserBaseDamage = 0.625f;
		internal static float LaserStackDamage = 0.0f;
		internal static float ShotBaseDamage = 1.5f;
		internal static float ShotStackDamage = 0.0f;
		internal static float BaseDuration = 4f;
		internal static float StackDuration = 2f;
		internal static float LaserProcRate = 0.15f;
		internal static float ShotProcRate = 0.15f;
		internal static float ChargeChance = 100f;
		internal static int ExtraShots = 3;
		internal static float HurtProcMult = 5f;
		internal static float HitProcMult = 1f;
		public TitanicKnurl_Rework_B()
		{
			if (!Enable)
			{
				new Items.TitanicKnurl_Rework();
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			CreateProjectiles();
			CreateBuff();
			Hooks();
		}
		private void ClampConfig()
		{
			LaserBaseDamage = Math.Max(0f, LaserBaseDamage);
			LaserStackDamage = Math.Max(0f, LaserStackDamage);
			ShotBaseDamage = Math.Max(0f, ShotBaseDamage);
			ShotStackDamage = Math.Max(0f, ShotStackDamage);
			BaseDuration = Math.Max(0f, BaseDuration);
			StackDuration = Math.Max(0f, StackDuration);
			LaserProcRate = Math.Max(0f, LaserProcRate);
			ShotProcRate = Math.Max(0f, ShotProcRate);
			ChargeCap = Math.Max(1, ChargeCap);
			ExtraShots = Math.Max(0, ExtraShots);
			HitProcMult = Math.Max(0f, HitProcMult);
			HurtProcMult = Math.Max(0f, HurtProcMult);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "";
			string chargeDesc = "";
			string laserDesc;
			if (HitProcMult > 0f && HurtProcMult > 0f)
			{
				chargeDesc += string.Format("On hit and when hit ");
				pickup += string.Format("Build charge on hit and when hit. Unleash a laser by activating your Special skill at full charge.");
			}
			else if (HurtProcMult > 0f)
            {
				chargeDesc += string.Format("When hit ");
				pickup += string.Format("Build charge when hit. Unleash a laser by activating your Special skill at full charge.");
			}
			else
			{
				chargeDesc += string.Format("On hit ");
				pickup += string.Format("Build charge on hit. Unleash a laser by activating your Special skill at full charge.");
			}
			chargeDesc += string.Format("build charge, up to <style=cIsUtility>{0} stacks</style>. ", ChargeCap);
			if (LaserStackDamage > 0f)
            {
				laserDesc = string.Format("Activating your <style=cIsUtility>Special skill</style> at full charge fires a <style=cIsDamage>laser</style> for <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style></style> base damage per second", LaserBaseDamage * 800f, LaserStackDamage * 800f);
			}
			else
            {
				laserDesc = string.Format("Activating your <style=cIsUtility>Special skill</style> at full charge fires a <style=cIsDamage>laser</style> for <style=cIsDamage>{0}%</style> base damage per second", LaserBaseDamage * 800f);
			}
			if (StackDuration > 0f)
			{
				laserDesc += string.Format(" for <style=cIsUtility>{0} <style=cStack>(+{1} per stack)</style> seconds</style>.", BaseDuration, StackDuration);
			}
			else
            {
				laserDesc += string.Format(" for <style=cIsUtility>{0} seconds</style>.", BaseDuration);
			}
			if (ExtraShots > 0)
            {
				if (ShotStackDamage > 0f)
                {
					laserDesc += string.Format(" <style=cIsUtility>Charges</style> built during the laser are fired as additional <style=cIsDamage>shots</style> for <style=cIsDamage>{0}% <style=cStack>(+{1}% per stack)</style></style> base damage.", ShotBaseDamage * 100f, ShotStackDamage * 100f);
				}
				else
                {
					laserDesc += string.Format(" <style=cIsUtility>Charges</style> built during the laser are fired as additional <style=cIsDamage>shots</style> for <style=cIsDamage>{0}%</style> base damage.", ShotBaseDamage * 100f);
				}
			}
			string desc = chargeDesc + laserDesc;
			LanguageAPI.Add("ITEM_KNURL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNURL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += new ILContext.Manipulator(IL_VisualEffects);
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
			if (HitProcMult > 0f || HurtProcMult > 0f)
            {
				SharedHooks.Handle_GlobalDamageEvent_Actions += GlobalDamageEvent;
			}	
		}
		private void CreateBuff()
		{
			//"RoR2/Base/ShockNearby/bdTeslaField.asset"
			LaserChargeBuff = Utils.ContentManager.AddBuff("Vanguard Charge", Addressables.LoadAssetAsync<BuffDef>("2353f759d7e907443854d73cb8372b7e").WaitForCompletion().iconSprite, BuffColor, true, false, false, false, false);
		}
		private void CreateProjectiles()
        {
			if (ExtraShots > 0)
            {
				//"RoR2/Base/Titan/TitanRockProjectile.prefab"
				LaserShotProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("28bcd151f55bad9499962668fddeef29").WaitForCompletion(), MainPlugin.MODTOKEN + "LaserShot");
				ProjectileDamage projDmg = LaserShotProjectile.GetComponent<ProjectileDamage>();
				ProjectileController projController = LaserShotProjectile.GetComponent<ProjectileController>();
				projController.procCoefficient = ShotProcRate;
				projDmg.damageColorIndex = DamageColorIndex.Item;
				Utils.ContentManager.AddProjectile(LaserShotProjectile);
			}
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.TitanicKnurl_Rework_B>(self.inventory.GetItemCountEffective(RoR2Content.Items.Knurl));
		}
		private void GlobalDamageEvent(DamageReport damageReport)
		{
			CharacterBody attackerBody = damageReport.attackerBody;
			if (attackerBody)
			{
				ProcChainMask procChainMask = damageReport.damageInfo.procChainMask;
				float procRate = damageReport.damageInfo.procCoefficient;
				if (procRate > 0)
                {
					//building charge through being hit
					if (HurtProcMult > 0f)
                    {
						CharacterMaster victimMaster = damageReport.victimMaster;
						if (victimMaster && victimMaster.inventory && victimMaster.GetBody())
						{
							CharacterBody victimBody = victimMaster.GetBody();
							int itemCount = victimMaster.inventory.GetItemCountEffective(RoR2Content.Items.Knurl);
							if (itemCount > 0)
							{
								int buffCount = Math.Max(0, victimBody.GetBuffCount(LaserChargeBuff.buffIndex));
								if (buffCount < ChargeCap)
								{
									int giveCharge = 0;
									procRate *= HurtProcMult;
									while (procRate > 1f)
									{
										giveCharge++;
										procRate -= 1f;
									}
									if (procRate > 0 && Util.CheckRoll(procRate * ChargeChance, victimMaster))
									{
										giveCharge++;
									}
									if (giveCharge > 0)
									{
										giveCharge = Math.Min(ChargeCap, giveCharge + buffCount);
										victimBody.SetBuffCount(LaserChargeBuff.buffIndex, giveCharge);
									}
								}
							}
						}
						procRate = damageReport.damageInfo.procCoefficient;
					}
					//building charge through hitting
					if (HitProcMult > 0f)
                    {
						CharacterMaster attackerMaster = damageReport.attackerMaster;
						if (attackerMaster && attackerMaster.inventory)
						{
							int itemCount = attackerMaster.inventory.GetItemCountEffective(RoR2Content.Items.Knurl);
							if (itemCount > 0)
							{
								int buffCount = Math.Max(0, attackerBody.GetBuffCount(LaserChargeBuff.buffIndex));
								if (buffCount < ChargeCap)
								{
									int giveCharge = 0;
									procRate *= HitProcMult;
									while (procRate > 1f)
									{
										giveCharge++;
										procRate -= 1f;
									}
									if (procRate > 0 && Util.CheckRoll(procRate * ChargeChance, attackerMaster))
									{
										giveCharge++;
									}
									if (giveCharge > 0)
									{
										giveCharge = Math.Min(ChargeCap, giveCharge + buffCount);
										attackerBody.SetBuffCount(LaserChargeBuff.buffIndex, giveCharge);
									}
								}
							}
						}
					}
				}
			}
		}
		private void IL_VisualEffects(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "TeamWarCry")
			))
			{
				ilcursor.RemoveRange(2);
				ilcursor.EmitDelegate<Func<CharacterBody, bool>>((self) =>
				{
					return self.HasBuff(RoR2Content.Buffs.TeamWarCry) || (self.GetBuffCount(LaserChargeBuff) >= ChargeCap);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_VisualEffect - Hook failed");
			}
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if(ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Knurl"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective")
			))
			{
				ilcursor.Index -= 2;
				ilcursor.RemoveRange(5);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats - Hook failed");
			}
		}
	}
}
