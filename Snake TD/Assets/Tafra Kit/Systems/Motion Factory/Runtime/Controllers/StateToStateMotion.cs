using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    public class StateToStateMotion : SimpleMotionController
    {
        [Tooltip("Should the object's state be set to all the motions' start state (\"A\" state) once the motion starts to play regardless of the motions' delay?" +
            " (for example, if a motion scales the object from 1 to 2, and the object's current scale is 1.5, and a play of the motion was requested, " +
            "the object's scale will immediately jump to 1. This is irrelevant if all the delays are set to 0.)")]
        [SerializeField] private bool resetStateOnPlayStarting = true;
        [SerializeField] private TwoStatesMotionContainer motionsContainer;

        protected override async Task PlayMotion(bool inverted, bool instant, CancellationToken ct)
        {
            try
            {
                for(int i = 0; i < motionsContainer.motions.Count; i++)
                {
                    activeMotionTasks.Add(motionsContainer.motions[i].PlayAsync(inverted, useUnscaledTime, instant, ct));
                }

                await Task.WhenAll(activeMotionTasks);

                ct.ThrowIfCancellationRequested();
            }
            catch(OperationCanceledException)
            {

            }
        }

        protected override void OnPlayStarting()
        {
            if(resetStateOnPlayStarting)
            {
                for(int i = 0; i < motionsContainer.motions.Count; i++)
                {
                    motionsContainer.motions[i].Seek(0);
                }
            }
        }
    }
}