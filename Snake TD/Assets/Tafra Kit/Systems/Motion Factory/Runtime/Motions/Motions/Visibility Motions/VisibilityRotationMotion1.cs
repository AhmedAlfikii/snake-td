using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Rect Transform")]
    public class VisibilityRectMotion : GenericVisibilityMotion<RectTransform>
    {
        [SerializeField] private RectMotionSelfReferences references;
        [SerializeField] private RectTransform targetPlacement;

        public override string MotionName => "Rotation Motion";
        protected override bool IsReferenceAvailable => references.rectTransform;
        protected override RectTransform ReferenceValue => references.rectTransform;

        protected override void SeekEased(float t)
        {
            Debug.Log(t);
            references.rectTransform.position = Vector3.Lerp(StateA.position, StateB.position, t);
        }
    }
}