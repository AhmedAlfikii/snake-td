using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TafraKit.SceneManagement
{
    public class SceneTransition : MonoBehaviour
    {
        [Header("Async Loading Properties")]
        [SerializeField] protected float delayBeforeLoading = 0.5f;
        [SerializeField] protected float barCatchingSpeed = 1f;
        [SerializeField] protected float fakeLoadingSpeed = 0.25f;
        [SerializeField, Range(0, 1)] protected float minimumBarValue = 0;

        public UnityEvent OnInStarted;
        public UnityEvent OnInFinished;
        public UnityEvent OnOutStarted;
        public UnityEvent OnOutFinished;

        public void In(Action onFinished, bool loadAsync, int sceneIndex = -1)
        {
            OnInStarted?.Invoke();

            if (loadAsync)
            {
                OnLoadingBarProgressUpdate(minimumBarValue);
                StartCoroutine(LoadingOperation(sceneIndex));
            }
            else
                OnLoadingBarProgressUpdate(1);

            OnIn(onFinished, loadAsync, sceneIndex);
        }

        public void Out(Action onFinished)
        {
            OnOutStarted?.Invoke();

            OnOut(onFinished);
        }

        protected virtual void OnIn(Action onFinished, bool loadAsync, int sceneIndex = -1)
        {
            OnInFinished?.Invoke();
            onFinished?.Invoke();
        }
        protected virtual void OnOut(Action onFinished)
        {
            OnOutFinished?.Invoke();
            onFinished?.Invoke();
        }

        protected IEnumerator LoadingOperation(int sceneIndex)
        {
            yield return Yielders.GetWaitForSeconds(delayBeforeLoading);

            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);

            loadingOperation.allowSceneActivation = false;

            float barTargetProgress = 0;
            float barCurrentProgress = 0;
            float fakeLoading = minimumBarValue;
            while (barCurrentProgress < 1f)
            {
                barTargetProgress = loadingOperation.progress >= 0.9f ? 1 : loadingOperation.progress * 0.75f;
                fakeLoading = Mathf.MoveTowards(fakeLoading, 0.8f, fakeLoadingSpeed * Time.deltaTime);

                barCurrentProgress = Mathf.MoveTowards(barCurrentProgress, Mathf.Max(fakeLoading, barTargetProgress), barCatchingSpeed * Time.deltaTime);

                OnLoadingBarProgressUpdate(Mathf.Max(minimumBarValue, barCurrentProgress));
                yield return null;
            }
            loadingOperation.allowSceneActivation = true;

            while(!loadingOperation.isDone)
            {
                yield return null;
            }

            Out(null);
        }

        protected virtual void OnLoadingBarProgressUpdate(float progress)
        { 

        }
    }
}