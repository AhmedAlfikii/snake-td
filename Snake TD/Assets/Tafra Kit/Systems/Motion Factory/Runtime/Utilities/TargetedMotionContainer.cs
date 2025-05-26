using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class TargetedMotionContainer : MotionContainer
    {
        [SerializeReference]
        public List<TargetedMotion> motions = new List<TargetedMotion>();

        public TargetedMotionContainer(List<TargetedMotion> motions)
        {
            this.motions = motions;
        }
        public TargetedMotionContainer()
        {
            this.motions = new List<TargetedMotion>();
        }

        public void SetController(UnityEngine.Object controller)
        {
            for(int i = 0; i < motions.Count; i++)
            {
                motions[i].SetController(controller);
            }
        }
        public void CaptureInitialState()
        {
            for(int i = 0; i < motions.Count; i++)
            {
                motions[i].CaptureInitialState();
            }
        }
    }
}