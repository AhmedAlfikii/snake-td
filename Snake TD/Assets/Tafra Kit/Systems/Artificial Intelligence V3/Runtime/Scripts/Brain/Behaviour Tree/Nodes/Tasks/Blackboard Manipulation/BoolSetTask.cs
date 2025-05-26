using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Bool/Bool Set"), GraphNodeName("Bool Set")]
    public class BoolSetTask : TaskNode
    {
        [SerializeField] private BlackboardBoolSetter boolProperty;
        [SerializeField] private BlackboardBoolGetter setTo;

        protected override void OnInitialize()
        {
            boolProperty.Initialize(agent.BlackboardCollection);
            setTo.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            boolProperty.SetValue(setTo.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}