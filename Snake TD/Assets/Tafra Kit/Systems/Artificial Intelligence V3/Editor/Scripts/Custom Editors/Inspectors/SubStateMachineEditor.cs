using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    [CustomEditor(typeof(SubStateMachine))]
    public class SubStateMachineEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return new VisualElement();
        }

        public override void OnInspectorGUI()
        {

        }
    }
}