using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    /// <summary>
    /// This tasks changes the state of a blackboard bool to a certain value on start, and on end, it switches it to the opposite value.
    /// </summary>
    [SearchMenuItem("Tasks/Blackboard Manipulation/Bool/Bool Set (Running)"), GraphNodeName("Bool Set (Running)")]
    public class BoolSetRunningTask : TaskNode
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
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            boolProperty.SetValue(!setTo.Value);
        }
    }
}