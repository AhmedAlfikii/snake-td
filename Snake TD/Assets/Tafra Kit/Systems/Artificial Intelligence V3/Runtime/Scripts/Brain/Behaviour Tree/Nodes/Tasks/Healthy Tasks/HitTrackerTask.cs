using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.Healthies;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Healthy/Hit Tracker"), GraphNodeName("Hit Tracker", "Got Hit")]
    public class HitTrackerTask : TaskNode
    {
        private TafraKit.Healthies.Healthy healthy;
        private bool gotHit;

        protected override void OnInitialize()
        {
            healthy = agent.GetCachedComponent<TafraKit.Healthies.Healthy>();
        }
        protected override void OnStart()
        {
            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
        }
        protected override void OnEnd()
        {
            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
            gotHit = false;
        }
        protected override BTNodeState OnUpdate()
        {
            if (gotHit)
                return BTNodeState.Success;
            else 
                return BTNodeState.Running;
        }

        private void OnTakenDamage(TafraKit.Healthies.Healthy health, HitInfo hitInfo)
        {
            gotHit = true;
        }
    }
}