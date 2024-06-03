using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace VoidDamageConfig.Changes
{
	public class EffectA
	{
		internal static bool VoidFog_Enable = false;
		internal static bool VoidFog_OnlyMinion = true;
		internal static bool VoidFog_AllowArena = false;
		internal static bool VoidFog_AllowLocus = true;
		internal static int VoidFog_DamageType = 0;
		internal static bool VoidFog_NerfVoidLocus = false;

		internal static bool VoidDamage_Enable = false;
		internal static bool VoidDamage_OnlyMinion = true;
		internal static float VoidDamage_MaxPercent = 0.05f;
		internal static float VoidDamage_CurPercent = 0.45f;
		internal static bool VoidDamage_NonLethal = false;
		internal static float VoidDamage_Nullify_Duration = 3f;
		internal static bool VoidDamage_AllowOverride = false;
		internal static float[] VoidDamage_OverrideDamage;
		internal static string VoidDamage_OverrideDamage_Raw;
		private static DamageType VoidDamage_AddFlags = DamageType.BypassBlock|DamageType.BypassArmor;

		internal static bool Minion_AllowPlayer = false;
		internal static string Minion_BodyWhiteList_Raw;
		internal static bool[] Minion_BodyWhiteList;
		internal static string Minion_BodyBlackList_Raw;
		internal static bool[] Minion_BodyBlackList;
		internal static string Minion_ItemBlackList_Raw;
		internal static List<ItemIndex> Minion_ItemBlackList;
		public EffectA()
		{
			if (!VoidFog_Enable && !VoidDamage_Enable)
            {
				return; //the fuck
            }
			ClampConfig();
			On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
			if (VoidDamage_Enable)
			{
				if (VoidDamage_AllowOverride)
				{
					On.RoR2.HealthComponent.Suicide += OnSuicide;
				}
			}
			if (VoidFog_Enable)
			{
				if (VoidFog_AllowArena)
                {
					SceneChanges();
				}
				if (VoidFog_NerfVoidLocus)
				{
					On.RoR2.VoidStageMissionController.RequestFog += VoidLocusRequestFog;
				}
			}
		}
		internal static void PostLoad()
        {
			int totalBodyCount = BodyCatalog.bodyCount;
			VoidDamage_OverrideDamage = new float[totalBodyCount];
			for(int i = 0; i< VoidDamage_OverrideDamage.Length; i++)
            {
				VoidDamage_OverrideDamage[i] = 1f;
			}
			string[] listData = VoidDamage_OverrideDamage_Raw.Split(',');
			for (int i = 0; i+1 < listData.Length; i += 2)
            {
				listData[i] = listData[i].Trim();
				listData[i+1] = listData[i+1].Trim();
				BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(listData[i]);
				if (bodyIndex > BodyIndex.None)
                {
					float divider;
					bool isNumber = float.TryParse(listData[1], out divider);
					if (isNumber)
					{
						VoidDamage_OverrideDamage[(int)bodyIndex] = divider;
					}
				}
            }

			bool useFilter = false;
			Minion_BodyWhiteList = new bool[totalBodyCount];
			listData = Minion_BodyWhiteList_Raw.Split(',');
			for (int i = 0; i < listData.Length; i++)
			{
				BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(listData[i].Trim());
				if (bodyIndex > BodyIndex.None)
				{
					Minion_BodyWhiteList[(int)bodyIndex] = true;
					useFilter = true;
				}
			}
			if (!useFilter)
			{
				Minion_BodyWhiteList = null;
			}

			useFilter = false;
			Minion_BodyBlackList = new bool[totalBodyCount];
			listData = Minion_BodyBlackList_Raw.Split(',');
			for (int i = 0; i < listData.Length; i++)
			{
				BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(listData[i].Trim());
				if (bodyIndex > BodyIndex.None)
				{
					Minion_BodyBlackList[(int)bodyIndex] = true;
					useFilter = true;
				}
			}
			if (!useFilter)
			{
				Minion_BodyBlackList = null;
			}

			useFilter = false;
			Minion_ItemBlackList = new List<ItemIndex>();
			listData = Minion_ItemBlackList_Raw.Split(',');
			for (int i = 0; i < listData.Length; i++)
			{
				ItemIndex itemIndex = ItemCatalog.FindItemIndex(listData[i].Trim());
				if (itemIndex > ItemIndex.None)
				{
					if (!Minion_ItemBlackList.Contains(itemIndex))
                    {
						Minion_ItemBlackList.Add(itemIndex);
						useFilter = true;
					}
				}
			}
			if (!useFilter)
            {
				Minion_ItemBlackList = null;
			}
		}
		private void ClampConfig()
		{
			Math.Max(0f, VoidDamage_MaxPercent);
			Math.Max(0f, VoidDamage_CurPercent);
			if (VoidDamage_NonLethal)
            {
				VoidDamage_AddFlags |= DamageType.NonLethal;
			}
		}
		private void SceneChanges()
		{
			if (VoidFog_AllowArena)
			{
				SceneDef arenaDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
				arenaDef.suppressNpcEntry = false;
			}
			
			if (!VoidFog_AllowLocus)
            {
				SceneDef arenaDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidstage/voidstage.asset").WaitForCompletion();
				arenaDef.suppressNpcEntry = true;
			}
		}
		private VoidStageMissionController.FogRequest VoidLocusRequestFog(On.RoR2.VoidStageMissionController.orig_RequestFog orig, VoidStageMissionController self, IZone zone)
		{
			//From RiskyMod
			if (self.fogDamageController)
            {
				self.fogDamageController.healthFractionPerSecond = 0.025f;
				self.fogDamageController.healthFractionRampCoefficientPerSecond = 0f;
			}
			return orig(self, zone);
		}

		private void OnSuicide(On.RoR2.HealthComponent.orig_Suicide orig, HealthComponent self, GameObject killerOverride, GameObject inflictorOverride, DamageType damageType)
		{
			if (NetworkServer.active)
			{
				if ((damageType & DamageType.VoidDeath) != DamageType.Generic)
                {
					TeamComponent teamComp = self.body.teamComponent;
					if (teamComp && teamComp.teamIndex == TeamIndex.Player)
                    {
						DamageInfo damageInfo = new DamageInfo
						{
							damage = Run.instance.ambientLevel * 2.4f + 12f,
							damageType = damageType,
							position = self.transform.position,
							procCoefficient = 1f
						};
						if (killerOverride)
						{
							damageInfo.attacker = killerOverride;
						}
						if (inflictorOverride)
						{
							damageInfo.inflictor = inflictorOverride;
						}
						damageInfo = GetNewForcedVoidDamage(damageInfo);
						self.TakeDamage(damageInfo);
						return;
					}
                }
			}
			orig(self, killerOverride, inflictorOverride, damageType);
		}
		private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
			bool doNullify = false;
			if (NetworkServer.active)
            {
				int voidDamage = GetVoidDamageType(damageInfo);
				if (voidDamage > 0)
				{
					if (voidDamage == 1 && VoidFog_PassFilter(self.body))
                    {
						if (VoidFog_DamageType == 2)
						{
							damageInfo.damage = 0f;
							damageInfo.rejected = true;
						}
						else if (VoidFog_DamageType == 1)
						{
							damageInfo.damageType |= DamageType.NonLethal;
						}
					}
					else if (voidDamage == 2 && VoidDamage_PassFilter(self.body))
					{
						self.body.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
						damageInfo.damage = GetNewVoidDamage(self, damageInfo, voidDamage);
						damageInfo.damageType |= VoidDamage_AddFlags;
						doNullify = VoidDamage_Nullify_Duration > 0f;
					}
				}
            }
			orig(self, damageInfo);
			if (doNullify)
            {
				if (NetworkServer.active)
                {
					if (!damageInfo.rejected)
                    {
						CharacterBody victimBody = self.GetComponent<CharacterBody>();
						if (victimBody)
						{
							victimBody.AddTimedBuff(RoR2Content.Buffs.Nullified, VoidDamage_Nullify_Duration);
						}
					}
				}
			}
        }
		private bool VoidFog_PassFilter(CharacterBody body)
        {
			TeamComponent teamComp = body.teamComponent;
			CharacterMaster master = body.master;
			if (!teamComp)
			{
				return false;
			}
			if (teamComp.teamIndex == TeamIndex.Player)
			{
				if (!VoidFog_OnlyMinion)
				{
					return true;
				}
				if (master)
				{
					return PassFilter_ValidMinion(master, body);
				}
			}
			return false;
		}
		private bool VoidDamage_PassFilter(CharacterBody body)
		{
			TeamComponent teamComp = body.teamComponent;
			CharacterMaster master = body.master;
			if (!teamComp)
			{
				return false;
			}
			if (teamComp.teamIndex == TeamIndex.Player)
			{
				if (!VoidDamage_OnlyMinion)
				{
					return true;
				}
				if (master)
				{
					return PassFilter_ValidMinion(master, body);
				}
			}
			return false;
		}
		private bool PassFilter_ValidMinion(CharacterMaster master, CharacterBody body)
        {
			//Ignore if a player is controlling
			if (!Minion_AllowPlayer && body.isPlayerControlled)
            {
				return false;
			}
			//Body Filter
			int bodyIndex = (int)body.bodyIndex;
			if (Minion_BodyWhiteList != null)
			{
				if (Minion_BodyWhiteList[bodyIndex])
				{
					return true;
				}
			}
			if (Minion_BodyBlackList != null)
            {
				if (Minion_BodyBlackList[bodyIndex])
                {
					return false;
                }
			}
			if (Minion_ItemBlackList != null)
            {
				Inventory inventory = master.inventory;
				if (PassItemFilter(inventory))
                {
					return false;
				}
			}
			//Ignore if no ownerShip or no master.
			MinionOwnership ownerShip = master.GetComponent<MinionOwnership>();
			if (!ownerShip || !ownerShip.ownerMaster)
			{
				return false;
			}
			//Ignore if they own theirself
			if (master == ownerShip.ownerMaster)
			{
				return false;
			}
			return true;
        }
		private bool PassItemFilter(Inventory inventory)
        {
			for(int i = 0; i< Minion_ItemBlackList.Count; i++)
            {
				if (inventory.GetItemCount(Minion_ItemBlackList.ElementAt(i)) > 0)
				{
					return true;
				}
			}
			return false;
        }
		private int GetVoidDamageType(DamageInfo damageInfo)
        {
			if (damageInfo.damageColorIndex == DamageColorIndex.Void)
            {
				if (VoidFog_Enable)
                {
					if (damageInfo.damageType == (DamageType.BypassArmor | DamageType.BypassBlock))
					{
						if (!damageInfo.attacker & !damageInfo.inflictor)
						{
							return 1;
						}
					}
				}
			}
			else
            {
				if (VoidDamage_Enable)
                {
					if ((damageInfo.damageType & DamageType.VoidDeath) != DamageType.Generic)
					{
						return 2;
					}
				}
			}
			return 0;
        }

		private float GetNewVoidDamage(HealthComponent victimHP, DamageInfo damageInfo, int voidDamage)
        {
			float dmgMult = damageInfo.procCoefficient;
			if (dmgMult <= 0f)
            {
				dmgMult = 1f;
			}
			float newDamage = victimHP.combinedHealth * VoidDamage_CurPercent * dmgMult;
			newDamage += victimHP.fullCombinedHealth * VoidDamage_MaxPercent * dmgMult;
			float oldDamage = damageInfo.damage * dmgMult;
			return newDamage > oldDamage ? newDamage : oldDamage;
        }

		private DamageInfo GetNewForcedVoidDamage(DamageInfo damageInfo)
        {
			if (damageInfo.attacker)
            {
				CharacterBody killerBody = damageInfo.attacker.GetComponent<CharacterBody>();
				if (killerBody)
                {
					BodyIndex bodyIndex = killerBody.bodyIndex;
					if (bodyIndex > BodyIndex.None && (int)bodyIndex < VoidDamage_OverrideDamage.Length)
					{
						damageInfo.procCoefficient /= VoidDamage_OverrideDamage[(int)bodyIndex];
					}
					damageInfo.damage = killerBody.damage * 1f;
				}
			}
			return damageInfo;
        }
	}
}
