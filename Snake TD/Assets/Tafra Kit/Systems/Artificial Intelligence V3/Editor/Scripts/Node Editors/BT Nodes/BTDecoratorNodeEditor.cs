using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(DecoratorNode))]
    public class BTDecoratorNodeEditor : AIBTNodeEditor
    {
        public BTDecoratorNodeEditor(AIBTNode btNode) : base(btNode)
        {
            AddToClassList("decoratorNode");
        }
    }
}