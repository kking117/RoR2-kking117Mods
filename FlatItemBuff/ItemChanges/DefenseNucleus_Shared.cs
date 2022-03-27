using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using FlatItemBuff.Utils;

namespace FlatItemBuff.ItemChanges
{
    public class DefenseNucleus_Shared
    {
        public static void EnableChanges()
        {
            if (MainPlugin.NucleusShared_TweakAI.Value)
            {
                UpdateAI();
            }
            if (MainPlugin.NucleusShared_BlastRadius.Value > 0f && MainPlugin.NucleusShared_BlastDamage.Value > 0f)
            {
                On.RoR2.CharacterMaster.OnBodyDeath += CharacterMaster_OnBodyDeath;
            }
            On.RoR2.Projectile.ProjectileSpawnMaster.SpawnMaster += Projectile_SpawnMaster;
        }
        private static void UpdateAI()
        {
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MinorConstructOnKillMaster.prefab").WaitForCompletion();
            BaseAI baseai = prefab.GetComponent<BaseAI>();
            if (baseai)
            {
                baseai.fullVision = true;
                baseai.neverRetaliateFriendlies = true;
            }
        }
        private static void Projectile_SpawnMaster(On.RoR2.Projectile.ProjectileSpawnMaster.orig_SpawnMaster orig, ProjectileSpawnMaster self)
        {
            if (self.deployableSlot == DeployableSlot.MinorConstructOnKill)
            {
                ProjectileController projController = self.GetComponent<ProjectileController>();
                if (projController && projController.owner)
                {
                    CharacterBody ownerbody = projController.owner.GetComponent<CharacterBody>();
                    if (ownerbody && ownerbody.master)
                    {
                        int killCount = ownerbody.master.GetDeployableCount(DeployableSlot.MinorConstructOnKill) + 1 - ownerbody.master.GetDeployableSameSlotLimit(DeployableSlot.MinorConstructOnKill);
                        if (killCount > 0)
                        {
                            Helpers.KillDeployables(ownerbody.master, DeployableSlot.MinorConstructOnKill, killCount);
                        }
                    }
                }
            }
            orig(self);
        }
        private static void CharacterMaster_OnBodyDeath(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (NetworkServer.active)
            {
                if (BodyCatalog.FindBodyIndex("MinorConstructOnKillBody") == body.bodyIndex)
                {
                    float damage = body.damage * MainPlugin.NucleusShared_BlastDamage.Value;
                    float radius = MainPlugin.NucleusShared_BlastRadius.Value;
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                    {
                        origin = body.transform.position,
                        scale = radius,
                        rotation = Util.QuaternionSafeLookRotation(body.transform.forward)
                    }, true);
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.position = body.transform.position;
                    blastAttack.baseDamage = damage;
                    blastAttack.baseForce = 0f;
                    blastAttack.radius = radius;
                    blastAttack.attacker = body.gameObject;
                    blastAttack.inflictor = null;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    blastAttack.crit = false;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.damageColorIndex = DamageColorIndex.Default;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.damageType = DamageType.SlowOnHit;
                    blastAttack.Fire();
                }
            }
        }
    }
}
