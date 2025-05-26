using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.SceneManagement;

namespace TafraKitEditor.SceneManagement
{
    [CustomEditor(typeof(SceneManagerSettings))]
    public class SceneManagerSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty defaultTransition;

        private SceneTransition[] transitions;
        private string[] transitionNames;
        private int selectedTransitionIndex;

        private GUIStyle defaultTransitionPathStyle;

        private void Awake()
        {
            #region Initialize Styles
            defaultTransitionPathStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
            #endregion
        }

        private void OnEnable()
        {
            defaultTransition = serializedObject.FindProperty("DefaultTransition");

            GetAllTransitions();
        }

        public override void OnFocus()
        {
            GetAllTransitions();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox("To use custom transitions, add your transition prefabs to a folder named \"Scene Transitions\" under any Resources folder in your project and add a SceneTransitionUIEG component to it, or a custom transition component that inherits from SceneTransition", MessageType.Info);

            EditorGUILayout.Space(5);


            EditorGUI.BeginChangeCheck();

            selectedTransitionIndex = EditorGUILayout.Popup("Default Transition", selectedTransitionIndex, transitionNames);
           
            if (defaultTransition.objectReferenceValue != null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Transition Path", EditorStyles.label, EditorStyles.miniLabel);
                if (GUILayout.Button(AssetDatabase.GetAssetPath(defaultTransition.objectReferenceValue), defaultTransitionPathStyle))
                    EditorGUIUtility.PingObject(defaultTransition.objectReferenceValue);

                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedTransitionIndex > 0)
                {
                    SceneTransition selectedTransition = transitions[selectedTransitionIndex - 1];

                    if (selectedTransition != null)
                        defaultTransition.objectReferenceValue = selectedTransition.gameObject;
                    else
                        defaultTransition.objectReferenceValue = null;
                }
                else
                    defaultTransition.objectReferenceValue = null;
            }

            serializedObject.ApplyModifiedProperties();
        }

        void GetAllTransitions()
        {
            List<string> names = new List<string>();

            names.Add("None");
            
            selectedTransitionIndex = 0;

            transitions = Resources.LoadAll<SceneTransition>("Scene Transitions");

            for (int i = 0; i < transitions.Length; i++)
            {
                if (!names.Contains(transitions[i].name))
                    names.Add(transitions[i].name);
                else
                {
                    int iteration = 0;
                    string newName = "";
                    do
                    {
                        newName = transitions[i].name + $" ({iteration + 1})";
                        iteration++;
                    } while (names.Contains(newName));

                    names.Add(newName);
                }

                if (defaultTransition.objectReferenceValue != null && transitions[i].gameObject == defaultTransition.objectReferenceValue)
                    selectedTransitionIndex = i + 1;
            }

            transitionNames = names.ToArray();
        }
    }
}