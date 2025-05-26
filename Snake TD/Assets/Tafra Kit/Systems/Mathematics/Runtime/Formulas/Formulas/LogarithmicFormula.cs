using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Logarithmic")]
    public class LogarithmicFormula : Formula
    {
        [Header("y = LOGb(x) * a")]
        [SerializeField] private float a;
        [SerializeField] private float b;

        public override float Evaluate(float x)
        {
            return Mathf.Log(x, b) * a;
        }
    }
}