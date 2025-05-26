using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Int/Int Add Or Subtract"), GraphNodeName("Int Add Or Subtract", "Int Add")]
    public class IntAddOrSubtractTask : TaskNode
    {
        [SerializeField] private BlackboardIntSetter targetInt;
        [SerializeField] private BlackboardDynamicIntGetter valueToAdd;

        protected override void OnInitialize()
        {
            targetInt.Initialize(agent.BlackboardCollection);
            valueToAdd.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            targetInt.SetValue(targetInt.Value + valueToAdd.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}