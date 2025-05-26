using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TAFRA_VIBRATION
using Lofelt.NiceVibrations;
#endif

namespace TafraKit.Vibration
{
    public static class TafraVibration
    {
        private static VibrationSettings settings;
        private static bool isEnabled;
        private static bool isMuted;

        public static bool IsEnabled => isEnabled;
        public static bool IsMuted
        {
            get
            {
                return isMuted;
            }
            set
            {
                isMuted = value;
                PlayerPrefs.SetInt("TAFRAKIT_VIBRATION_ISMUTED", isMuted ? 1 : 0);

                #if TAFRA_VIBRATION
                HapticController.hapticsEnabled = !isMuted;
                #endif
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<VibrationSettings>();

            if (settings && settings.Enabled)
            {
                isEnabled = true;

                isMuted = PlayerPrefs.GetInt("TAFRAKIT_VIBRATION_ISMUTED", settings.MutedByDefault ? 1 : 0) == 1 ? true : false;

                #if TAFRA_VIBRATION
                HapticController.hapticsEnabled = !isMuted;
                #endif
            }
        }
    }
}