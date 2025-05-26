using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Float/Float Add Or Subtract"), GraphNodeName("Float Add Or Subtract")]
    public class FloatAddOrSubtractTask : TaskNode
    {
        [SerializeField] private BlackboardFloatSetter targetFloat;
        [SerializeField] private BlackboardDynamicFloatGetter valueToAdd;

        protected override void OnInitialize()
        {
            targetFloat.Initialize(agent.BlackboardCollection);
            valueToAdd.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            targetFloat.SetValue(targetFloat.Value + valueToAdd.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}