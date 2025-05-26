using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class VisibilityMotionContainer : MotionContainer
    {
        [SerializeReference]
        public List<VisibilityMotion> motions = new List<VisibilityMotion>();

        public void CaptureInitialState()
        {
            for(int i = 0; i < motions.Count; i++)
            {
                motions[i].CaptureInitialState();
            }
        }
    }
}