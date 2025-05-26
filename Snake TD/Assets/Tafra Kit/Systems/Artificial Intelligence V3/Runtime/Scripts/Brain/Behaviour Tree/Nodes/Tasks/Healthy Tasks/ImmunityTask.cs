using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.Healthies;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Healthy/Immunity"), GraphNodeName("Immunity", "Immunity")]
    public class ImmunityTask : TaskNode
    {
        [SerializeField] private string toggleID = "immunityTask";

        private TafraKit.Healthies.Healthy healthy;
        private ImmunityModule immunityModule;

        protected override void OnInitialize()
        {
            healthy = agent.GetCachedComponent<TafraKit.Healthies.Healthy>();
            immunityModule = healthy.GetModule<ImmunityModule>();
        }
        protected override void OnStart()
        {
            if (immunityModule != null)
                immunityModule.AddImmunityActivator(toggleID);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if(immunityModule != null)
                immunityModule.RemoveImmunityActivator(toggleID);
        }
    }
}