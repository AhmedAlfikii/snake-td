using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit.Queueables
{
    public static class TafraQueues
    {
        #region Classes, Structs & Enums
        private class TafraQueue
        {
            private List<ITafraQueueable> queueables;
            private ITafraQueueable performingQueueable;

            public TafraQueue()
            { 
                queueables = new List<ITafraQueueable>();
            }

            public void Add(ITafraQueueable queueable)
            {
                queueables.Add(queueable);

                queueables.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            }

            public void AttemptPerformance()
            {
                if(performingQueueable != null || queueables.Count == 0)
                    return;

                performingQueueable = queueables[0];
                queueables.RemoveAt(0);

                performingQueueable.OnQueueableConcludedPerformance = OnPerformingQueueableConcluded;

                performingQueueable.QueueablePerform();
            }

            private void OnPerformingQueueableConcluded()
            {
                performingQueueable.OnQueueableConcludedPerformance = null;

                performingQueueable = null;

                AttemptPerformance();
            }
        }
        #endregion

        private static Dictionary<string, TafraQueue> queues = new Dictionary<string, TafraQueue>();
        private static Dictionary<string, IEnumerator> latePerformanceEnumByQueue = new Dictionary<string, IEnumerator>();

        private static IEnumerator LatePerformQueue(TafraQueue queue, string queueID)
        {
            if(Time.frameCount == 0)
                yield return null;

            yield return Yielders.EndOfFrame;

            Debug.Log($"Will attempt to perform queue {queueID}");

            queue.AttemptPerformance();

            latePerformanceEnumByQueue[queueID] = null;
        }

        public static void Enqueue(ITafraQueueable queueable, string queueID = "default_queue")
        {
            Debug.Log($"Adding Queueable: {queueable} to {queueID}");

            TafraQueue queue;

            if (!queues.TryGetValue(queueID, out queue))
            {
                Debug.Log($"Queue ({queueID}) doesn't exist. Creating it.");

                queue = new TafraQueue();
                queues.Add(queueID, queue);
                latePerformanceEnumByQueue.Add(queueID, null);
            }

            queue.Add(queueable);

            //Only start the enumerator if it wasn't already started this frame.
            if(latePerformanceEnumByQueue[queueID] == null)
            {
                Debug.Log($"Nothing was added to the queue ({queueID}) this frame, will start late performance.");

                IEnumerator e = LatePerformQueue(queue, queueID);

                latePerformanceEnumByQueue[queueID] = e;

                GeneralCoroutinePlayer.StartCoroutine(e);
            }
            else
            {
                Debug.Log($"Something was already added to the queue ({queueID}) this frame, no need to start late performance.");
            }
        }
    }
}