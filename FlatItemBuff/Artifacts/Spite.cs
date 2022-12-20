using System;
using RoR2;
using MonoMod.Cil;

namespace FlatItemBuff.Artifacts
{
	public class Spite
	{
		private static float BaseDamage = 12f;
		private static float LevelDamage = 2.4f;
		public Spite()
		{
			MainPlugin.ModLogger.LogInfo("Changing Artifact of Spite");
			SetupConfigValues();
			Hooks();
		}
		private void SetupConfigValues()
        {
			BaseDamage = MainPlugin.ArtifactSpite_BaseDamage.Value;
			LevelDamage = MainPlugin.ArtifactSpite_LevelDamage.Value;
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.Artifacts.BombArtifactManager.OnServerCharacterDeath += new ILContext.Manipulator(IL_CharacterDeath);
		}
		private void IL_CharacterDeath(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => x.MatchLdloca(6),
				x => x.MatchLdloc(0)
			);
			ilcursor.Index +=2;
			ilcursor.Remove();
			ilcursor.EmitDelegate<Func<CharacterBody, float>>((victimBody) =>
			{
				float damage = BaseDamage;
				if (LevelDamage != 0)
				{
					damage += LevelDamage * (victimBody.level - 1);
				}
				return damage;
			});
		}
	}
}
