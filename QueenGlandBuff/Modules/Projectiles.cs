using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace QueenGlandBuff.Modules
{
    public static class Projectiles
    {
        internal static GameObject slamrockPrefab;
        internal static void RegisterProjectiles()
        {
            CreateSlamRock();
        }

        internal static void AddProjectile(GameObject proj)
        {
            Prefabs.projectilePrefabs.Add(proj);
        }

        private static void CreateSlamRock()
        {
            slamrockPrefab = PrefabAPI.InstantiateClone(Main.Default_Proj, Main.ModLangToken + "RockProjectile", true);
            slamrockPrefab.GetComponent<ProjectileExplosion>().falloffModel = BlastAttack.FalloffModel.Linear;
            slamrockPrefab.GetComponent<ProjectileExplosion>().bonusBlastForce = Vector3.down * 4f;
            slamrockPrefab.GetComponent<ProjectileExplosion>().blastRadius *= 1.5f;
            AddProjectile(slamrockPrefab);
        }
    }
}
