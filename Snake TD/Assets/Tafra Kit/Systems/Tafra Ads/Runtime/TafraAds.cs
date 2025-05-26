using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TafraKit;
using TafraKit.UI;
using TafraKit.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.Ads
{
    public static class TafraAds
    {
        private static TafraAdsSettings settings;
        private static AdsSocket activeSocket;
        private static InterstitialAlert defaultInterstitialAlert;
        private static InterstitialAlert interstitialAlertOverride;
        private static CancellationTokenSource alertCTS;
        private static bool noAdsEnabled;
        private static bool isBannerVisible;
        private static bool isRewardedVideoLoaded;
        private static bool isInterstitialLoaded;
        private static bool isVideoAdPlaying;
        private static bool isRVAdPlaying;
        private static bool isInterstitialAdPlaying;
        private static int interstitialsLoadRetryAttempts;
        private static int rewardedLoadRetryAttempts;
        private static CancellationTokenSource interstitialRetryCTS;
        private static CancellationTokenSource rewardedRetryLoadCTS;

        private static InterstitialAlert InterstitialAlert
        {
            get
            {
                if (interstitialAlertOverride)
                    return interstitialAlertOverride;
                else
                    return defaultInterstitialAlert; 
            }
        }
        public static bool IsRewardedVideoLoaded
        {
            get
            {
                if(activeSocket == null)
                    return true;

                return isRewardedVideoLoaded;
            }
        }
        public static bool IsInterstitialLoaded
        {
            get
            {
                if(activeSocket == null)
                    return true;

                return isInterstitialLoaded;
            }
        }
        /// <summary>
        /// Intended for objects that want to know if the application was taken out of focus because of an ad.
        /// </summary>
        public static bool IsVideoAdPlaying => isVideoAdPlaying;
        public static bool IsRVAdPlaying => isRVAdPlaying;
        public static bool IsInterstitialAdPlaying => isInterstitialAdPlaying;
        public static bool IsNoAdsEnabled => noAdsEnabled;

        #region Events
        public static UnityEvent OnRewardedVideoLoadRequest = new UnityEvent();
        public static UnityEvent OnRewardedVideoLoadFail = new UnityEvent();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoLoaded = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoShowRequest = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoShowFail = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoShow = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoStart = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoEnd = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnRewardedVideoClick = new UnityEvent<TafraAdInfo>();

        public static UnityEvent OnInterstitialLoadRequest = new UnityEvent();
        public static UnityEvent OnInterstitialLoadFail = new UnityEvent();
        public static UnityEvent<TafraAdInfo> OnInterstitialLoaded = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnInterstitialShowRequest = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnInterstitialShowFail = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnInterstitialShow = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnInterstitialStart = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnInterstitialEnd = new UnityEvent<TafraAdInfo>();
        public static UnityEvent<TafraAdInfo> OnInterstitialClick = new UnityEvent<TafraAdInfo>();

        public static UnityEvent<TafraAdInfo> OnBannerShowRequest = new UnityEvent<TafraAdInfo>();
        public static UnityEvent OnBannerShowFail = new UnityEvent();
        public static UnityEvent<TafraAdInfo> OnBannerShow = new UnityEvent<TafraAdInfo>();
        public static UnityEvent OnBannerHide = new UnityEvent();
        public static UnityEvent<TafraAdInfo> OnBannerClick = new UnityEvent<TafraAdInfo>();
        
        public static UnityEvent<TafraAdInfo> OnRevenueCollected = new UnityEvent<TafraAdInfo>();
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void FetchSettings()
        {
            settings = TafraSettings.GetSettings<TafraAdsSettings>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AttemptInitialization()
        {
            if (settings == null || !settings.Enabled)
                return;

            if (settings.DefaultInterstitialAlert != null)
            {
                defaultInterstitialAlert = GameObject.Instantiate(settings.DefaultInterstitialAlert).GetComponent<InterstitialAlert>();

                GameObject.DontDestroyOnLoad(defaultInterstitialAlert);
            }

            if (settings.NoAdsSF)
            {
                SetNoAds(settings.NoAdsSF.ValueInt > 0);

                settings.NoAdsSF.OnValueChange.AddListener(OnNoAdsSFChanged);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;

            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            #endif
        }

        #region Non-Ad Callbacks
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (settings.CancelInterstitialAlertOnSceneChange)
            {
                if (alertCTS != null)
                {
                    alertCTS.Cancel();
                    alertCTS.Dispose();
                    alertCTS = null;
                }
            }
        }
        private static void OnNoAdsSFChanged(float newValue)
        {
            SetNoAds(Mathf.RoundToInt(newValue) > 0);
        }

        #if UNITY_EDITOR
        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            //If we're exiting playmode, cancel the the active tasks.
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                if (alertCTS != null)
                {
                    alertCTS.Cancel();
                    alertCTS.Dispose();
                }
                if(interstitialRetryCTS != null)
                {
                    interstitialRetryCTS.Cancel();
                    interstitialRetryCTS.Dispose();
                }
                if(rewardedRetryLoadCTS != null)
                {
                    rewardedRetryLoadCTS.Cancel();
                    rewardedRetryLoadCTS.Dispose();
                }
            }
        }
        #endif

        #endregion

        #region Rewarded Video Callbacks
        private static void OnRewardedVideoAdLoadRequest()
        {
            OnRewardedVideoLoadRequest?.Invoke();
        }
        private static void OnRewardedVideoAdLoadFail()
        {
            isRewardedVideoLoaded = false;

            OnRewardedVideoLoadFail?.Invoke();

            if(settings.AutoLoadRewardedAds)
            {
                rewardedLoadRetryAttempts++;

                if(rewardedRetryLoadCTS != null)
                {
                    rewardedRetryLoadCTS.Cancel();
                    rewardedRetryLoadCTS.Dispose();
                }

                rewardedRetryLoadCTS = new CancellationTokenSource();

                float retryDelay = (float)Math.Pow(2, Math.Min(6, rewardedLoadRetryAttempts));
                ReloadRewarded(retryDelay, rewardedRetryLoadCTS.Token);
            }
        }
        private static void OnRewardedVideoAdLoaded(TafraAdInfo adInfo)
        {
            isRewardedVideoLoaded = true;

            rewardedLoadRetryAttempts = 0;

            OnRewardedVideoLoaded?.Invoke(adInfo);
        }
        private static void OnRewardedVideoAdShowRequest(TafraAdInfo adInfo)
        {
            isVideoAdPlaying = true;
            isRVAdPlaying = true;

            OnRewardedVideoShowRequest?.Invoke(adInfo);
        }
        private static void OnRewardedVideoAdShowFail(TafraAdInfo adInfo)
        {
            isRewardedVideoLoaded = false;

            isVideoAdPlaying = false;
            isRVAdPlaying = false;

            OnRewardedVideoShowFail?.Invoke(adInfo);

            if(settings.AutoLoadRewardedAds)
                LoadRewardedAd(null, null);
        }
        private static void OnRewardedVideoAdShow(TafraAdInfo adInfo)
        {
            isRewardedVideoLoaded = false;

            isVideoAdPlaying = true;
            isRVAdPlaying = true;

            OnRewardedVideoShow?.Invoke(adInfo);
        }
        private static void OnRewardedVideoAdStart(TafraAdInfo adInfo)
        {
            isRewardedVideoLoaded = false;
            
            OnRewardedVideoStart?.Invoke(adInfo);
        }
        private static void OnRewardedVideoAdEnd(TafraAdInfo adInfo)
        {
            isVideoAdPlaying = false;
            isRVAdPlaying = false;

            OnRewardedVideoEnd?.Invoke(adInfo);

            if(settings.AutoLoadRewardedAds)
                LoadRewardedAd(null, null);
        }
        private static void OnRewardedVideoAdClick(TafraAdInfo adInfo)
        {
            OnRewardedVideoClick?.Invoke(adInfo);
        }
        private static async Task ReloadRewarded(float delay, CancellationToken ct)
        {
            TafraDebugger.Log("Tafra Ads", $"Reloading rewarded ad in {delay} seconds.", TafraDebugger.LogType.Verbose);

            await TafraTasker.WaitUnscaled(delay, ct);

            LoadRewardedAd(null, null);
        }
        #endregion

        #region Interstitial Callbacks
        private static void OnInterstitialAdLoadRequest()
        {
            OnInterstitialLoadRequest?.Invoke();
        }
        private static void OnInterstitialAdLoadFail()
        {
            isInterstitialLoaded = false;

            OnInterstitialLoadFail?.Invoke();

            if(settings.AutoLoadInterstitialAds)
            {
                interstitialsLoadRetryAttempts++;

                if(interstitialRetryCTS != null)
                {
                    interstitialRetryCTS.Cancel();
                    interstitialRetryCTS.Dispose();
                }

                interstitialRetryCTS = new CancellationTokenSource();

                float retryDelay = (float)Math.Pow(2, Math.Min(6, interstitialsLoadRetryAttempts));
                ReloadInterstitial(retryDelay, interstitialRetryCTS.Token);
            }
        }
        private static void OnInterstitialAdLoaded(TafraAdInfo adInfo)
        {
            isInterstitialLoaded = true;

            interstitialsLoadRetryAttempts = 0;

            OnInterstitialLoaded?.Invoke(adInfo);
        }
        private static void OnInterstitialAdShowRequest(TafraAdInfo adInfo)
        {
            isVideoAdPlaying = true;
            isInterstitialAdPlaying = true;

            OnInterstitialShowRequest?.Invoke(adInfo);
        }
        private static void OnInterstitialAdShowFail(TafraAdInfo adInfo)
        {
            isInterstitialLoaded = false;

            isVideoAdPlaying = false;
            isInterstitialAdPlaying = false;

            OnInterstitialShowFail?.Invoke(adInfo);
        }
        private static void OnInterstitialAdShow(TafraAdInfo adInfo)
        {
            isInterstitialLoaded = false;

            isVideoAdPlaying = true;
            isInterstitialAdPlaying = true;

            OnInterstitialShow?.Invoke(adInfo);
        }
        private static void OnInterstitialAdStart(TafraAdInfo adInfo)
        {
            isInterstitialLoaded = false;

            OnInterstitialStart?.Invoke(adInfo);
        }
        private static void OnInterstitialAdEnd(TafraAdInfo adInfo)
        {
            isVideoAdPlaying = false;
            isInterstitialAdPlaying = false;

            OnInterstitialEnd?.Invoke(adInfo);
        }
        private static void OnInterstitialAdClick(TafraAdInfo adInfo)
        {
            OnInterstitialClick?.Invoke(adInfo);
        }
        private static async Task ReloadInterstitial(float delay, CancellationToken ct)
        {
            TafraDebugger.Log("Tafra Ads", $"Reloading interstitial in {delay} seconds.", TafraDebugger.LogType.Verbose);

            await TafraTasker.WaitUnscaled(delay, ct);

            LoadInterstitialAd(null, null);
        }
        #endregion

        #region Banner Callbacks
        private static void OnBannerAdShowRequest(TafraAdInfo adInfo)
        {
            OnBannerShowRequest?.Invoke(adInfo);
        }
        private static void OnBannerAdShowFail()
        {
            OnBannerShowFail?.Invoke();
        }
        private static void OnBannerAdShow(TafraAdInfo adInfo)
        {
            UpdateSafeAreasAccordingToBannerState();

            OnBannerShow?.Invoke(adInfo);
        }
        private static void OnBannerAdHide()
        {
            UpdateSafeAreasAccordingToBannerState();
     
            OnBannerHide?.Invoke();
        }
        private static void OnBannerAdClick(TafraAdInfo adInfo)
        {
            OnBannerClick?.Invoke(adInfo);
        }
        #endregion
         
        #region General Ad Callbacks
        private static void OnAdRevenueCollected(TafraAdInfo adInfo)
        {
            OnRevenueCollected?.Invoke(adInfo);
        }
        #endregion

        #region Private Functions
        private static void UpdateSafeAreasAccordingToBannerState()
        {
            if (isBannerVisible)
                TafraSafeAreas.UpdateArea(settings.BannerSafeAreaTopMargin, settings.BannerSafeAreaBotMargin, settings.BannerSafeAreaLeftMargin, settings.BannerSafeAreaRightMargin);
            else
                TafraSafeAreas.UpdateAreaUsingDefaultMargins();
        }
        private static void OnSocketReady()
        {
            if(settings.AutoLoadInterstitialAds)
                LoadInterstitialAd(null, null);
            if(settings.AutoLoadRewardedAds)
                LoadRewardedAd(null, null);
        }
        #endregion

        #region Public Functions
        public static void SetActiveSocket(AdsSocket socket)
        {
            TafraDebugger.Log("Tafra Ads", $"Ad socket was set ({socket}).", TafraDebugger.LogType.Info);

            #region Remove Previous Socket Listeners
            if(activeSocket != null)
            {
                activeSocket.OnReady.RemoveListener(OnSocketReady);

                activeSocket.OnRewardedVideoLoadRequest.RemoveListener(OnRewardedVideoAdLoadRequest);
                activeSocket.OnRewardedVideoLoadFail.RemoveListener(OnRewardedVideoAdLoadFail);
                activeSocket.OnRewardedVideoLoaded.RemoveListener(OnRewardedVideoAdLoaded);
                activeSocket.OnRewardedVideoShowRequest.RemoveListener(OnRewardedVideoAdShowRequest);
                activeSocket.OnRewardedVideoShowFail.RemoveListener(OnRewardedVideoAdShowFail);
                activeSocket.OnRewardedVideoShow.RemoveListener(OnRewardedVideoAdShow);
                activeSocket.OnRewardedVideoStart.RemoveListener(OnRewardedVideoAdStart);
                activeSocket.OnRewardedVideoEnd.RemoveListener(OnRewardedVideoAdEnd);
                activeSocket.OnRewardedVideoClick.RemoveListener(OnRewardedVideoAdClick);

                activeSocket.OnInterstitialLoadRequest.RemoveListener(OnInterstitialAdLoadRequest);
                activeSocket.OnInterstitialLoadFail.RemoveListener(OnInterstitialAdLoadFail);
                activeSocket.OnInterstitialLoaded.RemoveListener(OnInterstitialAdLoaded);
                activeSocket.OnInterstitialShowRequest.RemoveListener(OnInterstitialAdShowRequest);
                activeSocket.OnInterstitialShowFail.RemoveListener(OnInterstitialAdShowFail);
                activeSocket.OnInterstitialShow.RemoveListener(OnInterstitialAdShow);
                activeSocket.OnInterstitialStart.RemoveListener(OnInterstitialAdStart);
                activeSocket.OnInterstitialEnd.RemoveListener(OnInterstitialAdEnd);
                activeSocket.OnInterstitialClick.RemoveListener(OnInterstitialAdClick);

                activeSocket.OnBannerShowRequest.RemoveListener(OnBannerAdShowRequest);
                activeSocket.OnBannerShowFail.RemoveListener(OnBannerAdShowFail);
                activeSocket.OnBannerShow.RemoveListener(OnBannerAdShow);
                activeSocket.OnBannerHide.RemoveListener(OnBannerAdHide);
                activeSocket.OnBannerClick.RemoveListener(OnBannerAdClick);

                activeSocket.OnRevenueCollected.RemoveListener(OnAdRevenueCollected);
            }
            #endregion

            activeSocket = socket;
            activeSocket.Initialize();

            if(activeSocket.IsReady)
                OnSocketReady();

            #region Add New Socket Listeners
            activeSocket.OnReady.AddListener(OnSocketReady);

            activeSocket.OnRewardedVideoLoadRequest.AddListener(OnRewardedVideoAdLoadRequest);
            activeSocket.OnRewardedVideoLoadFail.AddListener(OnRewardedVideoAdLoadFail);
            activeSocket.OnRewardedVideoLoaded.AddListener(OnRewardedVideoAdLoaded);
            activeSocket.OnRewardedVideoShowRequest.AddListener(OnRewardedVideoAdShowRequest);
            activeSocket.OnRewardedVideoShowFail.AddListener(OnRewardedVideoAdShowFail);
            activeSocket.OnRewardedVideoShow.AddListener(OnRewardedVideoAdShow);
            activeSocket.OnRewardedVideoStart.AddListener(OnRewardedVideoAdStart);
            activeSocket.OnRewardedVideoEnd.AddListener(OnRewardedVideoAdEnd);
            activeSocket.OnRewardedVideoClick.AddListener(OnRewardedVideoAdClick);

            activeSocket.OnInterstitialLoadRequest.AddListener(OnInterstitialAdLoadRequest);
            activeSocket.OnInterstitialLoadFail.AddListener(OnInterstitialAdLoadFail);
            activeSocket.OnInterstitialLoaded.AddListener(OnInterstitialAdLoaded);
            activeSocket.OnInterstitialShowRequest.AddListener(OnInterstitialAdShowRequest);
            activeSocket.OnInterstitialShowFail.AddListener(OnInterstitialAdShowFail);
            activeSocket.OnInterstitialShow.AddListener(OnInterstitialAdShow);
            activeSocket.OnInterstitialStart.AddListener(OnInterstitialAdStart);
            activeSocket.OnInterstitialEnd.AddListener(OnInterstitialAdEnd);
            activeSocket.OnInterstitialClick.AddListener(OnInterstitialAdClick);

            activeSocket.OnBannerShowRequest.AddListener(OnBannerAdShowRequest);
            activeSocket.OnBannerShowFail.AddListener(OnBannerAdShowFail);
            activeSocket.OnBannerShow.AddListener(OnBannerAdShow);
            activeSocket.OnBannerHide.AddListener(OnBannerAdHide);
            activeSocket.OnBannerClick.AddListener(OnBannerAdClick);

            activeSocket.OnRevenueCollected.AddListener(OnAdRevenueCollected);
            #endregion
        }

        public static void LoadInterstitialAd(Action onSuccessfullyLoaded, Action onFailedToLoad)
        {
            if (activeSocket == null)
            {
                TafraDebugger.Log("Tafra Ads", "No active socket assigned, make sure to have a script that inherits from AdsSocket.", TafraDebugger.LogType.Error);
                onFailedToLoad?.Invoke();
                return;
            }

            activeSocket.LoadInterstitialAd(onSuccessfullyLoaded, onFailedToLoad);
        }
        public async static void ShowInterstitialAd(string source, Action onShow = null, Action onComplete = null, Action onFailed = null, Action onCanceledDuringAlert = null, bool ignoreAlert = false)
        {
            if (noAdsEnabled)
            {
                onFailed?.Invoke();
                return;
            }

            if (activeSocket == null)
            {
                TafraDebugger.Log("Tafra Ads", "No active socket assigned, make sure to have a script that inherits from AdsSocket.", TafraDebugger.LogType.Error);
                onFailed?.Invoke();
                return;
            }

            if (alertCTS != null)
            {
                alertCTS.Cancel();
                alertCTS.Dispose();
            }

            if(settings.SavePrefsOnAdShow)
                PlayerPrefs.Save();

            alertCTS = new CancellationTokenSource();
            
            if (!ignoreAlert && InterstitialAlert)
            {
                Task<BoolOperationResult> showTask = InterstitialAlert.Show(alertCTS.Token);

                await showTask;

                if (showTask.Result == BoolOperationResult.Success)
                {
                    activeSocket.ShowInterstitialAd(source, onShow, onComplete, onFailed);
                }
                else
                {
                    TafraDebugger.Log("Tafra Ads", "Interstitial alert wasn't completed successfully, cancelling interstitial call.", TafraDebugger.LogType.Info);

                    onCanceledDuringAlert?.Invoke();
                }
            }
            else
                activeSocket.ShowInterstitialAd(source, onShow, onComplete, onFailed);
        }
        public static void LoadRewardedAd(Action onSuccessfullyLoaded, Action onFailedToLoad)
        {
            if (activeSocket == null)
            {
                TafraDebugger.Log("Tafra Ads", "No active socket assigned, make sure to have a script that inherits from AdsSocket.", TafraDebugger.LogType.Error);
                onFailedToLoad?.Invoke();
                return;
            }

            activeSocket.LoadRewardedAd(onSuccessfullyLoaded, onFailedToLoad);
        }
        public static void ShowRewardedAd(string source, Action onShow = null, Action onReward = null, Action onComplete = null, Action onFailed = null)
        {
            if (activeSocket == null)
            {
                TafraDebugger.Log("Tafra Ads", "No active socket assigned, make sure to have a script that inherits from AdsSocket. Will succeed for now.", TafraDebugger.LogType.Error);
                onReward?.Invoke();
                onComplete?.Invoke();
                return;
            }

            if(settings.SavePrefsOnAdShow)
                PlayerPrefs.Save();

            activeSocket.ShowRewardedAd(source, onShow, onReward, onComplete, onFailed);
        }
        public static void ShowBannerAd(string source)
        {
            if (noAdsEnabled)
                return;

            if (activeSocket == null)
            {
                TafraDebugger.Log("Tafra Ads", "No active socket assigned, make sure to have a script that inherits from AdsSocket.", TafraDebugger.LogType.Error);
                return;
            }

            isBannerVisible = true;

            activeSocket.ShowBannerAd(source);
        }
        public static void HideBannerAd()
        {
            if (!isBannerVisible)
                return;

            if (activeSocket == null)
            {
                TafraDebugger.Log("Tafra Ads", "No active socket assigned, make sure to have a script that inherits from AdsSocket.", TafraDebugger.LogType.Error);
                return;
            }

            isBannerVisible = false;
            activeSocket.HideBannerAd();
        }

        public static void SetInterstitialAlertOverride(InterstitialAlert alert)
        {
            interstitialAlertOverride = alert;
        }
        public static void ClearInterstitialAlertOverride()
        {
            interstitialAlertOverride = null;
        }

        public static void SetNoAds(bool enabled)
        {
            noAdsEnabled = enabled;

            if (isBannerVisible)
                HideBannerAd();
        }
        #endregion
    }
}
