using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(ScriptableVector3))]
    public class ScriptableVector3Editor : Editor
    {
        private static bool eventsFoldout;

        private SerializedProperty defaultValueProperty;
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
                ScriptableVector3 sv3 = target as ScriptableVector3;
                if (target)
                    EditorGUILayout.LabelField($"Current Value: {sv3.Value}");
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