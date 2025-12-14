using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.ExpansionManagement;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Railroad.Changes
{
    public class InteractablePaths
    {
        internal static bool Enable = true;

        private List<InteractableSpawnCard> HalcyoniteShrines = new List<InteractableSpawnCard>();

        //"RoR2/DLC2/iscShrineHalcyonite.asset"
        private InteractableSpawnCard HalcyonShrine = Addressables.LoadAssetAsync<InteractableSpawnCard>("0fc0fd67b3dab9645acb561484c72d28").WaitForCompletion();
        //"RoR2/DLC2/iscShrineHalcyoniteTier1.asset"
        private InteractableSpawnCard HalcyonShrineT1 = Addressables.LoadAssetAsync<InteractableSpawnCard>("73074fe5c5128b04b85a42385324f447").WaitForCompletion();

        private List<InteractableSpawnCard> AccessNodes = new List<InteractableSpawnCard>();

        //"RoR2/DLC3/iscACPortalConduitCanyon.asset"
        //private InteractableSpawnCard HalcyonShrine = Addressables.LoadAssetAsync<InteractableSpawnCard>("0fc0fd67b3dab9645acb561484c72d28").WaitForCompletion();
        //"RoR2/DLC3/iscAcPortalIronAlluvium.asset"
        //private InteractableSpawnCard HalcyonShrineT1 = Addressables.LoadAssetAsync<InteractableSpawnCard>("73074fe5c5128b04b85a42385324f447").WaitForCompletion();
        public InteractablePaths()
        {
            if (!Enable)
            {
                return;
            }
            Hooks();
            CreateLists();
        }

        private void CreateLists()
        {
            if (HalcyonShrine != null)
            {
                HalcyoniteShrines.Add(HalcyonShrine);
            }
            if (HalcyonShrineT1 != null)
            {
                HalcyoniteShrines.Add(HalcyonShrineT1);
            }
        }
        private void Hooks()
        {
            //On.RoR2.DCCSBlender.GetBlendedDCCS += GetBlendedCards;
        }
        /*8private DirectorCardCategorySelection GetBlendedCards(On.RoR2.DCCSBlender.orig_GetBlendedDCCS orig, DccsPool.Category poolCategory, ref Xoroshiro128Plus rng, ClassicStageInfo stageInfo, int contentMixLimit, List<ExpansionDef> requiredDLCList)
        {
            SceneDef currentSceneDef = Stage.instance.sceneDef;
            DirectorCardCategorySelection cardList = orig.Invoke(poolCategory, ref rng, stageInfo, contentMixLimit, requiredDLCList);
            for(int i = 0; i< cardList.categories.Length; i++)
            {
                DirectorCard[] cardList = cardList.categories[i].cards;
                foreach (DirectorCard card in cardList)
                {
                    if (HalcyoniteShrines.Contains(card.spawnCard))
                    {
                        if (currentSceneDef.stageOrder < 3 && currentSceneDef.stageOrder > 4)
                        {
                            ((InteractableSpawnCard)card.spawnCard).maxSpawnsPerStage = 0;
                            ((InteractableSpawnCard)card.spawnCard).
                        }
                    }
                }
            }
            return cardList;
        }*/
    }
}