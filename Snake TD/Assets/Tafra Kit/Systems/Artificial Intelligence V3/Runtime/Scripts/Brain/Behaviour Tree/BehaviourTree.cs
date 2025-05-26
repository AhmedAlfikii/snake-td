using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;

namespace TafraKit.Internal.AI3
{
    [CreateAssetMenu(menuName = "Tafra Kit/AI3/Behaviour Tree", fileName = "Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        [SerializeField] private InternalBehaviourTree behaviourTree;

        public InternalBehaviourTree Tree => behaviourTree;

        public void Clone()
        {
            for (int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                var node = behaviourTree.Nodes[i];

                if (node is SubBehaviourTreeTaskNode subBehaviourTreeNode)
                    subBehaviourTreeNode.Clone();
            }
        }

        #if UNITY_EDITOR
        public void EditorSetTree(InternalBehaviourTree tree)
        {
            behaviourTree = tree;
        }
        [ContextMenu("Fix References")]
        public void FixInternalTreeReferences()
        {
            behaviourTree.EditorFixChildrenReferences();
        }
        public void EditorRefreshNodesHoldingObject()
        {
            for (int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                var node = behaviourTree.Nodes[i];
                
                node.SetHoldingObject(this);

                if (node is SubBehaviourTreeTaskNode subBehaviourTreeNode)
                    subBehaviourTreeNode.EditorRefreshNodesHoldingObject(this);
            }
        }
        #endif
    }
}