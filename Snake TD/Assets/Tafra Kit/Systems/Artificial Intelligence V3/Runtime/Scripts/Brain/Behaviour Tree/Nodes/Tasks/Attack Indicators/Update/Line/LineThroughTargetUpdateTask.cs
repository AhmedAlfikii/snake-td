using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Healthies;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Update/Line/Line Through Target Update"), GraphNodeName("Line Through Target Update", "Line Update")]
    public class LineThroughTargetUpdateTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter startPoint;
        [SerializeField] private BlackboardDynamicPointGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter extraLength = new BlackboardDynamicFloatGetter(2);
        [SerializeField] private BlackboardObjectGetter attackIndicator = new BlackboardObjectGetter();

        private LineAttackIndicator activeAttackIndicator;

        protected override void OnInitialize()
        {
            startPoint.Initialize(agent.BlackboardCollection);
            target.Initialize(agent.BlackboardCollection);
            extraLength.Initialize(agent.BlackboardCollection);
            attackIndicator.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            activeAttackIndicator = attackIndicator.Value as LineAttackIndicator;
        }
        protected override BTNodeState OnUpdate()
        {
            if(activeAttackIndicator == null)
            {
                activeAttackIndicator = attackIndicator.Value as LineAttackIndicator;

                if(activeAttackIndicator == null)
                    return BTNodeState.Running;
            }

            Vector3 startPosition = startPoint.Value;

            Vector3 targetPosition = target.Value;

            float extraLengthValue = extraLength.Value;
            if(extraLengthValue > 0.001f)
            {
                Vector3 dir = targetPosition - startPosition;
                targetPosition += dir.normalized * extraLengthValue;
            }

            activeAttackIndicator.SetStartPosition(startPosition);
            activeAttackIndicator.SetEndPosition(targetPosition);

            return BTNodeState.Running;
        }
    }
}