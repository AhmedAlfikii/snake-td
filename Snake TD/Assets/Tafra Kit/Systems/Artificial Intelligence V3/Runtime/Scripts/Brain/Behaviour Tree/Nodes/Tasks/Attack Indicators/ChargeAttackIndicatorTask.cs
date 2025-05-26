using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Attack Indicators/Charge Attack Indicator"), GraphNodeName("Charge Attack Indicator", "Charge")]
    public class ChargeAttackIndicatorTask : TaskNode
    {
        [SerializeField] private BlackboardObjectGetter indicator;
        [SerializeField] private BlackboardDynamicFloatGetter chargeDuration = new BlackboardDynamicFloatGetter(2);


        protected override void OnInitialize()
        {
            indicator.Initialize(agent.BlackboardCollection);
            chargeDuration.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            Object indicatorValue = indicator.Value;

            if (indicatorValue != null)
                ((AttackIndicator)indicatorValue).StartCharging(chargeDuration.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}