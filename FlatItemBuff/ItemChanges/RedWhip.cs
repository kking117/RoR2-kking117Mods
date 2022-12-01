using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class RedWhip
	{
		internal static int BuildUpSecond = 2;
		private static float BaseSpeed = 0.3f;
		private static float StackSpeed = 0.3f;
		private static float BuildBaseSpeed = 0.06f;
		private static float BuildStackSpeed = 0.06f;
		public static BuffDef WhipBuildBuff;
		internal static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Red Whip");
			SetupConfigValues();
			if (BuildUpSecond > -1)
			{
				CreateBuff();
			}
			UpdateText();
			Hooks();
		}
		private static void SetupConfigValues()
        {
			BuildUpSecond = MainPlugin.RedWhip_StartSecond.Value;
			BaseSpeed = MainPlugin.RedWhip_BaseSpeed.Value;
			StackSpeed = MainPlugin.RedWhip_StackSpeed.Value;

			if (BuildUpSecond > -1)
			{
				int divider = (int)Math.Ceiling(Math.Max(0f, MainPlugin.General_OutOfCombatTime.Value - BuildUpSecond));
				if(divider == 0)
                {
					BuildUpSecond = -1;
				}
				else
                {
					divider ++;
					divider *= 3;
					BuildStackSpeed = StackSpeed / divider;
					BuildBaseSpeed = BaseSpeed / divider;
				}
			}
		}
		private static void CreateBuff()
		{
			BuffDef refBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SprintOutOfCombat/bdWhipBoost.asset").WaitForCompletion();
			WhipBuildBuff = Modules.Buffs.AddNewBuff("WhipBuildUp", refBuff.iconSprite, new Color(0.415f, 0.4f, 0.4f, 1f), true, false, true);
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = string.Format("Leaving combat boosts your <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style> <style=cStack>(+{1}% per stack)</style>.", BaseSpeed * 100f, StackSpeed * 100f);
			LanguageAPI.Add("ITEM_SPRINTOUTOFCOMBAT_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			if (BuildUpSecond > -1)
			{
				CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
				RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			}
		}
		private static void OnInventoryChanged(CharacterBody self)
		{
			if (NetworkServer.active)
			{
				self.AddItemBehavior<RedWhip_Behavior>(self.inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat));
			}
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat);
				if (itemCount > 0)
				{
					if (!sender.HasBuff(RoR2Content.Buffs.WhipBoost.buffIndex))
					{
						int buffCount = sender.GetBuffCount(WhipBuildBuff.buffIndex);
						if (buffCount > 0)
                        {
							itemCount = Math.Max(0, itemCount - 1);
							args.moveSpeedMultAdd += buffCount * (BuildBaseSpeed + (BuildStackSpeed * itemCount));
						}
					}
				}
			}
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 75),
				x => ILPatternMatchingExt.MatchLdloc(x, 7),
				x => ILPatternMatchingExt.MatchConvR4(x)
			);
			ilcursor.Index += 2;
			ilcursor.RemoveRange(3);
			ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
			{
				itemCount = Math.Max(0, itemCount - 1);
				return BaseSpeed + (StackSpeed * itemCount);
			});
		}
	}
}
