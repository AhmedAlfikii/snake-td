using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Point/Locate/Get Target Point"), GraphNodeName("Get Target Point")]
    public class GetTargetPoint : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter target;
        [SerializeField] private BlackboardVector3Getter offset;
        [SerializeField] private BlackboardVector3Setter resultDestination;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
            offset.Initialize(agent.BlackboardCollection);
            resultDestination.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Vector3 targetPosition = target.Value;

            Vector3 result = targetPosition + offset.Value;

            resultDestination.SetValue(result);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}