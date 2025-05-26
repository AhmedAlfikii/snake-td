using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using System;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/General/Sub-Behaviour Tree"), GraphNodeName("Sub-Behaviour Tree")]
    public class SubBehaviourTreeTaskNode : AbilityTaskNode
    {
        public SubBehaviourTreeTaskNode()
        {
            internalBehaviourTree = new InternalBehaviourTree();
        }

        [SerializeField] private AbilityBehaviourTree externalBehaviourTree;
        [SerializeField, HideInGraphInspector] private InternalBehaviourTree internalBehaviourTree;

        [NonSerialized] private RootNode rootNode;
        [NonSerialized] private AbilityBehaviourTree curExternalBehaviourTree;

        private IBTNodesContainer<AbilityNode> activeBehaviourTree;
        public IBTNodesContainer<AbilityNode> InternalBehaviourTree => internalBehaviourTree;
        public IBTNodesContainer<AbilityNode> ExternalBehaviourTree => curExternalBehaviourTree.Tree;
        public IBTNodesContainer<AbilityNode> AvailableBehaviourTree
        {
            get
            {
                if (externalBehaviourTree != null)
                    return externalBehaviourTree.Tree;
                else
                    return internalBehaviourTree;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (curExternalBehaviourTree != null)
                activeBehaviourTree = curExternalBehaviourTree.Tree;
            else
                activeBehaviourTree = internalBehaviourTree;

            rootNode = activeBehaviourTree.RootNode as RootNode;

            for (int i = 0; i < activeBehaviourTree.Nodes.Count; i++)
            {
                var node = activeBehaviourTree.Nodes[i];
                node.Initialize(ability);
            }
        }

        protected override BTNodeState OnUpdate()
        {
            BTNodeState state = rootNode.Update();

            return state;
        }
        public override void OnDestroy()
        {
            if(curExternalBehaviourTree != null)
                UnityEngine.Object.Destroy(curExternalBehaviourTree);

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

        protected override void OnReset()
        {
            activeBehaviourTree.RootNode.Reset();
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
    }
}