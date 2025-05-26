using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.SceneManagement
{
    /// <summary>
    /// For transitions that are based on a UIElementsGroup.
    /// </summary>
    public class SceneTransitionUIEG : SceneTransition
    {
        #region Private Serialized Fields
        [SerializeField] private UIElementsGroup myUIEG;

        [Header("Loading Bar")]
        [SerializeField] private Slider loadingBar;
        #endregion

        #region Private Fields
        private IEnumerator visibilityEnum;
        private float hideDuration;
        private float showDuration;
        private bool isFullyVisible;
        #endregion

        public bool IsFullyVisible => isFullyVisible;

        #region MonoBehaviour Messages
        void Start()
        {
            hideDuration = myUIEG.GetAllHidingTime();
            showDuration = myUIEG.GetAllShowingTime();
        }
        #endregion

        #region Private Functions
        IEnumerator VisibilityChanging(float waitDuration, Action onFinished)
        {
            yield return Yielders.GetWaitForSecondsRealtime(waitDuration);

            onFinished?.Invoke();
        }
        protected override void OnLoadingBarProgressUpdate(float progress)
        {
            if (loadingBar)
                loadingBar.value = progress;
        }
        #endregion

        #region Public Functions
        protected override void OnIn(Action onFinished, bool loadAsync, int sceneIndex = -1)
        {
            isFullyVisible = false;
            myUIEG.ChangeVisibility(true);

            if (visibilityEnum != null)
                StopCoroutine(visibilityEnum);

            visibilityEnum = VisibilityChanging(showDuration, () => {
                isFullyVisible = true;
                onFinished?.Invoke();
                OnInFinished?.Invoke();
            });
            StartCoroutine(visibilityEnum);
        }

        protected override void OnOut(Action onFinished)
        {
            myUIEG.ChangeVisibility(false);

            if (visibilityEnum != null)
                StopCoroutine(visibilityEnum);

            visibilityEnum = VisibilityChanging(hideDuration, ()=> {
                onFinished?.Invoke();
                OnOutFinished?.Invoke();
            });
            StartCoroutine(visibilityEnum);
        }
        #endregion
    }
}