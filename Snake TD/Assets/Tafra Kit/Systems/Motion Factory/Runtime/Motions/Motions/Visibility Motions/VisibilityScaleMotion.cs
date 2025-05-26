using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Scale")]
    public class VisibilityScaleMotion : GenericVisibilityMotion<Vector3>
    {
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Scale Motion";
        protected override bool IsReferenceAvailable => references.transform;
        protected override Vector3 ReferenceValue => references.transform.localScale;

        protected override void SeekEased(float t)
        {
            references.transform.localScale = Vector3.LerpUnclamped(StateA, StateB, t);
        }
    }
}