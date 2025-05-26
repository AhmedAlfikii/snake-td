using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [System.Serializable]
    public class State : GraphNode
    {
        [SerializeField, RawDisplayInGraphInspector] private List<StateTransition> transitions = new List<StateTransition>();

        public Action OnStateComplete;
        public Action<StateTransition> OnTransitionSatisfied;

        protected AIAgent agent;
        protected bool isPlaying;
        private bool isEntryState;

        public bool IsPlaying => isPlaying;
        public List<StateTransition> Transitions => transitions;

        //The following fields/properties should ONLY be used in editor scripts.
        #region Editor Only Fields
        #if UNITY_EDITOR
        [HideInInspector] public bool LogEditorEvents;
        public Action<State> EditorOnPlay;
        public Action<State> EditorOnConclude;
        #endif
        #endregion

        public void Initialize(AIAgent agent)
        {
            this.agent = agent;
            
            isEntryState = this is EntryState;

            for(int i = 0; i < transitions.Count; i++)
            {
                StateTransition transition = transitions[i];
                transition.Initialize(agent);
            }

            OnInitialize();
        }

        public void Play()
        {
            if(isPlaying)
            {
                TafraDebugger.Log("State", $"State {name} is already playing, no need to play it again.", TafraDebugger.LogType.Verbose);
                return;
            }

            isPlaying = true;

            for(int i = 0; i < transitions.Count; i++)
            {
                StateTransition transition = transitions[i];
                transition.Activate();
            }

            #if UNITY_EDITOR
            if(LogEditorEvents)
                EditorOnPlay?.Invoke(this);
            #endif

            OnPlay();
        }

        #region MonoBehaviour Messages Mimic
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }

        #endregion

        public void EvaluateNonCompletionTransitions() 
        {
            //No need to evaluate entry state transitions, they will be evaluated at the end of the state, which happens the same frame it stars.
            if(isEntryState)
                return;

            for(int i = 0; i < transitions.Count; i++)
            {
                StateTransition transition = transitions[i];
                if(transition.IsEvaluateOnCompleteOnly || transition.HasNoConditions)
                    continue;

                if(transition.EvaluateConditions())
                {
                    OnTransitionSatisfied?.Invoke(transition);
                    break;
                }
            }
        }
        public void EvaluateAllTransitions() 
        {
            for(int i = 0; i < transitions.Count; i++)
            {
                StateTransition transition = transitions[i];

                if(transition.EvaluateConditions())
                {
                    OnTransitionSatisfied?.Invoke(transition);
                    break;
                }
            }
        }

        /// <summary>
        /// Force the state to stop playing. Typically called when a transition is triggered or when the state machine operator is deactivated.
        /// </summary>
        public void Terminate()
        {
            if(!isPlaying)
            {
                TafraDebugger.Log("State", "State isn't playing, no need to terminate it.", TafraDebugger.LogType.Verbose);
                return;
            }

            isPlaying = false;

            #if UNITY_EDITOR
            if(LogEditorEvents)
                EditorOnConclude?.Invoke(this);
            #endif

            OnTerminate();
            OnConclude();

            for(int i = 0; i < transitions.Count; i++)
            {
                StateTransition transition = transitions[i];
                transition.Deactivate();
            }
        }

        /// <summary>
        /// Call this when the state is done and should complete. (optional, states don't need to complete, infinitely play until they're terminated by the state machine)
        /// </summary>
        protected void Complete()
        {
            if(!isPlaying)
            {
                TafraDebugger.Log("State", "State isn't playing, no need to complete it.", TafraDebugger.LogType.Verbose);
                return;
            }

            isPlaying = false;

            #if UNITY_EDITOR
            if(LogEditorEvents)
                EditorOnConclude?.Invoke(this);
            #endif

            OnComplete();
            OnConclude();

            for(int i = 0; i < transitions.Count; i++)
            {
                StateTransition transition = transitions[i];
                transition.Deactivate();
            }

            OnStateComplete?.Invoke();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnPlay() { }
        protected virtual void OnComplete() { }
        protected virtual void OnTerminate() { }
        protected virtual void OnConclude() { }
    }
}
