using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [SearchMenuItem("Ricochet")]
    public class ProjectileRicochetExtension : ProjectileExtension
    {
        public int bouncesCount = 5;
        public float bounceOffDistance = 5f;
        [Tooltip("What percentage of damage should the bouncing projectile take from the projectile before it.")]
        [Range(0f, 1f)] public float bounceDamage = 0.8f;
        public LayerMask detectionLayers;

        private Collider[] detectedColliders = new Collider[10];
        private List<GameObject> alreadyHitGOs = new List<GameObject>();
        private int remainingBounces;

        public ProjectileRicochetExtension() { }

        public void SetData(ProjectileRicochetExtension other)
        {
            bouncesCount = other.bouncesCount;
            bounceOffDistance = other.bounceOffDistance;
            bounceDamage = other.bounceDamage;
            detectionLayers = other.detectionLayers;
        }

        protected override void OnBind()
        {
            base.OnBind();
            
            remainingBounces = bouncesCount;

            alreadyHitGOs.Clear();

            projectile.OnWouldDie.AddListener(OnProjectileWouldDie);
            projectile.OnDespawn.AddListener(OnProjectileDespawns);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            projectile.OnWouldDie.RemoveListener(OnProjectileWouldDie);
            projectile.OnDespawn.RemoveListener(OnProjectileDespawns);
        }

        private void OnProjectileWouldDie()
        {
            if(projectile.ImpactedTarget == null)
                return;

            alreadyHitGOs.Add(projectile.ImpactedTarget.gameObject);

            Vector3 projectilePosition = projectile.transform.position;

            int detectedCount = Physics.OverlapSphereNonAlloc(projectilePosition, bounceOffDistance, detectedColliders, detectionLayers);
            float closestDistance = 9999f;
            
            Collider closestCollider = null;
            for(int i = 0; i < detectedCount; i++)
            {
                Collider collider = detectedColliders[i];

                if(alreadyHitGOs.Contains(collider.gameObject))
                    continue;

                float sqrDistance = (projectilePosition - collider.transform.position).sqrMagnitude;

                if (sqrDistance < closestDistance)
                {
                    closestDistance = sqrDistance;
                    closestCollider = collider;
                }
            }

            if(closestCollider == null)
                return;

            Healthy newTarget = ComponentProvider.GetComponent<TafraActor>(closestCollider.gameObject).GetCachedComponent<Healthy>();

            if(newTarget == null)
                return;

            Vector3 targetPosition = newTarget.TargetPoint.position;

            Vector3 projectileDirection = targetPosition - projectilePosition;

            //projectile.transform.rotation = Quaternion.LookRotation(projectileDirection);

            HitInfo newHitInfo = projectile.HitInfo;
            newHitInfo.damage = Mathf.RoundToInt(newHitInfo.damage * bounceDamage);
            projectile.Initialize(newHitInfo, projectile.transform.position, projectileDirection, newTarget, projectile.Pool, null);
            
            remainingBounces--;

            if (remainingBounces <= 0)
            {
                Unbind();
                return;
            }

            projectile.PreventDeath();
        }
        private void OnProjectileDespawns()
        {
            Unbind();
        }

        public override void CopyTo(ProjectileExtension other)
        {
            if (other is ProjectileRicochetExtension ricochet)
                ricochet.SetData(this);
        }
    }
}