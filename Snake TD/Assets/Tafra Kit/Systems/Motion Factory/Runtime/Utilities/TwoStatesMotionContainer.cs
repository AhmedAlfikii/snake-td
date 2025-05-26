using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class TwoStatesMotionContainer : MotionContainer
    {
        [SerializeReference]
        public List<TwoStatesMotion> motions = new List<TwoStatesMotion>();

        public TwoStatesMotionContainer(List<TwoStatesMotion> motions)
        {
            this.motions = motions;
        }
        public TwoStatesMotionContainer()
        {
            this.motions = new List<TwoStatesMotion>();
        }
    }
}