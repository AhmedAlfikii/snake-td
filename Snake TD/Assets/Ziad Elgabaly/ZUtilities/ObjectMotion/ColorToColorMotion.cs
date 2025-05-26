using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public class ColorToColorMotion : ObjectMotion
    {
        [SerializeField] private Color colorA;
        [SerializeField] private Color colorB;
        [SerializeField] private Graphic graphic;

        private Color normalColor;

       protected override void Initialize()
        {
            base.Initialize();

            normalColor = graphic.color;
        }

        protected override void ApplyMotion(float easedT, float rawT, bool inverted)
        {
            graphic.color = Color.LerpUnclamped(inverted ? colorB : colorA, inverted ? colorA : colorB, easedT);
        }

        public override void GoToNormalState()
        {
            base.GoToNormalState();

            graphic.color = normalColor;
        }
    }
}
