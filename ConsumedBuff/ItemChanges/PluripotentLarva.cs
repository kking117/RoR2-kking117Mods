using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Items;
using R2API;
using R2API.Utils;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace ConsumedBuff.ItemChanges
{
    public class PluripotentLarva
    {
        public static void Enable()
        {
            UpdateText();
            RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
            if (MainPlugin.VoidDio_Corrupt.Value)
            {
                On.RoR2.CharacterMaster.OnInventoryChanged += OnInventoryChanged;
            }
            if (MainPlugin.VoidDio_BearWorth.Value != 0)
            {
                IL.RoR2.HealthComponent.TakeDamage += new ILContext.Manipulator(IL_TakeDamage);
            }
            if (MainPlugin.VoidDio_BleedWorth.Value != 0)
            {
                IL.RoR2.GlobalEventManager.OnHitEnemy += new ILContext.Manipulator(IL_OnHitEnemy);
            }
        }
        public static void UpdateText()
        {
            string pickup = string.Format("");
            string desc = string.Format("");
            if (MainPlugin.VoidDio_Corrupt.Value)
            {
                pickup = string.Format("Corrupts your life. ");
                desc = string.Format("All of your items that can be <style=cIsUtility>corrupted</style> will be. ");
            }
            if(MainPlugin.VoidDio_Curse.Value > 0f)
            {
                pickup += string.Format("Reduced maximum health. ");
                desc += string.Format("<style=cIsHealth>Reduces maximum health by {0}%</style> <style=cStack>(+{0}% per stack)</style>. ", MainPlugin.VoidDio_Curse.Value * 100f);
            }
            if (MainPlugin.VoidDio_BearWorth.Value != 0 || MainPlugin.VoidDio_BleedWorth.Value != 0)
            {
                pickup += string.Format("Gain the powers of the Void.");
                desc += string.Format("Counts as ");
                if (MainPlugin.VoidDio_BearWorth.Value != 0)
                {
                    desc += string.Format("<style=cIsVoid>{0}<style=cStack> (+{0} per stack)</style> Safer Spaces</style>", MainPlugin.VoidDio_BearWorth.Value);
                    if (MainPlugin.VoidDio_BleedWorth.Value != 0)
                    {
                        desc += string.Format(" and ");
                    }
                }
                if (MainPlugin.VoidDio_BleedWorth.Value != 0)
                {
                    desc += string.Format("<style=cIsVoid>{0}<style=cStack> (+{0} per stack)</style> Needleticks</style>", MainPlugin.VoidDio_BleedWorth.Value);
                }
                desc += string.Format(".");
            }

            LanguageAPI.Add("ITEM_EXTRALIFEVOIDCONSUMED_PICKUP", pickup);
            LanguageAPI.Add("ITEM_EXTRALIFEVOIDCONSUMED_DESC", desc);
        }
        private static void CalculateStatsHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                int itemCount = sender.inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoidConsumed);
                if (MainPlugin.VoidDio_Corrupt.Value)
                {
                    ManageCorruption(sender, itemCount);
                }
                if (itemCount > 0)
                {
                    if (MainPlugin.VoidDio_Curse.Value > 0.0f)
                    {
                        args.baseCurseAdd += MainPlugin.VoidDio_Curse.Value * itemCount;
                    }
                    if (MainPlugin.VoidDio_BearWorth.Value > 0)
                    {
                        if (!sender.HasBuff(DLC1Content.Buffs.BearVoidCooldown) && !sender.HasBuff(DLC1Content.Buffs.BearVoidReady))
                        {
                            sender.AddBuff(DLC1Content.Buffs.BearVoidReady);
                        }
                    }
                }
            }
        }
        private static void ManageCorruption(CharacterBody self, int itemCount)
        {
            if (itemCount > 0)
            {
                if (!self.bodyFlags.HasFlag(CharacterBody.BodyFlags.Void))
                {
                    self.bodyFlags |= CharacterBody.BodyFlags.Void;
                    Components.ExtraLifeVoidEffect comp = self.GetComponent<Components.ExtraLifeVoidEffect>();
                    if (!comp)
                    {
                        self.gameObject.AddComponent<Components.ExtraLifeVoidEffect>();
                    }
                }
            }
            else
            {
                Components.ExtraLifeVoidEffect comp = self.GetComponent<Components.ExtraLifeVoidEffect>();
                if (comp)
                {
                    UnityEngine.Object.DestroyImmediate(comp);
                    self.bodyFlags &= ~CharacterBody.BodyFlags.Void;
                }
            }
        }
        private static void OnInventoryChanged(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            orig(self);
            int itemCount = self.inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoidConsumed);
            if (itemCount > 0)
            {
                CorruptAllItems(self.inventory);
            }
        }
        private static void IL_OnHitEnemy(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchStloc(x, 24)
            );
            ilcursor.Emit(OpCodes.Ldarg_1);
            ilcursor.EmitDelegate<Func<DamageInfo, int>>((damageInfo) =>
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoidConsumed) * MainPlugin.VoidDio_BleedWorth.Value;
                return itemCount;
            });
            ilcursor.Emit(OpCodes.Add);
        }
        private static void IL_TakeDamage(ILContext il)
        {
            ILCursor ilcursor = new ILCursor(il);
            ilcursor.GotoNext(
                x => ILPatternMatchingExt.MatchStloc(x, 14)
            );
            ilcursor.Emit(OpCodes.Ldarg_0);
            ilcursor.EmitDelegate<Func<HealthComponent, int>>((hpcomp) =>
            {
                CharacterBody body = hpcomp.body;
                if (body.inventory)
                {
                    return body.inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoidConsumed) * MainPlugin.VoidDio_BearWorth.Value;
                }
                return 0;
            });
            ilcursor.Emit(OpCodes.Add);
        }
        private static void CorruptAllItems(Inventory inventory)
        {
            foreach (ContagiousItemManager.TransformationInfo transformationInfo in ContagiousItemManager.transformationInfos)
            {
                ContagiousItemManager.TryForceReplacement(inventory, transformationInfo.transformedItem);
            }
        }
    }
}
