using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Internal.AI3;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    [CustomPropertyDrawer(typeof(StateTransition))]
    public class StateTransitionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            SerializedProperty evaluateOnCompleteOnlyProperty = property.FindPropertyRelative("evaluateOnCompleteOnly");
            SerializedProperty conditionsGroupProperty = property.FindPropertyRelative("conditionsGroup");

            HelpBox helpBox = new HelpBox("No conditions set. Transitions that have no conditions will only be evaluated after the state is completed.", HelpBoxMessageType.Info);
           
            PropertyField evaluateOnCompleteField = new PropertyField(evaluateOnCompleteOnlyProperty);
            PropertyField conditionsGroupField = new PropertyField(conditionsGroupProperty);

            conditionsGroupField.RegisterValueChangeCallback((ev) =>
            {
                int conditionsCount = conditionsGroupProperty.FindPropertyRelative("conditions").arraySize;

                if(conditionsCount == 0)
                    helpBox.style.display = DisplayStyle.Flex;
                else
                    helpBox.style.display = DisplayStyle.None;
            });

            root.Add(evaluateOnCompleteField);
            root.Add(helpBox);
            root.Add(conditionsGroupField);

            return root;
        }
    }
}