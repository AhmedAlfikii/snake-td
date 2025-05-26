using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TafraKit;
using TafraKitEditor.AI3;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(PropertyBoxAttribute))]
    public class PropertyBoxAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyBoxAttribute att = attribute as PropertyBoxAttribute;

            string title = string.IsNullOrEmpty(att.Title) ? property.displayName : att.Title;
            string tooltip = string.IsNullOrEmpty(att.Tooltip) ? property.tooltip : att.Tooltip;

            return new PropertyBox(property, title, att.Subtitle, tooltip, att.DontDrawContent, att.DrawPropertyInContent);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}