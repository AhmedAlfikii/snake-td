using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ZTweeners
{
    public class ZTweenColor : ZTween
    {
        public Color CurColor { get; private set; }

        private Color startColor;
        private Color targetColor;

        private UnityEvent onColorUpdated;
        public ZTweenColor() { }
        public ZTweenColor(UnityAction onUpdated) 
        {
            onColorUpdated = new UnityEvent();
            onColorUpdated.AddListener(onUpdated);
        }

        public void Construct(Color startColor, Color targetColor, float duration, MonoBehaviour player)
        {
            this.startColor = startColor;
            this.targetColor = targetColor;
            this.duration = duration;
            this.player = player;
         
            ResetEvents();
        }

        protected override void Scrub(float t)
        {
            CurColor = Color.LerpUnclamped(startColor, targetColor, t);
            onColorUpdated?.Invoke();
        }

        public ZTweenColor SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }
        public ZTweenColor SetEasingType(EasingType easingType)
        {
            this.easingType = easingType;
            return this;
        }
        public ZTweenColor SetUnscaledTimeUsage(bool useUnscaledTime)
        {
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }
        public ZTweenColor SetOnActualStart(UnityAction action)
        {
            onActualStart.AddListener(action);
            return this;
        }
        public ZTweenColor SetOnUpdated(UnityAction action)
        {
            onUpdated.AddListener(action);
            return this;
        }
        public ZTweenColor SetOnCompleted(UnityAction action)
        {
            onCompleted.AddListener(action);
            return this;
        }
        public ZTweenColor SetOnTerminated(UnityAction action)
        {
            onStopped.AddListener(action);
            return this;
        }
    }
}
