using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(TafraAdvancedFloat))]
    public class TafraAdvancedFloatDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new TafraAdvancedFloatField(property, property.displayName, property.tooltip);
        }
    }
}