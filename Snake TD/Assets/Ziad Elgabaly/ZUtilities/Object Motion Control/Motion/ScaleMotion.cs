using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [Serializable]
    public class ScaleMotion : MotionBase
    {
        public Vector3 TargetScale;

        private Transform transform;

        public ScaleMotion(float delay, float duration, EasingType motionType, bool motionTargetIsObjectInitialValue, Vector3 targetScale)
            : base(delay, duration, motionType, motionTargetIsObjectInitialValue)
        {
            TargetScale = targetScale;
        }

        public void Initialize(MonoBehaviour motionPlayer, Transform motionTransform)
        {
            player = motionPlayer;
            transform = motionTransform;

            if (MotionTargetIsObjectInitialValue)
                TargetScale = transform.localScale;

            isInitialized = true;
        }

        public override void Play(bool immediate)
        {
            base.Play(immediate);

            if (!immediate)
            {
                motionEnumerator = PlayingMotion();
                player.StartCoroutine(motionEnumerator);
            }
            else
            {
                transform.localScale = TargetScale;
            }
        }

        IEnumerator PlayingMotion()
        {
            onStarted?.Invoke();
            
            if (Delay > 0)
                yield return Yielders.GetWaitForSeconds(Delay);

            float startTime = Time.time;
            float endTime = startTime + Duration;

            Vector3 startScale = transform.localScale;

            float t;
            while (Time.time < endTime)
            {
                t = (Time.time - startTime) / Duration;
                t = MotionEquations.GetEaseFloat(t, MotionType);

                transform.localScale = Vector3.LerpUnclamped(startScale, TargetScale, t);

                yield return null;
            }

            transform.localScale = TargetScale;

            isPlaying = false;
            
            onEnded?.Invoke();
        }
    }
}