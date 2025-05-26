using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Linear Incremental")]
    public class LinearIncrementalFormula : Formula
    {
        [SerializeField] private float pivotX;
        [SerializeField] private float startValue;
        [SerializeField] private float increments;
        [SerializeField] private bool isLimited;
        [SerializeField] private float limit;

        public override float Evaluate(float x)
        {
            float val = startValue + (x * increments) - (pivotX * increments);

            if(isLimited && ((increments >= 0 && val > limit) || (increments < 0 && val < limit)))
                val = limit;

            return val;
        }
    }
}