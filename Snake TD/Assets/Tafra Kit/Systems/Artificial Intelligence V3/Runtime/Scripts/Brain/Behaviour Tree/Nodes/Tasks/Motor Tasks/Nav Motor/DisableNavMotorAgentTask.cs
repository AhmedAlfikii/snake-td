using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.AI3.Motor;
using UnityEngine;
using UnityEngine.AI;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Nav Motor/Disable Nav Motor Agent"), GraphNodeName("Disable Nav Motor Agent")]
    public class DisableNavMotorAgentTask : TaskNode
    {
        private AINavMotor navMotor;

        protected override void OnInitialize()
        {
            navMotor = agent.GetCachedComponent<AINavMotor>();
        }
        protected override void OnStart()
        {
            navMotor.NavAgent.enabled = false;
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            navMotor.NavAgent.enabled = true;
        }
    }
}