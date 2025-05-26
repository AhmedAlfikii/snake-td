using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using TafraKit.CharacterControls;
using UnityEditor.UIElements;
using System;

namespace TafraKitEditor.CharacterControls
{
    [CustomEditor(typeof(CharacterAbilities))]
    public class CharacterAbilitiesEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            SerializedProperty abilitiesContainerTypeProperty = serializedObject.FindProperty("abilitiesContainerType");
            SerializedProperty externalAbilitiesContainerProperty = serializedObject.FindProperty("externalAbilitiesContainer");
            SerializedProperty internalAbilitiesContainerProperty = serializedObject.FindProperty("internalAbilitiesContainer");
            SerializedProperty externalBlackboardsProperty = serializedObject.FindProperty("externalBlackboards");

            PropertyField abilitiesContainerTypePropertyField = new PropertyField(abilitiesContainerTypeProperty);
            PropertyField externalAbilitiesContainerField = new PropertyField(externalAbilitiesContainerProperty);
            PropertyField internalAbilitiesContainerField = new PropertyField(internalAbilitiesContainerProperty);
            PropertyField externalBlackboardsField = new PropertyField(externalBlackboardsProperty);

            abilitiesContainerTypePropertyField.RegisterValueChangeCallback((evt) =>
            {
                if (abilitiesContainerTypeProperty.enumValueIndex == 1)
                {
                    internalAbilitiesContainerField.style.display = DisplayStyle.None;
                    externalAbilitiesContainerField.style.display = DisplayStyle.Flex;
                }
                else
                {
                    internalAbilitiesContainerField.style.display = DisplayStyle.Flex;
                    externalAbilitiesContainerField.style.display = DisplayStyle.None;
                }
            });

            if (abilitiesContainerTypeProperty.enumValueIndex == 1)
            {
                internalAbilitiesContainerField.style.display = DisplayStyle.None;
                externalAbilitiesContainerField.style.display = DisplayStyle.Flex;
            }
            else
            {
                internalAbilitiesContainerField.style.display = DisplayStyle.Flex;
                externalAbilitiesContainerField.style.display = DisplayStyle.None;
            }

            root.Add(abilitiesContainerTypePropertyField);
            root.Add(externalAbilitiesContainerField);
            root.Add(internalAbilitiesContainerField);
            root.Add(externalBlackboardsField);

            return root;
        }
    }
}