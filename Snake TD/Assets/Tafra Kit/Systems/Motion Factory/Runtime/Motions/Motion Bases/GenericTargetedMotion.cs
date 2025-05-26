using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public abstract class GenericTargetedMotion<T> : TargetedMotion
    {
        protected abstract T ReferenceValue { get; }
        protected abstract T TargetValue { get; }

        protected T initialState;
        protected T animationStartState;
        protected abstract void SeekTarget(float t, T target);

        protected override void OnPlayStart()
        {
            base.OnPlayStart();

            if(!IsReferenceAvailable) 
                return;

            animationStartState = ReferenceValue;
        }

        protected override void SeekEased(float t)
        {
            if(!IsReferenceAvailable)
                return;

            T target = targetInitialState ? initialState : TargetValue;

            SeekTarget(t, target);
        }

        public override void CaptureInitialState()
        {
            if(!IsReferenceAvailable)
            {
                TafraDebugger.Log($"Motion Factory - {MotionName}", "No reference found, can't capture initial state.", TafraDebugger.LogType.Error);
                return;
            }

            capturedInitialState = true;

            initialState = ReferenceValue;
        }

        public override void RevertToInitialState()
        {
            SeekTarget(1, initialState);
        }
        public override async Task RevertToInitialState(float duration = 0, EasingType easing = null, bool useUnscaledTime = false, CancellationToken ct = default)
        {
            if (duration < 0.001f)
                SeekTarget(1, initialState);
            else
            {
                animationStartState = ReferenceValue;

                Task revertingTask = Reverting(duration, easing, useUnscaledTime, ct);

                await revertingTask;

                revertingTask.Dispose();
            }
        }

        protected async Task Reverting(float duration, EasingType easing, bool useUnscaledTime, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                float time = useUnscaledTime ? Time.unscaledTime : Time.time;

                float startTime = time;
                float endTime = startTime + duration;

                while (time < endTime)
                {
                    float t = (time - startTime) / duration;

                    SeekTarget(t, initialState);

                    ct.ThrowIfCancellationRequested();

                    await Task.Yield();

                    time = useUnscaledTime ? Time.unscaledTime : Time.time;

                    ct.ThrowIfCancellationRequested();
                }

                ct.ThrowIfCancellationRequested();

                SeekTarget(1, initialState);
            }
            catch (OperationCanceledException)
            { 

            }
        }
    }
}
