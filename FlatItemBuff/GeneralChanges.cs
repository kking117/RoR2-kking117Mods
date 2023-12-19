using System;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff
{
	public class GeneralChanges
	{
		internal static bool FixExpiredPings = false;
		internal static bool TweakBarrierDecay = false;
		public GeneralChanges()
		{
			MainPlugin.ModLogger.LogInfo("Performing general changes.");
			//ClampConfig();
			Hooks();
		}
		private void Hooks()
		{
			if (FixExpiredPings)
            {
				MainPlugin.ModLogger.LogInfo("Attempting IL to fix expired pings.");
				IL.RoR2.PingerController.AttemptPing += new ILContext.Manipulator(IL_AttemptPing);
			}
			if (TweakBarrierDecay)
			{
				MainPlugin.ModLogger.LogInfo("Attempting IL to tweak barrier decay.");
				IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			}
		}
		private void IL_AttemptPing(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
					x => x.MatchLdloc(0),
					x => x.MatchLdfld(typeof(RoR2.PingerController.PingInfo), "targetNetworkIdentity")
				)
			&&
			ilcursor.TryGotoNext(
					x => x.MatchLdloc(0),
					x => x.MatchLdfld(typeof(RoR2.PingerController.PingInfo), "targetNetworkIdentity")
				)
			)
            {
				ilcursor.Index++;
				ilcursor.RemoveRange(4);
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<PingerController.PingInfo, PingerController, bool>>((newInfo, pingerController) =>
				{
					if (pingerController.pingIndicator)
					{
						return pingerController.currentPing.targetNetworkIdentity == newInfo.targetNetworkIdentity;
					}
					return false;
				});
				ilcursor.Emit(OpCodes.Ldc_I4_1);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": General - Expired Ping Fix - IL Hook failed");
			}
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchCall(typeof(CharacterBody), "get_maxBarrier")
			))
			{
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					return self.maxHealth + self.maxShield;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": General - Improve Barrier Decay - IL Hook failed");
			}
		}
	}
}
