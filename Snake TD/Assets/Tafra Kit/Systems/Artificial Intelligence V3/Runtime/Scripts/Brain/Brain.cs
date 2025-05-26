using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Internal.AI3;
using TafraKit.GraphViews;
using TafraKit.Internal.GraphViews;

namespace TafraKit.AI3
{
    [CreateAssetMenu(menuName = "Tafra Kit/AI3/Brain", fileName = "Brain")]
    public class Brain : ScriptableObject, IGraphBlackboardContainer
    {
        #region Serialized Hidden Fields
        [Header("Temporarily Visible")]
        [SerializeField] private List<Layer> layers = new List<Layer>();
        [SerializeField] private GraphBlackboard blackboard = new GraphBlackboard();
        #endregion

        #region Editor Only Fields
        /// <summary>
        /// This is meant for editor use only. To keep track of the brain's displayed layer index in the brain graph window.
        /// </summary>
        public int DisplayedLayerIndex;
        #endregion

        #region Non-serialized Fields
        #endregion

        #region Public Properties
        public List<Layer> Layers => layers;
        public GraphBlackboard Blackboard => blackboard;
        #endregion

        public Brain()
        {
            CreateLayer();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].OnDestroy();
            }
        }

        #region Data Creation
        /// <summary>
        /// Creates a new layer and returns it.
        /// </summary>
        /// <returns>The newly created layer</returns>
        public Layer CreateLayer()
        {
            string layerName = layers.Count == 0 ? "Base Layer" : $"Layer {layers.Count}";

            Layer layer = new Layer(layerName);

            layers.Add(layer);

            return layer;
        }
        public State CreateStateNode(int layerIndex, Type nodeType)
        {
            if(layerIndex > layers.Count - 1)
            {
                TafraDebugger.Log("Brain", $"Failed to create a state node of type {nodeType}. A layer with the index {layerIndex} couldn't be found.", TafraDebugger.LogType.Error);
                return null;
            }

            State stateNode = Activator.CreateInstance(nodeType) as State;

            if (stateNode == null)
            {
                TafraDebugger.Log("Brain", $"Failed to create a state node of type {nodeType}. Type must inherit from StateNode.", TafraDebugger.LogType.Error);
                return null;
            }

            layers[layerIndex].StateMachine.States.Add(stateNode);

            return stateNode;
        }
        public State CreateStateNode<T>(int layerindex) where T : State
        { 
            return null;
        }
        #endregion

        public Brain Clone()
        {
            Brain clonedBrain = Instantiate(this);

            for (int layerIndex = 0; layerIndex < clonedBrain.layers.Count; layerIndex++)
            {
                var layer = clonedBrain.layers[layerIndex];

                for (int stateIndex = 0; stateIndex < layer.StateMachine.States.Count; stateIndex++)
                {
                    var state = layer.StateMachine.States[stateIndex];

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
            return clonedBrain;
        }
    }
}
