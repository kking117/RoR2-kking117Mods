using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZoeaRework.Changes
{
    public class VoidMegaCrabAllyDisplays
    {
        static List<ItemDisplayRuleSet.KeyAssetRuleGroup> idrgroups;
        internal static void Create(GameObject body)
        {
            if(body)
            {
                ItemDisplayRuleSet idr = body.GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;
                if(idr)
                {
                    idrgroups = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>(idr.keyAssetRuleGroups);
                    BaseGameDisplays();
                    DLC1Displays();
                    idr.keyAssetRuleGroups = idrgroups.ToArray();
                    idr.GenerateRuntimeValues();
                }
            }
        }
        private static void BaseGameDisplays()
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup Syringe = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Syringe/DisplaySyringeCluster.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.39005F, -0.40184F, 0.17513F),
                            localAngles = new Vector3(6.19884F, 11.93964F, 202.211F),
                            localScale = new Vector3(0.35F, 0.35F, 0.35F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Mushroom = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Mushroom,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mushroom/DisplayMushroom.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-7.24997F, 6.34788F, 0.83195F),
                            localAngles = new Vector3(331.8201F, 89.92665F, 11.17588F),
                            localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ArmorReductionOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ArmorReductionOnHit/DisplayWarhammer.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(5.59819F, 6.85389F, 4.67879F),
                            localAngles = new Vector3(25.8489F, 315.801F, 242.306F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Hoof = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Hoof,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Hoof/DisplayHoof.prefab").WaitForCompletion(),
                            childName = "FrontFootL",
                            localPos = new Vector3(1.77528F, 0.35457F, -2.70543F),
                            localAngles = new Vector3(7.39844F, 194.4065F, 267.6492F),
                            localScale = new Vector3(1.1F, 1.1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup GhostOnKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/GhostOnKill/DisplayMask.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.05595F, 2.59788F, 6.22897F),
                            localAngles = new Vector3(354.6444F, 3.46166F, 359.274F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BarrierOnOverHeal = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BarrierOnOverHeal/DisplayAegis.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.10451F, 0.27667F, 2.3546F),
                            localAngles = new Vector3(25.35371F, 161.0139F, 307.2729F),
                            localScale = new Vector3(0.75F, 0.75F, 0.75F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Bear = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bear/DisplayBear.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-6.34222F, 5.58901F, 3.30526F),
                            localAngles = new Vector3(316.3687F, 0.70888F, 3.04792F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ExtraLife = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ExtraLife/DisplayHippo.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(0.90413F, -0.54082F, 1.44995F),
                            localAngles = new Vector3(6.94992F, 11.81551F, 153.7545F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Bandolier = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bandolier,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/DisplayBandolier.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(-0.19679F, 0.05809F, 0.79148F),
                            localAngles = new Vector3(345.0268F, 7.04977F, 187.5542F),
                            localScale = new Vector3(1.6F, 1.3F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Clover = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Clover,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Clover/DisplayClover.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(2.17497F, 4.69877F, 6.01925F),
                            localAngles = new Vector3(16.34685F, 26.73651F, 354.8405F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FlatHealth = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FlatHealth/DisplaySteakCurved.prefab").WaitForCompletion(),
                            childName = "LeftBackMuzzle",
                            localPos = new Vector3(-1.73625F, 0.84787F, 1.76866F),
                            localAngles = new Vector3(4.97204F, 334.3797F, 26.00761F),
                            localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Crowbar = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Crowbar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Crowbar/DisplayCrowbar.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-0.06608F, 4.02972F, 3.87907F),
                            localAngles = new Vector3(62.55939F, 236.7019F, 52.87563F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup HealWhileSafe = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealWhileSafe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/HealWhileSafe/DisplaySnail.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-3.36291F, 2.84721F, -7.36829F),
                            localAngles = new Vector3(78.64568F, 352.0094F, 121.7129F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Knurl = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Knurl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Knurl/DisplayKnurl.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(0.79417F, 1.24345F, 3.46039F),
                            localAngles = new Vector3(321.2226F, 346.0586F, 9.07347F),
                            localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Medkit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Medkit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Medkit/DisplayMedkit.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-1.01786F, 0.08924F, -0.39725F),
                            localAngles = new Vector3(325.8983F, 304.7193F, 31.60635F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ShieldOnly = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShieldOnly,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShieldOnly/DisplayShieldBug.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-1.50608F, 4.48408F, 6.18018F),
                            localAngles = new Vector3(1.86569F, 248.5332F, 349.3364F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShieldOnly/DisplayShieldBug.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(1.51133F, 4.48367F, 6.14645F),
                            localAngles = new Vector3(359.9082F, 260.8006F, 342.6776F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BossDamageBonus = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BossDamageBonus/DisplayAPRound.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(3.00535F, 0.78586F, 1.52898F),
                            localAngles = new Vector3(320.6469F, 71.59185F, 137.2397F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SecondarySkillMagazine = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SecondarySkillMagazine/DisplayDoubleMag.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(0.2643F, -0.65264F, 0.36937F),
                            localAngles = new Vector3(8.93963F, 170.9112F, 352.6168F),
                            localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Firework = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Firework,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Firework/DisplayFirework.prefab").WaitForCompletion(),
                            childName = "LeftBackMuzzle",
                            localPos = new Vector3(-0.27986F, 1.30431F, 1.69172F),
                            localAngles = new Vector3(331.8425F, 5.50309F, 18.60096F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SprintBonus = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintBonus/DisplaySoda.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(1.25F, -0.38695F, -8.2621F),
                            localAngles = new Vector3(353.9646F, 181.2857F, 179.9379F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintBonus/DisplaySoda.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-1.25F, -0.38695F, -8.2621F),
                            localAngles = new Vector3(353.9646F, 181.2857F, 179.9379F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup NearbyDamageBonus = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NearbyDamageBonus/DisplayDiamond.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.00037F, 0.75621F, 6.36918F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup IgniteOnKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IgniteOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/DisplayGasoline.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-3.12309F, -0.17638F, -6.84268F),
                            localAngles = new Vector3(279.8721F, 271.9936F, 258.4198F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup CritGlasses = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CritGlasses,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CritGlasses/DisplayGlasses.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.0019F, 2.1526F, 6.91666F),
                            localAngles = new Vector3(0.05925F, 359.9326F, 5.02378F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup StickyBomb = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/StickyBomb/DisplayStickyBomb.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(-0.19873F, -0.21353F, -0.04174F),
                            localAngles = new Vector3(321.2897F, 281.0095F, 283.2007F),
                            localScale = new Vector3(0.4F, 0.55F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup WardOnLevel = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WardOnLevel,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WardOnLevel/DisplayWarbanner.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.01464F, 4.96713F, -8.71434F),
                            localAngles = new Vector3(313F, 0F, 270F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BleedOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHit/DisplayTriTip.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-4.51696F, 1.66186F, -7.91377F),
                            localAngles = new Vector3(334.9604F, 13.83776F, 249.1614F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ArmorPlate = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ArmorPlate/DisplayRepulsionArmorPlate.prefab").WaitForCompletion(),
                            childName = "FrontFootR",
                            localPos = new Vector3(-1.40385F, 0.09795F, -4.78844F),
                            localAngles = new Vector3(355.8022F, 180.2313F, 104.5135F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup PersonalShield = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.PersonalShield,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/PersonalShield/DisplayShieldGenerator.prefab").WaitForCompletion(),
                            childName = "BackFootR",
                            localPos = new Vector3(1.35223F, -0.04753F, -7.11869F),
                            localAngles = new Vector3(357.4218F, 347.9545F, 89.35858F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup TreasureCache = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/TreasureCache/DisplayKey.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(4.98828F, 3.09758F, -7.59614F),
                            localAngles = new Vector3(298.405F, 44.15102F, 357.8575F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup StunChanceOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StunChanceOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/StunChanceOnHit/DisplayStunGrenade.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(0.37671F, -0.23801F, -0.08846F),
                            localAngles = new Vector3(316.01F, 278.6631F, 201.1436F),
                            localScale = new Vector3(1F, 1F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BarrierOnKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BarrierOnKill/DisplayBrooch.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.0029F, 7.34315F, -0.25008F),
                            localAngles = new Vector3(352.8255F, 179.9946F, 0.08074F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Missile = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Missile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Missile/DisplayMissileLauncher.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-2.22808F, -1.14637F, -0.0298F),
                            localAngles = new Vector3(56.88838F, 140.8108F, 186.1091F),
                            localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup WarCryOnMultiKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WarCryOnMultiKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/WarCryOnMultiKill/DisplayPauldron.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-10.88995F, 3.92403F, 1.90454F),
                            localAngles = new Vector3(304.1131F, 104.6495F, 1.79026F),
                            localScale = new Vector3(8F, 8F, 8F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup DeathMark = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/DeathMark/DisplayDeathMark.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.03703F, 6.63459F, 3.27862F),
                            localAngles = new Vector3(282.6846F, 197.1187F, 162.688F),
                            localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SlowOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SlowOnHit/DisplayBauble.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(2.07741F, -1.85865F, 7.74365F),
                            localAngles = new Vector3(0.372F, 275.0918F, 347.6111F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup EquipmentMagazine = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EquipmentMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EquipmentMagazine/DisplayBattery.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.43402F, 0.51268F, 2.0127F),
                            localAngles = new Vector3(321.1273F, 298.1721F, 66.25185F),
                            localScale = new Vector3(0.2F, 0.3F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BonusGoldPackOnKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BonusGoldPackOnKill/DisplayTome.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-3.13285F, 4.53966F, 6.46402F),
                            localAngles = new Vector3(307.8306F, 0.65489F, 343.6791F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup HealOnCrit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/HealOnCrit/DisplayScythe.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-3.60245F, -1.06912F, -5.30033F),
                            localAngles = new Vector3(301.0326F, 305.4293F, 235.7309F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Feather = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Feather,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Feather/DisplayFeather.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.20705F, 4.52067F, 5.58461F),
                            localAngles = new Vector3(312.2155F, 205.607F, 340.9738F),
                            localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Infusion = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Infusion,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Infusion/DisplayInfusion.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.61398F, -0.26485F, 0.30786F),
                            localAngles = new Vector3(320.7129F, 260.3998F, 7.8722F),
                            localScale = new Vector3(0.5F, 0.4F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FireRing = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/DisplayFireRing.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.05142F, -0.01417F, 0.25451F),
                            localAngles = new Vector3(358.6336F, 358.5022F, 359.3903F),
                            localScale = new Vector3(1.75F, 1.75F, 1.75F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup IceRing = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/DisplayIceRing.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.03216F, -0.00079F, 0.04979F),
                            localAngles = new Vector3(358.6336F, 358.5022F, 359.3903F),
                            localScale = new Vector3(1.75F, 1.75F, 1.75F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup TPHealingNova = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/TPHealingNova/DisplayGlowFlower.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.01528F, -0.00841F, 0.09519F),
                            localAngles = new Vector3(0F, 180F, 0F),
                            localScale = new Vector3(1F, 1F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ExecuteLowHealthElite = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ExecuteLowHealthElite/DisplayGuillotine.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(0.00915F, -0.54104F, 0.13135F),
                            localAngles = new Vector3(2.71975F, 355.045F, 14.85856F),
                            localScale = new Vector3(0.4F, 0.4F, 0.6F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Phasing = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Phasing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Phasing/DisplayStealthkit.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0F, -2.05419F, -7.60193F),
                            localAngles = new Vector3(53.83344F, 0.64216F, 0.64031F),
                            localScale = new Vector3(5F, 5F, 5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup AttackSpeedOnCrit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/AttackSpeedOnCrit/DisplayWolfPelt.prefab").WaitForCompletion(),
                            childName = "BackFootL",
                            localPos = new Vector3(-1.088F, 0.02427F, -7.59747F),
                            localAngles = new Vector3(350.8276F, 305.2711F, 88.29471F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Thorns = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Thorns/DisplayRazorwireLeft.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(0.01788F, 0.08336F, 1.05358F),
                            localAngles = new Vector3(356.0009F, 356.0803F, 50.43696F),
                            localScale = new Vector3(2F, 3.25F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SprintOutOfCombat = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintOutOfCombat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintOutOfCombat/DisplayWhip.prefab").WaitForCompletion(),
                            childName = "FrontFootR",
                            localPos = new Vector3(-1.09766F, 0.85067F, -6.33187F),
                            localAngles = new Vector3(343.0655F, 40.58155F, 288.2435F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SprintArmor = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintArmor/DisplayBuckler.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.2443F, -0.00451F, 1.26691F),
                            localAngles = new Vector3(54.15662F, 61.20535F, 339.186F),
                            localScale = new Vector3(0.5F, 0.3F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Squid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Squid/DisplaySquidTurret.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(-0.01709F, 0.0282F, 0.08019F),
                            localAngles = new Vector3(310.3825F, 272.2824F, 83.23032F),
                            localScale = new Vector3(0.15F, 0.2F, 0.15F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ChainLightning = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ChainLightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ChainLightning/DisplayUkulele.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-6.62996F, 4.33915F, 5.16411F),
                            localAngles = new Vector3(301.9634F, 11.30304F, 60.13704F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup EnergizedOnEquipmentUse = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EnergizedOnEquipmentUse/DisplayWarHorn.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-1.07176F, -0.67862F, 7.04616F),
                            localAngles = new Vector3(356.6307F, 245.2702F, 353.8284F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup JumpBoost = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/JumpBoost/DisplayWaxBird.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0F, 3.43752F, 4.30265F),
                            localAngles = new Vector3(57.45488F, 359.4448F, 358.9927F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ExplodeOnDeath = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExplodeOnDeath,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ExplodeOnDeath/DisplayWilloWisp.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-1.69775F, -0.82758F, -1.37429F),
                            localAngles = new Vector3(45.10933F, 83.18239F, 167.1942F),
                            localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup NovaOnLowHealth = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NovaOnLowHealth/DisplayJellyGuts.prefab").WaitForCompletion(),
                            childName = "LeftBackMuzzle",
                            localPos = new Vector3(1.13894F, 0.61387F, -0.10655F),
                            localAngles = new Vector3(10.03484F, 7.05754F, 164.3728F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Pearl = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Pearl/DisplayPearl.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.00905F, 9.19839F, -0.84965F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ShinyPearl = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShinyPearl/DisplayShinyPearl.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.00905F, 9.19839F, -0.84965F),
                            localAngles = new Vector3(71.24702F, 27.14362F, 239.667F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup RoboBallBuddy = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RoboBallBuddy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBuddy/DisplayEmpathyChip.prefab").WaitForCompletion(),
                            childName = "BackFootL",
                            localPos = new Vector3(-1.65785F, -0.77744F, -2.03025F),
                            localAngles = new Vector3(348.5231F, 359.5535F, 324.189F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FireballsOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FireballsOnHit/DisplayFireballsOnHit.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.33745F, -0.0372F, -0.19412F),
                            localAngles = new Vector3(355.1987F, 193.7479F, 86.8071F),
                            localScale = new Vector3(0.05F, 0.05F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LightningStrikeOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LightningStrikeOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LightningStrikeOnHit/DisplayChargedPerforator.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.25322F, 0.2026F, -0.08799F),
                            localAngles = new Vector3(342.6754F, 107.7369F, 81.24968F),
                            localScale = new Vector3(1F, 1.5F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SprintWisp = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintWisp/DisplayBrokenMask.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(5.1106F, 5.11766F, 4.71506F),
                            localAngles = new Vector3(308.1541F, 20.57803F, 355.372F),
                            localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SiphonOnLowHealth = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SiphonOnLowHealth/DisplaySiphonOnLowHealth.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-2.77887F, 4.38664F, -8.42682F),
                            localAngles = new Vector3(2.80797F, 211.3976F, 2.49363F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ParentEgg = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ParentEgg,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ParentEgg/DisplayParentEgg.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(0.07128F, 1.05859F, 1.25894F),
                            localAngles = new Vector3(60.07309F, 356.7326F, 202.5737F),
                            localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BeetleGland = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BeetleGland,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGland/DisplayBeetleGland.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(4.57321F, -1.3494F, -4.28339F),
                            localAngles = new Vector3(345.6441F, 76.45128F, 301.8777F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BleedOnHitAndExplode = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHitAndExplode/DisplayBleedOnHitAndExplode.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-4.14268F, 5.93952F, 3.27509F),
                            localAngles = new Vector3(28.85856F, 40.40855F, 4.95947F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup TitanGoldDuringTP = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/TitanGoldDuringTP/DisplayGoldHeart.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.75769F, -2.00373F, 5.7329F),
                            localAngles = new Vector3(341.0353F, 359.9642F, 0.2202F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup AlienHead = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/AlienHead/DisplayAlienHead.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-4.87601F, 6.76852F, 0.40788F),
                            localAngles = new Vector3(356.3654F, 312.7357F, 185.2675F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup KillEliteFrenzy = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/KillEliteFrenzy/DisplayBrainstalk.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.00326F, 6.7387F, -0.32123F),
                            localAngles = new Vector3(4.85803F, -0.00255F, 359.9455F),
                            localScale = new Vector3(3F, 1.5F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Behemoth = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Behemoth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Behemoth/DisplayBehemoth.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(2.84672F, 7.88598F, 0.12136F),
                            localAngles = new Vector3(289.0591F, 65.24403F, 294.4274F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Dagger = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Dagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Dagger/DisplayDagger.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(0.82742F, 1.27587F, 3.35501F),
                            localAngles = new Vector3(338.0856F, 284.9409F, 288.2113F),
                            localScale = new Vector3(2F, 2F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup UtilitySkillMagazine = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UtilitySkillMagazine/DisplayAfterburner.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(1.25212F, 3.28857F, -9.2462F),
                            localAngles = new Vector3(353.9496F, 174.465F, 179.6193F),
                            localScale = new Vector3(3F, 3F, 4F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UtilitySkillMagazine/DisplayAfterburner.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-1.25028F, 3.28702F, -9.2535F),
                            localAngles = new Vector3(353.9578F, 184.6357F, 180.1031F),
                            localScale = new Vector3(3F, 3F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Plant = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Plant/DisplayInterstellarDeskPlant.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-3.69599F, 6.5565F, 1.99159F),
                            localAngles = new Vector3(284.8698F, 5.01132F, 350.0528F),
                            localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup NovaOnHeal = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NovaOnHeal/DisplayDevilHorns.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(1.00324F, 0.6992F, 6.29104F),
                            localAngles = new Vector3(346.631F, 323.6774F, 27.76366F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NovaOnHeal/DisplayDevilHorns.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-1.0786F, 0.69867F, 6.31457F),
                            localAngles = new Vector3(296.8799F, 359.8417F, 307.5616F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup IncreaseHealing = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IncreaseHealing/DisplayAntler.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(1.5579F, -0.23971F, 0.68537F),
                            localAngles = new Vector3(8.43673F, 67.83693F, 137.3287F),
                            localScale = new Vector3(-2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IncreaseHealing/DisplayAntler.prefab").WaitForCompletion(),
                            childName = "LeftBackMuzzle",
                            localPos = new Vector3(-1.91955F, -0.06921F, 0.76913F),
                            localAngles = new Vector3(9.70669F, 320.2435F, 197.4652F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BounceNearby = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BounceNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BounceNearby/DisplayHook.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.08036F, 3.49304F, 5.09325F),
                            localAngles = new Vector3(21.17544F, 15.48454F, 318.079F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ShockNearby = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShockNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ShockNearby/DisplayTeslaCoil.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(7.09195F, 6.77283F, 0.30342F),
                            localAngles = new Vector3(10.31115F, 358.5568F, 343.1931F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Talisman = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Talisman,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Talisman/DisplayTalisman.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-2.48036F, 7.56927F, 5.52497F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup HeadHunter = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/HeadHunter/DisplaySkullcrown.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.13935F, 5.9849F, 4.15848F),
                            localAngles = new Vector3(24.94407F, 359.9327F, 359.7014F),
                            localScale = new Vector3(4F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup Icicle = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Icicle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Icicle/DisplayFrostRelic.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-8.88106F, 6.06679F, -5.70849F),
                            localAngles = new Vector3(90F, 45F, 0F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LaserTurbine = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LaserTurbine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LaserTurbine/DisplayLaserTurbine.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(1.85052F, 7.38462F, -2.15105F),
                            localAngles = new Vector3(289.0595F, 98.27692F, 312.7396F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FallBoots = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FallBoots/DisplayGravBoots.prefab").WaitForCompletion(),
                            childName = "FrontFootR",
                            localPos = new Vector3(-1.09259F, 0.05787F, -3.18058F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FallBoots/DisplayGravBoots.prefab").WaitForCompletion(),
                            childName = "FrontFootL",
                            localPos = new Vector3(1.35799F, -0.02294F, -2.68149F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FallBoots/DisplayGravBoots.prefab").WaitForCompletion(),
                            childName = "BackFootL",
                            localPos = new Vector3(-1.18659F, 0.04617F, -2.9221F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FallBoots/DisplayGravBoots.prefab").WaitForCompletion(),
                            childName = "BackFootR",
                            localPos = new Vector3(1.1069F, 0.02719F, -2.7606F),
                            localAngles = new Vector3(90F, 0F, 0F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup GoldOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GoldOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/GoldOnHit/DisplayBoneCrown.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(2.48885F, 6.65554F, -2.12206F),
                            localAngles = new Vector3(9.01874F, 26.59634F, 339.7988F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FocusConvergence = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FocusConvergence/DisplayFocusedConvergence.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.00381F, 14.97597F, -0.40647F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarTrinket = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarTrinket/DisplayBeads.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-6.65966F, 3.96171F, -6.11263F),
                            localAngles = new Vector3(12.67176F, 351.1444F, 286.0385F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup RepeatHeal = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RepeatHeal/DisplayCorpseflower.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.05394F, 0.00584F, -0.01584F),
                            localAngles = new Vector3(309.8225F, 279.1164F, 88.73952F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup MonstersOnShrineUse = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MonstersOnShrineUse/DisplayMonstersOnShrineUse.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.29154F, 6.55914F, -7.66651F),
                            localAngles = new Vector3(345.997F, 340.2003F, 286.9088F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup AutoCastEquipment = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/AutoCastEquipment/DisplayFossil.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(6.79525F, 2.08264F, -5.43822F),
                            localAngles = new Vector3(297.8204F, 121.3466F, 287.9064F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarDagger = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarDagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarDagger/DisplayLunarDagger.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-4.57492F, 7.88839F, -2.92832F),
                            localAngles = new Vector3(0.52024F, 35.81143F, 88.33212F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup RandomDamageZone = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RandomDamageZone/DisplayRandomDamageZone.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.1092F, 3.77526F, -9.77396F),
                            localAngles = new Vector3(350.9671F, 359.9924F, 0.10191F),
                            localScale = new Vector3(1F, 1.25F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarPrimaryReplacement = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/DisplayBirdEye.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.01909F, 2.62215F, 6.01166F),
                            localAngles = new Vector3(89.15028F, 279.1934F, 100.9797F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarSecondaryReplacement = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarSecondaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/DisplayBirdClaw.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.37011F, 0.16083F, -0.08139F),
                            localAngles = new Vector3(298.9373F, 309.5919F, 335.5644F),
                            localScale = new Vector3(0.5F, 0.25F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarUtilityReplacement = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/DisplayBirdFoot.prefab").WaitForCompletion(),
                            childName = "BackFootL",
                            localPos = new Vector3(-1.83382F, -0.01354F, -3.30996F),
                            localAngles = new Vector3(281.1575F, 82.4161F, 324.7551F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarSpecialReplacement = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarSpecialReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/DisplayBirdHeart.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(9.42183F, -1.86788F, -6.2181F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            //White
            idrgroups.Add(Medkit);
            idrgroups.Add(BossDamageBonus);
            idrgroups.Add(SecondarySkillMagazine);
            idrgroups.Add(Firework);
            idrgroups.Add(SprintBonus);
            idrgroups.Add(NearbyDamageBonus);
            idrgroups.Add(IgniteOnKill);
            idrgroups.Add(CritGlasses);
            idrgroups.Add(StickyBomb);
            idrgroups.Add(WardOnLevel);
            idrgroups.Add(BleedOnHit);
            idrgroups.Add(ArmorPlate);
            idrgroups.Add(PersonalShield);
            idrgroups.Add(TreasureCache);
            idrgroups.Add(BarrierOnKill);
            idrgroups.Add(StunChanceOnHit);
            idrgroups.Add(Syringe);
            idrgroups.Add(Mushroom);
            idrgroups.Add(Hoof);
            idrgroups.Add(Bear);
            idrgroups.Add(Crowbar);
            idrgroups.Add(FlatHealth);
            idrgroups.Add(HealWhileSafe);
            //Green
            idrgroups.Add(Missile);
            idrgroups.Add(WarCryOnMultiKill);
            idrgroups.Add(DeathMark);
            idrgroups.Add(Bandolier);
            idrgroups.Add(SlowOnHit);
            idrgroups.Add(EquipmentMagazine);
            idrgroups.Add(BonusGoldPackOnKill);
            idrgroups.Add(HealOnCrit);
            idrgroups.Add(Feather);
            idrgroups.Add(Infusion);
            idrgroups.Add(FireRing);
            idrgroups.Add(IceRing);
            idrgroups.Add(TPHealingNova);
            idrgroups.Add(ExecuteLowHealthElite);
            idrgroups.Add(Phasing);
            idrgroups.Add(AttackSpeedOnCrit);
            idrgroups.Add(Thorns);
            idrgroups.Add(SprintOutOfCombat);
            idrgroups.Add(SprintArmor);
            idrgroups.Add(Squid);
            idrgroups.Add(ChainLightning);
            idrgroups.Add(EnergizedOnEquipmentUse);
            idrgroups.Add(JumpBoost);
            idrgroups.Add(ExplodeOnDeath);
            //Red
            idrgroups.Add(Clover);
            idrgroups.Add(ExtraLife);
            idrgroups.Add(GhostOnKill);
            idrgroups.Add(BarrierOnOverHeal);
            idrgroups.Add(ArmorReductionOnHit);
            idrgroups.Add(AlienHead);
            idrgroups.Add(KillEliteFrenzy);
            idrgroups.Add(Behemoth);
            idrgroups.Add(Dagger);
            idrgroups.Add(UtilitySkillMagazine);

            idrgroups.Add(IncreaseHealing);
            idrgroups.Add(Plant);
            idrgroups.Add(NovaOnHeal);
            idrgroups.Add(IncreaseHealing);
            idrgroups.Add(BounceNearby);
            idrgroups.Add(ShockNearby);
            idrgroups.Add(Talisman);
            idrgroups.Add(HeadHunter);
            idrgroups.Add(Icicle);
            idrgroups.Add(LaserTurbine);
            idrgroups.Add(FallBoots);
            //Yellow
            idrgroups.Add(Knurl);
            idrgroups.Add(NovaOnLowHealth);
            idrgroups.Add(RoboBallBuddy);
            idrgroups.Add(LightningStrikeOnHit);
            idrgroups.Add(FireballsOnHit);
            idrgroups.Add(SprintWisp);
            idrgroups.Add(SiphonOnLowHealth);
            idrgroups.Add(BeetleGland);
            idrgroups.Add(ParentEgg);
            idrgroups.Add(BleedOnHitAndExplode);
            idrgroups.Add(TitanGoldDuringTP);
            idrgroups.Add(Pearl);
            idrgroups.Add(ShinyPearl);
            //Lunar
            idrgroups.Add(ShieldOnly);
            idrgroups.Add(GoldOnHit);
            idrgroups.Add(FocusConvergence);
            idrgroups.Add(LunarTrinket);
            idrgroups.Add(RepeatHeal);
            idrgroups.Add(MonstersOnShrineUse);
            idrgroups.Add(AutoCastEquipment);
            idrgroups.Add(RandomDamageZone);
            idrgroups.Add(LunarDagger);
            idrgroups.Add(LunarPrimaryReplacement);
            idrgroups.Add(LunarSecondaryReplacement);
            idrgroups.Add(LunarUtilityReplacement);
            idrgroups.Add(LunarSpecialReplacement);
            //Missing
            //Tooth - Monster Teeth - Reason: Each invdividual tooth needs to be given a displayrule.
            //Seed - Leeching Seed - Reason: The display kinda sucks and it's also hardly noticeable.
            //CaptainDefenseMatrix - Microbots - Reason: Doesn't use displays.
            //LunarBadLuck - Purity - Reason: Doesn't use displays.
        }
        private static void DLC1Displays()
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup ImmuneToDebuff = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.ImmuneToDebuff,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ImmuneToDebuff/DisplayRainCoatBelt.prefab").WaitForCompletion(),
                            childName = "FrontFootR",
                            localPos = new Vector3(-1.082F, 0.06234F, -2.48708F),
                            localAngles = new Vector3(25.10994F, 93.40456F, 258.6685F),
                            localScale = new Vector3(3.25F, 3.25F, 3.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup PermanentDebuffOnHit = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.PermanentDebuffOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/PermanentDebuffOnHit/DisplayScorpion.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.10545F, 0.8736F, -8.48271F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(12F, 12F, 12F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup CritDamage = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.CritDamage,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/CritDamage/DisplayLaserSight.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.22753F, 0.48879F, 0.19737F),
                            localAngles = new Vector3(9.78078F, 265.3435F, 355.017F),
                            localScale = new Vector3(0.15F, 0.15F, 0.15F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FragileDamageBonus = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.FragileDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FragileDamageBonus/DisplayDelicateWatch.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(-0.1882F, 0.08807F, 1.19951F),
                            localAngles = new Vector3(352.6045F, 351.0513F, 182.4812F),
                            localScale = new Vector3(5F, 5F, 5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup AttackSpeedAndMoveSpeed = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.AttackSpeedAndMoveSpeed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/AttackSpeedAndMoveSpeed/DisplayCoffee.prefab").WaitForCompletion(),
                            childName = "MuzzleWhiteCannon",
                            localPos = new Vector3(0.08904F, 0.56699F, 0.14244F),
                            localAngles = new Vector3(355.7134F, 267.5872F, 354.7192F),
                            localScale = new Vector3(0.4F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup OutOfCombatArmor = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.OutOfCombatArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OutOfCombatArmor/DisplayOddlyShapedOpal.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0F, 1.36976F, 6.69882F),
                            localAngles = new Vector3(346.992F, 359.9838F, 0.14807F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup HealingPotion = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.HealingPotion,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/HealingPotion/DisplayHealingPotion.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-3.47866F, 6.92318F, -1.8682F),
                            localAngles = new Vector3(0.05702F, 0.97259F, 20.83092F),
                            localScale = new Vector3(0.25F, 0.25F, 0.25F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup GoldOnHurt = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.GoldOnHurt,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/GoldOnHurt/DisplayRollOfPennies.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-6.12709F, 3.89787F, -6.50067F),
                            localAngles = new Vector3(359.914F, 0.55754F, 346.468F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup PrimarySkillShuriken = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.PrimarySkillShuriken,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/PrimarySkillShuriken/DisplayShuriken.prefab").WaitForCompletion(),
                            childName = "BackFootR",
                            localPos = new Vector3(0.95399F, 0.01286F, -0.88536F),
                            localAngles = new Vector3(359.6298F, 352.9912F, 0.67301F),
                            localScale = new Vector3(5F, 5F, 5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup MoveSpeedOnKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.MoveSpeedOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MoveSpeedOnKill/DisplayGrappleHook.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(1.4301F, 0.84394F, 7.21541F),
                            localAngles = new Vector3(54.89399F, 63.08176F, 35.5683F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup StrengthenBurn = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.StrengthenBurn,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/StrengthenBurn/DisplayGasTank.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-0.836F, 0.52258F, 0.21503F),
                            localAngles = new Vector3(321.0205F, 68.99921F, 233.195F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup RegeneratingScrap = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.RegeneratingScrap,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/RegeneratingScrap/DisplayRegeneratingScrap.prefab").WaitForCompletion(),
                            childName = "LeftBackMuzzle",
                            localPos = new Vector3(0.84764F, 0.08778F, 0.92328F),
                            localAngles = new Vector3(357.9803F, 220.7642F, 201.0231F),
                            localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup FreeChest = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.FreeChest,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FreeChest/DisplayShippingRequestForm.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(7.54096F, 5.0463F, -5.26531F),
                            localAngles = new Vector3(25.86842F, 155.6347F, 16.22653F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup MoreMissile = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.MoreMissile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MoreMissile/DisplayICBM.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.00003F, 7.88516F, -2.6147F),
                            localAngles = new Vector3(30.87198F, 273.2052F, 280.8075F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup RandomEquipmentTrigger = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.RandomEquipmentTrigger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/RandomEquipmentTrigger/DisplayBottledChaos.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(6.83895F, 6.05103F, -1.83957F),
                            localAngles = new Vector3(54.81056F, 241.3428F, 226.4906F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup MinorConstructOnKill = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.MinorConstructOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MinorConstructOnKill/DisplayDefenseNucleus.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-4.02973F, 9.09059F, -10.89421F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup LunarSun = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.LunarSun,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/LunarSun/DisplaySunHead.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-0.06711F, 2.56548F, 6.36523F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup RandomlyLunar = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.RandomlyLunar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/RandomlyLunar/DisplayDomino.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(3.65926F, 10.80018F, -0.45191F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup HalfAttackSpeedHalfCooldowns = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.HalfAttackSpeedHalfCooldowns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/HalfAttackSpeedHalfCooldowns/DisplayLunarShoulderNature.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-10.81325F, 4.27929F, -0.62694F),
                            localAngles = new Vector3(0.45529F, 358.1193F, 7.1188F),
                            localScale = new Vector3(8F, 8F, 8F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup HalfSpeedDoubleHealth = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.HalfSpeedDoubleHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/HalfSpeedDoubleHealth/DisplayLunarShoulderStone.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(9.7068F, 5.20249F, 1.4216F),
                            localAngles = new Vector3(355.0033F, 162.5116F, 4.65835F),
                            localScale = new Vector3(7F, 7F, 7F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup VoidMegaCrabItem = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.VoidMegaCrabItem,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DisplayMegaCrabItem.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(4.36417F, -0.63814F, -4.62152F),
                            localAngles = new Vector3(40.05453F, 164.4657F, 359.2657F),
                            localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BearVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.BearVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/BearVoid/DisplayBearVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-6.34222F, 5.58901F, 3.30526F),
                            localAngles = new Vector3(316.3687F, 0.70888F, 3.04792F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup BleedOnHitVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.BleedOnHitVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/BleedOnHitVoid/DisplayTriTipVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-4.51696F, 1.66186F, -7.91377F),
                            localAngles = new Vector3(334.9604F, 13.83776F, 249.1614F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ChainLightningVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.ChainLightningVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ChainLightningVoid/DisplayUkuleleVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-6.62996F, 4.33915F, 5.16411F),
                            localAngles = new Vector3(301.9634F, 11.30304F, 60.13704F),
                            localScale = new Vector3(3F, 3F, 3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup CloverVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.CloverVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/CloverVoid/DisplayCloverVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(2.17497F, 4.69877F, 6.01925F),
                            localAngles = new Vector3(16.34685F, 26.73651F, 354.8405F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup CritGlassesVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.CritGlassesVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/CritGlassesVoid/DisplayGlassesVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(0.00253F, 2.14994F, 6.97261F),
                            localAngles = new Vector3(0.05925F, 359.9326F, 5.02378F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ElementalRingVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.ElementalRingVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ElementalRingVoid/DisplayVoidRing.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.04108F, -0.00672F, 0.18397F),
                            localAngles = new Vector3(358.6336F, 358.5022F, 359.3903F),
                            localScale = new Vector3(1.75F, 1.75F, 1.75F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup EquipmentMagazineVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.EquipmentMagazineVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EquipmentMagazineVoid/DisplayFuelCellVoid.prefab").WaitForCompletion(),
                            childName = "MuzzleBlackCannon",
                            localPos = new Vector3(-0.43402F, 0.51268F, 2.0127F),
                            localAngles = new Vector3(353.229F, 12.44995F, 57.40187F),
                            localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ExplodeOnDeathVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.ExplodeOnDeathVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ExplodeOnDeathVoid/DisplayWillowWispVoid.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-1.69775F, -0.82758F, -1.37429F),
                            localAngles = new Vector3(45.10933F, 83.18239F, 167.1942F),
                            localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup ExtraLifeVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.ExtraLifeVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ExtraLifeVoid/DisplayHippoVoid.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(1.14041F, -0.23914F, 1.21434F),
                            localAngles = new Vector3(16.25856F, 14.39472F, 165.9836F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup MissileVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.MissileVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MissileVoid/DisplayMissileLauncherVoid.prefab").WaitForCompletion(),
                            childName = "RightBackMuzzle",
                            localPos = new Vector3(-2.22808F, -1.14637F, -0.0298F),
                            localAngles = new Vector3(56.88838F, 140.8108F, 186.1091F),
                            localScale = new Vector3(0.4F, 0.4F, 0.4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup MushroomVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.MushroomVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MushroomVoid/DisplayMushroomVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(-7.24997F, 6.34788F, 0.83195F),
                            localAngles = new Vector3(331.8201F, 89.92665F, 11.17588F),
                            localScale = new Vector3(1.5F, 1.5F, 1.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup SlowOnHitVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.SlowOnHitVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/SlowOnHitVoid/DisplayBaubleVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(2.07741F, -1.85865F, 7.74365F),
                            localAngles = new Vector3(0.372F, 275.0918F, 347.6111F),
                            localScale = new Vector3(2F, 2F, 2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            ItemDisplayRuleSet.KeyAssetRuleGroup TreasureCacheVoid = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.TreasureCacheVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/TreasureCacheVoid/DisplayKeyVoid.prefab").WaitForCompletion(),
                            childName = "BodyBase",
                            localPos = new Vector3(4.98828F, 3.09758F, -7.59614F),
                            localAngles = new Vector3(298.405F, 44.15102F, 357.8575F),
                            localScale = new Vector3(4F, 4F, 4F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            };
            //White
            idrgroups.Add(FragileDamageBonus);
            idrgroups.Add(AttackSpeedAndMoveSpeed);
            idrgroups.Add(OutOfCombatArmor);
            idrgroups.Add(HealingPotion);
            idrgroups.Add(GoldOnHurt);
            //Green
            idrgroups.Add(PrimarySkillShuriken);
            idrgroups.Add(MoveSpeedOnKill);
            idrgroups.Add(StrengthenBurn);
            idrgroups.Add(RegeneratingScrap);
            idrgroups.Add(FreeChest);
            //Red
            idrgroups.Add(ImmuneToDebuff);
            idrgroups.Add(PermanentDebuffOnHit);
            idrgroups.Add(CritDamage);
            idrgroups.Add(MoreMissile);
            idrgroups.Add(RandomEquipmentTrigger);
            //Yellow
            idrgroups.Add(MinorConstructOnKill);
            //Lunar
            idrgroups.Add(RandomlyLunar);
            idrgroups.Add(LunarSun);
            idrgroups.Add(HalfAttackSpeedHalfCooldowns);
            idrgroups.Add(HalfSpeedDoubleHealth);
            //Void-White
            idrgroups.Add(BearVoid);
            idrgroups.Add(BleedOnHitVoid);
            idrgroups.Add(CritGlassesVoid);
            idrgroups.Add(MushroomVoid);
            idrgroups.Add(TreasureCacheVoid);
            //Void-Green
            idrgroups.Add(ElementalRingVoid);
            idrgroups.Add(ChainLightningVoid);
            idrgroups.Add(EquipmentMagazineVoid);
            idrgroups.Add(ExplodeOnDeathVoid);
            //Void-Red
            idrgroups.Add(MissileVoid);
            idrgroups.Add(SlowOnHitVoid);
            idrgroups.Add(ExtraLifeVoid);
            idrgroups.Add(CloverVoid);
            //Void-Yellow
            idrgroups.Add(VoidMegaCrabItem);
            //Missing
            //DroneWeapons - Spare Drone Parts - Reason: Doesn't use displays.
            //DroneDisplay1 - ??? - Reason: Doesn't even use them.
            //DroneDisplay2 - ??? - Reason: Doesn't even use them.
        }
    }
}
