#if TAFRA_ADJUST
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;
using TafraKit.Ads;
using TafraKit.IAP;
using System;
using UnityEngine.Purchasing;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Purchasing;
#if TAFRA_ADJUST_PURCHASING
using com.adjust.sdk.purchase;
#endif
using AnalyticsProduct = LionStudios.Suite.Analytics.Product;

namespace TafraKit.ExternalSDKs.AdjustSDK
{
    public static class TafraAdjustSDKManager
    {
        private static AdjustSDKSettings settings;

        public static bool IsInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeManager()
        {
            settings = TafraSettings.GetSettings<AdjustSDKSettings>();

            if(settings == null || !settings.Enabled)
                return;

            if(settings.InitializeAfterMAX)
            {
                #if TAFRA_MAX
                if (settings.DebugLogs)
                    TafraDebugger.Log("Adjust SDK Manager", "Will wait until MAX is initialized before I initialize.", TafraDebugger.LogType.Info);

                if (MaxSdk.IsInitialized())
                    OnMaxInitialized(MaxSdk.GetSdkConfiguration());
                else
                    MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxInitialized;

                #else
                if (settings.AutoInitialize)
                {
                    if (settings.DebugLogs)
                        TafraDebugger.Log("Adjust SDK Manager", "Couldn't find MAX. I will intialize now.", TafraDebugger.LogType.Info);

                    Initialize();
                }
                #endif
            }
            else if (settings.AutoInitialize)
                Initialize();

            if(settings.TrackAdRevenue)
                TafraAds.OnRevenueCollected.AddListener(OnAdRevenueCollected);

            if(settings.TrackIAPRevenue)
                TafraIAP.OnPurchaseValidationResult.AddListener(OnIAPPurchaseValidationResult);
        }

        private static void OnMaxInitialized(MaxSdkBase.SdkConfiguration config)
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", "MAX was initialized. I should initialize too.", TafraDebugger.LogType.Info);

            if (settings.AutoInitialize)
                Initialize();
        }

        private static void Initialize()
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", "Attempting to initialize Adjust SDK...", TafraDebugger.LogType.Info);

            if (!settings.EnableiOSAAT)
            {
                StartSDK();
                return;
            }

            #if UNITY_IOS
            if (settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", "Checking iOS AAT dialogue status.", TafraDebugger.LogType.Info);

            bool shouldDisplayAATConsent = true;

            if ((MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") == MaxSdkUtils.VersionComparisonResult.Lesser)
            && (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5-beta") == MaxSdkUtils.VersionComparisonResult.Lesser))
            {
                shouldDisplayAATConsent = false;
            }

            PlayerPrefs.SetInt("TAFRA_ATT_STAT", Adjust.getAppTrackingAuthorizationStatus());
            if (shouldDisplayAATConsent && Adjust.getAppTrackingAuthorizationStatus() == 0) //Authorization Status Not Determined
                ATTRequest();
            else
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Adjust SDK Manager", "Don't need to display an iOS AAT dialogue. Will start the SDK now.", TafraDebugger.LogType.Info);

                StartSDK();
            }
            #else
            StartSDK();
            #endif
        }
        private static void StartSDK()
        {
            string appToken = settings.GetAtiveAppToken();

            AdjustEnvironment environment = settings.AdjustEnvironment;

            #if DEVELOPMENT_BUILD
            environment = AdjustEnvironment.Sandbox;
            #endif

            //AdjustConfig adjustConfig = new AdjustConfig(
            //    appToken,
            //    environment,
            //    true
            //);

            //adjustConfig.setLogLevel(settings.AdjustLogLevel);
            //adjustConfig.setSendInBackground(true);
            //Adjust.start(adjustConfig);

            #if TAFRA_ADJUST_PURCHASING
            if(settings.EnablePurcheseValidation)
            {
                ADJPEnvironment purchaseEnvironment = settings.AdjustPurchaseEnvironment;

                #if DEVELOPMENT_BUILD
                purchaseEnvironment = ADJPEnvironment.Sandbox;
                #endif

                ADJPConfig adjustPVConfig = new ADJPConfig(
                    appToken,
                    purchaseEnvironment //ADJPEnvironment.Production // ADJPEnvironment.Sandbox to validate sandbox receipts
                );
                adjustPVConfig.SetLogLevel(settings.PurchaseLogLevel);
                new GameObject("AdjustPurchase").AddComponent<AdjustPurchase>(); // do not remove or rename
                AdjustPurchase.Init(adjustPVConfig);

                #if TAFRA_IAP
                //TafraIAP.SetPurchaseValidator(new AdjustIAPValidator());
                #endif
            }
            #endif

            IsInitialized = true;

            if (settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", "Initialized Adjust SDK!", TafraDebugger.LogType.Info);
        }

        private static void ATTRequest()
        {
            if (settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", "Attempting to display an iOS AAT dialogue...", TafraDebugger.LogType.Info);

            Adjust.requestTrackingAuthorizationWithCompletionHandler((status) =>
            {
                if (settings.DebugLogs)
                    TafraDebugger.Log("Adjust SDK Manager", $"Displayed an iOS AAT dialogue. Authorization status is: {status}.", TafraDebugger.LogType.Info);

                switch (status)
                {
                    case 0:
                        // ATTrackingManagerAuthorizationStatusNotDetermined case
                        Debug.Log("Adjust: The user has not responded to the access prompt yet.");
                        break;
                    case 1:
                        // ATTrackingManagerAuthorizationStatusRestricted case
                        Debug.Log("Adjust: Access to app-related data is blocked at the device level.");
                        break;
                    case 2:
                        // ATTrackingManagerAuthorizationStatusDenied case
                        Debug.Log("Adjust: The user has denied access to app-related data for device tracking.");
                        break;
                    case 3:
                        // ATTrackingManagerAuthorizationStatusAuthorized case
                        Debug.Log("Adjust: The user has approved access to app-related data for device tracking.");
                        break;
                }

                PlayerPrefs.SetInt("TAFRA_ATT_STAT", 1);
                StartSDK();
            });
        }

        #region Callbacks
        private static void OnIAPPurchaseValidationResult(IAPValidationResults validationResult, UnityEngine.Purchasing.Product product)
        {
            #if DEVELOPMENT_BUILD
            return;
            #endif

            if(validationResult != IAPValidationResults.Passed)
            {
                if(settings.DebugLogs)
                    TafraDebugger.Log("Adjust SDK Manager", "Purchase isn't validated, will not log it.", TafraDebugger.LogType.Info);

                return;
            }

            string productID = product.definition.id;
            string productType = product.definition.type.ToString();
            decimal localizedPrice = product.metadata.localizedPrice;
            double price = decimal.ToDouble(localizedPrice);
            string currencyCode = product.metadata.isoCurrencyCode;
            string transactionID = product.transactionID;
            string eventToken = settings.GetAtiveIAPRevenueEventToken();

            if(validationResult == IAPValidationResults.Passed)
            {
                AdjustEvent adjustEvent = new AdjustEvent(eventToken);
                adjustEvent.setRevenue(price, currencyCode);
                adjustEvent.setTransactionId(transactionID);
                adjustEvent.addCallbackParameter("productID", productID);
                adjustEvent.addCallbackParameter("productType", productType);
                adjustEvent.addCallbackParameter("transactionID", transactionID);
                adjustEvent.addCallbackParameter("validator", "Adjust");
                Adjust.trackEvent(adjustEvent);
            }
            else
            {
                AdjustEvent adjustEvent;
                if(validationResult == IAPValidationResults.Failed)
                {
                    adjustEvent = new AdjustEvent("purchase_notverified");
                }
                else
                {
                    adjustEvent = new AdjustEvent("purchase_failed");
                }

                adjustEvent.addCallbackParameter("productID", productID);
                adjustEvent.addCallbackParameter("transactionID", transactionID);
                adjustEvent.addCallbackParameter("productType", productType);
                adjustEvent.addCallbackParameter("validator", "Adjust");
                Adjust.trackEvent(adjustEvent);
            }

            #region Lion Analytics
            IAPProduct productInfo = TafraIAP.GetProductInfo(product.definition.id);

            List<VirtualCurrency> receivedCurrencies = new List<VirtualCurrency>();

            if(productInfo.GameplayData.Consumable.consumable != null)
            {
                VirtualCurrency virtualCurrency = new VirtualCurrency(
                            name: productInfo.GameplayData.Consumable.consumable.ID,
                            type: productInfo.GameplayData.ConsumableType,
                            amount: (int)productInfo.GameplayData.Consumable.changeAmount);

                receivedCurrencies.Add(virtualCurrency);
            }
 
            IAPGameplayInfo gameplayInfo = new IAPGameplayInfo(
                receivedItems: new List<Item>(),
                receivedCurrencies: receivedCurrencies,
                purchaseLocation: productInfo.GameplayData.LastPlacement);

            var spent = new AnalyticsProduct();
            spent.AddRealCurrency(new RealCurrency(currencyCode, (float)price));
          
            var received = new AnalyticsProduct();
            if(gameplayInfo.ReceivedCurrencies != null)
            {
                foreach(VirtualCurrency currency in gameplayInfo.ReceivedCurrencies)
                {
                    received.AddVirtualCurrency(currency);
                }
            }

            Dictionary<string, object> additionalData = new Dictionary<string, object>();

            ValidationStatus validationStatus;
            if(validationResult == IAPValidationResults.Passed)
                validationStatus = ValidationStatus.Accepted;
            else if (validationResult == IAPValidationResults.Failed)
                validationStatus = ValidationStatus.Failed;
            else
                validationStatus = ValidationStatus.Unknown;

            additionalData.Add("validationStatus", validationStatus.ToString());
            additionalData.Add("productType", productType);
            additionalData.Add("validator", "Adjust");

            AnalyticsManager.Instance.InAppPurchase(
                productInfo.GameplayData.LastPlacement,
                spent,
                received,
                productInfo.GameplayData.LastPlacement,
                productID,
                transactionID);
            #endregion

            if(settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", $"Recorded an in-app purchase of product: {productID}, with placement: {productInfo.GameplayData.LastPlacement}!", TafraDebugger.LogType.Info);
        }

        private static void OnAdRevenueCollected(TafraAdInfo adInfo)
        {
            AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);

            adRevenue.setRevenue(adInfo.Revenue, "USD");
            adRevenue.setAdRevenueNetwork(adInfo.NetworkName);
            adRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
            adRevenue.setAdRevenuePlacement(adInfo.Placement);

            Adjust.trackAdRevenue(adRevenue);
           
            if (settings.DebugLogs)
                TafraDebugger.Log("Adjust SDK Manager", $"Adjust ad revenue collected ({adInfo.Revenue}).", TafraDebugger.LogType.Info);
        }
#endregion
    }
}
#endif