using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework.Constraints;
using TafraKit.Internal;
#if TAFRA_CINEMACHINE
using Unity.Cinemachine;
#endif

namespace TafraKit.Cinemachine
{
    public static class ScreenShaker
    {
        private static List<ShakeableCamera> cameras = new List<ShakeableCamera>();
        private static IEnumerator shakingEnum;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        { 

        }

        private static IEnumerator Shaking(float amplitude, float frequency, float duration)
        {
            #if TAFRA_CINEMACHINE
            for (int i = 0; i < cameras.Count; i++)
            {
                var cam = cameras[i];

                cam.NoiseComponent.AmplitudeGain = amplitude;
                cam.NoiseComponent.FrequencyGain = frequency;
            }

            yield return Yielders.GetWaitForSeconds(duration);

            for(int i = 0; i < cameras.Count; i++)
            {
                var cam = cameras[i];

                cam.NoiseComponent.AmplitudeGain = 0;
                cam.NoiseComponent.FrequencyGain = 0;
            }
            #else
            yield break;
            #endif
        }

        public static void RegisterCamera(ShakeableCamera shakeableCamera)
        {
            if (!cameras.Contains(shakeableCamera))
                cameras.Add(shakeableCamera);
        }
        public static void UnregisterCamera(ShakeableCamera shakeableCamera)
        {
            cameras.Remove(shakeableCamera);
        }

        public static void Shake(float amplitude, float frequency, float duration) 
        {
            if(shakingEnum != null)
                GeneralCoroutinePlayer.StopCoroutine(shakingEnum);

            shakingEnum = Shaking(amplitude, frequency, duration);

            GeneralCoroutinePlayer.StartCoroutine(shakingEnum);
        }
    }
}