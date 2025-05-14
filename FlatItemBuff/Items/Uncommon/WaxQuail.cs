using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using EntityStates;

namespace FlatItemBuff.Items
{
	public class WaxQuail
	{
		private const string LogName = "Wax Quail";
		internal static bool Enable = false;
		internal static float BaseHori = 12f;
		internal static float StackHori = 8f;
		internal static float ActualBaseHori = 0.12f;
		internal static float ActualStackHori = 0.08f;
		internal static float CapHori = 120f;

		internal static float BaseVert = 0.3f;
		internal static float StackVert = 0f;
		internal static float ActualBaseVert = 0f;
		internal static float ActualStackVert = 0f;
		internal static float CapVert = 0.0f;

		internal static float BaseAirSpeed = 0.12f;
		internal static float StackAirSpeed = 0.08f;
		internal static float ActualBaseAirSpeed = 0.12f;
		internal static float ActualStackAirSpeed = 0.08f;
		internal static float CapAirSpeed = 1.2f;
		public WaxQuail()
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
			BaseHori = Math.Max(0f, BaseHori);
			StackHori = Math.Max(0f, StackHori);
			CapHori = Math.Max(0f, CapHori);
			BaseVert = Math.Max(0f, BaseVert);
			StackVert = Math.Max(0f, StackVert);
			CapVert = Math.Max(0f, CapVert);
			BaseAirSpeed = Math.Max(0f, BaseAirSpeed);
			StackAirSpeed = Math.Max(0f, StackAirSpeed);
			CapAirSpeed = Math.Max(0f, CapAirSpeed);

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
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
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
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.EntityStates.GenericCharacterMain.ProcessJump_bool += new ILContext.Manipulator(IL_ProcessJump);
			if (StackAirSpeed != 0f || StackAirSpeed != 0f)
            {
				SharedHooks.Handle_GlobalInventoryChangedEvent_Actions += OnInventoryChanged;
				SharedHooks.Handle_GetStatCoefficients_Actions += GetStatCoefficients;
			}
		}
		private void OnInventoryChanged(CharacterBody self)
		{
			self.AddItemBehavior<Behaviors.WaxQuail>(self.inventory.GetItemCount(RoR2Content.Items.JumpBoost));
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
			//Make effect run
			if (ilcursor.TryGotoNext(
				x => x.MatchStloc(1)
			))
			{
				ilcursor.Index -= 1;
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<EntityState, bool>>((stateBase) =>
				{
					if (stateBase.characterMotor.isGrounded && stateBase.characterBody.isSprinting)
					{
						return stateBase.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost) > 0;
					}
					return false;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_ProcessJump C - Hook failed");
			}
			//Disable Old Behaviour
			if (ilcursor.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "JumpBoost"),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount")
			))
			{
				ilcursor.Index += 2;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_ProcessJump A - Hook failed");
			}
			//Now do the cool stuff
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(4),
				x => x.MatchLdloc(5)
			))
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldloc, 4);
				ilcursor.Emit(OpCodes.Ldloc, 1);
				ilcursor.EmitDelegate<Func<EntityState, float, bool, float>>((stateBase, returnValue, canBoost) =>
				{
					int itemCount = stateBase.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
					if (canBoost && itemCount > 0)
                    {
						float jumpBoost = 0f;
						if (CapHori > 0f)
						{
							jumpBoost = Utils.Helpers.HyperbolicResult(itemCount, ActualBaseHori, ActualStackHori, 1) * CapHori;
						}
						else
                        {
							jumpBoost = BaseHori + (StackHori * (itemCount - 1));
						}
						float airControl = stateBase.characterBody.acceleration * stateBase.characterMotor.airControl;
						jumpBoost = (float)Math.Sqrt(jumpBoost / airControl);
						airControl = stateBase.characterBody.moveSpeed / airControl;
						jumpBoost = (jumpBoost + airControl) / airControl;
						return returnValue + (jumpBoost - 1f);
					}
					return returnValue;
				});
				if (ilcursor.TryGotoNext(
					x => x.MatchLdloc(5)
				))
                {
					ilcursor.Remove();
					ilcursor.Emit(OpCodes.Ldarg, 0);
					ilcursor.Emit(OpCodes.Ldloc, 5);
					ilcursor.Emit(OpCodes.Ldloc, 1);
					ilcursor.EmitDelegate<Func<EntityState, float, bool, float>>((stateBase, returnValue, canBoost) =>
					{
						int itemCount = stateBase.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
						if (canBoost && itemCount > 0)
						{
							float jumpBoost = 0f;
							if (CapVert > 0f)
							{
								jumpBoost = Utils.Helpers.HyperbolicResult(itemCount, ActualBaseVert, ActualStackVert, 1) * CapVert;
							}
							{
								jumpBoost = BaseVert + (StackVert * (itemCount - 1));
							}
							return returnValue + jumpBoost;
						}
						return returnValue;
					});
				}
				else
				{
					UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_ProcessJump C - Hook failed");
				}
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_ProcessJump B - Hook failed");
			}
		}
	}
}
