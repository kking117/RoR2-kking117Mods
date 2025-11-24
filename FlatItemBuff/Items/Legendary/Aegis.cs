using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class Aegis
	{
		private const string LogName = "Aegis";
		internal static bool Enable = false;
		internal static bool AllowRegen = true;
		internal static float BaseOverheal = 1f;
		internal static float StackOverheal = 0f;
		internal static float BaseMaxBarrier = 1f;
		internal static float StackMaxBarrier = 1f;
		public Aegis()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseOverheal = Math.Max(0f, BaseOverheal);
			StackOverheal = Math.Max(0f, StackOverheal);
			BaseMaxBarrier = Math.Max(0f, BaseMaxBarrier);
			StackMaxBarrier = Math.Max(0f, StackMaxBarrier);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup_overheal = "";
			string pickup_maxbarrier = "";
			string desc_overheal = "";
			string desc_maxbarrier = "";
			if (BaseOverheal > 0f || StackOverheal > 0f)
            {
				pickup_overheal = "Healing past full grants you a temporary barrier.";
				if (StackOverheal > 0f)
                {
					desc_overheal = string.Format("Healing past full grants you <style=cIsHealing>barrier</style> equal to <style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style></style> of the amount <style=cIsHealing>healed</style>.", BaseOverheal * 100f, StackOverheal * 100f);
				}
				else
                {
					desc_overheal = string.Format("Healing past full grants you <style=cIsHealing>barrier</style> equal to <style=cIsHealing>{0}%</style> of the amount <style=cIsHealing>healed</style>.", BaseOverheal * 100f);
				}
			}
			if (BaseMaxBarrier > 0f || StackMaxBarrier > 0f)
			{
				if (BaseOverheal > 0f || StackOverheal > 0f)
                {
					pickup_maxbarrier = " ";
					desc_maxbarrier = " ";
				}
				pickup_maxbarrier += "Increases maximum barrier.";
				if (StackMaxBarrier > 0f)
				{
					desc_maxbarrier += string.Format("Increases <style=cIsHealing>maximum barrier</style> by <style=cIsHealing>{0}% <style=cStack>(+{1}% per stack)</style></style>.", BaseMaxBarrier * 100f, StackMaxBarrier * 100f);
				}
				else
                {
					desc_maxbarrier += string.Format("Increases <style=cIsHealing>maximum barrier</style> by <style=cIsHealing>{0}%</style>.", BaseMaxBarrier * 100f);
				}
			}
			string pickup = pickup_overheal + pickup_maxbarrier;
			string desc = desc_overheal + desc_maxbarrier;
			LanguageAPI.Add("ITEM_BARRIERONOVERHEAL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_BARRIERONOVERHEAL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.Heal += new ILContext.Manipulator(IL_Heal);
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			IL.RoR2.HealthComponent.GetHealthBarValues += new ILContext.Manipulator(IL_GetHealthBarValues);
		}
		private void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdarg(0),
				x => x.MatchCall(typeof(CharacterBody), "get_maxHealth"),
				x => x.MatchLdarg(0),
				x => x.MatchCall(typeof(CharacterBody), "get_maxShield"),
				x => x.MatchAdd(),
				x => x.MatchCall(typeof(CharacterBody), "set_maxBarrier")
            ))
            {
				ilcursor.Index += 2;
				ilcursor.RemoveRange(4);
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					float fullHealth = self.maxHealth + self.maxShield;
					if (self.inventory)
					{
						int itemCount = self.inventory.GetItemCountEffective(RoR2Content.Items.BarrierOnOverHeal);
						if (itemCount > 0)
						{
							itemCount = Math.Max(0, itemCount - 1);
							return fullHealth * (1f + BaseMaxBarrier + StackMaxBarrier * itemCount);
						}
					}
					return fullHealth;
				});
			}
			else
            {
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_RecalculateStats - Hook failed");
            }
		}
		private void IL_GetHealthBarValues(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(typeof(HealthComponent), "barrier"),
				x => x.MatchLdloc(1),
				x => x.MatchMul()
			))
			{
				ilcursor.Index += 1;
				ilcursor.RemoveRange(3);
				ilcursor.Emit(OpCodes.Ldloc, 0);
				ilcursor.EmitDelegate<Func<HealthComponent, float, float>>((self, curse) =>
				{
					float result = (1f - curse) / self.fullBarrier;
					return result * self.barrier;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_GetHealthBarValues - Hook failed");
			}
		}
		private void IL_Heal(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (AllowRegen)
			{
				if (ilcursor.TryGotoNext(
					x => x.MatchLdloc(2),
					x => x.MatchLdcR4(0.0f)
				))
                {
					ilcursor.Index += 3;
					ilcursor.RemoveRange(2);
				}
				else
                {
					UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_Heal A - Hook failed");
				}
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(2),
				x => x.MatchLdarg(0),
				x => x.MatchLdflda(typeof(HealthComponent), "itemCounts"),
				x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "barrierOnOverHeal")
			))
            {
				ilcursor.Index += 2;
				ilcursor.RemoveRange(5);
				ilcursor.EmitDelegate<Func<HealthComponent, float>>((self) =>
				{
					int itemCount = Math.Max(0, self.itemCounts.barrierOnOverHeal -1);
					return BaseOverheal + (itemCount * StackOverheal);
				});
			}
            else
            {
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_Heal B - Hook failed");
			}
		}
	}
}
