using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Utils
{
    public static class ContentManager
    {
        internal static BuffDef AddBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool isCooldown, bool isHidden = false, bool ignoreGrowthNectar = false)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName + " (FlatItemBuff)";
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.isCooldown = isCooldown;
            buffDef.isHidden = isHidden;
            buffDef.ignoreGrowthNectar = ignoreGrowthNectar;
            (buffDef as ScriptableObject).name = buffDef.name;

            R2API.ContentAddition.AddBuffDef(buffDef);

            return buffDef;
        }

        internal static ItemDef AddItem(ItemDef itemDef)
        {
            (itemDef as ScriptableObject).name = itemDef.name;
            R2API.ContentAddition.AddItemDef(itemDef);
            return itemDef;
        }

        internal static GameObject AddProjectile(GameObject proj)
        {
            R2API.ContentAddition.AddProjectile(proj);
            return proj;
        }

        public static Type RegisterState(Type state)
        {
            bool wasAdded = false;
            R2API.ContentAddition.AddEntityState(state, out wasAdded);
            return state;
        }
    }
}
