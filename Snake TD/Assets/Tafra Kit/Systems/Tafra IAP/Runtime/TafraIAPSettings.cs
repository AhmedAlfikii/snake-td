#if TAFRA_IAP
using System.Collections;
using System.Collections.Generic;
using TafraKit.Ads;
using UnityEngine;
using UnityEngine.Purchasing;

namespace TafraKit.IAP
{
    public class TafraIAPSettings : SettingsModule
    {
        public bool Enabled = true;
        public IAPProduct[] Products;

        [Header("Validation")]
        public bool BypassValidationFailure = true;

        [Header("Loading Screens")]
        public ZUIScreen ProcessPurchaseLoadingScreen;
        public ZUIScreen iOSRestorationLoadingScreen;

        [Header("Testing")]
        public bool DebugLogs;
        public FakeStoreUIMode FakeStoreUI;

        public override int Priority => 55;

        public override string Name => "Monetization/Tafra IAP";

        public override string Description => "Handle real money purchases and restoration.";
    }
}
#endif