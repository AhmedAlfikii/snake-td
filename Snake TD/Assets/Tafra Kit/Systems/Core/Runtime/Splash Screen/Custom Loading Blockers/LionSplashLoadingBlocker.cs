#if HAS_LION_APPLOVIN_SDK
using LionStudios.Suite.Core;
using System;
using UnityEngine;

namespace TafraKit.Internal
{
    public class LionSplashLoadingBlocker
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if(!SplashScreen.IsInSplashScreen)
                return;

            if(!LionCore.IsInitialized)
            {
                SplashScreen.AddExitBlocker("lion_core");

                LionCore.OnInitialized += OnLionCoreInitialized;
            }

            if(!MaxSdk.IsInitialized())
            {
                SplashScreen.AddExitBlocker("max");

                MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxLoaded;
            }
        }

        private static void OnLionCoreInitialized()
        {
            SplashScreen.RemoveExitBlocker("lion_core");
        }

        private static void OnMaxLoaded(MaxSdkBase.SdkConfiguration configuration)
        {
            SplashScreen.RemoveExitBlocker("max");
        }
    }
}
#endif