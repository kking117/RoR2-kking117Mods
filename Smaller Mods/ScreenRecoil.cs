using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using RoR2;
using System.Security.Permissions;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ScreenRecoil
{
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class ScreenRecoil : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.ScreenRecoil";
		public const string MODNAME = "ScreenRecoil";
		public const string MODTOKEN = "KKING117_SCREENRECOIL_";
		public const string MODVERSION = "1.0.0";

		//internal static BepInEx.Logging.ManualLogSource ModLogger;

		public static ConfigEntry<float> RecoilMult;
		private static bool RiskOfOptionsLoaded = false;
		private void Awake()
        {
			RiskOfOptionsLoaded = Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
			//ModLogger = this.Logger;
			ReadConfig();
			On.RoR2.CameraTargetParams.AddRecoil += OnAddRecoil;
        }

		private void OnAddRecoil(On.RoR2.CameraTargetParams.orig_AddRecoil orig, CameraTargetParams self, float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
        {
			CharacterBody body = self.GetComponent<CharacterBody>();
			if (body)
            {
				CharacterMaster master = body.master;
				if (master)
				{
					if (master.playerCharacterMasterController && master.isClient)
					{
						float mult = RecoilMult.Value;
						verticalMax *= mult;
						verticalMin *= mult;
						horizontalMax *= mult;
						horizontalMin *= mult;
					}
				}
			}
			orig(self, verticalMin, verticalMax, horizontalMin, horizontalMax);
		}
		private void ReadConfig()
        {
			RecoilMult = Config.Bind<float>(new ConfigDefinition("Screen Recoil", "Screen Recoil Scale"), 1.0f, new ConfigDescription("How much of a badass are you? (1 = Default)", null, Array.Empty<object>()));
			if (RiskOfOptionsLoaded)
			{
				SetupOptions();
			}
		}

		private void SetupOptions()
        {
			ModSettingsManager.AddOption(new StepSliderOption(RecoilMult, new StepSliderConfig() { min = 0f, max = 10f, increment = 0.01f }));
		}
	}
}
