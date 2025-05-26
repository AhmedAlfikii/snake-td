using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.Internal.GraphViews;

namespace TafraKit.Internal.AI3
{
    [System.Serializable]
    public class StateTransition
    {
        [SerializeReference] private State fromState;
        [SerializeReference] private State toState;
        [Tooltip("Should this transition be evaluated only when the state is completed?")]
        [SerializeField] private bool evaluateOnCompleteOnly;
        [SerializeField] private BlackboardConditionsGroup conditionsGroup;

        public State FromState => fromState;
        public State ToState => toState;
        public bool IsEvaluateOnCompleteOnly => evaluateOnCompleteOnly;
        public bool HasNoConditions => conditionsGroup.Conditions.Count == 0;

        public StateTransition()
        { 

        }
        public StateTransition(State fromState, State toState) 
        {
            this.fromState = fromState;
            this.toState = toState;
        }

        public void Initialize(AIAgent agent)
        {
            conditionsGroup.SetDependencies(agent, agent.BlackboardCollection);
        }
        public void Activate()
        {
            conditionsGroup.Activate();
        }
        public void Deactivate()
        {
            conditionsGroup.Deactivate();
        }
        public bool EvaluateConditions()
        {
            return conditionsGroup.Check();
        }

        public void EditorSetFromState(State state)
        {
            fromState = state;
        }
        public void EditorSetToState(State state)
        { 
            toState = state;
        }
    }
}
