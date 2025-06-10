using System;
using RoR2;
using R2API;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class TitanicKnurl_Rework
	{
		public static GameObject StoneFistProjectile;
		private const string LogName = "Titanic Knurl Rework";
		internal static bool Enable = false;
		internal static float BaseDamage = 8f;
		internal static float StackDamage = 6f;
		internal static float BaseCooldown = 8f;
		internal static float StackCooldown = 0.15f;
		internal static float ProcRate = 1f;
		internal static bool ProcBands = true;
		internal static float TargetRadius = 60f;
		internal static int TargetMode = 0;
		public TitanicKnurl_Rework()
		{
			if (!Enable)
			{
				new Items.TitanicKnurl();
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			CreateProjectiles();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseCooldown = Math.Max(0f, BaseCooldown);
			StackCooldown = Math.Max(0f, StackCooldown);
			ProcRate = Math.Max(0f, ProcRate);
			TargetRadius = Math.Max(0f, TargetRadius);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = string.Format("Every few seconds a Stone Titan attacks a nearby enemy.");
			string stackCool = "";
			if (StackCooldown != 0f)
            {
				stackCool = string.Format(" <style=cStack>(-{0}% per stack)</style>", StackCooldown * 100f);
			}
			string targetText;
			if (TargetMode == 0)
            {
				targetText = string.Format(" a weak enemy within <style=cIsUtility>{0}m</style>", TargetRadius);
			}
			else
            {
				targetText = string.Format(" a nearby enemy within <style=cIsUtility>{0}m</style>", TargetRadius);
			}
			string stackDamage = "";
			if (StackDamage != 0f)
			{
				stackDamage = string.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 100f);
			}
			string desc = string.Format("Every <style=cIsUtility>{0}</style> seconds{1}{2} is attacked by a <style=cIsDamage>Stone Titan's fist</style> for <style=cIsDamage>{3}%</style>{4} base damage.", BaseCooldown, stackCool, targetText, BaseDamage * 100f, stackDamage);
			LanguageAPI.Add("ITEM_KNURL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNURL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void CreateProjectiles()
        {
			//"RoR2/Base/Titan/TitanPreFistProjectile.prefab"
			StoneFistProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("44b979afbdb688c438bd14bcf12e249f").WaitForCompletion(), MainPlugin.MODTOKEN + "StonePreFist");
			ProjectileDamage projDmg = StoneFistProjectile.GetComponent<ProjectileDamage>();
			ProjectileController projController = StoneFistProjectile.GetComponent<ProjectileController>();
			projController.procCoefficient = ProcRate;
			projDmg.damageType = DamageType.Stun1s;
			Utils.ContentManager.AddProjectile(StoneFistProjectile);
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.TitanicKnurl_Rework>(self.inventory.GetItemCount(RoR2Content.Items.Knurl));
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if(ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Knurl"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
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

		internal static float GetFistDamage(int itemCount)
        {
			return BaseDamage + (StackDamage * (itemCount - 1));
        }

		internal static float GetFistCooldown(int itemCount)
		{
			return Math.Max(0.5f, BaseCooldown / (1 + (StackCooldown * (itemCount - 1))));
		}
	}
}
