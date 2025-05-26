using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public struct CameraShakeProperties
    {
        [Tooltip("Max change in position in all axes.")]
        public Vector3 PositionDisplacement;
        [Tooltip("Max change in rotation (in degrees) in all axes.")]
        public Vector3 RotationDisplacement;
        [Tooltip("The duration of the shake.")]
        public float Duration;

        public CameraShakeProperties(Vector3 positionDisplacement, Vector3 rotationDisplacement, float duration)
        {
            PositionDisplacement = positionDisplacement;
            RotationDisplacement = rotationDisplacement;
            Duration = duration;
        }
    }
}