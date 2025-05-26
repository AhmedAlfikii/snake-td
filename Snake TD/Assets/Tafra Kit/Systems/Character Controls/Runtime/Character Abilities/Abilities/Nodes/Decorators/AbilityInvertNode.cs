using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Decorators/Invert"), GraphNodeName("Invert", "Invert")]
    public class AbilityInvertNode : AbilityDecoratorNode
    {
        public AbilityInvertNode(AbilityInvertNode other) : base(other)
        {

        }
        public AbilityInvertNode()
        {

        }

        protected override BTNodeState OnUpdate()
        {
            if(child != null)
            {
                BTNodeState childState = child.Update();

                switch(childState)
                {
                    case BTNodeState.Running:
                        return BTNodeState.Running;
                    case BTNodeState.Success:
                        return BTNodeState.Failure;
                    case BTNodeState.Failure:
                        return BTNodeState.Success;
                }
            }

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            AbilityInvertNode clonedNode = new AbilityInvertNode(this);

            return clonedNode;
        }
    }
}