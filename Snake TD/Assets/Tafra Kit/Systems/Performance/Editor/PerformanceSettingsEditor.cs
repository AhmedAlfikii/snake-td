using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.Performance;

namespace TafraKitEditor.Performance
{
    [CustomEditor(typeof(PerformanceSettings))]
    public class PerformanceSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty enabled;
        private SerializedProperty controlFrameRate;
        private SerializedProperty targetFrameRate;
        private SerializedProperty applyToEditor;
        private SerializedProperty screenStayAwake;

        private void OnEnable()
        {
            enabled = serializedObject.FindProperty("Enabled");
            controlFrameRate = serializedObject.FindProperty("controlFrameRate");
            targetFrameRate = serializedObject.FindProperty("targetFrameRate");
            applyToEditor = serializedObject.FindProperty("applyFrameRateToEditor");
            screenStayAwake = serializedObject.FindProperty("screenStayAwake");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(enabled);

            EditorGUI.BeginDisabledGroup(!enabled.boolValue);
            
            EditorGUILayout.PropertyField(controlFrameRate);

            EditorGUI.BeginDisabledGroup(!controlFrameRate.boolValue);
            EditorGUILayout.PropertyField(targetFrameRate);
            EditorGUILayout.PropertyField(applyToEditor);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(screenStayAwake);

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}