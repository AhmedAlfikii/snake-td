using UnityEngine;

namespace TafraKit.UI
{
    public static class TextHighlighter
    {
        private static TextHighlighterSettings settings;
       
        private static bool hasPositivePrefix;
        private static bool hasPositivePostfix;
        private static string positivePrefix;
        private static string positivePostfix;

        private static bool hasNegativePrefix;
        private static bool hasNegativePostfix;
        private static string negativePrefix;
        private static string negativePostfix;

        public static bool HasPositivePrefix => hasPositivePrefix;
        public static bool HasPositivePostfix => hasPositivePostfix;
        public static string PositivePrefix => positivePrefix;
        public static string PositivePostfix => positivePostfix;
        public static bool HasNegativePrefix => hasNegativePrefix;
        public static bool HasNegativePostfix => hasNegativePostfix;
        public static string NegativePrefix => negativePrefix;
        public static string NegativePostfix => negativePostfix;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<TextHighlighterSettings>();

            hasPositivePrefix = settings.EnablePositivePrefix;
            hasPositivePostfix = settings.EnablePositivePostfix;
            positivePrefix = settings.PositivePrefix;
            positivePostfix = settings.PositivePostfix;

            hasNegativePrefix = settings.EnableNegativePrefix;
            hasNegativePostfix = settings.EnableNegativePostfix;
            negativePrefix = settings.NegativePrefix;
            negativePostfix = settings.NegativePostfix;
        }
    }
}