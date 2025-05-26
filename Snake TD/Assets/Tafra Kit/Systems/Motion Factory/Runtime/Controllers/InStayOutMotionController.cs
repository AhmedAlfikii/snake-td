using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    public class InStayOutMotionController : InOutMotionController
    {
        [SerializeField] protected float revertDuration = 0.25f;
        [SerializeField] protected EasingType revertEasing;
        [SerializeField] protected TargetedMotionFrames stayMotions;

        protected List<TargetedMotion> stayMotionsNotFoundInOut = new List<TargetedMotion>();
        protected List<Task> activeStayMotionTasks = new List<Task>();
        protected List<Task> activeRevertingMotionTasks = new List<Task>();
        protected CancellationTokenSource stayMotionCTS;
        protected CancellationTokenSource revertMotionCTS;
        protected bool isStayMotionPlaying;
        protected bool isRevertMotionPlaying;

        protected override void OnEnable()
        {
            base.OnEnable();

            //To play the stay motion.
            if(state == VisibilityControllerState.Shown)
                OnShowCompleted();
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            //Reset stay motions that are not in the out motions.
            if (isStayMotionPlaying || isRevertMotionPlaying)
            {
                for (int i = 0; i < stayMotionsNotFoundInOut.Count; i++)
                {
                    stayMotionsNotFoundInOut[i].RevertToInitialState();
                }
            }

            if (stayMotionCTS != null)
                stayMotionCTS.Cancel();
            if(revertMotionCTS != null)
                revertMotionCTS.Cancel();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(stayMotionCTS != null)
            {
                stayMotionCTS.Cancel();
                stayMotionCTS.Dispose();
            }
            if(revertMotionCTS != null)
            {
                revertMotionCTS.Cancel();
                revertMotionCTS.Dispose();
            }

            DisposeStayMotionCompletedTasks();
            DisposeRevertMotionCompletedTasks();
        }

        protected override void OnInitializeMotions()
        {
            base.OnInitializeMotions();

            stayMotions.CaptureInitialState();

            for(int i = 0; i < stayMotions.frames.Count; i++)
            {
                for(int j = 0; j < stayMotions.frames[i].motions.Count; j++)
                {
                    TargetedMotion stayMotion = stayMotions.frames[i].motions[j];

                    stayMotion.SetController(this);

                    int motionTypeHash = Animator.StringToHash(stayMotion.MotionName);

                    if(!outMotionNameHashes.Contains(motionTypeHash))
                        stayMotionsNotFoundInOut.Add(stayMotion);
                }
            }
        }

        protected async override void OnShowCompleted()
        {
            base.OnShowCompleted();

            if(!gameObject.activeInHierarchy)
                return;

            if(isStayMotionPlaying)
                stayMotionCTS.Cancel();

            stayMotionCTS?.Dispose();

            DisposeStayMotionCompletedTasks();

            if(stayMotions.frames.Count > 0)
            {
                stayMotionCTS = new CancellationTokenSource();

                Task stayMotionTask = PlayingStayMotion(stayMotionCTS.Token);

                await stayMotionTask;

                stayMotionTask.Dispose();
            }
        }
        protected async override void OnHideStarted()
        {
            base.OnHideStarted();

            //Revert stay motions that are not in the out motions.
            if (isStayMotionPlaying)
            {
                stayMotionCTS?.Cancel();

                revertMotionCTS?.Dispose();

                revertMotionCTS = new CancellationTokenSource();

                isRevertMotionPlaying = true;
                for (int i = 0; i < stayMotionsNotFoundInOut.Count; i++)
                {
                    Task motionTask = stayMotionsNotFoundInOut[i].RevertToInitialState(0.25f, revertEasing, useUnscaledTime, revertMotionCTS.Token);
                    activeRevertingMotionTasks.Add(motionTask);
                }

                await Task.WhenAll(activeRevertingMotionTasks);

                isRevertMotionPlaying = false;
            }
        }
        protected override void OnHideCompleted()
        {
            base.OnHideCompleted();
            
            if (isRevertMotionPlaying)
            {
                // Cancel all revert motions and instantly revert.
                revertMotionCTS.Cancel();

                for (int i = 0; i < stayMotionsNotFoundInOut.Count; i++)
                {
                    stayMotionsNotFoundInOut[i].RevertToInitialState();
                }
            }

            DisposeRevertMotionCompletedTasks();
        }
        private async Task PlayingStayMotion(CancellationToken ct)
        {
            try
            {
                isStayMotionPlaying = true;
                additionalPlayingMotions.AddController("stayMotoin");

                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    for(int i = 0; i < stayMotions.frames.Count; i++)
                    {
                        ct.ThrowIfCancellationRequested();

                        for(int j = 0; j < stayMotions.frames[i].motions.Count; j++)
                        {
                            ct.ThrowIfCancellationRequested();

                            Task motion = stayMotions.frames[i].motions[j].PlayAsync(false, useUnscaledTime, false, ct);
                            activeStayMotionTasks.Add(motion);
                        }

                        await Task.WhenAll(activeStayMotionTasks);

                        ct.ThrowIfCancellationRequested();
                    }
                }
            }
            catch(OperationCanceledException)
            { 
                additionalPlayingMotions.RemoveController("stayMotoin");
                isStayMotionPlaying = false;
            }
        }

        private void DisposeStayMotionCompletedTasks()
        {
            if(activeStayMotionTasks.Count > 0)
            {
                for(int i = 0; i < activeStayMotionTasks.Count; i++)
                {
                    if(activeStayMotionTasks[i].IsCompleted)
                    {
                        activeStayMotionTasks[i].Dispose();
                        activeStayMotionTasks.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        private void DisposeRevertMotionCompletedTasks()
        {
            if(activeRevertingMotionTasks.Count > 0)
            {
                for(int i = 0; i < activeRevertingMotionTasks.Count; i++)
                {
                    if(activeRevertingMotionTasks[i].IsCompleted)
                    {
                        activeRevertingMotionTasks[i].Dispose();
                        activeRevertingMotionTasks.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}