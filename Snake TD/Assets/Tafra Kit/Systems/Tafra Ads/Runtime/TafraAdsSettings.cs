using System.Collections;
using System.Collections.Generic;
using TafraKit.Ads;
using UnityEngine;

namespace TafraKit
{
    public class TafraAdsSettings : SettingsModule
    {
        [SerializeField] private bool enabled;

        [Header("Auto Loading")]
        [Tooltip("Should the system make sure that there's always an interstitial loaded (or at least being loaded).")]
        [SerializeField] private bool autoLoadInterstitialAds = false;
        [Tooltip("Should the system make sure that there's always a rewarded video loaded (or at least being loaded).")]
        [SerializeField] private bool autoLoadRewardedAds = false;

        [Header("Interstitials Alert")]
        [SerializeField] private InterstitialAlert defaultInterstitialAlert;
        [SerializeField] private bool cancelInterstitialAlertOnSceneChange = true;

        [Header("Banner Properties")]
        [SerializeField] private Color bannerBGColor = Color.white;
        [Range(0, 1)]
        [SerializeField] private float bannerSafeAreaTopMargin;
        [Range(0, 1)]
        [SerializeField] private float bannerSafeAreaBotMargin = 0.09f;
        [Range(0, 1)]
        [SerializeField] private float bannerSafeAreaLeftMargin;
        [Range(0, 1)]
        [SerializeField] private float bannerSafeAreaRightMargin;

        [Header("General Properties")]
        [Tooltip("Automatically save PlayerPrefs before displaying an ad to make sure the data is saved in case the ad caused a crash.")]
        [SerializeField] private bool savePrefsOnAdShow = true;

        [Header("No Ads")]
        [SerializeField] private ScriptableFloat noAdsSF;

        public bool Enabled => enabled;
        public bool AutoLoadInterstitialAds => autoLoadInterstitialAds;
        public bool AutoLoadRewardedAds => autoLoadRewardedAds;
        public InterstitialAlert DefaultInterstitialAlert => defaultInterstitialAlert;
        public bool CancelInterstitialAlertOnSceneChange => cancelInterstitialAlertOnSceneChange;
        public Color BannerBGColor => bannerBGColor;
        public float BannerSafeAreaTopMargin => BannerSafeAreaTopMargin;
        public float BannerSafeAreaBotMargin => BannerSafeAreaBotMargin; 
        public float BannerSafeAreaLeftMargin => bannerSafeAreaLeftMargin; 
        public float BannerSafeAreaRightMargin => bannerSafeAreaRightMargin;
        public bool SavePrefsOnAdShow => savePrefsOnAdShow;
        public ScriptableFloat NoAdsSF => noAdsSF;

        public override int Priority => 50;
        public override string Name => "Monetization/Tafra Ads";
        public override string Description => "Displaying ads through ad sockets.";
    }
}
