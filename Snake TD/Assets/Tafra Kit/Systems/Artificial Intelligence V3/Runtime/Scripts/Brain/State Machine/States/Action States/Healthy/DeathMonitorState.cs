using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.Healthies;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Healthy/Death Monitor"), GraphNodeName("Death Monitor", "DM")]
    public class DeathMonitorState : ActionState
    {
        private TafraKit.Healthies.Healthy healthy;

        protected override void OnInitialize()
        {
            healthy = agent.GetCachedComponent<TafraKit.Healthies.Healthy>();
        }
        protected override void OnPlay()
        {
            if(healthy.IsInitialized && healthy.IsDead)
            {
                Complete();
                return;
            }

            healthy.Events.OnDeath.AddListener(OnDeath);
        }
        protected override void OnConclude()
        {
            healthy.Events.OnDeath.RemoveListener(OnDeath);
        }

        private void OnDeath(Healthy healthy, HitInfo killerHit)
        {
            Complete();
        }
    }
}