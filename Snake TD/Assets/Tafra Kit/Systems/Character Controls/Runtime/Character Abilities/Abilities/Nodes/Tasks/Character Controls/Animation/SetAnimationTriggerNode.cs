using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Animation/Set Trigger"), GraphNodeName("Set Animation Trigger")]
    public class SetAnimationTriggerNode : AbilityTaskNode
    {
        [SerializeField] private string triggerName;

        private CharacterAnimator characterAnimator;
        private Animator animator;
        private int triggerHashName;

        public SetAnimationTriggerNode(SetAnimationTriggerNode other) : base(other)
        {
            triggerName = other.triggerName;
            characterAnimator = other.characterAnimator;
            animator = other.animator;
            triggerHashName = other.triggerHashName;
        }
        public SetAnimationTriggerNode()
        {

        }

        protected override void OnInitialize()
        {
            CharacterAnimator characterAnimator = actor.GetCachedComponent<CharacterAnimator>();

            if (characterAnimator != null)
            {
                animator = characterAnimator.Animator;
                triggerHashName = Animator.StringToHash(triggerName);
            }
        }
        protected override void OnStart()
        {
            if (animator == null)
                return;

            animator.SetTrigger(triggerHashName);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            SetAnimationTriggerNode clonedNode = new SetAnimationTriggerNode(this);

            return clonedNode;
        }
    }
}