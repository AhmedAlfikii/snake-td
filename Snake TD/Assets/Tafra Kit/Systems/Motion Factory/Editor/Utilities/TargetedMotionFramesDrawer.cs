using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor.MotionFactory
{
    [CustomPropertyDrawer(typeof(TargetedMotionFrames))]
    public class TargetedMotionFramesDrawer : PropertyDrawer
    {
        public VisualTreeAsset uxml;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/UI Toolkit Assets/UXML/Motion Factory/MotionsFramesEditor.uxml");
            uxml.CloneTree(root);

            string viewDataKey = property.propertyPath;
            string isContentExpandedKey = $"{viewDataKey}_IS_CONTENT_EXPANDED";
            bool isContentExpanded = EditorPrefs.GetBool(isContentExpandedKey, true);

            Label propertyNameLabel = root.Q<Label>("PropertyName");
            VisualElement noFramesInfo = root.Q<VisualElement>("NoFramesInfo");
            VisualElement framesList = root.Q<VisualElement>("FramesList");
            Button addFrameButton = root.Q<Button>("AddFrameButton");
            VisualElement header = root.Q<VisualElement>("Header");
            VisualElement headerIcon = root.Q<VisualElement>("HeaderIcon");
            VisualElement content = root.Q<VisualElement>("Content");

            SerializedProperty framesProperty = property.FindPropertyRelative("frames");

            propertyNameLabel.text = property.displayName;
            noFramesInfo.style.display = framesProperty.arraySize == 0 ? DisplayStyle.Flex : DisplayStyle.None;

            addFrameButton.clicked += () =>
            {
                AddFrame();
            };

            header.RegisterCallback<MouseUpEvent>(OnHeaderClicked);

            UpdateHeaderState();
            BuildFramesList();

            #region Local Functions
            void BuildFramesList()
            {
                framesList.Clear();
                for(int i = 0; i < framesProperty.arraySize; i++)
                {
                    VisualElement propertyContainer = new VisualElement();
                    propertyContainer.style.flexGrow = 1;
                    propertyContainer.style.flexDirection = FlexDirection.Row;

                    Button motionRemoveButton = new Button()
                    {
                        text = "-",
                        tooltip = "Remove frame",
                    };

                    motionRemoveButton.style.borderTopLeftRadius = 0f;
                    motionRemoveButton.style.borderTopRightRadius = 5f;
                    motionRemoveButton.style.borderBottomRightRadius = 5f;
                    motionRemoveButton.style.borderBottomLeftRadius = 0f;
                    motionRemoveButton.style.width = 25f;
                    motionRemoveButton.style.marginTop = 4f;
                    motionRemoveButton.style.marginLeft = 0f;
                    motionRemoveButton.style.marginRight = 0f;
                    motionRemoveButton.style.marginBottom = 4f;

                    int motionIndex = i;
                    motionRemoveButton.clicked += () => {
                        RemoveFrame(motionIndex);
                    };

                    PropertyField pf = new PropertyField(framesProperty.GetArrayElementAtIndex(i));
                    pf.style.flexGrow = 1;

                    propertyContainer.Add(pf);
                    propertyContainer.Add(motionRemoveButton);

                    framesList.Add(propertyContainer);
                }
                noFramesInfo.style.display = framesProperty.arraySize > 0 ? DisplayStyle.None : DisplayStyle.Flex;

            }
            void UpdateFramesList()
            {
                BuildFramesList();

                root.Bind(property.serializedObject);
            }
            void AddFrame()
            {
                framesProperty.arraySize++;
                SerializedProperty newFrame = framesProperty.GetArrayElementAtIndex(framesProperty.arraySize - 1);
                newFrame.FindPropertyRelative("motions").arraySize = 0;
                property.serializedObject.ApplyModifiedProperties();

                UpdateFramesList();
            }
            void RemoveFrame(int frameIndex)
            {
                framesProperty.DeleteArrayElementAtIndex(frameIndex);

                property.serializedObject.ApplyModifiedProperties();

                UpdateFramesList();
            }
            void OnHeaderClicked(MouseUpEvent evt)
            {
                isContentExpanded = !isContentExpanded;

                EditorPrefs.SetBool(isContentExpandedKey, isContentExpanded);

                UpdateHeaderState();
            }
            void UpdateHeaderState()
            {
                string headerPlusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Plus.png" : "Tafra Kit/Icons/Tafra_Toolbar Plus.png";
                string headerMinusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Minus.png" : "Tafra Kit/Icons/Tafra_Toolbar Minus.png";

                string headerIconPath = isContentExpanded ? headerMinusIconPath : headerPlusIconPath;

                Texture2D icon = EditorGUIUtility.Load(headerIconPath) as Texture2D;
                headerIcon.style.backgroundImage = new StyleBackground(icon);
                content.style.display = isContentExpanded ? DisplayStyle.Flex : DisplayStyle.None;
            }

            #endregion

            return root;
        }
    }
}