using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public abstract class TwoStatesMotion : Motion
    {
        protected abstract void SeekState(float t);

        protected override void SeekEased(float t)
        {
            if(!IsReferenceAvailable)
                return;

            SeekState(t);
        }
    }
}