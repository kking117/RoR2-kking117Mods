using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace ConsumedBuff.ItemChanges
{
    public class DelicateWatchBroken
    {
        private static bool canDouble = true;
        public static BuffDef TrackerBuff;
        public static BuffDef TrackerDoubleBuff;
        private static Color buffColor = new Color(0.678f, 0.611f, 0.411f, 1f);
        private static Color buff2Color = new Color(0.96f, 0.623f, 0.282f, 1f);
        public static void Enable()
        {
            MainPlugin.Watch_HitsToProc = Math.Max(1, MainPlugin.Watch_HitsToProc);
            if (MainPlugin.Watch_ProcsToDouble < 2)
            {
                canDouble = false;
            }
            On.RoR2.HealthComponent.TakeDamageProcess += OnTakeDamage;
            if (MainPlugin.Watch_Indicator)
            {
                CreateBuff();
            }
            UpdateText();
        }
        private static void CreateBuff()
        {
            Sprite buffIcon = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/LaserTurbine/bdLaserTurbineKillCharge.asset").WaitForCompletion().iconSprite;
            TrackerBuff = Modules.Buffs.AddNewBuff("BrokenWatchCounter", buffIcon, buffColor, true, false, true);
            if (canDouble)
            {
                TrackerDoubleBuff = Modules.Buffs.AddNewBuff("BrokenWatchDoubleCounter", buffIcon, buff2Color, true, false, true, true);
            }
        }
        private static string GetNthString(int number)
        {
            String result = "th";
            if(number < 0)
            {
                number *= -1;
            }
            int number2 = number % 100;
            if (number2 < 11 || number2 > 19)
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
        private static void UpdateText()
        {
            string pickup = string.Format("");
            string desc = string.Format("");

            bool addAnd = false;

            pickup = string.Format("Every {0}{1} hit", MainPlugin.Watch_HitsToProc, GetNthString(MainPlugin.Watch_HitsToProc));
            desc = string.Format("Every <style=cIsDamage>{0}{1} hit</style>", MainPlugin.Watch_HitsToProc, GetNthString(MainPlugin.Watch_HitsToProc));
            if (MainPlugin.Watch_Damage > 0f)
            {
                pickup += string.Format(" deals bonus damage");
                desc += string.Format(" deals <style=cIsDamage>{0}%</style> <style=cStack>(+{0}% per stack)</style> extra damage", MainPlugin.Watch_Damage * 100f);
                addAnd = true;
            }

            if(MainPlugin.Watch_SlowBase > 0f || MainPlugin.Watch_SlowStack > 0f)
            {
                if(addAnd)
                {
                    pickup += string.Format(" and");
                    desc += string.Format(" and");
                }
                pickup += string.Format(" slows");
                desc += string.Format(" applies <style=cIsUtility>slow</style> for <style=cIsUtility>-60% movement speed</style> for <style=cIsUtility>{0}s</style> <style=cStack>(+{1}s per stack)</style>", MainPlugin.Watch_SlowBase, MainPlugin.Watch_SlowStack);
            }
            pickup += string.Format(".");
            desc += string.Format(".");
            LanguageAPI.Add("ITEM_FRAGILEDAMAGEBONUSCONSUMED_PICKUP", pickup);
            LanguageAPI.Add("ITEM_FRAGILEDAMAGEBONUSCONSUMED_DESC", desc);
            LanguageAPI.Add("ITEM_FRAGILEDAMAGEBONUSCONSUMED_PICKUP", pickup, "en");
            LanguageAPI.Add("ITEM_FRAGILEDAMAGEBONUSCONSUMED_DESC", desc, "en");
        }

        private static void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                if (self.alive && !self.godMode && !damageInfo.rejected && self.ospTimer <= 0f)
                {
                    if (self.body && damageInfo.attacker)
                    {
                        CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                        if (attackerBody)
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
                                        damageInfo.damage *= 1f + (itemCount * MainPlugin.Watch_Damage * powerMult);
                                        //The funny effect
                                        EffectData watchEffect = new EffectData
                                        {
                                            origin = self.body.transform.position
                                        };
                                        watchEffect.SetNetworkedObjectReference(self.body.gameObject);
                                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.fragileDamageBonusBreakEffectPrefab, watchEffect, true);
                                        //The buff

                                        float buffdur = MainPlugin.Watch_SlowBase + (Math.Max(0, itemCount - 1) * MainPlugin.Watch_SlowStack) * powerMult;
                                        if (buffdur > 0f)
                                        {
                                            self.body.AddTimedBuff(RoR2Content.Buffs.Slow60, buffdur);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }
        private static void AddWatchCounter(CharacterBody body)
        {
            Components.BrokenWatchCounter comp = body.GetComponent<Components.BrokenWatchCounter>();
            if (!comp)
            {
                comp = body.gameObject.AddComponent<Components.BrokenWatchCounter>();
            }
            comp.hits += 1;
            if (comp.hits >= MainPlugin.Watch_HitsToProc)
            {
                comp.hits -= MainPlugin.Watch_HitsToProc;
                comp.hitmult = 1;
                if (canDouble)
                {
                    comp.procs += 1;
                    if (comp.procs % MainPlugin.Watch_ProcsToDouble == 0)
                    {
                        comp.procs -= MainPlugin.Watch_ProcsToDouble;
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
                if (hits % MainPlugin.Watch_HitsToProc == 0)
                {
                    if (canDouble)
                    {
                        int procs = comp.procs + 1;
                        if (procs % MainPlugin.Watch_ProcsToDouble == 0)
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
            if (MainPlugin.Watch_Indicator)
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

                if (canDouble && procCount % MainPlugin.Watch_ProcsToDouble == 0)
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