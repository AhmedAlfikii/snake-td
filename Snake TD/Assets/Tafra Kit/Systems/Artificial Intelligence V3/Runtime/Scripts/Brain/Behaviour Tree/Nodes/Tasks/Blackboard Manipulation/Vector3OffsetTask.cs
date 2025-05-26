using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Blackboard Manipulation/Vector3/Vector3 Offset"), GraphNodeName("Vector3 Set", "Vector3 Offset")]
    public class Vector3OffsetTask : TaskNode
    {
        [SerializeField] private BlackboardVector3Getter vector3Property;
        [SerializeField] private BlackboardVector3Getter offset;
        [SerializeField] private BlackboardVector3Setter resultProperty;

        protected override void OnInitialize()
        {
            offset.Initialize(agent.BlackboardCollection);
            vector3Property.Initialize(agent.BlackboardCollection);
            resultProperty.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            resultProperty.SetValue(vector3Property.Value + offset.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}