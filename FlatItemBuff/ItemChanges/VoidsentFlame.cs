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
		private static float BaseRadius = 10f;
		private static float StackRadius = 2f;
		private static float BaseDamage = 2.6f;
		private static float StackDamage = 1.56f;
		private static float ProcRate = 1f;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Voidsent Flame");
			SetupConfigValues();
			UpdateText();
			Hooks();
		}

		private static void SetupConfigValues()
		{
			BaseRadius = MainPlugin.VoidsentFlame_BaseRadius.Value;
			StackRadius = MainPlugin.VoidsentFlame_StackRadius.Value;
			BaseDamage = MainPlugin.VoidsentFlame_BaseDamage.Value;
			StackDamage = MainPlugin.VoidsentFlame_StackDamage.Value;
			ProcRate = MainPlugin.VoidsentFlame_ProcRate.Value;
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = "Detonate an enemy on your first hit against them. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";
			string stackA = "";
			if (StackRadius != 0)
            {
				stackA = String.Format(" <style=cStack>(+{0}m per stack)</style>", StackRadius);
			}
			string stackB = "";
			if (StackDamage != 0)
			{
				stackB = String.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 100f);
			}
			string desc = String.Format("Upon hitting an enemy for the <style=cIsDamage>first time</style>, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>{0}m</style>{1} radius for <style=cIsDamage>{2}%</style>{3} base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.", BaseRadius, stackA, BaseDamage * 100f, stackB);
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
			//Remove max health condition
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
			//Add new condition
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
			//Damage
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 2.6f)
			);
			ilcursor.Next.Operand = BaseDamage;
			ilcursor.Index++;
			ilcursor.Next.Operand = 0f;
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 0.6f)
			);
			ilcursor.Next.Operand = StackDamage;
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchMul(x),
				x => ILPatternMatchingExt.MatchStloc(x, 22)
			);
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Add);
			//Proc Coefficient, ripped from RiskyMod
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStfld<DelayBlast>(x, "position")
			);
			ilcursor.Index--;
			ilcursor.EmitDelegate<Func<DelayBlast, DelayBlast>>((delayblast) =>
			{
				delayblast.procCoefficient = ProcRate;
				return delayblast;
			});
			//Radius
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcR4(x, 12f),
				x => ILPatternMatchingExt.MatchLdcR4(x, 2.4f)
			);
			if (ilcursor.Index > 0)
			{
				ilcursor.Next.Operand = BaseRadius;
				ilcursor.Index++;
				ilcursor.Next.Operand = StackRadius;
			}
		}
	}
}
