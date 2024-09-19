using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace AmbientSpite
{
	[BepInPlugin(MODUID, MODNAME, MODVERSION)]
	public class MainPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.kking117.AmbientSpite";
		public const string MODNAME = "AmbientSpite";
		public const string MODVERSION = "1.0.0";

		public static float BaseDamage;
		public static float LevelDamage;
		public void Awake()
		{
			ReadConfig();
			IL.RoR2.Artifacts.BombArtifactManager.OnServerCharacterDeath += new ILContext.Manipulator(IL_CharacterDeath);
		}
		public void ReadConfig()
		{
			BaseDamage = Config.Bind("General", "Base Damage", 12f, "Damage of Spite Bombs at level 1.").Value;
			LevelDamage = Config.Bind("General", "Level Damage", 2.4f, "Damage Spite Bombs gain per additional level.").Value;
		}
		private void IL_CharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchCallvirt<CharacterBody>("get_damage")
			))
			{
				ilcursor.Remove();
				ilcursor.EmitDelegate<Func<CharacterBody, float>>((victimBody) =>
				{
					float level = Math.Max(0f, Run.instance.ambientLevel -1f);
					float damage = BaseDamage + (LevelDamage * level);
					return damage;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ":  IL Hook failed");
			}
		}
	}
}
