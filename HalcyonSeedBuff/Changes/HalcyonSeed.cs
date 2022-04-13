using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
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
		private static Vector3 SpawnLocation;
		private static MasterCatalog.MasterIndex VoidRaidCrabIndex = MasterCatalog.MasterIndex.none;
		private static MasterCatalog.MasterIndex MithrixIndex = MasterCatalog.MasterIndex.none;
		public static void EnableChanges()
		{
			Hooks();
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
			if (MainPlugin.Config_Halcyon_ItemMult.Value != 1)
			{
				IL.RoR2.GoldTitanManager.CalcTitanPowerAndBestTeam += new ILContext.Manipulator(IL_CalcTitanPowerAndBestTeam);
				IL.RoR2.GoldTitanManager.TryStartChannelingTitansServer += new ILContext.Manipulator(IL_TrySpawnTitan);
			}
			if (MainPlugin.Config_Halcyon_ChannelOnFocus.Value)
			{
				//Simulacrum spawning
				On.RoR2.InfiniteTowerRun.OnSafeWardActivated += InfiniteTowerRun_OnSafeWardActivated;
				On.RoR2.InfiniteTowerRun.MoveSafeWard += InfiniteTowerRun_MoveSafeWard;
				On.RoR2.InfiniteTowerRun.OnWaveAllEnemiesDefeatedServer += InfiniteTowerRun_OnWaveFinish;
			}
			if (MainPlugin.Config_Halcyon_ChannelOnVoidRaid.Value)
			{
				//Spawn Aurelionite on Voidling phase transition
				On.RoR2.VoidRaidGauntletController.CallRpcActivateDonut += VoidRaidGauntlet_CallRpcActivateDonut;
				//First Voidling spawn
				//TODO: Figure out a better/foolproof way to get the first Voidling that spawns.
				On.RoR2.CharacterMaster.Start += CharacterMaster_Start;
			}
			if (MainPlugin.Config_Halcyon_ChannelOnMoonPhase.Value != 4)
			{
				//Prevents Aurelionite from spawning as usual after Mithrix steals your items.
				On.RoR2.GoldTitanManager.OnBossGroupStartServer += GoldTitanManager_OnBossGroupStartServer;
				//This is so we can spawn Aurelionite during these phases.
				switch (MainPlugin.Config_Halcyon_ChannelOnMoonPhase.Value)
				{
					case 1:
						On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += OnPhase1;
						break;
					case 2:
						On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += OnPhase2;
						break;
					case 3:
						On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += OnPhase3;
						break;
				}
			}
			if (MainPlugin.Config_Halcyon_CanBeStolen.Value)
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
			On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += OnPhase4;
			//This is to kill Aurelionite after the Moon boss is finished.
			On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += OnEncounterFinish;
			//This is for spawning Aurelionite with a delay + some other related stuff
			On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
			//Clear timers
			On.RoR2.Run.BeginStage += Run_BeginStage;
			//Get the masterindex for Voidling and Mithrix
			On.RoR2.MasterCatalog.Init += MasterCatalog_Init;
		}
		private static void MasterCatalog_Init(On.RoR2.MasterCatalog.orig_Init orig)
		{
			orig();
			MithrixIndex = MasterCatalog.FindMasterIndex("BrotherHurtMaster");
			VoidRaidCrabIndex = MasterCatalog.FindMasterIndex("MiniVoidRaidCrabMasterPhase1");
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
				if (DisbandTimer > 0f)
				{
					DisbandTimer -= Time.fixedDeltaTime;
					if (DisbandTimer <= 0f)
					{
						GoldTitanManager.KillTitansInList(GoldTitanManager.currentTitans);
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
			CombatSquad combatSquad = self.combatSquad;
			using (IEnumerator<CharacterMaster> enumerator = combatSquad.readOnlyMembersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.masterIndex == MithrixIndex)
					{
						IsMithrix = true;
						break;
					}
				}
			}
			if (!IsMithrix)
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
							VoidRaidCrabIndex = MasterCatalog.MasterIndex.none;
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
		private static void OnPhase1(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 5.5f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void OnPhase2(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 2f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void OnPhase3(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				SummonTimer = 6f;
				SpawnLocation = MoonPosition;
			}
		}
		private static void OnPhase4(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
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
		private static void IL_CalcTitanPowerAndBestTeam(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 2),
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.GoldTitanManager", "goldTitanItemIndex"),
				x => ILPatternMatchingExt.MatchLdcI4(x, 1),
				x => ILPatternMatchingExt.MatchLdcI4(x, 1)
			);
			if (ilcursor.Index > 0)
            {
				ilcursor.Index+=5;
				ilcursor.Emit(OpCodes.Ldc_I4, MainPlugin.Config_Halcyon_ItemMult.Value);
				ilcursor.Emit(OpCodes.Mul);
			}
		}
		private static void IL_TrySpawnTitan(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);

			if (MainPlugin.Config_Halcyon_ChannelTeamFix.Value)
			{
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdloc(x, 5),
					x => ILPatternMatchingExt.MatchLdcI4(x, 1)
				);
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdloc(x, 5),
					x => ILPatternMatchingExt.MatchLdcI4(x, 1)
				);
				ilcursor.Index += 1;
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<TeamIndex>>(() =>
				{
					TeamIndex team;
					int itemCount;
					GoldTitanManager.CalcTitanPowerAndBestTeam(out itemCount, out team);
					return team;
				});
			}
			ilcursor.GotoNext(
				   x => ILPatternMatchingExt.MatchLdloc(x, 1),
				   x => ILPatternMatchingExt.MatchConvR4(x),
				   x => ILPatternMatchingExt.MatchLdcR4(x, 1f)
				);
			if (ilcursor.Index > 0)
			{
				ilcursor.Index += 2;
				ilcursor.Next.Operand = 1f;
			}
			ilcursor.Index = 0;
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 1),
				x => ILPatternMatchingExt.MatchConvR4(x),
				x => ILPatternMatchingExt.MatchLdcR4(x, 0.5f)
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Index += 2;
				ilcursor.Next.Operand = 1f;
			}
		}
	}
}
