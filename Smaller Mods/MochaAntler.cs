using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API;
using R2API.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.AddressableAssets;

namespace MochaAntler
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.MochaAntler";
		public const string MODNAME = "MochaAntler";
		public const string MODVERSION = "1.0.1";

		public static float Stack_MoveSpeed;
		public static float Stack_Armor;
		public void Awake()
		{
			ReadConfig();
			GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));
		}
		public void PostLoad()
		{
			RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
			UpdateText();
			UpdateItemDef();
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_DisableOldEffect);
		}
		private void UpdateItemDef()
		{
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC2/Items/NegateAttack/NegateAttack.asset").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemTags = itemDef.tags.ToList();
				itemTags.Remove(ItemTag.AIBlacklist);
				itemTags.Remove(ItemTag.BrotherBlacklist);
				itemDef.tags = itemTags.ToArray();
			}
		}
		private void UpdateText()
        {
			string pickup = "Slightly increase";
			string desc = "Increases";
			if (Stack_Armor > 0f)
			{
				pickup += " armor";
				desc += string.Format(" <style=cIsHealing>armor</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>", Stack_Armor);
				if (Stack_MoveSpeed > 0f)
                {
					pickup += " and";
					desc += " and";
				}
			}
			if (Stack_MoveSpeed > 0f)
            {
				pickup += " movement speed";
				desc += string.Format(" <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style> <style=cStack>(+{0}% per stack)</style>", Stack_MoveSpeed * 100f);
			}
			LanguageAPI.Add("ITEM_NEGATEATTACK_PICKUP", pickup + ".");
			LanguageAPI.Add("ITEM_NEGATEATTACK_DESC", desc + ".");
		}
		private static void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
		{
			if (sender.inventory)
			{
				int itemCount = sender.inventory.GetItemCount(DLC2Content.Items.NegateAttack);
				if (itemCount > 0)
                {
					args.moveSpeedMultAdd += itemCount * Stack_MoveSpeed;
					args.armorAdd += itemCount * Stack_Armor;
                }
			}
		}
		public void ReadConfig()
		{
			Stack_MoveSpeed = Config.Bind<float>(new ConfigDefinition("General", "Movement Speed"), 0.07f, new ConfigDescription("Movement Speed given per stack of the item.", null, Array.Empty<object>())).Value;
			Stack_Armor = Config.Bind<float>(new ConfigDefinition("General", "Armor"), 7.5f, new ConfigDescription("Armor given per stack of the item.", null, Array.Empty<object>())).Value;
		}
		private void IL_DisableOldEffect(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "antlerShield")
			))
			{
				ilcursor.Index += 1;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MODNAME + ": IL Hook failed");
			}
		}
	}
}
