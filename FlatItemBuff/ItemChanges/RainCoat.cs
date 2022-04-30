using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.ItemChanges
{
	public class RainCoat
	{
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing RainCoat");
			UpdateText();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = String.Format("Prevents <style=cIsUtility>{0} <style=cStack>(+{1} per stack)</style></style> <style=cIsDamage>debuff</style> and instead grants a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>10%</style> of <style=cIsHealing>maximum health</style>. Recharges every <style=cIsUtility>{2}</style> seconds.", MainPlugin.RainCoat_BaseBlock.Value, MainPlugin.RainCoat_StackBlock.Value, MainPlugin.RainCoat_Cooldown.Value);
			LanguageAPI.Add("ITEM_IMMUNETODEBUFF_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += new ILContext.Manipulator(IL_TryApplyOverride);
			if(MainPlugin.RainCoat_ImproveCooldown.Value)
            {
				On.RoR2.Items.ImmuneToDebuffBehavior.FixedUpdate += ImmuneToDebuff_FixedUpdate_Improved;
			}
			else
            {
				On.RoR2.Items.ImmuneToDebuffBehavior.FixedUpdate += ImmuneToDebuff_FixedUpdate;
			}
			
		}
		private static void ImmuneToDebuff_FixedUpdate_Improved(On.RoR2.Items.ImmuneToDebuffBehavior.orig_FixedUpdate orig, RoR2.Items.ImmuneToDebuffBehavior self)
        {
			self.isProtected = false;
			int buffCount = MainPlugin.RainCoat_BaseBlock.Value + ((self.stack - 1) * MainPlugin.RainCoat_StackBlock.Value);
			int curbuffCount = self.body.GetBuffCount(DLC1Content.Buffs.ImmuneToDebuffReady);
			if (!self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffCooldown))
			{
				for (int i = 0; i < buffCount - curbuffCount; i++)
				{
					self.body.AddBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
				}
			}
		}
		private static void ImmuneToDebuff_FixedUpdate(On.RoR2.Items.ImmuneToDebuffBehavior.orig_FixedUpdate orig, RoR2.Items.ImmuneToDebuffBehavior self)
		{
			self.isProtected = false;
			bool onCool = self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffCooldown);
			bool isReady = self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
			if (!onCool && !isReady)
			{
				int buffCount = MainPlugin.RainCoat_BaseBlock.Value + ((self.stack - 1) * MainPlugin.RainCoat_StackBlock.Value);
				int curbuffCount = self.body.GetBuffCount(DLC1Content.Buffs.ImmuneToDebuffReady);
				for (int i = 0; i < buffCount - curbuffCount; i++)
				{
					self.body.AddBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
				}
			}
			if (onCool && isReady)
			{
				self.body.RemoveBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
			}
		}
		private static void IL_TryApplyOverride(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (MainPlugin.RainCoat_ImproveCooldown.Value)
			{
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdarg(x, 0),
					x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Buffs", "ImmuneToDebuffReady"),
					x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "HasBuff")
				);
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdarg(x, 0),
					x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Buffs", "ImmuneToDebuffReady"),
					x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "HasBuff")
				);

				ilcursor.Index++;
				ilcursor.RemoveRange(2);
				ilcursor.EmitDelegate<Func<CharacterBody, bool>>((body) =>
				{
					return body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffCooldown);
				});
			}
			if (MainPlugin.RainCoat_Cooldown.Value >= 0f)
			{
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdarg(x, 0),
					x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Buffs", "ImmuneToDebuffCooldown"),
					x => ILPatternMatchingExt.MatchLdcR4(x, 5),
					x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "AddTimedBuff")
				);
				ilcursor.Index += 2;
				ilcursor.Next.Operand = MainPlugin.RainCoat_Cooldown.Value;
			}
		}
	}
}
