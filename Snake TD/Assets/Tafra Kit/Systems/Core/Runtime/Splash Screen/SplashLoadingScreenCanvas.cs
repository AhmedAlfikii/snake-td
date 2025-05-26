using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit
{
    public class SplashLoadingScreenCanvas : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIElementsGroup uieg;
        [SerializeField] private Image background;
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TextMeshProUGUI loadingProgressTXT;

        public UIElementsGroup UIEG => uieg;
        public Image Background => background;
        public Slider LoadingBar => loadingBar;
        public TextMeshProUGUI LoadingProgressTXT => loadingProgressTXT;
    }
}