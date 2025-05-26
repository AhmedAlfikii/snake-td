using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [Serializable]
    public class InternalBehaviourTree : IBehaviourTree
    {
        [SerializeReference, HideInGraphInspector] private List<AIBTNode> nodes;
        [SerializeReference, HideInInspector] private RootNode root;

        public List<AIBTNode> Nodes => nodes;
        public RootNode Root => root;

        public InternalBehaviourTree()
        {
            nodes = new List<AIBTNode>();

            root = new RootNode();

            nodes.Add(root);
        }

        public BTNodeState Update()
        {
            BTNodeState state = root.Update();

            return state;
        }
        public void OnDestroy()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].OnDestroy();
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Since the new children (if copied from another behaviour tree) now have the default node as reference, we need to get the actual nodes.
        /// Example: the root node is now just a AIBTNode, it's not actually of type RootNode, so what we need to do is get the corect reference from the nodes list.
        /// </summary>
        public void EditorFixChildrenReferences()
        {
            root = nodes[0] as RootNode;

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                EditorFixNodeChildrenReferences(node);
            }
        }
        /// <summary>
        /// Since the new children (if copied from another behaviour tree) now have the default node as reference, we need to get the actual nodes.
        /// Example: the root node is now just a AIBTNode, it's not actually of type RootNode, so what we need to do is get the corect reference from the nodes list.
        /// </summary>
        private void EditorFixNodeChildrenReferences(AIBTNode node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                BTNode messedUpChild = node.Children[i];
                BTNode correctChild = null;

                for (int j = 0; j < nodes.Count; j++)
                {
                    var mainNode = nodes[j];

                    if (mainNode.GUID == messedUpChild.GUID)
                    { 
                        correctChild = mainNode;
                        break;
                    }
                }

                if (correctChild == null)
                    Debug.LogError($"Couldn't find the correct child index ({i}) for node ({node.Name}).");
                else
                    node.Children[i] = correctChild;
            }

            if (node is SubBehaviourTreeTaskNode subBT)
            {
                subBT.EditorFixInternalBTChildrenReferences();
            }
        }
#endif
    }
}