using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.CharacterAI;

namespace QueenGlandBuff.Utils
{
    internal class Helpers
    {
		private static readonly System.Random rng = new System.Random();
		internal static void GiveRandomEliteAffix(CharacterMaster self)
		{
			if(Changes.QueensGland.AffixMode == 0)
            {
				return;
            }
			if (Changes.QueensGland.AffixMode == 2 && !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
				return;
            }
			if (Changes.QueensGland.StageEliteEquipmentDefs.Count > 0)
            {
				int result = rng.Next(Changes.QueensGland.StageEliteEquipmentDefs.Count);
				if (Changes.QueensGland.StageEliteEquipmentDefs[result])
				{
					self.inventory.SetEquipmentIndex(Changes.QueensGland.StageEliteEquipmentDefs[result].equipmentIndex);
					return;
				}
			}
			self.inventory.SetEquipmentIndex(Changes.QueensGland.DefaultAffixIndex);
		}
	}
}
