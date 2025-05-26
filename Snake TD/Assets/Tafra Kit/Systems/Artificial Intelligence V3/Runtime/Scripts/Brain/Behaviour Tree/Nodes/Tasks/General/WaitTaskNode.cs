using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/General/Wait"), GraphNodeName("Wait", "Wait")]
    public class WaitTaskNode : TaskNode
    {
        [SerializeField] private BlackboardDynamicFloatGetter waitDuration = new BlackboardDynamicFloatGetter(1);

        [NonSerialized] private float endTime;

        protected override void OnInitialize()
        {
            waitDuration.Initialize(agent.BlackboardCollection);
        }

        protected override void OnStart()
        {
            endTime = Time.time + waitDuration.Value;
        }
        protected override BTNodeState OnUpdate()
        {
            if(Time.time < endTime)
                return BTNodeState.Running;

            return BTNodeState.Success;
        }
    }
}