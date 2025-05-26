using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.MotionFactory
{

    [CustomPropertyDrawer(typeof(TwoStatesEulerMotion))]
    public class TwoStatesEulerMotionDrawer : TwoStatesMotionDrawer
    {
        protected override string Name => "Euler Motion";
    }
}