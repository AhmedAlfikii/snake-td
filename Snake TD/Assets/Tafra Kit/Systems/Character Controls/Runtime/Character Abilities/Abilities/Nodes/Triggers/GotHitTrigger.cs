using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Triggers/Healthy/Got Hit"), GraphNodeName("Got Hit")]
    public class GotHitTrigger : TriggerAbilityNode
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
            
            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
        }
        protected override void OnEnd()
        {
            base.OnEnd();

            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
        }

        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            TriggerInvoked();
        }
    }
}