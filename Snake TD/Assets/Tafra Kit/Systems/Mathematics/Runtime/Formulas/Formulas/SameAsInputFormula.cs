using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.Mathematics
{
    [SearchMenuItem("Same As Input")]
    public class SameAsInputFormula : Formula
    {
        public override float Evaluate(float x)
        {
            return x;
        }
    }
}