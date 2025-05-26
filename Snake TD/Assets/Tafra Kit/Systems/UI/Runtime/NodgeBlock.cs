using System;
using UnityEngine;

namespace TafraKit.UI
{
    [Serializable]
    public struct NodgeBlock
    {
        public Vector2 normalNodge;
        public Vector2 highlightedNodge;
        public Vector2 pressedNodge;
        public Vector2 selectedNodge;
        public Vector2 disabledNodge;

        public float defaultNodgeDuration;
        public EasingType defaultNodgeEasing;

        public bool customPressedNodgeProperties;
        public float pressedNodgeDuration;
        public EasingType pressedNodgeEasing;
    }
}