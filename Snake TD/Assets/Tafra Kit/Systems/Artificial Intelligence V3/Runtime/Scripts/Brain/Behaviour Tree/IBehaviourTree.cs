using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    public interface IBehaviourTree
    {
        public List<AIBTNode> Nodes { get; }
        public RootNode Root { get; }

        public void Initialize(AIAgent agent)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                AIBTNode node = Nodes[i];
                node.Initialize(agent);
            }
        }

        public BTNodeState Update()
        {
            return Root.Update();
        }
    }
}
