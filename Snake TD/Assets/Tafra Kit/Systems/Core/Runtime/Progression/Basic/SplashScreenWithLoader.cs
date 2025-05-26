using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TafraKit.SceneManagement;
using TMPro;

namespace TafraKit
{
    public class SplashScreenWithLoader : MonoBehaviour
    {
        #region Private Serialized Fields
        [Header("References")]
        [Tooltip("Optional: a slider that will display loading process.")]
        [SerializeField] private Slider loadingSlider;
        [Tooltip("Optional: a text that will display loading process in persentage.")]
        [SerializeField] private TextMeshProUGUI loadingPercentTXT;
        [Tooltip("If there's a loading percentage text, what string should prefix the percentage? (e.g. if the prefix is \"Loading...\" and the current progress is 75% then the final loading percentage text will be: \"Loading...75%\".)")]
        [SerializeField] private string loadingPercentPrefix = "Loading...";

        [Header("Durations")]
        [Tooltip("The delay before actually starting to load the scene, if you'd like to give the player a moment to look at the splash screen before the bar starts moving.")]
        [SerializeField] private float delayBeforeStartLoading = 1f;
        [Tooltip("After the bar reaches the end, how long should we wait before the scene is loaded? In case you want to give the player a moment to see that it reached 100%.")]
        [SerializeField] private float delayBeforeGoingToNextScene = 0.25f;

        [Header("Bar Properties")]
        [Tooltip("The maximum speed the bar will be moving in regardless of how fast the scene was loaded. 0 means stick to the actual loading progress.")]
        [SerializeField] private float barSpeed = 1;

        [Header("Scene To Load")]
        [Tooltip("Should the scene be loaded right after the loading bar fills up?")]
        [SerializeField] private bool loadSceneOnFinish = true;
        [Tooltip("Should the opened level in progress manager (if any) be opened after loading?")]
        [SerializeField] private bool loadOpenedScene = true;
        [Tooltip("The build index of the scene to load if there were no opened level in progress manager or if load opened scene was disabled.")]
        [SerializeField] private int sceneToLoad;
        #endregion

        #region Public Events
        [Tooltip("Fires once the next scene is loaded and the progress bar is filled.")]
        public UnityEvent OnReadyToOpen;
        #endregion

        #region Private Fields
        private AsyncOperation asyncOp = new AsyncOperation();
        private bool isLoaded;
        private StringBuilder loadingPercentSB;
        #endregion

        #region MonoBehaviour Messages
        private IEnumerator Start()
        {
            if(loadingSlider)
                loadingSlider.value = 0;

            if(loadingPercentTXT)
            {
                loadingPercentSB = new StringBuilder(loadingPercentPrefix);
                loadingPercentSB.Append("0%");
                loadingPercentTXT.text = loadingPercentSB.ToString();
            }

            yield return Yielders.GetWaitForSeconds(delayBeforeStartLoading);

            sceneToLoad = 1;

            if (loadOpenedScene && BasicProgressManager.OpenedLevelNumber > -1)
                sceneToLoad = BasicProgressManager.GetOpenedLevelIndexInBuild();

            asyncOp = SceneManager.LoadSceneAsync(sceneToLoad);
            asyncOp.allowSceneActivation = false;

            float curProgress = 0;

            while (curProgress < 1)
            {
                float inverseLerp = Mathf.InverseLerp(0, 0.9f, asyncOp.progress);

                if(barSpeed > 0.001f)
                    curProgress = Mathf.MoveTowards(curProgress, inverseLerp, barSpeed * Time.deltaTime);
                else
                    curProgress = inverseLerp;

                if (loadingSlider)
                    loadingSlider.value = curProgress;

                if(loadingPercentTXT)
                {
                    loadingPercentSB.Clear();

                    int percent = Mathf.RoundToInt(curProgress * 100);

                    loadingPercentSB.Append(loadingPercentPrefix);
                    loadingPercentSB.Append(percent);
                    loadingPercentSB.Append('%');

                    loadingPercentTXT.text = loadingPercentSB.ToString();
                }

                yield return null;
            }

            if (loadingSlider)
                loadingSlider.value = 1f;

            if(loadingPercentTXT)
            {
                loadingPercentSB.Clear();

                loadingPercentSB.Append(loadingPercentPrefix);
                loadingPercentSB.Append("100%");

                loadingPercentTXT.text = loadingPercentSB.ToString();
            }

            isLoaded = true;

            yield return Yielders.GetWaitForSeconds(delayBeforeGoingToNextScene);

            OnReadyToOpen?.Invoke();

            if(loadSceneOnFinish)
                OpenIfLoaded();
        }
        #endregion

        public void OpenIfLoaded()
        {
            if(!isLoaded) return;

            TafraSceneManager.GetActiveTransition().In(() =>
            {
                asyncOp.allowSceneActivation = true;
                TafraSceneManager.GetActiveTransition().Out(null);
            }, false);
        }

        public void WaitUntilLoadedAndOpen()
        {
            StartCoroutine(WaitingUntilLoadedAndOpen());
        }
        private IEnumerator WaitingUntilLoadedAndOpen()
        {
            while (!isLoaded)
                yield return null;

            OpenIfLoaded();
        }
    }
}