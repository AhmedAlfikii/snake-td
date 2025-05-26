using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TafraKit;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Threading.Tasks;
using System.Reflection;

namespace TafraKitEditor
{
    public abstract class ReorderableListContainerDrawer : PropertyDrawer
    {
        protected abstract Type SupportedType { get; }
        protected abstract string ListPropertyName { get; }
        protected abstract string UXMLPathOverride { get; }
        protected abstract string SingleElementName { get; }
        protected abstract string PluralElementName { get; }
        protected abstract bool ExpandedByDefault { get; }
        protected abstract Color AddButtonColor { get; }
        protected abstract Color ElementBackgroundColor { get; }
        protected abstract Color ElementBorderColor { get; }
        protected abstract bool IsListSerialziedByReference { get; }
        /// <summary>
        /// Should you be able to use only one of each type (true) or allow multiple instances of the same type (false).
        /// </summary>
        protected abstract bool MaintainUniqueTypeElements { get; }
        protected abstract bool OverrideElementDrawer { get; }
        protected virtual bool IsSearchEnabled => false;

        private string defaultUXMLPath = "Assets/Tafra Kit/UI Toolkit Assets/UXML/General/ReorderableListContainer.uxml";

        private string searchInput;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Type[] supportedMotionTypes = ClassesGatherer.GetChildrenOf(SupportedType);

            IntContainer curElementsCount = new IntContainer();
            string viewDataKey = property.propertyPath;
            string isContentExpandedKey = $"{viewDataKey}_IS_CONTENT_EXPANDED";

            bool isContentExpanded = EditorPrefs.GetBool(isContentExpandedKey, ExpandedByDefault);
            bool isElementChoicesExpanded = false;

            VisualElement root = new VisualElement();

            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(string.IsNullOrEmpty(UXMLPathOverride) ? defaultUXMLPath : UXMLPathOverride);
            uxml.CloneTree(root);

            VisualElement headerIcon = root.Q<VisualElement>("HeaderIcon");

            UpdateHeaderState();

            Label propertyNameLabel = root.Q<Label>("PropertyName");
            string propertyName = property.displayName;

            //if(propertyName.EndsWith("Container"))
            //    propertyName = propertyName.Substring(0, propertyName.Length - 10);

            propertyNameLabel.text = propertyName;

            VisualElement content = root.Q<VisualElement>("Content");
            content.style.display = isContentExpanded ? DisplayStyle.Flex : DisplayStyle.None;

            Button headerButton = root.Q<Button>("HeaderButton");
            headerButton.clicked += OnHeaderClicked;

            SerializedProperty elementsProperty = property.FindPropertyRelative(ListPropertyName);

            VisualElement elementChoices = root.Q<VisualElement>("ElementChoices");

            Button addElementButton = root.Q<Button>("AddElementButton");
            addElementButton.text = $"Add {SingleElementName}";
            addElementButton.style.backgroundColor = AddButtonColor;
            
            VisualElement searchBox = root.Q<VisualElement>("SearchBox");
            
            TextField searchField = root.Q<TextField>("SearchField");
            
            searchBox.style.display = DisplayStyle.None;
            
            Label noElementsSearchLabel = root.Q<Label>("NoElementsSearchLabel");

            noElementsSearchLabel.style.display = DisplayStyle.None;
            
            searchField.RegisterCallback<ChangeEvent<string>>(OnSearchFieldValueChanged);

            addElementButton.clicked += OnAddElementButtonClicked;

            VisualElement elementsList = root.Q<VisualElement>("ElementsList");
            curElementsCount.IntValue = elementsProperty.arraySize;

            VisualElement noElementsInfo = root.Q<VisualElement>("NoElementsInfo");
            Label noElementsInfoLabel = root.Q<Label>("NoElementsInfoLabel");
            noElementsInfoLabel.text = $"No {PluralElementName.ToLower()} added (press the button below).";

            if(IsListSerialziedByReference)
            {
                List<string> supportedTypesNames = new List<string>();
                for(int i = 0; i < supportedMotionTypes.Length; i++)
                {
                    supportedTypesNames.Add(supportedMotionTypes[i].Name);
                }

                bool noMoreElementUniqueChoices = MaintainUniqueTypeElements ? elementsProperty.arraySize >= supportedMotionTypes.Length : false;

                addElementButton.style.display = noMoreElementUniqueChoices ? DisplayStyle.None : DisplayStyle.Flex;
                elementChoices.style.display = isElementChoicesExpanded ? DisplayStyle.Flex : DisplayStyle.None;

                if(isElementChoicesExpanded)
                    GenerateElementChoicesList();

                addElementButton.AddToClassList("listMainButtonActive");
                addElementButton.EnableInClassList("listMainButtonActive", isElementChoicesExpanded);
            }

            BuildElementsList();

            PropertyField elementArrayCountPF = new PropertyField();
            elementArrayCountPF.bindingPath = $"{ListPropertyName}.Array.size";
            elementArrayCountPF.style.display = DisplayStyle.None;

            elementArrayCountPF.RegisterCallback<ChangeEvent<int>>(OnElementsArraySizeValueChanged);
            root.Add(elementArrayCountPF);

            #region Local Functions
            async void OnElementsArraySizeValueChanged(ChangeEvent<int> newSize)
            {
                if(curElementsCount.IntValue == newSize.newValue) return;

                elementArrayCountPF.UnregisterCallback<ChangeEvent<int>>(OnElementsArraySizeValueChanged);

                UpdateElementsList();

                await Task.Yield();

                elementArrayCountPF.RegisterCallback<ChangeEvent<int>>(OnElementsArraySizeValueChanged);
            }
            void UpdateHeaderState()
            {
                string headerPlusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Plus.png" : "Tafra Kit/Icons/Tafra_Toolbar Plus.png";
                string headerMinusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Minus.png" : "Tafra Kit/Icons/Tafra_Toolbar Minus.png";

                string headerIconPath = isContentExpanded ? headerMinusIconPath : headerPlusIconPath;

                Texture2D icon = EditorGUIUtility.Load(headerIconPath) as Texture2D;
                headerIcon.style.backgroundImage = new StyleBackground(icon);
            }
            void OnHeaderClicked()
            {
                isContentExpanded = !isContentExpanded;

                EditorPrefs.SetBool(isContentExpandedKey, isContentExpanded);

                content.style.display = isContentExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                UpdateHeaderState();
            }
            void OnAddElementButtonClicked()
            {
                isElementChoicesExpanded = !isElementChoicesExpanded;

                searchField.value = searchInput = String.Empty;
                
                searchBox.style.display = IsSearchEnabled && isElementChoicesExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                
                elementChoices.style.display = isElementChoicesExpanded ? DisplayStyle.Flex : DisplayStyle.None;

                addElementButton.EnableInClassList("listMainButtonActive", isElementChoicesExpanded);

                if(IsListSerialziedByReference)
                {
                    if(isElementChoicesExpanded)
                        GenerateElementChoicesList();
                }
                else
                    AddElement(SupportedType);
            }
            void OnSearchFieldValueChanged(ChangeEvent<string> evt)
            {
                searchInput = searchField.value.ToLower();
                
                GenerateElementChoicesList();
            }
            void GenerateElementChoicesList()
            {
                elementChoices.Clear();
                int availableMotionChoices = 0;
                for(int i = 0; i < supportedMotionTypes.Length; i++)
                {
                    Type motionType = supportedMotionTypes[i];
                    
                    if(!String.IsNullOrEmpty(searchInput) && !motionType.Name.ToLower().Contains(searchInput))
                        continue;

                    if(MaintainUniqueTypeElements)
                    {
                        bool motionAlreadyActive = false;
                        for(int activeMotionIndex = 0; activeMotionIndex < elementsProperty.arraySize; activeMotionIndex++)
                        {
                            SerializedProperty activeMotion = elementsProperty.GetArrayElementAtIndex(activeMotionIndex);
                            if(activeMotion.managedReferenceValue.GetType() == motionType)
                            {
                                motionAlreadyActive = true;
                                break;
                            }
                        }

                        if(motionAlreadyActive)
                            continue;
                    }

                    Button motionChoiceButton = new Button();

                    motionChoiceButton.AddToClassList("listButton");
                    string motionName = motionType.Name;

                    motionChoiceButton.text = motionName;
                    motionChoiceButton.clicked += () =>
                    {
                        AddElement(motionType);
                        GenerateElementChoicesList();
                    };

                    elementChoices.Add(motionChoiceButton);
                    availableMotionChoices++;
                }

                if (availableMotionChoices == 0)
                {
                    noElementsSearchLabel.text = $"No {PluralElementName.ToLower()} named <b>{searchInput}</b> are found.";
                    
                    noElementsSearchLabel.style.display = DisplayStyle.Flex;
                }
                else
                    noElementsSearchLabel.style.display = DisplayStyle.None;
                
                addElementButton.style.display = availableMotionChoices > 0 ? DisplayStyle.Flex : DisplayStyle.None;

                if(MaintainUniqueTypeElements && availableMotionChoices == 0)
                {
                    isElementChoicesExpanded = false;
                    elementChoices.style.display = DisplayStyle.None;
                }
            }
            void AddElement(Type elementType)
            {
                elementsProperty.arraySize++;

                int newElementIndex = elementsProperty.arraySize - 1;

                if(IsListSerialziedByReference)
                    elementsProperty.GetArrayElementAtIndex(newElementIndex).managedReferenceValue = Activator.CreateInstance(elementType);

                property.serializedObject.ApplyModifiedProperties();
            }
            void RemoveElement(int elementIndex)
            {
                elementsProperty.DeleteArrayElementAtIndex(elementIndex);

                property.serializedObject.ApplyModifiedProperties();

                UpdateElementsList();
            }
            void MoveElementUp(int elementIndex)
            {
                if(elementIndex <= 0)
                    return;

                elementsProperty.InsertArrayElementAtIndex(elementIndex - 1);
                SerializedProperty newElement = elementsProperty.GetArrayElementAtIndex(elementIndex - 1);
                SerializedProperty oldElement = elementsProperty.GetArrayElementAtIndex(elementIndex);

                if (IsListSerialziedByReference)
                    newElement.managedReferenceValue = elementsProperty.GetArrayElementAtIndex(elementIndex + 1).managedReferenceValue;
                else
                    newElement.objectReferenceValue = oldElement.objectReferenceValue;

                elementsProperty.DeleteArrayElementAtIndex(elementIndex + 1);

                elementsProperty.serializedObject.ApplyModifiedProperties();

                UpdateElementsList();
            }
            void MoveElementDown(int elementIndex)
            {
                if(elementIndex >= elementsProperty.arraySize - 1)
                    return;

                elementsProperty.InsertArrayElementAtIndex(elementIndex + 2);
                SerializedProperty newElement = elementsProperty.GetArrayElementAtIndex(elementIndex + 2);
                SerializedProperty oldElement = elementsProperty.GetArrayElementAtIndex(elementIndex);

                if(IsListSerialziedByReference)
                    newElement.managedReferenceValue = elementsProperty.GetArrayElementAtIndex(elementIndex).managedReferenceValue;
                else
                    newElement.objectReferenceValue = oldElement.objectReferenceValue;

                elementsProperty.DeleteArrayElementAtIndex(elementIndex);

                elementsProperty.serializedObject.ApplyModifiedProperties();

                UpdateElementsList();
            }
            void BuildElementsList()
            {
                elementsList.Clear();

                for(int i = 0; i < elementsProperty.arraySize; i++)
                {
                    int elementIndex = i;
                
                    VisualElement propertyContainer = new VisualElement();
                    propertyContainer.style.flexGrow = 1;
                    propertyContainer.style.flexDirection = FlexDirection.Row;

                    #region Remove Button
                    Button elementRemoveButton = new Button()
                    {
                        text = "-",
                        tooltip = $"Remove {SingleElementName.ToLower()}"
                    };

                    elementRemoveButton.style.borderTopLeftRadius = 0f;
                    elementRemoveButton.style.borderTopRightRadius = 5f;
                    elementRemoveButton.style.borderBottomRightRadius = 5f;
                    elementRemoveButton.style.borderBottomLeftRadius = 0f;

                    elementRemoveButton.style.width = 25f;

                    elementRemoveButton.style.marginTop = 4f;
                    elementRemoveButton.style.marginLeft = 2f;
                    elementRemoveButton.style.marginRight = 0f;
                    elementRemoveButton.style.marginBottom = 4f;

                    elementRemoveButton.clicked += () =>
                    {
                        RemoveElement(elementIndex);

                        if(IsListSerialziedByReference)
                            GenerateElementChoicesList();
                    };
                    #endregion

                    #region Move Up Button
                    Button moveUpButton = new Button()
                    {
                        text = "U",
                        tooltip = $"Moves the element up in the list"
                    };

                    moveUpButton.style.borderTopLeftRadius = 0f;
                    moveUpButton.style.borderTopRightRadius = 5f;
                    moveUpButton.style.borderBottomRightRadius = 0f;
                    moveUpButton.style.borderBottomLeftRadius = 0f;

                    moveUpButton.style.width = 25f;

                    moveUpButton.style.marginTop = 4f;
                    moveUpButton.style.marginLeft = 2f;
                    moveUpButton.style.marginRight = 0f;

                    moveUpButton.clicked += () =>
                    {
                        MoveElementUp(elementIndex);
                    };
                    #endregion

                    #region Move Down Button
                    Button moveDownButton = new Button()
                    {
                        text = "D",
                        tooltip = $"Moves the element down in the list"
                    };

                    moveDownButton.style.borderTopLeftRadius = 0f;
                    moveDownButton.style.borderTopRightRadius = 0f;
                    moveDownButton.style.borderBottomRightRadius = 5f;
                    moveDownButton.style.borderBottomLeftRadius = 0f;

                    moveDownButton.style.width = 25f;

                    moveDownButton.style.marginLeft = 2f;
                    moveDownButton.style.marginRight = 0f;
                    moveDownButton.style.marginBottom = 4f;

                    moveDownButton.clicked += () =>
                    {
                        MoveElementDown(elementIndex);
                    };
                    #endregion

                    VisualElement elementVisual = CreateElementContainer(elementsProperty.GetArrayElementAtIndex(i));

                    propertyContainer.Add(elementVisual);

                    propertyContainer.Add(elementRemoveButton);
                    //propertyContainer.Add(moveUpButton);
                    //propertyContainer.Add(moveDownButton);

                    //VisualElement actionButtonsContainer = new VisualElement();
                    //actionButtonsContainer.Add(elementRemoveButton);
                    
                    //if (elementIndex > 0)
                    //    actionButtonsContainer.Add(moveUpButton);
                    //if (elementIndex < elementsProperty.arraySize - 1)
                    //    actionButtonsContainer.Add(moveDownButton);

                    //propertyContainer.Add(actionButtonsContainer);

                    elementsList.Add(propertyContainer);
                }

                curElementsCount.IntValue = elementsProperty.arraySize;

                noElementsInfo.style.display = curElementsCount.IntValue > 0 ? DisplayStyle.None : DisplayStyle.Flex;

            }
            void UpdateElementsList()
            {
                BuildElementsList();

                root.Bind(property.serializedObject);
            }
            #endregion

            return root;
        }

        protected virtual VisualElement CreateElementContainer(SerializedProperty elementProperty)
        {
            VisualElement container = new VisualElement();

            container.style.flexGrow = 1;

            container.style.SetBorderWidth(1f);
            container.style.SetBorderColor(ElementBorderColor);
            container.style.SetBorderRadius(3f);

            container.style.SetPadding(5f);

            container.style.marginTop = 10f;
            container.style.marginBottom = 10f;

            container.style.backgroundColor = new StyleColor(ElementBackgroundColor);

            Label titleLabel = new Label();

            SerializedProperty elementPropertyParent = elementProperty.Parent();
            bool isArrayElement = elementPropertyParent != null && elementPropertyParent.isArray;
            int elementIndexInArray = elementProperty.GetMyIndexInParentArray();

            if(IsListSerialziedByReference)
                titleLabel.text = GetElementPropertyDisplayName(elementProperty);
            else
            {
                if(elementIndexInArray > -1)
                    titleLabel.text = $"{SingleElementName} {elementIndexInArray}";
                else
                    titleLabel.text = $"{SingleElementName} ({elementProperty.displayName})";
            }

            titleLabel.style.paddingBottom = 2f;

            titleLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);

            container.Add(titleLabel);

            bool hasCustomDrawer = false;

            if(IsListSerialziedByReference)
            {
                object managerReferenceValue = elementProperty.managedReferenceValue;

                Type propertyType = null;

                if(managerReferenceValue != null)
                    propertyType = elementProperty.managedReferenceValue.GetType();
                
                hasCustomDrawer = propertyType != null && GetPropertyDrawer(propertyType) != null;
            }

            if(!OverrideElementDrawer && hasCustomDrawer)
            {
                PropertyField normalPF = new PropertyField(elementProperty);
                normalPF.style.flexGrow = 1;

                container.Add(normalPF);
            }
            else
            {
                IEnumerable<SerializedProperty> childProperties = elementProperty.GetVisibleChildren();

                foreach(SerializedProperty cp in childProperties)
                {
                    PropertyField pf = new PropertyField();
                    pf.bindingPath = cp.propertyPath;
                    pf.name = cp.name;
                    container.Add(pf);
                }

                ProcessElementContainer(container, elementProperty);
            }


            return container;
        }
        public Type GetPropertyDrawer(Type classType)
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var scriptAttributeUtility = assembly.CreateInstance("UnityEditor.ScriptAttributeUtility");
            var scriptAttributeUtilityType = scriptAttributeUtility.GetType();


            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            var getDrawerTypeForType = scriptAttributeUtilityType.GetMethod("GetDrawerTypeForType", bindingFlags);

            return (Type)getDrawerTypeForType.Invoke(scriptAttributeUtility, new object[] { classType });
        }
        protected virtual void ProcessElementContainer(VisualElement elementContainer, SerializedProperty elementProperty)
        { 

        }
        public virtual string GetElementPropertyDisplayName(SerializedProperty elementProperty)
        {
            return ObjectNames.NicifyVariableName(elementProperty.GetTypeActualName());
        }
    }
}