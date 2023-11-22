using System;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Mono.Cecil;
using UnityEngine;
using FlatItemBuff.Utils;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class BensRaincoat
	{
		internal static bool Enable = true;
		internal static int BaseBlock = 2;
		internal static int StackBlock = 1;
		internal static float Cooldown = 7f;
		internal static float GraceTime = 0.25f;
		public BensRaincoat()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Ben's Raincoat");
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
        {
			Math.Max(1, BaseBlock);
			Math.Max(0, StackBlock);
			Math.Max(0f, Cooldown);
			Math.Max(0f, GraceTime);
        }
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = String.Format("Prevents <style=cIsUtility>{0} <style=cStack>(+{1} per stack)</style></style> <style=cIsDamage>debuffs</style> and instead grants a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>10%</style> of <style=cIsHealing>maximum health</style>. Recharges every <style=cIsUtility>{2}</style> seconds.", BaseBlock, StackBlock, Cooldown);
			LanguageAPI.Add("ITEM_IMMUNETODEBUFF_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += new ILContext.Manipulator(IL_TryApplyOverride);
			On.RoR2.Items.ImmuneToDebuffBehavior.FixedUpdate += ImmuneToDebuff_FixedUpdate;
		}
		private void ImmuneToDebuff_FixedUpdate(On.RoR2.Items.ImmuneToDebuffBehavior.orig_FixedUpdate orig, RoR2.Items.ImmuneToDebuffBehavior self)
        {
			self.isProtected = false;
			if (!self.body.HasBuff(DLC1Content.Buffs.ImmuneToDebuffCooldown))
			{
				int buffCount = BaseBlock + ((self.stack - 1) * StackBlock);
				buffCount -= self.body.GetBuffCount(DLC1Content.Buffs.ImmuneToDebuffReady);
				for (int i = 0; i < buffCount; i++)
				{
					self.body.AddBuff(DLC1Content.Buffs.ImmuneToDebuffReady);
				}
			}
		}
		private void IL_TryApplyOverride(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (GraceTime > 0f)
			{
				ilcursor.GotoNext(
						x => ILPatternMatchingExt.MatchLdloc(x, 0),
						x => ILPatternMatchingExt.MatchLdfld(x, typeof(RoR2.Items.ImmuneToDebuffBehavior), "isProtected")
					);
				ilcursor.RemoveRange(2);
				ilcursor.Emit(OpCodes.Ldarg_0);
				ilcursor.EmitDelegate<Func<CharacterBody, bool>>((body) =>
				{
					Components.RaincoatBuffer raincoatBuffer = body.GetComponent<Components.RaincoatBuffer>();
					if (!raincoatBuffer)
                    {
						raincoatBuffer = body.gameObject.AddComponent<Components.RaincoatBuffer>();
					}
					return raincoatBuffer.IsActive();
				});
				MainPlugin.ModLogger.LogInfo("Grace Time Boolean Done");
			}

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
			if (Cooldown >= 0f)
			{
				ilcursor.GotoNext(
					x => ILPatternMatchingExt.MatchLdarg(x, 0),
					x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.DLC1Content/Buffs", "ImmuneToDebuffCooldown"),
					x => ILPatternMatchingExt.MatchLdcR4(x, 5),
					x => ILPatternMatchingExt.MatchCallOrCallvirt<CharacterBody>(x, "AddTimedBuff")
				);
				ilcursor.Index += 2;
				ilcursor.Next.Operand = Cooldown;
			}

			if (GraceTime > 0f)
			{
				ilcursor.GotoNext(
						x => ILPatternMatchingExt.MatchLdloc(x, 0),
						x => ILPatternMatchingExt.MatchLdcI4(x, 1)
					);
				ilcursor.Index++;
				ilcursor.RemoveRange(3);
				ilcursor.EmitDelegate<Func<RoR2.Items.ImmuneToDebuffBehavior, bool>>((itemBehavior) =>
				{
					CharacterBody body = itemBehavior.body;
					Components.RaincoatBuffer raincoatBuffer = body.GetComponent<Components.RaincoatBuffer>();
					if (!raincoatBuffer)
					{
						raincoatBuffer = body.gameObject.AddComponent<Components.RaincoatBuffer>();
					}
					raincoatBuffer.Refresh();
					return true;
				});
				MainPlugin.ModLogger.LogInfo("Apply Grace Time Done");
			}
		}
	}
}
