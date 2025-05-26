#if TAFRA_ADJUST
using com.adjust.sdk;
#if TAFRA_ADJUST_PURCHASING
using com.adjust.sdk.purchase;
#endif
using System.Collections;
using System.Collections.Generic;
using TafraKit.Ads;
using UnityEngine;

namespace TafraKit.ExternalSDKs.AdjustSDK
{
    public class AdjustSDKSettings : SettingsModule
    {
        public bool Enabled;
        public string AndroidAppToken;
        [InspectorDisplayName("iOS App Token")]
        public string IOSAppToken;
        public bool AutoInitialize = false;
        public bool InitializeAfterMAX = true;
        public bool TrackAdRevenue = true;

        [Header("EnvironmentS")]
        public AdjustEnvironment AdjustEnvironment = AdjustEnvironment.Production;

        [Header("Log Level")]
        public AdjustLogLevel AdjustLogLevel = AdjustLogLevel.Verbose;
      
        [Header("In App Purchase")]
        public bool TrackIAPRevenue;
        public string AndroidIAPRevenueEventToken;
        [InspectorDisplayName("iOS IAP Revenue Event Token")]
        public string IOSIAPRevenueEventToken;

        #if TAFRA_ADJUST_PURCHASING
        [Header("Purchasing")]
        public bool EnablePurcheseValidation = true;
        public ADJPEnvironment AdjustPurchaseEnvironment = ADJPEnvironment.Production;
        public ADJPLogLevel PurchaseLogLevel = ADJPLogLevel.Verbose;
        #endif

        [Header("iOS App Tracking Transparency")]
        [InspectorDisplayName("Enable iOS AAT")]
        public bool EnableiOSAAT;

        [Header("Testing")]
        public bool DebugLogs;

        public override int Priority => 70;

        public override string Name => "External SDKs/Adjust SDK";

        public override string Description => "Controls Adjust SDK logic and parameters.";

        public string GetAtiveAppToken()
        {
            if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                return IOSAppToken;
            else
                return AndroidAppToken;
        }
        public string GetAtiveIAPRevenueEventToken()
        {
            if (TafraApplication.Platform == RuntimePlatform.IPhonePlayer)
                return IOSIAPRevenueEventToken;
            else
                return AndroidIAPRevenueEventToken;
        }
    }
}
#endif