using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(TaskNode))]
    public class BTTaskNodeEditor : AIBTNodeEditor
    {
        public BTTaskNodeEditor(AIBTNode btNode) : base(btNode)
        {
            AddToClassList("taskNode");
        }
    }
}