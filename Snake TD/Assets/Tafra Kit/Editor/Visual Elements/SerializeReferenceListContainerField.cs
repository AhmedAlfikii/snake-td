using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using TafraKit;
using TafraKitEditor.AI3;
using UnityEditor.Experimental.GraphView;
using System.Threading.Tasks;
using TafraKit.Conditions;
using System.Reflection;

namespace TafraKitEditor
{
    public class SerializeReferenceListContainerField : VisualElement
    {
        private VisualElement content;
        private int maxElementsCount;
        private ControlReceiver addButtonHiders;

        public VisualElement Content => content;

        public SerializeReferenceListContainerField(SerializedProperty property, string listPropertyName, bool forceUniqueElements, string singleElementName = "Element", string pluralElementName = "Elements", string tooltip = "", bool drawElementsAsTheyAre = false, int maxElementsCount = 0)
        {
            this.maxElementsCount = maxElementsCount;
            IntContainer curElementsCount = new IntContainer();
            string viewDataKey = property.propertyPath;
            string isContentExpandedKey = $"{viewDataKey}_IS_CONTENT_EXPANDED";

            bool isContentExpanded = EditorPrefs.GetBool(isContentExpandedKey, false);

            SerializedProperty listProperty = property.FindPropertyRelative(listPropertyName);

            Type elementsType = null;
            Type listType = listProperty.GetActualType();
            
            if (listType == null )
                return;

            if (listType.IsArray)
                elementsType = listType.GetElementType();
            else //If it's a list.
                elementsType = listType.GetGenericArguments()[0];

            Type containerType = property.GetActualType();
            ClassSearchProvider searchProvider = ClassSearchProvider.CreateOrGetInstance();

            curElementsCount.IntValue = listProperty.arraySize;

            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/UI Toolkit Assets/UXML/General/SerializeReferenceListContainer.uxml");
            uxml.CloneTree(this);

            content = this.Q<VisualElement>("content");
            VisualElement elementsContainer = this.Q<VisualElement>("elements-container");
            VisualElement noElementsBox = this.Q<VisualElement>("no-elements-box");
            VisualElement headerSign = this.Q<VisualElement>("header-sign");
            VisualElement additionalPropertiesContainer = this.Q<VisualElement>("additional-properties-container");

            Button addButton = this.Q<Button>("add-button");
            addButton.text = $"Add {singleElementName}";
            addButton.clicked += OnAddButtonClicked;

            addButtonHiders = new ControlReceiver(() => { addButton.style.display = DisplayStyle.None; }, null, () => { addButton.style.display = DisplayStyle.Flex; });

            if(maxElementsCount > 0 && listProperty.arraySize >= maxElementsCount)
                addButtonHiders.AddController("full");

            SetupHeader();

            UpdateHeaderState();

            DrawElements();

            //Just to be able to know if the list elements count was changed duo to an undo (or any other external force) so that we can redraw the list.
            PropertyField elementArrayCountPF = new PropertyField();
            elementArrayCountPF.bindingPath = $"{property.propertyPath}.{listPropertyName}.Array.size";
            elementArrayCountPF.style.display = DisplayStyle.None;
            elementArrayCountPF.RegisterCallback<ChangeEvent<int>>(OnElementsArraySizeValueChanged);
            this.Add(elementArrayCountPF);

            this.RegisterCallback<AttachToPanelEvent>((ev) => {
                Undo.undoRedoPerformed += DrawElements;
            });
            this.RegisterCallback<DetachFromPanelEvent>((ev) => {
                Undo.undoRedoPerformed -= DrawElements;
            });

            this.Bind(property.serializedObject);

            #region Local Functions
            void SetupHeader()
            {
                Button headerButton = this.Q<Button>("header");
                headerButton.tooltip = tooltip;
                headerButton.text = property.displayName;
                headerButton.clicked += OnHeaderButtonClicked;
                headerButton.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent ev) =>
                {
                    TafraEditorUtility.BuildPropertyContextualMenu(property, ev);

                    bool hasPastableElement = false;

                    object clipboardObject = TafraEditorUtility.GetClipboardObject();
                    if(clipboardObject != null && elementsType.IsAssignableFrom(clipboardObject.GetType()))
                        hasPastableElement = true;

                    bool isElementUnique = true;
                    SerializedProperty pasteValueOnObject = null;

                    if(hasPastableElement)
                    {
                        for(int i = 0; i < listProperty.arraySize; i++)
                        {
                            var element = listProperty.GetArrayElementAtIndex(i);

                            if(element.managedReferenceValue.GetType() == clipboardObject.GetType())
                            {
                                pasteValueOnObject = element;
                                isElementUnique = false;
                                break;
                            }
                        }
                    }

                    bool canPasteElement = hasPastableElement && (!forceUniqueElements || isElementUnique);

                    ev.menu.AppendSeparator();

                    ev.menu.AppendAction($"Paste {singleElementName} As New",
                        (a) => { PasteElementAsNew(clipboardObject); },
                        canPasteElement ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                    ev.menu.AppendAction($"Paste {singleElementName} Values",
                        (a) => { PasteElementValues(pasteValueOnObject); },
                        isElementUnique ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

                    ev.menu.AppendSeparator();

                    ev.menu.AppendAction($"Clear List",
                        (a) => { ClearList(); },
                        DropdownMenuAction.Status.Normal);
                }));
            }
            void UpdateHeaderState()
            {
                string headerPlusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Plus.png" : "Tafra Kit/Icons/Tafra_Toolbar Plus.png";
                string headerMinusIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Tafra_Toolbar Minus.png" : "Tafra Kit/Icons/Tafra_Toolbar Minus.png";

                string headerIconPath = isContentExpanded ? headerMinusIconPath : headerPlusIconPath;

                Texture2D icon = EditorGUIUtility.Load(headerIconPath) as Texture2D;
                headerSign.style.backgroundImage = new StyleBackground(icon);

                content.style.display = isContentExpanded ? DisplayStyle.Flex : DisplayStyle.None;

                if(isContentExpanded)
                    addButtonHiders.RemoveController("collapsed");
                else
                    addButtonHiders.AddController("collapsed");
            }
            void UpdateNoElemntsBoxState()
            {
                listProperty.serializedObject.Update();

                noElementsBox.style.display = listProperty.arraySize > 0 ? DisplayStyle.None : DisplayStyle.Flex;
            }
            void DrawElements()
            {
                property.serializedObject.Update();

                elementsContainer.Clear();

                DrawPropertyAdditionalChildren();

                curElementsCount.IntValue = listProperty.arraySize;
                for(int i = 0; i < listProperty.arraySize; i++)
                {
                    int elementIndex = i;
                    var element = listProperty.GetArrayElementAtIndex(elementIndex);
                    object managedReference = element.managedReferenceValue;
                    
                    if(managedReference == null)
                        continue;

                    Type elementType = managedReference.GetType();
                    VisualElement elementBox = CreateElementBox(element, elementIndex);

                    Button header = elementBox.Q<Button>("element-header");
                    header.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent ev) =>
                    {
                        ev.menu.AppendAction("Delete", (a) => { DeleteElement(elementIndex); });

                        ev.menu.AppendAction("Move Up", (a) => { MoveElementUp(elementIndex); },
                            elementIndex == 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

                        ev.menu.AppendAction("Move Down", (a) => { MoveElementDown(elementIndex); },
                            elementIndex == listProperty.arraySize - 1 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

                        ev.menu.AppendAction("Copy", (a) => { CopyElement(elementIndex); });

                        ev.menu.AppendAction("Paste", (a) => { PasteElementValues(element); },
                            TafraEditorUtility.CanPasteOnObject(element.managedReferenceValue) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                        ev.menu.AppendSeparator();

                        ev.menu.AppendAction("Select Script", (a) => {
                            TafraEditorUtility.SelectClassScript(elementType);
                        });
                        ev.menu.AppendAction("Edit Script", (a) => {
                            TafraEditorUtility.OpenClassScript(elementType);
                        });

                        ev.menu.AppendSeparator();

                        TafraEditorUtility.BuildPropertyContextualMenu(element, ev);
                    }));
                    elementsContainer.Add(elementBox);

                }

                UpdateNoElemntsBoxState();

                this.Bind(property.serializedObject);
            }
            void DrawPropertyAdditionalChildren()
            {
                additionalPropertiesContainer.Clear();

                var children = property.GetVisibleChildren();

                bool drewChild = false;
                foreach(var child in children)
                {
                    if(child.name == listPropertyName)
                        continue;

                    PropertyField pf = new PropertyField(child);
                    additionalPropertiesContainer.Add(pf);

                    drewChild = true;
                }

                additionalPropertiesContainer.style.display = drewChild ? DisplayStyle.Flex : DisplayStyle.None;
            }
            void DeleteElement(int elementIndex)
            {
                listProperty.DeleteArrayElementAtIndex(elementIndex);

                property.serializedObject.ApplyModifiedProperties();

                if(maxElementsCount > 0 && listProperty.arraySize < maxElementsCount)
                    addButtonHiders.RemoveController("full");

                DrawElements();
            }
            void MoveElementUp(int elementIndex)
            {
                listProperty.MoveArrayElementUp(elementIndex, true);

                property.serializedObject.ApplyModifiedProperties();

                DrawElements();
            }
            void MoveElementDown(int elementIndex)
            {
                listProperty.MoveArrayElementDown(elementIndex, true);

                property.serializedObject.ApplyModifiedProperties();

                DrawElements();
            }
            void CopyElement(int elementIndex)
            {
                TafraEditorUtility.AddObjectToClipboard(listProperty.GetArrayElementAtIndex(elementIndex).managedReferenceValue);
            }
            void PasteElementValues(SerializedProperty element)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Paste Element");

                TafraEditorUtility.PasteClipboardObjectOnObject(element.managedReferenceValue);

                property.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            void PasteElementAsNew(object objectToPaste)
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Paste Element");

                object newObject = Activator.CreateInstance(objectToPaste.GetType());

                TafraEditorUtility.Clone(objectToPaste, newObject);

                listProperty.arraySize++;

                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).managedReferenceValue = newObject;

                property.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            void ClearList()
            {
                listProperty.arraySize = 0;
                listProperty.serializedObject.ApplyModifiedProperties();
            }
            void OnHeaderButtonClicked()
            {
                isContentExpanded = !isContentExpanded;

                EditorPrefs.SetBool(isContentExpandedKey, isContentExpanded);

                UpdateHeaderState();
            }
            void OnAddButtonClicked()
            {
                listProperty.serializedObject.Update();

                searchProvider.Initialize(elementsType, pluralElementName, OnSearchMenuItemSelected);

                if(forceUniqueElements)
                {
                    for(int i = 0; i < listProperty.arraySize; i++)
                    {
                        var element = listProperty.GetArrayElementAtIndex(i);
                        if(element.managedReferenceValue != null)
                            searchProvider.AddNextShowExcludedType(element.managedReferenceValue.GetType());
                    }
                }

                Vector3 addButtonPos = addButton.worldTransform.GetPosition();
                addButtonPos += new Vector3(addButton.resolvedStyle.width / 2f, addButton.resolvedStyle.height / 2f, 0);
                Vector2 pos = GUIUtility.GUIToScreenPoint(addButtonPos);
                pos.y += 25;

                SearchWindow.Open(new SearchWindowContext(pos), searchProvider);
            }
            void OnSearchMenuItemSelected(Type type, SearchWindowContext context)
            {
                listProperty.arraySize++;

                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).managedReferenceValue = Activator.CreateInstance(type);

                property.serializedObject.ApplyModifiedProperties();

                if(maxElementsCount > 0 && listProperty.arraySize >= maxElementsCount)
                    addButtonHiders.AddController("full");

                UpdateNoElemntsBoxState();
            }
            async void OnElementsArraySizeValueChanged(ChangeEvent<int> evt)
            {
                if(curElementsCount.IntValue == evt.newValue) return;

                elementArrayCountPF.UnregisterCallback<ChangeEvent<int>>(OnElementsArraySizeValueChanged);

                DrawElements();

                await Task.Yield();

                elementArrayCountPF.RegisterCallback<ChangeEvent<int>>(OnElementsArraySizeValueChanged);
            }
            VisualElement CreateElementBox(SerializedProperty elementProperty, int elementIndex)
            {
                string viewDataKey = elementProperty.propertyPath;
                string isContentExpandedKey = $"{viewDataKey}_IS_CONTENT_EXPANDED";

                bool isContentExpanded = EditorPrefs.GetBool(isContentExpandedKey, false);

                Type elementType = elementProperty.managedReferenceValue.GetType();

                VisualElement elementBox = new VisualElement();
                elementBox.name = "element-box";
                elementBox.AddToClassList("elementBox");

                VisualElement elementContent = new VisualElement();
                elementContent.name = "element-content";
                elementContent.AddToClassList("elementContent");
                elementContent.style.display = DisplayStyle.None;

                bool isNestedContainer = elementProperty.managedReferenceValue.GetType() == containerType;

                if(!isNestedContainer)
                {
                    if (drawElementsAsTheyAre)
                    {
                        PropertyField pf = new PropertyField(elementProperty);
                        elementContent.Add(pf);
                    }
                    else
                    {
                        var elementChildren = elementProperty.GetVisibleChildren();
                        foreach (var child in elementChildren)
                        {
                            PropertyField pf = new PropertyField(child);
                            elementContent.Add(pf);
                        }
                    }
                }
                else
                {
                    var nestedPropertyField = new SerializeReferenceListContainerField(elementProperty, listPropertyName, forceUniqueElements, singleElementName, pluralElementName);
                    elementContent.Add(nestedPropertyField);
                }

                Button elementHeader = new Button();
                elementHeader.name = "element-header";
                elementHeader.AddToClassList("elementHeader");
                elementHeader.clicked += OnHeaderButtonClicked;
                elementHeader.style.flexDirection = FlexDirection.Row;
                elementBox.Add(elementHeader);

                string elementNiceName = ObjectNames.NicifyVariableName(elementType.Name);

                if (elementType.GetCustomAttribute<SearchMenuItem>() is SearchMenuItem searchMenuItem)
                    elementNiceName = searchMenuItem.MenuName.Substring(searchMenuItem.MenuName.LastIndexOf("/") + 1);

                Label elementName = new Label(elementNiceName);
                elementName.name = "element-name";
                elementName.style.flexGrow = 1;
                elementHeader.Add(elementName);

                Label elementIndexLabel = new Label(elementIndex.ToString());
                elementIndexLabel.name = "element-index";
                elementIndexLabel.style.color = new Color(1, 1, 1, 0.5f);
                elementIndexLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
                elementIndexLabel.style.marginRight = 5f;
                elementHeader.Add(elementIndexLabel);

                elementBox.Add(elementContent);

                UpdateHeaderState();

                void UpdateHeaderState()
                {
                    elementContent.style.display = isContentExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                }
                void OnHeaderButtonClicked()
                {
                    isContentExpanded = !isContentExpanded;

                    EditorPrefs.SetBool(isContentExpandedKey, isContentExpanded);

                    UpdateHeaderState();
                }

                return elementBox;
            }
            #endregion
        }
    }
}