using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [SearchMenuItem("Ranged Weapon/Basic Projectile Shot")]
    public class BasicProjectileShot : RangedWeaponAction
    {
        [SerializeField] protected Projectile defaultProjectile;
        [SerializeField] protected float beforeShotDelay = 0.1f;
        [SerializeField] protected float afterShotDelay = 0.1f;

        [Header("Animator")]
        [SerializeField] private string shootAnimationTrigger = "Basic Shot";

        protected DynamicPool<Projectile> projectilesPool = new DynamicPool<Projectile>();
        private IEnumerator performanceEnum;
        private int shootAnimationTriggetHash;
        private Transform pointOfInterset;

        private const string id = nameof(BasicProjectileShot);

        protected override void OnInitialize()
        {
            base.OnInitialize();
            shootAnimationTriggetHash = Animator.StringToHash(shootAnimationTrigger);
            pointOfInterset = rangedWeapon.CharacterCombat.PointOfInterest;

            SetProjectile(defaultProjectile);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            projectilesPool.Uninitialize();
        }
        protected override void StartPerformance()
        {
            if (isInAfterPerformance)
            {
                weapon.CharacterCombat.RemovePotentialPoIContinuousApplier(id);
                bodyRotationModule.UnlockTarget(id);
                weapon.CharacterCombat.RemoveAggressiveStateContributer(id);
            }

            if (performanceEnum != null)
                weapon.StopCoroutine(performanceEnum);

            performanceEnum = Performing();

            weapon.StartCoroutine(performanceEnum);
        }
        protected override void InterruptPerformance()
        {
            if(performanceEnum != null)
                weapon.StopCoroutine(performanceEnum);
        }
        protected override void OnPerformanceInputReleased()
        {

        }
        bool isInAfterPerformance;
        private IEnumerator Performing()
        {
            isInAfterPerformance = false;


            weapon.CharacterCombat.AddPotentialPoIContinuousApplier(id);

            Vector3 direction = pointOfInterset.position - weaponMuzzle.position;

            bodyRotationModule.SetLookingInDirection(id, direction);
            weapon.CharacterCombat.AddAggressiveStateContributer(id);
            
            weapon.CharacterAnimator.Animator.SetTrigger(shootAnimationTriggetHash);

            yield return Yielders.GetWaitForSeconds(beforeShotDelay);

            //direction = pointOfInterset.position - spawnPoint;

            Projectile projectile = projectilesPool.RequestUnit(activateUnit: false);

            HitInfo hitInfo = new HitInfo(10);

            projectile.Initialize(hitInfo, weaponMuzzle.position, direction, null, projectilesPool);

            rangedWeapon.Fire(projectile);

            bodyRotationModule.UnsetLookingAtDirection(id);
            bodyRotationModule.LockAtTarget(id, pointOfInterset);

            yield return Yielders.GetWaitForSeconds(afterShotDelay);

            CompletePerformance();

            isInAfterPerformance = true;

            yield return Yielders.GetWaitForSeconds(0.5f);

            weapon.CharacterCombat.RemovePotentialPoIContinuousApplier(id);
            bodyRotationModule.UnlockTarget(id);
            weapon.CharacterCombat.RemoveAggressiveStateContributer(id);
        }

        public void SetProjectile(Projectile projectile)
        {
            bool originalActiveState = projectile.gameObject.activeSelf;

            if(originalActiveState == true)
                projectile.gameObject.SetActive(false);

            GameObject projectileGO = Object.Instantiate(projectile.gameObject);

            if (originalActiveState == true)
                projectile.gameObject.SetActive(true);

            projectilesPool.Uninitialize();

            projectilesPool.AddUnit(projectileGO.GetComponent<Projectile>());

            projectilesPool.Initialize();
        }

    }
}