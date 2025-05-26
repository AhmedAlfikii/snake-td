using System;
using UnityEngine;

namespace TafraKit.Queueables
{
    public interface ITafraQueueable
    {
        /// <summary>
        /// Lower Value = higher priority.
        /// </summary>
        public int Priority { get; }
        public Action OnQueueableConcludedPerformance { get; set; }

        public void QueueablePerform();
    }
}