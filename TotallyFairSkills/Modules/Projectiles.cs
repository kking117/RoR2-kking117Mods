using System.Collections.Generic;
using R2API;
using UnityEngine;
using RoR2.Projectile;

namespace TotallyFairSkills.Modules
{
    internal static class Projectiles
    {
        internal static List<GameObject> Prefabs = new List<GameObject>();

        internal static GameObject FMJMK2Prefab;
        internal static void RegisterProjectiles()
        {
            if (Main.FMJMK2_Enable.Value)
            {
                CreateFMJMK2();
            }
        }
        internal static void AddProjectile(GameObject proj)
        {
            Prefabs.Add(proj);
        }
        private static void CreateFMJMK2()
        {
            FMJMK2Prefab = Resources.Load<GameObject>("prefabs/projectiles/fmjramping").InstantiateClone(Main.MODTOKEN + "FMJMK2", true);
            FMJMK2Prefab.transform.localScale *= 2f;

            ProjectileSimple ps = FMJMK2Prefab.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;
            ps.desiredForwardSpeed *= 2f;

            ProjectileOverlapAttack poa = FMJMK2Prefab.GetComponent<ProjectileOverlapAttack>();
            poa.onServerHit = null;
            poa.damageCoefficient = 1f;

            ProjectileDamage pd = FMJMK2Prefab.GetComponent<ProjectileDamage>();
            pd.damageType = RoR2.DamageType.Stun1s;

            AddProjectile(FMJMK2Prefab);
        }
    }
}