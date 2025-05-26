using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MotionFactory
{
    [SearchMenuItem("Opacity")]
    public class VisibilityOpacityMotion : GenericVisibilityMotion<float>
    {
        [SerializeField] private OpacityMotionSelfReferences references;
      
        public override string MotionName => "Opacity Motion";
        protected override bool IsReferenceAvailable => references.IsAvailable();
        protected override float ReferenceValue => references.GetCurrentOpacity();

        protected override void SeekEased(float t)
        {
            references.SetOpacity(Mathf.LerpUnclamped(StateA, StateB, t));
        }
    }
}