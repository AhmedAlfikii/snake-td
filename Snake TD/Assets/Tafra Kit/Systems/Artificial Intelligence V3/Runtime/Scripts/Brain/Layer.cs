using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;

namespace TafraKit.Internal.AI3
{
    [System.Serializable]
    public class Layer
    {
        [SerializeField] private string layerName;
        [SerializeField] private InternalStateMachine stateMachine;

        public string Name 
        {
            get
            {
                return layerName;
            }
            set
            {
                layerName = value;
            }
        }
        public InternalStateMachine StateMachine => stateMachine;

        public Layer(string name)
        {
            layerName = name;

            stateMachine = new InternalStateMachine();
        }

        public void Initialize()
        {
            stateMachine.Initialize();
        }

        public void OnDestroy()
        {
            stateMachine.OnDestroy();
        }
    }
}
