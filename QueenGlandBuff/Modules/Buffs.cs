using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace QueenGlandBuff.Modules
{
    public static class Buffs
    {
        internal static List<BuffDef> buffDefs = new List<BuffDef>();
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool isCooldown)
        {
            buffName += "(" + MainPlugin.MODNAME + ")";
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.isCooldown = isCooldown;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            (buffDef as ScriptableObject).name = buffDef.name;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}
