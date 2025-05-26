using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TafraKitEditor
{
    public class TafraSceneNavigator : EditorWindow
    {
        private struct SceneData
        {
            public string name;
            public string path;

            public SceneData(string name, string path)
            {
                this.name = name;
                this.path = path;
            }
        }

        private GUIStyle sceneNameStyle;
        private GUIStyle scenePathsStyle;

        List<SceneData> inBuildScenes = new List<SceneData>();
        List<SceneData> outOfBuildScenes = new List<SceneData>();
        private GUIContent[] inBuildSceneNames;
        private GUIContent[] inBuildScenePathesButton;
        private GUIContent[] inBuildSceneRemoveButton;
        private GUIContent[] inBuildSceneMoveUpButton;
        private GUIContent[] inBuildSceneMoveDownButton;
        private GUIContent[] outOfBuildSceneNames;
        private GUIContent[] outOfBuildScenePathesButton;
        private GUIContent[] outOfBuildSceneAddToBuildButton;
        private int selectedInBuildSceneIndex;
        private int selectedOutOfBuildSceneIndex;
        private bool stylesInitialized;
        private Vector2 scrollPos;

        [MenuItem("Tafra Games/Windows/Scene Navigator", priority = 1)]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<TafraSceneNavigator>();
        }

        private void OnFocus()
        {
            string iconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_TafraSettings.png" : "Tafra Kit/Icons/TafraSettings.png";
            Texture icon = EditorGUIUtility.Load(iconPath) as Texture;
            titleContent = new GUIContent("Scene Navigator", icon);

            RefreshList();
        }

        private void OnDisable()
        {

        }

        private void Awake()
        {
        }

        private void OnGUI()
        {
            if (!stylesInitialized)
                InitializeStlyes();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.LabelField("Inside Build", EditorStyles.miniLabel);

            DrawScenesGrid(selectedInBuildSceneIndex, inBuildSceneNames, inBuildScenePathesButton, inBuildScenes, true);

            EditorGUILayout.LabelField("Outside Build", EditorStyles.miniLabel);

            DrawScenesGrid(selectedOutOfBuildSceneIndex, outOfBuildSceneNames, outOfBuildScenePathesButton, outOfBuildScenes, false);

            EditorGUILayout.EndScrollView();
        }

        void DrawScenesGrid(int selectedSceneIndex, GUIContent[] names, GUIContent[] paths, List<SceneData> scenes, bool inBuild)
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            selectedSceneIndex = GUILayout.SelectionGrid(selectedSceneIndex, names, 1, sceneNameStyle);

            if (EditorGUI.EndChangeCheck())
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenes[selectedSceneIndex].path);
                    if (inBuild)
                    {
                        selectedInBuildSceneIndex = selectedSceneIndex;
                        selectedOutOfBuildSceneIndex = -1;
                    }
                    else
                    {
                        selectedOutOfBuildSceneIndex = selectedSceneIndex;
                        selectedInBuildSceneIndex = -1;
                    }
                }
            }

            if(!inBuild)
            {
                #region Add To Scenes In Build Buttons
                int addToScenesInBuildGrid = -1;
                addToScenesInBuildGrid = GUILayout.SelectionGrid(addToScenesInBuildGrid, outOfBuildSceneAddToBuildButton, 1, scenePathsStyle, GUILayout.Width(30));

                if(addToScenesInBuildGrid > -1)
                {
                    EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(scenes[addToScenesInBuildGrid].path, true);

                    List<EditorBuildSettingsScene> scenesInBuild = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                    scenesInBuild.Add(sceneToAdd);

                    EditorBuildSettings.scenes = scenesInBuild.ToArray();

                    RefreshList();
                }
                #endregion
            }
            else
            {
                #region Move Up In Scenes In Build Buttons
                EditorGUI.BeginChangeCheck();
                selectedSceneIndex = GUILayout.SelectionGrid(selectedSceneIndex, inBuildSceneMoveUpButton, 1, scenePathsStyle, GUILayout.Width(30));

                if(EditorGUI.EndChangeCheck())
                    MoveBuildScene(scenes[selectedSceneIndex].path, true);

                #endregion

                #region Move Down In Scenes In Build Buttons
                EditorGUI.BeginChangeCheck();
                selectedSceneIndex = GUILayout.SelectionGrid(selectedSceneIndex, inBuildSceneMoveDownButton, 1, scenePathsStyle, GUILayout.Width(30));

                if(EditorGUI.EndChangeCheck())
                    MoveBuildScene(scenes[selectedSceneIndex].path, false);
                #endregion

                #region Remove From Scenes In Build Buttons
                EditorGUI.BeginChangeCheck();
                selectedSceneIndex = GUILayout.SelectionGrid(selectedSceneIndex, inBuildSceneRemoveButton, 1, scenePathsStyle, GUILayout.Width(30));

                if(EditorGUI.EndChangeCheck())
                {
                    string sceneToRemovePath = scenes[selectedSceneIndex].path;

                    List<EditorBuildSettingsScene> scenesInBuild = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

                    for(int i = 0; i < scenesInBuild.Count; i++)
                    {
                        if(scenesInBuild[i].path == sceneToRemovePath)
                        {
                            scenesInBuild.RemoveAt(i);
                            break;
                        }
                    }

                    EditorBuildSettings.scenes = scenesInBuild.ToArray();

                    RefreshList();
                }
                #endregion
            }

            #region Scene Path Buttons
            EditorGUI.BeginChangeCheck();
            selectedSceneIndex = GUILayout.SelectionGrid(selectedSceneIndex, paths, 1, scenePathsStyle, GUILayout.Width(30));

            if (EditorGUI.EndChangeCheck())
            {
                Object sceneObject = AssetDatabase.LoadMainAssetAtPath(scenes[selectedSceneIndex].path);

                EditorGUIUtility.PingObject(sceneObject);
            }
            #endregion

            EditorGUILayout.EndHorizontal();
        }

        void RefreshList()
        {
            inBuildScenes.Clear();
            outOfBuildScenes.Clear();

            inBuildSceneNames = new GUIContent[EditorBuildSettings.scenes.Length];
            inBuildScenePathesButton = new GUIContent[EditorBuildSettings.scenes.Length];
            inBuildSceneRemoveButton = new GUIContent[EditorBuildSettings.scenes.Length];
            inBuildSceneMoveUpButton = new GUIContent[EditorBuildSettings.scenes.Length];
            inBuildSceneMoveDownButton = new GUIContent[EditorBuildSettings.scenes.Length];
            for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];

                string sceneName = scene.path.Substring(scene.path.LastIndexOf('/') + 1).Replace(".unity", "");
                inBuildSceneNames[i] = new GUIContent(sceneName);
                inBuildScenePathesButton[i] = new GUIContent("#");
                inBuildSceneRemoveButton[i] = new GUIContent("x");
                if (i != 0)
                    inBuildSceneMoveUpButton[i] = new GUIContent("↑");
                else
                    inBuildSceneMoveUpButton[i] = new GUIContent("");

                if(i != EditorBuildSettings.scenes.Length - 1)
                    inBuildSceneMoveDownButton[i] = new GUIContent("↓");
                else
                    inBuildSceneMoveDownButton[i] = new GUIContent("");

                inBuildScenes.Add(new SceneData(sceneName, scene.path));
            }

            string[] sceneFiles = AssetDatabase.FindAssets("t:scene");
            for(int i = 0; i < sceneFiles.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(sceneFiles[i]);
                string sceneName = path.Substring(path.LastIndexOf('/') + 1).Replace(".unity", "");

                bool inBuild = false;
                for(int j = 0; j < inBuildScenes.Count; j++)
                {
                    if(path == inBuildScenes[j].path)
                    {
                        inBuild = true;
                        break;
                    }
                }

                if(inBuild) continue;

                outOfBuildScenes.Add(new SceneData(sceneName, path));
            }

            outOfBuildSceneNames = new GUIContent[outOfBuildScenes.Count];
            outOfBuildScenePathesButton = new GUIContent[outOfBuildScenes.Count];
            outOfBuildSceneAddToBuildButton = new GUIContent[outOfBuildScenes.Count];
            for(int i = 0; i < outOfBuildScenes.Count; i++)
            {
                outOfBuildSceneNames[i] = new GUIContent(outOfBuildScenes[i].name);
                outOfBuildScenePathesButton[i] = new GUIContent("#");
                outOfBuildSceneAddToBuildButton[i] = new GUIContent("+");
            }

            int openedSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

            if(openedSceneBuildIndex > -1)
            {
                selectedInBuildSceneIndex = openedSceneBuildIndex;
                selectedOutOfBuildSceneIndex = -1;
            }
            else
            {
                selectedInBuildSceneIndex = -1;

                string openedScenePath = SceneManager.GetActiveScene().path;
                for(int i = 0; i < outOfBuildSceneNames.Length; i++)
                {
                    if(outOfBuildScenes[i].path == openedScenePath)
                        selectedOutOfBuildSceneIndex = i;
                }
            }
        }
        void InitializeStlyes()
        {
            if (stylesInitialized)
                return;
            sceneNameStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };
            scenePathsStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };

            stylesInitialized = true;
        }
        void MoveBuildScene(string scenePath, bool up)
        {
            List<EditorBuildSettingsScene> scenesInBuild = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int indexOfScene = 0;
            for(int i = 0; i < scenesInBuild.Count; i++)
            {
                if(scenesInBuild[i].path == scenePath)
                {
                    indexOfScene = i;
                    break;
                }
            }

            if((up && indexOfScene > 0) || (!up && indexOfScene < scenesInBuild.Count -1))
            {
                EditorBuildSettingsScene sceneToMoveUp = scenesInBuild[indexOfScene];
                scenesInBuild.RemoveAt(indexOfScene);
                scenesInBuild.Insert(indexOfScene + (up ? -1 : 1), sceneToMoveUp);

                EditorBuildSettings.scenes = scenesInBuild.ToArray();

                RefreshList();
            }
        }
    }
}