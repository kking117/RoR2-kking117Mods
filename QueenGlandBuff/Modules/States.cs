using System;
using System.Collections.Generic;
using System.Text;
using QueenGlandBuff.States;

namespace QueenGlandBuff.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            if (MainPlugin.Gland_PrimaryBuff.Value)
            {
                States.entityStates.Add(typeof(Slam));
            }
            if (MainPlugin.Gland_SecondaryBuff.Value)
            {
                States.entityStates.Add(typeof(Sunder));
            }
            if (MainPlugin.Gland_AddUtility.Value)
            {
                States.entityStates.Add(typeof(Recall));
            }
            if (MainPlugin.Gland_AddSpecial.Value)
            {
                States.entityStates.Add(typeof(Staunch));
            }
        }
        internal static List<Type> entityStates = new List<Type>();
    }
}
