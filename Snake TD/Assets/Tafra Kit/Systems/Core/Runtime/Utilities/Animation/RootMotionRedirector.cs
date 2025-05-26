using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class RootMotionRedirector : MonoBehaviour
    {
        [SerializeField] private Transform redirectTo;

        private Vector3 deltaPosition;
        private Vector3 deltaRotation;
        private void OnAnimatorMove()
        {
            UpdatePosition();
            UpdateRotation();
        }
        private void UpdatePosition()
        {
            var newPosition = redirectTo.position + deltaPosition;

            redirectTo.position = newPosition;

            deltaPosition = Vector3.zero;
        }

        private void UpdateRotation()
        {
            var newAngles = redirectTo.eulerAngles + deltaRotation;

            redirectTo.eulerAngles = newAngles;

            deltaRotation = Vector3.zero;
        }
        public void SetDeltaPosition(float deltaX, float deltaY, float deltaZ)
        {
            deltaPosition = new Vector3(deltaX, deltaY, deltaZ);
        }
        public void SetDeltaRotation(float deltaX, float deltaY, float deltaZ)
        {
            deltaRotation = new Vector3(deltaX, deltaY, deltaZ);
        }
    }
}