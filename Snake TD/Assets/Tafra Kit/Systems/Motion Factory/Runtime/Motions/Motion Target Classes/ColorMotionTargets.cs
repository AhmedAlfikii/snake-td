using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class ColorMotionTargets : MotionTargets
    {
        [Tooltip("The color this graphic should animate towards.")]
        public Color TargetColor;
    }
}