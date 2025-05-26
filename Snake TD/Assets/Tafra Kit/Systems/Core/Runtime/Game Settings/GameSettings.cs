using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI;
using TafraKit.Internal;

namespace TafraKit
{
    public static class GameSettings
    {
        private static GameSettingsSettings settings;
        private static bool enabled;
        private static GameSettingsPopup popup;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<GameSettingsSettings>();

            enabled = settings.Enabled;

            if (settings == null || !enabled || settings.Popup == null)
                return;

            popup = GameObject.Instantiate<GameSettingsPopup>(settings.Popup);


            GameObject.DontDestroyOnLoad(popup.gameObject);
        }

        public static void OpenPopup()
        {
            if (!enabled)
            {
                TafraDebugger.Log("Game Settings", "Game Settings isn't enabled, make sure to enable it from Tafra Kit's window.", TafraDebugger.LogType.Error);
                return;
            }

            if (popup == null)
            {
                TafraDebugger.Log("Game Settings", "Couldn't find a popup to open, make sure that you've assigned a popup in Tafra Kit window's Game Settings section.", TafraDebugger.LogType.Error);
                return;
            }

            if (popup.GraphicRaycaster)
                popup.GraphicRaycaster.enabled = true;

            if (settings.PauseOnOpen)
                TimeScaler.SetTimeScale("gameSettingsPopup", 0);

            popup.UpdateSections();
            popup.UIEG.ChangeVisibility(true);
        }
        public static void ClosePopup()
        {
            if (!enabled)
            {
                TafraDebugger.Log("Game Settings", "Game Settings isn't enabled, make sure to enable it from Tafra Kit's window.", TafraDebugger.LogType.Error);
                return;
            }

            if (popup == null)
            {
                TafraDebugger.Log("Game Settings", "Couldn't find a popup to open, make sure that you've assigned a popup in Tafra Kit window's Game Settings section.", TafraDebugger.LogType.Error);
                return;
            }

            if (popup.GraphicRaycaster)
                popup.GraphicRaycaster.enabled = false;

            if (settings.PauseOnOpen)
                TimeScaler.RemoveTimeScaleControl("gameSettingsPopup");

            popup.UIEG.ChangeVisibility(false);
        }
    }
}