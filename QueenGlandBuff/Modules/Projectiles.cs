using RoR2;
using UnityEngine;

namespace QueenGlandBuff.Modules
{
    public static class Projectiles
    {
        internal static void AddProjectile(GameObject proj)
        {
            Prefabs.projectilePrefabs.Add(proj);
        }
    }
}
