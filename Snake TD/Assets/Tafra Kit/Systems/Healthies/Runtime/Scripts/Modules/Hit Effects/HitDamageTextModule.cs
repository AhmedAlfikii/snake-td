using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Hit Effects/Hit Damage Text")]
    public class HitDamageTextModule : HealthyModule
    {
        public override bool DisableOnDeath => true;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
        }

        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            DamageDisplayer.DisplayDamageOnTarget(healthy, hitInfo);
        }
    }
}