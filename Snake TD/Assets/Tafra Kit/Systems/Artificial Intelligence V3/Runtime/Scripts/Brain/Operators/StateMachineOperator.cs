using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;

namespace TafraKit.Internal.AI3
{
    public class StateMachineOperator : PlayableOperator
    {
        public Action<StateMachineOperator> OnComplete;
        
        private AIAgent agent;
        private IStateMachine stateMachine;
        private State activeState;
        private int lastTransitionsEvaluatedFrame;

        //I dropped this back to 1 instead of 2 because there's an issue that needs to be solved if it's more than 1:
        //Nested state machines become desynchronized when it comes to transition evaluation...
        //so if a sub-state machine state in a parent state machine has a transition with certain conditions...
        //And a state inside that sub-state machine has the same conditions. If the condition's requirements were met, ideally we want...
        //...the sub-state machine state in the parent state machine to trigger its transition and the state inside the sub-state machine should not trigger it's transition
        //and this, by default, won't happen if the parent triggered its transition first. However, if this number is higher than 1, then the frames where the child state machine
        //and the parent state machine evaluate their transitions will be desynchronized, therefore the child transition could be triggered first, resulting in
        //playing another state in the sub-state machine, and then right after it plays, the parent state machine's transition will be triggered.
        //This very short play of the sub-state machine's state could most likely result in undesired behaviours.
        //Find a way to synchronize transition evaluation across a group of state machines (all layers and all sub-state machines in those layers).
        private int transitionsEvaluationFrameInterval = 1;

        public StateMachineOperator(IStateMachine stateMachine, AIAgent agent)
        {
            this.agent = agent;

            AssignStateMachine(stateMachine);
        }
        public void SwitchStateMachine(IStateMachine stateMachine)
        {
            CleanUp();

            AssignStateMachine(stateMachine);

            //Since no one will be initializing the states, we should do that now.
            //(since whoever created the operator already initialized it after the it constructed it)
            InitializeStates();
        }

        #region Playable Operator Functions
        protected override void OnInitialize()
        {
            lastTransitionsEvaluatedFrame = -100;
            InitializeStates();
        }
        protected override void OnPlayFlagRaised()
        {
            ActivateState(stateMachine.EntryState);

            activeState.Play();
        }
        protected override void OnStopFlagRaised()
        {
            CleanUp();
        }
        public override void Update()
        {
            if(activeState != null)
            {
                //State tempActiveState = activeState;
                //Debug.Log($"Brain - Operator {stateMachine.ID} - Update - Evaluating Transitions");
                //Evaluate the state's conditions before updating it.
                if(Time.frameCount >= lastTransitionsEvaluatedFrame + transitionsEvaluationFrameInterval)
                {
                    lastTransitionsEvaluatedFrame = Time.frameCount;

                    //If the state isn't already completed, then only evaluate non competion transitions.
                    if (activeState.IsPlaying)
                        activeState.EvaluateNonCompletionTransitions();
                    else
                        activeState.EvaluateAllTransitions();
                }

                //Active state could be null at this point if the above transition evaluation resulted in the state machine being completed.
                //Also in case the evaluation above resulted in a new state, we gurantee that the state will at least be updated once before it transitions to another state
                //(in case the new stat's conditions were satisfied on first evaluation)
                if(activeState != null && activeState.IsPlaying)
                    activeState.Update();
            }
        }
        public override void LateUpdate()
        {
            if(activeState != null && activeState.IsPlaying)
                activeState.LateUpdate();
        }
        public override void FixedUpdate()
        {
            if(activeState != null && activeState.IsPlaying)
                activeState.FixedUpdate();
        }

        #if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            if(activeState != null && activeState.IsPlaying)
                activeState.OnDrawGizmos();
        }
        public override void OnDrawGizmosSelected()
        {
            if(activeState != null && activeState.IsPlaying)
                activeState.OnDrawGizmosSelected();
        }
        #endif
        #endregion

        public void Restart()
        {
            ActivateState(stateMachine.EntryState);
            RaisePlayFlag();
        }

        private void AssignStateMachine(IStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        private void InitializeStates()
        {
            for(int i = 0; i < stateMachine.States.Count; i++)
            {
                stateMachine.States[i].Initialize(agent);
            }
        }
        private void ActivateState(State state)
        {
            if(activeState != null)
            {
                activeState.OnStateComplete = null;
                activeState.OnTransitionSatisfied = null;
            }

            activeState = state;

            lastTransitionsEvaluatedFrame = -100;

            activeState.OnStateComplete = OnActiveStateComplete;
            activeState.OnTransitionSatisfied = OnActiveStateTransitionSatisfied;
        }

        private void OnActiveStateTransitionSatisfied(StateTransition transition)
        {
            if(activeState != null)
            {
                //Transitions could satisfy while the state isn't playing (after it has been completed).
                if (activeState.IsPlaying)
                    activeState.Terminate();

                activeState.OnStateComplete = null;
                activeState.OnTransitionSatisfied = null;
            }

            State transitionState = transition.ToState;

            //If the state this transition directs towards doesn't exit, then do nothing (?).
            if(transitionState == null)
            {
                TafraDebugger.Log("State Machine Operator", "Transition directs toward a state that doesn't exist.", TafraDebugger.LogType.Error);
                return;
            }

            ActivateState(transitionState);
            activeState.Play();
        }

        private void OnActiveStateComplete()
        {
            //If the completed state was the exit state, then complete the entire state machine.
            if(activeState == stateMachine.ExitState)
            {
                activeState = null;
                OnComplete?.Invoke(this);
                return;
            }

            //We will not evaluate the state's conditions here and move to a new state, instead they will be evaluated in the next update call...
            //...to avoid stack overflows if states loop and all of them complete immediately.

            //Force a transitions evaluation in the next frame to find a new state.
            lastTransitionsEvaluatedFrame = -100;

            //activeState.EvaluateAllTransitions();
        }

        public void CleanUp()
        {
            activeState?.Terminate();
        }
    }
}