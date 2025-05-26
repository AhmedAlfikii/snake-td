using UnityEngine;

namespace TafraKit.Internal
{
    public class SplashScreenSettings : SettingsModule
    {
        [SerializeField] private bool enabled;
        [SerializeField] private float logoDuration = 4.5f;

        [Header("Loading Screen")]
        [SerializeField] private SplashLoadingScreenCanvas loadingScreenCanvas;

        public bool Enabled => enabled;
        public float LogoDuration => logoDuration;
        public SplashLoadingScreenCanvas LoadingScreenCanvas => loadingScreenCanvas;
        public override string Name => "General/Splash/Splash Screen";
        public override string Description => "Handle how the splash screen behaves.";
        public override int Priority => 0;
    }
}