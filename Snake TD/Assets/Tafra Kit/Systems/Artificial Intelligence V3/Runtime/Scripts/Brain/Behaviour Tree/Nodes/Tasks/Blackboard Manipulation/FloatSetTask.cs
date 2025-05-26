using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Float/Float Set"), GraphNodeName("Float Set")]
    public class FloatSetTask : TaskNode
    {
        [SerializeField] private BlackboardFloatSetter targetFloat;
        [SerializeField] private BlackboardDynamicFloatGetter setTo;

        protected override void OnInitialize()
        {
            targetFloat.Initialize(agent.BlackboardCollection);
            setTo.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            targetFloat.SetValue(setTo.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}