#if HAS_LION_APPLOVIN_SDK
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TafraKit.Tasks;

#if TAFRA_AMAZON_ADS
using AmazonAds;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.Ads
{
    public class TafraMaxAdSocket : AdsSocket
    {
        private static MaxAdsSocketSettings settings;
        private static TafraAdsSettings generalAdsSettings;

        private bool maxInitialized;

        private int interstitialRetryAttempts;
        private int rewardedRetryAttempts;
        private CancellationTokenSource interstitialRetryCTS;
        private CancellationTokenSource rewardedRetryCTS;

        private bool isInterstitialFirstLoad = true;
        private bool isRewardedFirstLoad = true;

        public override bool IsReady => maxInitialized;

        #region Callback Actions
        private static Action onInterstitialSuccessfllyLoadedCallback;
        private static Action onInterstitialFailedToLoadCallback;

        private static Action onInterstitialShowCallback;
        private static Action onInterstitialCompleteCallback;
        private static Action onInterstitialFailCallback;

        private static Action onRewardedSuccessfllyLoadedCallback;
        private static Action onRewardedFailedToLoadCallback;

        private static Action onRewardedShowCallback;
        private static Action onRewardedRewardCallback;
        private static Action onRewardedCompleteCallback;
        private static Action onRewardedFailCallback;
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AttemptInitialization()
        {
            generalAdsSettings = TafraSettings.GetSettings<TafraAdsSettings>();
            settings = TafraSettings.GetSettings<MaxAdsSocketSettings>();
        
            if (settings == null || !settings.Enabled)
                return;

            TafraMaxAdSocket instance = new TafraMaxAdSocket();

            TafraAds.SetActiveSocket(instance);
        }

        public override void Initialize()
        {
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            #endif

            #if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            Amazon.Initialize(settings.AmazonAppId);
            Amazon.EnableTesting(settings.APSTestMode);
            Amazon.EnableLogging(settings.APSTestMode);
            Amazon.UseGeoLocation(true);
            Amazon.SetMRAIDPolicy(Amazon.MRAIDPolicy.CUSTOM);
            Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.MAX));
            Amazon.SetMRAIDSupportedVersions(new string[] { "1.0", "2.0", "3.0" });

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon APS initalized ", TafraDebugger.LogType.Info);
            #endif

            if (MaxSdk.IsInitialized())
                OnMaxInitialized(MaxSdk.GetSdkConfiguration());
            else
                MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxInitialized;
        }

        #if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            //If we're exiting playmode, cancel the the active tasks.
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                if (interstitialRetryCTS != null)
                {
                    interstitialRetryCTS.Cancel();
                    interstitialRetryCTS.Dispose();
                }
                if (rewardedRetryCTS != null)
                {
                    rewardedRetryCTS.Cancel();
                    rewardedRetryCTS.Dispose();
                }
            }
        }
        #endif

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

            if (generalAdsSettings.AutoLoadInterstitialAds)
                LoadInterstitialAd(null, null);
            if (generalAdsSettings.AutoLoadRewardedAds)
                LoadRewardedAd(null, null);
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
            onInterstitialSuccessfllyLoadedCallback?.Invoke();
            
            OnInterstitialLoaded?.Invoke(GetTafraAdInfo(adInfo));

            interstitialRetryAttempts = 0;

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial loaded.", TafraDebugger.LogType.Info);
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            onInterstitialFailedToLoadCallback?.Invoke();

            OnInterstitialLoadFail?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial failed to loaded.", TafraDebugger.LogType.Info);

            if (generalAdsSettings.AutoLoadInterstitialAds)
            {
                interstitialRetryAttempts++;

                if (interstitialRetryCTS != null)
                {
                    interstitialRetryCTS.Cancel();
                    interstitialRetryCTS.Dispose();
                }

                interstitialRetryCTS = new CancellationTokenSource();

                float retryDelay = (float)Math.Pow(2, Math.Min(6, interstitialRetryAttempts));
                ReloadInterstitial(retryDelay, interstitialRetryCTS.Token);
            }
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onInterstitialShowCallback?.Invoke();

            OnInterstitialShow?.Invoke(GetTafraAdInfo(adInfo));
            OnInterstitialStart?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial displayed.", TafraDebugger.LogType.Info);
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            onInterstitialFailCallback?.Invoke();

            OnInterstitialShowFail?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial failed to display.", TafraDebugger.LogType.Info);

            if (generalAdsSettings.AutoLoadInterstitialAds)
                LoadInterstitialAd(null, null);
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnInterstitialClick?.Invoke(GetTafraAdInfo(adInfo));
       
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial clicked.", TafraDebugger.LogType.Info);
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onInterstitialCompleteCallback?.Invoke();

            OnInterstitialEnd?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial hidden.", TafraDebugger.LogType.Info);

            if (generalAdsSettings.AutoLoadInterstitialAds)
                LoadInterstitialAd(null, null);
        }

        private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Interstitial ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);

            LogRevenueEvent(adInfo);
        }

        private async Task ReloadInterstitial(float delay, CancellationToken ct)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Reloading interstitial in {delay} seconds.", TafraDebugger.LogType.Info);

            await TafraTasker.WaitUnscaled(delay, ct);

            LoadInterstitialAd(null, null);
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
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onRewardedSuccessfllyLoadedCallback?.Invoke();
            
            OnRewardedVideoLoaded?.Invoke(GetTafraAdInfo(adInfo));

            rewardedRetryAttempts = 0;

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad loaded.", TafraDebugger.LogType.Info);
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            onRewardedFailedToLoadCallback?.Invoke();

            OnRewardedVideoLoadFail?.Invoke();
            
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad failed to load.", TafraDebugger.LogType.Info);

            if (generalAdsSettings.AutoLoadRewardedAds)
            {
                rewardedRetryAttempts++;

                if (rewardedRetryCTS != null)
                {
                    rewardedRetryCTS.Cancel();
                    rewardedRetryCTS.Dispose();
                }

                rewardedRetryCTS = new CancellationTokenSource();

                float retryDelay = (float)Math.Pow(2, Math.Min(6, rewardedRetryAttempts));
                ReloadRewarded(retryDelay, rewardedRetryCTS.Token);
            }
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onRewardedShowCallback?.Invoke();

            OnRewardedVideoShow?.Invoke(GetTafraAdInfo(adInfo));
            OnRewardedVideoStart?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad displayed.", TafraDebugger.LogType.Info);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            onRewardedFailCallback?.Invoke();

            OnRewardedVideoShowFail?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad failed to display.", TafraDebugger.LogType.Info);

            if (generalAdsSettings.AutoLoadRewardedAds)
                LoadRewardedAd(null, null);
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnRewardedVideoClick?.Invoke(GetTafraAdInfo(adInfo));
      
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad clicked.", TafraDebugger.LogType.Info);
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);

            LogRevenueEvent(adInfo);
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onRewardedCompleteCallback?.Invoke();

            OnRewardedVideoEnd?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad hidden.", TafraDebugger.LogType.Info);

            if (generalAdsSettings.AutoLoadRewardedAds)
                LoadRewardedAd(null, null);
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            onRewardedRewardCallback?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Rewarded ad reward received.", TafraDebugger.LogType.Info);
        }

        private async Task ReloadRewarded(float delay, CancellationToken ct)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Reloading rewarded ad in {delay} seconds.", TafraDebugger.LogType.Info);

            await TafraTasker.WaitUnscaled(delay, ct);

            LoadRewardedAd(null, null);
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
                TafraDebugger.Log("Tafra Max Ad Socket", $"Banner ad displayed.", TafraDebugger.LogType.Info);
        }
        private void OnBannerAdLoadFailEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Banner ad failed to display.", TafraDebugger.LogType.Info);
        }
        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnBannerClick?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Banner ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);
        }
        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Banner ad revenue paid ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);

            LogRevenueEvent(adInfo);
        }
        #endregion

        #region Revenue Logging
        private void LogRevenueEvent(MaxSdkBase.AdInfo adInfo)
        {
            OnRevenueCollected?.Invoke(GetTafraAdInfo(adInfo));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Revenue logged ({adInfo.Revenue} USD).", TafraDebugger.LogType.Info);
        }
        #endregion

        #region Public Functions
        public override void LoadInterstitialAd(Action onSuccessfullyLoaded, Action onFailedToLoad)
        {
            onInterstitialSuccessfllyLoadedCallback = onSuccessfullyLoaded;
            onInterstitialFailedToLoadCallback = onFailedToLoad;

            OnInterstitialLoadRequest?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Attempting to load an interstitial ad ({settings.InterstitialAdUnit}).", TafraDebugger.LogType.Info);

            #if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            if (Amazon.IsInitialized() && isInterstitialFirstLoad)
            {
                var apsInterstitialVideoAdRequest = new APSVideoAdRequest(320, 480, settings.AmazonInterstitialAdUnit);
                apsInterstitialVideoAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetInterstitialLocalExtraParameter(settings.InterstitialAdUnit, "amazon_ad_response", adResponse.GetResponse());
                    MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);

                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Interstitial AD - Success: {adResponse.GetResponse()}", TafraDebugger.LogType.Info);
                };
                apsInterstitialVideoAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetInterstitialLocalExtraParameter(settings.InterstitialAdUnit, "amazon_ad_error", adError.GetAdError());
                    MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);

                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Interstitial AD - Failed: {adError.GetAdError()}", TafraDebugger.LogType.Info);
                };

                apsInterstitialVideoAdRequest.LoadAd();
            }
            else MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);
            #else
            MaxSdk.LoadInterstitial(settings.InterstitialAdUnit);
            #endif

            if(isInterstitialFirstLoad)
                isInterstitialFirstLoad = false;
        }
        public override void ShowInterstitialAd(string source, Action onShow, Action onComplete, Action onFail)
        {
            onInterstitialShowCallback = onShow;
            onInterstitialCompleteCallback = onComplete;
            onInterstitialFailCallback = onFail;

            OnInterstitialShowRequest?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Attempting to show an interstitial ad.", TafraDebugger.LogType.Info);

            if (MaxSdk.IsInterstitialReady(settings.InterstitialAdUnit))
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Max Ad Socket", $"An interstitial is ready to be shown. Attempting to show it...", TafraDebugger.LogType.Info);

                MaxSdk.ShowInterstitial(settings.InterstitialAdUnit, source);
            }
            else
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Max Ad Socket", $"An interstitial ad isn't ready. Please load one first.", TafraDebugger.LogType.Info);

                OnInterstitialShowFail?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));
                onFail?.Invoke();
            }
        }

        public override void LoadRewardedAd(Action onSuccessfullyLoaded, Action onFailedToLoad)
        {
            onRewardedSuccessfllyLoadedCallback = onSuccessfullyLoaded;
            onRewardedFailedToLoadCallback = onFailedToLoad;

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Attempting to load a rewarded ad ({settings.RewardedAdUnit}).", TafraDebugger.LogType.Info);

            OnRewardedVideoLoadRequest?.Invoke();

            #if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            if (Amazon.IsInitialized() && isRewardedFirstLoad)
            {
                var apsRewardedVideoAdRequest = new APSVideoAdRequest(320, 480, settings.AmazonRewardedAdUnit);
                apsRewardedVideoAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetRewardedAdLocalExtraParameter(settings.RewardedAdUnit, "amazon_ad_response", adResponse.GetResponse());
                    MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);

                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Rewarded AD - Success: {adResponse.GetResponse()}", TafraDebugger.LogType.Info);
                };
                apsRewardedVideoAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetRewardedAdLocalExtraParameter(settings.RewardedAdUnit, "amazon_ad_error", adError.GetAdError());
                    MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);

                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Rewarded AD - Failed: {adError.GetAdError()}", TafraDebugger.LogType.Info);
                };

                apsRewardedVideoAdRequest.LoadAd();
            }
            else MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);
            #else
            MaxSdk.LoadRewardedAd(settings.RewardedAdUnit);
            #endif

            if(isRewardedFirstLoad)
                isRewardedFirstLoad = false;
        }
        public override void ShowRewardedAd(string source, Action onShow, Action onReward, Action onComplete, Action onFail)
        {
            onRewardedShowCallback = onShow;
            onRewardedRewardCallback = onReward;
            onRewardedCompleteCallback = onComplete;
            onRewardedFailCallback = onFail;

            OnRewardedVideoShowRequest?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Attempting to show a rewarded ad.", TafraDebugger.LogType.Info);

            if (MaxSdk.IsRewardedAdReady(settings.RewardedAdUnit))
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Max Ad Socket", $"A rewarded ad is ready to be shown. Attempting to show it...", TafraDebugger.LogType.Info);

                MaxSdk.ShowRewardedAd(settings.RewardedAdUnit, source);
            }
            else
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Tafra Max Ad Socket", $"A rewarded ad isn't ready. Please load one first.", TafraDebugger.LogType.Info);

                OnRewardedVideoShowFail?.Invoke(new TafraAdInfo(settings.RewardedAdUnit, source));

                onFail?.Invoke();
            }
        }

        public override void ShowBannerAd(string source)
        {
            TafraAdInfo adInfo = new TafraAdInfo(settings.BannerAdUnit, source);

            OnBannerShowRequest?.Invoke(adInfo);
            OnBannerShow?.Invoke(adInfo);

            #if !UNITY_EDITOR && TAFRA_AMAZON_ADS
            if (Amazon.IsInitialized())
            {
                int width, height;
                if (MaxSdkUtils.IsTablet()) { width = 728; height = 90; }
                else { width = 320; height = 50; }

                var apsBannerAdRequest = new APSBannerAdRequest(width, height, settings.AmazonBannerAdUnit);
                apsBannerAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetBannerLocalExtraParameter(settings.BannerAdUnit, "amazon_ad_response", adResponse.GetResponse());
                    MaxSdk.CreateBanner(settings.BannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);

                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Banner AD - Success: {adResponse.GetResponse()}", TafraDebugger.LogType.Info);
                };
                apsBannerAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetBannerLocalExtraParameter(settings.BannerAdUnit, "amazon_ad_error", adError.GetAdError());
                    MaxSdk.CreateBanner(settings.BannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);

                    if (settings.DebugLogs)
                        TafraDebugger.Log("Tafra Max Ad Socket", $"Amazon Banner AD - Failed: {adError.GetAdError()}", TafraDebugger.LogType.Info);
                };

                apsBannerAdRequest.LoadAd();
            }
            else
            {
                MaxSdk.CreateBanner(settings.BannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);
                MaxSdk.ShowBanner(settings.BannerAdUnit);
                MaxSdk.SetBannerPlacement(settings.BannerAdUnit, source);
                MaxSdk.SetBannerBackgroundColor(settings.BannerAdUnit, generalAdsSettings.BannerBGColor);
            }
#else
            MaxSdk.CreateBanner(settings.BannerAdUnit, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.ShowBanner(settings.BannerAdUnit);
            MaxSdk.SetBannerPlacement(settings.BannerAdUnit, source);
            MaxSdk.SetBannerBackgroundColor(settings.BannerAdUnit, generalAdsSettings.BannerBGColor);
            #endif

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Showing a banner ad.", TafraDebugger.LogType.Info);
        }
        public override void HideBannerAd()
        {
            MaxSdk.HideBanner(settings.BannerAdUnit);

            OnBannerHide?.Invoke();

            if (settings.DebugLogs)
                TafraDebugger.Log("Tafra Max Ad Socket", $"Hiding banner ad.", TafraDebugger.LogType.Info);
        }
        #endregion
    }
}
#endif