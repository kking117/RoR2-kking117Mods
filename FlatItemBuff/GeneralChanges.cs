using System;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff
{
	public class GeneralChanges
	{
		internal static bool TweakBarrierDecay = false;
		public GeneralChanges()
		{
			MainPlugin.ModLogger.LogInfo("Performing general changes.");
			//ClampConfig();
			Hooks();
		}
		private void Hooks()
		{
			if (TweakBarrierDecay)
			{
				MainPlugin.ModLogger.LogInfo("Attempting IL to tweak barrier decay.");
				IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
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
