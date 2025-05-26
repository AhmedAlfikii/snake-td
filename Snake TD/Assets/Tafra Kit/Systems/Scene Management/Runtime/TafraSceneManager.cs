using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit.SceneManagement
{
    public static class TafraSceneManager
    {
        private static SceneManagerSettings settings;

        private static List<SceneTransition> activeTransitions = new List<SceneTransition>();
        private static Dictionary<string, object> loadSceneParameters = new Dictionary<string, object>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<SceneManagerSettings>();
            
            if (settings)
            {
                if (settings.DefaultTransition != null)
                    RegisterSceneTransition(settings.DefaultTransition);
            }
        }

        private static IEnumerator OutTransitionDelayed(SceneTransition transition)
        {
            yield return null;

            transition.Out(null);
        }

        #region Public Functions
        public static void LoadScene(int sceneBuildIndex, LoadSceneMode loadMode = LoadSceneMode.Single, Action onLoaded = null, params KeyValuePair<string, object>[]  loadParams)
        {
            loadSceneParameters.Clear();

            for(int i = 0; i < loadParams.Length; i++)
            {
                loadSceneParameters.TryAdd(loadParams[i].Key, loadParams[i].Value);
            }

            if(activeTransitions.Count > 0)
            {
                SceneTransition transition = activeTransitions[0];

                if(transition != null)
                {
                    transition.In(() =>
                    {
                        SceneManager.LoadScene(sceneBuildIndex, loadMode);

                        onLoaded?.Invoke();

                        GeneralCoroutinePlayer.StartCoroutine(OutTransitionDelayed(transition));
                    }, false);
                }
                else
                {
                    SceneManager.LoadScene(sceneBuildIndex, loadMode);
                    onLoaded?.Invoke();
                }
            }
            else
            {
                SceneManager.LoadScene(sceneBuildIndex, loadMode);
                onLoaded?.Invoke();
            }
        }
        public static void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, Action onLoaded = null, params KeyValuePair<string, object>[] loadParams)
        {
            loadSceneParameters.Clear();

            for (int i = 0; i < loadParams.Length; i++)
            {
                loadSceneParameters.TryAdd(loadParams[i].Key, loadParams[i].Value);
            }

            if(activeTransitions.Count > 0)
            {
                SceneTransition transition = activeTransitions[0];

                if(transition != null)
                {
                    transition.In(() =>
                    {
                        SceneManager.LoadScene(sceneName, loadMode);

                        onLoaded?.Invoke();

                        GeneralCoroutinePlayer.StartCoroutine(OutTransitionDelayed(transition));
                    }, false);
                }
                else
                {
                    SceneManager.LoadScene(sceneName, loadMode);
                    onLoaded?.Invoke();
                }
            }
            else
            {
                SceneManager.LoadScene(sceneName, loadMode);
                onLoaded?.Invoke();
            }
        }

        public static void LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadMode = LoadSceneMode.Single, params KeyValuePair<string, object>[] loadParams)
        {
            loadSceneParameters.Clear();

            for(int i = 0; i < loadParams.Length; i++)
            {
                loadSceneParameters.Add(loadParams[i].Key, loadParams[i].Value);
            }

            if(activeTransitions.Count > 0)
            {
                SceneTransition transition = activeTransitions[0];

                if (transition != null)
                    transition.In(null, true, sceneBuildIndex);
                else
                    SceneManager.LoadScene(sceneBuildIndex, loadMode);
            }
            else
                SceneManager.LoadScene(sceneBuildIndex, loadMode);
        }
        public static void UnloadScenesAsync(List<string> sceneNames, Action onUnloaded = null)
        {
            if(activeTransitions.Count > 0)
            {
                SceneTransition transition = activeTransitions[0];

                if(transition != null)
                    transition.In(() =>
                    {
                        for (int i = 0; i < sceneNames.Count; i++)
                        {
                            SceneManager.UnloadSceneAsync(sceneNames[i]);
                        }

                        onUnloaded?.Invoke();

                        transition.Out(null);
                    }, false);
                else
                {
                    for(int i = 0; i < sceneNames.Count; i++)
                    {
                        SceneManager.UnloadSceneAsync(sceneNames[i]);
                    }
                    onUnloaded?.Invoke();
                }
            }
            else
            {
                for(int i = 0; i < sceneNames.Count; i++)
                {
                    SceneManager.UnloadSceneAsync(sceneNames[i]);
                }
                onUnloaded?.Invoke();
            }
        }

        public static void ReloadOpenedScene(params KeyValuePair<string, object>[] loadParams)
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single, null, loadParams);
        }

        /// <summary>
        /// Register a scene transition in the transitions list, this will be the first transition in the list, and it will be used for transitions until unregistered or another transition was registered.
        /// </summary>
        /// <param name="transitionPrefab"></param>
        /// <returns>The transition object that was registered.</returns>
        public static SceneTransition RegisterSceneTransition(GameObject transitionPrefab)
        {
            if (transitionPrefab == null)
            {
                TafraDebugger.Log("Scene Manager", "The transition you're attempting to register is null.", TafraDebugger.LogType.Error);
                return null;
            }

            SceneTransition prefabTransition = transitionPrefab.GetComponent<SceneTransition>();

            if (prefabTransition == null)
            {
                TafraDebugger.Log("Scene Manager", "The transition game object you're attempting to register does not have a transition script attached to it.", TafraDebugger.LogType.Error);
                return null;
            }

            if (prefabTransition)
            {
                GameObject transitionGO = GameObject.Instantiate(transitionPrefab);

                MonoBehaviour.DontDestroyOnLoad(transitionGO);

                SceneTransition transition = transitionGO.GetComponent<SceneTransition>();

                activeTransitions.Insert(0, transition);

                return transition;
            }
            else
                return null;
        }

        /// <summary>
        /// Remove a transition from the transitions list.
        /// </summary>
        /// <param name="transition"></param>
        public static void UnregisterSceneTransition(SceneTransition transition)
        {
            int transitionIndex = activeTransitions.IndexOf(transition);

            if (transitionIndex > -1)
                UnregisterSceneTransition(transitionIndex);
            else
                TafraDebugger.Log("Scene Manager", "The transition you're trying to unregister isn't in the active transitions list.", TafraDebugger.LogType.Info);
        }
        /// <summary>
        /// Remove a transition from the transitions list.
        /// </summary>
        /// <param name="transitionIndex"></param>
        public static void UnregisterSceneTransition(int transitionIndex)
        {
            if (transitionIndex >= 0 && transitionIndex < activeTransitions.Count)
            {
                MonoBehaviour.Destroy(activeTransitions[transitionIndex]);
                activeTransitions.RemoveAt(transitionIndex);
                TafraDebugger.Log("Scene Manager", $"Unregistered transitoin (index: {transitionIndex}).", TafraDebugger.LogType.Verbose);
            }
            else
                TafraDebugger.Log("Scene Manager", $"The index of the transition you're trying to unregister ({transitionIndex}) isn't in the range of the active transitions list (count: {activeTransitions.Count}).", TafraDebugger.LogType.Info);
        }

        public static SceneTransition GetActiveTransition()
        {
            if (activeTransitions.Count == 0) return null;

            return activeTransitions[0];
        }
        public static Dictionary<string, object> GetLoadParameters()
        {
            return loadSceneParameters;
        }
        #endregion
    }
}