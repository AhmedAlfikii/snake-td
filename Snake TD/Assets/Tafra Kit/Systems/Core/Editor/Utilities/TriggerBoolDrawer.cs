using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TafraKit;

namespace TafraKitEditor
{
    [CustomPropertyDrawer(typeof(TriggerBool))]
    public class TriggerBoolDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.FindPropertyRelative("trigger").boolValue = EditorGUI.Toggle(position, new GUIContent(property.displayName), property.FindPropertyRelative("trigger").boolValue, EditorStyles.radioButton);

            EditorGUI.EndProperty();
        }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();

            PropertyField triggerField = new PropertyField(property.FindPropertyRelative("trigger"));

            container.Add(triggerField);

            return container;
        }
    }
}