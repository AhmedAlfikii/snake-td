using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Exponential")]
    public class ExponentialFormula : Formula
    {
        [Header("y = ab^x + c")]
        [SerializeField] private float a;
        [SerializeField] private float b;
        [SerializeField] private float c;

        public override float Evaluate(float x)
        {
            return a * Mathf.Pow(b, x) + c;
        }
    }
}