using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TafraKit.MotionFactory;
using ZUtilities;

namespace TafraKitEditor.MotionFactory
{
    [CustomEditor(typeof(InOutMotionController))]
    public class InOutMotionControllerEditor : Editor
    {
        public VisualTreeAsset uxml;
        
        private static string tabActiveClassName = "tabButtonActive";
        private static int activeTabIndex = 0;

        private List<Button> tabButtons = new List<Button>();
        private List<VisualElement> tabContentVisualElements = new List<VisualElement>();

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            if (uxml != null) 
                uxml.CloneTree(root);
            
            tabButtons.Clear();

            Button inButton = root.Q<Button>("InButton");
            Button outButton = root.Q<Button>("OutButton");

            tabButtons.Add(inButton);
            tabButtons.Add(outButton);

            for(int i = 0; i < tabButtons.Count; i++)
            {
                int buttonIndex = i;
                Button button = tabButtons[buttonIndex];

                button.AddToClassList(tabActiveClassName);
                button.EnableInClassList(tabActiveClassName, buttonIndex == activeTabIndex);
                
                button.clicked += () => { TabButtonClicked(buttonIndex); };
            }

            VisualElement inMotionContainer = root.Q<VisualElement>("InMotionContainer");
            VisualElement outMotionContainer = root.Q<VisualElement>("OutMotionContainer");

            tabContentVisualElements.Add(inMotionContainer);
            tabContentVisualElements.Add(outMotionContainer);

            DisplayActiveTabContent();

            return root;
        }

        private void TabButtonClicked(int tabIndex)
        {
            activeTabIndex = tabIndex;

            for(int i = 0; i < tabButtons.Count; i++)
            {
                Button button = tabButtons[i];
                button.EnableInClassList(tabActiveClassName, i == tabIndex);
            }

            DisplayActiveTabContent();
        }

        private void DisplayActiveTabContent()
        {
            for(int i = 0; i < tabContentVisualElements.Count; i++)
            {
                tabContentVisualElements[i].style.display = (i == activeTabIndex ? DisplayStyle.Flex : DisplayStyle.None);
            }
        }
    }
}
