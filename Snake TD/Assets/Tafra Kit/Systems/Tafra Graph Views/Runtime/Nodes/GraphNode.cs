using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [System.Serializable]
    public class GraphNode
    {
        [SerializeField, HideInInspector] protected string name;
        [SerializeField, HideInInspector] protected string guid;
        [SerializeField, HideInInspector] private Rect position;
        /// <summary>
        /// The object that contains this node. For example: an AI brain, an external sub-state machine, external behaviour tree, etc...
        /// </summary>
        [SerializeField, HideInInspector] private UnityEngine.Object holdingObject;
        
        public string GUID => guid;
        public string Name 
        {
            get
            { 
                return name;
            }
            set 
            { 
                name = value;
            }
        }
        public Rect Position 
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// The object that contains this node. For example: an AI brain, an external sub-state machine, external behaviour tree, etc...
        /// </summary>
        public UnityEngine.Object HoldingObject => holdingObject;

        public GraphNode()
        {
            guid = Guid.NewGuid().ToString();
        }

        public void SetHoldingObject(UnityEngine.Object holdingObject)
        {
            this.holdingObject = holdingObject;
        }

        public virtual void OnDestroy() { }
    }
}
