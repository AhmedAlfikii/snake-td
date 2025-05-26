using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [Serializable]
    public class TwoStatesEulerMotion : GenericTwoStatesMotion<Vector3>
    {
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Euler Motion";
        protected override bool IsReferenceAvailable => references.transform;

        protected override void SeekState(float t)
        {
            references.transform.rotation = Quaternion.SlerpUnclamped(Quaternion.Euler(stateA), Quaternion.Euler(stateB), t);
        }
    }
}