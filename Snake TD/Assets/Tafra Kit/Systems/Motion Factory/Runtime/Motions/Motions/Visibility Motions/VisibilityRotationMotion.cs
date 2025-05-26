using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Rotation")]
    public class VisibilityRotationMotion : GenericVisibilityMotion<Vector3>
    {
        [SerializeField] private TransformMotionSelfReferences references;

        public override string MotionName => "Rotation Motion";
        protected override bool IsReferenceAvailable => references.transform;
        protected override Vector3 ReferenceValue => references.transform.eulerAngles;

        protected override void SeekEased(float t)
        {
            references.transform.rotation = Quaternion.SlerpUnclamped(Quaternion.Euler(StateA), Quaternion.Euler(StateB), t);
        }
    }
}