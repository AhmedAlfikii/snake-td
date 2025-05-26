using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using UnityEngine.AI;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Point/Manipulate/Sample On NavMesh"), GraphNodeName("Sample On NavMesh")]
    public class SamplePointOnNavMeshTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter point;
        [SerializeField] private BlackboardFloatGetter maxSearchDistance = new BlackboardFloatGetter(5);
        [SerializeField] private BlackboardVector3Setter resultDestination;
        [SerializeField] private BlackboardDynamicIntGetter areaMask = new BlackboardDynamicIntGetter(1);

        protected override void OnInitialize()
        {
            point.Initialize(agent.BlackboardCollection);
            maxSearchDistance.Initialize(agent.BlackboardCollection);
            resultDestination.Initialize(agent.BlackboardCollection);
            areaMask.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Vector3 pointValue = point.Value;

            if(NavMesh.SamplePosition(pointValue, out NavMeshHit hit, maxSearchDistance.Value, areaMask.Value))
                pointValue = hit.position;

            resultDestination.SetValue(pointValue);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}