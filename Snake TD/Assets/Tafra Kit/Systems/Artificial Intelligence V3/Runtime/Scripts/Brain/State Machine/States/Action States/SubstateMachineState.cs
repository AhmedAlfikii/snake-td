using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Sub-State Machine"), GraphNodeName("Sub-State Machine", "SM")]
    public class SubstateMachineState : ActionState
    {
        [SerializeField] private SubStateMachine externalStateMachine;
        [SerializeField, HideInGraphInspector] private InternalStateMachine internalStateMachine;

        [NonSerialized] private SubStateMachine curExternalStateMachine;

        private IStateMachine activeStateMachine;
        private StateMachineOperator stateMachineOperator;

        public IStateMachine InternalStateMachine => internalStateMachine;
        public IStateMachine ExternalStateMachine => curExternalStateMachine.StateMachine;
        public IStateMachine AvailableStateMachine 
        {
            get 
            {
                if(externalStateMachine != null)
                    return externalStateMachine.StateMachine;
                else
                    return internalStateMachine;
            }
        }

        public SubstateMachineState()
        {
            internalStateMachine = new InternalStateMachine();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if(curExternalStateMachine != null)
            {
                curExternalStateMachine.Initialize();
                activeStateMachine = curExternalStateMachine.StateMachine;
            }
            else
            {
                internalStateMachine.Initialize();
                activeStateMachine = internalStateMachine;
            }

            stateMachineOperator = new StateMachineOperator(activeStateMachine, agent);
            stateMachineOperator.Initialize();
            stateMachineOperator.OnComplete = OnOperatorComplete;
        }

        protected override void OnPlay()
        {
            stateMachineOperator.RaisePlayFlag();
        }
        protected override void OnTerminate()
        {
            stateMachineOperator.RaiseStopFlag();
        }

        public override void Update()
        {
            stateMachineOperator.Update();
        }
        public override void LateUpdate()
        {
            stateMachineOperator.LateUpdate();
        }
        public override void FixedUpdate()
        {
            stateMachineOperator.FixedUpdate();
        }
        public override void OnDestroy()
        {
            if (curExternalStateMachine != null)
                UnityEngine.Object.Destroy(curExternalStateMachine);

            internalStateMachine.OnDestroy();
        }

        #if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            stateMachineOperator.OnDrawGizmos();
        }
        public override void OnDrawGizmosSelected()
        {
            stateMachineOperator.OnDrawGizmosSelected();
        }
        public void EditorClearInternalStateMachine()
        {
            internalStateMachine = new AI3.InternalStateMachine();
        }
        public void EditorSetExternalStateMachine(SubStateMachine externalStateMachine)
        {
            this.externalStateMachine = externalStateMachine;
        }
        public void EditorRefreshNodesHoldingObject(UnityEngine.Object holdingObject)
        {
            for (int i = 0; i < internalStateMachine.States.Count; i++)
            {
                var state = internalStateMachine.States[i];

                state.SetHoldingObject(holdingObject);

                if (state is SubstateMachineState subStateMachineState)
                    subStateMachineState.EditorRefreshNodesHoldingObject(holdingObject);
            }

            if (externalStateMachine != null)
                externalStateMachine.EditorRefreshNodesHoldingObject();
        }
        public void EditorFixInternalStateMachineReferences()
        {
            internalStateMachine.EditorFixChildrenReferences();
        }
        #endif

        private void OnOperatorComplete(StateMachineOperator @operator)
        {
            Complete();
        }

        public void Clone()
        {
            if(externalStateMachine != null)
            {
                curExternalStateMachine = UnityEngine.Object.Instantiate(externalStateMachine);

                for(int i = 0; i < curExternalStateMachine.StateMachine.States.Count; i++)
                {
                    var state = curExternalStateMachine.StateMachine.States[i];

                    if(state is SubstateMachineState substateMachineState)
                    {
                        substateMachineState.Clone();
                    }
                    else if(state is BehaviourTreeState behaviourTreeState)
                    {
                        behaviourTreeState.Clone();
                    }
                }
            }
        }
    }
}
