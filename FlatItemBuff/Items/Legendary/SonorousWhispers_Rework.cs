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
		//"RoR2/Base/ShadowClone/ShadowCloneEncounter.prefab"
		//"RoR2/Base/Titan/cscTitanDampCave.asset"
		//"RoR2/Base/RoboBallBoss/cscSuperRoboBallBoss.asset"
		internal static GameObject CombatEncounterObject = Addressables.LoadAssetAsync<GameObject>("21c30a52b0ea4074580b6528804b0efb").WaitForCompletion();
		internal static SpawnCard BaseSpawnCard = Addressables.LoadAssetAsync<SpawnCard>("5d95c9377c88031459b0e9d3c41c69c8").WaitForCompletion();
		internal static SpawnCard BossCardTemplate = Addressables.LoadAssetAsync<SpawnCard>("01d9753594cff56448b8326d4254c7e0").WaitForCompletion();
		private const string LogName = "Sonorous Whispers";
		internal static bool Enable = false;
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
			MainPlugin.ModLogger.LogInfo(LogName);
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
			//"RoR2/Base/golemplains/golemplains.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("8ba24a95e46b3944280a4b66afd0c4dc").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-78.95f, -142.39f, -6.6f));
			//"RoR2/Base/golemplains2/golemplains2.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("657a62eb3e4c409429c91ba6fdb7d921").WaitForCompletion());
			SceneSpawn.Add(new Vector3(238.17f, 48.71f, 34.07f));
			//"RoR2/Base/blackbeach/blackbeach.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("c129d2b9e62c31b4ba165091d1ae2c50").WaitForCompletion());
			SceneSpawn.Add(new Vector3(44.96f, -235.94f, -105.27f));
			//"RoR2/Base/blackbeach2/blackbeach2.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("f87198140bf9b5a4f82e206231df9091").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-163.82f, 5.44f, 22.86f));
			//"RoR2/Base/goolake/goolake.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("fd4d3738b6cbd8040bf47795cc2eb9e8").WaitForCompletion());
			SceneSpawn.Add(new Vector3(148.56f, -94.54f, -321.07f));
			//"RoR2/Base/foggyswamp/foggyswamp.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("759c255e5828e48478cd582df2897826").WaitForCompletion());
			SceneSpawn.Add(new Vector3(131.72f, -103.84f, -98.62f));
			//"RoR2/Base/frozenwall/frozenwall.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("4f4173fad59e5b24f98edbc1c7bf95a0").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-40.24f, 29.81f, 121.65f));
			//"RoR2/Base/wispgraveyard/wispgraveyard.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("32a76e99d300d0d4d86ce8f9f486512b").WaitForCompletion());
			SceneSpawn.Add(new Vector3(68.79f, 46.92f, -8.74f));
			//"RoR2/Base/rootjungle/rootjungle.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("ced489f798226594db0d115af2101a9b").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-102.25f, 44.77f, -91.5f));
			//"RoR2/Base/shipgraveyard/shipgraveyard.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("a4d138197f9eacb4eaa16e4df24108ef").WaitForCompletion());
			SceneSpawn.Add(new Vector3(111.93f, 5.27f, -98.25f));
			//"RoR2/Base/dampcavesimple/dampcavesimple.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("a0a19f82185253240951dc6b8750f67f").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-15.5f, -169.82f, -256.5f));
			//"RoR2/Base/skymeadow/skymeadow.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("e07c7a5799b2fe745a5bad477384ed7e").WaitForCompletion());
			SceneSpawn.Add(new Vector3(266.87f, 25f, -90.23f));
			//"RoR2/Base/moon2/moon2.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("5d69f3396feb7ba428d1e53a44479594").WaitForCompletion());
			SceneSpawn.Add(new Vector3(242.89f, -168.38f, 320.99f));
			//"RoR2/Base/arena/arena.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("a478a034d8da76244b2e1fb463ef1b81").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-120.8f, 64.11f, 64.05f));
			//"RoR2/Base/goldshores/goldshores.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("b8682562d8eb846458bd32ed9f6c8941").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-10.47f, -3.11f, -65.29f));
			//"RoR2/Base/moon/moon.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("7278d516dc482e24f932f0edd27fa3ff").WaitForCompletion());
			SceneSpawn.Add(new Vector3(1697.38f, 336.78f, 724.9f));

			//DLC1
			//"RoR2/DLC1/snowyforest/snowyforest.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("b46534a3026e7f844bde582a829d19f3").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-122.43f, 7.73f, 0f));
			//"RoR2/DLC1/ancientloft/ancientloft.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("8d0ca457a2e3a974796d21c4d5d920e8").WaitForCompletion());
			SceneSpawn.Add(new Vector3(164.46f, 23.43f, -29.2f));
			//"RoR2/DLC1/sulfurpools/sulfurpools.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("796f9b67682b3db4c8c5af7294d0490c").WaitForCompletion());
			SceneSpawn.Add(new Vector3(22.66f, 5.5f, 71.26f));
			//"RoR2/DLC1/voidstage/voidstage.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("66e0cfba315981a40afae481363ea0da").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-11.68f, 34.1f, -18.1f));
			//"RoR2/DLC1/voidraid/voidraid.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("223a0f0a86052654a9e473d13f77cb41").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-10.14f, 32.12f, -181.93f));

			//DLC2
			//"RoR2/DLC2/lakes/lakes.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("6de54e0cb8ad59d40a1682d5782a79f3").WaitForCompletion());
			SceneSpawn.Add(new Vector3(72.87f, 24.23f, -89.85f));
			//"RoR2/DLC2/lakesnight/lakesnight.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("bbe87fabcab391d4b86adf605c00cb50").WaitForCompletion());
			SceneSpawn.Add(new Vector3(72.87f, 24.23f, -89.85f));
			//"RoR2/DLC2/village/village.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("29a242327a19b2b43a53992d186471f7").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-1.85f, -19.62f, -95.11f));
			//"RoR2/DLC2/villagenight/villagenight.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("9baf31098cf5ff24ba2226cd73e7a5fc").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-1.85f, -19.62f, -95.11f));
			//"RoR2/DLC2/lemuriantemple/lemuriantemple.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("ec84d6383fdf1b945871e3925df0f5d3").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-9.4f, -14.64f, -102.72f));
			//"RoR2/DLC2/habitat/habitat.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("0b33829c6cdfad34a8160c6ae17edfcc").WaitForCompletion());
			SceneSpawn.Add(new Vector3(10.34f, 26.65f, -144.72f));
			//"RoR2/DLC2/habitatfall/habitatfall.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("962a6a8a56f584b468a4835c82c11951").WaitForCompletion());
			SceneSpawn.Add(new Vector3(17.89f, 30.27f, -144.15f));
			//"RoR2/DLC2/meridian/meridian.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("520b764e3ac5743459fd923204083395").WaitForCompletion());
			SceneSpawn.Add(new Vector3(102.19f, -134.6f, 227.29f));
			//"RoR2/DLC2/helminthroost/helminthroost.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("56cef57fbbd34d247bbd21a2c315db7d").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-708.69f, 15.35f, 282.79f));

			//DLC3
			//"RoR2/DLC3/nest/nest.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("211fd01a390c9904c9f822bd949311bc").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-105.72f, 65.88f, 13.5f));
			//"RoR2/DLC3/ironalluvium/ironalluvium.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("c44438fa966c1b44aab740d5eacdc612").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-3.55f, 74.42f, 57.4f));
			//"RoR2/DLC3/ironalluvium2/ironalluvium2.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("53756fbbd15268a489fb9f781fa3b200").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-3.55f, 74.42f, 57.4f));
			//"RoR2/DLC3/conduitcanyon/conduitcanyon.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("1f3622d7a6e49184cb1a3f5bc06ee223").WaitForCompletion());
			SceneSpawn.Add(new Vector3(95.64f, 117.05f, 127.71f));
			//"RoR2/DLC3/repurposedcrater/repurposedcrater.asset"
			SceneList.Add(Addressables.LoadAssetAsync<SceneDef>("c3b8b99631744564daf8944d1335901e").WaitForCompletion());
			SceneSpawn.Add(new Vector3(-154.12f, 6.45f, 77.15f));
		}
		private void UpdateItemDef()
		{
			//"RoR2/DLC2/Items/ItemDropChanceOnKill/ItemDropChanceOnKill.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("4838bba8e7fc5d6439e4ca0ab60a18bc").WaitForCompletion();
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
			MainPlugin.ModLogger.LogInfo("Updating Text");
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
			LanguageAPI.Add("ITEM_ITEMDROPCHANCEONKILL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_ITEMDROPCHANCEONKILL_DESC", description);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
			Run.onRunStartGlobal += OnRunStartGlobal;
			SharedHooks.Handle_GlobalKillEvent_Actions += GlobalKillEvent;
			SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			int itemCount = self.inventory.GetItemCountEffective(DLC2Content.Items.ItemDropChanceOnKill);
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
				x => x.MatchLdsfld(typeof(DLC2Content.Items), "ItemDropChanceOnKill")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnCharacterDeath - Hook failed");
			}
		}
	}
}
