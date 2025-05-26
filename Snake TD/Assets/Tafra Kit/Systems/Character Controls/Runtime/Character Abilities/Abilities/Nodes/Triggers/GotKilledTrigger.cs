using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Triggers/Healthy/Got Killed"), GraphNodeName("Got Killed")]
    public class GotKilledTrigger : TriggerAbilityNode
    {
        private Healthy healthy;

        public override bool UseTriggerBlackboard => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            healthy = actor.GetCachedComponent<Healthy>();
        }
        protected override void OnStart()
        {
            base.OnStart();
            
            if(healthy == null)
                return;
            
            healthy.Events.OnDeath.AddListener(OnDeath);
        }
        protected override void OnEnd()
        {
            base.OnEnd();

            healthy.Events.OnDeath.RemoveListener(OnDeath);
        }

        private void OnDeath(Healthy healthy, HitInfo hitInfo)
        {
            TriggerInvoked();
        }
    }
}