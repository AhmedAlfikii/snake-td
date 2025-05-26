using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.CharacterControls
{
    [Serializable]
    public class InternalBehaviourTree : IBTNodesContainer<AbilityNode>
    {
        [SerializeReference, HideInGraphInspector] private List<AbilityNode> nodes;
        [SerializeReference, HideInInspector] private RootNode root;

        public List<AbilityNode> Nodes => nodes;
        public AbilityNode ActiveNode => null;
        public AbilityNode PassiveNode => null;
        public AbilityNode RootNode => root;

        public InternalBehaviourTree()
        {
            nodes = new List<AbilityNode>();

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
    }
}