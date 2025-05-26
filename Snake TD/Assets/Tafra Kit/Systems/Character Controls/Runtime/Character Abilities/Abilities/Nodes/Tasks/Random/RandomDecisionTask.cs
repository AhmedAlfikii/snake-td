using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;
using System;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Random/Random Decision"), GraphNodeName("Random Decision")]
    public class RandomDecisionTask : AbilityTaskNode
    {
        [SerializeField] private BlackboardAdvancedFloatGetter probability;
        [Tooltip("If higher than 0, then the time this task is triggered X amounts after the last time it was successful, it will be a guaranteed success" +
            " (e.g, if this value is 3, then the third time this task is triggered after the last time it was successful, it will be a guaranteed success.")]
        [SerializeField] private int guaranteeEvery;

        [NonSerialized] private int failedTries;

        public RandomDecisionTask(RandomDecisionTask other) : base(other)
        {
            probability = new BlackboardAdvancedFloatGetter(other.probability);
            guaranteeEvery = other.guaranteeEvery;
        }
        public RandomDecisionTask()
        {

        }

        protected override void OnInitialize()
        {
            probability.Initialize(ability.BlackboardCollection);
        }
        protected override void OnTriggerBlackboardSet()
        {
            probability.SetSecondaryBlackboard(triggerBlackboard);
        }
        protected override BTNodeState OnUpdate()
        {
            Debug.Log(guaranteeEvery + $" {failedTries} >= {guaranteeEvery - 1} ? {failedTries >= guaranteeEvery - 1}");
            if(guaranteeEvery > 0 && failedTries >= guaranteeEvery - 1)
            {
                failedTries = 0;
                return BTNodeState.Success;
            }

            float value = probability.Value.Value;

            if(value <= 0 || UnityEngine.Random.value > value)
            {
                failedTries++;
                return BTNodeState.Failure;
            }
            else
            {
                failedTries = 0;
                return BTNodeState.Success;
            }
        }
        protected override BTNode CloneContent()
        {
            RandomDecisionTask clonedNode = new RandomDecisionTask(this);

            return clonedNode;
        }
    }
}