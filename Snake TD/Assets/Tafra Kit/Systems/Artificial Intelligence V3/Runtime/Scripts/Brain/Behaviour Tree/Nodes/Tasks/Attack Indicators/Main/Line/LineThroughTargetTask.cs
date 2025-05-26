using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Healthies;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Main/Line/Line Through Target"), GraphNodeName("Line Through Target", "Line Indicator")]
    public class LineThroughTargetTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter startPoint;
        [SerializeField] private BlackboardDynamicPointGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter width = new BlackboardDynamicFloatGetter(0.25f);
        [SerializeField] private BlackboardDynamicFloatGetter extraLength = new BlackboardDynamicFloatGetter(2);
        [Tooltip("The object property in the blackboard that will be used to store the indicator in.")]
        [SerializeField] private BlackboardObjectSetter storageProperty = new BlackboardObjectSetter();

        private LineAttackIndicatorData indicatorData = new LineAttackIndicatorData();
        private LineAttackIndicator attackIndicator;

        protected override void OnInitialize()
        {
            startPoint.Initialize(agent.BlackboardCollection);
            target.Initialize(agent.BlackboardCollection);
            width.Initialize(agent.BlackboardCollection);
            extraLength.Initialize(agent.BlackboardCollection);
            storageProperty.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            //Fetch the indicator.

            Vector3 startPosition = startPoint.Value;

            Vector3 targetPosition = target.Value;

            float extraLengthValue = extraLength.Value;
            if(extraLengthValue > 0.001f)
            {
                Vector3 dir = targetPosition - startPosition;
                targetPosition += dir.normalized * extraLengthValue;
            }

            indicatorData.SetData(startPosition, targetPosition, width.Value);

            attackIndicator = AttackIndicatorsHandler.Instance.RequestAttackIndicator<LineAttackIndicator>(indicatorData);
            storageProperty.SetValue(attackIndicator);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            //Release the indicator.

            if(attackIndicator != null)
            {
                storageProperty.SetValue(null);
                AttackIndicatorsHandler.Instance.ReleaseAttackIndicator(attackIndicator);
            }
        }
    }
}