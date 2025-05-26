using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

namespace TafraKit
{
    [CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
    public class NavMeshAreaMaskAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var width = position.width;
            position.width = EditorGUIUtility.labelWidth;
            EditorGUI.PrefixLabel(position, label);

            string[] navMeshAreaNames = NavMesh.GetAreaNames();
            var mask = property.intValue;
            position.x += EditorGUIUtility.labelWidth;
            position.width = width - EditorGUIUtility.labelWidth;

            EditorGUI.BeginChangeCheck();
            mask = EditorGUI.MaskField(position, mask, navMeshAreaNames);
            if(EditorGUI.EndChangeCheck())
                property.intValue = mask;
        }
    }
}