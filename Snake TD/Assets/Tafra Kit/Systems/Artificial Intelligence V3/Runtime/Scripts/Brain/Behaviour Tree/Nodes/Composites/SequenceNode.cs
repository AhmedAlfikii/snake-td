using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Composites/Sequence"), GraphNodeName("Sequence", "Sequence")]
    public class SequenceNode : CompositeNode
    {
        private int targetChildIndex;

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
    }
}