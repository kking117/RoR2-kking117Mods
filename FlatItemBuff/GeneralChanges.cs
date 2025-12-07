using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff
{
	public class GeneralChanges
	{
		internal static bool TweakBarrierDecay = false;
		internal static List<SceneDef> BannedSceneSpawns = new List<SceneDef>();
		public GeneralChanges()
		{
			MainPlugin.ModLogger.LogInfo("Performing general changes.");
			//ClampConfig();
			SetupLists();
			Hooks();
		}
		private void SetupLists()
		{
			//"RoR2/Base/bazaar/bazaar.asset"
			SceneDef newScene = Addressables.LoadAssetAsync<SceneDef>("4116724a9bd3d05499bac80e6297a949").WaitForCompletion();
			if (newScene != null)
			{
				BannedSceneSpawns.Add(newScene);
			}
			//"RoR2/DLC3/computationalexchange/computationalexchange.asset"
			newScene = Addressables.LoadAssetAsync<SceneDef>("55dcf3dbd137f99458af33ad467bb574").WaitForCompletion();
			if (newScene != null)
			{
				BannedSceneSpawns.Add(newScene);
			}
		}
		private void Hooks()
		{
			if (TweakBarrierDecay)
			{
				MainPlugin.ModLogger.LogInfo("Attempting IL to tweak barrier decay.");
				IL.RoR2.HealthComponent.GetBarrierDecayRate += new ILContext.Manipulator(IL_GetBarrierDecayRate);
			}
		}
		private void IL_GetBarrierDecayRate(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchCall(typeof(HealthComponent), "get_fullBarrier")
			))
			{
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
				{
					return self.fullCombinedHealth;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": General - Improve Barrier Decay A - IL Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchCall(typeof(HealthComponent), "get_fullBarrier")
			))
			{
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
				{
					return self.fullCombinedHealth;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": General - Improve Barrier Decay B - IL Hook failed");
			}
		}
	}
}
