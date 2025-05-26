using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class ObjectMotion : MonoBehaviour
    {
        [SerializeField] protected bool playOnEnable = true;
        [SerializeField] private bool useUnscaledTime;
        [Range(0, 1)]
        [SerializeField] protected float prewarmValue;
        [SerializeField] protected WrapMode wrapMode = WrapMode.PingPong;
        [SerializeField] protected EasingType easingType = new EasingType(MotionType.EaseInOut, new TafraKit.EasingEquationsParameters());
        [SerializeField] protected float cycleDuration = 0.5f;
        [SerializeField] protected FloatRange firstCycleDelay;
        [SerializeField] protected FloatRange newCyclesDelay;

        protected IEnumerator motionEnum;
        protected bool isInitialized;
        protected bool isPlaying;
        protected float prewarmDuration;

        protected virtual void Awake()
        {
            if(!isInitialized)
                Initialize();
        }

        protected virtual void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        IEnumerator Motion(float delay, float prewarmedDuration, bool inverted)
        {
            if (!useUnscaledTime)
                yield return Yielders.GetWaitForSeconds(delay);
            else
                yield return Yielders.GetWaitForSecondsRealtime(delay);

            float time = useUnscaledTime ? Time.unscaledTime : Time.time;

            float startTime = time - prewarmedDuration;
            
            float endTime = startTime + cycleDuration;

            while (time < endTime)
            {
                float rawT = (time - startTime) / cycleDuration;

                float easedT = MotionEquations.GetEaseFloat(rawT, easingType.Easing, easingType.Parameters);

                ApplyMotion(easedT, rawT, inverted);

                yield return null;

                time = useUnscaledTime ? Time.unscaledTime : Time.time;
            }

            ApplyMotion(1, 1, inverted);

            switch(wrapMode)
            {
                case WrapMode.Loop:
                    motionEnum = Motion(newCyclesDelay.GetRandomValue(), 0, false);
                    StartCoroutine(motionEnum);
                    break;
                case WrapMode.PingPong:
                    motionEnum = Motion(newCyclesDelay.GetRandomValue(), 0, !inverted);
                    StartCoroutine(motionEnum);
                    break;
            }
        }

        protected virtual void Initialize()
        {
            prewarmDuration = prewarmValue * cycleDuration;
            isInitialized = true;
        }
        protected virtual void ApplyMotion(float easedT, float rawT, bool inverted)
        {

        }

        public virtual void GoToNormalState()
        {
            if(!isInitialized)
                Initialize();
        }

        public void SetPlayOnEnable(bool active)
        {
            playOnEnable = active;
        }

        public void Play()
        {
            if(!isInitialized)
                Initialize();

            isPlaying = true;

            if (!gameObject.activeInHierarchy)
                return;

            if (motionEnum != null)
                StopCoroutine(motionEnum);

            motionEnum = Motion(firstCycleDelay.GetRandomValue(), prewarmDuration, false);

            StartCoroutine(motionEnum);
        }

        public void Stop()
        {
            isPlaying = false;

            if (motionEnum != null)
                StopCoroutine(motionEnum);
        }

        public void Seek(float rawT)
        {
            float easedT = MotionEquations.GetEaseFloat(rawT, easingType.Easing, easingType.Parameters);
            ApplyMotion(easedT, rawT, false);
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }
    }
}