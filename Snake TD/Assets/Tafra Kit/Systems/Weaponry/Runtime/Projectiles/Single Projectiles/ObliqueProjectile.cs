using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Weaponry
{
    /// <summary>
    /// A projectile that will move in a curve until it reaches its destination, exploding on arrival and damaging damageables in a radius.
    /// </summary>
    public class ObliqueProjectile : SingleProjectile
    {
        [Header("Collision Detection")]
        [SerializeField] private bool hitTriggers = true;
        [SerializeField] private TafraLayerMask damageableLayers;

        private Vector3 midPoint;
        private Vector3 landingPoint;
        private float throwHeight;
        private float landingDamageRadius;
        private float travelDuration;
        private float startTime;
        private float endTime;
        private bool displayAttackIndicator;
        private Collider[] detectedColliders = new Collider[10];
        private CircleAttackIndicatorData indicatorData;
        private CircleAttackIndicator activeIndicator;

        /// <summary>
        /// Call this before calling Initialize(...).
        /// </summary>
        /// <param name="landingPoint"></param>
        /// <param name="travelDuration"></param>
        public void SetProperties(Vector3 landingPoint, float travelDuration, float throwHeight, float landingDamageRadius, bool displayAttackIndicator)
        {
            this.landingPoint = landingPoint;
            this.travelDuration = travelDuration;
            this.throwHeight = throwHeight;
            this.landingDamageRadius = landingDamageRadius;
            this.displayAttackIndicator = displayAttackIndicator;
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            startTime = Time.time;
            endTime = startTime + travelDuration;

            midPoint = spawnPoint + ((landingPoint - spawnPoint) / 2f + new Vector3(0, throwHeight, 0));

            if(displayAttackIndicator)
            {
                if (indicatorData == null)
                    indicatorData = new CircleAttackIndicatorData();

                indicatorData.SetData(landingPoint, landingDamageRadius);
                activeIndicator = AttackIndicatorsHandler.Instance.RequestAttackIndicator<CircleAttackIndicator>(indicatorData);

                activeIndicator.StartCharging(travelDuration);
            }
        }
        protected void Update()
        {
            if (!isActive)
                return;

            if(Time.time < endTime)
            {
                float t = (Time.time - startTime) / travelDuration;
                Vector3 pos = ZBezier.GetPointOnQuadraticCurve(t, spawnPoint, midPoint, landingPoint);
                transform.position = pos;
            }
            else
            {
                transform.position = landingPoint;

                int detectedHitsCount = Physics.OverlapSphereNonAlloc(landingPoint, landingDamageRadius, detectedColliders, damageableLayers.Value, hitTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore);

                for (int i = 0; i < detectedHitsCount; i++)
                {
                    TafraActor hitActor = ComponentProvider.GetComponent<TafraActor>(detectedColliders[i].gameObject);

                    if(hitActor != null)
                    {
                        Healthy hitHealthy = hitActor.GetCachedComponent<Healthy>();

                        hitHealthy.TakeDamage(hitInfo);
                    }
                }

                if(activeIndicator != null)
                {
                    AttackIndicatorsHandler.Instance.ReleaseAttackIndicator(activeIndicator);
                    activeIndicator = null;
                }

                FoundTarget(null, landingPoint, Vector3.up);
            }
        }

        protected override void OnBeforeDespawn()
        {
            base.OnBeforeDespawn();

            if(activeIndicator != null)
            {
                AttackIndicatorsHandler.Instance.ReleaseAttackIndicator(activeIndicator);
                activeIndicator = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(activeIndicator != null)
            {
                AttackIndicatorsHandler.Instance.ReleaseAttackIndicator(activeIndicator);
                activeIndicator = null;
            }
        }
    }
}