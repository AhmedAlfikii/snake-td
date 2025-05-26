using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Main/Circle/Circle At Target"), GraphNodeName("Circle At Target", "Circle At Target")]
    public class CircleAtTargetTask : TaskNode
    {
        [SerializeField] private BlackboardDynamicPointGetter target;
        [SerializeField] private BlackboardDynamicFloatGetter diameter = new BlackboardDynamicFloatGetter(3);
        [Tooltip("The object property in the blackboard that will be used to store the indicator in.")]
        [SerializeField] private BlackboardObjectSetter storageProperty = new BlackboardObjectSetter();

        private CircleAttackIndicatorData indicatorData = new CircleAttackIndicatorData();
        private CircleAttackIndicator attackIndicator;

        protected override void OnInitialize()
        {
            target.Initialize(agent.BlackboardCollection);
            diameter.Initialize(agent.BlackboardCollection);
            storageProperty.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            //Fetch the indicator.

            Vector3 targetPosition = target.Value;

            indicatorData.SetData(targetPosition, diameter.Value / 2f);

            attackIndicator = AttackIndicatorsHandler.Instance.RequestAttackIndicator<CircleAttackIndicator>(indicatorData);
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