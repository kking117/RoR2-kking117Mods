using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using IL.RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.RoR2;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace FlatItemBuff
{
	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.OkIGotIt.Fresh_Bison_Steak", BepInDependency.DependencyFlags.SoftDependency)]
	[R2APISubmoduleDependency(new string[]
	{
		"LanguageAPI"
	})]
	[BepInPlugin("com.kking117.FlatItemBuff", "FlatItemBuff", "1.0.0")]
	public class Base : BaseUnityPlugin
	{
		public static ConfigEntry<float> Steak_BaseHP;
		public static ConfigEntry<float> Steak_LevelMult;
		//IL Search Stuff
		private string Steak_CatalogName = "FlatHealth";
		private int Steak_LocationOffset = 2;
		private int Steak_Location = 35;
		public void Awake()
        {
			ReadConfig();
			Logger.LogInfo("Changing Bison Steak.");
			ChangeBisonSteak();
		}
		private void ChangeBisonSteak()
        {
			Logger.LogInfo("Applying IL modifications.");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(BisonSteakILChange);
			Logger.LogInfo("Changing descriptions.");
			if (Chainloader.PluginInfos.ContainsKey("com.OkIGotIt.Fresh_Bison_Steak"))
			{
				LanguageAPI.Add("ITEM_FLATHEALTH_NAME", string.Format("Fresh Bison Steak"));
				LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", string.Format("Gain <style=cIsHealing>{0}</style> max health. <style=cIsHealing>Regenerate health</style> after killing an enemy.", Steak_BaseHP.Value));
				LanguageAPI.Add("ITEM_FLATHEALTH_DESC", string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>. Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+2 hp/s</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> after killing an enemy.", Steak_BaseHP.Value));
			}
			else
            {
				LanguageAPI.Add("ITEM_FLATHEALTH_PICKUP", string.Format("Gain <style=cIsHealing>{0}</style> max health.", Steak_BaseHP.Value));
				LanguageAPI.Add("ITEM_FLATHEALTH_DESC", string.Format("Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{0}</style> <style=cStack>(+{0} per stack)</style>.", Steak_BaseHP.Value));
			}
		}
		private void BisonSteakILChange(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", Steak_CatalogName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<RoR2.Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, Steak_Location)
			);
			ilcursor.GotoNext(0, new Func<Instruction, bool>[]
			{
				(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, Steak_Location)
			});
			ilcursor.Index += Steak_LocationOffset;
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_0);
			ilcursor.EmitDelegate<Func<RoR2.CharacterBody, float>>((bs) =>
			{
				return Steak_BaseHP.Value * (1f + (bs.level - 1f) * Steak_LevelMult.Value);
			});
		}
		public void ReadConfig()
		{
			Steak_BaseHP = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Base HP"), 25.0f, new ConfigDescription("The amount of HP each stack increases.", null, Array.Empty<object>()));
			Steak_LevelMult = Config.Bind<float>(new ConfigDefinition("Bison Steak", "Level HP"), 0.2f, new ConfigDescription("Inceases the total HP gained per level. (0.2 is what Cautious Slug uses)", null, Array.Empty<object>()));
		}
	}
}
