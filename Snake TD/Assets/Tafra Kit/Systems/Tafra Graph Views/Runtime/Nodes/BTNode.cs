using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [Serializable]
    public abstract class BTNode : GraphNode
    {
        public BTNode()
        { 

        }
        public BTNode(List<BTNode> children)
        { 
            this.children.Clear();
            for (int i = 0; i < children.Count; i++)
            {
                this.children.Add(children[i]);
            }
        }
        public BTNode(BTNode otherNode)
        { 
            children.Clear();
            for (int i = 0; i < otherNode.children.Count; i++)
            {
                children.Add(otherNode.children[i]);
            }
        }

        [SerializeReference, HideInInspector] protected List<BTNode> children = new List<BTNode>();

        [NonSerialized] protected bool isStarted;
        [NonSerialized] protected BTNodeState state;

        public bool IsStarted => isStarted;
        public BTNodeState State => state;
        public List<BTNode> Children => children;
        public abstract bool HasInputPort { get; }
        public abstract bool HasOutputPort { get; }
        public abstract bool MultiInputs { get; }
        public abstract bool MultiOutputs { get; }

        //The following fields/properties should ONLY be used in editor scripts.
        #region Editor Only Fields
        #if UNITY_EDITOR
        [NonSerialized] public bool LogEditorEvents;
        [NonSerialized] public Action<BTNode> EditorOnStart;
        [NonSerialized] public Action<BTNode> EditorOnSuccess;
        [NonSerialized] public Action<BTNode> EditorOnFailure;
        [NonSerialized] public Action<BTNode> EditorOnReset;
        #endif
        #endregion

        public BTNodeState Update()
        {
            if(!isStarted)
            {
                #if UNITY_EDITOR
                if(LogEditorEvents)
                    EditorOnStart?.Invoke(this);
                #endif

                OnStart();
                isStarted = true;
            }

            state = OnUpdate();

            if(state == BTNodeState.Success || state == BTNodeState.Failure)
            {
                #if UNITY_EDITOR
                if(LogEditorEvents)
                {
                    if(state == BTNodeState.Success)
                        EditorOnSuccess?.Invoke(this);
                    else if(state == BTNodeState.Failure)
                        EditorOnFailure?.Invoke(this);
                }
                #endif

                OnEnd();
                isStarted = false;
            }

            return state;
        }
        public void Reset()
        {
            #if UNITY_EDITOR
            if(LogEditorEvents)
            {
                EditorOnReset?.Invoke(this);
            }
            #endif

            for(int i = 0; i < children.Count; i++)
            {
                children[i].Reset();
            }

            isStarted = false;

            OnEnd();
            OnReset();
        }

        protected virtual void OnStart() { }
        protected abstract BTNodeState OnUpdate();
        /// <summary>
        /// Gets called whenever once the node finishes running, whether it succeeded, failed, or was forced to stop (Reset).
        /// </summary>
        protected virtual void OnEnd() { }
        /// <summary>
        /// Gets called whenever the node is forced to stop (Reset).
        /// </summary>
        protected virtual void OnReset() { }
        /// <summary>
        /// The first function to get called. Gets called once no matter how many times the node repeated.
        /// </summary>
        protected virtual void OnInitialize() { }

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }

        public BTNode CloneNode()
        {
            BTNode clonedNode = CloneContent();

            clonedNode.children.Clear();

            //Clone childs
            for(int i = 0; i < children.Count; i++)
            {
                BTNode originalChild = children[i];

                BTNode clonedChild = originalChild.CloneNode();

                clonedNode.children.Add(clonedChild);
            }

            return clonedNode;
        }
        protected virtual BTNode CloneContent()
        {
            throw new NotImplementedException();
        }
    }
}