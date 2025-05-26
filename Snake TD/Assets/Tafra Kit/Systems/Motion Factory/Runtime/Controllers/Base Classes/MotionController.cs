using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace TafraKit.MotionFactory
{
    public abstract class MotionController : MonoBehaviour
    {
        protected CancellationTokenSource motionCTS;
        /// <summary>
        /// Tasks that are currently controling the motion of the object.
        /// </summary>
        protected List<Task> activeMotionTasks = new List<Task>();

        protected virtual void OnEnable()
        {

        }
        protected virtual void OnDisable()
        {
            if(motionCTS != null)
                motionCTS.Cancel();
        }
        protected virtual void OnDestroy()
        {
            if(motionCTS != null)
                motionCTS.Cancel();

            DisposeCompletedTasks();
            DisposeMotionCTS();
        }

        protected void DisposeCompletedTasks()
        {
            if(activeMotionTasks.Count > 0)
            {
                for(int i = 0; i < activeMotionTasks.Count; i++)
                {
                    if(activeMotionTasks[i].IsCompleted)
                    {
                        activeMotionTasks[i].Dispose();
                        activeMotionTasks.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        protected void DisposeMotionCTS()
        {
            if(motionCTS != null)
            {
                motionCTS.Dispose();
                motionCTS = null;
            }
        }
    }
}