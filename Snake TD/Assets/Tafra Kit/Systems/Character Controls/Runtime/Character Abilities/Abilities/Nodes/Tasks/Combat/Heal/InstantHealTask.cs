using TafraKit.GraphViews;
using TafraKit.Healthies;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Combat/Heal/Instant Heal"), GraphNodeName("Instant Heal")]
    public class InstantHealTask : AbilityTaskNode
    {
        [SerializeField] private BlackboardAdvancedFloatGetter healAmount = new BlackboardAdvancedFloatGetter();
        [SerializeField] private bool targetIsThisActor;
        [SerializeField] private BlackboardActorGetter targetActor = new BlackboardActorGetter();

        public InstantHealTask(InstantHealTask other) : base(other)
        {
            healAmount = new BlackboardAdvancedFloatGetter(other.healAmount);
            targetIsThisActor = other.targetIsThisActor;
            targetActor = new BlackboardActorGetter(other.targetActor);
        }
        public InstantHealTask()
        {

        }

        protected override void OnInitialize()
        {
            healAmount.Initialize(ability.BlackboardCollection);
            targetActor.Initialize(ability.BlackboardCollection);
        }
        protected override void OnTriggerBlackboardSet()
        {
            healAmount.SetSecondaryBlackboard(triggerBlackboard);
            targetActor.SetSecondaryBlackboard(triggerBlackboard);
        }

        protected override BTNodeState OnUpdate()
        {
            TafraActor actorValue = targetIsThisActor ? actor : targetActor.Value;

            if (actorValue != null)
            {
                Healthy healthy = actorValue.GetCachedComponent<Healthy>();

                if(healthy != null)
                    healthy.Heal(healAmount.Value.Value);
            }

            return BTNodeState.Success;
        }

        protected override BTNode CloneContent()
        {
            InstantHealTask clonedNode = new InstantHealTask(this);

            return clonedNode;
        }
    }
}