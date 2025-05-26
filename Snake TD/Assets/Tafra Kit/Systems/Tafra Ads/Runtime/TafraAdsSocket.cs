using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Ads
{
    public abstract class AdsSocket 
    {
        public abstract bool IsReady { get; }

        public UnityEvent OnReady = new UnityEvent();

        public UnityEvent OnRewardedVideoLoadRequest = new UnityEvent();
        public UnityEvent OnRewardedVideoLoadFail = new UnityEvent();
        public UnityEvent<TafraAdInfo> OnRewardedVideoLoaded = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnRewardedVideoShowRequest = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnRewardedVideoShowFail = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnRewardedVideoShow = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnRewardedVideoStart = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnRewardedVideoEnd = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnRewardedVideoClick = new UnityEvent<TafraAdInfo>();

        public UnityEvent OnInterstitialLoadRequest = new UnityEvent();
        public UnityEvent OnInterstitialLoadFail = new UnityEvent();
        public UnityEvent<TafraAdInfo> OnInterstitialLoaded = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnInterstitialShowRequest = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnInterstitialShowFail = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnInterstitialShow = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnInterstitialStart = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnInterstitialEnd = new UnityEvent<TafraAdInfo>();
        public UnityEvent<TafraAdInfo> OnInterstitialClick = new UnityEvent<TafraAdInfo>();

        public UnityEvent<TafraAdInfo> OnBannerShowRequest = new UnityEvent<TafraAdInfo>();
        public UnityEvent OnBannerShowFail = new UnityEvent();
        public UnityEvent<TafraAdInfo> OnBannerShow = new UnityEvent<TafraAdInfo>();
        public UnityEvent OnBannerHide = new UnityEvent();
        public UnityEvent<TafraAdInfo> OnBannerClick = new UnityEvent<TafraAdInfo>();

        public UnityEvent<TafraAdInfo> OnRevenueCollected = new UnityEvent<TafraAdInfo>();

        public abstract void Initialize();
        public abstract void LoadInterstitialAd(Action onSuccessfullyLoaded, Action onFailedToLoad);
        public abstract void ShowInterstitialAd(string source, Action onShow, Action onComplete, Action onFail);
        public abstract void LoadRewardedAd(Action onSuccessfullyLoaded, Action onFailedToLoad);
        public abstract void ShowRewardedAd(string source, Action onShow, Action onReward, Action onComplete, Action onFail);
        public abstract void ShowBannerAd(string source);
        public abstract void HideBannerAd();
    }
}