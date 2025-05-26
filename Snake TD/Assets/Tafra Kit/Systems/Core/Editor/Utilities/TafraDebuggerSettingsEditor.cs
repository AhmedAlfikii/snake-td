using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(TafraDebuggerSettings))]
    public class TafraDebuggerSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty buildLogLevel;
        private SerializedProperty editorLogLevel;
        private SerializedProperty disableColorCoding;

        private void OnEnable()
        {
            buildLogLevel = serializedObject.FindProperty("BuildLogLevel");
            editorLogLevel = serializedObject.FindProperty("EditorLogLevel");
            disableColorCoding = serializedObject.FindProperty("DisableColorCoding");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(buildLogLevel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(editorLogLevel);
            EditorGUILayout.PropertyField(disableColorCoding);

            serializedObject.ApplyModifiedProperties();
        }
    }
}