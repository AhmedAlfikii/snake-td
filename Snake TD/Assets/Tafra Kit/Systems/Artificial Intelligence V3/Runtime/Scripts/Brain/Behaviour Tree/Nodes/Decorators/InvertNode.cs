using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Decorators/Invert"), GraphNodeName("Invert", "Invert")]
    public class InvertNode : DecoratorNode
    {
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
    }
}