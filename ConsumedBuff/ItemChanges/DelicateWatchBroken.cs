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
using UnityEngine.AddressableAssets;

namespace ConsumedBuff.ItemChanges
{
    public class DelicateWatchBroken
    {
        private static float dmgMult = 1f;
        private static bool canDouble = true;
        public static BuffDef TrackerBuff;
        public static BuffDef TrackerDoubleBuff;
        private static Color buffColor = new Color(0.678f, 0.611f, 0.411f, 1f);
        private static Color buff2Color = new Color(0.96f, 0.623f, 0.282f, 1f);
        public static void Enable()
        {
            MainPlugin.Watch_HitsToProc.Value = Math.Max(1, MainPlugin.Watch_HitsToProc.Value);
            if (MainPlugin.Watch_ProcsToDouble.Value < 2)
            {
                canDouble = false;
            }
            if (MainPlugin.Watch_Damage.Value > 0.0f)
            {
                dmgMult = MainPlugin.Watch_Damage.Value * 5f;
                IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
            }
            UpdateText();
            On.RoR2.GlobalEventManager.ServerDamageDealt += OnTakeDamagePost;
            if (MainPlugin.Watch_Indicator.Value)
            {
                CreateBuff();
            }
        }
        private static void CreateBuff()
        {
            Sprite buffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/LaserTurbine/bdLaserTurbineKillCharge.asset").WaitForCompletion().iconSprite;
            TrackerBuff = Modules.Buffs.AddNewBuff("BrokenWatchCounter", buffIcon, buffColor, true, false, true);
            if (canDouble)
            {
                TrackerDoubleBuff = Modules.Buffs.AddNewBuff("BrokenWatchDoubleCounter", buffIcon, buff2Color, true, false, true);
            }
        }
        private static string GetNthString(int number)
        {
            String result = "th";
            if(number < 0)
            {
                number *= -1;
            }
            if(number < 11 || number > 19)
            {
                number = number % 10;
                switch(number)
                {
                    case 1:
                        result = "st";
                        break;
                    case 2:
                        result = "nd";
                        break;
                    case 3:
                        result = "rd";
                        break;
                }
            }
            return result;
        }
        public static void UpdateText()
        {
            string pickup = string.Format("");
            string desc = string.Format("");

            bool addAnd = false;

            pickup = string.Format("Every {0}{1} hit", MainPlugin.Watch_HitsToProc.Value, GetNthString(MainPlugin.Watch_HitsToProc.Value));
            desc = string.Format("Every <style=cIsDamage>{0}{1} hit</style>", MainPlugin.Watch_HitsToProc.Value, GetNthString(MainPlugin.Watch_HitsToProc.Value));
            if (dmgMult > 0f)
            {
                pickup += string.Format(" deals bonus damage");
                desc += string.Format(" deals <style=cIsDamage>{0}%</style> <style=cStack>(+{0}% per stack)</style> extra damage", dmgMult * 20f);
                addAnd = true;
            }

            if(MainPlugin.Watch_SlowBase.Value > 0f || MainPlugin.Watch_SlowStack.Value > 0f)
            {
                if(addAnd)
                {
                    pickup += string.Format(" and");
                    desc += string.Format(" and");
                }
                pickup += string.Format(" applies slow");
                desc += string.Format(" applies <style=cIsUtility>slow</style> for <style=cIsUtility>-60% movement speed</style> for <style=cIsUtility>{0}s</style> <style=cStack>(+{1}s per stack)</style>", MainPlugin.Watch_SlowBase.Value, MainPlugin.Watch_SlowStack.Value);
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
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchLdloc(x, 27)
            );
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchLdloc(x, 27)
            );
            ilcursor.Remove();
            ilcursor.Emit(OpCodes.Ldarg_1);
            ilcursor.EmitDelegate<Func<DamageInfo, float>>((damageInfo) =>
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                float itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus) * 1.0f;
                itemCount += (attackerBody.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonusConsumed) * dmgMult) * PredictWatchBoostMult(attackerBody);
                return itemCount;
            });
        }
        private static void OnTakeDamagePost(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport dr)
        {
            orig(dr);
            if (NetworkServer.active)
            {
                if (!dr.damageInfo.rejected)
                {
                    if (dr.damageInfo.damage > 0f)
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
                                        //The funny effect
                                        EffectData watchEffect = new EffectData
                                        {
                                            origin = victimBody.transform.position
                                        };
                                        watchEffect.SetNetworkedObjectReference(victimBody.gameObject);
                                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.fragileDamageBonusBreakEffectPrefab, watchEffect, true);
                                        //The buff
                                        float buffdur = MainPlugin.Watch_SlowBase.Value + ((itemCount - 1) * MainPlugin.Watch_SlowStack.Value) * powerMult;
                                        if (buffdur > 0f)
                                        {
                                            victimBody.AddTimedBuff(RoR2Content.Buffs.Slow60, buffdur);
                                        }
                                    }
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
            comp.hits += 1;
            if (comp.hits >= MainPlugin.Watch_HitsToProc.Value)
            {
                comp.hits -= MainPlugin.Watch_HitsToProc.Value;
                comp.hitmult = 1;
                if (canDouble)
                {
                    comp.procs += 1;
                    if (comp.procs % MainPlugin.Watch_ProcsToDouble.Value == 0)
                    {
                        comp.procs -= MainPlugin.Watch_ProcsToDouble.Value;
                        comp.hitmult = 2;
                    }
                }
            }
            else
            {
                comp.hitmult = 0;
            }
            UpdateTrackerBuff(body);
        }
        private static int PredictWatchBoostMult(CharacterBody body)
        {
            Components.BrokenWatchCounter comp = body.GetComponent<Components.BrokenWatchCounter>();
            if (comp)
            {
                int hits = comp.hits + 1;
                if (hits % MainPlugin.Watch_HitsToProc.Value == 0)
                {
                    if (canDouble)
                    {
                        int procs = comp.procs + 1;
                        if (procs % MainPlugin.Watch_ProcsToDouble.Value == 0)
                        {
                            return 2;
                        }
                    }
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
        private static void UpdateTrackerBuff(CharacterBody body)
        {
            if (MainPlugin.Watch_Indicator.Value)
            {
                int buffCount = 0;
                int procCount = 0;
                Components.BrokenWatchCounter comp = body.GetComponent<Components.BrokenWatchCounter>();
                if (comp)
                {
                    buffCount = comp.hits;
                    procCount = comp.procs;
                }

                if (canDouble)
                {
                    for (; body.GetBuffCount(TrackerDoubleBuff.buffIndex) > 0;)
                    {
                        body.RemoveBuff(TrackerDoubleBuff.buffIndex);
                    }
                }
                for (; body.GetBuffCount(TrackerBuff.buffIndex) > 0;)
                {
                    body.RemoveBuff(TrackerBuff.buffIndex);
                }
                procCount++;

                if (canDouble && procCount % MainPlugin.Watch_ProcsToDouble.Value == 0)
                {
                    for (; body.GetBuffCount(TrackerDoubleBuff.buffIndex) < buffCount;)
                    {
                        body.AddBuff(TrackerDoubleBuff.buffIndex);
                    }
                }
                else
                {
                    for (; body.GetBuffCount(TrackerBuff.buffIndex) < buffCount;)
                    {
                        body.AddBuff(TrackerBuff.buffIndex);
                    }
                }
            }
        }
    }
}