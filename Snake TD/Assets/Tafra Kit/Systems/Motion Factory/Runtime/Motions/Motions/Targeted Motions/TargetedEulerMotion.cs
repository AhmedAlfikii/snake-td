using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Euler")]
    public class TargetedEulerMotion : GenericTargetedMotion<Quaternion>
    {
        [SerializeField] private EulerMotionTargets targets;
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Euler Motion";
        protected override bool IsReferenceAvailable => references.transform;
        protected override Quaternion ReferenceValue => references.transform.rotation;
        protected override Quaternion TargetValue => Quaternion.Euler(targets.TargetEuler);

        protected override void SeekTarget(float t, Quaternion target)
        {
            references.transform.rotation = Quaternion.SlerpUnclamped(animationStartState, target, t);
        }
    }
}