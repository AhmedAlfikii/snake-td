using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [System.Serializable]
    public abstract class Formula
    {
        [SerializeField] private FloatRange inputRange;

        public FloatRange InputRange => inputRange;

        public void SetInputRange(float min, float max)
        {
            inputRange = new FloatRange(min, max);
        }
        
        public abstract float Evaluate(float x);
    }
}