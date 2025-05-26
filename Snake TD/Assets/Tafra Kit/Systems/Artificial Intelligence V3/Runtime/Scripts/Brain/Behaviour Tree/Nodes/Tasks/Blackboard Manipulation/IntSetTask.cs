using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Int/Int Set"), GraphNodeName("Int Set", "Int Set")]
    public class IntSetTask : TaskNode
    {
        [SerializeField] private BlackboardIntSetter targetInt;
        [SerializeField] private BlackboardDynamicIntGetter setTo;

        protected override void OnInitialize()
        {
            targetInt.Initialize(agent.BlackboardCollection);
            setTo.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            targetInt.SetValue(setTo.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}