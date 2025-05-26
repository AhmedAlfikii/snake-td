using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    [CustomEditor(typeof(PlayableActor))]
    public class PlayableActorEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            

            PropertyField statsContainerField = new PropertyField(serializedObject.FindProperty("statsContainer"));
            PropertyField characterControlCategoriesField = new PropertyField(serializedObject.FindProperty("characterControlCategories"));
            PropertyField cachedComponentsField = new PropertyField(serializedObject.FindProperty("cachedComponents"));
            PropertyField crCachedComponentsField = new PropertyField(serializedObject.FindProperty("componentProviderCachedComponents"));
            PropertyField modulesContainerField = new PropertyField(serializedObject.FindProperty("modulesContainer"));

            root.Add(statsContainerField);
            root.Add(characterControlCategoriesField);
            root.Add(cachedComponentsField);
            root.Add(crCachedComponentsField);
            root.Add(modulesContainerField);

            return root;
        }
    }
}