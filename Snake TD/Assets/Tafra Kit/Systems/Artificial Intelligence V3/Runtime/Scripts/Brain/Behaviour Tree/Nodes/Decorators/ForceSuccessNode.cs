using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Decorators/Force Success"), GraphNodeName("Force Success", "Force Success")]
    public class ForceSuccessNode : DecoratorNode
    {
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
    }
}