using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ZTweeners
{
    public class ZTweenVector3 : ZTween
    {
        public Vector3 CurValue { get; private set; }

        private Vector3 startValue;
        private Vector3 targetValue;

        public ZTweenVector3() { }

        public void Construct(Vector3 startValue, Vector3 targetValue, float duration, MonoBehaviour player)
        {
            this.startValue = startValue;
            this.targetValue = targetValue;
            this.duration = duration;
            this.player = player;
         
            ResetEvents();
        }

        protected override void Scrub(float t)
        {
            CurValue = Vector3.LerpUnclamped(startValue, targetValue, t);
        }

        public ZTweenVector3 SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }
        public ZTweenVector3 SetEasingType(EasingType easingType)
        {
            this.easingType = easingType;
            return this;
        }
        public ZTweenVector3 SetUnscaledTimeUsage(bool useUnscaledTime)
        {
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }
        public ZTweenVector3 SetOnActualStart(UnityAction action)
        {
            onActualStart.AddListener(action);
            return this;
        }
        public ZTweenVector3 SetOnUpdated(UnityAction action)
        {
            onUpdated.AddListener(action);
            return this;
        }
        public ZTweenVector3 SetOnCompleted(UnityAction action)
        {
            onCompleted.AddListener(action);
            return this;
        }
        public ZTweenVector3 SetOnTerminated(UnityAction action)
        {
            onStopped.AddListener(action);
            return this;
        }
    }
}
