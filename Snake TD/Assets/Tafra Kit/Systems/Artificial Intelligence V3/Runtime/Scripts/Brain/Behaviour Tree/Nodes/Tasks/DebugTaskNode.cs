using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Debugging/Debug Log"), GraphNodeName("Debug Log", "Log")]
    public class DebugTaskNode : TaskNode
    {
        [SerializeField, TextArea()] private string debugMessage = "Debug";

        protected override void OnStart()
        {
            Debug.Log(debugMessage);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}