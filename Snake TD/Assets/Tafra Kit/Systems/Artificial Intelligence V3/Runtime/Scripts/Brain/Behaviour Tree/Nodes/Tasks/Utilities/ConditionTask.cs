using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.Internal.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Utilities/Condition"), GraphNodeName("Condition", "Condition")]
    public class ConditionTask : TaskNode
    {
        [SerializeField] private BlackboardConditionsGroup conditions;

        protected override void OnInitialize()
        {
            conditions.SetDependencies(agent, agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            conditions.Activate();
        }
        protected override void OnEnd()
        {
            conditions.Deactivate();
        }
        protected override BTNodeState OnUpdate()
        {
            if (conditions.Check())
                return BTNodeState.Success;
            else
                return BTNodeState.Failure;
        }
    }
}