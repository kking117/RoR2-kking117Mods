﻿using System;
using RoR2;
using R2API;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using RoR2.Projectile;

namespace FlatItemBuff.ItemChanges
{
	public class TitanicKnurl_Rework
	{
		private static string IL_ItemName = "Knurl";
		private static int IL_LocationOffset = 2;
		private static int IL_Location = 16;

		public static GameObject StoneFistProjectile;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Titanic Knurl");
			UpdateText();
			CreateProjectiles();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("Watched over by a stone guardian.");
			string desc = string.Format("Every <style=cIsUtility>{0}</style> seconds <style=cStack>(+{1}% attack speed per stack)</style> the weakest enemy within <style=cIsUtility>50m</style> is attacked by a <style=cIsDamage>Stone Titan's fist</style> for <style=cIsDamage>{2}%</style> <style=cStack>(+{3}% per stack)</style> damage.", MainPlugin.Knurl_BaseSpeed.Value, MainPlugin.Knurl_StackSpeed.Value * 100f, MainPlugin.Knurl_BaseDamage.Value * 100f, MainPlugin.Knurl_StackDamage.Value * 100f);
			LanguageAPI.Add("ITEM_KNURL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNURL_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
		}
		private static void CreateProjectiles()
        {
			StoneFistProjectile = Resources.Load<GameObject>("prefabs/projectiles/titanprefistprojectile").InstantiateClone(MainPlugin.MODTOKEN + "StonePreFist", true);
			ProjectileDamage projDmg = StoneFistProjectile.GetComponent<ProjectileDamage>();
			projDmg.damageType = DamageType.Stun1s;
			Modules.Projectiles.AddProjectile(StoneFistProjectile);
		}
		private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, RoR2.CharacterBody self)
		{
			orig(self);
			if(NetworkServer.active)
            {
				self.AddItemBehavior<TitanicKnurl_Behavior>(self.inventory.GetItemCount(RoR2Content.Items.Knurl));
			}
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", IL_ItemName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, IL_Location)
			);
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, IL_Location)
			});
			ilcursor.Index += IL_LocationOffset;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((bs) =>
			{
				return 0f;
			});
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, IL_Location)
			});
			ilcursor.Index += IL_LocationOffset;
			ilcursor.RemoveRange(3);
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((bs) =>
			{
				return 0f;
			});
			
		}
	}
}