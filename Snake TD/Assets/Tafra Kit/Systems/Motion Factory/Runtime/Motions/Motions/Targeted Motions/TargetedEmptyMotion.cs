using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Empty")]
    public class TargetedEmptyMotion : GenericTargetedMotion<float>
    {
        public override string MotionName => "Empty Motion";
        protected override bool IsReferenceAvailable => true;
        protected override float ReferenceValue => 0;
        protected override float TargetValue => 0;

        protected override void SeekTarget(float t, float target)
        {
        }
    }
}