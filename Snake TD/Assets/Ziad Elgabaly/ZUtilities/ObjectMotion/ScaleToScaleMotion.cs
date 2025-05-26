using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class ScaleToScaleMotion : ObjectMotion
    {
        [SerializeField] private Vector3 scaleA;
        [SerializeField] private Vector3 scaleB;

        private Vector3 normalLocalScale;

        protected override void Initialize()
        {
            base.Initialize();

            normalLocalScale = transform.localScale;
        }

        protected override void ApplyMotion(float easedT, float rawT, bool inverted)
        {
            transform.localScale = Vector3.LerpUnclamped(inverted ? scaleB : scaleA, inverted ? scaleA : scaleB, easedT);
        }


        public override void GoToNormalState()
        {
            base.GoToNormalState();

            transform.localScale = normalLocalScale;
        }
    }
}