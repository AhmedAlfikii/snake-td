using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class PositionToPositionMotion : ObjectMotion
    {
        [SerializeField] private Vector3 localPositionA;
        [SerializeField] private Vector3 localPositionB;

        private Vector3 normalLocalPosition;

        protected override void Initialize()
        {
            base.Initialize();

            normalLocalPosition = transform.localPosition;
        }

        protected override void ApplyMotion(float easedT, float rawT, bool inverted)
        {
            transform.localPosition = Vector3.Lerp(inverted ? localPositionB : localPositionA, inverted ? localPositionA : localPositionB, easedT);
        }

        public override void GoToNormalState()
        {
            base.GoToNormalState();

            transform.localPosition = normalLocalPosition;
        }
    }
}