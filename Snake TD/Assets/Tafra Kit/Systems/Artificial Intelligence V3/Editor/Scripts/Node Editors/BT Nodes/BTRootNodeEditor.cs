using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TafraKitEditor.GraphViews;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    [CustomNodeEditor(typeof(RootNode))]
    public class BTRootNodeEditor : AIBTNodeEditor
    {
        public BTRootNodeEditor(AIBTNode btNode) : base(btNode)
        {
            capabilities = capabilities & ~Capabilities.Deletable;

            this.Q<VisualElement>("order-container").style.display = DisplayStyle.None;
            this.Q<VisualElement>("type-name").style.display = DisplayStyle.None;

            AddToClassList("rootNode");
        }
    }
}