using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Composites/Selector"), GraphNodeName("Selector", "Selector")]
    public class SelectorNode : CompositeNode
    {
        private int targetChildIndex;

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
    }
}