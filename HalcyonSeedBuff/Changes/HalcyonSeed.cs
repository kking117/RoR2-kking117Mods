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
		private static float ValidateTimer = -1f;
		private static Vector3 MoonPosition = new Vector3(-87.4f, 491.95f, -2.54f);
		private static Vector3 MeridianPosition = new Vector3(92.7f, 152.4f, -143.5f);
		private static Vector3 SpawnLocation;
		private static MasterCatalog.MasterIndex VoidRaidCrabIndex = MasterCatalog.MasterIndex.none;
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
			if (MainPlugin.ChannelOn_Focus)
			{
				//Simulacrum spawning
				On.RoR2.InfiniteTowerRun.OnSafeWardActivated += InfiniteTowerRun_OnSafeWardActivated;
				On.RoR2.InfiniteTowerRun.MoveSafeWard += InfiniteTowerRun_MoveSafeWard;
				On.RoR2.InfiniteTowerRun.OnWaveAllEnemiesDefeatedServer += InfiniteTowerRun_OnWaveFinish;
			}
			if (MainPlugin.ChannelOn_VoidRaid)
			{
				//Spawn Aurelionite on Voidling phase transition
				On.RoR2.VoidRaidGauntletController.CallRpcActivateDonut += VoidRaidGauntlet_CallRpcActivateDonut;
				//First Voidling spawn
				//TODO: Figure out a better/foolproof way to get the first Voidling that spawns.
				On.RoR2.CharacterMaster.Start += CharacterMaster_Start;
			}
			if (MainPlugin.ChannelOn_MoonPhase != 4)
			{
				//Prevents Aurelionite from spawning as usual after Mithrix steals your items.
				On.RoR2.GoldTitanManager.OnBossGroupStartServer += GoldTitanManager_OnBossGroupStartServer;
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
				On.EntityStates.BrotherMonster.SpellChannelExitState.OnEnter += OnItemSteal_End;
				//Validate Aurelionite spawns after giving the Halcyon Seed back.
				On.RoR2.ItemStealController.StolenInventoryInfo.TakeItemFromLendee += (orig, self, item, stacks) =>
				{
					if (item == RoR2Content.Items.TitanGoldDuringTP.itemIndex)
					{
						ValidateTimer = 0.1f;
					}
					return orig(self, item, stacks);
				};
			}
			//This is to kill Aurelionite before the item steal phase.
			On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Moon_Phase4;
			//This is to kill Aurelionite after the Moon boss is finished.
			On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += OnEncounterFinish;
			//This is for spawning Aurelionite with a delay + some other related stuff
			On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
			//Clear timers
			On.RoR2.Run.BeginStage += Run_BeginStage;
			//Get the masterindex for Voidling and Mithrix
			On.RoR2.MasterCatalog.Init += MasterCatalog_Init;
			//Override team and power
			On.RoR2.GoldTitanManager.CalcTitanPowerAndBestTeam += CalcTitantPowerAndTeam;
		}
		private static void MasterCatalog_Init(On.RoR2.MasterCatalog.orig_Init orig)
		{
			orig();
			MithrixIndex = MasterCatalog.FindMasterIndex("BrotherHurtMaster");
			VoidRaidCrabIndex = MasterCatalog.FindMasterIndex("MiniVoidRaidCrabMasterPhase1");

			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonMonsterMaster"));
			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonBossMaster"));
			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonBossLunarShardMaster"));
			FalseSonIndices.Add(MasterCatalog.FindMasterIndex("FalseSonBossLunarShardBrokenMaster"));

			FalseSonBodies.Add(BodyCatalog.FindBodyIndex("FalseSonBody"));
			FalseSonBodies.Add(BodyCatalog.FindBodyIndex("FalseSonBossBody"));
			FalseSonBodies.Add(BodyCatalog.FindBodyIndex("FalseSonBossBodyLunarShard"));
			FalseSonBodies.Add(BodyCatalog.FindBodyIndex("FalseSonBossBodyBrokenLunarShard"));
		}
		private static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
		{
			orig(self);
			SummonTimer = -1f;
			DisbandTimer = -1f;
			ValidateTimer = -1f;
		}
		private static void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, Run self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				if (DisbandTimer > 0f)
				{
					SummonTimer = 0f;
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
				
				if (ValidateTimer > 0f)
				{
					ValidateTimer -= Time.fixedDeltaTime;
					if (ValidateTimer <= 0f)
					{
						KillInvalidTitans();
					}
				}
			}
		}

		private static void CalcTitantPowerAndTeam(On.RoR2.GoldTitanManager.orig_CalcTitanPowerAndBestTeam orig, out int returnItemCount, out TeamIndex returnTeamIndex)
		{
			bool countFalseSon = false;
			if (MainPlugin.FalseSon_PlayerLoyal || MainPlugin.FalseSon_BossLoyal)
            {
				countFalseSon = true;
			}
			int bestItemCount = 0;
			TeamIndex itemTeam = TeamIndex.Neutral;
			TeamIndex playerTeam = TeamIndex.Neutral;
			TeamIndex bossTeam = TeamIndex.Neutral;
			int playerSon = 0;
			int bossSon = 0;
			for (int iTeam = 0; iTeam < (int)TeamIndex.Count; iTeam += 1)
			{
				int itemCountTemp = Util.GetItemCountForTeam((TeamIndex)iTeam, GoldTitanManager.goldTitanItemIndex, true, true);
				if (itemCountTemp > bestItemCount)
				{
					bestItemCount = itemCountTemp;
					itemTeam = (TeamIndex)iTeam;
				}

				if (countFalseSon)
				{
					int playerSonTemp = 0;
					int bossSonTemp = 0;
					foreach (TeamComponent comp in TeamComponent.GetTeamMembers((TeamIndex)iTeam))
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
					if (playerSonTemp > playerSon)
					{
						playerSon = playerSonTemp;
						playerTeam = (TeamIndex)iTeam;
					}
					if (bossSonTemp > bossSon)
					{
						bossSon = bossSonTemp;
						bossTeam = (TeamIndex)iTeam;
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
			GoldTitanManager.isFalseSonBossLunarShardBrokenMaster = false;
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
		private static void KillInvalidTitans()
		{
			List<CharacterMaster> titans = GoldTitanManager.currentTitans;
			for (TeamIndex team = (TeamIndex)0; team < TeamIndex.Count; team++)
			{
				if (Util.GetItemCountForTeam(team, RoR2Content.Items.TitanGoldDuringTP.itemIndex, true, true) < 1)
				{
					for (int i = 0; i < GoldTitanManager.currentTitans.Count; i++)
					{
						if (GoldTitanManager.currentTitans[i] && GoldTitanManager.currentTitans[i].GetBody())
						{
							if (GoldTitanManager.currentTitans[i].teamIndex == team)
							{
								GoldTitanManager.currentTitans[i].TrueKill();
							}
						}
					}
				}
			}
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
			}
			if (!IsMithrix && !isFalseSon)
			{
				orig(self);
			}
		}

		private static void CharacterMaster_Start(On.RoR2.CharacterMaster.orig_Start orig, CharacterMaster self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				if (self.isBoss && self.teamIndex == TeamIndex.Void)
				{
					if(self.masterIndex == VoidRaidCrabIndex)
                    {
						VoidRaidGauntletController VoidRaidGauntlet = VoidRaidGauntletController.instance;
						if (VoidRaidGauntlet)
						{
							SummonTimer = 9f;
							SpawnLocation = VoidRaidGauntlet.initialDonut.crabPosition.position;
						}
					}
				}
			}
		}
		private static void VoidRaidGauntlet_CallRpcActivateDonut(On.RoR2.VoidRaidGauntletController.orig_CallRpcActivateDonut orig, VoidRaidGauntletController self, int donutIndex)
		{
			orig(self, donutIndex);
			if (NetworkServer.active)
			{
				//We have to summon Aurelionite in advanced because there's no way to check if a player has gone through a portal on the server-side.
				DisbandTimer = 1f;
				SummonTimer = 20f;
				SpawnLocation = self.currentDonut.crabPosition.position + (Vector3.up * 10f);
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
				SummonTimer = 2f;
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
				DisbandTimer = 3f;
			}
		}
		private static void OnItemSteal_End(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnEnter orig, EntityStates.BrotherMonster.SpellChannelExitState self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 1f;
				SpawnLocation = self.characterBody.footPosition;
			}
		}
		private static void OnEncounterFinish(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				DisbandTimer = 3f;
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
				DisbandTimer = 2f;
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
				ilcursor.Index += 3;
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<TeamIndex>>(() =>
				{
					TeamIndex team;
					int itemCount;
					GoldTitanManager.CalcTitanPowerAndBestTeam(out itemCount, out team);
					return team;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": Enforce Team - IL Hook failed");
			}
		}
	}
}
