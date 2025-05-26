using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Color")]
    public class TargetedColorMotion : GenericTargetedMotion<Color>
    {
        [SerializeField] private ColorMotionTargets targets;
        [SerializeField] private GraphicMotionSelfReferences references;

        public override string MotionName => "Color Motion";
        protected override bool IsReferenceAvailable => references.graphic;
        protected override Color ReferenceValue => references.graphic.color;
        protected override Color TargetValue => targets.TargetColor;

        protected override void SeekTarget(float t, Color target)
        {
            references.graphic.color = Color.LerpUnclamped(animationStartState, target, t);
        }
    }
}