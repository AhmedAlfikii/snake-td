using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class EasingType
    {
        public MotionType Easing = MotionType.EaseIn;
        public EasingEquationsParameters Parameters;

        public EasingType()
        {
            Easing = MotionType.EaseIn;
            Parameters = new EasingEquationsParameters();
        }
        public EasingType(MotionType easing, EasingEquationsParameters parameters)
        {
            Easing = easing;
            Parameters = parameters;
        }
    }
}