using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using FlatItemBuff.Components;

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
			string pickup = "The first hit on every enemy causes an explosion. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";
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
				x => ILPatternMatchingExt.MatchLdloc(x, 20)
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Remove();
				ilcursor.Emit(OpCodes.Ldloc, 20);
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.Emit(OpCodes.Ldloc_1);
				ilcursor.EmitDelegate<Func<int, HealthComponent, CharacterBody, int>>((itemCount, self, attackerBody) =>
				{
					if (self.body != attackerBody)
					{
						VoidsentTracker tracker = self.GetComponent<VoidsentTracker>();
						if (!tracker)
						{
							tracker = self.gameObject.AddComponent<VoidsentTracker>();
						}
						if (tracker.RegisterAttacker(attackerBody.master))
						{
							if (itemCount > 0)
							{
								MinionOwnership ownership = attackerBody.master.minionOwnership;
								if (ownership)
								{
									CharacterMaster owner = Utils.Helpers.GetOwner(ownership);
									if (owner && owner != attackerBody.master)
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
