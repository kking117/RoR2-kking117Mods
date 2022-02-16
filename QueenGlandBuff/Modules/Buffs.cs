using System;
using System.Collections.Generic;
using System.Text;
using QueenGlandBuff.States;
using RoR2;
using UnityEngine;

namespace QueenGlandBuff.Modules
{
    public static class Buffs
    {
        internal static BuffDef Staunching;
        internal static BuffDef BeetleFrenzy;
        internal static List<BuffDef> buffDefs = new List<BuffDef>();
        internal static void RegisterBuffs()
        {
            if (MainPlugin.Gland_AddSpecial.Value)
            {
                Staunching = AddNewBuff(MainPlugin.MODNAME + "Staunch", RoR2Content.Buffs.BeetleJuice.iconSprite, RoR2Content.Buffs.TeamWarCry.buffColor, false, true);
                BeetleFrenzy = AddNewBuff(MainPlugin.MODNAME + "BeetleFrenzy", RoR2Content.Buffs.TeamWarCry.iconSprite, RoR2Content.Buffs.BeetleJuice.buffColor, false, true);
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
            (buffDef as ScriptableObject).name = buffDef.name;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}
