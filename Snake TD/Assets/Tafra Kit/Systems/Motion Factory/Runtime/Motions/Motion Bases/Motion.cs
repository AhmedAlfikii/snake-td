using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Tasks;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public abstract class Motion
    {
        [Tooltip("The duration in seconds before the animation start playing.")]
        [SerializeField] protected float delay;
        [Tooltip("The duration in seconds the animation takes in order to reach it's target state.")]
        [SerializeField] protected float duration;
        [Tooltip("The easing type of which the animation should follow.")]
        [SerializeField] protected EasingType easing;

        [NonSerialized] protected UnityEngine.Object controller;
        //Temp
        [NonSerialized] protected string controllerName;
        [NonSerialized] protected string controllerParentName;
        [NonSerialized] protected string controllerParentParentName;
        [NonSerialized] protected string controllerParentParentParentName;
        [NonSerialized] protected string controllerParentParentParentParentName;

        public abstract string MotionName { get; }
        protected abstract bool IsReferenceAvailable { get; }
       
        protected bool isPlaying;

        /// <summary>
        /// Fires when the motion is requested to play (before it waits the assigned delay).
        /// </summary>
        protected virtual void OnPlayStarting()
        {
            if(!IsReferenceAvailable)
            {
                TafraDebugger.Log($"Motion Factory - {MotionName}", $"No reference found, can't perform motion ({controllerName}) ({controllerParentName}) ({controllerParentParentName}) ({controllerParentParentParentName}) ({controllerParentParentParentParentName})", TafraDebugger.LogType.Error, controller);
                return;
            }
        }
        /// <summary>
        /// Fires when the motion starts to actually play (right before the first seek).
        /// </summary>
        protected virtual void OnPlayStart()
        { 

        }
        /// <summary>
        /// Fires when the motion completes playing whether it got canceled or completed normally.
        /// </summary>
        protected virtual void OnPlayCompleted()
        { 

        }

        public void SetController(UnityEngine.Object controller)
        {
            this.controller = controller;
            controllerName = controller.name;

            Transform parent = ((Component)controller).transform.parent;
            if(parent != null)
            {
                controllerParentName = parent.name;

                Transform parentParent = parent.parent;

                if(parentParent != null)
                {
                    controllerParentParentName = parentParent.name;

                    Transform parentParentParent = parentParent.parent;

                    if(parentParentParent != null)
                    {
                        controllerParentParentName = parentParentParent.name;

                        Transform parentParentParentParent = parentParentParent.parent;

                        if(parentParentParentParent != null)
                        {
                            controllerParentParentParentName = parentParentParentParent.name;
                        }
                    }
                }
            }
        }

        public virtual async Task PlayAsync(bool inverted = false, bool useUnscaledTime = false, bool instant = false, CancellationToken ct = default)
        {
            isPlaying = true;

            try
            {
                if(instant)
                {
                    Seek(1, inverted);
                    return;
                }

                //Just so that when the game starts playing it the unscaled time doesn't jump and the motion would instantly seek forward.
                while(Time.frameCount < 1)
                {
                    await Task.Yield();
                    ct.ThrowIfCancellationRequested();
                }

                ct.ThrowIfCancellationRequested();

                OnPlayStarting();

                if(delay > 0.0001f)
                {
                    if (!useUnscaledTime)
                        await TafraTasker.Wait(delay, ct);
                    else
                        await TafraTasker.WaitUnscaled(delay, ct);
                }

                ct.ThrowIfCancellationRequested();

                OnPlayStart();

                float time = useUnscaledTime ? Time.unscaledTime : Time.time;

                float startTime = time;
                float endTime = startTime + duration;

                //***********************************************************************************************
                //The below if conditoin was commented since now the motion controllers are responsible of making sure
                //that at least 1 frame has passe since playing the animation before looping it.
                //But keeping it for future reference in case the issue arised again.
                //***********************************************************************************************
                //^
                //if(time >= endTime && delay <= 0.0001f)  //To avoid infinite looping if the motion was set to loop and the duration and delay are 0.
                //    await Task.Yield();

                while(time < endTime)
                {
                    float t = (time - startTime) / duration;

                    Seek(t, inverted);

                    ct.ThrowIfCancellationRequested();

                    await Task.Yield();

                    time = useUnscaledTime ? Time.unscaledTime : Time.time;

                    ct.ThrowIfCancellationRequested();
                }

                ct.ThrowIfCancellationRequested();

                Seek(1, inverted);
                OnPlayCompleted();
            }
            catch(OperationCanceledException)
            {
                OnPlayCompleted();
            }
            finally
            {
                isPlaying = false;
            }
        }

        public void Seek(float t, bool inverted = false)
        {
            t = MotionEquations.GetEaseFloat(t, easing);

            if(inverted)
                t = 1 - t;

            SeekEased(t);
        }
        protected abstract void SeekEased(float t);
    }
}
