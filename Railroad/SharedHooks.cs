using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;

namespace Railroad
{
	public class SharedHooks
	{
		public delegate void Handle_EclipseRun_Start(EclipseRun self);
		public static Handle_EclipseRun_Start Handle_EclipseRun_Start_Actions;

		public delegate void Handle_Mithrix_Clear(EntityStates.Missions.BrotherEncounter.EncounterFinished self);
		public static Handle_Mithrix_Clear Handle_Mithrix_Clear_Actions;

		public delegate void Handle_TwistedScavenger_Clear(EntityStates.Missions.LunarScavengerEncounter.FadeOut self);
		public static Handle_TwistedScavenger_Clear Handle_TwistedScavenger_Clear_Actions;

		public delegate void Handle_Voidling_Clear(RoR2.VoidRaidGauntletController self);
		public static Handle_Voidling_Clear Handle_Voidling_Clear_Actions;

		public delegate void Handle_FalseSon_Clear(EntityStates.MeridianEvent.Phase3 self);
		public static Handle_FalseSon_Clear Handle_FalseSon_Clear_Actions;

		public delegate void Handle_SolusWing_Clear(EntityStates.SolusWing2.Mission5Death self);
		public static Handle_SolusWing_Clear Handle_SolusWing_Clear_Actions;

		public delegate void Handle_SolusHeart_Clear(EntityStates.SolusHeart.Death.MissionCompleted self);
		public static Handle_SolusHeart_Clear Handle_SolusHeart_Clear_Actions;

		public delegate void Handle_PostLoad();
		public static Handle_PostLoad Handle_PostLoad_Actions;

		public static void Setup()
		{
			if (Handle_EclipseRun_Start_Actions != null)
			{
				On.RoR2.EclipseRun.Start += EclipseRun_Start;
			}
			if (Handle_Mithrix_Clear_Actions != null)
			{
				On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += Mithrix_Clear;
			}
			if (Handle_TwistedScavenger_Clear_Actions != null)
			{
				On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += TwistedScavenger_Clear;
			}
			if (Handle_Voidling_Clear_Actions != null)
			{
				On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += Voidling_Clear;
			}
			if (Handle_FalseSon_Clear_Actions != null)
			{
				On.EntityStates.MeridianEvent.Phase3.OnExit += FalseSon_Clear;
			}
			if (Handle_SolusWing_Clear_Actions != null)
			{
				On.EntityStates.SolusWing2.Mission5Death.OnEnter += SolusWing_Clear;
			}
			if (Handle_SolusHeart_Clear_Actions != null)
			{
				On.EntityStates.SolusHeart.Death.MissionCompleted.OnEnter += SolusHeart_Clear;
			}
			if (Handle_PostLoad_Actions != null)
			{
				GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad_GameModeCatalog));
			}
		}

		internal static void PostLoad_GameModeCatalog()
		{
			Handle_PostLoad_Actions.Invoke();
		}
		internal static void Mithrix_Clear(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
		{
			orig(self);
			Handle_Mithrix_Clear_Actions.Invoke(self);
		}
		internal static void TwistedScavenger_Clear(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
		{
			orig(self);
			Handle_TwistedScavenger_Clear_Actions.Invoke(self);
		}
		internal static void Voidling_Clear(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, RoR2.VoidRaidGauntletController self)
		{
			orig(self);
			Handle_Voidling_Clear_Actions.Invoke(self);
		}
		internal static void FalseSon_Clear(On.EntityStates.MeridianEvent.Phase3.orig_OnExit orig, EntityStates.MeridianEvent.Phase3 self)
		{
			orig(self);
			Handle_FalseSon_Clear_Actions.Invoke(self);
		}
		internal static void SolusWing_Clear(On.EntityStates.SolusWing2.Mission5Death.orig_OnEnter orig, EntityStates.SolusWing2.Mission5Death self)
		{
			orig(self);
			Handle_SolusWing_Clear_Actions.Invoke(self);
		}
		internal static void SolusHeart_Clear(On.EntityStates.SolusHeart.Death.MissionCompleted.orig_OnEnter orig, EntityStates.SolusHeart.Death.MissionCompleted self)
		{
			//this triggers when solus heart is defeated, not when they are purged.
			orig(self);
			Handle_SolusHeart_Clear_Actions.Invoke(self);
		}
		internal static void EclipseRun_Start(On.RoR2.EclipseRun.orig_Start orig, EclipseRun self)
		{
			orig(self);
			Handle_EclipseRun_Start_Actions.Invoke(self);
		}
	}
}
