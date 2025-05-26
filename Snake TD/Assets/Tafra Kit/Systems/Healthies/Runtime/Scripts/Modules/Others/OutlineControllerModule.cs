using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Others/Outline Controller")]
    public class OutlineControllerModule : HealthyModule
    {
        [Header("References")]
        [SerializeField] private TafraOutline outline;

        [Header("Properties")]
        [SerializeField] private bool disableOutlineOnDeath = true;
        [SerializeField] private bool enableOutlineOnRevive = true;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            if(disableOutlineOnDeath)
                healthy.Events.OnDeath.AddListener(OnDeath);

            if (enableOutlineOnRevive)
                healthy.Events.OnRevive.AddListener(OnRevive);
        }
        protected override void OnDisable()
        {
            if(disableOutlineOnDeath)
                healthy.Events.OnDeath.RemoveListener(OnDeath);

            if(enableOutlineOnRevive)
                healthy.Events.OnRevive.RemoveListener(OnRevive);
        }
        private void OnDeath(Healthy healthy, HitInfo killerHit)
        {
            if(disableOutlineOnDeath)
                outline.enabled = false;
        }
        private void OnRevive()
        {
            if(enableOutlineOnRevive)
                outline.enabled = true;
        }
    }
}