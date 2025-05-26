using System;
using System.Collections;
using System.Text;
using TafraKit.Internal;
using TafraKit.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    public static class SplashScreen
    {
        private static SplashScreenSettings settings;
        private static SplashScreenExit exit;
        private static ControlReceiver controlReceiver;
        private static bool hasLoadBlocker;
        private static bool isInSplashScreen;

        public static bool IsInSplashScreen => isInSplashScreen;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<SplashScreenSettings>();

            if(settings == null || !settings.Enabled)
                return;

            controlReceiver = new ControlReceiver(OnFirstLoadBlockerAdded, null, OnAllLoadBlockersCleared);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if(scene.buildIndex != 0)
                return;

            isInSplashScreen = true;

            GeneralCoroutinePlayer.StartCoroutine(LoadAfterDelay());
        }
        private static void OnFirstLoadBlockerAdded()
        {
            hasLoadBlocker = true;
        }
        private static void OnAllLoadBlockersCleared()
        {
            hasLoadBlocker = false;
        }

        private static IEnumerator LoadAfterDelay()
        {
            //Wait two frames before waiting for realtime, since if we don't do this, the realtime will pass during startup.
            yield return null;
            yield return null;

            yield return Yielders.GetWaitForSecondsRealtime(4.5f);

            Debug.Log("TODO: Implement async loading for scenes.");

            SplashLoadingScreenCanvas loadingScreenPrefab = settings.LoadingScreenCanvas;

            if(loadingScreenPrefab != null)
                yield return GeneralCoroutinePlayer.StartCoroutine(LoadingProgress(loadingScreenPrefab));
            else
            {
                exit.Exit();
                isInSplashScreen = false;
            }
        }
        private static IEnumerator LoadingProgress(SplashLoadingScreenCanvas loadingScreenPrefab)
        {
            SplashLoadingScreenCanvas loadingScreen = GameObject.Instantiate(loadingScreenPrefab);

            loadingScreen.UIEG.ChangeVisibility(true);

            float duration = 1.5f;
            float startTime = Time.time;
            float endTime = startTime + duration;

            StringBuilder loadingSB = new StringBuilder();
            while(Time.time < endTime)
            {
                float t = (Time.time - startTime) / duration;

                loadingSB.Clear();
                loadingSB.Append(Mathf.RoundToInt(t * 100));
                loadingSB.Append('%');

                loadingScreen.LoadingBar.value = t;
                loadingScreen.LoadingProgressTXT.text = loadingSB.ToString();

                yield return null;
            }
            loadingScreen.LoadingBar.value = 1;
            loadingScreen.LoadingProgressTXT.text = "100%";

            //Temporarily fake the progress bar.
            //TODO: make it an actual progress bar.

            while(hasLoadBlocker)
                yield return null;

            exit.Exit();
            isInSplashScreen = false;
        }

        public static void SetExit(SplashScreenExit exit)
        {
            SplashScreen.exit = exit;
        }
        public static void AddExitBlocker(string blockerId)
        {
            TafraDebugger.Log("Splash Screen", $"Added load blocker {blockerId}", TafraDebugger.LogType.Info);

            controlReceiver.AddController(blockerId);
        }
        public static void RemoveExitBlocker(string blockerId)
        {
            TafraDebugger.Log("Splash Screen", $"Removed load blocker {blockerId}", TafraDebugger.LogType.Info);

            controlReceiver.RemoveController(blockerId);
        }
    }
}