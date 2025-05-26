using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Animation/Is Animation State Playing"), GraphNodeName("Is Animation State Playing")]
    public class IsAnimationStatePlayingNode : AbilityTaskNode
    {
        [SerializeField] private string stateName;
        [SerializeField] private int layer = 0;
        [SerializeField] private bool mustNotBeInTransition = true;

        private CharacterAnimator characterAnimator;
        private Animator animator;
        private int stateHashName;

        public IsAnimationStatePlayingNode(IsAnimationStatePlayingNode other) : base(other)
        {
            stateName = other.stateName;
            layer = other.layer;
            mustNotBeInTransition = other.mustNotBeInTransition;
            characterAnimator = other.characterAnimator;
            animator = other.animator;
            stateHashName = other.stateHashName;
        }
        public IsAnimationStatePlayingNode()
        {

        }

        protected override void OnInitialize()
        {
            characterAnimator  = actor.GetCachedComponent<CharacterAnimator>();

            if (characterAnimator != null)
            {
                animator = characterAnimator.Animator;
                stateHashName = Animator.StringToHash(stateName);
            }
        }

        protected override BTNodeState OnUpdate()
        {
            if (animator == null)
                return BTNodeState.Success;

            animator.Update(Time.deltaTime);

            if(animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHashName && (!mustNotBeInTransition || !animator.IsInTransition(layer)))
                return BTNodeState.Success;
            else
                return BTNodeState.Failure;
        }
        protected override BTNode CloneContent()
        {
            IsAnimationStatePlayingNode clonedNode = new IsAnimationStatePlayingNode(this);

            return clonedNode;
        }
    }
}