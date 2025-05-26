using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.Healthies;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Healthy/Revive"), GraphNodeName("Revive", "Revive")]
    public class ReviveState : ActionState
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
                Revive();
                return;
            }
            //else
            //    Complete();
        }

        private void Revive()
        {
            healthy.Revive();
            Complete();
        }
    }
}