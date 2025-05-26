using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Composites/Sequence"), GraphNodeName("Sequence", "Sequence")]
    public class AbilitySequenceNode : AbilityCompositeNode
    {
        private int targetChildIndex;

        public AbilitySequenceNode(AbilitySequenceNode other) : base(other)
        {

        }
        public AbilitySequenceNode()
        {

        }

        protected override void OnStart()
        {
            targetChildIndex = 0;
        }
        protected override BTNodeState OnUpdate()
        {
            if (children.Count > targetChildIndex)
            {
                BTNode targetChild = children[targetChildIndex];
                BTNodeState childState = targetChild.Update();

                switch(childState)
                {
                    case BTNodeState.Running:
                        return BTNodeState.Running;
                    case BTNodeState.Success:
                        {
                            if(targetChildIndex < children.Count - 1)
                            {
                                targetChildIndex++;
                                return BTNodeState.Running;
                            }
                            else
                                return BTNodeState.Success;
                        }
                    case BTNodeState.Failure:
                        return BTNodeState.Failure;
                }
            }

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            AbilitySequenceNode clonedNode = new AbilitySequenceNode(this);

            return clonedNode;
        }
    }
}