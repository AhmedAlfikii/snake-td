using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Animation/Play Animation State"), GraphNodeName("Play Animation State")]
    public class PlayAnimationStateNode : AbilityTaskNode
    {
        [SerializeField] private string stateName;
        [SerializeField] private int layer = 0;
        [SerializeField] private float normalizedTransitionDuration = 0.25f;

        private CharacterAnimator characterAnimator;
        private Animator animator;
        private int stateHashName;

        public PlayAnimationStateNode(PlayAnimationStateNode other) : base(other)
        {
            stateName = other.stateName;
            layer = other.layer;
            normalizedTransitionDuration = other.normalizedTransitionDuration;
            characterAnimator = other.characterAnimator;
            animator = other.animator;
            stateHashName = other.stateHashName;
        }
        public PlayAnimationStateNode()
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
        protected override void OnStart()
        {
            if (animator == null)
                return;

            animator.CrossFade(stateHashName, normalizedTransitionDuration, layer);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
        protected override BTNode CloneContent()
        {
            PlayAnimationStateNode clonedNode = new PlayAnimationStateNode(this);

            return clonedNode;
        }
    }
}