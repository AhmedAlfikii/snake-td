using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Quadratic")]
    public class QuadraticFormula : Formula
    {
        [Header("y = ax^2 + bx + c")]
        [SerializeField] private float a;
        [SerializeField] private float b;
        [SerializeField] private float c;

        public override float Evaluate(float x)
        {
            return a * (x * x) + b * x + c;
        }
    }
}