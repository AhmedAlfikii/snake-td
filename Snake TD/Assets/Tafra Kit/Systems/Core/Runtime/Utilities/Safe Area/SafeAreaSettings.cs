using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class SafeAreaSettings : SettingsModule
    {
        [Range(0, 1)]
        public float TopMargin;
        [Range(0, 1)]
        public float BotMargin;
        [Range(0, 1)]
        public float LeftMargin;
        [Range(0, 1)]
        public float RightMargin;

        public override int Priority => 10;

        public override string Name => "UI/Safe Area";

        public override string Description => "Controls how all the safe areas in the game behave.";
    }
}