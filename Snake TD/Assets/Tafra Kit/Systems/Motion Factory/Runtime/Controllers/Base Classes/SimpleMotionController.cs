using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using TafraKit.Tasks;

namespace TafraKit.MotionFactory
{
    public abstract class SimpleMotionController : MotionController
    {
        [Tooltip("Should the motion start playing automatically once the object is enabled?")]
        [SerializeField] protected bool playOnEnable = true;
        [Tooltip("Should the motion animate independently of the time scale?")]
        [SerializeField] protected bool useUnscaledTime;
        [Tooltip("The duration in seconds the motion should wait before start playing.")]
        [SerializeField] protected float playDelay;
        [Tooltip("Determine what happens once the motion ends.")]
        [SerializeField] protected MotionWrapMode wrapMode;
       
        protected bool isPlaying;

        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
        }

        protected virtual void OnEnable()
        {
            if(!isPlaying && playOnEnable)
                Play();
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            isPlaying = false;
        }

        protected abstract Task PlayMotion(bool inverted, bool instant, CancellationToken ct);

        public async void Play()
        {
            if(isPlaying)
                motionCTS.Cancel();

            DisposeCompletedTasks();
            DisposeMotionCTS();

            isPlaying = true;
            
            OnPlayStarting();

            motionCTS = new CancellationTokenSource();
            CancellationToken ct = motionCTS.Token;

            try
            {
                if(playDelay > 0.0001f)
                {
                    Task delayTask = null;

                    if(!useUnscaledTime)
                        delayTask = TafraTasker.Wait(playDelay, ct);
                    else
                        delayTask = TafraTasker.WaitUnscaled(playDelay, ct);

                    activeMotionTasks.Add(delayTask);

                    await delayTask;
                }

                ct.ThrowIfCancellationRequested();

                OnPlayStart();

                bool playMotion = true;
                bool playInverted = false;

                while(playMotion)
                {
                    int startFrame = Time.frameCount;

                    await PlayMotion(playInverted, false, ct);

                    ct.ThrowIfCancellationRequested();

                    int animationFrames = Time.frameCount - startFrame;

                    //To avoid infinite looping if the motions were set to loop and the duration and delay are 0 (or no motions at all were played).
                    if(animationFrames == 0)
                    {
                        break;
                    }

                    switch(wrapMode)
                    {
                        case MotionWrapMode.Once:
                            playMotion = false;
                            break;
                        case MotionWrapMode.Loop:
                            playMotion = true;
                            playInverted = false;
                            break;
                        case MotionWrapMode.PingPong:
                            playMotion = true;
                            playInverted = !playInverted;
                            break;
                    }
                }

                isPlaying = false;
            }
            catch(OperationCanceledException)
            {

            }
        }

        public void Stop()
        {
            if(motionCTS != null)
                motionCTS.Cancel();

            isPlaying = false;
        }

        /// <summary>
        /// Fires when the motion is requested to play (before it waits the assigned delay).
        /// </summary>
        protected virtual void OnPlayStarting()
        {

        }
        /// <summary>
        /// Fires when the motion starts to actually play (right before the first seek).
        /// </summary>
        protected virtual void OnPlayStart()
        {

        }
    }
}