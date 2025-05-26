using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class OpacityMotionTargets : MotionTargets
    {
        [Tooltip("The opacity this graphic should animate towards.")]
        [Range(0, 1)]
        public float TargetOpacity;
    }
}