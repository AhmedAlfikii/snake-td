using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(SFXPlayerSettings))]
    public class SFXPlayerSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty enabled;
        private SerializedProperty volumeScale;
        private SerializedProperty mutedByDefault;

        private void OnEnable()
        {
            enabled = serializedObject.FindProperty("Enabled");
            volumeScale = serializedObject.FindProperty("VolumeScale");
            mutedByDefault = serializedObject.FindProperty("MutedByDefault");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(enabled);

            EditorGUI.BeginDisabledGroup(!enabled.boolValue);
            
            EditorGUILayout.PropertyField(volumeScale);
            EditorGUILayout.PropertyField(mutedByDefault);

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}