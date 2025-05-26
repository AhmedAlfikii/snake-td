using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Fixed Value")]
    public class FixedValueFormula : Formula
    {
        [Header("y = value")]
        [SerializeField] private float value;

        public override float Evaluate(float x)
        {
            return value;
        }
    }
}