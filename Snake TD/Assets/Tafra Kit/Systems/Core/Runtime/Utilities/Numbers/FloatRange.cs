using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TafraKit
{
    [Serializable]
    public class FloatRange
    {
        [Tooltip("The minimum value of the range (inclusive).")]
        public float Min;
        [Tooltip("The maximum value of the range (inclusive).")]
        public float Max;

        public FloatRange()
        { 
        
        }
        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Returns a random value within the float's range.
        /// </summary>
        /// <returns></returns>
        public float GetRandomValue()
        {
            return Random.Range(Min, Max);
        }
        public float GetRandomValueAndSign()
        {
            float val = Random.Range(Min, Max);

            val *= Random.value > 0.5f ? 1 : -1;

            return val;
        }

        /// <summary>
        /// Returns whether or not the number provided is within the min/max values of this range (or equals to either of them).
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool IsInRange(float num)
        {
            return (num >= Min && num <= Max);
        }

        /// <summary>
        /// Returns the actual value of the range at the given percentage (0 to 1).
        /// </summary>
        /// <param name="percent">The value between 0 and 1 to evaluate the range at.</param>
        /// <returns></returns>
        public float Evaluate(float percent)
        {
            return Min + (Max - Min) * percent;
        }

        /// <summary>
        /// Returns the normalized value (0 to 1) of the range based on the actual value sent.
        /// </summary>
        /// <param name="actualValue"></param>
        /// <returns></returns>
        public float GetNormalizedValue(float actualValue)
        {
            return Mathf.Clamp01((actualValue - Min) / (Max - Min));
        }
    }
}