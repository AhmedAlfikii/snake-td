using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.MotionFactory
{

    [CustomPropertyDrawer(typeof(TargetedScaleMotion))]
    public class TargetedScaleMotionDrawer : TargetedMotionDrawer
    {
        protected override string Name => "Scale Motion";
    }
}