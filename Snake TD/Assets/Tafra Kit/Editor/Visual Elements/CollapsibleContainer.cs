using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;

namespace TafraKitEditor
{
    public class CollapsibleContainer : VisualElement
    {
        private SerializedProperty property;
        private string title;
        private bool drawContentAsIs;
        private string isContentExpandedKey;
        private bool isContentExpanded;

        private VisualElement mainContainer;
        private VisualElement content;
        private Button headerButton;
        private VisualElement headerSign;

        public VisualElement Content => content;
        public Button HeaderButton => headerButton;

        public CollapsibleContainer(string title, SerializedProperty property, bool drawContent = true, bool drawContentAsIs = false, bool expandedByDefault = true, string uxmlOverride = null)
        {
            this.property = property;
            this.title = title;
            this.drawContentAsIs = drawContentAsIs;

            string viewDataKey = property.propertyPath;
            isContentExpandedKey = $"{viewDataKey}_IS_CONTENT_EXPANDED";
            isContentExpanded = EditorPrefs.GetBool(isContentExpandedKey, expandedByDefault);

            string uxmlPath = !string.IsNullOrEmpty(uxmlOverride) ? uxmlOverride : "Assets/Tafra Kit/UI Toolkit Assets/UXML/General/GenericCollapsibleContainer.uxml";
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            uxml.CloneTree(this);

            mainContainer = this.Q<VisualElement>("main-container");
            content = this.Q<VisualElement>("content");
            headerSign = this.Q<VisualElement>("header-sign");
            headerButton = this.Q<Button>("header");
            headerButton.AddManipulator(new ContextualMenuManipulator((ev) =>
            {
                TafraEditorUtility.BuildPropertyContextualMenu(property, ev);
            }));

            SetupHeader();
            UpdateHeaderState();

            if (drawContent)
                DrawContent();
        }
        public CollapsibleContainer(string title, string viewDataKey = "", bool expandedByDefault = true, string uxmlOverride = null)
        {
            this.title = title;

            isContentExpandedKey = $"{viewDataKey}_IS_CONTENT_EXPANDED";
            isContentExpanded = EditorPrefs.GetBool(isContentExpandedKey, expandedByDefault);

            string uxmlPath = !string.IsNullOrEmpty(uxmlOverride) ? uxmlOverride : "Assets/Tafra Kit/UI Toolkit Assets/UXML/General/GenericCollapsibleContainer.uxml";
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            uxml.CloneTree(this);

            mainContainer = this.Q<VisualElement>("main-container");
            content = this.Q<VisualElement>("content");
            headerSign = this.Q<VisualElement>("header-sign");
            headerButton = this.Q<Button>("header");

            SetupHeader();
            UpdateHeaderState();
        }

        private void SetupHeader()
        {
            headerButton.text = title;
            headerButton.clicked += OnHeaderButtonClicked;
        }
        private void OnHeaderButtonClicked()
        {
            isContentExpanded = !isContentExpanded;

            EditorPrefs.SetBool(isContentExpandedKey, isContentExpanded);

            UpdateHeaderState();
        }
        private void UpdateHeaderState()
        {
            string headerPlusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Plus.png" : "Tafra Kit/Icons/Tafra_Toolbar Plus.png";
            string headerMinusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Minus.png" : "Tafra Kit/Icons/Tafra_Toolbar Minus.png";

            string headerIconPath = isContentExpanded ? headerMinusIconPath : headerPlusIconPath;

            Texture2D icon = EditorGUIUtility.Load(headerIconPath) as Texture2D;
            headerSign.style.backgroundImage = new StyleBackground(icon);

            content.style.display = isContentExpanded ? DisplayStyle.Flex : DisplayStyle.None;
        }
        private void DrawContent()
        {
            if(drawContentAsIs)
            {
                PropertyField contentPF = new PropertyField(property);
                content.Add(contentPF);
            }
            else
            {
                var children = property.GetVisibleChildren();

                foreach(var child in children)
                { 
                    PropertyField childPF = new PropertyField(child);
                    content.Add(childPF);
                }
            }
        }
    }
}