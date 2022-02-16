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
            if (MainPlugin.Gland_PrimaryBuff.Value || MainPlugin.Gland_SecondaryBuff.Value)
            {
                CreateSlamRock();
            }
        }
        internal static void AddProjectile(GameObject proj)
        {
            Prefabs.projectilePrefabs.Add(proj);
        }
        private static void CreateSlamRock()
        {
            slamrockPrefab = PrefabAPI.InstantiateClone(MainPlugin.Default_Proj, MainPlugin.MODTOKEN + "RockProjectile", true);
            slamrockPrefab.GetComponent<ProjectileExplosion>().falloffModel = BlastAttack.FalloffModel.Linear;
            slamrockPrefab.GetComponent<ProjectileExplosion>().bonusBlastForce = Vector3.down * 4f;
            slamrockPrefab.GetComponent<ProjectileExplosion>().blastRadius *= 1.5f;
            slamrockPrefab.GetComponent<ProjectileDamage>().force = Resources.Load<GameObject>("prefabs/projectiles/sunder").GetComponent<ProjectileDamage>().force * -1f;
            AddProjectile(slamrockPrefab);
        }
    }
}
