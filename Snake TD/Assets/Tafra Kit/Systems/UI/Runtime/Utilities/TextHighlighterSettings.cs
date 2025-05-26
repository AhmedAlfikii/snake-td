using UnityEngine;

namespace TafraKit.UI
{
    public class TextHighlighterSettings : SettingsModule
    {
        [Header("Positive")]
        [SerializeField] private bool enablePositivePrefix;
        [SerializeField] private TafraString positivePrefix;
        [SerializeField] private bool enablePositivePostfix;
        [SerializeField] private TafraString positivePostfix;

        [Header("Negative")]
        [SerializeField] private bool enableNegativePrefix;
        [SerializeField] private TafraString negativePrefix;
        [SerializeField] private bool enableNegativePostfix;
        [SerializeField] private TafraString negativePostfix;

        public bool EnablePositivePrefix => enablePositivePrefix;
        public string PositivePrefix => positivePrefix.Value;
        public bool EnablePositivePostfix => enablePositivePostfix;
        public string PositivePostfix => positivePostfix.Value;
        public bool EnableNegativePrefix => enableNegativePrefix;
        public string NegativePrefix => negativePrefix.Value;
        public bool EnableNegativePostfix => enableNegativePostfix;
        public string NegativePostfix => negativePostfix.Value;

        public override string Name => "UI/Text Highlighter";
        public override string Description => "Handle how text should be higlighted in the game.";
    }
}