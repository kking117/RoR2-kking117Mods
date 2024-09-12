using System;
using BepInEx;
using RoR2;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace VoidInfestorTweak
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class VoidInfestorTweak : BaseUnityPlugin
    {
        public const string MODUID = "com.kking117.VoidInfestorTweak";
        public const string MODNAME = "VoidInfestorTweak";
        public const string MODVERSION = "1.2.2";
		public void Awake()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_CharacterDeath);
        }
		private static void IL_CharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcI4(4),
				x => x.MatchCallOrCallvirt<CharacterMaster>("set_teamIndex")
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_1);
				ilcursor.EmitDelegate<Func<DamageReport, TeamIndex>>((dr) =>
				{
					if (dr.victimBody)
					{
						return dr.victimTeamIndex;
					}
					return TeamIndex.Void;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MODNAME + ": IL Hook failed");
			}
		}
	}
}