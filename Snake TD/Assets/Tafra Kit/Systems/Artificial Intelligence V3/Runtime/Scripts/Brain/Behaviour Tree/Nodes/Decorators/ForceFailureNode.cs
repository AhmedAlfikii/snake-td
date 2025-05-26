using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.AI3;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Decorators/Force Failure"), GraphNodeName("Force Failure", "Force Failure")]
    public class ForceFailureNode : DecoratorNode
    {
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
    }
}