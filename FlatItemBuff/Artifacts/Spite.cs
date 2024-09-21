using System;
using RoR2;
using MonoMod.Cil;

namespace FlatItemBuff.Artifacts
{
	public class Spite
	{
		internal static bool Enable = false;
		internal static float BaseDamage = 12f;
		internal static float LevelDamage = 2.4f;
		public Spite()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Artifact of Spite");
			Hooks();
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
				x => x.MatchCallvirt<CharacterBody>("get_damage")
			);
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
