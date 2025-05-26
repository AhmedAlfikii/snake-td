using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Position")]
    public class TargetedPositionMotion : GenericTargetedMotion<Vector3>
    {
        [SerializeField] private PositionMotionTargets targets;
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Position Motion";

        protected override Vector3 ReferenceValue
        {
            get
            {
                if(targets.MotionSpace == Space.World)
                    return references.transform.position;
                else
                    return references.transform.localPosition;
            }
        }
        protected override Vector3 TargetValue => targets.TargetPosition;

        protected override bool IsReferenceAvailable => references.transform;

        protected override void SeekTarget(float t, Vector3 target)
        {
            if(targets.MotionSpace == Space.World)
                references.transform.position = Vector3.LerpUnclamped(animationStartState, target, t);
            else if(targets.MotionSpace == Space.Self)
                references.transform.localPosition = Vector3.LerpUnclamped(animationStartState, target, t);
        }
    }
}