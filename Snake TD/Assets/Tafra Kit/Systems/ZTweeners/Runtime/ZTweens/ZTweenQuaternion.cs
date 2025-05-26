using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ZTweeners
{
    public class ZTweenQuaternion : ZTween
    {
        public Quaternion CurValue { get; private set; }

        private Quaternion startValue;
        private Quaternion targetValue;

        public ZTweenQuaternion() { }

        public void Construct(Quaternion startValue, Quaternion targetValue, float duration, MonoBehaviour player)
        {
            this.startValue = startValue;
            this.targetValue = targetValue;
            this.duration = duration;
            this.player = player;

            ResetEvents();
        }

        protected override void Scrub(float t)
        {
            CurValue = Quaternion.LerpUnclamped(startValue, targetValue, t);
        }

        public ZTweenQuaternion SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }
        public ZTweenQuaternion SetEasingType(EasingType easingType)
        {
            this.easingType = easingType;
            return this;
        }
        public ZTweenQuaternion SetUnscaledTimeUsage(bool useUnscaledTime)
        {
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }
        public ZTweenQuaternion SetOnActualStart(UnityAction action)
        {
            onActualStart.AddListener(action);
            return this;
        }
        public ZTweenQuaternion SetOnUpdated(UnityAction action)
        {
            onUpdated.AddListener(action);
            return this;
        }
        public ZTweenQuaternion SetOnCompleted(UnityAction action)
        {
            onCompleted.AddListener(action);
            return this;
        }
        public ZTweenQuaternion SetOnTerminated(UnityAction action)
        {
            onStopped.AddListener(action);
            return this;
        }
    }
}
