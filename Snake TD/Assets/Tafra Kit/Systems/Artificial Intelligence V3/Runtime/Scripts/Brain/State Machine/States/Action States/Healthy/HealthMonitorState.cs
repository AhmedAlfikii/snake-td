using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.Healthies;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Healthy/Health Monitor"), GraphNodeName("Health Monitor", "HM")]
    public class HealthMonitorState : ActionState
    {
        [SerializeField] private BlackboardDynamicFloatGetter targetHealthPercentage = new BlackboardDynamicFloatGetter(0.5f);
        [SerializeField] private NumberRelation targetRelation;

        private Healthy healthy;
        private float targetHealthPercentageValue;

        protected override void OnInitialize()
        {
            healthy = agent.GetCachedComponent<Healthy>();
            targetHealthPercentage.Initialize(agent.BlackboardCollection);
        }
        protected override void OnPlay()
        {
            targetHealthPercentageValue = targetHealthPercentage.Value;
        }
        public override void Update()
        {
            if (ZHelper.IsNumberRelationValid(healthy.NormalizedHealth, targetHealthPercentageValue, targetRelation))
                Complete();
        }
    }
}