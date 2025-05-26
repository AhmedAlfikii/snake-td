using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.Healthies;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    /// <summary>
    /// Sends damage to the specified target.
    /// </summary>
    [SearchMenuItem("Tasks/Damage/Damage Target"), GraphNodeName("Damage Target", "Damage Target")]
    public class DamageTargetTask : TaskNode
    {
        [SerializeField] private bool agentIsTarget;
        [SerializeField] private BlackboardActorGetter target = new BlackboardActorGetter();
        [SerializeField] private BlackboardAdvancedFloatGetter damage = new BlackboardAdvancedFloatGetter(new TafraAdvancedFloat(10));

        protected override void OnInitialize()
        {
            damage.Initialize(agent.BlackboardCollection);
            target.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            TafraActor targetActor = agentIsTarget ? agent : target.Value;
            if (targetActor != null)
            {
                Healthy targetHealthy = targetActor.GetCachedComponent<Healthy>();
                targetHealthy.TakeDamage(new HitInfo(damage.Value.Value, agent));
            }
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}