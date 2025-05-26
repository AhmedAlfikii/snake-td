using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using TafraKit.GraphViews;
using TafraKit.AI3;
using System;
using TafraKit;
using TafraKit.Internal.GraphViews;

namespace TafraKitEditor
{
    public class BlackboardPropertyReferenceField<T> : VisualElement
    {
        private SerializedProperty property;
        private SerializedProperty targetBBProperty;
        private SerializedProperty tooltipProperty;
        private SerializedProperty bbPropertyNameProperty;
        private SerializedProperty defaultValueProperty;
        private SerializedProperty enableDefaultProperty;
        private SerializedProperty internalBBIDProperty;

        private IGraphBlackboardContainer blackboardContainer;
        private bool hasDefaultValue;
        private PropertyBox propertyBox;
        private PropertyField targetBBField;
        private List<ExposableProperty> internalBlackboardProperties = new List<ExposableProperty>();
        private GenericMenu propertiesMenu = new GenericMenu();

        public PropertyBox PropertyBox => propertyBox;

        public BlackboardPropertyReferenceField(SerializedProperty property, bool hasDefaultValue)
        {
            this.property = property;
            this.hasDefaultValue = hasDefaultValue;

            if(property.serializedObject.targetObject is IGraphBlackboardContainer container)
                blackboardContainer = container;

            targetBBProperty = property.FindPropertyRelative("targetBlackboard");
            internalBBIDProperty = property.FindPropertyRelative("internalBlackboardPropertyID");
            bbPropertyNameProperty = property.FindPropertyRelative("blackboardPropertyName");
            defaultValueProperty = hasDefaultValue ? property.FindPropertyRelative("defaultValue") : null;
            enableDefaultProperty = hasDefaultValue ? property.FindPropertyRelative("enableDefault") : null;

            propertyBox = new PropertyBox(property, property.displayName, ZHelper.GetNiceTypeName(typeof(T)), property.tooltip, true);
            Add(propertyBox);

            targetBBField = new PropertyField(targetBBProperty, "");
            targetBBField.name = "target-blackboard";
            propertyBox.Header.Add(targetBBField);

            this.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachedToPanel);

            #region Properties Dropdown Setup
            ExposableProperty alreadySelectedProp = null;

            if(blackboardContainer != null)
            {
                List<GenericExposableProperty<T>> properties = blackboardContainer.Blackboard.GetAllPropertiesOfGenericType<T>();
                for(int i = 0; i < properties.Count; i++)
                {
                    var prop = properties[i];
                    internalBlackboardProperties.Add(prop);

                    if(prop.ID == internalBBIDProperty.intValue)
                        alreadySelectedProp = prop;
                }
            }

            for(int i = 0; i < internalBlackboardProperties.Count; i++)
            {
                var prop = internalBlackboardProperties[i];
                propertiesMenu.AddItem(new GUIContent(prop.name), false, () => { SelectProperty(prop); });
            }

            if (alreadySelectedProp != null)
                bbPropertyNameProperty.stringValue = alreadySelectedProp.name;
            else
                internalBBIDProperty.intValue = -1;

            property.serializedObject.ApplyModifiedProperties();
            #endregion

            DrawContent();
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            targetBBField.RegisterCallback<SerializedPropertyChangeEvent>(TargetBlackboardChanged);
        }
        private void OnDetachedToPanel(DetachFromPanelEvent evt)
        {
            targetBBField.UnregisterCallback<SerializedPropertyChangeEvent>(TargetBlackboardChanged);
        }

        private void TargetBlackboardChanged(SerializedPropertyChangeEvent evt)
        {
            targetBBField.UnregisterCallback<SerializedPropertyChangeEvent>(TargetBlackboardChanged);

            DrawContent();

            targetBBField.schedule.Execute(() =>
            {
                targetBBField.RegisterCallback<SerializedPropertyChangeEvent>(TargetBlackboardChanged);
            });
        }

        private void DrawContent()
        {
            propertyBox.Content.Clear();
            
            TargetBlackboard selectedTargetBlackboard = (TargetBlackboard)targetBBProperty.enumValueIndex;

            if(selectedTargetBlackboard == TargetBlackboard.Internal || selectedTargetBlackboard == TargetBlackboard.InternalOrExternal)
                DrawInternalBBNameField();
            else if(selectedTargetBlackboard == TargetBlackboard.External || selectedTargetBlackboard == TargetBlackboard.Secondary)
            {
                PropertyField bbPropertyNameField = new PropertyField(bbPropertyNameProperty, "Property Name");
                propertyBox.Content.Add(bbPropertyNameField);
            }

            if(hasDefaultValue)
            {
                if(selectedTargetBlackboard == TargetBlackboard.None)
                {
                    PropertyField defaultValueField = new PropertyField(defaultValueProperty, "Value");
                    propertyBox.Content.Add(defaultValueField);
                }
                else
                {
                    VisualElement defaultRow = new VisualElement();
                    defaultRow.name = "default-row";
                    defaultRow.style.flexDirection = FlexDirection.Row;
                    propertyBox.Content.Add(defaultRow);

                    VisualElement labelHalf = new VisualElement();
                    labelHalf.name = "label-half";
                    labelHalf.style.flexDirection = FlexDirection.Row;
                    labelHalf.style.alignSelf = Align.Center;
                    labelHalf.style.alignItems = Align.Center;
                    labelHalf.style.minWidth = 120;
                    labelHalf.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
                    defaultRow.Add(labelHalf);

                    VisualElement valueHalf = new VisualElement();
                    valueHalf.name = "value-half";
                    valueHalf.style.flexDirection = FlexDirection.Row;
                    valueHalf.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
                    defaultRow.Add(valueHalf);

                    Toggle defaultValueToggle = new Toggle();
                    defaultValueToggle.style.marginTop = 2;
                    defaultValueToggle.style.marginRight = 2;
                    defaultValueToggle.style.marginBottom = 2;
                    defaultValueToggle.BindProperty(enableDefaultProperty);
                    labelHalf.Add(defaultValueToggle);

                    if(defaultValueProperty != null)
                    {
                        Label defaultValueLabel = new Label("Fallback Value");
                        defaultValueLabel.tooltip = defaultValueProperty.tooltip;
                        defaultValueLabel.style.marginLeft = 0;
                        defaultValueLabel.style.paddingLeft = 0;
                        defaultValueLabel.SetEnabled(enableDefaultProperty.boolValue);
                        labelHalf.Add(defaultValueLabel);

                        PropertyField defaultValueField = new PropertyField(defaultValueProperty, "");
                        defaultValueField.style.flexGrow = 1;
                        defaultValueField.SetEnabled(enableDefaultProperty.boolValue);
                        bool showDefaultValueBelow = defaultValueProperty.GetFieldInfo().FieldType == typeof(TafraAdvancedFloat);

                        if(!showDefaultValueBelow)
                        {
                            defaultValueField.style.display = enableDefaultProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                            valueHalf.Add(defaultValueField);
                        }
                        else
                        {
                            defaultValueField.style.display = enableDefaultProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                            propertyBox.Content.Add(defaultValueField);
                        }

                        defaultValueToggle.RegisterValueChangedCallback((ev) =>
                        {
                            bool defaultEnabled = enableDefaultProperty.boolValue;
                            if(!showDefaultValueBelow)
                                defaultValueField.style.display = defaultEnabled ? DisplayStyle.Flex : DisplayStyle.None;
                            else
                                defaultValueField.style.display = defaultEnabled ? DisplayStyle.Flex : DisplayStyle.None;

                            defaultValueLabel.SetEnabled(defaultEnabled);
                            defaultValueField.SetEnabled(defaultEnabled);
                        });
                    }
                }
            }

            //Uncomment this if you want to debug the referenced property ID.
            PropertyField idField = new PropertyField(internalBBIDProperty);
            propertyBox.Content.Add(idField);
            idField.style.display = DisplayStyle.None;

            this.Bind(property.serializedObject);
        }

        void DrawInternalBBNameField()
        {
            //Validate that the assigned property name has the same ID as the property with the same name in the internal blackboard.
            string assignedPropertyName = bbPropertyNameProperty.stringValue;
            int assignedPropertyID = internalBBIDProperty.intValue;
            for (int i = 0; i < internalBlackboardProperties.Count; i++)
            {
                var prop = internalBlackboardProperties[i];
                if (prop.name == assignedPropertyName && internalBBIDProperty.intValue != prop.ID)
                {
                    internalBBIDProperty.intValue = prop.ID;

                    property.serializedObject.ApplyModifiedProperties();
                }
                else if (prop.ID == assignedPropertyID && prop.name != assignedPropertyName)
                {
                    bbPropertyNameProperty.stringValue = prop.name;
                 
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            VisualElement propertyRow = new VisualElement();
            propertyRow.style.flexDirection = FlexDirection.Row;
            propertyRow.style.marginRight = 20;

            TextField nameChangeField = new TextField("Property Name");
            nameChangeField.BindProperty(bbPropertyNameProperty);
            nameChangeField.style.marginRight = 0;
            nameChangeField.style.flexGrow = 1;
            nameChangeField.style.alignSelf = Align.FlexStart;
            nameChangeField.isDelayed = true;
            nameChangeField.RegisterValueChangedCallback((ev) =>
            {
                ValidateName();
            });
            propertyRow.Add(nameChangeField);

            Button dropDownButton = new Button();
            dropDownButton.style.marginLeft = 0;
            dropDownButton.style.borderBottomLeftRadius = 0;
            dropDownButton.style.borderTopLeftRadius = 0;
            dropDownButton.style.borderTopRightRadius = 5;
            dropDownButton.style.borderBottomRightRadius = 5;
            dropDownButton.style.SetPadding(5);
            dropDownButton.style.width = 22;
            dropDownButton.clicked += () =>
            {
                propertiesMenu.ShowAsContext();
            };
            propertyRow.Add(dropDownButton);

            string arrowIconPath = EditorGUIUtility.isProSkin ? "Tafra Kit/Icons/d_Down_Arrow.png" : "Tafra Kit/Icons/Down_Arrow.png";
            VisualElement downArrow = new VisualElement();
            downArrow.style.flexGrow = 1;
            downArrow.style.backgroundImage = EditorGUIUtility.Load(arrowIconPath) as Texture2D;
            dropDownButton.Add(downArrow);

            propertyBox.Content.Add(propertyRow);
        }

        private void SelectProperty(ExposableProperty exposableProperty)
        {
            internalBBIDProperty.intValue = exposableProperty.ID;
            bbPropertyNameProperty.stringValue = exposableProperty.name;

            property.serializedObject.ApplyModifiedProperties();
        }
        private void ValidateName()
        {
            //Validate that the assigned property name has the same ID as the property with the same name in the internal blackboard.
            string assignedPropertyName = bbPropertyNameProperty.stringValue;
            for(int i = 0; i < internalBlackboardProperties.Count; i++)
            {
                var prop = internalBlackboardProperties[i];
                if(prop.name == assignedPropertyName && internalBBIDProperty.intValue != prop.ID)
                {
                    internalBBIDProperty.intValue = prop.ID;
                    property.serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            //If no property with the given name was found, then reset the ID.
            internalBBIDProperty.intValue = -1;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}