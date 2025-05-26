using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TafraKit.ZTweeners;

namespace TafraKit.UI
{
    public class RectToRect : MonoBehaviour
    {
        [SerializeField] private RectTransform[] rectsPlaces;
        [SerializeField] private EasingType easingType;
        [SerializeField] private float duration = 1f;
        [SerializeField] private bool useUnscaledTime;
        [SerializeField] private int defaultPlace = 0;

        private bool initialized;
        private RectTransform myRT;
        private bool atA = true;
        private ZTweenRect myTween = new ZTweenRect();
        private int curPlacement;

        private void Start()
        {
            if (!initialized)
                Initialize();

            if (defaultPlace >= 0 && defaultPlace < rectsPlaces.Length)
                GoToRect(defaultPlace, 0);
        }

        private void OnDisable()
        {
            if (myTween.IsPlaying)
                myTween.Stop();
        }

        private void Initialize()
        {
            myRT = GetComponent<RectTransform>();
            myRT.SetAnchorsSoft(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            initialized = true;
        }

        public void GoToRect(int rectIndex)
        {
            if (!initialized)
                Initialize();

            myRT.ZTweenAdaptRect(myTween, rectsPlaces[rectIndex], duration).SetEasingType(easingType).SetUnscaledTimeUsage(useUnscaledTime);
            atA = !atA;
        }
        public void GoToNextRect()
        {
            curPlacement = (curPlacement + 1) % rectsPlaces.Length;

            GoToRect(curPlacement);
        }
        public void RecatchCurrentRect()
        {
            GoToRect(curPlacement);
        }
        public void GoToRect(int rectIndex, float customDuration)
        {
            if (!initialized)
                Initialize();

            curPlacement = rectIndex;

            myRT.ZTweenAdaptRect(myTween, rectsPlaces[rectIndex], customDuration).SetEasingType(easingType).SetUnscaledTimeUsage(useUnscaledTime);
                
            atA = !atA;
        }
        public void GoToRect(RectTransform targetRect, float customDuration)
        {
            if (!initialized)
                Initialize();

            myRT.ZTweenAdaptRect(myTween, targetRect, customDuration).SetEasingType(easingType).SetUnscaledTimeUsage(useUnscaledTime);
        }
    }
}