using System;
using System.Collections.Generic;
using TafraKit.Healthies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TafraKit.Weaponry
{
    public abstract class SingleProjectile : Projectile
    {
        [Tooltip("The damage will be plus or minus this value.")]
        [SerializeField] private int damageWiggle = 0;
        [Tooltip("The total seconds the projectile should remain idle before despawning after it hits its target. Typically used to give the trail or VFX enough time to hide.")]
        [SerializeField] protected float despawnAfterReachingTargetDelay = 3f;
        [Tooltip("A list of game objects that should only be active if the projectile is active (visually visible and acting) and deactive once it's no longer active.")]
        [SerializeField] protected GameObject[] activeGameObjects;
        [Tooltip("A list of particle systems that should only be playing if the projectile is active (visually visible and acting) and deactive once it's no longer active.")]
        [SerializeField] protected ParticleSystem[] activeParticleSystems;
        [Tooltip("Trails in this list will be cleared once the projectile spawns")]
        [SerializeField] protected TrailRenderer[] trails;
        [SerializeField] protected GameObject impactVFX;
        [SerializeField] protected ParticleSystem impactDecalPS;
        [SerializeField] protected bool alignImpactWithHitNormal;

        protected bool foundTarget;
        [NonSerialized] protected DynamicPool<GameObject> impactPool;
        [NonSerialized] protected DynamicPool<ParticleSystem> impactDecalPool;

        private GameObject impactUnitVFX;
        private ParticleSystem decalUnitVFX;
        private int impactPoolHash = -1;
        private int decalPoolHash = -1;

        protected override void Awake()
        {
            base.Awake();

            if(impactVFX != null)
            {
                impactPoolHash = Animator.StringToHash(impactVFX.name);
                impactPool = TafraPooler.CreateOrGetPool(impactPoolHash, impactVFX, true);
            }

            if(impactDecalPS != null)
            {
                decalPoolHash = Animator.StringToHash(impactDecalPS.name);
                impactDecalPool = TafraPooler.CreateOrGetPool(decalPoolHash, impactDecalPS, true);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(impactUnitVFX != null)
            {
                impactPool.ReleaseUnit(impactUnitVFX, false);
                impactUnitVFX = null;
            }

            if(decalUnitVFX != null)
            {
                impactDecalPool.ReleaseUnit(decalUnitVFX, false);
                decalUnitVFX = null;
            }
        }
        protected virtual void FoundTarget(Healthy healthy, Vector3 impactPoint, Vector3 impactNormal)
        {
            impactedTarget = healthy;

            canHitTarget = true;

            onWouldHitTarget?.Invoke();

            if(!canHitTarget)
                return;

            if (healthy && hitInfo.damage > 0)
            {
                HitInfo newHitInfo = hitInfo;

                newHitInfo.damage = hitInfo.damage + Random.Range(-damageWiggle, damageWiggle + 1);
                newHitInfo.pointOfImpact = impactPoint;
                newHitInfo.pointOfImpactNormal = impactNormal;
                newHitInfo.isRanged = true;

                //TODO: Healthy - Push Back
                //healthy.TakeDamage(attackDamage, isShotByPlayer, impactPoint, transform.forward, pushBackPower, pushBackDuration,isCritical);
                healthy.TakeDamage(hitInfo);
            }

            //TODO: Healthy
            //if (healthy && sfxBasedOnID.TryGetValue(healthy.ID, out var sfx))
            //    SFXPlayer.Play(sfx);
            //else
                SFXPlayer.Play(genericImpactSFX);

            if (impactPool != null)
            {
                impactUnitVFX = impactPool.RequestUnit();

                if (impactUnitVFX != null)
                {
                    impactUnitVFX.transform.SetPositionAndRotation(impactPoint, transform.rotation);

                    if (alignImpactWithHitNormal)
                        impactUnitVFX.transform.forward = impactNormal;
                }
            }
            
            if (!healthy && impactDecalPool != null)
            {
                decalUnitVFX = impactDecalPool.RequestUnit();
                
                if (decalUnitVFX != null)
                {
                    decalUnitVFX.transform.position = impactPoint;

                    if (alignImpactWithHitNormal)
                        decalUnitVFX.transform.forward = impactNormal;
                }
                
                decalUnitVFX.Play();
            }
            
            canDie = true;

            onWouldDie?.Invoke();

            if(canDie)
            {
                for(int i = 0; i < activeParticleSystems.Length; i++)
                {
                    activeParticleSystems[i].Stop(true);
                }
                for(int i = 0; i < activeGameObjects.Length; i++)
                {
                    activeGameObjects[i].SetActive(false);
                }

                DespawnAfter(despawnAfterReachingTargetDelay);

                isActive = false;
            }

            onImpactAction?.Invoke(this);
            OnImpact?.Invoke(healthy);
        }
        protected override void OnInitialize()
        {
            for (int i = 0; i < activeParticleSystems.Length; i++)
            {
                activeParticleSystems[i].Play(true);
            }
            for (int i = 0; i < activeGameObjects.Length; i++)
            {
                activeGameObjects[i].SetActive(true);
            }

            for(int i = 0; i < trails.Length; i++)
            {
                trails[i].Clear();
            }

            isActive = true;
        }
        protected override void OnBeforeDespawn()
        {
            if(impactUnitVFX != null)
            {
                impactPool.ReleaseUnit(impactUnitVFX, false);
                impactUnitVFX = null;
            }

            if(decalUnitVFX != null)
            {
                impactDecalPool.ReleaseUnit(decalUnitVFX, false);
                decalUnitVFX = null;
            }
        }

        public override void UninitializeGlobalPools()
        {
            if(impactPoolHash != -1)
            {
                TafraPooler.DestroyLocalPool<GameObject>(impactPoolHash);
                impactPoolHash = -1;
            }

            if(decalPoolHash != -1)
            {
                TafraPooler.DestroyLocalPool<ParticleSystem>(decalPoolHash);
                decalPoolHash = -1;
            }
        }
    }
}