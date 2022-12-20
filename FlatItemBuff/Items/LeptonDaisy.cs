using System;
using System.Collections.ObjectModel;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Items
{
	public class LeptonDaisy
	{
		private static float BaseHealing = 0.1f;
		private static float StackHealing = 0.1f;
		private static float ActualBaseHealing = 0.05f;
		private static float ActualStackHealing = 0.05f;
		private static float CapHealing = 2f;
		private static float HealTime = 10f;
		public LeptonDaisy()
		{
			MainPlugin.ModLogger.LogInfo("Changing Lepton Daisy");
			SetupConfigValues();
			UpdateText();
			Hooks();
		}
		private void SetupConfigValues()
		{
			BaseHealing = MainPlugin.LeptonDaisy_BaseHeal.Value;
			StackHealing = MainPlugin.LeptonDaisy_StackHeal.Value;
			CapHealing = MainPlugin.LeptonDaisy_CapHeal.Value;
			HealTime = MainPlugin.LeptonDaisy_HealTime.Value;

			if (CapHealing > 0f)
            {
				ActualBaseHealing = BaseHealing / CapHealing;
				ActualStackHealing = StackHealing / CapHealing;
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Periodically release a healing nova during Holdout events.";
			string desc = String.Format("Every <style=cIsUtility>{0}</style> seconds during <style=cIsDamage>Holdout</style> events release a <style=cIsHealing>nova</style>. The nova <style=cIsHealing>heals</style> all nearby allies for <style=cIsHealing>{1}%</style> <style=cStack>(+{2}% per stack)</style> of their <style=cIsHealing>maximum health</style>.", HealTime, BaseHealing * 100f, StackHealing * 100f);
			LanguageAPI.Add("ITEM_TPHEALINGNOVA_PICKUP", pickup);
			LanguageAPI.Add("ITEM_TPHEALINGNOVA_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.EntityStates.TeleporterHealNovaController.TeleporterHealNovaPulse.OnEnter += new ILContext.Manipulator(IL_Heal);
			On.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.FixedUpdate += NovaFixedUpdate;
			On.RoR2.HoldoutZoneController.UpdateHealingNovas += UpdateHealingNovas;
		}
		private void UpdateHealingNovas(On.RoR2.HoldoutZoneController.orig_UpdateHealingNovas orig, RoR2.HoldoutZoneController self, bool isCharging)
        {
			orig(self, true);
        }
		private void NovaFixedUpdate(On.EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain.orig_FixedUpdate orig, EntityStates.TeleporterHealNovaController.TeleporterHealNovaGeneratorMain self)
        {
			if (NetworkServer.active && Time.fixedDeltaTime > 0f)
			{
				if (!self.holdoutZone)
				{
					EntityState.Destroy(self.outer.gameObject);
					return;
				}
				if (self.holdoutZone.isActiveAndEnabled)
				{
					if (self.secondsUntilPulseAvailable > 0f)
					{
						self.secondsUntilPulseAvailable -= Time.fixedDeltaTime;
					}
					if (self.secondsUntilPulseAvailable <= 0f)
					{
						self.Pulse();
						self.secondsUntilPulseAvailable += HealTime;
					}
				}
			}
		}
		private void IL_Heal(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 0.5f)
			);
			if(ilcursor.Index > 0)
            {
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldloc, 1);
				ilcursor.EmitDelegate<Func<TeamIndex, float>>((teamIndex) =>
				{
					return GetHealAmount(teamIndex);
				});
			}
		}
		private float GetHealAmount(TeamIndex teamIndex)
        {
			int num = 0;
			ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				CharacterMaster characterMaster = readOnlyInstancesList[i];
				if (characterMaster.teamIndex == teamIndex)
				{
					num += characterMaster.inventory.GetItemCount(RoR2Content.Items.TPHealingNova);
				}
			}
			if (CapHealing > 0f)
            {
				return Utils.Helpers.HyperbolicResult(num, ActualBaseHealing, ActualStackHealing, 1) * CapHealing;
			}
			return BaseHealing + (StackHealing * (num - 1));
		}
	}
}
