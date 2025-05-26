using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ZTweeners
{
    public class ZTweenFloat : ZTween
    {
        public float CurValue { get; private set; }

        private UnityEvent onFloatUpdated;
        private float startValue;
        private float targetValue;

        public ZTweenFloat(UnityAction onUpdated) 
        {
            onFloatUpdated = new UnityEvent();
            onFloatUpdated.AddListener(onUpdated);
        }

        public void Construct(float startValue, float targetValue, float duration, MonoBehaviour player)
        {
            this.startValue = startValue;
            this.targetValue = targetValue;
            this.duration = duration;
            this.player = player;

            ResetEvents();
        }

        protected override void Scrub(float t)
        {
            CurValue = Mathf.LerpUnclamped(startValue, targetValue, t);
            onFloatUpdated?.Invoke();
        }

        public ZTweenFloat SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }
        public ZTweenFloat SetEasingType(EasingType easingType)
        {
            this.easingType = easingType;
            return this;
        }
        public ZTweenFloat SetUnscaledTimeUsage(bool useUnscaledTime)
        {
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }
        public ZTweenFloat SetOnActualStart(UnityAction action)
        {
            onActualStart.AddListener(action);
            return this;
        }
        public ZTweenFloat SetOnUpdated(UnityAction action)
        {
            onUpdated.AddListener(action);
            return this;
        }
        public ZTweenFloat SetOnCompleted(UnityAction action)
        {
            onCompleted.AddListener(action);
            return this;
        }
        public ZTweenFloat SetOnTerminated(UnityAction action)
        {
            onStopped.AddListener(action);
            return this;
        }
    }
}
