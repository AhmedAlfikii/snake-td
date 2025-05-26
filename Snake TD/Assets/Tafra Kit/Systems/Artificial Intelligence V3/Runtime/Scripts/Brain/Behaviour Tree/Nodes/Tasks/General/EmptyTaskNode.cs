using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/General/Empty"), GraphNodeName("Empty")]
    public class EmptyTaskNode : TaskNode
    {
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}