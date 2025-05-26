using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;

namespace TafraKit.Internal.AI3
{
    [CreateAssetMenu(menuName = "Tafra Kit/AI3/Sub-State Machine", fileName = "Sub-State Machine")]
    public class SubStateMachine : ScriptableObject
    {
        [SerializeField] private InternalStateMachine stateMachine;

        public InternalStateMachine StateMachine => stateMachine;

        public void Initialize()
        {
            stateMachine.Initialize();
        }

        #if UNITY_EDITOR
        public void EditorSetStateMachine(InternalStateMachine sm)
        {
            stateMachine = sm;
        }
        [ContextMenu("Fix References")]
        public void FixInternalStateMachineReferences()
        {
            stateMachine.EditorFixChildrenReferences();
        }
        public void EditorRefreshNodesHoldingObject()
        {
            for (int i = 0; i < stateMachine.States.Count; i++)
            {
                var state = stateMachine.States[i];

                state.SetHoldingObject(this);

                if (state is SubstateMachineState subStateMachineState)
                    subStateMachineState.EditorRefreshNodesHoldingObject(this);
            }
        }
        #endif

    }
}
