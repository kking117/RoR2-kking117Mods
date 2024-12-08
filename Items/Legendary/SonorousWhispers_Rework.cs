using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
	public class SonorousWhispers_Rework
	{
		internal static GameObject CombatEncounterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShadowClone/ShadowCloneEncounter.prefab").WaitForCompletion();
		internal static SpawnCard BaseSpawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion();
		internal static SpawnCard BossCardTemplate = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/RoboBallBoss/cscSuperRoboBallBoss.asset").WaitForCompletion();
		internal static bool Enable = true;
		internal static bool HasAdaptive = false;
		internal static bool IsElite = true;
		internal static bool ScalePlayer = true;
		internal static bool GoodEnding = true;
		internal static float BasePower = 1f;
		internal static float StackPower = 0f;
		internal static float BaseDamage = 60f;
		internal static float StackDamage = 48f;
		internal static float BaseRadius = 100f;
		internal static float StackRadius = 20f;
		internal static int BaseReward = 1;
		internal static int StackReward = 1;
		internal static int BaseGold = 100;
		internal static int StackGold = 50;
		internal static int RewardLimit = 200;
		public static List<SceneDef> SceneList = new List<SceneDef>();
		public static List<Vector3> SceneSpawn = new List<Vector3>();

		public SonorousWhispers_Rework()
		{
			if (!Enable)
			{
				return;
			}
			MainPlugin.ModLogger.LogInfo("Changing Sonorous Whispers");
			ClampConfig();
			UpdateItemDef();
			CreateSpawnList();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BasePower = Math.Max(1f, BasePower);
			StackPower = Math.Max(0f, StackPower);
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseRadius = Math.Max(0f, BaseRadius);
			StackRadius = Math.Max(0f, StackRadius);
			BaseReward = Math.Max(0, BaseReward);
			StackReward = Math.Max(0, StackReward);
			RewardLimit = Math.Max(BaseReward, RewardLimit);
			BaseGold = Math.Max(0, BaseGold);
			StackGold = Math.Max(0, StackGold);
		}
		private void CreateSpawnList()
        {
			SceneList = new List<SceneDef>();
			SceneSpawn = new List<Vector3>();

			//ROR2

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/golemplains/golemplains.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-78.95f, -142.39f, -6.6f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/golemplains2/golemplains2.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(238.17f, 48.71f, 34.07f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/blackbeach/blackbeach.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(44.96f, -235.94f, -105.27f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/blackbeach2/blackbeach2.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-163.82f, 5.44f, 22.86f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/goolake/goolake.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(148.56f, -94.54f, -321.07f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/foggyswamp/foggyswamp.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(131.72f, -103.84f, -98.62f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/frozenwall/frozenwall.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-40.24f, 29.81f, 121.65f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/wispgraveyard/wispgraveyard.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(68.79f, 46.92f, -8.74f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/rootjungle/rootjungle.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-102.25f, 44.77f, -91.5f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/shipgraveyard/shipgraveyard.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(111.93f, 5.27f, -98.25f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/dampcavesimple/dampcavesimple.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-15.5f, -169.82f, -256.5f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/skymeadow/skymeadow.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(266.87f, 25f, -90.23f));
			
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/moon2/moon2.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(242.89f, -168.38f, 320.99f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-120.8f, 64.11f, 64.05f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/goldshores/goldshores.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-10.47f, -3.11f, -65.29f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/moon/moon.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(1697.38f, 336.78f, 724.9f));

			//DLC1

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/snowyforest/snowyforest.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-122.43f, 7.73f, 0f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/ancientloft/ancientloft.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(164.46f, 23.43f, -29.2f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/sulfurpools/sulfurpools.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(22.66f, 5.5f, 71.26f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidstage/voidstage.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-11.68f, 34.1f, -18.1f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidraid/voidraid.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-10.14f, 32.12f, -181.93f));

			//DLC2

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/lakes/lakes.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(72.87f, 24.23f, -89.85f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/lakesnight/lakesnight.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(72.87f, 24.23f, -89.85f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/village/village.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-1.85f, -19.62f, -95.11f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/villagenight/villagenight.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-1.85f, -19.62f, -95.11f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/lemuriantemple/lemuriantemple.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-9.4f, -14.64f, -102.72f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/habitat/habitat.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(10.34f, 26.65f, -144.72f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/habitatfall/habitatfall.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(17.89f, 30.27f, -144.15f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/meridian/meridian.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(102.19f, -134.6f, 227.29f));

			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC2/helminthroost/helminthroost.asset").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-708.69f, 15.35f, 282.79f));
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC2/Items/ResetChests/ResetChests.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Add(ItemTag.AIBlacklist);
				itemTags.Add(ItemTag.CannotCopy);
				itemTags.Add(ItemTag.OnStageBeginEffect);
				itemTags.Remove(ItemTag.OnKillEffect);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Summons a powerful monster each stage";
			string description = "";
			if (StackPower > 0f)
            {
				description += string.Format("<style=cIsUtility>On each stage</style> a <style=cIsHealth>powerful monster</style> is summoned with <style=cIsDamage>{0}%</style> <style=cStack>(+{1}% per stack)</style> <style=cIsDamage>scaling</style>.", BasePower * 100f, StackPower * 100f);
			}
			else
            {
				description += string.Format("<style=cIsUtility>On each stage</style> a <style=cIsHealth>powerful monster</style> is summoned with <style=cIsDamage>{0}%</style> <style=cIsDamage>scaling</style>.", BasePower * 100f);
			}
			if (BaseReward > 0)
            {
				pickup += ", drops its trophy upon defeat";
				if (StackReward > 0)
                {
					description += string.Format(" Defeating this monster will drop <style=cIsDamage>{0}</style> <style=cStack>(+{1} per stack)</style> <style=cIsDamage>unique rewards</style>", BaseReward, StackReward);
				}
				else
                {
					description += string.Format(" Defeating this monster will drop <style=cIsDamage>{0}</style> <style=cIsDamage>unique rewards</style>", BaseReward);
				}
				if (BaseGold < 1)
                {
					description += ".";
                }
			}
			if (BaseGold > 0)
			{
				if (BaseReward > 0)
                {
					if (StackGold > 0)
					{
						description += string.Format(" plus an additional <style=cIsUtility>{0}</style> <style=cStack>(+{1} per stack)</style> <style=cIsUtility>gold, scaling with time</style>.", BaseGold, StackGold);
					}
					else
					{
						description += string.Format(" plus an additional <style=cIsUtility>{0} gold, scaling with time</style>.", BaseGold);
					}
				}
				else
                {
					if (StackGold > 0)
					{
						description += string.Format(" Defeating this monster will drop <style=cIsUtility>{0}</style> <style=cStack>(+{1} per stack)</style> <style=cIsUtility>gold, scaling with time</style>.", BaseGold, StackGold);
					}
					else
					{
						description += string.Format(" Defeating this monster will drop <style=cIsUtility>{0} gold, scaling with time</style>.", BaseGold);
					}
					pickup += ", drops gold upon defeat";
				}
			}
			if (BaseRadius > 0f && BaseDamage > 0f)
			{
				string damage = "";
				if (StackDamage > 0f)
                {
					damage += string.Format(" for <style=cIsDamage>{0}%</style> <style=cStack>(+{1}% per stack)</style> <style=cIsDamage>ambient damage</style>.", BaseDamage * 100f, StackDamage * 100f);
				}
				else
                {
					damage += string.Format(" for <style=cIsDamage>{0}%</style> <style=cIsDamage>ambient damage</style>.", BaseDamage * 100f);
				}
				string radius = "";
				if (StackRadius > 0f)
				{
					radius += string.Format(" <style=cIsDamage>explodes</style> in a <style=cIsDamage>{0}m</style> <style=cStack>(+{1}m per stack)</style> radius", BaseRadius, StackRadius);
				}
				else
				{
					radius += string.Format(" <style=cIsDamage>explodes</style> in a <style=cIsDamage>{0}m</style> radius", BaseRadius);
				}
				description += string.Format("\n\nWhen defeated, the monster{0}{1}", radius, damage);
			}
			pickup += ".";
			LanguageAPI.Add("ITEM_RESETCHESTS_PICKUP", pickup);
			LanguageAPI.Add("ITEM_RESETCHESTS_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			Run.onRunStartGlobal += OnRunStartGlobal;
			SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCount = self.inventory.GetItemCount(DLC2Content.Items.ResetChests);
			if (itemCount > 0)
			{
				StartEchoSummon(Stage.instance);
			}
		}
		private void GlobalKillEvent(DamageReport damageReport)
		{
			CharacterMaster victimMaster = damageReport.victimMaster;
			CharacterBody victimBody = damageReport.victimBody;
			if (victimMaster && victimBody)
			{
				if (victimBody.isChampion)
                {
					DeathRewards deathRewards = victimBody.GetComponent<DeathRewards>();
					if (deathRewards && deathRewards.bossDropTable)
					{
						if (victimMaster.teamIndex != TeamIndex.Player)
						{
							CharacterMaster attackerMaster = damageReport.attackerMaster;
							if (!attackerMaster || attackerMaster != victimMaster)
							{
								Components.SonorousWhispersTracker comp = Run.instance.GetComponent<Components.SonorousWhispersTracker>();
								if (comp)
								{
									comp.IncrementKill(victimMaster);
								}
							}
						}
					}
				}
			}
		}
		private void OnRunStartGlobal(Run run)
		{
			Components.SonorousWhispersTracker comp = run.GetComponent<Components.SonorousWhispersTracker>();
			if (!comp)
			{
				run.gameObject.AddComponent<Components.SonorousWhispersTracker>();
			}
		}
		private void StartEchoSummon(Stage stage)
        {
			if (stage)
            {
				if (stage.sceneDef.sceneType == SceneType.Stage)
				{
					Components.SonorousWhispersEncounter comp = stage.GetComponent<Components.SonorousWhispersEncounter>();
					if (!comp)
					{
						stage.gameObject.AddComponent<Components.SonorousWhispersEncounter>();
					}
				}
			}
		}
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "ResetChests")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Sonorous Whispers - IL Hook failed");
			}
		}
	}
}
