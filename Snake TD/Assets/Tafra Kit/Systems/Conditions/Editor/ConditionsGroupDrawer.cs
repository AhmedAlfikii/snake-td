using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Conditions;
using TafraKitEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKit.AI2
{
    [CustomPropertyDrawer(typeof(ConditionsGroup))]
    public class ConditionsContainerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializeReferenceListContainerField srListField = new SerializeReferenceListContainerField(property, "conditions", false, "Condition", "Conditions", property.tooltip);

            return srListField;
        }
    }
}