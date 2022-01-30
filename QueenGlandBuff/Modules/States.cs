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
            States.entityStates.Add(typeof(Slam));
            States.entityStates.Add(typeof(Sunder));
            States.entityStates.Add(typeof(Recall));
            States.entityStates.Add(typeof(Staunch));
        }
        internal static List<Type> entityStates = new List<Type>();
    }
}
