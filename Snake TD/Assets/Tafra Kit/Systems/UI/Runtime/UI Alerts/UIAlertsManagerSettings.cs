using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;

namespace TafraKit.UI
{
    public class UIAlertsManagerSettings : SettingsModule
    {
        [System.Serializable]
        public class AlertByState
        {
            public UIAlertState state;
            public Sprite icon;
        }
        [Tooltip("This icon will be used in case no icon was found for the desired state.")]
        [SerializeField] private Sprite defaultIcon;
        [SerializeField] private List<AlertByState> alerts;

        public List<AlertByState> Alerts => alerts;
        public Sprite DefaultIcon => defaultIcon;
        public override int Priority => 30;
        public override string Name => "UI/UI Alerts Manager";
        public override string Description => "Control how UI alert (dots) are displayed in the game";
    }
}