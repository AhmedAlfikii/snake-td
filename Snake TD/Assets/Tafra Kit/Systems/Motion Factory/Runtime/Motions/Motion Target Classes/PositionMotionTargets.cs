using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class PositionMotionTargets : MotionTargets
    {
        [Tooltip("The position this transform should animate towards.")]
        public Vector3 TargetPosition;
        public Space MotionSpace;
    }
}