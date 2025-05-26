using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public class OpacityToOpacityMotion : ObjectMotion
    {
        [Range(0,1)]
        [SerializeField] private float opacityA;
        [Range(0,1)]
        [SerializeField] private float opacityB;
        [SerializeField] private Graphic graphic;

        private Color tempColor;
        private float normalOpacity;
        
        protected override void Initialize()
        {
            base.Initialize();

            normalOpacity = graphic.color.a;
        }

        protected override void ApplyMotion(float easedT, float rawT, bool inverted)
        {
            tempColor = graphic.color;
            tempColor.a = Mathf.LerpUnclamped(inverted ? opacityB : opacityA, inverted ? opacityA : opacityB, easedT);

            graphic.color = tempColor;
        }

        public override void GoToNormalState()
        {
            base.GoToNormalState();

            tempColor = graphic.color;
            tempColor.a = normalOpacity;

            graphic.color = tempColor;
        }
    }
}
