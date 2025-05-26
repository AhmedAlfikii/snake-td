using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Animation/Play Animation State (Running)"), GraphNodeName("Play Animation State (Running)")]
    public class PlayAnimationStateRunningNode : AbilityTaskNode
    {
        [Header("While Running")]
        [SerializeField] private string stateName;
        [SerializeField] private int layer = 0;
        [SerializeField] private float normalizedTransitionDuration = 0.25f;

        [Header("On End")]
        [SerializeField] private bool playEndState;
        [SerializeField] private string endStateName;
        [SerializeField] private float endStatNormalizedTransitionDuration = 0.25f;
        [SerializeField] private bool onlyPlayEndStateIfMainStateIsRunning = true;

        private CharacterAnimator characterAnimator;
        private Animator animator;
        private int stateHashName;
        private int endStateHashName;

        public PlayAnimationStateRunningNode(PlayAnimationStateRunningNode other) : base(other)
        {
            stateName = other.stateName;
            layer = other.layer;
            normalizedTransitionDuration = other.normalizedTransitionDuration;
            playEndState = other.playEndState;
            endStateName = other.endStateName;
            endStatNormalizedTransitionDuration = other.endStatNormalizedTransitionDuration;
            onlyPlayEndStateIfMainStateIsRunning = other.onlyPlayEndStateIfMainStateIsRunning;
            characterAnimator = other.characterAnimator;
            animator = other.animator;
            stateHashName = other.stateHashName;
            endStateHashName = other.endStateHashName;
        }
        public PlayAnimationStateRunningNode()
        {

        }

        protected override void OnInitialize()
        {
            characterAnimator  = actor.GetCachedComponent<CharacterAnimator>();

            if (characterAnimator != null)
            {
                animator = characterAnimator.Animator;
                stateHashName = Animator.StringToHash(stateName);
                endStateHashName = Animator.StringToHash(endStateName);
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
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if(animator == null)
                return;

            if(playEndState)
            {
                bool shouldPlay = true;

                if(onlyPlayEndStateIfMainStateIsRunning)
                {
                    //Update the animator so that it instantly knows that this state is the current one (in case it was instantly ended and we wanted to paly the end animation).
                    animator.Update(Time.deltaTime);
                    shouldPlay = animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHashName && !animator.IsInTransition(layer);

                    //if (animator.IsInTransition(layer)) 
                    //{
                    //    AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layer);

                    //    //TODO: handle this in a better way. This doesn't necessarily mean that it's transitioning to the start state.
                    //    //What I want to do is only play the end animation if the start animation is already playing or if there's an animation transitioning to the start animation.
                    //    if(transition.anyState)
                    //        shouldPlay = true;
                    //    //Debug.Log($"({transition.anyState}, {stateHashName})Transitioning to start animation? {transition.nameHash.ToString().EndsWith(stateHashName.ToString())}");
                    //}
                }

                if(shouldPlay)
                    animator.CrossFade(endStateHashName, endStatNormalizedTransitionDuration, layer);
            }
        }
        protected override BTNode CloneContent()
        {
            PlayAnimationStateRunningNode clonedNode = new PlayAnimationStateRunningNode(this);

            return clonedNode;
        }
    }
}