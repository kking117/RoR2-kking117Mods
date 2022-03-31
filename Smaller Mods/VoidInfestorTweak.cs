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
        public const string MODVERSION = "1.2.0";
		public void Awake()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_CharacterDeath);
        }
		private static void IL_CharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStloc(x, 31),
				x => ILPatternMatchingExt.MatchLdloc(x, 31)
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcI4(x, 4)
			);
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
	}
}