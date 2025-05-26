using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    public class ChanceCondition : Condition
    {
        [Range(0, 1)]
        [SerializeField] private float value;

        protected override bool PerformCheck()
        {
            if(value > 0.9999f)
                return true;

            return Random.value < value;
        }
    }
}