using System;
using RoR2;
using RoR2.Projectile;
using RoR2.Items;
using R2API;
using UnityEngine;
using UnityEngine.Networking;

namespace ConsumedBuff.ItemChanges
{
    public class PluripotentLarvaConsumed
    {
        private static float StackDamage;
        public static void Enable()
        {
            UpdateText();
            RecalculateStatsAPI.GetStatCoefficients += CalculateStatsHook;
            if (MainPlugin.VoidDio_Corrupt.Value)
            {
                On.RoR2.CharacterMaster.OnInventoryChanged += OnInventoryChanged;
            }
            if (MainPlugin.VoidDio_BlockCooldown.Value <= 1.0f)
            {
                //Figure out a better way of doing this?
                On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += AddTimedBuff;
            }
            if (MainPlugin.VoidDio_CollapseChance.Value > 0.0f)
            {
                On.RoR2.GlobalEventManager.ServerDamageDealt += OnTakeDamagePost;
            }
        }
        private static void UpdateText()
        {
            string pickup = "";
            string desc = "";

            if (MainPlugin.VoidDio_CollapseChance.Value > 0.0f)
            {
                pickup += string.Format("Chance to collapse enemies on hit.");
                desc += string.Format("<style=cIsDamage>{0}%</style> <style=cStack>(+{0}% per stack)</style> chance to <style=cIsDamage>collapse</style> an enemy for <style=cIsDamage>{1}%</style> ", MainPlugin.VoidDio_CollapseChance.Value, MainPlugin.VoidDio_CollapseDamage.Value * 100);
                if (MainPlugin.VoidDio_CollapseUseTotal.Value)
                {
                    desc += "TOTAL damage.";
                }
                else
                {
                    desc += "base damage.";
                }
            }
            if (MainPlugin.VoidDio_BlockCooldown.Value <= 1.0f)
            {
                if (pickup.Length > 0)
                {
                    pickup += " ";
                    desc += " ";
                }
                pickup += string.Format("Block the next source of damage.");
                desc += "<style=cIsHealing>Blocks</style> incoming damage once, recharging after <style=cIsUtility>15 ";
                if (MainPlugin.VoidDio_BlockCooldown.Value < 1.0f)
                {
                    desc += string.Format("<style=cStack>(-{0}% per stack)</style> ", (1.0f - MainPlugin.VoidDio_BlockCooldown.Value) * 100);
                }
                desc += "seconds</style>.";
            }
            if (MainPlugin.VoidDio_Curse.Value > 0f)
            {
                if (pickup.Length > 0)
                {
                    pickup += " ";
                    desc += " ";
                }
                pickup += string.Format("Reduced maximum health.");
                desc += string.Format("<style=cIsHealth>Reduces maximum health by {0}%</style> <style=cStack>(+{0}% per stack)</style>.", MainPlugin.VoidDio_Curse.Value * 100f);
            }
            if (MainPlugin.VoidDio_Corrupt.Value)
            {
                if (pickup.Length > 0)
                {
                    pickup += " ";
                    desc += " ";
                }
                pickup += "<style=cIsVoid>Corrupts ALL items</style>.";
                desc += "<style=cIsVoid>Corrupts ALL items</style>.";
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
                    if (MainPlugin.VoidDio_BlockCooldown.Value <= 1.0f)
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
        private static void AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if (NetworkServer.active)
            {
                if (self.inventory)
                {
                    if (buffDef == DLC1Content.Buffs.BearVoidCooldown)
                    {
                        int itemCount = self.inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoidConsumed);
                        if (itemCount > 0)
                        {
                            duration *= Mathf.Pow(MainPlugin.VoidDio_BlockCooldown.Value, itemCount);
                        }
                    }
                }
            }
            orig(self, buffDef, duration);
        }
        private static void OnTakeDamagePost(On.RoR2.GlobalEventManager.orig_ServerDamageDealt orig, DamageReport dr)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (dr.damageInfo.rejected || dr.damageInfo.procCoefficient <= 0f)
            {
                return;
            }
            if (dr.attacker)
            {
                uint? MaxStacks = null;
                if (dr.damageInfo.inflictor)
                {
                    ProjectileDamage component = dr.damageInfo.inflictor.GetComponent<ProjectileDamage>();
                    if (component && component.useDotMaxStacksFromAttacker)
                    {
                        MaxStacks = component.dotMaxStacksFromAttacker;
                    }
                }
                CharacterBody attackerBody = dr.attackerBody;
                CharacterBody victimBody = dr.victimBody;
                if (attackerBody)
                {
                    CharacterMaster attackerMaster = attackerBody.master;
                    if (attackerMaster && attackerMaster.inventory)
                    {
                        Inventory inventory = attackerMaster.inventory;

                        int itemCount = inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoidConsumed);
                        if (itemCount > 0 && Util.CheckRoll(dr.damageInfo.procCoefficient * itemCount * MainPlugin.VoidDio_CollapseChance.Value, attackerMaster))
                        {
                            DotController.DotDef dotDef = DotController.GetDotDef(DotController.DotIndex.Fracture);

                            float damage = 0.0f;

                            if (MainPlugin.VoidDio_CollapseUseTotal.Value)
                            {
                                damage = MainPlugin.VoidDio_CollapseDamage.Value * dr.damageDealt / dotDef.damageCoefficient / attackerBody.damage;
                            }
                            else
                            {
                                damage = MainPlugin.VoidDio_CollapseDamage.Value / dotDef.damageCoefficient;
                            }

                            if (damage > 0.0f)
                            {
                                DotController.InflictDot(victimBody.gameObject, attackerBody.gameObject, DotController.DotIndex.Fracture, dotDef.interval, damage, MaxStacks);
                            }
                        }
                    }
                }
            }
            orig(dr);
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
        private static void CorruptAllItems(Inventory inventory)
        {
            foreach (ContagiousItemManager.TransformationInfo transformationInfo in ContagiousItemManager.transformationInfos)
            {
                ContagiousItemManager.TryForceReplacement(inventory, transformationInfo.originalItem);
            }
        }
    }
}
