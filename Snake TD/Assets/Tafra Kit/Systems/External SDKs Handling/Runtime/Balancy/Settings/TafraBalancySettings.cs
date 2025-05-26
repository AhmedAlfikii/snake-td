using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TAFRA_BALANCY
using Balancy;
#endif

#if TAFRA_BALANCY
namespace TafraKit.ExternalSDKs.BalancySDK
{
    public class TafraBalancySettings : SettingsModule
    {
        [SerializeField] private bool enabled;
        [SerializeField] private string apiGameID;
        [SerializeField] private string publicKey;
        [SerializeField] private bool autoLoginWithDeviceID;
        [SerializeField] private PreInitType preInitType = PreInitType.None;
        [SerializeField] private Constants.Environment editorEnvironment;
        [SerializeField] private Constants.Environment buildEnvironment;

        [Header("Items")]
        [SerializeField] private BalancyItemConsumable[] balancyConsumables;
        [SerializeField] private BalancyItemStorable[] balancyStorables;

        public bool Enabled => enabled;
        public string APIGameID => apiGameID;
        public string PublicKey => publicKey;
        public bool AutoLoginWithDeviceID => autoLoginWithDeviceID;
        public PreInitType PreInitType => preInitType;
        public Constants.Environment Environment 
        {
            get
            {
#if UNITY_EDITOR
                return editorEnvironment;
#endif

                return buildEnvironment;
            }
        }
        public BalancyItemConsumable[] BalancyConsumables => balancyConsumables;
        public BalancyItemStorable[] BalancyStorables => balancyStorables;
        public override int Priority => 20;
        public override string Name => "External SDKs/Balancy";
        public override string Description => "Control how Balancy behaves.";
    }
}
#endif