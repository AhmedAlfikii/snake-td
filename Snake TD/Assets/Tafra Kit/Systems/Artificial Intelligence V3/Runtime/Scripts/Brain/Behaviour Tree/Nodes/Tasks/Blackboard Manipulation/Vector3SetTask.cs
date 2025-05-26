using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Vector3/Vector3 Set"), GraphNodeName("Vector3 Set", "Vector3 Set")]
    public class Vector3SetTask : TaskNode
    {
        [SerializeField] private BlackboardVector3Setter vector3Property;
        [SerializeField] private BlackboardDynamicPointGetter setTo;

        protected override void OnInitialize()
        {
            setTo.Initialize(agent.BlackboardCollection);
            vector3Property.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            vector3Property.SetValue(setTo.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}