using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

using Mono.Cecil.Cil;
using MonoMod.Cil;

using System.Security;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace AurelioniteStuck
{
	[BepInPlugin("com.kking117.AurelioniteStuck", "AurelioniteStuck", "2.2.0")]
	public class MainPlugin : BaseUnityPlugin
	{
		private float summontimer = -1f;
		private float disbandtimer = -1f;
		private float validatetimer = -1f;

		public static ConfigEntry<int> Config_SpawnOnPhase;
		public static ConfigEntry<int> Config_StealMode;

		Vector3 MoonPosition = new Vector3(-87.4f, 491.95f, -2.54f);

		Vector3 spawnLocation;

		//ToDo:
		//Nerf Non-Player titans that are in the titanmanager list?
		public void Awake()
		{
			ReadConfig();
			if (Config_SpawnOnPhase.Value != 4)
            {
				Hooks();
			}
		}
		private void Hooks()
		{
			On.RoR2.Run.Start += Run_Start;
			On.RoR2.Run.FixedUpdate += Run_FixedUpdate;
			IL.RoR2.GoldTitanManager.TryStartChannelingTitansServer += new ILContext.Manipulator(IL_TrySpawnTitan);
			EditTags();
			if (Config_SpawnOnPhase.Value == 1)
			{
				On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += OnPhase1;
			}
			else if (Config_SpawnOnPhase.Value == 2)
			{
				On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += OnPhase2;
			}
			else if (Config_SpawnOnPhase.Value == 3)
			{
				On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += OnPhase3;
			}
			On.RoR2.GoldTitanManager.OnBossGroupStartServer += (orig, bossGroup) =>
			{
				bool IsMithrix = false;
				CombatSquad combatSquad = bossGroup.combatSquad;
				using (IEnumerator<CharacterMaster> enumerator = combatSquad.readOnlyMembersList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.masterIndex == MasterCatalog.FindMasterIndex("BrotherHurtMaster"))
						{
							IsMithrix = true;
							break;
						}
					}
				}
				if (!IsMithrix)
				{
					orig(bossGroup);
				}
			};
			if (Config_StealMode.Value >= 1)
			{
				On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += OnPhase4;
				On.RoR2.ItemStealController.StolenInventoryInfo.TakeItemFromLendee += (orig, self, item, stacks) =>
				{
					if (item == RoR2Content.Items.TitanGoldDuringTP.itemIndex)
					{
						validatetimer = 0.5f;
					}
					return orig(self, item, stacks);
				};
			}
			if (Config_StealMode.Value >= 2)
			{
				On.EntityStates.BrotherMonster.SpellChannelExitState.OnEnter += OnItemSteal_End;
			}
			On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += OnEncounterFinish;
		}
		private static void IL_TrySpawnTitan(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
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
		private void EditTags()
        {
			ItemDef seed = RoR2Content.Items.TitanGoldDuringTP;
			if (seed)
			{
				List<ItemTag> newlist = seed.tags.ToList<ItemTag>();
				for(int i = 0; i<newlist.Count;i++)
                {
					if(newlist[i] == ItemTag.CannotSteal)
                    {
						newlist.RemoveAt(i);
						i--;
                    }
                }
				seed.tags = newlist.ToArray();
			}
		}
		private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
		{
			orig(self);
			summontimer = -1f;
			disbandtimer = -1f;
			validatetimer = -1f;
			spawnLocation = MoonPosition;
		}
		private void Run_FixedUpdate(On.RoR2.Run.orig_FixedUpdate orig, Run self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				if (summontimer > 0f)
				{
					summontimer -= Time.fixedDeltaTime;
					if (summontimer <= 0f)
					{
						if (GoldTitanManager.currentTitans.Count > -1)
						{
							GoldTitanManager.TryStartChannelingTitansServer(self, spawnLocation, null, null);
						}
					}
				}
				if (disbandtimer > 0f)
				{
					disbandtimer -= Time.fixedDeltaTime;
					if (disbandtimer <= 0f)
					{
						GoldTitanManager.KillTitansInList(GoldTitanManager.currentTitans);
					}
				}
				if (validatetimer > 0f)
                {
					validatetimer -= Time.fixedDeltaTime;
					if(validatetimer <= 0f)
                    {
						KillInvalidTitans();
					}
				}
			}
		}
		private void KillInvalidTitans()
        {
			List<CharacterMaster> titans = GoldTitanManager.currentTitans;
			for(TeamIndex team = (TeamIndex)0; team < TeamIndex.Count; team ++)
            {
				if(Util.GetItemCountForTeam(team, RoR2Content.Items.TitanGoldDuringTP.itemIndex, true, true) < 1)
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
		private void OnPhase1(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
        {
			orig(self);
			if (NetworkServer.active)
			{
				summontimer = 5.5f;
				spawnLocation = MoonPosition;
			}
		}
		private void OnPhase2(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				summontimer = 2f;
				spawnLocation = MoonPosition;
			}
		}
		private void OnPhase3(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				summontimer = 6f;
				spawnLocation = MoonPosition;
			}
		}
		private void OnPhase4(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				disbandtimer = 3f;
			}
		}
		private void OnItemSteal_End(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnEnter orig, EntityStates.BrotherMonster.SpellChannelExitState self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				summontimer = 1f;
				spawnLocation = self.characterBody.footPosition;
			}
		}
		private void OnEncounterFinish(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
		{
			orig(self);
			if (NetworkServer.active)
			{
				disbandtimer = 3f;
			}
		}
		public void ReadConfig()
        {
			Config_SpawnOnPhase = Config.Bind<int>(new ConfigDefinition("AurelioniteStuck", "Spawn Phase"), 2, new ConfigDescription("Spawns Aurelionite on this phase during the Final Boss if players have the Halcyon Seed. (0-4) (0 = Don't spawn at all) (4 pretty much just disables this mod)", null, Array.Empty<object>()));
			Config_StealMode = Config.Bind<int>(new ConfigDefinition("AurelioniteStuck", "Steal Mode"), 2, new ConfigDescription("0 = Nothing happens, 1 = Aurelionite is killed, 2 = Aurelionite is killed and Mithrix gets his own.", null, Array.Empty<object>()));
		}
	}
}
