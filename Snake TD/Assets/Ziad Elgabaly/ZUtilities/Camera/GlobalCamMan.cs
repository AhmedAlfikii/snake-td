using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZUtilities
{
    public class GlobalCamMan
    {
        #region Classes, Structs & Enums
        [System.Serializable]
        public struct FlashData
        {
            public Color Color;
            [Range(0, 1)]
            public float Power;
            public float ShowDuration;
            public float VisibleDuration;
            public float HideDuration;
        }
        #endregion

        public static CamChangeViewEvent OnChangeView;
        public static CamShakeEvent OnShake;
        public static CamFlashEvent OnFlash;

        private static bool initialized;

        public static void Initialize()
        {
            if (initialized) return;

            OnShake = new CamShakeEvent();
            OnFlash = new CamFlashEvent();
            OnChangeView = new CamChangeViewEvent();

            initialized = true;
        }

        public static void ChangeView(ZCameraView view, float delay, float duration, Action onReachedView = null)
        {
            OnChangeView?.Invoke(view, delay, duration, onReachedView);
        }

        public static void Shake(float power, float duration)
        {
            OnShake?.Invoke(power, duration);
        }

        public static void Flash(FlashData data)
        {
            OnFlash?.Invoke(data);
        }
    }
    public class CamShakeEvent : UnityEvent<float, float> { }
    public class CamFlashEvent : UnityEvent<GlobalCamMan.FlashData> { }
    public class CamChangeViewEvent : UnityEvent<ZCameraView, float, float, Action> { }
}