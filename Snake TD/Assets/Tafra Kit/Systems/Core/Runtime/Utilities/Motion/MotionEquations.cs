using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public enum MotionType { Custom, Linear, EaseIn, EaseOut, EaseInOut, EaseInBack, EaseOutBack, EaseInOutBack, EaseInElastic, EaseOutElastic, EaseInOutElastic, EaseInBounce, EaseOutBounce, EaseInOutBounce }
    public static class MotionEquations
    {
        #region Motion Functions
        public static float Custom(float t, AnimationCurve curve)
        {
            return curve.Evaluate(t);
        }

        public static float Linear(float t)
        {
            return t;
        }

        public static float EaseIn(float t, int power = 2)
        {
            return Mathf.Pow(t, power);
        }
        public static float EaseOut(float t, int power = 2)
        {
            return 1 - Mathf.Abs(Mathf.Pow(t - 1, power));
        }
        public static float EaseInOut(float t, int power = 2)
        {
            return t < 0.5f ? EaseIn(t * 2, power) / 2 : EaseOut(t * 2 - 1, power) / 2 + 0.5f;
        }

        public static float EaseInBack(float t, float magnitude = 1.7f)
        {
            return t * t * ((magnitude + 1) * t - magnitude);
        }
        public static float EaseOutBack(float t, float magnitude = 1.7f)
        {
            float scaledTime = (t / 1) - 1;

            return (scaledTime * scaledTime * ((magnitude + 1) * scaledTime + magnitude)) + 1;
        }
        public static float EaseInOutBack(float t, float magnitude = 1.7f)
        {
            float scaledTime = t * 2;
            float scaledTime2 = scaledTime - 2;

            float s = magnitude * 1.525f;

            if (scaledTime < 1)
                return 0.5f * scaledTime * scaledTime * (((s + 1) * scaledTime) - s);

            return 0.5f * (scaledTime2 * scaledTime2 * ((s + 1) * scaledTime2 + s) + 2);
        }

        public static float EaseInElastic(float t, float magnitude = 0.7f)
        {
            if (t == 0 || t == 1)
            {
                return t;
            }

            float scaledTime = t / 1;
            float scaledTime1 = scaledTime - 1;

            float p = 1 - magnitude;
            float s = p / (2 * Mathf.PI) * Mathf.Asin(1);

            return -(
                Mathf.Pow(2, 10 * scaledTime1) *
                Mathf.Sin((scaledTime1 - s) * (2 * Mathf.PI) / p)
            );
        }
        public static float EaseOutElastic(float t, float magnitude = 0.7f)
        {
            float p = 1 - magnitude;
            float scaledTime = t * 1f;

            if (t == 0 || t == 1)
            {
                return t;
            }

            float s = p / (2 * Mathf.PI) * Mathf.Asin(1);
            return (
                Mathf.Pow(2, -10 * scaledTime) *
                Mathf.Sin((scaledTime - s) * (2 * Mathf.PI) / p)
            ) + 1;
        }
        public static float EaseInOutElastic(float t, float magnitude = 0.7f)
        {
            float p = 1 - magnitude;

            if (t == 0 || t == 1)
            {
                return t;
            }

            float scaledTime = t * 2;
            float scaledTime1 = scaledTime - 1;

            float s = p / (2 * Mathf.PI) * Mathf.Asin(1);

            if (scaledTime < 1)
            {
                return -0.5f * (
                    Mathf.Pow(2, 10 * scaledTime1) *
                    Mathf.Sin((scaledTime1 - s) * (2 * Mathf.PI) / p)
                );
            }

            return (
                Mathf.Pow(2, -10 * scaledTime1) *
                Mathf.Sin((scaledTime1 - s) * (2 * Mathf.PI) / p) * 0.5f
            ) + 1;
        }

        public static float EaseInBounce(float t)
        {
            return 1 - EaseOutBounce(1 - t);
        }
        public static float EaseOutBounce(float t)
        {
            float scaledTime = t / 1;

            if (scaledTime < (1 / 2.75f))
            {

                return 7.5625f * scaledTime * scaledTime;

            }
            else if (scaledTime < (2 / 2.75f))
            {

                float scaledTime2 = scaledTime - (1.5f / 2.75f);
                return (7.5625f * scaledTime2 * scaledTime2) + 0.75f;

            }
            else if (scaledTime < (2.5f / 2.75f))
            {

                float scaledTime2 = scaledTime - (2.25f / 2.75f);
                return (7.5625f * scaledTime2 * scaledTime2) + 0.937f;

            }
            else
            {

                float scaledTime2 = scaledTime - (2.625f / 2.75f);
                return (7.5625f * scaledTime2 * scaledTime2) + 0.984375f;

            }
        }
        public static float EaseInOutBounce(float t)
        {
            if (t < 0.5)
                return EaseInBounce(t * 2) * 0.5f;
            else
                return (EaseOutBounce((t * 2) - 1) * 0.5f) + 0.5f;
        }
        #endregion

        /// <summary>
        /// Get's the ease float based on the motion type selected.
        /// </summary>
        /// <param name="t">Time (0 to 1).</param>
        /// <param name="type">Motion Type.</param>
        /// <param name="parameters">Desired ease function parameters.</param>
        /// <returns></returns>
        public static float GetEaseFloat(float t, MotionType type, EasingEquationsParameters p)
        {
            float ease = 0;
            switch (type)
            {
                case MotionType.Custom:
                    ease = Custom(t, p.Custom.Curve);
                    break;
                case MotionType.Linear:
                    ease = Linear(t);
                    break;
                case MotionType.EaseIn:
                    ease = EaseIn(t, p.EaseInOut.EasingPower);
                    break;
                case MotionType.EaseOut:
                    ease = EaseOut(t, p.EaseInOut.EasingPower);
                    break;
                case MotionType.EaseInOut:
                    ease = EaseInOut(t, p.EaseInOut.EasingPower);
                    break;
                case MotionType.EaseInBack:
                    ease = EaseInBack(t, p.EaseInOutBack.BackPower);
                    break;
                case MotionType.EaseOutBack:
                    ease = EaseOutBack(t, p.EaseInOutBack.BackPower);
                    break;
                case MotionType.EaseInOutBack:
                    ease = EaseInOutBack(t, p.EaseInOutBack.BackPower);
                    break;
                case MotionType.EaseInElastic:
                    ease = EaseInElastic(t, p.EaseInOutElastic.ElasticityPower);
                    break;
                case MotionType.EaseOutElastic:
                    ease = EaseOutElastic(t, p.EaseInOutElastic.ElasticityPower);
                    break;
                case MotionType.EaseInOutElastic:
                    ease = EaseInOutElastic(t, p.EaseInOutElastic.ElasticityPower);
                    break;
                case MotionType.EaseInBounce:
                    ease = EaseInBounce(t);
                    break;
                case MotionType.EaseOutBounce:
                    ease = EaseOutBounce(t);
                    break;
                case MotionType.EaseInOutBounce:
                    ease = EaseInOutBounce(t);
                    break;
            }
            return ease;
        }
        /// <summary>
        /// Get's the ease float based on the sent easing type.
        /// </summary>
        /// <param name="t">Time (0 to 1).</param>
        /// <param name="easingType">Desired easing type.</param>
        /// <returns></returns>
        public static float GetEaseFloat(float t, EasingType easingType)
        {
            float ease = 0;
            switch (easingType.Easing)
            {
                case MotionType.Custom:
                    ease = Custom(t, easingType.Parameters.Custom.Curve);
                    break;
                case MotionType.Linear:
                    ease = Linear(t);
                    break;
                case MotionType.EaseIn:
                    ease = EaseIn(t, easingType.Parameters.EaseInOut.EasingPower);
                    break;
                case MotionType.EaseOut:
                    ease = EaseOut(t, easingType.Parameters.EaseInOut.EasingPower);
                    break;
                case MotionType.EaseInOut:
                    ease = EaseInOut(t, easingType.Parameters.EaseInOut.EasingPower);
                    break;
                case MotionType.EaseInBack:
                    ease = EaseInBack(t, easingType.Parameters.EaseInOutBack.BackPower);
                    break;
                case MotionType.EaseOutBack:
                    ease = EaseOutBack(t, easingType.Parameters.EaseInOutBack.BackPower);
                    break;
                case MotionType.EaseInOutBack:
                    ease = EaseInOutBack(t, easingType.Parameters.EaseInOutBack.BackPower);
                    break;
                case MotionType.EaseInElastic:
                    ease = EaseInElastic(t, easingType.Parameters.EaseInOutElastic.ElasticityPower);
                    break;
                case MotionType.EaseOutElastic:
                    ease = EaseOutElastic(t, easingType.Parameters.EaseInOutElastic.ElasticityPower);
                    break;
                case MotionType.EaseInOutElastic:
                    ease = EaseInOutElastic(t, easingType.Parameters.EaseInOutElastic.ElasticityPower);
                    break;
                case MotionType.EaseInBounce:
                    ease = EaseInBounce(t);
                    break;
                case MotionType.EaseOutBounce:
                    ease = EaseOutBounce(t);
                    break;
                case MotionType.EaseInOutBounce:
                    ease = EaseInOutBounce(t);
                    break;
            }
            return ease;
        }
    }

    [Serializable]
    public class EasingEquationsParameters
    {
        #region Parameter Classes
        [Serializable]
        public class CustomParameters
        {
            public AnimationCurve Curve;
            public CustomParameters()
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1);
            }
        }
        [Serializable]
        public class EaseInOutParameters
        {
            [Range(2, 25)]
            public int EasingPower;

            public EaseInOutParameters()
            {
                EasingPower = 2;
            }
            public EaseInOutParameters(int easingPower)
            {
                EasingPower = easingPower;
            }
        }
        [Serializable]
        public class EaseInOutBackParameters
        {
            public float BackPower;

            public EaseInOutBackParameters()
            {
                BackPower = 1.5f;
            }
            public EaseInOutBackParameters(float backPower)
            {
                BackPower = backPower;
            }
        }
        [Serializable]
        public class EaseInOutElasticParameters
        {
            [Range(0.1f, 0.9f)]
            public float ElasticityPower;

            public EaseInOutElasticParameters()
            {
                ElasticityPower = 0.5f;
            }
            public EaseInOutElasticParameters(float elasticityPower = 0.5f)
            {
                ElasticityPower = elasticityPower;
            }
        }
        #endregion

        #region Constructors
        public EasingEquationsParameters()
        {
            Custom = new CustomParameters();
            EaseInOut = new EaseInOutParameters();
            EaseInOutBack = new EaseInOutBackParameters();
            EaseInOutElastic = new EaseInOutElasticParameters();
        }

        public EasingEquationsParameters(CustomParameters custom)
        {
            Custom = custom;
            EaseInOut = new EaseInOutParameters();
            EaseInOutBack = new EaseInOutBackParameters();
            EaseInOutElastic = new EaseInOutElasticParameters();
        }

        public EasingEquationsParameters(EaseInOutParameters easeInOut)
        {
            Custom = new CustomParameters();
            EaseInOut = easeInOut;
            EaseInOutBack = new EaseInOutBackParameters();
            EaseInOutElastic = new EaseInOutElasticParameters();
        }

        public EasingEquationsParameters(EaseInOutBackParameters easeInOutBack)
        {
            Custom = new CustomParameters();
            EaseInOut = new EaseInOutParameters();
            EaseInOutBack = easeInOutBack;
            EaseInOutElastic = new EaseInOutElasticParameters();
        }

        public EasingEquationsParameters(EaseInOutElasticParameters easeInOutElastic)
        {
            Custom = new CustomParameters();
            EaseInOut = new EaseInOutParameters();
            EaseInOutBack = new EaseInOutBackParameters();
            EaseInOutElastic = easeInOutElastic;
        }

        public EasingEquationsParameters(CustomParameters custom, EaseInOutParameters easeInOut,
            EaseInOutBackParameters easeInOutBack, EaseInOutElasticParameters easeInOutElastic)
        {
            Custom = custom;
            EaseInOut = easeInOut;
            EaseInOutBack = easeInOutBack;
            EaseInOutElastic = easeInOutElastic;
        }
        #endregion

        public CustomParameters Custom;
        public EaseInOutParameters EaseInOut;
        public EaseInOutBackParameters EaseInOutBack;
        public EaseInOutElasticParameters EaseInOutElastic;
    }
}