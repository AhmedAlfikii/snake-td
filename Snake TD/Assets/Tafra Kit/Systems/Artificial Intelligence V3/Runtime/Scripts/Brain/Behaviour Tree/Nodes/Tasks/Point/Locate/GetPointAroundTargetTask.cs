using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Point/Locate/Get Point Around Target"), GraphNodeName("Get Point Around Target")]
    public class GetPointAroundTargetTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter target;
        [Tooltip("The speed of which the agent will orbit it's target. 0 means use the agent's default speed.")]
        [SerializeField] private BlackboardDynamicFloatGetter orbitRadius = new BlackboardDynamicFloatGetter(6);
        [SerializeField] private BlackboardDynamicFloatGetter orbitAngle = new BlackboardDynamicFloatGetter(30);
        [SerializeField] private BlackboardVector3Setter resultDestination;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
            orbitRadius.Initialize(agent.BlackboardCollection);
            orbitAngle.Initialize(agent.BlackboardCollection);
            resultDestination.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Vector3 targePosition = target.Value;

            Vector3 targetToMe = (agent.transform.position - targePosition);
            targetToMe.Normalize();

            Vector3 rotatedDir = (Quaternion.AngleAxis(orbitAngle.Value, Vector3.up)) * targetToMe;

            Vector3 result = targePosition + rotatedDir * orbitRadius.Value;

            resultDestination.SetValue(result);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}