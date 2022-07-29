using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;
using EntityStates;

namespace FlatItemBuff.ItemChanges
{
	public class WaxQuail
	{
		private static float BaseHori = 15f;
		private static float StackHori = 5f;

		private static float BaseVert = 0.3f;
		private static float StackVert = 0.0f;

		private static float BaseAirSpeed = 0.14f;
		private static float StackAirSpeed = 0.07f;
		
		//Todo:
		//Disable WaxQuail_Behaviour if StormyItems is enabled to avoid redundant code.
		//Add hyperbolic scaling or even a hard cap to all effects. (Actually just do this, it would greatly improve it and no one would really complain.)
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Wax Quail");
			SetupConfigValues();
			UpdateText();
			Hooks();
		}
		private static void SetupConfigValues()
		{
			BaseHori = MainPlugin.WaxQuail_BaseHori.Value;
			StackHori = MainPlugin.WaxQuail_StackHori.Value;
			BaseVert = MainPlugin.WaxQuail_BaseVert.Value;
			StackVert = MainPlugin.WaxQuail_StackVert.Value;
			BaseAirSpeed = MainPlugin.WaxQuail_BaseAirSpeed.Value;
			StackAirSpeed = MainPlugin.WaxQuail_StackAirSpeed.Value;
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string jumpBoost = "";
			if (BaseHori != 0f || StackHori != 0f)
            {
				jumpBoost = string.Format("<style=cIsUtility>Jumping</style> while <style=cIsUtility>sprinting</style> boosts you forward by <style=cIsUtility>{0}m</style>", BaseHori);
				if (StackHori != 0f)
                {
					jumpBoost += string.Format(" <style=cStack>(+{0}m per stack)</style>", StackHori);
				}
				jumpBoost += ".";
			}
			string speedBoost = "";
			if (BaseAirSpeed != 0f || StackAirSpeed != 0f)
			{
				speedBoost = string.Format("Increases<style=cIsUtility> movement speed</style> by <style=cIsUtility>{0}%</style>", BaseAirSpeed * 100f);
				if (StackHori != 0f)
				{
					speedBoost += string.Format(" <style=cStack>(+{0}% per stack)</style>", StackAirSpeed * 100f);
				}
				speedBoost += " while <style=cIsUtility>airborne</style>.";
			}
			string desc = jumpBoost;
			if (jumpBoost.Length > 0)
            {
				desc += " ";
			}
			desc += speedBoost;
			LanguageAPI.Add("ITEM_JUMPBOOST_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.EntityStates.GenericCharacterMain.ProcessJump += new ILContext.Manipulator(IL_ProcessJump);
			CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
		}
		private static void OnInventoryChanged(CharacterBody self)
		{
			if (NetworkServer.active)
			{
				self.AddItemBehavior<WaxQuail_Behavior>(self.inventory.GetItemCount(RoR2Content.Items.JumpBoost));
			}
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
				if (itemCount > 0)
				{
					if (sender.characterMotor && !sender.characterMotor.isGrounded)
                    {
						float speedBonus = BaseAirSpeed + ((itemCount - 1) * StackAirSpeed);
						if (!sender.isSprinting)
						{
							speedBonus *= sender.sprintingSpeedMultiplier;
						}
						args.moveSpeedMultAdd += speedBonus;
					}
				}
			}
		}
		private static void IL_ProcessJump(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 5),
				x => ILPatternMatchingExt.MatchLdcR4(x, 0.0f)
			);
			//Prevent Quail from boosting airborne jumps
			if (ilcursor.Index > 0)
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 5);
				ilcursor.EmitDelegate<Func<EntityState, float, float>>((stateBase, originalValue) =>
				{
					if (stateBase.characterMotor.isGrounded)
                    {
						return originalValue;
					}
					return 0.0f;
				});
			}
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStloc(x, 1),
				x => ILPatternMatchingExt.MatchLdcR4(x, 10f),
				x => ILPatternMatchingExt.MatchLdloc(x, 2)
			);
			//Horizontal Boost
			if(ilcursor.Index > 0)
            {
				ilcursor.Index += 1;
				ilcursor.RemoveRange(4);
				ilcursor.Emit(OpCodes.Ldloc_2);
				ilcursor.EmitDelegate<Func<int, float>>((itemCount) =>
				{
					return BaseHori + (StackHori * (itemCount - 1));
				});
			}
			//Vertical Boost
			//This was surprisingly easy to add, was expecting it to be an hour of head banging to no end.
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStloc(x, 3)
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Index += 1;
				ilcursor.Emit(OpCodes.Ldloc, 4);
				ilcursor.Emit(OpCodes.Ldloc_2);
				ilcursor.EmitDelegate<Func<float, int, float>>((verticalBonus, itemCount) =>
				{
					float jumpBoost = BaseVert + (StackVert * (itemCount - 1));
					return verticalBonus + jumpBoost;
				});
				ilcursor.Emit(OpCodes.Stloc, 4);
			}
		}
	}
}
