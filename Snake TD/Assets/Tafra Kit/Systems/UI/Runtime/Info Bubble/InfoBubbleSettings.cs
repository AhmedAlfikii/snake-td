using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Internal.UI;

namespace TafraKit.UI
{
    public class InfoBubbleSettings : SettingsModule
    {
        public bool Enabled;
        public Canvas BubblesCanvas;
        public InfoBubble[] Bubbles;

        [SerializeField] private float paddingLeft = 20;
        [SerializeField] private float paddingRight = 20;
        [SerializeField] private float paddingTop = 20;
        [SerializeField] private float paddingBottom = 20;

        public float PaddingLeft => paddingLeft;
        public float PaddingRight => paddingRight;
        public float PaddingTop => paddingTop;
        public float PaddingBottom => paddingBottom;
        public override int Priority => 14;
        public override string Name => "UI/Info Bubble";
        public override string Description => "Display info bubbles adaptively";
    }
}