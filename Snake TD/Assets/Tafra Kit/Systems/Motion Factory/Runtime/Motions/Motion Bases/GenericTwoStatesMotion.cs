using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public abstract class GenericTwoStatesMotion<T> : TwoStatesMotion
    {
        [SerializeField] protected T stateA;
        [SerializeField] protected T stateB;
    }
}