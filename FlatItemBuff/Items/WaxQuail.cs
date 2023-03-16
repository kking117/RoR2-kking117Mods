using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using EntityStates;

namespace FlatItemBuff.Items
{
	public class WaxQuail
	{
		internal static bool Enable = true;
		internal static float BaseHori = 12f;
		internal static float StackHori = 8f;
		internal static float ActualBaseHori = 0.12f;
		internal static float ActualStackHori = 0.08f;
		internal static float CapHori = 100f;

		internal static float BaseVert = 0.3f;
		internal static float StackVert = 0f;
		internal static float ActualBaseVert = 0f;
		internal static float ActualStackVert = 0f;
		internal static float CapVert = 0.0f;

		internal static float BaseAirSpeed = 0.12f;
		internal static float StackAirSpeed = 0.08f;
		internal static float ActualBaseAirSpeed = 0.12f;
		internal static float ActualStackAirSpeed = 0.08f;
		internal static float CapAirSpeed = 1f;
		public WaxQuail()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Wax Quail");
			if (CapAirSpeed > 0f)
			{
				ActualBaseAirSpeed = BaseAirSpeed / CapAirSpeed;
				ActualStackAirSpeed = StackAirSpeed / CapAirSpeed;
			}
			else
			{
				ActualBaseAirSpeed = BaseAirSpeed;
				ActualStackAirSpeed = StackAirSpeed;
			}
			if (CapVert > 0f)
			{
				ActualBaseVert = BaseVert / CapVert;
				ActualStackVert = StackVert / CapVert;
			}
			else
			{
				ActualBaseVert = BaseVert;
				ActualStackVert = StackVert;
			}
			if (CapHori > 0f)
			{
				ActualBaseHori = BaseHori / CapHori;
				ActualStackHori = StackHori / CapHori;
			}
			else
			{
				ActualBaseHori = BaseHori;
				ActualStackHori = StackHori;
			}
			UpdateText();
			Hooks();
		}
		private void UpdateText()
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
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.EntityStates.GenericCharacterMain.ProcessJump += new ILContext.Manipulator(IL_ProcessJump);
			if (StackAirSpeed != 0f || StackAirSpeed != 0f)
            {
				CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			if (NetworkServer.active)
			{
				self.AddItemBehavior<Behaviors.WaxQuail>(self.inventory.GetItemCount(RoR2Content.Items.JumpBoost));
			}
		}
		private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
		{
			int itemCount = inventory.GetItemCount(RoR2Content.Items.JumpBoost);
			if (itemCount > 0)
			{
				if (sender.characterMotor && !sender.characterMotor.isGrounded)
				{
					float speedBonus = 0f;
					if (CapAirSpeed > 0f)
					{
						speedBonus = Utils.Helpers.HyperbolicResult(itemCount, ActualBaseAirSpeed, ActualStackAirSpeed, 1) * CapAirSpeed;
					}
					else
					{
						speedBonus = BaseAirSpeed + ((itemCount - 1) * StackAirSpeed);
					}
					args.moveSpeedMultAdd += speedBonus;
				}
			}
		}
		private void IL_ProcessJump(ILContext il)
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
					if (CapHori > 0f)
					{
						return Utils.Helpers.HyperbolicResult(itemCount, ActualBaseHori, ActualStackHori, 1) * CapHori;
					}
					return BaseHori + (StackHori * (itemCount - 1));
				});
			}
			//Vertical Boost
			//This was surprisingly easy to add, was expecting it to be an hour of head banging to no end.
			if (BaseVert != 0f || StackVert != 0f)
			{
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchStloc(x, 3)
				);
				if (ilcursor.Index > 0)
				{
					ilcursor.Index += 1;
					ilcursor.Emit(OpCodes.Ldarg, 0);
					ilcursor.Emit(OpCodes.Ldloc, 4);
					ilcursor.Emit(OpCodes.Ldloc_2);
					ilcursor.EmitDelegate<Func<EntityState, float, int, float>>((statebase, verticalBonus, itemCount) =>
					{
						float jumpBoost = 0f;
						if (CapVert > 0f)
						{
							jumpBoost = Utils.Helpers.HyperbolicResult(itemCount, ActualBaseVert, ActualStackVert, 1) * CapVert;
						}
						{
							jumpBoost = BaseVert + (StackVert * (itemCount - 1));
						}
						return verticalBonus + jumpBoost;
					});
					ilcursor.Emit(OpCodes.Stloc, 4);
				}
			}
		}
	}
}
