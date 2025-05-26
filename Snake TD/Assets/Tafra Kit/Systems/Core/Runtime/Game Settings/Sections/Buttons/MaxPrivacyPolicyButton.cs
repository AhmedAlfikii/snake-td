using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.IAP;

namespace TafraKit.Internal
{
    public class MaxPrivacyPolicyButton : GameSettingsButton
    {
        public override bool AreConditionsSatisfied()
        {
            #if UNITY_EDITOR
            return true;
            #endif

            #if !HAS_LION_APPLOVIN_SDK
            return false;
            #else
            return MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography == MaxSdkBase.ConsentFlowUserGeography.Gdpr;
            #endif
        }

        protected override void OnClick()
        {

            #if HAS_LION_APPLOVIN_SDK
            MaxSdk.CmpService.ShowCmpForExistingUser(error =>
            {
                if(error != null)
                {
                    Debug.Log(error.Message + " ERROR CODE: " + error.Code);
                }
                else
                {
                    Debug.Log("Max Consent Showing");
                }
            });
            #endif
        }
    }
}