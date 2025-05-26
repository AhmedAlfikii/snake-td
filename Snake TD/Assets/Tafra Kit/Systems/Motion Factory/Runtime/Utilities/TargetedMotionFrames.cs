using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class TargetedMotionFrames
    {
        public List<TargetedMotionContainer> frames = new List<TargetedMotionContainer>();

        public void SetController(UnityEngine.Object controller)
        {
            for(int i = 0; i < frames.Count; i++)
            {
                frames[i].SetController(controller);
            }
        }
        public void CaptureInitialState()
        {
            for(int i = 0; i < frames.Count; i++)
            {
                frames[i].CaptureInitialState();
            }
        }
    }
}