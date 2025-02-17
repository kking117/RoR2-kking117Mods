using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class TopazBrooch
	{
		private const string LogName = "Topaz Brooch";
		internal static bool Enable = false;
		internal static float BaseFlatBarrier = 8.0f;
		internal static float StackFlatBarrier = 0.0f;
		internal static float BaseCentBarrier = 0.02f;
		internal static float StackCentBarrier = 0.02f;
		internal static bool Comp_AssistManager = true;
		public TopazBrooch()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			Hooks();
			if (MainPlugin.AssistManager_Loaded)
			{
				ApplyAssistManager();
			}
		}
		private void ApplyAssistManager()
        {
			AssistManager.VanillaTweaks.TopazBrooch.Instance.SetEnabled(false);
			if (Comp_AssistManager)
			{
				AssistManager.AssistManager.HandleAssistInventoryActions += AssistManger_OnKill;
			}
		}
		private void ClampConfig()
		{
			BaseFlatBarrier = Math.Max(0f, BaseFlatBarrier);
			StackFlatBarrier = Math.Max(0f, StackFlatBarrier);
			BaseCentBarrier = Math.Max(0f, BaseCentBarrier);
			StackCentBarrier = Math.Max(0f, StackCentBarrier);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string desc = "Gain a <style=cIsHealing>temporary barrier</style> on kill for ";
			if (BaseFlatBarrier > 0f || StackFlatBarrier > 0f)
			{
				string textStackFlatBarrier = "";
				if (StackFlatBarrier > 0f)
                {
					textStackFlatBarrier = string.Format(" <style=cStack>(+{0} per stack)</style>", StackFlatBarrier);
				}
				desc += string.Format("<style=cIsHealing>{0}{1} health</style>", BaseFlatBarrier, textStackFlatBarrier);
			}
			if (BaseCentBarrier > 0f || StackCentBarrier > 0f)
			{
				if (BaseFlatBarrier > 0f || StackCentBarrier > 0f)
				{
					desc += " plus an additional ";
				}
				string textStackCentBarrier = "";
				if (StackCentBarrier > 0f)
				{
					textStackCentBarrier = string.Format(" <style=cStack>(+{0}% per stack)</style>", StackCentBarrier * 100f);
				}
				desc += string.Format("<style=cIsHealing>{0}%{1}</style> of <style=cIsHealing>maximum health</style>", BaseCentBarrier * 100f, textStackCentBarrier);
			}
			desc += ".";
			LanguageAPI.Add("ITEM_BARRIERONKILL_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_OnCharacterDeath);
		}
		private void AssistManger_OnKill(AssistManager.AssistManager.Assist assist, Inventory assistInventory, CharacterBody killerBody, DamageInfo damageInfo)
		{
			CharacterBody assistBody = assist.attackerBody;
			if (assistBody == killerBody)
			{
				return;
			}

			HealthComponent hpComp = assistBody.healthComponent;
			if (hpComp)
            {
				int itemCount = assistInventory.GetItemCount(RoR2Content.Items.BarrierOnKill);
				if (itemCount > 0)
				{
					hpComp.AddBarrier(GetFlatBarrier(hpComp, itemCount));
				}
			}
		}
		private float GetFlatBarrier(HealthComponent hpComp, int itemCount)
        {
			itemCount = Math.Max(0, itemCount-1);
			float amount = BaseFlatBarrier + (StackFlatBarrier * itemCount);
			amount += hpComp.fullCombinedHealth * (BaseCentBarrier + (StackCentBarrier * itemCount));
			return amount;
        }
		private void IL_OnCharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcR4(15),
				x => x.MatchLdloc(53),
				x => x.MatchConvR4(),
				x => x.MatchMul()
			))
			{
				ilcursor.RemoveRange(4);
				ilcursor.Emit(OpCodes.Ldarg_1);
				ilcursor.Emit(OpCodes.Ldloc, 53);
				ilcursor.EmitDelegate<Func<DamageReport, int, float>>((damageReport, itemCount) =>
				{
					return GetFlatBarrier(damageReport.attackerBody.healthComponent, itemCount);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnCharacterDeath - Hook failed");
			}
		}
	}
}
