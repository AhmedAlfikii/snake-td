using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ZTweeners
{
    public class ZTweenRect : ZTween
    {
        private RectTransform myRT;
        private RectTransform targetRT;
        private bool controlScale = true;
        private Vector2 targetSize;
        private Vector2 startSizeDelta;
        private Vector3 startPosition;
        private Vector3 startLocalScale;
        private Quaternion startRotation;
        private Vector3 positionDiff;

        public ZTweenRect() { }

        public void Construct(RectTransform myRT, RectTransform targetRT, float duration, MonoBehaviour player)
        {
            if (myRT == null)
            {
                Debug.LogError("You're trying to construct a tween with a null rect transform.");
                return;
            }

            this.myRT = myRT;
            this.targetRT = targetRT;
            this.duration = duration;
            this.player = player;

            ResetEvents();

            targetSize = new Vector2(targetRT.rect.width, targetRT.rect.height);

            startSizeDelta = myRT.sizeDelta;
            startPosition = myRT.position;
            startLocalScale = myRT.localScale;
            startRotation = myRT.rotation;

            Vector2 pivotDiff = myRT.pivot - targetRT.pivot;
            positionDiff = new Vector3(targetRT.rect.width * pivotDiff.x, targetRT.rect.height * pivotDiff.y, 0) * myRT.lossyScale.x;
        }

        protected override void Scrub(float t)
        {
            if (myRT == null) return;

            targetSize = new Vector2(targetRT.rect.width, targetRT.rect.height);
            myRT.sizeDelta = Vector2.LerpUnclamped(startSizeDelta, targetSize, t);
            myRT.position = Vector3.LerpUnclamped(startPosition, targetRT.position + positionDiff, t);

            if(controlScale)
                myRT.localScale = Vector3.LerpUnclamped(startLocalScale, targetRT.localScale, t);

            myRT.rotation = Quaternion.LerpUnclamped(startRotation, targetRT.rotation, t);
        }

        public ZTweenRect SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }
        public ZTweenRect SetEasingType(EasingType easingType)
        {
            this.easingType = easingType;
            return this;
        }
        public ZTweenRect SetUnscaledTimeUsage(bool useUnscaledTime)
        {
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }
        public ZTweenRect SetOnActualStart(UnityAction action)
        {
            onActualStart.AddListener(action);
            return this;
        }
        public ZTweenRect SetOnUpdated(UnityAction action)
        {
            onUpdated.AddListener(action);
            return this;
        }
        public ZTweenRect SetOnCompleted(UnityAction action)
        {
            onCompleted.AddListener(action);
            return this;
        }
        public ZTweenRect SetOnTerminated(UnityAction action)
        {
            onStopped.AddListener(action);
            return this;
        }

        public ZTweenRect SetControlScale(bool controlScale)
        {
            this.controlScale = controlScale;
            return this;
        }
    }
}
