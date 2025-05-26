using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Position")]
    public class VisibilityPositionMotion : GenericVisibilityMotion<Vector3>
    {
        [SerializeField] private TransformMotionSelfReferences references;
        [SerializeField] private int myInt;
        [SerializeField] private List<int> myListOfInt;
        public override string MotionName => "Position Motion";
        protected override bool IsReferenceAvailable => references.transform;
        protected override Vector3 ReferenceValue => references.transform.position;

        protected override void SeekEased(float t)
        {
            references.transform.position = Vector3.LerpUnclamped(StateA, StateB, t);
        }
    }
}