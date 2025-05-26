using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit.Internal.CharacterControls;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.CharacterControls
{
    [CustomNodeEditor(typeof(RootNode))]
    public class RootNodeEditor : AbilityNodeEditor
    {
        public RootNodeEditor(BTNode btNode) : base(btNode)
        {
            capabilities = capabilities & ~Capabilities.Deletable;

            this.Q<VisualElement>("order-container").style.display = DisplayStyle.None;
            this.Q<VisualElement>("type-name").style.display = DisplayStyle.None;

            AddToClassList("rootNode");
        }
    }
}