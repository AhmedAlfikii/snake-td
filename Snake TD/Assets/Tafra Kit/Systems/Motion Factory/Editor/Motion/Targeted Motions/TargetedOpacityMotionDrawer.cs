using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.MotionFactory
{

    [CustomPropertyDrawer(typeof(TargetedOpacityMotion))]
    public class TargetedOpacityMotionDrawer : TargetedMotionDrawer
    {
        protected override string Name => "Opacity Motion";
    }
}