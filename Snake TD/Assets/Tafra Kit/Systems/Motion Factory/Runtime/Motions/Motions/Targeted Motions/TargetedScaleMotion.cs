using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Scale")]
    public class TargetedScaleMotion : GenericTargetedMotion<Vector3>
    {
        [SerializeField] private ScaleMotionTargets targets;
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Scale Motion";
        protected override bool IsReferenceAvailable => references.transform;
        protected override Vector3 ReferenceValue => references.transform.localScale;
        protected override Vector3 TargetValue => targets.TargetScale;

        protected override void SeekTarget(float t, Vector3 target)
        {
            references.transform.localScale = Vector3.LerpUnclamped(animationStartState, target, t);
        }
    }
}