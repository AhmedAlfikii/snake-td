using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.MotionFactory
{

    [CustomPropertyDrawer(typeof(VisibilityRotationMotion))]
    public class VisibilityRotationMotionDrawer : VisibilityMotionDrawer
    {
        protected override string Name => "Rotation Motion";
    }
}