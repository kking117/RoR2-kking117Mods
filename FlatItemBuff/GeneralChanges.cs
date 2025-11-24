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
