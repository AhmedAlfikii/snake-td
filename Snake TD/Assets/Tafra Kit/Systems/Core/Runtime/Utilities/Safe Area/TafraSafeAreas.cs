using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public static class TafraSafeAreas
    {
        private static SafeAreaSettings settings;
        private static Rect safeArea;
        private static float topMargin;
        private static float botMargin;
        private static float leftMargin;
        private static float rightMargin;

        public static UnityEvent OnAreaUpdated = new UnityEvent();
        
        public static Rect SafeArea => safeArea;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<SafeAreaSettings>();

            UpdateAreaUsingDefaultMargins();
        }

        public static void UpdateAreaUsingDefaultMargins()
        {
            if (settings != null)
                UpdateArea(settings.TopMargin, settings.BotMargin, settings.LeftMargin, settings.RightMargin);
            else
                UpdateArea(0, 0, 0, 0);
        }
        public static void UpdateArea(float top, float bot, float left, float right)
        {
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;

            topMargin = top * screenHeight;
            botMargin = bot * screenHeight;
            leftMargin = left * screenWidth;
            rightMargin = right * screenWidth;

            safeArea = Screen.safeArea;

            safeArea.yMin += botMargin;
            safeArea.yMax -= topMargin;
            safeArea.xMin += leftMargin;
            safeArea.xMax -= rightMargin;

            OnAreaUpdated?.Invoke();
        }
    }
}