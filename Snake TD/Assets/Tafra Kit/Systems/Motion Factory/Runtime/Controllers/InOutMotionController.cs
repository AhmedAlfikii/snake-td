using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace TafraKit.MotionFactory
{
    public class InOutMotionController : VisibilityMotionController
    {
        [SerializeField] protected VisibilityMotionContainer inMotions;
        [SerializeField] protected VisibilityMotionContainer outMotions;

        /// <summary>
        /// Motions of types that are only present in the out motions and not the in motions.
        /// </summary>
        protected List<VisibilityMotion> uniqueOutMotions = new List<VisibilityMotion>();

        protected HashSet<int> inMotionNameHashes = new HashSet<int>();
        protected HashSet<int> outMotionNameHashes = new HashSet<int>();

        protected override void OnInitializeMotions()
        {
            for(int i = 0; i < inMotions.motions.Count; i++)
            {
                inMotionNameHashes.Add(Animator.StringToHash(inMotions.motions[i].MotionName));
                inMotions.motions[i].SetVisibilityDirection(true);
            }

            for(int i = 0; i < outMotions.motions.Count; i++)
            {
                VisibilityMotion outMotion = outMotions.motions[i];

                int motionTypeHash = Animator.StringToHash(outMotions.motions[i].MotionName);

                outMotionNameHashes.Add(motionTypeHash);

                outMotion.SetVisibilityDirection(false);

                if(!inMotionNameHashes.Contains(motionTypeHash))
                    uniqueOutMotions.Add(outMotion);
            }

            inMotions.CaptureInitialState();
            outMotions.CaptureInitialState();
        }

        protected override async Task OnShowRequest(bool instant, bool isMidAnimation, CancellationToken ct)
        {
            DisposeCompletedTasks();

            try
            {
                //Reset the object to the initial state of the unique motions, otherwise it will remain at the state those motions changed the object to, which is not intended. 
                for(int i = 0; i < uniqueOutMotions.Count; i++)
                {
                    uniqueOutMotions[i].Seek(0);
                }

                for(int i = 0; i < inMotions.motions.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    inMotions.motions[i].SetInterruptedMotionFlag(isMidAnimation);

                    Task motionTask = inMotions.motions[i].PlayAsync(false, useUnscaledTime, instant, ct);
                    activeMotionTasks.Add(motionTask);
                }

                await Task.WhenAll(activeMotionTasks);
            }
            catch(OperationCanceledException)
            {

            }
        }

        protected override async Task OnHideRequest(bool instant, bool isMidAnimation, CancellationToken ct)
        {
            DisposeCompletedTasks();

            try
            {
                for(int i = 0; i < outMotions.motions.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    outMotions.motions[i].SetInterruptedMotionFlag(isMidAnimation);

                    Task motionTask = outMotions.motions[i].PlayAsync(false, useUnscaledTime, instant, ct);

                    activeMotionTasks.Add(motionTask);
                }

                await Task.WhenAll(activeMotionTasks);
            }
            catch(OperationCanceledException)
            {

            }
        }
    }
}