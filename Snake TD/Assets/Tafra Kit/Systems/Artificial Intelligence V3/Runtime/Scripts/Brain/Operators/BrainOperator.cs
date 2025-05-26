using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using UnityEngine;

namespace TafraKit.Internal.AI3
{
    public class BrainOperator : PlayableOperator
    {
        private AIAgent agent;
        private Brain brain;
        private List<StateMachineOperator> layerStateMachineOperators;

        public BrainOperator(Brain brain, AIAgent agent)
        {
            this.agent = agent;
            
            layerStateMachineOperators = new List<StateMachineOperator>();

            if(brain == null)
            {
                this.brain = null;
                return;
            }

            AssignBrain(brain);
        }

        public void SwitchBrain(Brain brain)
        {
            if(this.brain == brain)
                return;

            //Stop the current brain's layer operators if any.
            StopLayerOperators();

            if(brain == null)
            {
                this.brain = null;
                return;
            }

            AssignBrain(brain);

            //Since no one will be initializing and playing the operators, we should do that now.
            //(since whoever created the brain operator already initialized and played the brain operator after the it constructed it)
            InitializeLayerOperators();
            PlayLayerOperators();
        }

        #region MonoBehaviour Messages Mimic
        protected override void OnInitialize()
        {
            InitializeLayerOperators();
        }
        protected override void OnPlayFlagRaised()
        {
            PlayLayerOperators();
        }
        protected override void OnStopFlagRaised()
        {
            StopLayerOperators();
        }
        public override void Update()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].Update();
            }
        }
        public override void LateUpdate()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].LateUpdate();
            }
        }
        public override void FixedUpdate()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].FixedUpdate();
            }
        }

        #if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].OnDrawGizmos();
            }
        }
        public override void OnDrawGizmosSelected()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].OnDrawGizmosSelected();
            }
        }
        #endif
        #endregion

        private void AssignBrain(Brain brain)
        {
            this.brain = brain;

            //Create a new state machine operator for each layer in the brain. Reuse old operators in case some where created for a previously assigned brain.
            for(int i = 0; i < brain.Layers.Count; i++)
            {
                StateMachineOperator layerStateMachineOperator;
                Layer layer = brain.Layers[i];

                layer.Initialize();

                if(layerStateMachineOperators.Count < i)
                {
                    layerStateMachineOperator = layerStateMachineOperators[i];
                    layerStateMachineOperator.SwitchStateMachine(layer.StateMachine);
                }
                else
                {
                    layerStateMachineOperator = new StateMachineOperator(layer.StateMachine, agent);
                    layerStateMachineOperator.OnComplete = OnLayerStateMachineOperatorComplete;
                }

                layerStateMachineOperators.Add(layerStateMachineOperator);
            }

            //Remove extra layer state machines operators that were here because of a previous brain.
            int extraOperators = layerStateMachineOperators.Count - brain.Layers.Count;
            for(int i = 0; i < extraOperators; i++)
            {
                layerStateMachineOperators.RemoveAt(layerStateMachineOperators.Count - 1);
            }
        }

        private void OnLayerStateMachineOperatorComplete(StateMachineOperator layerOperator)
        {
            layerOperator.Restart();
        }

        private void InitializeLayerOperators()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].Initialize();
            }
        }
        private void PlayLayerOperators()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].RaisePlayFlag();
            }
        }
        private void StopLayerOperators()
        {
            for(int i = 0; i < layerStateMachineOperators.Count; i++)
            {
                layerStateMachineOperators[i].RaiseStopFlag();
            }
        }
    }
}