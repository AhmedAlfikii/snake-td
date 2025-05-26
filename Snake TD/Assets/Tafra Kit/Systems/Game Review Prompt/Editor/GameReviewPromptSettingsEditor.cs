using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKitEditor;

namespace TafraKit.ReviewPrompt
{
    [CustomEditor(typeof(GameReviewPromptSettings))]
    public class GameReviewPromptSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty enabled;
        private SerializedProperty level;
        private SerializedProperty getReviewInfoOnLevelStart;

        private bool androidReviewPackageFound;
        private string androidPackageDefningSymbol = "TAFRA_REVIEW_PROMPT";

        private void OnEnable()
        {
            enabled = serializedObject.FindProperty("Enabled");
            level = serializedObject.FindProperty("Level");
            getReviewInfoOnLevelStart = serializedObject.FindProperty("GetReviewInfoOnLevelStart");

            CheckIfAndroidReviewPackageExist();

            if (androidReviewPackageFound && enabled.boolValue)
                TafraEditorUtility.AddDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
            else if (!androidReviewPackageFound)
                TafraEditorUtility.RemoveDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
        }

        public override void OnFocus()
        {
            CheckIfAndroidReviewPackageExist();

            if (androidReviewPackageFound && enabled.boolValue)
                TafraEditorUtility.AddDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
            else if (!androidReviewPackageFound)
                TafraEditorUtility.RemoveDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(enabled);

            if (EditorGUI.EndChangeCheck())
            {
                if (enabled.boolValue)
                {
                    if (androidReviewPackageFound)
                        TafraEditorUtility.AddDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
                    else
                        TafraEditorUtility.RemoveDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
                }
                else
                    TafraEditorUtility.RemoveDefiningSymbols(androidPackageDefningSymbol, BuildTargetGroup.Android);
            }

            if (enabled.boolValue)
            {
                BasicProgressManagerSettings progressSettings = TafraSettings.GetSettings_Editor<BasicProgressManagerSettings>();

                if (!progressSettings || !progressSettings.Enabled)
                {
                    EditorGUILayout.HelpBox("Progress Manager isn't enabled, you need to enable it in order to display review prompts.", MessageType.Error);
                }

                if (!androidReviewPackageFound)
                {
                    EditorGUILayout.HelpBox("Google Play Review package is not installed, review prompt will not be displayed on android devices.", MessageType.Error);

                    if (GUILayout.Button("Install Google Play Review Package"))
                        Application.OpenURL("https://developers.google.com/unity/packages#play_in-app_review");

                }
            }

            EditorGUI.BeginDisabledGroup(!enabled.boolValue);

            EditorGUILayout.PropertyField(level);
            EditorGUILayout.PropertyField(getReviewInfoOnLevelStart);

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        bool CheckIfAndroidReviewPackageExist()
        {
            string namespaceName = "Google.Play.Review";
            string className = "PlayReviewInfo";

            Type type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from typeX in assembly.GetTypes()
                         where typeX.Namespace == namespaceName && typeX.Name == className
                         select typeX).FirstOrDefault();

            androidReviewPackageFound = type != null;

            return androidReviewPackageFound;
        }
    }
}