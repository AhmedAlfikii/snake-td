using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Debugging/Blackboard Message Log"), GraphNodeName("Blackboard Message Log", "BB Log")]
    public class DebugBlackboardMessageState : ActionState
    {
        [SerializeField] private BlackboardStringGetter debugMessage = new BlackboardStringGetter("Debug");

        protected override void OnInitialize()
        {
            debugMessage.Initialize(agent.BlackboardCollection);
        }
        protected override void OnPlay()
        {
            Debug.Log(debugMessage.Value);

            Complete();
        }
    }
}
