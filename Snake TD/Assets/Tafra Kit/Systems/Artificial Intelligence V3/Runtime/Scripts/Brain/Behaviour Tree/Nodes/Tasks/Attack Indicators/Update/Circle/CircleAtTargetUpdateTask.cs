using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Update/Circle/Circle At Target Update"), GraphNodeName("Circle At Target Update")]
    public class CircleAtTargetUpdateTask : TaskNode
    {
        [SerializeField] private BlackboardObjectSetter circleIndicator = new BlackboardObjectSetter();
        [SerializeField] private BlackboardDynamicPointGetter targetPosition;

        private CircleAttackIndicator indicator;
        private Transform indicatorTransform;

        protected override void OnInitialize()
        {
            circleIndicator.Initialize(agent.BlackboardCollection);
            targetPosition.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            //Fetch the indicator.

            Object circleIndicatorValue = circleIndicator.Value;

            if(circleIndicatorValue != null)
            {
                indicator = circleIndicatorValue as CircleAttackIndicator;

                if (indicator != null)
                    indicatorTransform = indicator.transform;
                else
                    indicatorTransform = null;
            }
            else
                indicatorTransform = null;
        }
        protected override BTNodeState OnUpdate()
        {
            if(indicatorTransform != null)
            {
                indicatorTransform.position = targetPosition.Value;
                return BTNodeState.Running;
            }
            else
                return BTNodeState.Success;
        }
    }
}