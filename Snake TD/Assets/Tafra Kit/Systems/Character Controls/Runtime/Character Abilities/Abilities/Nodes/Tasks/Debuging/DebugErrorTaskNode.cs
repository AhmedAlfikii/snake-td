using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Debugging/Debug Error"), GraphNodeName("Debug Error")]
    public class DebugErrorTaskNode : AbilityTaskNode
    {
        [SerializeField] private string message;

        public DebugErrorTaskNode(DebugErrorTaskNode other) : base(other)
        {
            message = other.message;
        }
        public DebugErrorTaskNode()
        {

        }

        protected override BTNodeState OnUpdate()
        {
            Debug.LogError(message);

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            DebugErrorTaskNode clonedNode = new DebugErrorTaskNode(this);

            return clonedNode;
        }
    }
}