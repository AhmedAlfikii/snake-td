using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Linear")]
    public class LinearFormula : Formula
    {
        [Header("y = a + x.b")]
        [SerializeField] private float a;
        [SerializeField] private float b;

        public override float Evaluate(float x)
        {
            return a + x * b;
        }
    }
}