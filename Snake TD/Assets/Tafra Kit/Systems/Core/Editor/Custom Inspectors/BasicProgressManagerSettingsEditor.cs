using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(BasicProgressManagerSettings))]
    public class BasicProgressManagerSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty progressionEnabled;
        private SerializedProperty level1IndexInBuild;
        private SerializedProperty loopLevelsProperty;
        private SerializedProperty snapPlayerToNewLatestLevelIfLoopedProperty;
        private SerializedProperty changeLoopStartLevel;
        private SerializedProperty loopStartLevel;
        private SerializedProperty openLevel1ByDefault;
        private SerializedProperty autoLoadAtSplash;
        private SerializedProperty increaseAtWinScreen;
        private SerializedProperty editorForceLatestLevel;
        private SerializedProperty editorEnableShortcuts;

        private bool foundSplash;
        private int splashSceneIndex;

        private void OnEnable()
        {
            progressionEnabled = serializedObject.FindProperty("Enabled");
            level1IndexInBuild = serializedObject.FindProperty("Level1IndexInBuild");
            loopLevelsProperty = serializedObject.FindProperty("LoopLevels");
            snapPlayerToNewLatestLevelIfLoopedProperty = serializedObject.FindProperty("SnapPlayerToNewLatestLevelIfLooped");
            changeLoopStartLevel = serializedObject.FindProperty("ChangeLoopStartLevel");
            loopStartLevel = serializedObject.FindProperty("LoopStartLevel");
            openLevel1ByDefault = serializedObject.FindProperty("OpenLevel1ByDefault");
            autoLoadAtSplash = serializedObject.FindProperty("AutoLoadAtSplash");
            increaseAtWinScreen = serializedObject.FindProperty("IncreaseAtWinScreen");
            editorForceLatestLevel = serializedObject.FindProperty("EditorForceLoadLatestLevel");
            editorEnableShortcuts = serializedObject.FindProperty("EditorEnableShortcuts");

            CheckSplashScreen();
        }
        public override void OnFocus()
        {
            CheckSplashScreen();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int totalLevelsFound = SceneManager.sceneCountInBuildSettings - level1IndexInBuild.intValue;
           
            EditorGUILayout.HelpBox("Make sure that after the first level in the \"Scenes In Build\" window, all the following scenes are level scenes that are correctly ordered. All the other scenes (splash scene, main menu scene, etc...) should be added before the first level in the scenes in build.", MessageType.Info);

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(progressionEnabled);
            
            EditorGUI.BeginDisabledGroup(!progressionEnabled.boolValue);

            EditorGUILayout.PropertyField(level1IndexInBuild);

            if (level1IndexInBuild.intValue < 0)
                EditorGUILayout.HelpBox("Level index can't be a negative value.", MessageType.Error);
            else if (SceneManager.sceneCountInBuildSettings <= level1IndexInBuild.intValue)
                EditorGUILayout.HelpBox("The index you've specified for level one does not exist in the scenes in build, are you sure you've added it?", MessageType.Error);
            else if (foundSplash && splashSceneIndex == level1IndexInBuild.intValue)
                EditorGUILayout.HelpBox($"Your first level can't have the same index as the splash scene.", MessageType.Error);
           
            EditorGUILayout.PropertyField(loopLevelsProperty);
            if (loopLevelsProperty.boolValue)
            {
                EditorGUILayout.PropertyField(changeLoopStartLevel);
                EditorGUILayout.PropertyField(snapPlayerToNewLatestLevelIfLoopedProperty);
                if (changeLoopStartLevel.boolValue)
                {
                    EditorGUILayout.IntSlider(loopStartLevel, 1, totalLevelsFound);
                    if (loopStartLevel.intValue < 1)
                        EditorGUILayout.HelpBox("Loop start level can't be less than 1 (note that this is a number not an index).", MessageType.Error);
                    else if (loopStartLevel.intValue > totalLevelsFound)
                        EditorGUILayout.HelpBox($"Level {loopStartLevel.intValue} doesn't exist in the scenes in build. You only have {totalLevelsFound} levels.", MessageType.Error);
                }
            }

            EditorGUILayout.PropertyField(openLevel1ByDefault);
            EditorGUILayout.PropertyField(autoLoadAtSplash);

            if (!autoLoadAtSplash.boolValue)
                EditorGUILayout.HelpBox($"You'll have to manually call \"LoadLatestLevel()\" in the ProgressManager script when you wish to load the saved level.", MessageType.Info);

            EditorGUILayout.PropertyField(increaseAtWinScreen);
            
            EditorGUILayout.LabelField("Editor Only", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(editorForceLatestLevel, new GUIContent("Force Load Latest Level"));
            EditorGUILayout.PropertyField(editorEnableShortcuts, new GUIContent("Enable Shortcuts"));

            #region Splash Error Handling
            if (!foundSplash || splashSceneIndex != 0)
            {
                if (!foundSplash)
                    EditorGUILayout.HelpBox($"No splash scene detected, you must have a splash scene in order for progression to work.", MessageType.Error);
                else if (splashSceneIndex != 0)
                    EditorGUILayout.HelpBox($"The splash scene was misplaced, it should be placed at index 0 in the scenes in build list.", MessageType.Error);

                if (GUILayout.Button("Fix Now"))
                {
                    CheckSplashScreen();

                    if (!foundSplash)
                    {
                        string path = EditorUtility.SaveFilePanelInProject("Save Splash Scene", "Splash", "unity", "Please don't change the scene name.");
                        if (path.Length != 0)
                        {
                            Scene splashScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
                            EditorSceneManager.SaveScene(splashScene, path);
                            EditorSceneManager.CloseScene(splashScene, true);

                            EditorBuildSettingsScene[] scenesInBuild = EditorBuildSettings.scenes;
                            EditorBuildSettingsScene splashSceneBuildSettings = new EditorBuildSettingsScene(path, true);

                            List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>();

                            newScenes.Add(splashSceneBuildSettings);
                            newScenes.AddRange(scenesInBuild);

                            EditorBuildSettings.scenes = newScenes.ToArray();

                            foundSplash = true;
                            splashSceneIndex = 0;
                        }
                    }
                    else if (splashSceneIndex != 0)
                    {
                        List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                      
                        EditorBuildSettingsScene splashScene = newScenes[splashSceneIndex];
                    
                        newScenes.RemoveAt(splashSceneIndex);
                        newScenes.Insert(0, splashScene);

                        EditorBuildSettings.scenes = newScenes.ToArray();
                     
                        splashSceneIndex = 0;
                    }
                }
            }
            #endregion

            if (SceneManager.sceneCountInBuildSettings > level1IndexInBuild.intValue)
            {
                string msg = $"Total levels found: {totalLevelsFound}.";

                if (loopLevelsProperty.boolValue)
                {
                    int loopingLevels = totalLevelsFound;

                    if (changeLoopStartLevel.boolValue)
                        loopingLevels = (totalLevelsFound - loopStartLevel.intValue) + 1;

                    msg += $"\nLooping levels: {loopingLevels}.";
                }

                EditorGUILayout.HelpBox(msg, MessageType.Info);
            }

            #region Scene Name Fixing
            string levelNamePostFix = "Level ";
            bool faultyNames = false;

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int level1Index = level1IndexInBuild.intValue;

            for(int i = level1Index; i < scenes.Length; i++)
            {
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[i].path);
                string correctName = levelNamePostFix + ((i - level1Index) + 1);

                if(scene.name != correctName)
                    faultyNames = true;
            }


            if(faultyNames && GUILayout.Button("Fix Names"))
            {
                //Change all the scenes to random names to make sure changing them to final names cause conflict during the process (if a scene is already named exactly like another scene should be named).
                for(int i = level1Index; i < scenes.Length; i++)
                {
                    string output = AssetDatabase.RenameAsset(scenes[i].path, Random.value.ToString() + "_" + i);
                }
                
                scenes = EditorBuildSettings.scenes;

                for(int i = level1Index; i < scenes.Length; i++)
                {
                    string correctName = levelNamePostFix + ((i - level1Index) + 1);
                    string output = AssetDatabase.RenameAsset(scenes[i].path, correctName);
                    Debug.Log(output);
                }

            }
            #endregion

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        void CheckSplashScreen()
        {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path.EndsWith("Splash.unity"))
                {
                    foundSplash = true;
                    splashSceneIndex = i;
                    break;
                }
            }
        }
    }
}