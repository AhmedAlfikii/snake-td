using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Point/Locate/Get Actor Future Position"), GraphNodeName("Get Actor Future Position")]
    public class GetActorFuturePosition : TaskNode
    {
        [SerializeField] private BlackboardActorGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter positionAfter;
        [SerializeField] private BlackboardVector3Setter resultDestination;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
            positionAfter.Initialize(agent.BlackboardCollection);
            resultDestination.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Rigidbody targetRB = target.Value.GetCachedComponent<Rigidbody>();

            Vector3 targetCurrentPosition = target.Value.transform.position;
            
            Vector3 deltaAfterDuration = targetRB.linearVelocity * positionAfter.Value;

            Vector3 targetFuturePosition = targetCurrentPosition + deltaAfterDuration;

            resultDestination.SetValue(targetFuturePosition);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}