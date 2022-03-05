using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace TestTest.Characters
{
    public class VoidInfestor
    {
        private static GameObject Prefab;
        public static void Begin()
        {
            Prefab = BodyCatalog.FindBodyPrefab("VoidInfestorBody");
			if (Prefab)
			{
				ModSkills();
				IL.RoR2.GlobalEventManager.OnCharacterDeath += new ILContext.Manipulator(IL_CharacterDeath);
			}
        }
        public static void ModSkills()
        {
			SkillLocator skillLocator = Prefab.GetComponent<SkillLocator>();
			SkillFamily skillFamily = skillLocator.primary.skillFamily;
			SkillDef skill = skillFamily.variants[0].skillDef;
			if(skill)
            {
				skill.activationState = new SerializableEntityStateType(typeof(States.Infest));
				Modules.States.RegisterState(typeof(States.Infest));
			}
		}
		private static void IL_CharacterDeath(ILContext il)
        {
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchStloc(x, 31),
				x => ILPatternMatchingExt.MatchLdloc(x, 31)
			);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdcI4(x, 4)
			);
			ilcursor.Remove();
			ilcursor.Emit(OpCodes.Ldarg_1);
			ilcursor.EmitDelegate<Func<DamageReport, TeamIndex>>((dr) =>
			{
				if(dr.victimBody)
                {
					return dr.victimTeamIndex;
                }
				return TeamIndex.Void;
			});
		}
	}
}
