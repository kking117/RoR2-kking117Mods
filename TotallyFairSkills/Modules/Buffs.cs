using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace TotallyFairSkills.Modules
{
    public static class Buffs
    {
        internal static BuffDef ShowOff;
        internal static BuffDef ShowOffActive;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();
        internal static void RegisterBuffs()
        {
            if (Main.ShowTime_Enable.Value)
            {
                ShowOff = AddNewBuff("Show-Off", RoR2Content.Buffs.MercExpose.iconSprite, RoR2Content.Buffs.Energized.buffColor, false, false);
                ShowOffActive = AddNewBuff("Show-Off Active", RoR2Content.Buffs.MercExpose.iconSprite, RoR2Content.Buffs.FullCrit.buffColor, false, false);
            }
        }
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}
