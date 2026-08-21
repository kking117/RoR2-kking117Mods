using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Railroad.Changes
{
    public class Interactables
    {
        internal static bool Frog_Enable = false;
        internal static ConfigPortalType Frog_Portal = ConfigPortalType.DeepVoid;
        internal static int Frog_Pet_Count = 10;
        internal static ConfigCurrencyType Frog_Pet_Currency = ConfigCurrencyType.LunarCoin;
        internal static bool Frog_Stop_Petting = false;

        internal static GameObject GlassFrog = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/moon/FrogInteractable.prefab").WaitForCompletion();
        public Interactables()
        {
            ClampConfig();
            Hooks();
            ModifyObjects();
        }
        private void ClampConfig()
        {
            Frog_Portal = (ConfigPortalType)Math.Min((int)Frog_Portal, (int)ConfigPortalType.SolusBackout);
            Frog_Pet_Currency = (ConfigCurrencyType)Math.Min((int)Frog_Pet_Currency, (int)ConfigCurrencyType.VoidCoin);
        }
        private void ModifyObjects()
        {
            if (Frog_Enable && GlassFrog)
            {
                PurchaseInteraction purchaseInteraction = GlassFrog.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction)
                {
                    switch (Frog_Pet_Currency)
                    {
                        case ConfigCurrencyType.LunarCoin:
                            purchaseInteraction.cost = 1;
                            purchaseInteraction.costType = CostTypeIndex.LunarCoin;
                            break;
                        case ConfigCurrencyType.VoidCoin:
                            purchaseInteraction.cost = 1;
                            purchaseInteraction.costType = CostTypeIndex.VoidCoin;
                            break;
                        default:
                            purchaseInteraction.cost = 0;
                            purchaseInteraction.costType = CostTypeIndex.None;
                            break;
                    }
                }
            }
        }
        private void Hooks()
        {
            if (Frog_Enable)
            {
                On.RoR2.FrogController.Pet += FrogOnPet;
                if (Frog_Stop_Petting)
                {
                    On.RoR2.PurchaseInteraction.GetInteractability += GetInteractability;
                }
            }
        }
        private Interactability GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            if (self.gameObject)
            {
                FrogController frogController = self.gameObject.GetComponent<FrogController>();
                if (frogController && frogController.petCount >= frogController.maxPets)
                {
                    return Interactability.Disabled;
                }
            }
            return orig(self, activator);
        }
        private void FrogOnPet(On.RoR2.FrogController.orig_Pet orig, FrogController self, Interactor interactor)
        {
            //Works but needs a few fixes and changes:
            //Have the interactor be disabled after maxing out the petcount? - DONE, COULD BE IMPLEMENTED BETTER.
            //Only works if SOTV is enabled, the interactable still exists but you can only keep petting it. - FIXED - Likely done here because of SOTV.
            //Uses the Deep Void Portal chat line regardless of what it spawns. - FIXED
            //Errors if the interactable spawn card is null. - Not our problem and this should fix it in most cases.
            
            //fix up the portal
            if (self.petCount == 0)
            {
                self.maxPets = Frog_Pet_Count;
                if (self.portalSpawner)
                {
                    bool nullSpawner = true;
                    if (PortalUtility.CanSpawnPortal(Frog_Portal))
                    {
                        InteractableSpawnCard portalCard = PortalUtility.GetSpawnCardFromIndex(Frog_Portal);
                        if (portalCard)
                        {
                            self.portalSpawner.willSpawn = true;
                            self.portalSpawner.spawnChance = 1f;
                            self.portalSpawner.portalSpawnCard = portalCard;
                            self.portalSpawner.spawnMessageToken = PortalUtility.GetSpawnMessageFromIndex(Frog_Portal);
                            nullSpawner = false;
                        }
                    }
                    if (nullSpawner)
                    {
                        self.portalSpawner.willSpawn = false;
                    }
                }
            }
            orig(self, interactor);
        }
    }
}