using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Opacity")]
    public class TargetedOpacityMotion : GenericTargetedMotion<float>
    {
        [SerializeField] private OpacityMotionTargets targets;
        [SerializeField] private OpacityMotionSelfReferences references;

        public override string MotionName => "Opacity Motion";
        protected override bool IsReferenceAvailable => references.IsAvailable();
        protected override float ReferenceValue => references.GetCurrentOpacity();
        protected override float TargetValue => targets.TargetOpacity;

        protected override void SeekTarget(float t, float target)
        {
            references.SetOpacity(Mathf.LerpUnclamped(animationStartState, target, t));
        }
    }
}