using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Decorators/Force Success"), GraphNodeName("Force Success", "Force Success")]
    public class AbilityForceSuccessNode : AbilityDecoratorNode
    {
        public AbilityForceSuccessNode(AbilityForceSuccessNode other) : base(other)
        {

        }
        public AbilityForceSuccessNode()
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
                    return BTNodeState.Success;
            }

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            AbilityForceSuccessNode clonedNode = new AbilityForceSuccessNode(this);

            return clonedNode;
        }
    }
}