using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public abstract class TargetedMotion : Motion
    {
        [Tooltip("Should this motion target the object's initial state? (e.g. the object's scale on game start)")]
        [SerializeField] protected bool targetInitialState;

        protected bool capturedInitialState;

        public override Task PlayAsync(bool inverted = false, bool useUnscaledTime = false, bool instant = false, CancellationToken ct = default)
        {
            if(targetInitialState && !capturedInitialState)
            {
                TafraDebugger.Log($"Motion Factory - {MotionName}",
                    "The motion was set to target initial state, but initial state wasn't captured. Make sure to call CaptureInitialState() on the motion.",
                    TafraDebugger.LogType.Error);
            }

            return base.PlayAsync(inverted, useUnscaledTime, instant, ct);
        }

        public abstract void CaptureInitialState();
        public abstract void RevertToInitialState();
        public abstract Task RevertToInitialState(float duration = 0, EasingType easing = null, bool useUnscaledTime = false, CancellationToken ct = default);
    }
}
