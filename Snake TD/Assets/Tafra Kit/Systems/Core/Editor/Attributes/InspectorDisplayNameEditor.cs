using UnityEngine;
using UnityEditor;

namespace TafraKit
{
    [CustomPropertyDrawer(typeof(InspectorDisplayNameAttribute))]
    public class DisplayNameEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent((attribute as InspectorDisplayNameAttribute).DisplayName));
        }
    }
}