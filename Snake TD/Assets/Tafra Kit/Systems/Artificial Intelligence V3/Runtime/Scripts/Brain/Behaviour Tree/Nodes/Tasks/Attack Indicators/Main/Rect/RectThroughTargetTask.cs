using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Main/Rect/Rect Through Target"), GraphNodeName("Rect Through Target", "Rect Through Target")]
    public class RectThroughTargetTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter width = new BlackboardDynamicFloatGetter(1);
        [SerializeField] private BlackboardDynamicFloatGetter distanceAfterTarget = new BlackboardDynamicFloatGetter(2);
        [Tooltip("The object property in the blackboard that will be used to store the indicator in.")]
        [SerializeField] private BlackboardObjectSetter storageProperty = new BlackboardObjectSetter();

        private RectAttackIndicatorData indicatorData = new RectAttackIndicatorData();
        private RectAttackIndicator attackIndicator;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
            width.Initialize(agent.BlackboardCollection);
            distanceAfterTarget.Initialize(agent.BlackboardCollection);
            storageProperty.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            //Fetch the indicator.

            Vector3 startPosition = agent.transform.position;

            Vector3 targetPosition = target.Value;
            targetPosition.y = startPosition.y;

            Vector3 dir = targetPosition - startPosition;
            indicatorData.SetData(startPosition, dir, width.Value, dir.magnitude + distanceAfterTarget.Value);

            attackIndicator = AttackIndicatorsHandler.Instance.RequestAttackIndicator<RectAttackIndicator>(indicatorData);
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