using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class IntRange
    {
        [Tooltip("The minimum value of the range (inclusive).")]
        public int Min;
        [Tooltip("The maximum value of the range (inclusive).")]
        public int Max;

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Returns a random value within the int's range including the min and max values.
        /// </summary>
        /// <returns></returns>
        public int GetRandomValue()
        {
            return UnityEngine.Random.Range(Min, Max + 1);
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
        /// Returns whether or not the number provided is within the min/max values of this range (or equals to either of them).
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool IsInRange(int num)
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
            return Mathf.Clamp01((actualValue - Min) / (float)(Max - Min));
        }

        public int Length()
        {
            return Max - Min;
        }
    }
}