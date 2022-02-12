using System;
using System.Collections.Generic;

namespace FlatItemBuff.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();
        public static void RegisterState(Type state)
        {
            States.entityStates.Add(state);
        }
    }
}
