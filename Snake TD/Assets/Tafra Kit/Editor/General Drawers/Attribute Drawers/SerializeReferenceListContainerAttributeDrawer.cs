using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(SerializeReferenceListContainerAttribute))]
    public class SerializeReferenceListContainerAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializeReferenceListContainerAttribute att = attribute as SerializeReferenceListContainerAttribute;
            return new SerializeReferenceListContainerField(property, att.ListPropretyName, att.ForceUniqueElements, att.SingleElementName, att.PluralElementName, property.tooltip, false, att.MaxElementsCount);
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