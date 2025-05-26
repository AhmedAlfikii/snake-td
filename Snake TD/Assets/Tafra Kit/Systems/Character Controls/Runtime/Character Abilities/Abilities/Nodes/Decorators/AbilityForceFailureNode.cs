using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Decorators/Force Failure"), GraphNodeName("Force Failure", "Force Failure")]
    public class AbilityForceFailureNode : AbilityDecoratorNode
    {
        public AbilityForceFailureNode(AbilityForceFailureNode other) : base(other)
        {

        }
        public AbilityForceFailureNode()
        {

        }

        protected override BTNodeState OnUpdate()
        {
            if(child != null)
            {
                BTNodeState childState = child.Update();

                if(childState == BTNodeState.Running)
                    return BTNodeState.Running;
                else
                    return BTNodeState.Failure;
            }

            return BTNodeState.Failure;
        }

        protected override BTNode CloneContent()
        {
            AbilityForceFailureNode clonedNode = new AbilityForceFailureNode(this);

            return clonedNode;
        }
    }
}