using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Internal;

namespace TafraKit
{
    public class TimeScaler : MonoBehaviour
    {
        #region Private Fields
        private static InfluenceReceiver<float> influenceReceiver = new InfluenceReceiver<float>(ShouldReplace, OnActiveInfluenceUpdated, OnActiveControllerUpdated, OnAllInfluencesCleared);

        private static bool prioritizeLowerScale = true;
        #endregion

        #region Private Functions
        private static void OnActiveInfluenceUpdated(float timeScale) 
        {
            Time.timeScale = timeScale;
        }
        private static void OnActiveControllerUpdated(string controllerId, float timeScale) 
        {
        }
        private static void OnAllInfluencesCleared() 
        {
            Time.timeScale = 1;
        }
        private static bool ShouldReplace(float newScale, float oldScale)
        {
            if (prioritizeLowerScale)
                return newScale < oldScale;
            else
                return newScale > oldScale;
        }
        private static IEnumerator SettingTimeScaleForDuration(string controllerId, float timeScale, float duration)
        {
            influenceReceiver.AddInfluence(controllerId, timeScale);

            yield return Yielders.GetWaitForSecondsRealtime(duration);

            influenceReceiver.RemoveInfluence(controllerId);
        }
        #endregion

        #region Public Functions
        public static void SetTimeScale(string controllerId, float timeScale)
        {
            influenceReceiver.AddInfluence(controllerId, timeScale);
        }

        public static void RemoveTimeScaleControl(string controllerId)
        {
            influenceReceiver.RemoveInfluence(controllerId);
        }

        public static void SetTimeScaleForDuration(string controllerId, float timeScale, float duration)
        {
            GeneralCoroutinePlayer.StartCoroutine(SettingTimeScaleForDuration(controllerId, timeScale, duration));
        }

        public static void RemoveAllTimeScaleControls()
        {
            influenceReceiver.RemoveAllInfluences();
        }
        #endregion
    }
}