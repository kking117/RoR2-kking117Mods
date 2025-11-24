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
		private const string LogName = "Lepton Daisy";
		internal static bool Enable = false;
		internal static float BaseHeal = 0.1f;
		internal static float StackHeal = 0.05f;
		internal static float Cooldown = 10f;
		internal static bool UseBaseRadius = false;
		public LeptonDaisy()
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
			BaseHeal = Math.Max(0f, BaseHeal);
			StackHeal = Math.Max(0f, StackHeal);
			Cooldown = Math.Max(0f, Cooldown);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "Periodically release a healing nova during Holdout events.";
			string desc = String.Format("Every <style=cIsUtility>{0}</style> seconds during <style=cIsDamage>Holdout</style> events release a <style=cIsHealing>nova</style>. The nova <style=cIsHealing>heals</style> all nearby allies for <style=cIsHealing>{1}%</style> <style=cStack>(+{2}% per stack)</style> of their <style=cIsHealing>maximum health</style>.", Cooldown, BaseHeal * 100f, StackHeal * 100f);
			LanguageAPI.Add("ITEM_TPHEALINGNOVA_PICKUP", pickup);
			LanguageAPI.Add("ITEM_TPHEALINGNOVA_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
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
						self.secondsUntilPulseAvailable += Cooldown;
					}
				}
			}
		}
		private void IL_Heal(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdloc(2)
			))
			{
				ilcursor.Index += 2;
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<HoldoutZoneController, float>>((holdoutZone) =>
				{
					return UseBaseRadius ? holdoutZone.baseRadius : holdoutZone.currentRadius;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_Heal A - Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdcR4(0.5f)
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldloc, 1);
				ilcursor.EmitDelegate<Func<TeamIndex, float>>((teamIndex) =>
				{
					return GetHealAmount(teamIndex);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_Heal B - Hook failed");
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
					num += characterMaster.inventory.GetItemCountEffective(RoR2Content.Items.TPHealingNova);
				}
			}
			return BaseHeal + (StackHeal * (num - 1));
		}
	}
}
