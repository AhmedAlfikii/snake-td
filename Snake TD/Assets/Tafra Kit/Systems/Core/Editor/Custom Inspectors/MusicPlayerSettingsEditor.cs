using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(MusicPlayerSettings))]
    public class MusicPlayerSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty enabled;
        private SerializedProperty defaultTrack;
        private SerializedProperty defaultTrackCenterScene;
        private SerializedProperty defaultTackPlayPointAroundScene;
        private SerializedProperty defaultTrackStartVolume;
        private SerializedProperty defaultTrackFadeInDuration;
        private SerializedProperty volumeScale;
        private SerializedProperty mutedByDefault;

        private GenericMenu scenesMenu;
        private string[] sceneNames;

        private void OnEnable()
        {
            enabled = serializedObject.FindProperty("Enabled");
            defaultTrack = serializedObject.FindProperty("DefaultTrack");
            defaultTrackCenterScene = serializedObject.FindProperty("DefaultTrackCenterScene");
            defaultTackPlayPointAroundScene = serializedObject.FindProperty("DefaultTackPlayPointAroundScene");
            defaultTrackStartVolume = serializedObject.FindProperty("DefaultTrackStartVolume");
            defaultTrackFadeInDuration = serializedObject.FindProperty("DefaultTrackFadeInDuration");
            volumeScale = serializedObject.FindProperty("VolumeScale");
            mutedByDefault = serializedObject.FindProperty("MutedByDefault");

            UpdateScenesList();
        }

        public override void OnFocus()
        {
            UpdateScenesList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(enabled);

            EditorGUI.BeginDisabledGroup(!enabled.boolValue);

            EditorGUILayout.PropertyField(defaultTrack);

            if (defaultTrack.objectReferenceValue != null)
            {
                int defaultIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                defaultTrackCenterScene.intValue = EditorGUILayout.Popup(new GUIContent("Center Scene", defaultTrackCenterScene.tooltip), defaultTrackCenterScene.intValue + 2, sceneNames) - 2;

                if (defaultTrackCenterScene.intValue > -1)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(defaultTackPlayPointAroundScene, new GUIContent("Play Point", defaultTackPlayPointAroundScene.tooltip));

                    EditorGUI.indentLevel--;
                }

                if (defaultTrackCenterScene.intValue > -2)
                {
                    EditorGUILayout.PropertyField(defaultTrackStartVolume, new GUIContent("Volume", defaultTrackStartVolume.tooltip));
                    EditorGUILayout.PropertyField(defaultTrackFadeInDuration, new GUIContent("Fade-in Duration", defaultTrackFadeInDuration.tooltip));
                }

                EditorGUI.indentLevel = defaultIndent;
            }

            EditorGUILayout.PropertyField(volumeScale);
            EditorGUILayout.PropertyField(mutedByDefault);

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        void UpdateScenesList()
        {
            List<string> names = new List<string>();
            if (EditorBuildSettings.scenes.Length == 0)
            {
                names.Add("! NO SCENES IN BUILD !");
                sceneNames = names.ToArray();
                defaultTrackCenterScene.intValue = -2;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            names.Add("None");
            names.Add("Any Scene");

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string sceneName = EditorBuildSettings.scenes[i].path;
                int nameStartIndex = sceneName.LastIndexOf("/") + 1;
                sceneName = sceneName.Substring(nameStartIndex, sceneName.Length - nameStartIndex - 6);
                names.Add($"{i} ({sceneName})");
            }

            sceneNames = names.ToArray();
        }
    }
}