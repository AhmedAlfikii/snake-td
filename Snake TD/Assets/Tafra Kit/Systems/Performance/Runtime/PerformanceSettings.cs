using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.Performance
{
    public class PerformanceSettings : SettingsModule
    {
        public bool Enabled = true;

        [Header("Frame Rate")]
        [SerializeField] private bool controlFrameRate = true;
        [SerializeField] private int targetFrameRate = 60;
        [Tooltip("Should the editor's target framerate also be set?")]
        [SerializeField] private bool applyFrameRateToEditor;

        [Header("Prevent Sleep")]
        [Tooltip("Should the mobile screen not automatically turn off?")]
        [SerializeField] private bool screenStayAwake;

        public bool ControlFrameRate => controlFrameRate;
        public int TargetFrameRate => targetFrameRate;
        public bool ApplyFrameRateToEditor => applyFrameRateToEditor;
        public bool ScreenStayAwake => screenStayAwake;
        public override int Priority => 30;
        public override string Name => "General/Performance";
        public override string Description => "Control the performance of the game.";
    }
}