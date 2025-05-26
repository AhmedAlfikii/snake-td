using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(ScriptableFloat))]
    public class ScriptableFloatEditor : Editor
    {
        private static bool eventsFoldout;

        private SerializedProperty defaultValueProperty;
        private SerializedProperty allowNegativeValuesProperty;
        private SerializedProperty cappedProperty;
        private SerializedProperty capProperty;
        private SerializedProperty idProperty;
        private SerializedProperty autoSaveProperty;
        private SerializedProperty migrateFromOldIdProperty;
        private SerializedProperty oldIdProperty;

        private SerializedProperty onValueChangeProperty;
        private SerializedProperty onDisplayValueChangeProperty;
        private SerializedProperty onValueAddProperty;
        private SerializedProperty onValueDeductProperty;

        protected virtual void OnEnable()
        {
            defaultValueProperty = serializedObject.FindProperty("defaultValue");
            allowNegativeValuesProperty = serializedObject.FindProperty("allowNegativeValues");
            cappedProperty = serializedObject.FindProperty("capped");
            capProperty = serializedObject.FindProperty("cap");
            idProperty = serializedObject.FindProperty("id");
            autoSaveProperty = serializedObject.FindProperty("autoSave");
            migrateFromOldIdProperty = serializedObject.FindProperty("migrateFromOldId");
            oldIdProperty = serializedObject.FindProperty("oldId");

            onValueChangeProperty = serializedObject.FindProperty("onValueChange");
            onDisplayValueChangeProperty = serializedObject.FindProperty("onDisplayValueChange");
            onValueAddProperty = serializedObject.FindProperty("onValueAdd");
            onValueDeductProperty = serializedObject.FindProperty("onValueDeduct");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            BuildSFUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected void BuildSFUI()
        {
            EditorGUILayout.PropertyField(defaultValueProperty);

            if (/*Application.isPlaying &&*/ targets.Length == 1)
            {
                ScriptableFloat sf = target as ScriptableFloat;
                if (target)
                    EditorGUILayout.LabelField($"Current Value: {sf.Value}");
            }

            EditorGUILayout.PropertyField(allowNegativeValuesProperty);
            EditorGUILayout.PropertyField(cappedProperty);
            
            if (cappedProperty.boolValue)
            {
                EditorGUILayout.PropertyField(capProperty);
            }

            EditorGUILayout.PropertyField(idProperty);
            EditorGUILayout.PropertyField(autoSaveProperty);
            EditorGUILayout.PropertyField(migrateFromOldIdProperty);
            EditorGUILayout.PropertyField(oldIdProperty);

            eventsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(eventsFoldout, "Events");

            if (eventsFoldout)
            {
                EditorGUILayout.PropertyField(onValueChangeProperty);
                EditorGUILayout.PropertyField(onDisplayValueChangeProperty);
                EditorGUILayout.PropertyField(onValueAddProperty);
                EditorGUILayout.PropertyField(onValueDeductProperty);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}