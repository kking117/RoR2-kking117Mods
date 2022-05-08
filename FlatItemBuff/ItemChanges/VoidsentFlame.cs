using System;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace FlatItemBuff.ItemChanges
{
	public class VoidsentFlame
	{
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Voidsent Flame");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "First hit on every enemy causes and explosion. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>..";
			string desc = String.Format("Upon hitting an enemy for the first time, <style=cIsDamage>detonate</style> them in a <style=cIsDamage>{0}m</style> <style=cStack>(+{1}m per stack)</style> radius burst for <style=cIsDamage>260%</style> <style=cStack>(+156% per stack)</style> base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.", MainPlugin.VoidsentFlame_BaseRadius.Value, MainPlugin.VoidsentFlame_StackRadius.Value);
			LanguageAPI.Add("ITEM_EXPLODEONDEATHVOID_PICKUP", pickup);
			LanguageAPI.Add("ITEM_EXPLODEONDEATHVOID_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
		}
		private static void IL_TakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 4),
				x => ILPatternMatchingExt.MatchLdarg(x, 0)
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 4),
				x => ILPatternMatchingExt.MatchLdarg(x, 0)
			);
			if (ilcursor.Index > 0)
            {
				ilcursor.Next.OpCode = OpCodes.Ldc_I4_1;
				ilcursor.Index += 1;
				ilcursor.RemoveRange(2);
				ilcursor.Emit(OpCodes.Ldc_I4_0);
			}
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 22)
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldloc, 22);
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.Emit(OpCodes.Ldloc_0);
				ilcursor.EmitDelegate<Func<int, HealthComponent, CharacterMaster, int>>((itemCount, self, attacker) =>
				{
					CharacterBody attackerBody = attacker.GetBody();
					if (attackerBody)
					{
						if (self.body != attackerBody)
						{
							Components.VoidsentTracker tracker = self.GetComponent<Components.VoidsentTracker>();
							if (!tracker)
							{
								tracker = self.gameObject.AddComponent<Components.VoidsentTracker>();
							}
							if (tracker.RegisterAttacker(attacker))
							{
								if (itemCount > 0)
								{
									MinionOwnership ownership = attacker.minionOwnership;
									if (ownership)
									{
										CharacterMaster owner = Utils.Helpers.GetOwner(ownership);
										if (owner && owner != attacker)
										{
											if(!tracker.RegisterAttacker(owner))
                                            {
												return 0;
                                            }
										}
									}
									return 1;
								}
							}
						}
					}
					return 0;
				});
			}
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 12f),
				x => ILPatternMatchingExt.MatchLdcR4(x, 2.4f)
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Next.Operand = MainPlugin.VoidsentFlame_BaseRadius.Value;
				ilcursor.Index++;
				ilcursor.Next.Operand = MainPlugin.VoidsentFlame_StackRadius.Value;
			}
		}
	}
}
