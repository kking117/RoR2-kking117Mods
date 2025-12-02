using System;
using System.Collections.Generic;
using RoR2;
using R2API;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class NewlyHatchedZoea_Rework
	{
		public static GameObject VoidMissileProjectile;
		public static BuffDef VoidMissileStockBuff;
		private static Color BuffColor = new Color(0.682f, 0.415f, 0.725f, 1f);

		private const string LogName = "Newly Hatched Zoea Rework";
		internal static bool Enable = false;
		internal static int BaseStock = 12;
		internal static int StackStock = 4;
		internal static float BaseDamage = 8f;
		internal static float StackDamage = 6f;
		internal static float ProcRate = 0.2f;
		internal static float FireDelay = 0.2f;
		internal static int RestockTime = 30;
		internal static bool CanCorrupt = true;
		internal static string CorruptList = "";
		internal static string CorruptText = "<style=cIsTierBoss>yellow items</style>";
		public NewlyHatchedZoea_Rework()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			SharedHooks.Handle_PostLoad_Actions += UpdateText;
			CreateBuff();
			CreateProjectiles();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseStock = Math.Max(0, BaseStock);
			StackStock = Math.Max(0, StackStock);
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			ProcRate = Math.Max(0f, ProcRate);
			RestockTime = Math.Max(0, RestockTime);
			FireDelay = Math.Max(0f, FireDelay);
		}
		private void CreateBuff()
		{
			//"RoR2/Base/Merc/bdMercExpose.asset"
			VoidMissileStockBuff = Utils.ContentManager.AddBuff("Void Missile", Addressables.LoadAssetAsync<BuffDef>("6ce812f39acfaff4c8b586774cd0fb37").WaitForCompletion().iconSprite, BuffColor, true, false, false, false, false);
		}
		private void UpdateText()
		{
			string corruptsAll = "";
			if (CanCorrupt)
            {
				corruptsAll = string.Format(" <style=cIsVoid>Corrupts all {0}</style>.", CorruptText);
			}
			string pickup = string.Format("Activating your Special skill also unleashes a missile swarm. Recharges over time.{0}", corruptsAll);

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
			
			string desc = string.Format("Activating your <style=cIsUtility>Special skill</style> also fires a <style=cIsDamage>missile swarm</style> that deals <style=cIsDamage>{0}</style> base damage each. You can hold up to <style=cIsUtility>{1}</style> <style=cIsDamage>missiles</style> which all reload over <style=cIsUtility>{2}</style> seconds.{3}", damageText, stockText, RestockTime, corruptsAll);
			LanguageAPI.Add("ITEM_VOIDMEGACRABITEM_PICKUP", pickup);
			LanguageAPI.Add("ITEM_VOIDMEGACRABITEM_DESC", desc);
		}
		private void Hooks()
		{
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
			if (CanCorrupt)
			{
				On.RoR2.Items.ContagiousItemManager.Init += ContagiousItemManager_Init;
			}
			else
			{
				On.RoR2.Items.ContagiousItemManager.Init += ContagiousItemManager_Init_Remove;
			}
		}
		private void CreateProjectiles()
        {
			//"RoR2/DLC1/MissileVoid/MissileVoidProjectile.prefab"
			VoidMissileProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("e006990e46d3b7b4eb92dcebf62d2307").WaitForCompletion(), MainPlugin.MODTOKEN + "VoidMissile");
			ProjectileController projController = VoidMissileProjectile.GetComponent<ProjectileController>();
			MissileController projMissile = VoidMissileProjectile.GetComponent<MissileController>();
			projMissile.deathTimer = 10f; //Originally 3.5f
			projMissile.giveupTimer = 6f;
			projMissile.turbulence = 2f; //Originally 15
			projMissile.timer = -0.5f;
			projController.procCoefficient = ProcRate;
			Utils.ContentManager.AddProjectile(VoidMissileProjectile);
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<VoidMegaCrabItemBehavior>(0);
			self.AddItemBehavior<Behaviors.NewlyHatchedZoea_Rework>(self.inventory.GetItemCountEffective(DLC1Content.Items.VoidMegaCrabItem));
		}
		internal static float GetMissileDamage(int itemCount)
        {
			return BaseDamage + (StackDamage * (itemCount - 1));
        }
		internal static float GetMaxStock(int itemCount)
		{
			return BaseStock + (StackStock * (itemCount - 1));
		}

		private static void Setup_CustomItemRelationships()
        {
			List<ItemDef> corruptList = new List<ItemDef>();
			string[] items = CorruptList.Split(' ');
			for (int i = 0; i < items.Length; i++)
			{
				ItemIndex itemIndex = ItemCatalog.FindItemIndex(items[i]);
				if (itemIndex > ItemIndex.None)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					if (itemDef)
					{
						corruptList.Add(itemDef);
					}
				}
			}

			if (corruptList.Count > 0)
            {
				List<ItemDef.Pair> newList = new List<ItemDef.Pair>();
				foreach (ItemDef.Pair oldPair in ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem])
				{
					if (oldPair.itemDef2 != DLC1Content.Items.VoidMegaCrabItem)
					{
						newList.Add(oldPair);
					}
				}
				for (int i = 0; i < corruptList.Count; i++)
				{
					ItemDef.Pair newItem = new ItemDef.Pair
					{
						itemDef1 = corruptList[i],
						itemDef2 = DLC1Content.Items.VoidMegaCrabItem
					};
					newList.Add(newItem);
				}
				ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = newList.ToArray();
			}
		}
		internal static void ContagiousItemManager_Init(On.RoR2.Items.ContagiousItemManager.orig_Init orig)
		{
			List<ItemDef> corruptList = new List<ItemDef>();
			string[] items = CorruptList.Split(' ');
			for (int i = 0; i < items.Length; i++)
			{
				ItemIndex itemIndex = ItemCatalog.FindItemIndex(items[i]);
				if (itemIndex > ItemIndex.None)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					if (itemDef)
					{
						corruptList.Add(itemDef);
					}
				}
			}

			if (corruptList.Count > 0)
			{
				List<ItemDef.Pair> newList = new List<ItemDef.Pair>();
				foreach (ItemDef.Pair oldPair in ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem])
				{
					if (oldPair.itemDef2 != DLC1Content.Items.VoidMegaCrabItem)
					{
						newList.Add(oldPair);
					}
				}
				for (int i = 0; i < corruptList.Count; i++)
				{
					ItemDef.Pair newItem = new ItemDef.Pair
					{
						itemDef1 = corruptList[i],
						itemDef2 = DLC1Content.Items.VoidMegaCrabItem
					};
					newList.Add(newItem);
				}
				ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = newList.ToArray();
			}
			orig();
        }
		internal static void ContagiousItemManager_Init_Remove(On.RoR2.Items.ContagiousItemManager.orig_Init orig)
		{
			List<ItemDef.Pair> newList = new List<ItemDef.Pair>();
			foreach (ItemDef.Pair oldPair in ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem])
			{
				if (oldPair.itemDef2 != DLC1Content.Items.VoidMegaCrabItem)
				{
					newList.Add(oldPair);
				}
			}
			ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = newList.ToArray();
			orig();
		}

		//Old code for setting item relationships, keeping it here just in case
		/*internal static void ItemCatalog_SetItemRelationships(On.RoR2.ItemCatalog.orig_SetItemRelationships orig, ItemRelationshipProvider[] relationshipProvider)
		{
			List<ItemDef> corruptList = new List<ItemDef>();
			string[] items = CorruptList.Split(' ');
			for (int i = 0; i < items.Length; i++)
			{
				ItemIndex itemIndex = ItemCatalog.FindItemIndex(items[i]);
				if (itemIndex > ItemIndex.None)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					if (itemDef)
					{
						corruptList.Add(itemDef);
					}
				}
			}

			if (corruptList.Count > 0)
            {
				List<ItemRelationshipProvider> newProvider = new List<ItemRelationshipProvider>();
				foreach (ItemRelationshipProvider itemRelation in relationshipProvider)
				{
					if (itemRelation.relationshipType == DLC1Content.ItemRelationshipTypes.ContagiousItem)
					{
						List<ItemDef.Pair> newitempair = new List<ItemDef.Pair>();
						foreach (ItemDef.Pair itempair in itemRelation.relationships)
						{
							if (itempair.itemDef2 != DLC1Content.Items.VoidMegaCrabItem)
							{
								newitempair.Add(itempair);
							}
						}
						itemRelation.relationships = newitempair.ToArray();
					}
					newProvider.Add(itemRelation);
				}

				ItemRelationshipProvider newItemRelationship = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
				newItemRelationship.relationshipType = DLC1Content.ItemRelationshipTypes.ContagiousItem;
				newItemRelationship.relationships = new ItemDef.Pair[corruptList.Count];
				for (int i = 0; i < corruptList.Count; i++)
				{
					newItemRelationship.relationships[i] = new ItemDef.Pair
					{
						itemDef1 = corruptList[i],
						itemDef2 = DLC1Content.Items.VoidMegaCrabItem
					};
				}
				
				newProvider.Add(newItemRelationship);

				orig(newProvider.ToArray());
				return;
			}
			MainPlugin.ModLogger.LogInfo("No items found for custom corruption list, leaving corruption list unchanged.");
			orig(relationshipProvider);
		}

		internal static void ItemCatalog_SetItemRelationships_Remove(On.RoR2.ItemCatalog.orig_SetItemRelationships orig, ItemRelationshipProvider[] relationshipProvider)
		{
			List<ItemRelationshipProvider> newProvider = new List<ItemRelationshipProvider>();
			foreach (ItemRelationshipProvider itemRelation in relationshipProvider)
			{
				if (itemRelation.relationshipType == DLC1Content.ItemRelationshipTypes.ContagiousItem)
				{
					List<ItemDef.Pair> newitempair = new List<ItemDef.Pair>();
					foreach (ItemDef.Pair itempair in itemRelation.relationships)
					{
						if (itempair.itemDef2 != DLC1Content.Items.VoidMegaCrabItem)
						{
							newitempair.Add(itempair);
						}
					}
					itemRelation.relationships = newitempair.ToArray();
				}
				newProvider.Add(itemRelation);
			}
			orig(newProvider.ToArray());
		}*/
	}
}
