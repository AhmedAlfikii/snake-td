using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;

namespace TafraKit.UI
{
    public static class UIAlertsManager
    {
        private static UIAlertsManagerSettings settings;
        private static Sprite defaultIcon;
        private static Dictionary<UIAlertState, Sprite> spriteByState = new Dictionary<UIAlertState, Sprite>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<UIAlertsManagerSettings>();

            if(settings == null)
                return;


            for(int i = 0; i < settings.Alerts.Count; i++)
            {
                var alert = settings.Alerts[i];

                spriteByState.TryAdd(alert.state, alert.icon);
            }

            defaultIcon = settings.DefaultIcon;
        }

        public static Sprite GetStateIcon(UIAlertState state)
        {
            if(spriteByState.TryGetValue(state, out var icon))
                return icon;
            else
                return defaultIcon;
        }
    }
}