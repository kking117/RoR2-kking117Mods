using System;
using System.Collections.Generic;

namespace TotallyFairSkills.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            if (Main.FMJMK2_Enable.Value)
            {
                States.entityStates.Add(typeof(TotallyFairSkills.States.FMJMK2));
                States.entityStates.Add(typeof(TotallyFairSkills.States.ReloadPistolsFancy));
            }
            if (Main.ShowTime_Enable.Value)
            {
                States.entityStates.Add(typeof(TotallyFairSkills.States.ShowTime));
            }
        }
        internal static List<Type> entityStates = new List<Type>();
    }
}
