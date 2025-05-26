    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using Unity.VisualScripting;

namespace TafraKit.Internal.AI3
{
    [Serializable]
    public class InternalStateMachine : IStateMachine
    {
        public string isOriginal = "Original";
        [SerializeReference, RawDisplayInGraphInspector] private List<State> states;
        [SerializeReference, HideInInspector] private AnyState anyState;
        [SerializeReference, HideInInspector] private EntryState entryState;
        [SerializeReference, HideInInspector] private ExitState exitState;

        private Dictionary<string, State> stateById;

        public List<State> States => states;
        public AnyState AnyState => anyState;
        public EntryState EntryState => entryState;
        public ExitState ExitState => exitState;

        public InternalStateMachine()
        {
            states = new List<State>();

            anyState = new AnyState();
            entryState = new EntryState();
            exitState = new ExitState();

            anyState.Position = new Rect(0, -85, 0, 0);
            entryState.Position = new Rect(0, 0, 0, 0);
            exitState.Position = new Rect(355, 0, 0, 0);

            states.Add(anyState);
            states.Add(entryState);
            states.Add(exitState);
        }

        public void Initialize()
        {
            stateById = new Dictionary<string, State>();

            for(int i = 0; i < states.Count; i++)
            {
                State state = states[i];
                stateById.Add(state.GUID, state);
            }
        }

        public State GetState(string stateGUID)
        {
            stateById.TryGetValue(stateGUID, out State state);

            return state;
        }

        public void OnDestroy()
        {
            for (int i = 0; i < states.Count; i++)
            {
                states[i].OnDestroy();
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Since the new transitions (if copied from another behaviour tree) now have the from and to states as basic state reference, we need to get the actual states.
        /// Meaning that a From field will contain a node of type State, but not the actual EntryState for example. So what we need to do is get the corect reference from the staets list.
        /// </summary>
        public void EditorFixChildrenReferences()
        {
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];

                if (state is AnyState anyState)
                    this.anyState = anyState;
                else if (state is EntryState entryState)
                    this.entryState = entryState;
                else if (state is ExitState exitState)
                    this.exitState = exitState;

                EditorFixStateTransitionReferences(state);
            }
        }
        private void EditorFixStateTransitionReferences(State state)
        {
            for (int i = 0; i < state.Transitions.Count; i++)
            {
                var transition = state.Transitions[i];
               
                transition.EditorSetFromState(state);

                bool foundToState = false;
                for (int j = 0; j < states.Count; j++)
                {
                    var mainState = states[j];

                    if (transition.ToState.GUID == mainState.GUID)
                    {
                        transition.EditorSetToState(mainState);
                        foundToState = true;
                        break;
                    }
                }

                if (!foundToState)
                    Debug.LogError($"Couldn't find \"To\" state for the transition {i} of the state {state}.");
            }

            if (state is SubstateMachineState subStateMachineSate)
            {
                subStateMachineSate.EditorFixInternalStateMachineReferences();
            }
            if (state is BehaviourTreeState behaviourTreeState)
            {
                behaviourTreeState.EditorFixInternalTreeChildrenReferences();
            }
        }
        #endif
    }
}
