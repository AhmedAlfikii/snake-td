using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.Internal.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Decorators/Repeat"), GraphNodeName("Repeat"), RawDisplayInGraphInspector()]
    public class RepeatNode : DecoratorNode
    {
        public enum RepeatRules
        { 
            Count,
            UntilSuccess,
            UntilFailure,
            Infinitely
        }
        [SerializeField] private RepeatRules repeatRule = RepeatRules.Infinitely;
        [SerializeField] private BlackboardDynamicIntGetter executionsCount = new BlackboardDynamicIntGetter(1);
        [SerializeField] private bool failIfChildFailed;
        [Tooltip("Each time the node has been updated, repeat conditions will be evaluated to determine whether it should play again or complete.")]
        [SerializeField] private BlackboardConditionsGroup repeatConditions;

        private int curExecutionsCount;
        private int executionsCountValue;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            repeatConditions.SetDependencies(agent, agent.BlackboardCollection);

            executionsCount.Initialize(agent.BlackboardCollection);
        }
        protected override void OnStart()
        {
            base.OnStart();

            repeatConditions.Activate();

            curExecutionsCount = 0;
            executionsCountValue = executionsCount.Value;
        }
        protected override void OnEnd()
        {
            repeatConditions.Deactivate();
        }
        protected override BTNodeState OnUpdate()
        {
            if (child == null || (repeatRule == RepeatRules.Count && executionsCountValue == 0))
                return BTNodeState.Success;

            BTNodeState childState = child.Update();

            if(childState == BTNodeState.Running)
                return BTNodeState.Running;

            switch(repeatRule)
            {
                case RepeatRules.Count:
                    {
                        if (failIfChildFailed && childState == BTNodeState.Failure)
                            return BTNodeState.Failure;

                        curExecutionsCount++;

                        if(curExecutionsCount >= executionsCountValue)
                            return childState;
                        else
                            return BTNodeState.Running;
                    }
                case RepeatRules.UntilSuccess:
                    {
                        if(childState == BTNodeState.Success)
                            return BTNodeState.Success;
                        else
                            return BTNodeState.Running;
                    }
                case RepeatRules.UntilFailure:
                    {
                        if(childState == BTNodeState.Failure)
                            return BTNodeState.Failure;
                        else
                            return BTNodeState.Running;
                    }
                case RepeatRules.Infinitely:
                    return BTNodeState.Running;
            }

            return BTNodeState.Success;
        }
    }
}