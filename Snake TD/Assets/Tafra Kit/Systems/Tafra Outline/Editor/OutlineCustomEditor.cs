using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(TafraOutline))]
    public class OutlineCustomEditor : Editor
    {
        private SerializedProperty renderersProperty;

        private void OnEnable()
        {
            renderersProperty = serializedObject.FindProperty("renderers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var properties = serializedObject.GetVisibleProperties();
            foreach(var property in properties)
            {
                if(property.name == renderersProperty.name)
                    break;

                EditorGUILayout.PropertyField(property);
            }

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(renderersProperty);

            EditorGUI.EndDisabledGroup();

            if(renderersProperty.arraySize == 0)
                EditorGUILayout.HelpBox("Renderers list is empty, no outline will appear. Press the \"Update Renderers\" button.", MessageType.Error);

            if(GUILayout.Button("Update Renderers", GUILayout.Height(25)))
            {
                TafraOutline outlineCustom = (TafraOutline)target;

                Undo.RecordObject(outlineCustom, "Update Renderers");

                outlineCustom.FetchReferences();

                EditorUtility.SetDirty(outlineCustom);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}