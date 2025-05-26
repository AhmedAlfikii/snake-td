using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Animator/Parameter Setters/Animator Bool Setter"), GraphNodeName("Animator Bool Setter", "Animator Bool Setter")]
    public class AnimatorBoolSetterTask : TaskNode
    {
        [SerializeField] private BlackboardStringGetter animatorParameterName;
        [Tooltip("The state of the bool aniamtor parameter as long as this task is active. Once this task is no longer active, the bool will be reversed")]
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
            animator.SetBool(animatorParameterName.Value, targetState.Value);
        }
        protected override void OnEnd()
        {
            animator.SetBool(animatorParameterName.Value, !targetState.Value);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
    }
}