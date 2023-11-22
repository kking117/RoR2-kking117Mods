using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Modules
{
    public static class Items
    {
        internal static List<ItemDef> itemDefs = new List<ItemDef>();
        internal static void AddNewItem(ItemDef itemDef)
        {
            (itemDef as ScriptableObject).name = itemDef.name;
            itemDefs.Add(itemDef);
        }
    }
}
