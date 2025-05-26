using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.AI3.Motor;
using UnityEngine;
using UnityEngine.AI;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Motor/Nav Motor/Set Nav Motor Position"), GraphNodeName("Set Nav Position", "Set Nav Position")]
    public class SetNavMotorPositionTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter targetPosition;
        [NavMeshAreaMask()]
        [SerializeField] private int navMeshArea = 0;

        private AINavMotor navMotor;

        protected override void OnInitialize()
        {
            navMotor = agent.GetCachedComponent<AINavMotor>();

            targetPosition.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Vector3 positionValue = targetPosition.Value;

            if (NavMesh.SamplePosition(positionValue, out NavMeshHit hit, 5, navMeshArea))
                navMotor.transform.position = hit.position;
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}