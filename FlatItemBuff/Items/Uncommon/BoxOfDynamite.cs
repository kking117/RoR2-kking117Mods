using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class BoxOfDynamite
	{
		//"RoR2/DLC3/Items/DronesDropDynamite/DynamiteProjectile.prefab"
		public static GameObject DynamiteProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("374f5d39c4d41944a818612c97627d62").WaitForCompletion();
		private const string LogName = "Box of Dynamite";
		internal static bool Enable = false;
		internal static float BaseDamage = 2.5f;
		internal static float StackDamage = 1.5f;
		internal static float BaseRadius = 7f;
		public BoxOfDynamite()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			UpdateProjectile();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0, StackDamage);
			BaseRadius = Math.Max(0, BaseRadius);
		}

		private void UpdateProjectile()
		{
			if (DynamiteProjectilePrefab)
            {
				ProjectileExplosion projExp = DynamiteProjectilePrefab.GetComponent<ProjectileExplosion>();
				if (projExp)
                {
					projExp.blastRadius = BaseRadius;
				}
			}
		}
		private void UpdateText()
		{
			string desc = "";
			if (StackDamage > 0f)
            {
				desc = string.Format("Gain <style=cIsDamage>Lt. Droneboy</style>. While in combat, your drones drop sticks of dynamite that detonate for <style=cIsDamage>{0}% damage <style=cStack>(+{1}% per stack)</style></style>, stunning enemies. Recharges after <style=cIsUtility>10</style> seconds.", BaseDamage * 100f, StackDamage * 100f);
            }
			else
            {
				desc = string.Format("Gain <style=cIsDamage>Lt. Droneboy</style>. While in combat, your drones drop sticks of dynamite that detonate for <style=cIsDamage>{0}% damage</style>, stunning enemies. Recharges after <style=cIsUtility>10</style> seconds.", BaseDamage * 100f);
			}
			LanguageAPI.Add("ITEM_DRONESDROPDYNAMITE_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.Items.DroneDynamiteBehaviour.FixedUpdate += new ILContext.Manipulator(IL_FixedUpdate);
		}
		private void IL_FixedUpdate(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdfld(typeof(RoR2.Items.BaseItemBodyBehavior), "stack")
			))
			{
				ilcursor.Index -= 2;
				ilcursor.Next.Operand = StackDamage;
				ilcursor.Index -= 1;
				ilcursor.Next.Operand = BaseDamage;
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_FixedUpdate - Hook failed");
			}
		}
	}
}
