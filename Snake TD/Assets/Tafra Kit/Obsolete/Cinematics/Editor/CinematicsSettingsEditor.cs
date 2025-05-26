using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;
using TafraKitEditor;

namespace TafraKitEditor
{
    [CustomEditor(typeof(CinematicsSettings))]
    public class SceneManagerSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty enabledProperty;
        private SerializedProperty displayCinematicBarsProperty;
        private SerializedProperty disableInputsDuringCinematicsProperty;

        private void Awake()
        {

        }

        private void OnEnable()
        {
            enabledProperty = serializedObject.FindProperty("Enabled");
            displayCinematicBarsProperty = serializedObject.FindProperty("DisplayCinematicBars");
            disableInputsDuringCinematicsProperty = serializedObject.FindProperty("DisableInputsDuringCinematics");
        }

        public override void OnFocus()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(enabledProperty);

            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);

            EditorGUILayout.PropertyField(displayCinematicBarsProperty);
            EditorGUILayout.PropertyField(disableInputsDuringCinematicsProperty);

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}