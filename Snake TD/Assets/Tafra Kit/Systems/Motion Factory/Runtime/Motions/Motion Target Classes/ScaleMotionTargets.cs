using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class ScaleMotionTargets : MotionTargets
    {
        [Tooltip("The scale this transform should animate towards.")]
        public Vector3 TargetScale;
    }
}