using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    public abstract class GenericVisibilityMotion<T> : VisibilityMotion
    {
        [SerializeField] protected T hiddenState;

        protected T shownState;
        protected T animationStartState;

        protected T StateA
        {
            get
            {
                if (interruptedMotion)
                    return animationStartState;
                else
                {
                    if (show)
                        return hiddenState;
                    else
                        return shownState;

                }
            }
        }

        protected T StateB => show ? shownState :hiddenState;
        protected abstract T ReferenceValue { get; }

        public override void CaptureInitialState()
        {
            if(!IsReferenceAvailable)
            {
                TafraDebugger.Log($"Motion Factory - {MotionName}", "No reference found, can't capture initial state.", TafraDebugger.LogType.Error);
                return;
            }

            capturedInitialState = true;

            shownState = ReferenceValue;
        }

        protected override void OnPlayStart()
        {
            base.OnPlayStart();

            animationStartState = ReferenceValue;
        }
    }
}