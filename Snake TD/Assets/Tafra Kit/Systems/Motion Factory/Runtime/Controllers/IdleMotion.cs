using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    public class IdleMotion : SimpleMotionController
    {
        [SerializeField] private TargetedMotionFrames motionFrames;

        protected void Awake()
        {
            motionFrames.CaptureInitialState();
        }

        protected override async Task PlayMotion(bool inverted, bool instant, CancellationToken ct)
        {
            try
            {
                for(int frameIndex = 0; frameIndex < motionFrames.frames.Count; frameIndex++)
                {
                    for(int motionIndex = 0; motionIndex < motionFrames.frames[frameIndex].motions.Count; motionIndex++)
                    {
                        Motion motion = motionFrames.frames[frameIndex].motions[motionIndex];
                        activeMotionTasks.Add(motion.PlayAsync(false, useUnscaledTime, instant, ct));
                    }

                    await Task.WhenAll(activeMotionTasks);

                    ct.ThrowIfCancellationRequested();
                }

                ct.ThrowIfCancellationRequested();
            }
            catch(OperationCanceledException)
            {

            }
        }
    }
}