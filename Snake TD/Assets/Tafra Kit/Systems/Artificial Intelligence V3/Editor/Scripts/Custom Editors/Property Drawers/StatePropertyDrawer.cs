using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.Internal.AI3;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.AI3
{
    [CustomPropertyDrawer(typeof(State))]
    public class StatePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            VisualTreeAsset mainUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/StatePropertyDrawer.uxml");

            mainUxml.CloneTree(root);

            VisualElement mainContainer = root.Q<VisualElement>("main-container");

            SerializedProperty transitionsProperty = property.FindPropertyRelative("transitions");

            var childProperties = property.GetVisibleChildren();

            foreach ( var childProperty in childProperties)
            {
                FieldInfo fi = childProperty.GetFieldInfo();

                if(fi != null && fi.GetCustomAttribute<HideInGraphInspector>() != null)
                    continue;

                //Don't draw the transition proprety, we'll draw it last.
                if(childProperty.name == transitionsProperty.name)
                    continue;

                PropertyField propertyField = new PropertyField(childProperty);

                mainContainer.Add(propertyField);
            }

            DrawTransitions(property.serializedObject.targetObject, property.propertyPath, root);

            //Undo.undoRedoPerformed += () =>
            //{
            //    DrawTransitions(property.serializedObject.targetObject, property.propertyPath, root);
            //};

            return root;
        }

        private void DrawTransitions(UnityEngine.Object obj, string propertyPath, VisualElement root)
        {
            SerializedObject serializedObject = new SerializedObject(obj);
            SerializedProperty property = serializedObject.FindProperty(propertyPath);

            SerializedProperty transitionsProperty = property.FindPropertyRelative("transitions");
            VisualTreeAsset transitionUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/StatePropertyDrawerTransition.uxml");
            VisualElement transitionsContainer = root.Q<VisualElement>("transitions-container");
            VisualElement transitionsHolder = root.Q<VisualElement>("transitions");

            if(transitionsHolder.childCount != 0)
                transitionsHolder.Clear();

            if(transitionsProperty.arraySize > 0)
            {
                State fromState = (State)property.managedReferenceValue;

                for(int i = 0; i < transitionsProperty.arraySize; i++)
                {
                    int transitionIndex = i;

                    SerializedProperty transitionProperty = transitionsProperty.GetArrayElementAtIndex(transitionIndex);

                    StateTransition transition = (StateTransition)transitionProperty.boxedValue;
                    State toState = transition.ToState;

                    VisualElement transitionContainer = new VisualElement();
                    transitionUxml.CloneTree(transitionContainer);

                    Label transitionTitle = transitionContainer.Q<Label>("transition-title");
                    if(fromState != toState)
                        transitionTitle.text = $"{fromState.Name} > {toState.Name}";
                    else
                        transitionTitle.text = "Self Transition";
                    
                    VisualElement transitionContent = transitionContainer.Q<VisualElement>("content");
                    transitionContent.style.display = DisplayStyle.None;

                    //PropertyField transitionField = new PropertyField(transitionProperty);

                    //transitionContent.Add(transitionField);

                    transitionsHolder.Add(transitionContainer);

                    #region Buttons
                    Button headerButton = transitionContainer.Q<Button>("header-button");
                    headerButton.clicked += () =>
                    {
                        transitionContent.style.display = transitionContent.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
                    };

                    Button upButton = transitionContainer.Q<Button>("up-button");
                    Button downButton = transitionContainer.Q<Button>("down-button");

                    if(transitionIndex == 0)
                        upButton.SetEnabled(false);
                    else if(transitionIndex == transitionsProperty.arraySize - 1)
                        downButton.SetEnabled(false);

                    upButton.clicked += () =>
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, "Move Transition Up");

                        transitionsProperty.MoveArrayElementUp(transitionIndex, false);

                        DrawTransitions(obj, propertyPath, root);

                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    };
                    downButton.clicked += () =>
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, "Move Transition Down");
                        transitionsProperty.MoveArrayElementDown(transitionIndex, false);

                        DrawTransitions(obj, propertyPath, root);

                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    };
                    #endregion
                }
            }
            else
                transitionsContainer.style.display = DisplayStyle.None;

        }
    }
}