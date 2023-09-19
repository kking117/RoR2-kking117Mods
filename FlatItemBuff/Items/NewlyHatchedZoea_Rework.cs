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
	public class NewlyHatchedZoea_Rework
	{
		public static GameObject VoidMissileProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MissileVoid/MissileVoidProjectile.prefab").WaitForCompletion();

		public static BuffDef VoidMissileStockBuff = RoR2Content.Buffs.MercExpose;
		private static Color BuffColor = new Color(0.682f, 0.415f, 0.725f, 1f);

		internal static bool Enable = false;
		internal static int BaseStock = 12;
		internal static int StackStock = 4;
		internal static float BaseDamage = 8f;
		internal static float StackDamage = 6f;
		internal static float ProcRate = 0.2f;
		internal static int RestockTime = 30;
		internal static bool RestockOnFinish = true;
		public NewlyHatchedZoea_Rework()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo("Changing Newly Hatched Zoea");
			UpdateText();
			CreateBuff();
			CreateProjectiles();
			Hooks();
		}

		private void CreateBuff()
		{
			VoidMissileStockBuff = Modules.Buffs.AddNewBuff("Void Missile", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Merc/bdMercExpose.asset").WaitForCompletion().iconSprite, BuffColor, true, false, false);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("Activating your Special skill also unleashes a missile swarm. Recharges over time. <style=cIsVoid>Corrupts all </style><style=cIsTierBoss>yellow items</style><style=cIsVoid></style>.");

			string damageText = string.Format("{0}%", BaseDamage * 100f);
			if (StackDamage != 0f)
			{
				damageText += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 100f);
			}

			string stockText = string.Format("{0}", BaseStock);
			if (StackStock != 0f)
			{
				stockText += string.Format(" <style=cStack>(+{0} per stack)</style>", StackStock);
			}
			
			string desc = string.Format("Activating your <style=cIsUtility>Special skill</style> also fires a <style=cIsDamage>missile swarm</style> that deal <style=cIsDamage>{0}</style> base damage each. You can hold up to <style=cIsUtility>{1}</style> <style=cIsDamage>missiles</style> which all reload over <style=cIsUtility>{2}</style> seconds. <style=cIsVoid>Corrupts all </style><style=cIsTierBoss>yellow items</style><style=cIsVoid></style>.", damageText, stockText, RestockTime);
			LanguageAPI.Add("ITEM_VOIDMEGACRABITEM_PICKUP", pickup);
			LanguageAPI.Add("ITEM_VOIDMEGACRABITEM_DESC", desc);
		}
		private void Hooks()
		{
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void CreateProjectiles()
        {
			VoidMissileProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MissileVoid/MissileVoidProjectile.prefab").WaitForCompletion(), MainPlugin.MODTOKEN + "VoidMissile");
			ProjectileDamage projDmg = VoidMissileProjectile.GetComponent<ProjectileDamage>();
			ProjectileController projController = VoidMissileProjectile.GetComponent<ProjectileController>();
			MissileController projMissile = VoidMissileProjectile.GetComponent<MissileController>();
			projMissile.deathTimer = 10f; //Originally 3.5f
			projMissile.giveupTimer = 6f;
			projMissile.turbulence = 2f; //Originally 15
			projMissile.timer = -0.5f;
			projController.procCoefficient = ProcRate;
			Modules.Projectiles.AddProjectile(VoidMissileProjectile);
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<VoidMegaCrabItemBehavior>(0);
			self.AddItemBehavior<Behaviors.NewlyHatchedZoea_Rework>(self.inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem));
		}
		internal static float GetMissileDamage(int itemCount)
        {
			return BaseDamage + (StackDamage * (itemCount - 1));
        }
		internal static float GetMaxStock(int itemCount)
		{
			return BaseStock + (StackStock * (itemCount - 1));
		}
	}
}
