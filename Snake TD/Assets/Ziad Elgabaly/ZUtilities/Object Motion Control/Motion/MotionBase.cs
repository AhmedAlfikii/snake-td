using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [Serializable]
    public class MotionBase
    {
        [Tooltip("The delay before requesting to play this motion and actually playing it.")]
        public float Delay;
        [Tooltip("The duration it takes for this motion to reach its target.")]
        public float Duration;
        [Tooltip("The easing type of this motion.")]
        public EasingType MotionType;
        [Tooltip("Should the target state of this motion be the same as what this object was on initialization? e.g. if this motion is a scale motion, and the object's scale is found to be (1,1,1) during initialization, then it will be used as the target scale for this motion.")]
        public bool MotionTargetIsObjectInitialValue;

        #region Events
        public UnityEvent onStarted;
        public UnityEvent onEnded;
        #endregion

        protected bool isInitialized;
        protected bool isPlaying;
        protected MonoBehaviour player;
        protected IEnumerator motionEnumerator;

        public MotionBase(float delay, float duration, EasingType motionType, bool motionTargetIsObjectInitialValue)
        {
            Delay = delay;
            Duration = duration;
            MotionType = motionType;
            MotionTargetIsObjectInitialValue = motionTargetIsObjectInitialValue;
        }

        public virtual void Play(bool immediate)
        {
            if (!isInitialized)
                throw new System.Exception("Motion was not initialized.");

            if (motionEnumerator != null)
                player.StopCoroutine(motionEnumerator);

            if (!immediate)
                isPlaying = true;
            else
                isPlaying = false;
        }

        public virtual void Stop()
        {
            if (!isInitialized)
                throw new System.Exception("Motion was not initialized.");

            if (motionEnumerator != null)
                player.StopCoroutine(motionEnumerator);

            isPlaying = false;
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }
    }
}
