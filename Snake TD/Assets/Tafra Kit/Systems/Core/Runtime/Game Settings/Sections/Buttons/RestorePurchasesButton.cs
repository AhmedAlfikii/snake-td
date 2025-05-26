using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.IAP;

namespace TafraKit.Internal
{
    public class RestorePurchasesButton : GameSettingsButton
    {
        public override bool AreConditionsSatisfied()
        {
            #if UNITY_EDITOR
            return true;
            #endif

            return TafraApplication.Platform == RuntimePlatform.IPhonePlayer;
        }

        protected override void OnClick()
        {
            #if TAFRA_IAP
            TafraIAP.RestoreIOSPurchases();
            #endif
        }
    }
}