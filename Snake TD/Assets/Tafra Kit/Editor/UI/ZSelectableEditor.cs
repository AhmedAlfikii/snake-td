using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
using TafraKit.UI;
using UnityEngine.UIElements;

namespace TafraKitEditor.UI
{
    [CustomEditor(typeof(ZSelectable))]
    public class ZSelectableEditor : SelectableEditor
    {
        private SerializedProperty m_EnableInteractableValuesSwappingProperty;
        private SerializedProperty m_InteractableValuesSwapperProperty;
        private SerializedProperty m_NodgeContentProperty;
        private SerializedProperty m_ContentProperty;
        private SerializedProperty m_NodgeBlockProperty;

        private AnimBool showContentControls = new AnimBool();
        private int selectedPreviewState = -1;
        private string[] gridString = new string[5] { "Normal", "Highlighted", "Pressed", "Selected", "Disabled" };

        protected override void OnEnable()
        {
            base.OnEnable();

            m_EnableInteractableValuesSwappingProperty = serializedObject.FindProperty("m_EnableInteractableValuesSwapping");
            m_InteractableValuesSwapperProperty = serializedObject.FindProperty("m_InteractableValuesSwapper");
            m_NodgeContentProperty = serializedObject.FindProperty("m_NodgeContent");
            m_ContentProperty = serializedObject.FindProperty("m_Content");
            m_NodgeBlockProperty = serializedObject.FindProperty("m_NodgeBlock");

            showContentControls.value = m_NodgeContentProperty.boolValue;

            showContentControls.valueChanged.AddListener(Repaint);
        }

        protected override void OnDisable()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                    ((ZSelectable)targets[i]).RevertToNormal();
            }

            base.OnDisable();

            showContentControls.valueChanged.RemoveListener(Repaint);
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_EnableInteractableValuesSwappingProperty);

            if(m_EnableInteractableValuesSwappingProperty.boolValue)
            {
                EditorGUILayout.PropertyField(m_InteractableValuesSwapperProperty);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(m_NodgeContentProperty);

            showContentControls.target = m_NodgeContentProperty.boolValue;

            if(EditorGUILayout.BeginFadeGroup(showContentControls.faded))
            {
                ++EditorGUI.indentLevel;
                {
                    EditorGUILayout.PropertyField(m_ContentProperty);
                    EditorGUILayout.PropertyField(m_NodgeBlockProperty, true);
                }
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Preview States", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            selectedPreviewState = GUILayout.SelectionGrid(selectedPreviewState, gridString, 5);
            if(EditorGUI.EndChangeCheck())
            {
                switch(selectedPreviewState)
                {
                    case 0:
                        for(int i = 0; i < targets.Length; i++)
                        {
                            ((ZSelectable)targets[i]).MimicNormal();
                        }
                        break;
                    case 1:
                        for(int i = 0; i < targets.Length; i++)
                        {
                            ((ZSelectable)targets[i]).MimicHighlighted();
                        }
                        break;
                    case 2:
                        for(int i = 0; i < targets.Length; i++)
                        {
                            ((ZSelectable)targets[i]).MimicPressed();
                        }
                        break;
                    case 3:
                        for(int i = 0; i < targets.Length; i++)
                        {
                            ((ZSelectable)targets[i]).MimicSelected();
                        }
                        break;
                    case 4:
                        for(int i = 0; i < targets.Length; i++)
                        {
                            ((ZSelectable)targets[i]).MimicDisabled();
                        }
                        break;
                }
            }

            if(GUILayout.Button("Revert"))
            {
                for(int i = 0; i < targets.Length; i++)
                {
                    ((ZSelectable)targets[i]).RevertToNormal();
                }

                selectedPreviewState = -1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}