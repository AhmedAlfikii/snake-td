using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.Healthies;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Healthy/Hit Monitor"), GraphNodeName("Hit Monitor")]
    public class HitMonitorState : ActionState
    {
        [SerializeField] private bool onlyMonitorPlayerHits;

        private Healthy healthy;

        protected override void OnInitialize()
        {
            healthy = agent.GetCachedComponent<Healthy>();
        }
        protected override void OnPlay()
        {
            if(healthy.IsInitialized && healthy.IsDead)
            {
                Complete();
                return;
            }

            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
        }
        protected override void OnConclude()
        {
            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
        }

        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            if (onlyMonitorPlayerHits && !hitInfo.hitterIsPlayer)
                return;

            Complete();
        }
    }
}