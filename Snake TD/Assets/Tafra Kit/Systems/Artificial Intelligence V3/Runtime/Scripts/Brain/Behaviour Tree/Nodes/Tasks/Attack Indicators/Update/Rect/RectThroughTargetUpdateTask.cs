using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Update/Rect/Rect Through Target Update"), GraphNodeName("Rect Through Target Update")]
    public class RectThroughTargetUpdateTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter distanceAfterTarget = new BlackboardDynamicFloatGetter(2);
        [SerializeField] private BlackboardObjectGetter attackIndicator = new BlackboardObjectGetter();

        private RectAttackIndicator activeAttackIndicator;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
            distanceAfterTarget.Initialize(agent.BlackboardCollection);
            attackIndicator.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            activeAttackIndicator = attackIndicator.Value as RectAttackIndicator;
        }
        protected override BTNodeState OnUpdate()
        {
            Vector3 startPosition = agent.transform.position;

            Vector3 targetPosition = target.Value;
            targetPosition.y = startPosition.y;

            Vector3 dir = targetPosition - startPosition;
            float length = dir.magnitude + distanceAfterTarget.Value;

            activeAttackIndicator.Refresh(startPosition, length);

            return BTNodeState.Running;
        }
    }
}