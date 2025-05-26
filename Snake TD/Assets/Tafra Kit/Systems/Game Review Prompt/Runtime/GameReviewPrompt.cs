using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
#if UNITY_ANDROID
#if TAFRA_REVIEW_PROMPT
using Google.Play.Review;
using Google.Play.Common;
#endif
#endif

namespace TafraKit.ReviewPrompt
{
    public class GameReviewPrompt
    {
        private static GameReviewPromptSettings settings;

        private static bool promptShown;

        #if UNITY_ANDROID
        private static GameReviewPromptMonoBridge monoBridge;
        #if TAFRA_REVIEW_PROMPT
        private static ReviewManager reviewManager;
        private static PlayReviewInfo playReviewInfo;
        #endif
        private static bool gettingReviewInfo;
        #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<GameReviewPromptSettings>();
            //ProgressManagerSettings progressSettings = TafraSettings.GetSettings<ProgressManagerSettings>();

            if (settings)
            {
                if (settings.Enabled)
                {
                    #if UNITY_ANDROID
                    #if TAFRA_REVIEW_PROMPT
                    reviewManager = new ReviewManager();
                    #endif
                    monoBridge = new GameObject("GameReviewPromptMonoBridge", typeof(GameReviewPromptMonoBridge)).GetComponent<GameReviewPromptMonoBridge>();
                    GameObject.DontDestroyOnLoad(monoBridge);
                    #endif

                    promptShown = PlayerPrefs.GetInt("TAFRAKIT_REVIEW_PROMPT_SHOWN") == 1;

                    //if (!promptShown && progressSettings != null && progressSettings.Enabled)
                    //{
                    //    ProgressManager.OnLevelStarted.AddListener(OnLevelStarted);
                    //    ProgressManager.OnLevelCompleted.AddListener(OnLevelCompleted);
                    //}
                }
            }
        }

        private static void OnLevelStarted()
        {
            #if UNITY_ANDROID
            if (BasicProgressManager.OpenedLevelNumber == settings.Level)
            {
                #if TAFRA_REVIEW_PROMPT
                if (settings.GetReviewInfoOnLevelStart)
                    GetPlayReviewInfo();
                #endif

                TafraDebugger.Log("Game Review Prompt","This is the level where game review prompt should be displayed.", TafraDebugger.LogType.Verbose);
            }
            #endif
        }
        private static void OnLevelCompleted()
        {
            if (settings.Level > 0 && BasicProgressManager.OpenedLevelNumber >= settings.Level)
            {
                ShowPrompt();
                TafraDebugger.Log("Game Review Prompt", "Game review prompt should be displayed now.", TafraDebugger.LogType.Verbose);
            }
        }

        #if TAFRA_REVIEW_PROMPT
        /// <summary>
        /// Get the Play Review Info object in preparation to display it later.
        /// </summary>
        /// <param name="bypassLevel">If true, the object will be fetched even if the current level isn't the review level set in Tafra Kit settings.</param>
        public static void GetPlayReviewInfo(bool bypassLevel = false)
        {
            #if UNITY_ANDROID
            if (bypassLevel || settings.Level <= 0 || ProgressManager.OpenedLevelNumber == settings.Level)
                monoBridge.StartCoroutine(GettingPlayReviewInfo());
            else
                TafraDebugger.Log("Game Review Prompt","Can't get play review info; this isnt't the review level.", TafraDebugger.LogType.Info);
            #endif
        }
        #if UNITY_ANDROID
        private static IEnumerator GettingPlayReviewInfo()
        {
            gettingReviewInfo = true;
           
            var requestFlowOperation = reviewManager.RequestReviewFlow();

            yield return requestFlowOperation;

            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                TafraDebugger.Log("Game Review Prompt", "Error Fetching PlayReviewInfo: " + requestFlowOperation.Error.ToString(), TafraDebugger.LogType.Info);

                gettingReviewInfo = false;
                yield break;
            }

            TafraDebugger.Log("Game Review Prompt", "Successfully fetched PlayReviewInfo.", TafraDebugger.LogType.Verbose);

            playReviewInfo = requestFlowOperation.GetResult();

            gettingReviewInfo = false;
        }
        private static IEnumerator LaunchPlayReview()
        {
            TafraDebugger.Log("Game Review Prompt", "Attempting to launch review flow.", TafraDebugger.LogType.Verbose);

            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);

            yield return launchFlowOperation;

            playReviewInfo = null; // Reset the object

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                TafraDebugger.Log("Game Review Prompt", "Error launching review flow " + launchFlowOperation.Error.ToString(), TafraDebugger.LogType.Info);
                yield break;
            }

            PlayerPrefs.SetInt("TAFRAKIT_REVIEW_PROMPT_SHOWN", 1);
            promptShown = true;
            //StopListeningToProgressManager();

            TafraDebugger.Log("Game Review Prompt", "End launching review flow." + launchFlowOperation.Error.ToString(), TafraDebugger.LogType.Verbose);
        }
        #endif
        #endif
        private static void StopListeningToProgressManager()
        {
            BasicProgressManager.OnLevelStarted.RemoveListener(OnLevelStarted);
            BasicProgressManager.OnLevelCompleted.RemoveListener(OnLevelCompleted);
        }

        public static void ShowPrompt()
        {
            if (!settings.Enabled) return;

#if UNITY_IOS
            Device.RequestStoreReview();
            PlayerPrefs.SetInt("REVIEW_PROMPT_SHOWN", 1);
            promptShown = true;
            StopListeningToProgressManager();
#elif UNITY_ANDROID
#if TAFRA_REVIEW_PROMPT
            if (playReviewInfo != null)
            {
                monoBridge.StartCoroutine(LaunchPlayReview());
            }
#endif
#endif
        }

    }
}