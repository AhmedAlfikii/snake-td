using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Ads
{
    public class InterstitialScene : MonoBehaviour
    {
        private const string prefsKey_initialInterstitialPassedTime = "INITIAL_INTERSITIAL_PASSED_TIME";

        [SerializeField] private string sourceName;

        [Space()]

        [SerializeField] private float initialInterstitialDelay = 20f;
        [SerializeField] private float interstitialInterval = 10f;
        [SerializeField] private bool resetTimerOnRVUse = true;

        private float initialInterstitialPassedTime;
        private float interstitialTimer;
        private bool passedInitialInterstitial;
        private bool intersitialIsBeingDisplayed;

        #region MonoBehaviour Messages
        private void OnEnable()
        {
            if (resetTimerOnRVUse)
                TafraAds.OnRewardedVideoShow.AddListener(OnRewardedVideoShow);
        }

        private void OnDisable()
        {
            if (resetTimerOnRVUse)
                TafraAds.OnRewardedVideoShow.RemoveListener(OnRewardedVideoShow);

            if (!passedInitialInterstitial)
                PlayerPrefs.SetFloat(prefsKey_initialInterstitialPassedTime, initialInterstitialPassedTime);
        }

        private void Start()
        {
            initialInterstitialPassedTime = PlayerPrefs.GetFloat(prefsKey_initialInterstitialPassedTime);

            if (initialInterstitialPassedTime >= initialInterstitialDelay)
                passedInitialInterstitial = true;
        }

        private void Update()
        {
            if (intersitialIsBeingDisplayed)
                return;

            interstitialTimer += Time.deltaTime;

            if(!passedInitialInterstitial)
            {
                initialInterstitialPassedTime += Time.deltaTime;

                if (initialInterstitialPassedTime > initialInterstitialDelay)
                {
                    passedInitialInterstitial = true;
                    PlayerPrefs.SetFloat(prefsKey_initialInterstitialPassedTime, initialInterstitialPassedTime);
                }
            }
            else if(interstitialTimer > interstitialInterval)
                ShowInterstitial();
        }

        #endregion

        #region Callbacks
        private void OnInterstitialShow()
        {
            TimeScaler.SetTimeScale(sourceName, 0);
        }

        private void OnInterstitialComplete()
        {
            intersitialIsBeingDisplayed = false;
            ResetInterstitialTimer();

            TimeScaler.RemoveTimeScaleControl(sourceName);
        }

        private void OnInterstitialFailed()
        {
            intersitialIsBeingDisplayed = false;
            ResetInterstitialTimer();

            TimeScaler.RemoveTimeScaleControl(sourceName);
        }

        private void OnCanceledDuringAlert()
        {
            intersitialIsBeingDisplayed = false;
        }

        private void OnRewardedVideoShow(TafraAdInfo adInfo)
        {
            ResetInterstitialTimer();
        }
        #endregion

        #region Private Functions
        private void ShowInterstitial()
        {
            intersitialIsBeingDisplayed = true;

            TafraAds.ShowInterstitialAd(sourceName, OnInterstitialShow, OnInterstitialComplete, OnInterstitialFailed, OnCanceledDuringAlert);

            ResetInterstitialTimer();
        }

        private void ResetInterstitialTimer()
        {
            interstitialTimer = 0;
        }
        #endregion
    }
}