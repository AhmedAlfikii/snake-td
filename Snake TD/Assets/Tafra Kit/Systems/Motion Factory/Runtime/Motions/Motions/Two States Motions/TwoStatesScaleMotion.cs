using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class TwoStatesScaleMotion : GenericTwoStatesMotion<Vector3>
    {
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Scale Motion";
        protected override bool IsReferenceAvailable => references.transform;

        protected override void SeekState(float t)
        {
            references.transform.localScale = Vector3.LerpUnclamped(stateA, stateB, t);
        }
    }
}