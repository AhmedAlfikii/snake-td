using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Tasks
{
    public static class TafraTasker
    {
        private static OperationCanceledException operationCanceledException = new OperationCanceledException();

        public static OperationCanceledException OperationCanceledException => operationCanceledException;

        /// <summary>
        /// Fires a task that waits until the sent duration has passed using game scaled time.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task Wait(float time, CancellationToken ct)
        {
            float startTime = Time.time;
            float endTime = startTime + time;

            while(Time.time < endTime)
            {
                await Task.Yield();

                if(ct.IsCancellationRequested)
                    break;
            }
        }
        /// <summary>
        /// Fires a task that waits until the sent duration has passed using game's unscaled time.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task WaitUnscaled(float time, CancellationToken ct)
        {
            float startTime = Time.unscaledTime;
            float endTime = startTime + time;

            while(Time.unscaledTime < endTime)
            {
                await Task.Yield();

                if(ct.IsCancellationRequested)
                    break;
            }
        }
    }
}