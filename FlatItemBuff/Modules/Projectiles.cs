using System.Collections.Generic;
using R2API;
using UnityEngine;
using RoR2.Projectile;

namespace FlatItemBuff.Modules
{
    internal static class Projectiles
    {
        internal static List<GameObject> Prefabs = new List<GameObject>();
        internal static void AddProjectile(GameObject proj)
        {
            Prefabs.Add(proj);
        }
    }
}