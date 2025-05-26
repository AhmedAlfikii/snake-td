using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    public interface IBTNodesContainer<TBTNode> where TBTNode : BTNode
    {
        public List<TBTNode> Nodes { get; }

        public abstract TBTNode PassiveNode { get; }
        public abstract TBTNode ActiveNode { get; }
        public abstract TBTNode RootNode { get; }
    }
}