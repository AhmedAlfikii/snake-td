using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using System;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/General/Sub-Behaviour Tree"), GraphNodeName("Sub-Behaviour Tree")]
    public class SubBehaviourTreeTaskNode : TaskNode
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
                if (externalBehaviourTree != null)
                    return externalBehaviourTree.Tree;
                else
                    return internalBehaviourTree;
            }
        }

        public SubBehaviourTreeTaskNode()
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

        protected override BTNodeState OnUpdate()
        {
            BTNodeState state = activeBehaviourTree.Update();

            return state;
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

        protected override void OnReset()
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
        public void EditorRefreshNodesHoldingObject(UnityEngine.Object holdingObject)
        {
            for (int i = 0; i < internalBehaviourTree.Nodes.Count; i++)
            {
                var node = internalBehaviourTree.Nodes[i];

                node.SetHoldingObject(holdingObject);

                if (node is SubBehaviourTreeTaskNode subBehaviourTreeNode)
                    subBehaviourTreeNode.EditorRefreshNodesHoldingObject(holdingObject);
            }

            if (externalBehaviourTree != null)
                externalBehaviourTree.EditorRefreshNodesHoldingObject();
        }
        public void EditorFixInternalBTChildrenReferences()
        {
            internalBehaviourTree.EditorFixChildrenReferences();
        }
        #endif
    }
}