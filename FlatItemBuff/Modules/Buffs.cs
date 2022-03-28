using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Modules
{
    public static class Buffs
    {
        internal static List<BuffDef> buffDefs = new List<BuffDef>();
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool isCooldown)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.isCooldown = isCooldown;
            (buffDef as ScriptableObject).name = buffDef.name;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}
