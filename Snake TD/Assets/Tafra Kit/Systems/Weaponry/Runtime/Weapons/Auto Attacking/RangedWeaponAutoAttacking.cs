using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Healthies;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TafraKit.Weaponry
{
    public class RangedWeaponAutoAttacking : WeaponAutoAttacking
    {
        #region Serialized Fields
        [Header("Ranged Weapon Properties")]
        [SerializeField] protected Projectile defaultProjectile;
        [Tooltip("The transform where the projectiles will be spawned at.")]
        [SerializeField] protected Transform muzzle;
        [Tooltip("Should the projectile direction be flattened? Only relevant if the projectile flies in the direction it's facing and doesn't have a preset path.")]
        [SerializeField] protected bool flattenProjectileYDirection = true;
        [SerializeField] protected float shootDelay = 0.15f;

        [Header("Projectile Extensions")]
        [SerializeReferenceListContainer("extensions", true, "Extension", "Extensions")]
        [SerializeField] private ProjectileExtensionContainer extensionsContainer;

        [Header("Accuracy")]
        [Tooltip("The maximum angle of inaccuracy.")]
        [SerializeField] protected TafraFloat inaccuracyAngle = new TafraFloat(0);
        [SerializeField] protected float damageWigglePercentage = 0.05f;

        [Header("Shot Effects")]
        [Tooltip("The sound effect that will be played whenever a projectile is shot.")]
        [SerializeField] protected SFXClips shotAudio;
        [Tooltip("The particle system that will be played whenever a projectile is shot.")]
        [SerializeField] protected ParticleSystem shotPS;
        #endregion

        #region Private Fields
        private int extensionsCount;
        private List<DynamicPool<ProjectileExtension>> defaultExtensionPools = new List<DynamicPool<ProjectileExtension>>();
        private List<ProjectileExtension> defaultExtensions = new List<ProjectileExtension>();
        protected Projectile nextProjectileOverride;
        protected DynamicPool<Projectile> nextProjectileOverridePool;
        protected DynamicPool<Projectile> projectilesPool = new DynamicPool<Projectile>();
        protected UnityEvent<Projectile> onShotProjectile = new UnityEvent<Projectile>();
        #endregion

        #region Properties
        public Transform Muzzle => muzzle;
        public UnityEvent<Projectile> OnShotProjectile => onShotProjectile;
        #endregion

        #region MonoBehaviour Messages
        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();

            defaultExtensions = extensionsContainer.Extensions;
            extensionsCount = defaultExtensions.Count;

            for (int i = 0; i < extensionsCount; i++)
            {
                DynamicPool<ProjectileExtension> pool = new DynamicPool<ProjectileExtension>();
                pool.Construct(new List<ProjectileExtension>(), null);

                ProjectileExtension sample = Activator.CreateInstance(extensionsContainer.Extensions[i].GetType()) as ProjectileExtension;

                extensionsContainer.Extensions[i].CopyTo(sample);

                pool.AddUnit(sample);

                pool.Initialize();

                defaultExtensionPools.Add(pool);
            }

            projectilesPool.Construct(new List<Projectile>(), null);

            SetProjectile(defaultProjectile);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            //List<Projectile> poolRemainingProjectiles = projectilesPool.GetAllRemainingUnits();
            //List<Projectile> poolTakenProjectiles = projectilesPool.GetAllTakenUnits();

            //if(poolRemainingProjectiles.Count > 0)
            //    poolRemainingProjectiles[0].UninitializeGlobalPools();
            //else if(poolTakenProjectiles.Count > 0)
            //    poolTakenProjectiles[0].UninitializeGlobalPools();

            projectilesPool.Uninitialize();

            for(int i = 0; i < extensionsCount; i++)
            {
                defaultExtensionPools[i].Uninitialize();
            }
        }
        public override void PerformAttackAction(Healthy target)
        {
            if(target == null || target.IsDead)
                return;

            SFXPlayer.Play(shotAudio);

            if(shotPS)
                shotPS.Play();

            bool isOverridenProjectile = nextProjectileOverride != null;

            Projectile projectile = isOverridenProjectile ? nextProjectileOverride : projectilesPool.RequestUnit(null, false);
            DynamicPool<Projectile> pool = isOverridenProjectile ? nextProjectileOverridePool : projectilesPool;

            projectile.gameObject.SetActive(true);

            if(isOverridenProjectile)
                nextProjectileOverride = null;

            Vector3 targetPosition = target.TargetPoint.position;

            if(flattenProjectileYDirection)
                targetPosition.y = muzzle.position.y;

            Vector3 projectileDirection = targetPosition - muzzle.position;

            float inaccuracyAngleValue = inaccuracyAngle.Value;

            if(inaccuracyAngleValue > 0.001f)
            {
                float addedYAngle = Random.Range(-inaccuracyAngleValue, inaccuracyAngleValue);

                Quaternion driftRot = Quaternion.AngleAxis(addedYAngle, Vector3.up);

                projectileDirection = driftRot * projectileDirection;

                float addedXAngle = Random.Range(-inaccuracyAngleValue, inaccuracyAngleValue);
                Quaternion driftXRot = Quaternion.AngleAxis(addedXAngle, muzzle.right);

                projectileDirection = driftXRot * projectileDirection;
            }

            projectile.transform.position = muzzle.position;

            int damageWiggleRange = Mathf.RoundToInt(curDamage * damageWigglePercentage);
            int damageValue = curDamage + Random.Range(-damageWiggleRange, damageWiggleRange + 1);
            bool isCritical = false;

            if(curCritChance > 0 && Random.value <= curCritChance)
            {
                isCritical = true;

                damageValue = Mathf.RoundToInt(damageValue * curCritDamage);
            }

            projectile.Initialize(new HitInfo(damageValue, isCritical, false, true, holderActor, true), muzzle.position, projectileDirection, target, pool, null);

            for (int i = 0; i < extensionsCount; i++)
            {
                DynamicPool<ProjectileExtension> extensionPool = defaultExtensionPools[i];

                ProjectileExtension extension = extensionPool.RequestUnit();

                defaultExtensions[i].CopyTo(extension);

                extension.Bind(projectile);
                
                //Debug.Log($"Binding {extension} to {projectile}");

                extension.OnUnbinded = () =>
                {
                    //Debug.Log($"Unbinding {extension} from {projectile}");

                    extensionPool.ReleaseUnit(extension);
                };
            }

            OnProjectileShot(projectile);
            onShotProjectile?.Invoke(projectile);
        }

        public void SetProjectile(Projectile projectile)
        {
            GameObject projectileGO = GameObject.Instantiate(projectile.gameObject);

            projectilesPool.Uninitialize();

            projectilesPool.AddUnit(projectileGO.GetComponent<Projectile>());

            projectilesPool.Initialize();
        }

        public override void StartAttackingTarget(Healthy target)
        {
            StartCoroutine(AttackProcess(target));
        }

        private IEnumerator AttackProcess(Healthy target)
        {
            animator.SetTrigger(animatorAttackTriggerHash);

            yield return Yielders.GetWaitForSeconds(shootDelay);

            //if(!fakeAttack)
            {
                if(target)
                    PerformAttackAction(target);
            }
        }

        public virtual void OnProjectileShot(Projectile projectile)
        { 

        }
    }
}