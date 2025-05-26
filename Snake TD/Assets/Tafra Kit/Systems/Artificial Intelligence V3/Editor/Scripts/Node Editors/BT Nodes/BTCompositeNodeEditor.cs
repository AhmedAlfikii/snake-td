using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(CompositeNode))]
    public class BTCompositeNodeEditor : AIBTNodeEditor
    {
        public BTCompositeNodeEditor(AIBTNode btNode) : base(btNode)
        {
            AddToClassList("compositeNode");
        }
    }
}