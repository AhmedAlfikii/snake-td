using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/General/Empty"), GraphNodeName("Empty")]
    public class EmptyTaskNode : AbilityTaskNode
    {
        public EmptyTaskNode(EmptyTaskNode other) : base(other)
        {
        }
        public EmptyTaskNode()
        {

        }

        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            EmptyTaskNode clonedNode = new EmptyTaskNode(this);

            return clonedNode;
        }
    }
}