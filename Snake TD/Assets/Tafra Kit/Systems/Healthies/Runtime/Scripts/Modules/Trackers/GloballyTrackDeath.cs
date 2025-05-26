using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Tracking/Globally Track Death")]
    public class GloballyTrackDeath : HealthyModule
    {
        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            healthy.Events.OnDeath.AddListener(OnDeath);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnDeath.RemoveListener(OnDeath);
        }

        private void OnDeath(Healthy healthy, HitInfo killHit)
        {
            HealthiesTracker.SignalHealthyDeath(healthy, killHit);
        }
    }
}