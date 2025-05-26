using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Debugging/Debug Log"), GraphNodeName("Debug Log", "Debug Log")]
    public class DebugState : ActionState
    {
        [SerializeField, TextArea] private string debugMessage = "Debug";

        protected override void OnPlay()
        {
            Debug.Log(debugMessage);

            Complete();
        }
    }
}
