using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal;
using UnityEngine;

namespace TafraKit
{
    public static class ActionBuffer
    {
        private static Dictionary<int, IEnumerator> buffers = new Dictionary<int, IEnumerator>();

        /// <summary>
        /// Keep attempting to perform an action until it returns true. If a buffer already exists, it will be overriden and restarted.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="duration">The duration of which the buffer will keep attempting to perform the action for (every frame).</param>
        public static void Buffer(int actionHash, Func<bool> action, float duration)
        {
            if(buffers.TryGetValue(actionHash, out IEnumerator existingBuffer))
            {
                GeneralCoroutinePlayer.StopCoroutine(existingBuffer);
                buffers.Remove(actionHash);
            }

            IEnumerator buffer = Buffering(actionHash, action, duration);

            GeneralCoroutinePlayer.StartCoroutine(buffer);

            buffers.Add(actionHash, buffer);
        }
        public static void RemoveBuffer(int actionHash)
        {
            if (buffers.TryGetValue(actionHash, out IEnumerator buffer))
            {
                GeneralCoroutinePlayer.StopCoroutine(buffer);
                buffers.Remove(actionHash);
            }
        }

        private static IEnumerator Buffering(int actionHash, Func<bool> action, float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                if(action.Invoke() == true)
                {
                    RemoveBuffer(actionHash);
                    yield break;
                }
                yield return null;
            }
        }
    }
}