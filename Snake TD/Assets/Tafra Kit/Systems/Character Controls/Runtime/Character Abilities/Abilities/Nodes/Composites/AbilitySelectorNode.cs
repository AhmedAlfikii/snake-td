using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Composites/Selector"), GraphNodeName("Selector", "Selector")]
    public class AbilitySelectorNode : AbilityCompositeNode
    {
        private int targetChildIndex;

        public AbilitySelectorNode(AbilityCompositeNode other) : base(other)
        {

        }
        public AbilitySelectorNode()
        {

        }

        protected override void OnStart()
        {
            targetChildIndex = 0;
        }
        protected override BTNodeState OnUpdate()
        {
            if(children.Count > targetChildIndex)
            {
                BTNode targetChild = children[targetChildIndex];

                BTNodeState childState = targetChild.Update();

                switch(childState)
                {
                    case BTNodeState.Running:
                        return BTNodeState.Running;
                    case BTNodeState.Success:
                        return BTNodeState.Success;
                    case BTNodeState.Failure:
                        {
                            if(targetChildIndex < children.Count - 1)
                            {
                                targetChildIndex++;
                                return BTNodeState.Running;
                            }
                            else
                                return BTNodeState.Failure;
                        }
                }
            }

            return BTNodeState.Failure;
        }
        protected override BTNode CloneContent()
        {
            AbilitySelectorNode clonedNode = new AbilitySelectorNode(this);

            return clonedNode;
        }
    }
}