#if HAS_LION_APPLOVIN_SDK
using UnityEngine;

namespace TafraKit
{
    public class LionAdsSocketSettings : SettingsModule
    {
        [SerializeField] private bool enabled;

        [Header("Ad Units")]
        [SerializeField] private string interstitialAdUnitAndroid;
        [SerializeField] private string rewardedAdUnitAndroid;
        [SerializeField] private string bannerAdUnitAndroid;

        [Space()]

        [SerializeField] private string interstitialAdUnitIOS;
        [SerializeField] private string rewardedAdUnitIOS;
        [SerializeField] private string bannerAdUnitIOS;

        [Header("Amazon Ads")]
        [SerializeField] private bool apsEnabled;
        [SerializeField] private bool apsTestMode;

        [Space()]

        [SerializeField] private string amazonAppIdAndroid;
        [SerializeField] private string amazonInterstitialAdUnitAndroid;
        [SerializeField] private string amazonRewardedAdUnitAndroid;
        [SerializeField] private string amazonBannerAdUnitAndroid;

        [Space()]

        [SerializeField] private string amazonAppIdIOS;
        [SerializeField] private string amazonInterstitialAdUnitIOS;
        [SerializeField] private string amazonRewardedAdUnitIOS;
        [SerializeField] private string amazonBannerAdUnitIOS;

        [Header("Debugging")]
        [SerializeField] private bool debugLogs;

        public bool Enabled => enabled;
        public string InterstitialAdUnitAndroid => interstitialAdUnitAndroid;
        public string RewardedAdUnitAndroid => rewardedAdUnitAndroid;
        public string BannerAdUnitAndroid => bannerAdUnitAndroid;
        public string InterstitialAdUnitIOS => interstitialAdUnitIOS;
        public string RewardedAdUnitIOS => rewardedAdUnitIOS;
        public string BannerAdUnitIOS => bannerAdUnitIOS;
        public bool APSEnabled => apsEnabled;
        public bool APSTestMode => apsTestMode;
        public string AmazonAppIdAndroid => amazonAppIdAndroid;
        public string AmazonInterstitialAdUnitAndroid => amazonInterstitialAdUnitAndroid;
        public string AmazonRewardedAdUnitAndroid => amazonRewardedAdUnitAndroid;
        public string AmazonBannerAdUnitAndroid => amazonBannerAdUnitAndroid;
        public string AmazonAppIdIOS => amazonAppIdIOS;
        public string AmazonInterstitialAdUnitIOS => amazonInterstitialAdUnitIOS;
        public string AmazonRewardedAdUnitIOS => amazonRewardedAdUnitIOS;
        public string AmazonBannerAdUnitIOS => amazonBannerAdUnitIOS;
        public bool DebugLogs => debugLogs;

        public override int Priority => 51;
        public override string Name => "Monetization/Tafra Ads/Lion Ads Socket";
        public override string Description => "Displaying ads through Lion Ads SDK.";

        #region Ad Units
        public string InterstitialAdUnit
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return InterstitialAdUnitIOS;
                else
                    return InterstitialAdUnitAndroid;
            }
        }
        public string RewardedAdUnit
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return RewardedAdUnitIOS;
                else
                    return RewardedAdUnitAndroid;
            }
        }
        public string BannerAdUnit
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return BannerAdUnitIOS;
                else
                    return BannerAdUnitAndroid;
            }
        }
        public string AmazonAppId
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return AmazonAppIdIOS;
                else
                    return AmazonAppIdAndroid;
            }
        }
        public string AmazonInterstitialAdUnit
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return AmazonInterstitialAdUnitIOS;
                else
                    return AmazonInterstitialAdUnitAndroid;
            }
        }
        public string AmazonRewardedAdUnit
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return AmazonRewardedAdUnitIOS;
                else
                    return AmazonRewardedAdUnitAndroid;
            }
        }
        public string AmazonBannerAdUnit
        {
            get
            {
                if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                    return AmazonBannerAdUnitIOS;
                else
                    return AmazonBannerAdUnitAndroid;
            }
        }
        #endregion
    }
}
#endif