using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Behavior Tree"), GraphNodeName("Behaviour Tree", "Tree")]
    public class BehaviourTreeState : ActionState
    {
        [SerializeField] private BehaviourTree externalBehaviourTree;
        [SerializeField, HideInGraphInspector] private InternalBehaviourTree internalBehaviourTree;

        [NonSerialized] private BehaviourTree curExternalBehaviourTree;

        private IBehaviourTree activeBehaviourTree;
        public IBehaviourTree InternalBehaviourTree => internalBehaviourTree;
        public IBehaviourTree ExternalBehaviourTree => curExternalBehaviourTree.Tree;
        public IBehaviourTree AvailableBehaviourTree
        {
            get
            {
                if(externalBehaviourTree != null)
                    return externalBehaviourTree.Tree;
                else
                    return internalBehaviourTree;
            }
        }

        public BehaviourTreeState()
        {
            internalBehaviourTree = new InternalBehaviourTree();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (curExternalBehaviourTree != null)
                activeBehaviourTree = curExternalBehaviourTree.Tree;
            else
                activeBehaviourTree = internalBehaviourTree;

            activeBehaviourTree.Initialize(agent);
        }

        public override void Update()
        {
            BTNodeState state = activeBehaviourTree.Update();

            if(state == BTNodeState.Success || state == BTNodeState.Failure)
                Complete();
        }
        public override void OnDestroy()
        {
            if(curExternalBehaviourTree != null)
            {
                Debug.Log($"Destroying {curExternalBehaviourTree}");
                UnityEngine.Object.Destroy(curExternalBehaviourTree);
            }

            internalBehaviourTree.OnDestroy();
        }

        public override void OnDrawGizmos()
        {
            for (int i = 0; i < activeBehaviourTree.Nodes.Count; i++)
            {
                var node = activeBehaviourTree.Nodes[i];
                node.OnDrawGizmos();
            }
        }
        public override void OnDrawGizmosSelected()
        {
            for (int i = 0; i < activeBehaviourTree.Nodes.Count; i++)
            {
                var node = activeBehaviourTree.Nodes[i];
                node.OnDrawGizmosSelected();
            }
        }
        protected override void OnTerminate()
        {
            activeBehaviourTree.Root.Reset();
        }
        public void Clone()
        {
            if (externalBehaviourTree != null)
            {
                curExternalBehaviourTree = UnityEngine.Object.Instantiate(externalBehaviourTree);

                curExternalBehaviourTree.Clone();
            }
            else
            {
                for (int i = 0; i < internalBehaviourTree.Nodes.Count; i++)
                {
                    var node = internalBehaviourTree.Nodes[i];

                    if (node is SubBehaviourTreeTaskNode subBehaviourTreeNode)
                        subBehaviourTreeNode.Clone();
                }
            }
        }

        #if UNITY_EDITOR
        public void EditorClearInternalBehaviourTree()
        {
            internalBehaviourTree = new InternalBehaviourTree();
        }
        public void EditorFixInternalTreeChildrenReferences()
        {
            internalBehaviourTree.EditorFixChildrenReferences();
        }
        public void EditorSetExternalBehaviourTree(BehaviourTree externalBehaviourTree)
        {
            this.externalBehaviourTree = externalBehaviourTree;
        }
        #endif
    }
}
