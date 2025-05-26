using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.Performance
{
    public static class PerformanceManager
    {
        private static PerformanceSettings settings;
      
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            settings = TafraSettings.GetSettings<PerformanceSettings>();

            if (settings && settings.Enabled)
            {
                if(settings.ControlFrameRate)
                {
                    if(!Application.isEditor)
                        Application.targetFrameRate = settings.TargetFrameRate;
                    else if(settings.ApplyFrameRateToEditor)
                        Application.targetFrameRate = settings.TargetFrameRate;
                }

                if(settings.ScreenStayAwake)
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }
    }
}