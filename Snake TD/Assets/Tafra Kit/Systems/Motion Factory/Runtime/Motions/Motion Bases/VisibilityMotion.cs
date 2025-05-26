using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public abstract class VisibilityMotion : Motion
    {
        protected bool capturedInitialState;
        protected bool show = true;
        protected bool interruptedMotion;
        public override Task PlayAsync(bool inverted = false, bool useUnscaledTime = false, bool instant = false, CancellationToken ct = default)
        {
            if(!capturedInitialState)
            {
                TafraDebugger.Log($"Motion Factory - {MotionName}",
                    "Motion initial state wasn't recorded. Make sure to call CaptureInitialState() on the motion.",
                    TafraDebugger.LogType.Error);
            }

            return base.PlayAsync(inverted, useUnscaledTime, instant, ct);
        }

        [Tooltip("Set whether this motion should show or hide the object.")]
        public void SetVisibilityDirection(bool show)
        {
            this.show = show;
        }
        public void SetInterruptedMotionFlag(bool interrupted)
        { 
            interruptedMotion = interrupted;
        }
        public abstract void CaptureInitialState();

        protected override void OnPlayCompleted()
        {
            base.OnPlayCompleted();

            interruptedMotion = false;
        }
    }
}