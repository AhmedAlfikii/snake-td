using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Debugging/Debug Log"), GraphNodeName("Debug Log")]
    public class DebugLogTaskNode : AbilityTaskNode
    {
        [SerializeField] private string message;

        public DebugLogTaskNode(DebugLogTaskNode other) : base(other)
        {
            message = other.message;
        }
        public DebugLogTaskNode()
        {

        }

        protected override BTNodeState OnUpdate()
        {
            Debug.Log(message);

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            DebugLogTaskNode clonedNode = new DebugLogTaskNode(this);

            return clonedNode;
        }
    }
}