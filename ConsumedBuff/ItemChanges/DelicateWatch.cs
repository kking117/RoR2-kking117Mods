using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace ConsumedBuff.ItemChanges
{
    public class DelicateWatch
    {
        private static int SuperHitMult = 2;
        private static int SuperHitNo = 12;
        private static int HitNo = 12;
        public static void Enable()
        {
            HitNo = Math.Max(1, MainPlugin.Watch_Hits.Value);
            if(MainPlugin.Watch_SuperHits.Value < HitNo)
            {
                SuperHitMult = 1;
                SuperHitNo = HitNo;
            }
            else
            {
                SuperHitNo = MainPlugin.Watch_SuperHits.Value;
            }
            UpdateText();
            On.RoR2.GlobalEventManager.ServerDamageDealt += OnTakeDamagePost;
            if (MainPlugin.Watch_Damage.Value)
            {
                IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
            }
        }
        public static void UpdateText()
        {
            string pickup = string.Format("");
            string desc = string.Format("");

            bool addAnd = false;

            pickup = string.Format("Every {0} hits", MainPlugin.Watch_Hits.Value);
            desc = string.Format("Every <style=cIsDamage>{0}</style> <style=cIsUtility>hits</style>", MainPlugin.Watch_Hits.Value);
            if (MainPlugin.Watch_Damage.Value)
            {
                pickup += string.Format(" deal bonus damage");
                desc += string.Format(" deal <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> extra damage");
                addAnd = true;
            }

            if(MainPlugin.Watch_SlowBase.Value > 0f || MainPlugin.Watch_SlowStack.Value > 0f)
            {
                if(addAnd)
                {
                    pickup += string.Format(" and");
                    desc += string.Format(" and");
                }
                pickup += string.Format(" apply slow");
                desc += string.Format(" apply <style=cIsUtility>slow</style> for <style=cIsUtility>-60% movement speed</style> for <style=cIsUtility>{0}s</style> <style=cStack>(+{1}s per stack)</style>", MainPlugin.Watch_SlowBase.Value + MainPlugin.Watch_SlowStack.Value, MainPlugin.Watch_SlowStack.Value);
            }
            pickup += string.Format(".");
            desc += string.Format(".");

            LanguageAPI.Add("ITEM_FRAGILEDAMAGEBONUSCONSUMED_PICKUP", pickup);
            LanguageAPI.Add("ITEM_FRAGILEDAMAGEBONUSCONSUMED_DESC", desc);
        }
        private static void IL_TakeDamage(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchStloc(x, 27)
            );
            ilcursor.Emit(OpCodes.Ldarg_1);
            ilcursor.EmitDelegate<Func<DamageInfo, int>>((damageInfo) =>
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonusConsumed) * PredictWatchBoostMult(attackerBody);
                return itemCount;
            });
            ilcursor.Emit(OpCodes.Add);
        }
        private static void OnTakeDamagePost(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport dr)
        {
            orig(dr);
            if (NetworkServer.active)
            {
                if (!dr.damageInfo.rejected)
                {
                    CharacterBody attackerBody = dr.attackerBody;
                    CharacterBody victimBody = dr.victimBody;
                    if (attackerBody && victimBody)
                    {
                        if (attackerBody.inventory)
                        {
                            int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonusConsumed);
                            if (itemCount > 0)
                            {
                                AddWatchCounter(attackerBody);
                                int powerMult = GetWatchPowerMult(attackerBody);
                                if (powerMult > 0)
                                {
                                    EffectData watchEffect = new EffectData
                                    {
                                        origin = victimBody.transform.position
                                    };
                                    watchEffect.SetNetworkedObjectReference(victimBody.gameObject);
                                    EffectManager.SpawnEffect(HealthComponent.AssetReferences.fragileDamageBonusBreakEffectPrefab, watchEffect, true);
                                    victimBody.AddTimedBuff(RoR2Content.Buffs.Slow60, MainPlugin.Watch_SlowBase.Value + (itemCount * MainPlugin.Watch_SlowStack.Value) * powerMult);
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void AddWatchCounter(CharacterBody body)
        {
            Components.BrokenWatchCounter comp = body.GetComponent<Components.BrokenWatchCounter>();
            if (!comp)
            {
                comp = body.gameObject.AddComponent<Components.BrokenWatchCounter>();
            }
            if (comp.LastHitFrame != Time.fixedTime)
            {
                comp.hits += 1;
                comp.LastHitFrame = Time.fixedTime;
            }
            if (comp.hits >= SuperHitNo)
            {
                comp.hits -= SuperHitNo;
                comp.hitmult = SuperHitMult;
                return;
            }
            if (comp.hits % HitNo == 0)
            {
                comp.hitmult = 1;
                return;
            }
            comp.hitmult = 0;
        }
        private static int PredictWatchBoostMult(CharacterBody body)
        {
            Components.BrokenWatchCounter comp = body.GetComponent<Components.BrokenWatchCounter>();
            if (comp)
            {
                int hits = comp.hits;
                if (comp.LastHitFrame != Time.fixedTime)
                {
                    hits += 1;
                }
                if (hits % SuperHitNo == 0)
                {
                    return SuperHitMult;
                }
                if (hits % HitNo == 0)
                {
                    return 1;
                }  
            }
            return 0;
        }
        private static int GetWatchPowerMult(CharacterBody body)
        {
            Components.BrokenWatchCounter comp = body.GetComponent<Components.BrokenWatchCounter>();
            if (comp)
            {
                return comp.hitmult;
            }
            return 0;
        }
    }
}