using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
	public class BensRaincoat
	{
		public BensRaincoat()
		{
			MainPlugin.ModLogger.LogInfo("Changing Ben's Raincoat");
			UpdateText();
			Hooks();
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = String.Format("Prevents <style=cIsUtility>{0} <style=cStack>(+{1} per stack)</style></style> <style=cIsDamage>debuff</style> and instead grants a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>10%</style> of <style=cIsHealing>maximum health</style>. Recharges every <style=cIsUtility>{2}</style> seconds.", MainPlugin.BensRaincoat_BaseBlock.Value, MainPlugin.BensRaincoat_StackBlock.Value, MainPlugin.BensRaincoat_Cooldown.Value);
			LanguageAPI.Add("ITEM_IMMUNETODEBUFF_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += new ILContext.Manipulator(IL_TryApplyOverride);
			if(MainPlugin.BensRaincoat_FixCooldown.Value)
            {
				On.RoR2.Items.ImmuneToDebuffBehavior.FixedUpdate += ImmuneToDebuff_FixedUpdate_Improved;
			}
			else
            {
				On.RoR2.Items.ImmuneToDebuffBehavior.FixedUpdate += ImmuneToDebuff_FixedUpdate;
			}
			
		}
		private void ImmuneToDebuff_FixedUpdate_Improved(On.RoR2.Items.ImmuneToDebuffBehavior.orig_FixedUpdate orig, RoR2.Items.ImmuneToDebuffBehavior self)
        {
			self.isProtected = false;
			int buffCount = MainPlugin.BensRaincoat_BaseBlock.Value + ((self.stack - 1) * MainPlugin.BensRaincoat_StackBlock.Value);
			int curbuffCount = self.body.GetBuffCount(DLC1Content.Buffs.ImmuneToDebuffReady);
			if (!self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffCooldown))
			{
				for (int i = 0; i < buffCount - curbuffCount; i++)
				{
					self.body.AddBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
				}
			}
		}
		private void ImmuneToDebuff_FixedUpdate(On.RoR2.Items.ImmuneToDebuffBehavior.orig_FixedUpdate orig, RoR2.Items.ImmuneToDebuffBehavior self)
		{
			self.isProtected = false;
			bool onCool = self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffCooldown);
			bool isReady = self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
			if (!onCool && !isReady)
			{
				int buffCount = MainPlugin.BensRaincoat_BaseBlock.Value + ((self.stack - 1) * MainPlugin.BensRaincoat_StackBlock.Value);
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
		private void IL_TryApplyOverride(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (MainPlugin.BensRaincoat_FixCooldown.Value)
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
			if (MainPlugin.BensRaincoat_Cooldown.Value >= 0f)
			{
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdarg(x, 0),
					x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Buffs", "ImmuneToDebuffCooldown"),
					x => ILPatternMatchingExt.MatchLdcR4(x, 5),
					x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "AddTimedBuff")
				);
				ilcursor.Index += 2;
				ilcursor.Next.Operand = MainPlugin.BensRaincoat_Cooldown.Value;
			}
		}
	}
}
