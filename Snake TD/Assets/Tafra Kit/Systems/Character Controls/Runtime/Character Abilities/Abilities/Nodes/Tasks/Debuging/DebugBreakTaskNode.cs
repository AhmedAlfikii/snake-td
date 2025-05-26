using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Debugging/Break"), GraphNodeName("Break")]
    public class DebugBreakTaskNode : AbilityTaskNode
    {
        [SerializeField] private string message;

        public DebugBreakTaskNode(DebugBreakTaskNode other) : base(other)
        {
            message = other.message;
        }
        public DebugBreakTaskNode()
        {

        }

        protected override BTNodeState OnUpdate()
        {
            Debug.Break();

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            DebugBreakTaskNode clonedNode = new DebugBreakTaskNode(this);

            return clonedNode;
        }
    }
}