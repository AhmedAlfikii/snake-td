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
    [CustomNodeEditor(typeof(TriggerAbilityNode))]
    public class TriggerAbilityNodeEditor : AbilityNodeEditor
    {
        public TriggerAbilityNodeEditor(BTNode btNode) : base(btNode)
        {
            this.Q<VisualElement>("order-container").style.display = DisplayStyle.None;
            this.Q<Label>("type-name").text += " - Trigger";
            //this.Q<VisualElement>("type-name").style.display = DisplayStyle.None;

            AddToClassList("triggerNode");
        }
    }
}