using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HG;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace HalcyonSeedBuff.Changes
{
	public class HalcyonSeed
	{
		private static float SummonTimer = -1f;
		private static float DisbandTimer = -1f;
		private static float BodyRadiusMult = 8f;
		private static Vector3 BodyOffset = new Vector3(1f, 0f, 0f);
		private static Vector3 MoonPosition = new Vector3(-87.4f, 491.95f, -2.54f);
		private static Vector3 MeridianPosition = new Vector3(92.7f, 152.4f, -143.5f);
		private static Vector3 SpawnLocation;
		private static List<MasterCatalog.MasterIndex> VoidlingIndices = new List<MasterCatalog.MasterIndex>();
		private static MasterCatalog.MasterIndex MithrixIndex = MasterCatalog.MasterIndex.none;
		private static List<BodyIndex> FalseSonBodies = new List<BodyIndex>();
		private static List<MasterCatalog.MasterIndex> FalseSonIndices = new List<MasterCatalog.MasterIndex>();
		public static void EnableChanges()
		{
			ClampConfig();
			Hooks();
		}
		private static void ClampConfig()
        {
			MainPlugin.Halcyon_ItemMult = Math.Max(1, MainPlugin.Halcyon_ItemMult);
			MainPlugin.ChannelOn_MeridianPhase = Math.Clamp(MainPlugin.ChannelOn_MeridianPhase, 0, 3);
		}
		private static void EditTags()
		{
			ItemDef TitanGoldDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/TitanGoldDuringTP/TitanGoldDuringTP.asset").WaitForCompletion();
			if (TitanGoldDef)
			{
				List<ItemTag> NewTags = TitanGoldDef.tags.ToList<ItemTag>();
				for (int i = 0; i < NewTags.Count; i++)
				{
					if (NewTags[i] == ItemTag.CannotSteal)
					{
						NewTags.RemoveAt(i);
						i--;
					}
				}
				TitanGoldDef.tags = NewTags.ToArray();
			}
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GoldTitanManager.TryStartChannelingTitansServer += new ILContext.Manipulator(IL_TrySpawnTitan);
			On.RoR2.GoldTitanManager.OnBossGroupStartServer += GoldTitanManager_OnBossGroupStartServer;
			if (MainPlugin.ChannelOn_Focus)
			{
				//Simulacrum spawning
				On.RoR2.InfiniteTowerRun.OnSafeWardActivated += InfiniteTowerRun_OnSafeWardActivated;
				On.RoR2.InfiniteTowerRun.MoveSafeWard += InfiniteTowerRun_MoveSafeWard;
				On.RoR2.InfiniteTowerRun.OnWaveAllEnemiesDefeatedServer += InfiniteTowerRun_OnWaveFinish;
			}
			if (MainPlugin.ChannelOn_MoonPhase != 4)
			{
				//This is so we can spawn Aurelionite during these phases.
				switch (MainPlugin.ChannelOn_MoonPhase)
				{
					case 1:
						On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Moon_Phase1;
						break;
					case 2:
						On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Moon_Phase2;
						break;
					case 3:
						On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Moon_Phase3;
						break;
				}
			}
			if (MainPlugin.ChannelOn_MeridianPhase != 0)
            {
				switch (MainPlugin.ChannelOn_MeridianPhase)
				{
					case 1:
						On.RoR2.MeridianEventTriggerInteraction.Phase1.OnEnter += Meridian_Phase1;
						break;
					case 2:
						On.RoR2.MeridianEventTriggerInteraction.Phase2.OnEnter += Meridian_Phase2;
						break;
					case 3:
						On.RoR2.MeridianEventTriggerInteraction.Phase3.OnEnter += Meridian_Phase3;
						break;
				}
				On.RoR2.MeridianEventTriggerInteraction.Phase3.OnExit += Meridian_Finish;
			}
			if (MainPlugin.Halcyon_CanBeStolen)
			{
				//Make it possible to steal the Halcyon Seed.
				EditTags();
				//Make the game channel Aurelionite after stealing items.
				On.EntityStates.BrotherMonster.SpellChannelEnterState.OnEnter += OnItemSteal_Enter;
				On.EntityStates.BrotherMonster.SpellChannelExitState.OnExit += OnItemSteal_End;
			}
			else
            {
				//Channel Aurelionite on Phase 4 unless it can be stolen
				if (MainPlugin.ChannelOn_MoonPhase == 4)
                {
					On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Moon_Phase4;
				}
			}
			//This is to kill Aurelionite after the Moon boss is finished.
			On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += OnEncounterFinish;
			//Spawn Timing
			On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
			On.RoR2.Run.BeginStage += Run_BeginStage;
			//Recording Indexes for important Characters
			On.RoR2.MasterCatalog.Init += MasterCatalog_Init;
			//Override team and power
			On.RoR2.GoldTitanManager.CalcTitanPowerAndBestTeam += CalcTitanPowerAndTeam;
		}
		private static void MasterCatalog_Init(On.RoR2.MasterCatalog.orig_Init orig)
		{
			orig();
			MithrixIndex = MasterCatalog.FindMasterIndex("BrotherHurtMaster");

			VoidlingIndices.Add(MasterCatalog.FindMasterIndex("MiniVoidRaidCrabMasterPhase1"));
			VoidlingIndices.Add(MasterCatalog.FindMasterIndex("MiniVoidRaidCrabMasterPhase2"));
			VoidlingIndices.Add(MasterCatalog.FindMasterIndex("MiniVoidRaidCrabMasterPhase3"));

			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonBossMaster"));
			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonBossLunarShardMaster"));
			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonBossLunarShardBrokenMaster"));

			string[] bodyNames = MainPlugin.FalseSon_BodyList.Split(',');
			for (int i = 0; i < bodyNames.Length; i++)
			{
				BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyNames[i].Trim());
				if (bodyIndex > BodyIndex.None)
				{
					FalseSonBodies.Add(bodyIndex);
				}
				else
                {
					UnityEngine.Debug.LogWarning(MainPlugin.MODNAME + ": Could not find body '" + bodyNames[i] + "'");
				}
			}
		}
		private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
		{
			orig(self);
			SummonTimer = -1f;
			DisbandTimer = -1f;
		}
		private static void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, Run self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				if (DisbandTimer > 0f)
				{
					DisbandTimer -= Time.fixedDeltaTime;
					if (DisbandTimer <= 0f)
					{
						GoldTitanManager.KillTitansInList(GoldTitanManager.currentTitans);
					}
				}
				if (SummonTimer > 0f)
				{
					SummonTimer -= Time.fixedDeltaTime;
					if (SummonTimer <= 0f)
					{
						if (GoldTitanManager.currentTitans.Count > -1)
						{
							GoldTitanManager.TryStartChannelingTitansServer(self, SpawnLocation, null, null);
						}
					}
				}
			}
		}

		private static void CalcTitanPowerAndTeam(On.RoR2.GoldTitanManager.orig_CalcTitanPowerAndBestTeam orig, out int returnItemCount, out TeamIndex returnTeamIndex)
		{
			GoldTitanManager.isFalseSonBossLunarShardBrokenMaster = false;
			bool countFalseSon = false;
			if (MainPlugin.FalseSon_PlayerLoyal || MainPlugin.FalseSon_BossLoyal)
            {
				countFalseSon = true;
			}
			int bestItemCount = 0;
			TeamIndex itemTeam = TeamIndex.Player;
			TeamIndex playerTeam = TeamIndex.Neutral;
			TeamIndex bossTeam = TeamIndex.Neutral;
			int playerSon = 0;
			int bossSon = 0;
			for (TeamIndex iTeam = TeamIndex.Neutral; iTeam < TeamIndex.Count; iTeam ++)
			{
				int itemCountTemp = Util.GetItemCountForTeam(iTeam, GoldTitanManager.goldTitanItemIndex, true, true);
				if (itemTeam == TeamIndex.Player)
                {
					if (itemCountTemp >= bestItemCount)
					{
						bestItemCount = itemCountTemp;
						itemTeam = iTeam;
					}
				}
				else
                {
					if (itemCountTemp > bestItemCount)
					{
						bestItemCount = itemCountTemp;
						itemTeam = iTeam;
					}
				}
				
				if (countFalseSon)
				{
					int playerSonTemp = 0;
					int bossSonTemp = 0;
					foreach (TeamComponent comp in TeamComponent.GetTeamMembers(iTeam))
					{
						CharacterBody ibody = comp.body;
						if (ibody)
						{
							CharacterMaster imaster = ibody.master;
							if (imaster)
                            {
								if (FalseSonBodies.Count > 0 && FalseSonBodies.Contains(ibody.bodyIndex))
                                {
									if (ibody.isPlayerControlled)
									{
										playerSonTemp += 1;
									}
									else
									{
										bossSonTemp += 1;
									}
								}
							}
						}
					}
					if (iTeam == TeamIndex.Player)
                    {
						if (playerSonTemp > 0)
						{
							playerSonTemp += 1;
						}
						if (bossSonTemp > 0)
						{
							bossSonTemp += 1;
						}
					}
					if (playerSonTemp > playerSon)
					{
						playerSon = playerSonTemp;
						playerTeam = iTeam;
					}
					if (bossSonTemp > bossSon)
					{
						bossSon = bossSonTemp;
						bossTeam = iTeam;
					}
					//MainPlugin.ModLogger.LogInfo("Found " + (playerSonTemp + bossSonTemp) + " False Sons on team " + (TeamIndex)iTeam);
				}
			}
			if (MainPlugin.FalseSon_BossLoyal)
			{
				if (bossSon > 0)
				{
					itemTeam = bossTeam;
				}
			}
			if (MainPlugin.FalseSon_PlayerLoyal)
			{
				if (playerSon > 0 && playerSon >= bossSon)
				{
					itemTeam = playerTeam;
				}
			}
			if (bestItemCount > 0)
			{
				if (itemTeam != TeamIndex.Player)
				{
					bestItemCount = 1;
				}
				else
				{
					bestItemCount *= MainPlugin.Halcyon_ItemMult;
				}
			}
			//MainPlugin.ModLogger.LogInfo("Forced Team = " + itemTeam);
			//MainPlugin.ModLogger.LogInfo("Item Amount = " + bestItemCount);
			returnItemCount = bestItemCount;
			returnTeamIndex = itemTeam;
		}
		private static void GoldTitanManager_OnBossGroupStartServer(On.RoR2.GoldTitanManager.orig_OnBossGroupStartServer orig, BossGroup self)
		{
			bool IsMithrix = false;
			bool isFalseSon = false;
			CombatSquad combatSquad = self.combatSquad;
			foreach (CharacterMaster characterMaster in combatSquad.readOnlyMembersList)
			{
				if (characterMaster.masterIndex == MithrixIndex)
				{
					IsMithrix = true;
					break;
				}
				if (FalseSonIndices.Count > 0 && FalseSonIndices.Contains(characterMaster.masterIndex))
				{
					isFalseSon = true;
					break;
				}
				//Channel Aurelionite if Voidling spawns in a boss group while VoidRaidGauntletController exists.
				if (MainPlugin.ChannelOn_VoidRaid)
                {
					VoidRaidGauntletController VoidRaidGauntlet = VoidRaidGauntletController.instance;
					if (VoidRaidGauntlet)
                    {
						if (VoidlingIndices.Count > 0 && VoidlingIndices.Contains(characterMaster.masterIndex))
						{
							DisbandTimer = 1f;
							SummonTimer = 10f;
							SpawnLocation = VoidRaidGauntlet.currentDonut.crabPosition.position;
						}
					}
				}
			}
			if (!IsMithrix && !isFalseSon)
			{
				orig(self);
			}
		}
		private static void Moon_Phase1(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 5.5f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void Moon_Phase2(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 3f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void Moon_Phase3(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 6f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void Moon_Phase4(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 10f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void OnItemSteal_Enter(On.EntityStates.BrotherMonster.SpellChannelEnterState.orig_OnEnter orig, EntityStates.BrotherMonster.SpellChannelEnterState self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				GoldTitanManager.KillTitansInList(GoldTitanManager.currentTitans);
				DisbandTimer = -1f;
				SummonTimer = -1f;
			}
		}
		private static void OnItemSteal_End(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnExit orig, EntityStates.BrotherMonster.SpellChannelExitState self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 1f;
				SpawnLocation = self.characterBody.footPosition + ((self.characterBody.radius + BodyRadiusMult) * BodyOffset);
			}
		}
		private static void OnEncounterFinish(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				DisbandTimer = 3f;
				SummonTimer = -1f;
			}
		}
		private static void Meridian_Phase1(On.RoR2.MeridianEventTriggerInteraction.Phase1.orig_OnEnter orig, MeridianEventTriggerInteraction.Phase1 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 9f;
				SpawnLocation = MeridianPosition;
			}
		}
		private static void Meridian_Phase2(On.RoR2.MeridianEventTriggerInteraction.Phase2.orig_OnEnter orig, MeridianEventTriggerInteraction.Phase2 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 3f;
				SpawnLocation = MeridianPosition;
			}
		}
		private static void Meridian_Phase3(On.RoR2.MeridianEventTriggerInteraction.Phase3.orig_OnEnter orig, MeridianEventTriggerInteraction.Phase3 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 6f;
				SpawnLocation = MeridianPosition;
			}
		}

		private static void Meridian_Finish(On.RoR2.MeridianEventTriggerInteraction.Phase3.orig_OnExit orig, MeridianEventTriggerInteraction.Phase3 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				DisbandTimer = 0.1f;
				SummonTimer = -1f;
			}
		}
		private static void InfiniteTowerRun_OnSafeWardActivated(On.RoR2.InfiniteTowerRun.orig_OnSafeWardActivated orig, InfiniteTowerRun self, InfiniteTowerSafeWardController safeWard)
		{
			orig(self, safeWard);
			if (NetworkServer.active)
			{
				if (safeWard)
				{
					SummonTimer = 3f;
					SpawnLocation = safeWard.transform.position;
				}
			}
		}
		private static void InfiniteTowerRun_OnWaveFinish(On.RoR2.InfiniteTowerRun.orig_OnWaveAllEnemiesDefeatedServer orig, InfiniteTowerRun self, InfiniteTowerWaveController wc)
        {
			orig(self, wc);
			if (NetworkServer.active)
			{
				if (self.IsStageTransitionWave())
				{
					DisbandTimer = 3f;
				}
			}
        }
		private static void InfiniteTowerRun_MoveSafeWard(On.RoR2.InfiniteTowerRun.orig_MoveSafeWard orig, InfiniteTowerRun self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				DisbandTimer = 3f;
				SummonTimer = -1f;
			}
		}
		private static void IL_TrySpawnTitan(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcI4(1),
				x => x.MatchStfld(typeof(RoR2.DirectorSpawnRequest), "ignoreTeamMemberLimit"),
				x => x.MatchLdloc(4),
				x => x.MatchLdcI4(1)
			))
			{
				//Finds: directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Player);
				//Then replaces the equals with the teamIndex variable the function created before.
				ilcursor.Index += 3;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldloc, 2);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Enforce Team - IL Hook failed");
			}
		}
	}
}
