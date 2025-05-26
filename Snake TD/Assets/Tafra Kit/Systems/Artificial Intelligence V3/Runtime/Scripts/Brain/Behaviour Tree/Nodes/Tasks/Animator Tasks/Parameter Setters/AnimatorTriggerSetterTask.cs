using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Animator/Parameter Setters/Animator Trigger Setter"), GraphNodeName("Animator Trigger Setter", "Animator Trigger")]
    public class AnimatorTriggerSetterTask : TaskNode
    {
        [SerializeField] private BlackboardStringGetter animatorParameterName;
        [Tooltip("Should the trigger be set or reset? true for set, false for reset")]
        [SerializeField] private BlackboardBoolGetter targetState = new BlackboardBoolGetter(true);
     
        private AIAnimator aiAnimator;
        private Animator animator;

        protected override void OnInitialize()
        {
            aiAnimator = agent.GetCachedComponent<AIAnimator>();
            animator = aiAnimator.Animator;

            animatorParameterName.Initialize(agent.BlackboardCollection);
            targetState.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            if (targetState.Value)
                animator.SetTrigger(animatorParameterName.Value);
            else
                animator.ResetTrigger(animatorParameterName.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }
    }
}