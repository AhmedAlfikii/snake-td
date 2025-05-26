using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.AI3;
using TafraKitEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit.Internal.GraphViews;

namespace TafraKit.AI3
{
    [CustomPropertyDrawer(typeof(BlackboardConditionsGroup))]
    public class BlackboardConditionsGroupDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SerializeReferenceListContainerField(property, "conditions", false, "Condition", "Conditions", property.tooltip);
        }
    }
}