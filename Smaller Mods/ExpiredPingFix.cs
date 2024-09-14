using System;
using BepInEx;
using RoR2;
using R2API.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ExpiredPingFix
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.ExpiredPingFix";
		public const string MODNAME = "ExpiredPingFix";
		public const string MODVERSION = "1.0.0";
		public void Awake()
		{
			//ReadConfig();
			IL.RoR2.PingerController.AttemptPing += new ILContext.Manipulator(IL_AttemptPing);
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
				UnityEngine.Debug.LogError(MODNAME + ": Expired Ping Fix - IL Hook failed");
			}
		}
	}
}
