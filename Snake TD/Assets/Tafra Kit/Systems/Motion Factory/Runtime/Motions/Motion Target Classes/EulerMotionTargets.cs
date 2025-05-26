using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class EulerMotionTargets : MotionTargets
    {
        [Tooltip("The euler angles this transform should animate towards.")]
        public Vector3 TargetEuler;
    }
}