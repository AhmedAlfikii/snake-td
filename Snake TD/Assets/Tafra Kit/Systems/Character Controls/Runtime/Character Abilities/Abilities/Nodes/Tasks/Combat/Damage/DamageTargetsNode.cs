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
    [SearchMenuItem("Tasks/Combat/Damage/Damage Targets"), GraphNodeName("Damage Targets")]
    public class DamageTargetsNode : AbilityTaskNode
    {
        [SerializeField] private BlackboardAdvancedFloatGetter damage = new BlackboardAdvancedFloatGetter();
        [SerializeField] private BlackboardSystemObjectGetter targets = new BlackboardSystemObjectGetter();

        public DamageTargetsNode(DamageTargetsNode other) : base(other)
        {
            damage = new BlackboardAdvancedFloatGetter(other.damage);
            targets = new BlackboardSystemObjectGetter(other.targets);
        }
        public DamageTargetsNode()
        {

        }

        protected override void OnInitialize()
        {
            damage.Initialize(ability.BlackboardCollection);
            targets.Initialize(ability.BlackboardCollection);
        }
        protected override void OnTriggerBlackboardSet()
        {
            damage.SetSecondaryBlackboard(triggerBlackboard);
            targets.SetSecondaryBlackboard(triggerBlackboard);
        }
        protected override void OnStart()
        {
            if(targets.Value != null)
            {
                float damageValue = damage.Value.Value;
                Type listType = targets.Value.GetType().GenericTypeArguments[0];

                if(listType == typeof(Healthy))
                { 
                    List<Healthy> healthyList = targets.Value as List<Healthy>;

                    for (int i = 0; i < healthyList.Count; i++)
                    {
                        var healthy = healthyList[i];
                        healthy.TakeDamage(new HitInfo(damageValue, actor));
                    }
                }
                else if(typeof(TafraActor).IsAssignableFrom(listType))
                {
                    IList actorList = targets.Value as IList;

                    for(int i = 0; i < actorList.Count; i++)
                    {
                        TafraActor adjacentActor = actorList[i] as TafraActor;

                        var healthy = adjacentActor.GetCachedComponent<Healthy>();
                        healthy.TakeDamage(new HitInfo(damageValue, actor));
                    }
                }
            }
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            DamageTargetsNode clonedNode = new DamageTargetsNode(this);

            return clonedNode;
        }
    }
}