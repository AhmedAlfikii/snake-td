using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Weaponry
{
    /// <summary>
    /// A projectile that will move in a curve until it reaches its target, damnaging its target upon arrival.
    /// </summary>
    public class DirectedObliqueProjectile : SingleProjectile
    {
        [SerializeField] private float defaultTarvelDuration = 0.5f;
        [SerializeField] private float defaultThrowHeight = 6f;
        [SerializeField] private float defaultMidPointPercentage = 0.5f;
        [SerializeField] private TafraAdvancedFloat defaultTravelAngle;

        private Vector3 midPoint;
        private float travelDuration;
        private float throwHeight;
        private float throwAngle;
        private float midPercentage;
        private float startTime;
        private float endTime;
        private bool propertiesAssigned;

        /// <summary>
        /// Call this before calling Initialize(...).
        /// </summary>
        /// <param name="travelDuration"></param>
        /// <param name="throwHeight"></param>
        /// <param name="throwAngle"></param>
        /// <param name="midPoint"></param>
        public void SetProperties(float travelDuration, float throwHeight, float throwAngle, float midPercentage = 0.5f)
        {
            this.travelDuration = travelDuration;
            this.throwHeight = throwHeight;
            this.throwAngle = throwAngle;
            this.midPercentage = midPercentage;

            propertiesAssigned = true;
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if(!propertiesAssigned)
            {
                travelDuration = defaultTarvelDuration;
                throwHeight = defaultThrowHeight;
                throwAngle = defaultTravelAngle.Value;
            }

            startTime = Time.time;
            endTime = startTime + travelDuration;

            Vector3 targetPoint = target.TargetPoint.position;

            Vector3 dir = targetPoint - spawnPoint;

            Vector3 mid = (dir * defaultMidPointPercentage) + new Vector3(0, throwHeight, 0);
            
            mid = Quaternion.AngleAxis(throwAngle, dir) * mid;

            midPoint = spawnPoint + mid;
        }
        protected void Update()
        {
            if (!isActive)
                return;

            Vector3 targetPoint = target.TargetPoint.position;

            if(Time.time < endTime)
            {
                float t = (Time.time - startTime) / travelDuration;
                Vector3 pos = ZBezier.GetPointOnQuadraticCurve(t, spawnPoint, midPoint, targetPoint);
                transform.position = pos;
            }
            else
            {
                transform.position = targetPoint;

                FoundTarget(target, targetPoint, Vector3.up);
            }
        }

        protected override void OnBeforeDespawn()
        {
            base.OnBeforeDespawn();
            propertiesAssigned = false;
        }
    }
}