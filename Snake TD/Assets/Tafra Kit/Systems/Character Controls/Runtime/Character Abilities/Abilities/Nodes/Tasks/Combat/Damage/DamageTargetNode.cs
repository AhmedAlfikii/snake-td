using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Cinemachine;
using TafraKit.GraphViews;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Combat/Damage/Damage Target"), GraphNodeName("Damage Target")]
    public class DamageTargetNode : AbilityTaskNode
    {
        [SerializeField] private BlackboardAdvancedFloatGetter damage = new BlackboardAdvancedFloatGetter();
        [SerializeField] private BlackboardActorGetter target = new BlackboardActorGetter();

        public DamageTargetNode(DamageTargetNode other) : base(other)
        {
            damage = new BlackboardAdvancedFloatGetter(other.damage);
            target = new BlackboardActorGetter(other.target);
        }
        public DamageTargetNode()
        {

        }

        protected override void OnInitialize()
        {
            damage.Initialize(ability.BlackboardCollection);
            target.Initialize(ability.BlackboardCollection);
        }
        protected override void OnTriggerBlackboardSet()
        {
            damage.SetSecondaryBlackboard(triggerBlackboard);
            target.SetSecondaryBlackboard(triggerBlackboard);
        }
        protected override void OnStart()
        {
            TafraActor targetActor = target.Value;

            if(targetActor != null)
            {
                float damageValue = damage.Value.Value;

                var healthy = targetActor.GetCachedComponent<Healthy>();
                healthy.TakeDamage(new HitInfo(damageValue, actor));
            }
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            DamageTargetNode clonedNode = new DamageTargetNode(this);

            return clonedNode;
        }
    }
}