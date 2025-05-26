using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Animator/Play Animation/Animator Cross Fade"), GraphNodeName("Animator Cross Fade")]
    public class AnimatorCrossFadeStateTask : TaskNode
    {
        [SerializeField] private string stateName;
        [SerializeField] private float normalizedTransitionDuration = 0.2f;
        [SerializeField] private float normalizedTimeOffset = 0;
        [SerializeField] private int layer;

        private AIAnimator aiAnimator;
        private Animator animator;
        private int stateNameHash;

        protected override void OnInitialize()
        {
            aiAnimator = agent.GetCachedComponent<AIAnimator>();
            animator = aiAnimator.Animator;
            stateNameHash = Animator.StringToHash(stateName);
        }
        protected override void OnStart()
        {
            animator.CrossFade(stateNameHash, normalizedTransitionDuration, layer, normalizedTimeOffset);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}