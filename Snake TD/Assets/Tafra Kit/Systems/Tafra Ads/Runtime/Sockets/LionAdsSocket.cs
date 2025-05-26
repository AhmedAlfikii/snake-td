#if HAS_LION_APPLOVIN_SDK
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TafraKit.Tasks;
using LionStudios.Suite.Ads;
using System.Collections.Generic;

#if TAFRA_AMAZON_ADS
using AmazonAds;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.Ads
{
    public class LionAdsSocket : AdsSocket
    {
        private static LionAdsSocketSettings settings;
        private static TafraAdsSettings generalAdsSettings;
        private static bool maxInitialized;

        public override bool IsReady => maxInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AttemptInitialization()
        {
            generalAdsSettings = TafraSettings.GetSettings<TafraAdsSettings>();
            settings = TafraSettings.GetSettings<LionAdsSocketSettings>();
        
            if (settings == null || !settings.Enabled)
                return;

            LionAdsSocket instance = new LionAdsSocket();

            TafraAds.SetActiveSocket(instance);
        }

        public override void Initialize()
        {
            //#if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            //Amazon.Initialize(settings.AmazonAppId);
            //Amazon.EnableTesting(settings.APSTestMode);
            //Amazon.EnableLogging(settings.APSTestMode);
            //Amazon.UseGeoLocation(true);
            //Amazon.SetMRAIDPolicy(Amazon.MRAIDPolicy.CUSTOM);
            //Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.MAX));
            //Amazon.SetMRAIDSupportedVersions(new string[] { "1.0", "2.0", "3.0" });

            //if (settings.DebugLogs)
            //    TafraDebugger.Log("Tafra Lion Ads Socket", $"Amazon APS initalized ", TafraDebugger.LogType.Info);
            //#endif


            if(MaxSdk.IsInitialized())
                OnMaxInitialized(MaxSdk.GetSdkConfiguration());
            else
                MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxInitialized;
        }

        private void OnMaxInitialized(MaxSdkBase.SdkConfiguration config)
        {
            InitializeAds();

            #if DEVELOPMENT_BUILD
            MaxSdk.ShowMediationDebugger();
            #endif

            maxInitialized = true;

            OnReady?.Invoke();
        }
        private void InitializeAds()
        {
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeBannerAds();
        }
        private TafraAdInfo GetTafraAdInfo(MaxSdkBase.AdInfo adInfo)
        {
            TafraAdInfo tafraAdInfo = new TafraAdInfo()
            {
                AdUnitIdentifier = adInfo.AdUnitIdentifier,
                AdFormat = adInfo.AdFormat,
                NetworkName = adInfo.NetworkName,
                NetworkPlacement = adInfo.Placement,
                Placement = adInfo.Placement,
                Revenue = adInfo.Revenue,
                RevenuePrecision = adInfo.RevenuePrecision
            };
            return tafraAdInfo;
        }

        #region Interstital Initialization
        private void InitializeInterstitialAds()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
        }
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialLoaded?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial loaded.", TafraDebugger.LogType.Info);
        }
        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            OnInterstitialLoadFail?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial failed to loaded.", TafraDebugger.LogType.Info);
        }
        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialShow?.Invoke(GetTafraAdInfo(adInfo));
            OnInterstitialStart?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial displayed.", TafraDebugger.LogType.Info);
        }
        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialShowFail?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial failed to display.", TafraDebugger.LogType.Info);
        }
        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialClick?.Invoke(GetTafraAdInfo(adInfo));
       
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial clicked.", TafraDebugger.LogType.Info);
        }
        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialEnd?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial hidden.", TafraDebugger.LogType.Info);
        }
        private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Interstitial ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);

            LogRevenueEvent(adInfo);
        }
        #endregion

        #region Rewarded Initialization
        private void InitializeRewardedAds()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        }
        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedVideoLoaded?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad loaded.", TafraDebugger.LogType.Info);
        }
        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            OnRewardedVideoLoadFail?.Invoke();
            
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad failed to load.", TafraDebugger.LogType.Info);
        }
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedVideoShow?.Invoke(GetTafraAdInfo(adInfo));
            OnRewardedVideoStart?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad displayed.", TafraDebugger.LogType.Info);
        }
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedVideoShowFail?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad failed to display.", TafraDebugger.LogType.Info);
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedVideoClick?.Invoke(GetTafraAdInfo(adInfo));
      
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad clicked.", TafraDebugger.LogType.Info);
        }
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);

            LogRevenueEvent(adInfo);
        }
        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedVideoEnd?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Rewarded ad hidden.", TafraDebugger.LogType.Info);
        }
        #endregion

        #region Banner Initialization
        public void InitializeBannerAds()
        {
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        }

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Banner ad loaded.", TafraDebugger.LogType.Info);
        }
        private void OnBannerAdLoadFailEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Banner ad failed to loaded.", TafraDebugger.LogType.Info);
        }
        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnBannerClick?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Banner ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);
        }
        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Banner ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);

            LogRevenueEvent(adInfo);
        }
        #endregion

        #region Revenue Logging
        private void LogRevenueEvent(MaxSdkBase.AdInfo adInfo)
        {
            OnRevenueCollected?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Revenue logged ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);
        }
        #endregion

        #region Public Functions
        public override void LoadInterstitialAd(Action onSuccessfullyLoaded, Action onFailedToLoad)
        {
            OnInterstitialLoadRequest?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Attempting to load an interstitial ad.", TafraDebugger.LogType.Info);

            //#if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            //if (Amazon.IsInitialized() && isInterstitialFirstLoad)
            //{
            //    var apsInterstitialVideoAdRequest = new APSVideoAdRequest(320, 480, settings.AmazonInterstitialAdUnit);
            //    apsInterstitialVideoAdRequest.onSuccess += (adResponse) =>
            //    {
            //        MaxSdk.SetInterstitialLocalExtraParameter(settings.InterstitialAdUnit, "amazon_ad_response", adResponse.GetResponse());
            //        MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);

            //        if (settings.DebugLogs)
            //            TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Interstitial AD - Success: {adResponse.GetResponse()}", TafraDebugger.LogType.Info);
            //    };
            //    apsInterstitialVideoAdRequest.onFailedWithError += (adError) =>
            //    {
            //        MaxSdk.SetInterstitialLocalExtraParameter(settings.InterstitialAdUnit, "amazon_ad_error", adError.GetAdError());
            //        MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);

            //        if (settings.DebugLogs)
            //            TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Interstitial AD - Failed: {adError.GetAdError()}", TafraDebugger.LogType.Info);
            //    };

            //    apsInterstitialVideoAdRequest.LoadAd();
            //}
            //else MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);
            //#else
            //MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);
            //#endif
        }
        public override void ShowInterstitialAd(string source, Action onShow, Action onComplete, Action onFail)
        {
            OnInterstitialShowRequest?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Attempting to show an interstitial ad.", TafraDebugger.LogType.Info);

            if (LionAds.IsInterstitialReady)
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Lion Ad Socket", $"An interstitial is ready to be shown. Attempting to show it...", TafraDebugger.LogType.Info);

                //Dictionary<string, object> additionalData = null;

                //if(AnalyticsManager.Instance != null)
                //    additionalData = AnalyticsManager.Instance.GetAdAdditionalData();

                if(LionAds.TryShowInterstitial(source, onComplete))
                    onShow?.Invoke();
                else
                    onFail?.Invoke();
            }
            else
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Lion Ad Socket", $"An interstitial ad isn't ready. Please load one first.", TafraDebugger.LogType.Info);

                OnInterstitialShowFail?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));
                onFail?.Invoke();
            }
        }

        public override void LoadRewardedAd(Action onSuccessfullyLoaded, Action onFailedToLoad)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Attempting to load a rewarded ad ({settings.RewardedAdUnit}).", TafraDebugger.LogType.Info);

            OnRewardedVideoLoadRequest?.Invoke();

            //#if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            //if (Amazon.IsInitialized() && isRewardedFirstLoad)
            //{
            //    var apsRewardedVideoAdRequest = new APSVideoAdRequest(320, 480, settings.AmazonRewardedAdUnit);
            //    apsRewardedVideoAdRequest.onSuccess += (adResponse) =>
            //    {
            //        MaxSdk.SetRewardedAdLocalExtraParameter(settings.RewardedAdUnit, "amazon_ad_response", adResponse.GetResponse());
            //        MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);

            //        if (settings.DebugLogs)
            //            TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Rewarded AD - Success: {adResponse.GetResponse()}", TafraDebugger.LogType.Info);
            //    };
            //    apsRewardedVideoAdRequest.onFailedWithError += (adError) =>
            //    {
            //        MaxSdk.SetRewardedAdLocalExtraParameter(settings.RewardedAdUnit, "amazon_ad_error", adError.GetAdError());
            //        MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);

            //        if (settings.DebugLogs)
            //            TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Rewarded AD - Failed: {adError.GetAdError()}", TafraDebugger.LogType.Info);
            //    };

            //    apsRewardedVideoAdRequest.LoadAd();
            //}
            //else MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);
            //#else
            //MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);
            //#endif
        }
        public override void ShowRewardedAd(string source, Action onShow, Action onReward, Action onComplete, Action onFail)
        {
            OnRewardedVideoShowRequest?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));

            if(settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Attempting to show a rewarded ad.", TafraDebugger.LogType.Info);

            if (LionAds.IsRewardedReady)
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Lion Ad Socket", $"A rewarded ad is ready to be shown. Attempting to show it...", TafraDebugger.LogType.Info);

                //Dictionary<string, object> additionalData = null;

                //if(AnalyticsManager.Instance != null)
                //    additionalData = AnalyticsManager.Instance.GetAdAdditionalData();

                if(LionAds.TryShowRewarded(source, onReward, onComplete, null))
                    onShow?.Invoke();
                else
                    onFail?.Invoke();
            }
            else
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Lion Ad Socket", $"A rewarded ad isn't ready. Please load one first.", TafraDebugger.LogType.Info);

                OnRewardedVideoShowFail?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));

                onFail?.Invoke();
            }
        }

        public override void ShowBannerAd(string source)
        {
            TafraAdInfo adInfo = new TafraAdInfo(settings.BannerAdUnit, source);

            LionAds.ShowBanner();

            OnBannerShowRequest?.Invoke(adInfo);
            OnBannerShow?.Invoke(adInfo);

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Showing a banner ad.", TafraDebugger.LogType.Info);
        }
        public override void HideBannerAd()
        {
            LionAds.HideBanner();

            OnBannerHide?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Lion Ad Socket", $"Hiding banner ad.", TafraDebugger.LogType.Info);
        }
        #endregion
    }
}
#endif