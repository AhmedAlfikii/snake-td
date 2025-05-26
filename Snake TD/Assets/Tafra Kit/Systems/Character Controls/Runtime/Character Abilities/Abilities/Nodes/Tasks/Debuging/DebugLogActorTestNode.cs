using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Debugging/Debug Log Actor"), GraphNodeName("Debug Log Actor")]
    public class DebugLogActorTestNode : AbilityTaskNode
    {
        [SerializeField] private BlackboardActorGetter targetActor;

        public DebugLogActorTestNode(DebugLogActorTestNode other) : base(other)
        {
            targetActor = other.targetActor;
        }
        public DebugLogActorTestNode()
        {

        }

        protected override void OnInitialize()
        {
            targetActor.Initialize(ability.BlackboardCollection);
        }
        protected override void OnTriggerBlackboardSet()
        {
            targetActor.SetSecondaryBlackboard(triggerBlackboard);
        }

        protected override BTNodeState OnUpdate()
        {
            Debug.Log(targetActor.Value, targetActor.Value);

            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            DebugLogActorTestNode clonedNode = new DebugLogActorTestNode(this);

            return clonedNode;
        }
    }
}