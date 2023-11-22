using System;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff
{
	public class GeneralChanges
	{
		internal static bool FixExpiredPings = true;
		public GeneralChanges()
		{
			MainPlugin.ModLogger.LogInfo("Performing general changes.");
			Hooks();
		}
		private void Hooks()
		{
			if (FixExpiredPings)
            {
				IL.RoR2.PingerController.AttemptPing += new ILContext.Manipulator(IL_AttemptPing);
			}
		}
		private void IL_AttemptPing(ILContext il)
		{
			MainPlugin.ModLogger.LogInfo("Attempting IL to fix expired pings.");
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
						x => ILPatternMatchingExt.MatchLdloc(x, 0),
						x => x.MatchLdfld(typeof(RoR2.PingerController.PingInfo), "targetNetworkIdentity")
					);
			ilcursor.GotoNext(
						x => ILPatternMatchingExt.MatchLdloc(x, 0),
						x => x.MatchLdfld(typeof(RoR2.PingerController.PingInfo), "targetNetworkIdentity")
					);
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
			MainPlugin.ModLogger.LogInfo("Expired Ping Fix Done");
		}
	}
}
